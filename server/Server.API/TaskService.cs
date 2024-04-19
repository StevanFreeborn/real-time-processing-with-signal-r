using Microsoft.AspNetCore.SignalR;

namespace Server.API;

class TaskService(
  BackgroundTaskQueue taskQueue,
  IHubContext<TaskHub> taskHub
) : BackgroundService
{
  private readonly BackgroundTaskQueue _taskQueue = taskQueue;
  private readonly IHubContext<TaskHub> _taskHub = taskHub;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      var task = await _taskQueue.DequeueAsync(stoppingToken);

      _ = Task.Run(async () =>
      {
        Console.WriteLine($"Task {task.Id} is starting");
        await _taskHub.Clients.All.SendAsync("ReceiveMessage", $"Task {task.Id} is starting", cancellationToken: stoppingToken);

        var randomNumberOfSeconds = new Random().Next(5, 30);
        await Task.Delay(TimeSpan.FromSeconds(randomNumberOfSeconds), stoppingToken);

        Console.WriteLine($"Task {task.Id} is complete");
        await _taskHub.Clients.All.SendAsync("ReceiveMessage", $"Task {task.Id} is completed in {randomNumberOfSeconds}", cancellationToken: stoppingToken);
      }, stoppingToken);
    }
  }
}