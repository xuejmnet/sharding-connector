using System.Threading.Channels;
using NCDC.ProxyServer.Abstractions;

namespace NCDC.ProxyClient.Command;

public sealed class MessageExecutor:IMessageExecutor
{
    private readonly Channel<ICommand> _channel;
    private readonly Task _commandTask;
    private readonly CancellationTokenSource _cts;
    public MessageExecutor()
    {
        _cts = new CancellationTokenSource();
        _channel = Channel.CreateUnbounded<ICommand>();
        _commandTask = Task.Factory.StartNew(ProcessAsync,_cts.Token, TaskCreationOptions.LongRunning,TaskScheduler.Default);
    }
    public bool TryAddMessage(ICommand command)
    {
        return _channel.Writer.TryWrite(command);
    }

    public async ValueTask ProcessAsync()
    {
        await foreach (var command in _channel.Reader.ReadAllAsync())
        {
            if (!_cts.IsCancellationRequested)
            {
                await command.ExecuteAsync();
            }
        }
    }

    public void Dispose()
    {
        _channel.Writer.Complete();
        _cts.Cancel();
        _commandTask.Wait(100);
    }
}