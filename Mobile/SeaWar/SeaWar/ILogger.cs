namespace SeaWar
{
    public interface ILogger
    {
        void Info(string msg);
        ILogger WithContext(string context);
    }
}