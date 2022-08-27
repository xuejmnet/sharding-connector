using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using ShardingConnector.Extensions;
using ShardingConnector.Logger;
using ShardingConnector.Protocol.Packets;
using ShardingConnector.ProxyClient.Abstractions;
using ShardingConnector.ProxyServer;
using ShardingConnector.ProxyServer.Session;

namespace ShardingConnector.ProxyClient.Command;

public sealed class ClientCommandExecutor : IClientCommandExecutor
{
    private static readonly ILogger<ClientCommandExecutor> _logger = InternalLoggerFactory.CreateLogger<ClientCommandExecutor>();
    private readonly IDatabaseProtocolClientEngine _databaseProtocolClientEngine;
    private readonly ConnectionSession _connectionSession;
    private readonly IChannelHandlerContext _context;
    private readonly IByteBuffer _message;
    private readonly bool _logDebug;

    public ClientCommandExecutor(IDatabaseProtocolClientEngine databaseProtocolClientEngine,
        ConnectionSession connectionSession, IChannelHandlerContext context, IByteBuffer message)
    {
        _databaseProtocolClientEngine = databaseProtocolClientEngine;
        _connectionSession = connectionSession;
        _context = context;
        _message = message;
        _logDebug = _logger.IsEnabled(LogLevel.Debug);
    }

    public  ValueTask ExecuteAsync()
    {
        bool isNeedFlush = false;
        bool sqlShow = true;
        using (var payload = _databaseProtocolClientEngine.GetPacketCodec().CreatePacketPayload(_message,
                   _context.Channel.GetAttribute(CommonConstants.CHARSET_ATTRIBUTE_KEY).Get()))
        {
            try
            {
                isNeedFlush =  ExecuteCommand(_context, payload);
            }
            catch (Exception exception)
            {
                 ProcessException(exception);
            }
            finally
            {
                if (isNeedFlush)
                {
                    _context.Flush();
                }
            }
        }
        return ValueTask.CompletedTask;
    }

    private void ProcessException(Exception exception)
    {
        // if (!ExpectedExceptions.isExpected(cause.getClass())) {
        //     log.error("Exception occur: ", cause);
        // }
        _logger.LogError($"Exception occur: {exception}");
        var clientDbConnection = _databaseProtocolClientEngine.GetClientDbConnection();
         _context.WriteAsync(clientDbConnection.GetErrorPacket(exception));
        var databasePacket = clientDbConnection.GetOtherPacket(_connectionSession);
        if (null != databasePacket)
        {
             _context.WriteAsync(databasePacket);
        }

        _context.Flush();
    }

    private bool ExecuteCommand(IChannelHandlerContext context, IPacketPayload payload)
    {
        var clientDbConnection = _databaseProtocolClientEngine.GetClientDbConnection();
        using (var clientCommand =
               clientDbConnection.CreateCommand(payload, _connectionSession))
        {
            try
            {
                using (var clientDataReader = clientCommand.ExecuteReader())
                {
                    var responsePackets = clientDataReader.SendCommand();
                    // var enumerable = responsePackets as IPacket[] ?? responsePackets.ToArray();
                    // if (enumerable.IsEmpty())
                    // {
                    //     return false;
                    // }

                    int i = 0;
                    foreach (var responsePacket in responsePackets)
                    {
                        context.WriteAsync(responsePacket);
                        i++;
                    }

                    if (i == 0)
                    {
                        return false;
                    }

                    if (clientDataReader is IClientQueryDataReader clientQueryDataReader)
                    {
                        clientDbConnection.WriteQueryData(context, _connectionSession,clientQueryDataReader, i);
                    }
                    return true;
                }
                
            }
            catch (Exception e)
            {
                _logger.LogError("client command execute error.",e);
                _databaseProtocolClientEngine.HandleException(_connectionSession, e);
                throw;
            }
        }
    }
}