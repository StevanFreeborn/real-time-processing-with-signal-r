using System.Threading.Channels;

namespace Server.API;

class BackgroundTaskQueue
{
  private readonly Channel<BackgroundTask> _channel = Channel.CreateUnbounded<BackgroundTask>();

  public async Task EnqueueAsync(BackgroundTask task)
  {
    await _channel.Writer.WriteAsync(task);
  }

  public async Task<BackgroundTask> DequeueAsync(CancellationToken cancellationToken)
  {
    return await _channel.Reader.ReadAsync(cancellationToken);
  }
}