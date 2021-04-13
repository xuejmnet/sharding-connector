using System;
using ShardingConnector.Kernels.Route.Context;
using ShardingConnector.Rewrite.Context;
using ShardingConnector.Rewrite.Sql.Token.SimpleObject;

namespace ShardingConnector.Rewrite.Sql.Impl
{
/*
* @Author: xjm
* @Description:
* @Date: Tuesday, 13 April 2021 21:03:09
* @Email: 326308290@qq.com
*/
    public class RouteSqlBuilder:AbstractSqlBuilder
    {
        private readonly RouteUnit _routeUnit;
        public RouteSqlBuilder(SqlRewriteContext context, RouteUnit routeUnit) : base(context)
        {
            _routeUnit = routeUnit;
        }

        protected override string GetSqlTokenText(SqlToken sqlToken)
        {
            if (sqlToken is IRouteUnitAware routeUnitAware) {
                return routeUnitAware.ToString(_routeUnit);
            }
            return sqlToken.ToString();
        }
    }
}