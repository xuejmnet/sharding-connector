﻿using System;
using System.Collections.Generic;
using System.Text;
using ShardingConnector.Kernels.Route;
using ShardingConnector.Kernels.Route.Rule;

namespace ShardingConnector.Kernels.Parse
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/6 14:38:14
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public class SimpleQueryPrepareEngine: BasePrepareEngine
    {
        public SimpleQueryPrepareEngine(DataNodeRouter router, ICollection<IBaseRule> rules) : base(router, rules)
        {
        }

        protected override IList<object> CloneParameters(IList<object> parameters)
        {
            return new List<object>();
        }

        protected override RouteContext Route(DataNodeRouter router, string sql, IList<object> parameters)
        {
            return router.
        }
    }
}
