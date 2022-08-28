using ShardingConnector.ProxyServer.Options.Context;
using ShardingConnector.ProxyServer.StreamMerges.ExecutePrepares.Merge;
using ShardingConnector.ProxyServer.StreamMerges.Executors.Context;
using ShardingConnector.ProxyServer.StreamMerges.Results;

namespace ShardingConnector.ProxyServer.StreamMerges;

public sealed class ServerExecuteResultShardingMerger:IShardingMerger<IExecuteResult>
{
    private ServerExecuteResultShardingMerger(){}
    public static ServerExecuteResultShardingMerger Instance = new ServerExecuteResultShardingMerger();
    public IExecuteResult StreamMerge(ShardingExecutionContext shardingExecutionContext, List<IExecuteResult> parallelResults)
    {
        var parallelResult = parallelResults[0];
        if (parallelResult is QueryExecuteResult queryExecuteResult)
        {
            var dbColumns = queryExecuteResult.DbColumns;
            ShardingRuntimeContext runtimeContext = ProxyContext.ShardingRuntimeContext;
            MergeEngine mergeEngine = new MergeEngine(runtimeContext.GetRule().ToRules(),
                runtimeContext.GetProperties(), runtimeContext.GetDatabaseType(), runtimeContext.GetMetaData().Schema);
            var streamDataReader = mergeEngine.Merge(parallelResults.Select(o=>((QueryExecuteResult)o).StreamDataReader).ToList(), shardingExecutionContext.GetSqlCommandContext());
            return new QueryExecuteResult(dbColumns, streamDataReader);
        }
        else
        {
            int recordsAffected = 0;
            long lastInsertId=0L;
            foreach (var r in parallelResults)
            {
                var affectedRowsExecuteResult = (AffectedRowsExecuteResult)r;
                recordsAffected += affectedRowsExecuteResult.RecordsAffected;
                lastInsertId = Math.Max(lastInsertId, affectedRowsExecuteResult.LastInsertId);
            }

            return new AffectedRowsExecuteResult(recordsAffected, lastInsertId);
        }
    }

    public void InMemoryMerge(ShardingExecutionContext shardingExecutionContext, List<IExecuteResult> beforeInMemoryResults, List<IExecuteResult> parallelResults)
    {
        beforeInMemoryResults.AddRange(parallelResults);
    }
}