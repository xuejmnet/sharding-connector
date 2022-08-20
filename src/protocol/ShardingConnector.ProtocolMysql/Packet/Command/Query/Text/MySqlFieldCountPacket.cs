using ShardingConnector.ProtocolCore.Payloads;
using ShardingConnector.ProtocolMysql.Payload;

namespace ShardingConnector.ProtocolMysql.Packet.Command.Query.Text;

/// <summary>
/// https://dev.mysql.com/doc/internals/en/com-query-response.html
/// </summary>
public sealed class MySqlFieldCountPacket:IMysqlPacket
{

    public int SequenceId { get; }
    public int ColumnCount { get; }

    public MySqlFieldCountPacket(int sequenceId, int columnCount)
    {
        SequenceId = sequenceId;
        ColumnCount = columnCount;
    }

    public MySqlFieldCountPacket(MySqlPacketPayload payload):this(payload.ReadInt1(),payload.ReadInt1())
    {
        
    }
    // public MySQLFieldCountPacket(final MySQLPacketPayload payload) {
    //     this(payload.readInt1(), payload.readInt1());
    // }
    public void Write(MySqlPacketPayload payload)
    {
        payload.WriteIntLenenc(ColumnCount);
    }
}