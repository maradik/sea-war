namespace SeaWar.Client.Contracts
{
    public enum CreateRoomStatus
    {
        EmptyRoom = 0,        
        NotReady = 1,
        Ready = 2,
        Orphaned = 3,
        Finished = 4
    }
}