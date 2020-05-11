using System;
using Backend.Models;

namespace Backend.Managers
{
    public class PlayerBuilder
    {
        private readonly MapBuilder mapBuilder;

        public PlayerBuilder(MapBuilder mapBuilder) =>
            this.mapBuilder = mapBuilder;

        public Player Build(string playerName) =>
            new Player
            {
                Id = Guid.NewGuid(),
                Name = playerName,
                OwnMap = mapBuilder.Build()
            };
    }
}