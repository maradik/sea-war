using System;

namespace Backend.Models
{
    public class Bot
    {
        private static readonly Random random = new Random();
        private readonly Room room;

        public Bot(Guid id, string name, Room room)
        {
            Id = id;
            Name = name;
            this.room = room;
        }

        public Guid Id { get; }
        private string Name { get; }
        private Map EnemyMap { get; set; }

        public bool TryPlay()
        {
            if (!room.IsActive)
            {
                return false;
            }

            try
            {
                if (room.IsOpened)
                {
                    room.Enter(Id, Name);
                }

                var game = room.GetGameFor(Id);
                if (game.GameStatus == GameStatus.YourChoice)
                {
                    EnemyMap = Fire();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Map Fire()
        {
            while (true)
            {
                var (x, y) = (random.Next(10), random.Next(10));
                if (EnemyMap == null || EnemyMap.Cells[x, y].Status == CellStatus.Empty)
                {
                    var fireResponse = room.Fire(x, y, Id);
                    return fireResponse.EnemyMap;
                }
            }
        }
    }
}