using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using ShardingConnector.ProtocolCore.Packets;
using ShardingConnector.ProtocolCore.Payloads;

namespace ShardingConnector.ProtocolCore.Codecs;

public interface IDatabasePacketCodecEngine
{
    /// <summary>
    /// 验证头部
    /// </summary>
    /// <param name="readableBytes"></param>
    /// <returns></returns>
    bool IsValidHeader(int readableBytes);
    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="context"></param>
    /// <param name="input"></param>
    /// <param name="output"></param>
    void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output);

    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="context"></param>
    /// <param name="message"></param>
    /// <param name="output"></param>
    void Encode(IChannelHandlerContext context, IDatabasePacket message, IByteBuffer output);

    /// <summary>
    /// 创建一个消息包
    /// </summary>
    /// <param name="message"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    IPacketPayload CreatePacketPayload(IByteBuffer message, Encoding encoding);
}