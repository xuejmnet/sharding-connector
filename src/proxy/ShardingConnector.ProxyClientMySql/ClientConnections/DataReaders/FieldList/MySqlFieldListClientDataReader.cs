using System.Text;
using ShardingConnector.Protocol.MySql.Constant;
using ShardingConnector.Protocol.MySql.Packet.Command;
using ShardingConnector.Protocol.MySql.Packet.Generic;
using ShardingConnector.Protocol.MySql.Payload;
using ShardingConnector.Protocol.Packets;
using ShardingConnector.ProxyClient.Abstractions;
using ShardingConnector.ProxyClientMySql.Common;
using ShardingConnector.ProxyServer.Abstractions;
using ShardingConnector.ProxyServer.Session;

namespace ShardingConnector.ProxyClientMySql.ClientConnections.DataReaders.FieldList;

public sealed class MySqlFieldListClientDataReader:IClientDataReader<MySqlPacketPayload>
{
    private const string SQL = "SHOW COLUMNS FROM {0} FROM {1}";
    private readonly string _table;
    private readonly string _filedWildcard;
    private readonly ConnectionSession _connectionSession;
    private readonly string _database;
    private readonly IServerDataReader _serverDbDataReader;
    private readonly int _dbEncoding;

    public MySqlFieldListClientDataReader(string table,string filedWildcard,ConnectionSession connectionSession,IServerDataReaderFactory serverDataReaderFactory)
    {
        _table = table;
        _filedWildcard = filedWildcard;
        _connectionSession = connectionSession;
        _dbEncoding=connectionSession.AttributeMap.GetAttribute(MySqlConstants.MYSQL_CHARACTER_SET_ATTRIBUTE_KEY).Get().DbEncoding;
        var sql = string.Format(SQL,_table,_connectionSession.DatabaseName);
        _serverDbDataReader = serverDataReaderFactory.Create(sql,_connectionSession);
    }
    public IEnumerable<IPacket<MySqlPacketPayload>> SendCommand()
    {
        _serverDbDataReader.ExecuteDbDataReader();
        return GetColumnDefinition41Packet();

    }

    private IEnumerable<IPacket<MySqlPacketPayload>> GetColumnDefinition41Packet()
    {
        var result = new LinkedList<IPacket<MySqlPacketPayload>>();
        int currentSequenceId = 0;
        while (_serverDbDataReader.Read())
        {
            var columnName = _serverDbDataReader.GetRowData().Cells[0].ToString();
            result.AddLast(new MySqlColumnDefinition41Packet(++currentSequenceId,_dbEncoding,_database,_table,_table,columnName??string.Empty,columnName??string.Empty,100,(int)MySqlColumnTypeEnum.MYSQL_TYPE_VARCHAR,0,true));
        }

        result.AddLast(new MySqlOkPacket(++currentSequenceId,
            ServerStatusFlagCalculator.CalculateFor(_connectionSession)));
        return result;
    }

    public void Dispose()
    {
        _serverDbDataReader?.Dispose();
    }
}