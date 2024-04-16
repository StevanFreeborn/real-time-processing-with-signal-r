namespace Server.API;

class BackgroundTask
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
}