using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mmd.GameController
{
    public class App : IHostedService
    {
        private readonly IGameInstance _game;
        private Timer _timer;
        private readonly ILogger<GameInstance> _logger;
        private readonly IServiceProvider _provider;

        public App(IGameInstance game, ILogger<GameInstance> logger, IServiceProvider provider)
        {
            _game = game;
            _logger = logger ?? throw new ArgumentNullException("logger");
            _timer = new Timer(Start, null, Timeout.Infinite, 0);
            _provider = provider ?? throw new ArgumentNullException("provider");
        }

        private void Start(object state)
        {
            _game.Initialize();
            _timer?.Change(0, Timeout.Infinite);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning($"{DateTime.UtcNow} Game Controller started");
            _timer?.Change(0, Timeout.Infinite);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning($"{DateTime.UtcNow} Game Controller stopped");
            _timer?.Change(0, Timeout.Infinite);
            _timer?.Dispose();
            return Task.CompletedTask;
        }
    }
}
