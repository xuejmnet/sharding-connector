using System.Data.Common;
using OpenConnector.Configuration.Session;
using OpenConnector.Extensions;
using OpenConnector.ProxyServer.Abstractions;
using OpenConnector.ProxyServer.Options.Context;
using OpenConnector.ProxyServer.Session;
using OpenConnector.ProxyServer.StreamMerges.Executors.Context;
using OpenConnector.ShardingAdoNet;
using BasePrepareEngine = OpenConnector.ProxyServer.StreamMerges.ExecutePrepares.Prepare.BasePrepareEngine;
using SimpleQueryPrepareEngine = OpenConnector.ProxyServer.StreamMerges.ExecutePrepares.Prepare.SimpleQueryPrepareEngine;

namespace OpenConnector.ProxyServer.ServerDataReaders;

public sealed class ServerDataReaderFactory:IServerDataReaderFactory
{
    public IServerDataReader Create(string sql, ConnectionSession connectionSession)
    {
        var shardingExecutionContext = DoShardingRoute(sql);
        if (shardingExecutionContext.GetExecutionUnits().IsEmpty())
        {
            return EmptyServerDataReader.Instance;
        }
        return new QueryServerDataReader(shardingExecutionContext, connectionSession);
    }
    private ShardingExecutionContext DoShardingRoute(string sql)
    {

        ShardingRuntimeContext runtimeContext = ProxyContext.ShardingRuntimeContext;
            
        BasePrepareEngine prepareEngine = new SimpleQueryPrepareEngine(
            runtimeContext.GetRule().ToRules(), runtimeContext.GetProperties(), runtimeContext.GetMetaData(),
            runtimeContext.GetSqlParserEngine());
        var parameterContext =
            new ParameterContext(Array.Empty<DbParameter>());
            
        ShardingExecutionContext result = prepareEngine.Prepare(sql, parameterContext);
        //TODO
        // _commandExecutor.Init(result);
        // //_commandExecutor.Commands.for
        // _commandExecutor.Commands.ForEach(ReplyTargetMethodInvoke);
        return result;
    }
}