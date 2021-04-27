﻿using System;
using System.Collections.Generic;
using System.Text;
using ShardingConnector.Common.Config.Properties;
using ShardingConnector.Extensions;
using ShardingConnector.RewriteEngine.Context;
using ShardingConnector.RewriteEngine.Sql.Token.Generator.Aware;
using ShardingConnector.Route.Context;
using ShardingConnector.ShardingCommon.Core.Rule;
using ShardingConnector.ShardingRewrite.Parameter;

namespace ShardingConnector.ShardingRewrite.Context
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/26 14:36:06
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class ShardingSqlRewriteContextDecorator:ISqlRewriteContextDecorator<ShardingRule>,IRouteContextAware
    {
        private RouteContext routeContext;
        public void Decorate(ShardingRule shardingRule, ConfigurationProperties properties, SqlRewriteContext sqlRewriteContext)
        {
            var parameterRewriters = new ShardingParameterRewriterBuilder(shardingRule, routeContext).GetParameterRewriters(sqlRewriteContext.GetSchemaMetaData());
            foreach (var parameterRewriter in parameterRewriters)
            {
                if (!sqlRewriteContext.GetParameters().IsEmpty() && parameterRewriter.IsNeedRewrite(sqlRewriteContext.GetSqlCommandContext()))
                {
                    parameterRewriter.Rewrite(sqlRewriteContext.GetParameterBuilder(), sqlRewriteContext.GetSqlCommandContext(), sqlRewriteContext.GetParameters());
                }
            }
            sqlRewriteContext.AddSqlTokenGenerators(new ShardingTokenGenerateBuilder(shardingRule, routeContext).getSQLTokenGenerators());

        }

        public int GetOrder()
        {
            return 0;
        }

        public void SetRouteContext(RouteContext routeContext)
        {
            this.routeContext = routeContext;
        }
    }
}
