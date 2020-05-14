using SeaWar;
using Xamarin.Forms;

namespace SeaWarBot
{
    public class DummyLogger : ILogger 
    {
        public void Info(string msg)
        {
        }

        public ILogger WithContext(string context) =>
            this;
    }
}