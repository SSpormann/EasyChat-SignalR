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
    options.MaximumReceiveMessageSize = 1 * 1024 * 1024 * 1024; // 1 GB
    options.MaximumParallelInvocationsPerClient = 32;
    options.KeepAliveInterval = new TimeSpan(0, 0, 0, 5);
    options.ClientTimeoutInterval = new TimeSpan(0, 0, 0, 10);
    options.EnableDetailedErrors = true;

});
//builder.WebHost.UseUrls("http://*:5000");
builder.WebHost.ConfigureLogging(logging =>
{
    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Trace);
    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Trace);
});

builder.Host.ConfigureLogging(logging =>
{
    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Trace);
    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Trace);
});






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets();

//app.Run(async (context) =>
//{
//    await context.Response.WriteAsync("Easy chat service");

//});
app.UseHttpsRedirection();
app.UseRouting();

//app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<EasyChatHub>("/EasyChatHub", options =>
    {
        options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling /*| HttpTransportType.ServerSentEvents*/;
    });
});

app.MapControllers();

app.Run();
