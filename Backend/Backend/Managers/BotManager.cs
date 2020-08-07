using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Managers
{
    public class BotManager
    {
        private readonly BotCreator botCreator;
        private readonly ConcurrentDictionary<Guid, Bot> bots = new ConcurrentDictionary<Guid, Bot>();

        public BotManager(BotCreator botCreator)
        {
            this.botCreator = botCreator;
            Task.Run(async () => await PlayBotsAsync().ConfigureAwait(false));
        }

        public void AddBot(Room room)
        {
            var bot = botCreator.Create(room);
            bots.TryAdd(bot.Id, bot);
        }

        private async Task PlayBotsAsync()
        {
            while (true)
            {
                foreach (var bot in bots.Values)
                {
                    if (!bot.TryPlay())
                    {
                        bots.TryRemove(bot.Id, out _);
                    }
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }
        }
    }
}