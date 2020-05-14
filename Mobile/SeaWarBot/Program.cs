using System.Threading.Tasks;

namespace SeaWarBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var gamer = new Gamer();
            while (true)
            {
                await gamer.PlayAsync();
            }
        }
    }
}