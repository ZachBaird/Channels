using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HowToUseChannels.Services
{
    public class Notifications
    {
        private readonly Database _database;
        private readonly IHttpClientFactory _httpClientFactory;

        public Notifications(Database database, IHttpClientFactory httpClientFactory)
        {
            _database = database;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> Send()
        {
            if (!await _database.Users.AnyAsync())
            {
                _database.Users.Add(new Data.User());
                await _database.SaveChangesAsync();
            }

            var user = await _database.Users.FirstOrDefaultAsync();

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://docs.microsoft.com");
            user.Message = response;

            await _database.SaveChangesAsync();

            return true;
        }

        public bool SendA()
        {
            Task.Run(async () =>
            {
                try
                {
                    if (!await _database.Users.AnyAsync())
                    {
                        _database.Users.Add(new Data.User());
                        await _database.SaveChangesAsync();
                    }

                    var user = await _database.Users.FirstOrDefaultAsync();

                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetStringAsync("https://docs.microsoft.com");
                    user.Message = response;

                    await _database.SaveChangesAsync();

                }
                catch (Exception ex)
                {
                    var a = ex;
                }
            });

            return true;
        }
    }
}
