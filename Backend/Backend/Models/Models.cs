using System;

namespace Backend.Models
{
    public enum RoomStatus
    {
        None = 0,
        NotReady = 1,
        Ready = 2
    }

    public class Room
    {
        public Guid Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public RoomStatus Status { get; set; }
    }

    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Map OwnMap { get; set; }
        public Map EnemyMap { get; set; }
    }

    public class Map
    {
        public Cell[,] Cells { get; set; }
    }

    public class Cell
    {
        public CellStatus Status { get; set; }

        public enum CellStatus
        {
            Empty = 0,
            EngagedByShip = 1,
            EmptyFired = 2,
            EngagedByShipFired = 3
        }
    }
}