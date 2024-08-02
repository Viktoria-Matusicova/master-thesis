using SocketIOClient;

public class SocketStartupService : IHostedService
{
    private readonly ILogger<SocketStartupService> _logger;
    private readonly SocketIO _socket;

    public SocketStartupService(ILogger<SocketStartupService> logger, SocketIO socket)
    {
        _logger = logger;
        _socket = socket;
    }

    /// <summary>Connects to the websocket server at the start of the application.</summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _socket.OnConnected += (sender, e) =>
        {
            _logger.LogInformation("Service is succesfully connected to socket io server.");
        };
        await _socket.ConnectAsync();

    }

    /// <summary>Disconnects from the websocket server when application is stopped.</summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _socket.DisconnectAsync();
    }
}