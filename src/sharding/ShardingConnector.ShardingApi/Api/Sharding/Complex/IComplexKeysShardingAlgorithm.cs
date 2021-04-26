﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ShardingConnector.ShardingApi.Api.Sharding.Complex
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/26 11:32:14
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public interface IComplexKeysShardingAlgorithm<T> : IShardingAlgorithm where T : IComparable
    {
        ICollection<string> DoSharding(ICollection<string> availableTargetNames, ComplexKeysShardingValue<T> shardingValue);
    }
}
