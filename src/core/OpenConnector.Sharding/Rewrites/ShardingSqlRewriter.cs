using OpenConnector.CommandParserBinder.MetaData;
using OpenConnector.Sharding.Rewrites.Abstractions;
using OpenConnector.Sharding.Routes;
using OpenConnector.Sharding.Routes.Abstractions;

namespace OpenConnector.Sharding.Rewrites;

public sealed class ShardingSqlRewriter:IShardingSqlRewriter
{
    private readonly ITableMetadataManager _tableMetadataManager;
    private readonly IParameterRewriterBuilder _parameterRewriterBuilder;

    public ShardingSqlRewriter(ITableMetadataManager tableMetadataManager,IParameterRewriterBuilder parameterRewriterBuilder)
    {
        _tableMetadataManager = tableMetadataManager;
        _parameterRewriterBuilder = parameterRewriterBuilder;
    }
    public SqlRewriteContext Rewrite(SqlParserResult sqlParserResult, RouteContext routeContext)
    {
        var sqlRewriteContext = new SqlRewriteContext(_tableMetadataManager,routeContext.GetSqlCommandContext(),routeContext.GetSql(),routeContext.GetParameterContext());
        var parameterRewriters = _parameterRewriterBuilder.GetParameterRewriters(routeContext);
        foreach (var parameterRewriter in parameterRewriters)
        {
            if (!sqlRewriteContext.GetParameterContext().IsEmpty() && parameterRewriter.IsNeedRewrite(sqlRewriteContext.GetSqlCommandContext()))
            {
                parameterRewriter.Rewrite(sqlRewriteContext.GetParameterBuilder(), sqlRewriteContext.GetSqlCommandContext(), sqlRewriteContext.GetParameterContext());
            }
        }
        sqlRewriteContext.AddSqlTokenGenerators(new ShardingTokenGenerateBuilder(rule, routeContext).GetSqlTokenGenerators());
    }
}