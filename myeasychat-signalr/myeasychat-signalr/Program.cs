using Microsoft.AspNetCore.Http.Connections;
using myeasychat;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR().AddHubOptions<EasyChatHub>(options =>
{
    options.MaximumReceiveMessageSize = 1 * 1024 * 1024; // 1 MB
    options.MaximumParallelInvocationsPerClient = 32;
    options.KeepAliveInterval = new TimeSpan(0, 0, 0, 5);
    options.ClientTimeoutInterval = new TimeSpan(0, 0, 0, 10);
    options.EnableDetailedErrors = true;

});

builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Trace);

//builder.WebHost.UseUrls("http://*:5000");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();
app.UseHttpsRedirection();
app.UseRouting();

//app.UseAuthorization();

app.MapControllers();
app.MapHub<EasyChatHub>("/EasyChatHub", options =>
{
    options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling /*| HttpTransportType.ServerSentEvents*/;
});

app.Run();
