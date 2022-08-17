using ShardingConnector.ProtocolMysql.Packet.Command;
using ShardingConnector.ProtocolMysql.Payload;

namespace ShardingConnector.ProtocolMysql.Packet.ServerCommand.Query;

/// <summary>
/// 查询命令
/// </summary>
public class MySqlQueryServerCommandPacket : AbstractMySqlServerCommandPacket
{
    public string Sql { get; }

    public MySqlQueryServerCommandPacket(string sql) : base(MySqlCommandTypeEnum.COM_QUERY)
    {
        Sql = sql;
    }

    public MySqlQueryServerCommandPacket(MySqlPacketPayload payload) : base(MySqlCommandTypeEnum.COM_QUERY)
    {
        Sql = payload.ReadStringEOF();
    }

    protected override void DoWrite(MySqlPacketPayload payload)
    {
        payload.WriteStringEOF(Sql);
    }
}