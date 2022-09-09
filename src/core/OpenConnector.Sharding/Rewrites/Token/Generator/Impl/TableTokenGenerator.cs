﻿using OpenConnector.CommandParser.Abstractions;
using OpenConnector.CommandParserBinder.Command;
using OpenConnector.CommandParserBinder.MetaData;
using OpenConnector.Sharding.Rewrites.Sql.Token.Generator;
using OpenConnector.Sharding.Rewrites.Sql.Token.SimpleObject;
using OpenConnector.Sharding.Rewrites.Token.SimpleObject;

namespace OpenConnector.Sharding.Rewrites.Token.Generator.Impl
{
    /*
    * @Author: xjm
    * @Description:
    * @Date: 2021/4/27 8:56:49
    * @Ver: 1.0
    * @Email: 326308290@qq.com
    */
    public sealed class TableTokenGenerator:ICollectionSqlTokenGenerator<ISqlCommandContext<ISqlCommand>>
    {
        private readonly ITableMetadataManager _tableMetadataManager;

        public TableTokenGenerator(ITableMetadataManager tableMetadataManager)
        {
            _tableMetadataManager = tableMetadataManager;
        }
        public ICollection<SqlToken> GenerateSqlTokens(ISqlCommandContext<ISqlCommand> sqlCommandContext)
        {
            if (sqlCommandContext is ITableAvailable tableAvailable)
            {
                return GenerateSqlTokens(tableAvailable);
            }

            return new List<SqlToken>(0);
        }

        public bool IsGenerateSqlToken(ISqlCommandContext<ISqlCommand> sqlCommandContext)
        {
            return true;
        }



        private ICollection<SqlToken> GenerateSqlTokens(ITableAvailable sqlStatementContext)
        {
            ICollection<SqlToken> result = new LinkedList<SqlToken>();
            foreach (var simpleTableSegment in sqlStatementContext.GetAllTables())
            {
                if (_tableMetadataManager.FindTableRule(simpleTableSegment.GetTableName().GetIdentifier().GetValue())!=null)
                {
                    result.Add(new TableToken(simpleTableSegment.GetStartIndex(), simpleTableSegment.GetStopIndex(), simpleTableSegment.GetTableName().GetIdentifier(), (ISqlCommandContext<ISqlCommand>)sqlStatementContext, shardingRule));
                }
            }
            return result;
        }
    }
}
