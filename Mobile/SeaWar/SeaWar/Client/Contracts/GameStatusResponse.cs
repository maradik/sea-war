namespace SeaWar.Client.Contracts
{
    public class GameStatusResponse
    {
        public GameStatus GameStatus { get; }
        public FinishReason FinishReason { get; }
        public Map MyMap { get; }
    }
}