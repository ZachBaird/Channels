using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HowToUseChannels.Services
{
    public class NotificationDispatcher : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NotificationDispatcher> _logger;
        private readonly Channel<string> _channel;
        private readonly IServiceProvider _provider;

        public NotificationDispatcher(IHttpClientFactory httpClientFactory, IServiceProvider provider,
                                      ILogger<NotificationDispatcher> logger, Channel<string> channel)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _channel = channel;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // If not complete, read from channel.
            while (!_channel.Reader.Completion.IsCompleted)
            {
                // Read from channel.
                var msg = await _channel.Reader.ReadAsync();

                try
                {
                    // Create a scope from our service provider.
                    using (var scope = _provider.CreateScope())
                    {
                        // Pull out the database from our service provider so we can use it here.
                        var database = scope.ServiceProvider.GetRequiredService<Database>();
                        if (!await database.Users.AnyAsync())
                        {
                            database.Users.Add(new Data.User());
                            await database.SaveChangesAsync();
                        }

                        var user = await database.Users.FirstOrDefaultAsync();

                        var client = _httpClientFactory.CreateClient();
                        var response = await client.GetStringAsync("https://docs.microsoft.com");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Notification failed");
                }
            }
        }
    }
}
