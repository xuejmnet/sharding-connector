using System;
using System.Collections.Generic;
using ShardingConnector.Parser.Sql.Constant;
using ShardingConnector.Parser.Sql.Util;

namespace ShardingConnector.Parser.Binder.Segment.Select.Projection.Impl
{
/*
* @Author: xjm
* @Description:
* @Date: Sunday, 11 April 2021 16:19:00
* @Email: 326308290@qq.com
*/
    public class AggregationProjection:IProjection
    {
        private readonly AggregationTypeEnum _type;
    
        private readonly string _innerExpression;
    
        private readonly string _alias;
    
        private readonly List<AggregationProjection> _derivedAggregationProjections = new List<AggregationProjection>(2);
    
        private int index = -1;

        public AggregationProjection(AggregationTypeEnum type, string innerExpression, string @alias)
        {
            _type = type;
            _innerExpression = innerExpression;
            _alias = alias;
        }

        public string GetExpression()
        {
            return SqlUtil.GetExactlyValue(_type.ToString() + _innerExpression);
        }

        public string GetAlias()
        {
            return _alias;
        }
        /// <summary>
        /// 别称或者是真实的表达式
        /// </summary>
        /// <returns></returns>
        public string GetColumnLabel()
        {
            return _alias ?? GetExpression();
        }

        public void SetIndex(int index)
        {
            this.index = index;
        }

        public List<AggregationProjection> GetDerivedAggregationProjections()
        {
            return _derivedAggregationProjections;
        }

        public string GetInnerExpression()
        {
            return _innerExpression;
        }
    }
}