using System.Net;

using Server.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<BackgroundTaskQueue>();
builder.Services.AddHostedService<TaskService>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(
  options =>
    options.AddDefaultPolicy(
      builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins("https://localhost:5173")
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(config =>
  {
    config.SwaggerEndpoint("/swagger/v1/swagger.json", "Server API");
    config.RoutePrefix = string.Empty;
  });
}

app.UseHttpsRedirection();

app
  .MapPost("/add-task", async (BackgroundTaskQueue queue) =>
  {
    var task = new BackgroundTask();
    await queue.EnqueueAsync(task);
    return Results.Json(data: task, statusCode: (int)HttpStatusCode.Created);
  })
  .WithName("AddTask")
  .WithDisplayName("Add Task")
  .WithDescription("Add a new task to the queue");

app.MapHub<TaskHub>("/task-hub");

app.UseCors();

app.Run();
