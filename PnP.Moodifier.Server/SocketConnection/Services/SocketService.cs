using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MightyPotato.PnP.Moodifier.Server.Configuration;
using SimpleSockets;
using SimpleSockets.Messaging.Metadata;
using SimpleSockets.Server;

namespace MightyPotato.PnP.Moodifier.Server.SocketConnection.Services
{
    public sealed class SocketService : IHostedService
    {
        #region Events
        public event EventHandler<(int, string)>? MessageReceived;
        public event EventHandler<int>? ClientConnected;
        public event EventHandler<int>? ClientDisconnected;
        #endregion

        private HostConfig _config;
        private ILogger _logger;
        private SimpleSocketTcpListener? _listener;
        private CancellationTokenSource _tokenSource;

        public SocketService(IOptions<HostConfig> config, ILogger<SocketService> logger)
        {
            _config = config.Value;
            _logger = logger;
            _tokenSource = new CancellationTokenSource();
            _listener = new SimpleSocketTcpListener();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Server starting up...");
            if (_listener == null)
            {
                _listener = new SimpleSocketTcpListener();
            }
            SubscribeEvents();
            _listener.StartListening(_config.Ip, _config.Port);
            _logger.LogInformation("Server ready and listening on port {Port}", _config.Port);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Server stopping...");
            UnsubscribeEvents();
            _listener?.Dispose();
            _listener = null;
            return Task.CompletedTask;
        }

        private void SubscribeEvents()
        {
            _logger.LogInformation("Subscribing to events");
            if (_listener != null)
            {
                _listener.MessageReceived += ListenerOnMessageReceived;
                _listener.ClientConnected += ListenerOnClientConnected;
                _listener.ClientDisconnected += ListenerOnClientDisconnected;
            }
            else
            {
                throw new NullReferenceException("No SimpleSocketTCPListener");
            }
        }

        private void UnsubscribeEvents()
        {
            _logger.LogInformation("Unsubscribing from events");
            if (_listener != null)
            {
                _listener.MessageReceived -= ListenerOnMessageReceived;
                _listener.ClientConnected -= ListenerOnClientConnected;
                _listener.ClientDisconnected -= ListenerOnClientDisconnected;
            }
            else
            {
                throw new NullReferenceException("No SimpleSocketTCPListener");
            }
        }

        private void ListenerOnClientConnected(IClientInfo clientInfo)
        {
            _logger.LogInformation("New client connected with ID {ClientInfoId}", clientInfo.Id);
            ClientConnected?.Invoke(this, clientInfo.Id);
        }

        private void ListenerOnClientDisconnected(IClientInfo clientInfo, DisconnectReason reason)
        {
            _logger.LogInformation("Client with ID {ClientInfoId} disconnected", clientInfo.Id);
            ClientDisconnected?.Invoke(this, clientInfo.Id);
        }

        private void ListenerOnMessageReceived(IClientInfo clientInfo, string message)
        {
            _logger.LogInformation("New Message from client {x}", clientInfo.Id);
            MessageReceived?.Invoke(this, (clientInfo.Id, message));
        }
    }
}