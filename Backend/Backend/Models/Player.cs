using System;
using System.Reflection.Metadata.Ecma335;

namespace Backend.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Map OwnMap { get; set; }

        public bool AnyShipsAlive() =>
            OwnMap.HasAliveShip();

        public FireResult ProcessEnemyMove(int x, int y)
        {
            if (OwnMap.HasShip(x, y))
            {
                OwnMap.Fire(x, y);

                var ship = OwnMap.GetShip(x, y);
                ship.Damage(x, y);

                var shipStatus = ship.Status == ShipStatus.Alive ? FireResult.Damaged : FireResult.Killed;
                if (shipStatus == FireResult.Killed)
                {
                    OwnMap.DismissShipNeighbours(x, y);
                }

                return shipStatus;
            }

            if (OwnMap.IsEmpty(x, y))
            {
                OwnMap.Fire(x, y);
                return FireResult.Missed;
            }

            return FireResult.NeedRetry;
        }
    }
}