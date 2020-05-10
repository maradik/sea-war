using System.Threading.Tasks;

namespace SeaWar.ViewModels
{
    public class GameViewModel
    {
        private readonly ISeaWarClient client;

        public GameViewModel(ISeaWarClient client)
        {
            this.client = client;
        }
        
        public CellStatus[,] Map { get; set; }
        public CellStatus[,] OpponentMap { get; set; }

        public async Task FireAsync(int x, int y)
        {
            var result = await client.FireAsync(x, y);
            OpponentMap = (CellStatus[,]) result;
            
            //ToDo redraw

            Task.Run(GetStatus);
        }

        private Task GetStatus()
        {
            client.GetStatus();
        }
    }

    public interface ISeaWarClient
    {
        Task<CellStatus[,]> FireAsync(int x, int y);
        Task<SomeStatus> GetStatus();
    }
}