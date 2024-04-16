using Microsoft.AspNetCore.SignalR;

namespace Server.API;

public class TaskHub : Hub
{
  public async Task SendMessage(string message)
  {
    await Clients.All.SendAsync("ReceiveMessage", message);
  }
}