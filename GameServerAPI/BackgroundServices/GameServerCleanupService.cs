using Microsoft.Extensions.Options;
using GameServerAPI.Configuration;
using GameServerAPI.Services;

namespace GameServerAPI.BackgroundServices
{
    /// <summary>
    /// Class responsible for cleaning up of inactive game servers. Designed
    /// to run as a background service that removes game servers that haven't
    /// sent a heartbeat in the configured amount of time.
    /// </summary>
    public class GameServerCleanupService : BackgroundService
    {
        private readonly GameServerManagerSettings _settings;
        private readonly IServiceScopeFactory _scopeFactory;
        /// <summary>
        /// Initializes instance of GameServerCleanupService.
        /// </summary>
        /// <param name="settings">Instance of IOptions for accessing Game Server Manager settings, such as heartbeat
        /// interval and enablement.</param>
        /// <param name="scopeFactory">Instance of IServiceScope factory for accessing game server manager.</param>
        public GameServerCleanupService(IOptions<GameServerManagerSettings> settings, IServiceScopeFactory scopeFactory)
        {
            _settings = settings.Value;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Background task for periodic server cleanup.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>Token that signals the cancellation of the background task.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            // Check if server heartbeat is enabled
            if(_settings.HeartbeatEnabled)
            {
                while(!cancellationToken.IsCancellationRequested)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var gameServerManager = scope.ServiceProvider.GetRequiredService<GameServerManager>();
                        await gameServerManager.RemoveInactiveServers();
                        await Task.Delay(TimeSpan.FromSeconds(_settings.HeartbeatCheckInterval), cancellationToken);
                    }
                }
            }
        }
    }
}
