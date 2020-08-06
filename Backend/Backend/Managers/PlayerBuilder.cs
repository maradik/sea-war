using System;
using Backend.Models;

namespace Backend.Managers
{
    public class PlayerBuilder
    {
        private readonly MapBuilder mapBuilder;

        public PlayerBuilder(MapBuilder mapBuilder) =>
            this.mapBuilder = mapBuilder;

        public Player Build(Guid id, string playerName) =>
            new Player
            {
                Id = id,
                Name = playerName,
                OwnMap = mapBuilder.Build()
            };
    }
}