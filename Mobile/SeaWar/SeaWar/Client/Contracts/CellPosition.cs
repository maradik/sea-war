namespace SeaWar.Client.Contracts
{
    public class CellPosition
    {
        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}