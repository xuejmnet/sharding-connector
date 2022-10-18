using Microsoft.Extensions.Logging;
using NCDC.CommandParser.Common.Command;
using NCDC.CommandParser.Common.Command.DAL;
using NCDC.CommandParser.Common.Command.DCL;
using NCDC.CommandParser.Common.Command.TCL;
using NCDC.CommandParser.Common.Util;
using NCDC.CommandParser.Dialect.Command.MySql.DAL;
using NCDC.Enums;
using NCDC.Logger;
using NCDC.ProxyServer.Abstractions;
using NCDC.ProxyServer.Connection;
using NCDC.ProxyServer.Connection.Abstractions;
using NCDC.ProxyServer.Helpers;
using NCDC.ShardingAdoNet;
using NCDC.ShardingParser.Abstractions;

namespace NCDC.ProxyServer.ServerHandlers;

public sealed class ServerHandlerFactory : IServerHandlerFactory
{
    private static readonly ILogger<ServerHandlerFactory> _logger =
        NCDCLoggerFactory.CreateLogger<ServerHandlerFactory>();

    private readonly IServerDataReaderFactory _serverDataReaderFactory;

    public ServerHandlerFactory(IServerDataReaderFactory serverDataReaderFactory)
    {
        _serverDataReaderFactory = serverDataReaderFactory;
    }

    public IServerHandler CreateAsync(DatabaseTypeEnum databaseType, string sql, ISqlCommand sqlCommand,
        IConnectionSession connectionSession)
    {
        _logger.LogDebug($"database type:{databaseType},sql:{sql},sql command:{sqlCommand}");
        //取消sql的注释信息
        var trimCommentSql = SqlUtil.TrimComment(sql);
        if (string.IsNullOrEmpty(trimCommentSql))
        {
            return SkipServerHandler.Default;
        }

        CheckNotSupportCommand(sqlCommand);
        // var sqlCommandContext = _sqlCommandContextFactory.Create(sql, ParameterContext.Empty, sqlCommand);
        // connectionSession.QueryContext = new QueryContext(sqlCommandContext, sql, ParameterContext.Empty);
        // await HandleAutoCommitAsync(sqlCommand, connectionSession);
        if (sqlCommand is ITCLCommand tclCommand)
        {
            return CreateTCLCommandServerHandler(tclCommand,sql,connectionSession);
        }

        if (sqlCommand is IDALCommand dalCommand)
        {
            return CreateDALCommandServerHandler(dalCommand, sql, connectionSession);
        }


        return new QueryServerHandler(sql, sqlCommand, connectionSession, _serverDataReaderFactory);
    }

    private static async ValueTask HandleAutoCommitAsync(ISqlCommand sqlCommand, IConnectionSession connectionSession)
    {
        if (AutoCommitHelper.NeedOpenTransaction(sqlCommand))
        {
            await connectionSession.HandleAutoCommitAsync();
        }
    }

    private IServerHandler CreateDALCommandServerHandler(IDALCommand dalCommand, string sql,
        IConnectionSession connectionSession)
    {
        if (dalCommand is MySqlUseCommand useCommand)
        {
            return new UseDatabaseServerHandler(useCommand, connectionSession);
        }

        if (dalCommand is MySqlShowDatabasesCommand)
        {
            return new ShowDatabasesServerHandler(connectionSession);
        }

        if (dalCommand is SetCommand && null == connectionSession.DatabaseName)
        {
            return SkipServerHandler.Default;
        }

        return new UnicastServerHandler(sql, connectionSession, _serverDataReaderFactory);
    }

    private IServerHandler CreateTCLCommandServerHandler(ITCLCommand itclCommand,string sql, IConnectionSession connectionSession)
    {
        if (itclCommand is BeginTransactionCommand beginTransactionCommand)
        {
            return new TransactionServerHandler(TransactionOperationTypeEnum.BEGIN, connectionSession);
        }

        if (itclCommand is SetAutoCommitCommand setAutoCommitCommand)
        {
            if (setAutoCommitCommand.AutoCommit)
            {
                return connectionSession.GetTransactionStatus().IsInTransaction()
                    ? new TransactionServerHandler(TransactionOperationTypeEnum.COMMIT, connectionSession)
                    : new SkipServerHandler();
            }

            throw new NotSupportedException("SetAutoCommitCommand");
        }

        if (itclCommand is CommitCommand commitCommand)
        {
            return new TransactionServerHandler(TransactionOperationTypeEnum.COMMIT, connectionSession);
        }

        if (itclCommand is RollbackCommand rollbackCommand)
        {
            return new TransactionServerHandler(TransactionOperationTypeEnum.ROLLBACK, connectionSession);
        }
        //todo 判断设置隔离级别

        return new UnicastServerHandler(sql,connectionSession, _serverDataReaderFactory);
    }

    private void CheckNotSupportCommand(ISqlCommand sqlCommand)
    {
        if (sqlCommand is IDCLCommand)
        {
            throw new NotSupportedException("unsupported operation");
        }
    }
}