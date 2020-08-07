using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Integration.Dtos.v2;
using SeaWar.Annotations;
using SeaWar.Client;
using SeaWar.DomainModels;
using SeaWar.Extensions;
using SeaWar.View;
using Xamarin.Forms;

namespace SeaWar.ViewModels
{
    public class MainMenuPageViewModel : INotifyPropertyChanged
    {
        private readonly GameModel gameModel;
        private readonly Func<GameModel, GamePage> createGamePage;
        private readonly IClient client;
        private bool pageEnabled;
        private Room[] rooms;

        public MainMenuPageViewModel(GameModel gameModel, Func<GameModel, GamePage> createGamePage, Func<GameModel, WaitGamePage> createWaitGamePage, IClient client)
        {
            this.gameModel = gameModel;
            this.createGamePage = createGamePage;
            this.client = client;
            PageEnabled = true;
            CreateRoomWithBot = new Command(async _ =>
            {
                PageEnabled = false;
                var room = await client.CreateRoomAsync(new CreateRoomRequestDto {PlayerName = gameModel.PlayerName}, gameModel.PlayerId).ConfigureAwait(true);
                gameModel.RoomId = room.RoomId;
                await Application.Current.MainPage.Navigation.PushModalAsync(createWaitGamePage(gameModel)).ConfigureAwait(true);
            });
            CreateRoomWithPlayer = new Command(async _ =>
            {
                PageEnabled = false;
                var room = await client.CreateRoomAsync(new CreateRoomRequestDto {PlayerName = gameModel.PlayerName}, gameModel.PlayerId).ConfigureAwait(true);
                gameModel.RoomId = room.RoomId;
                await Application.Current.MainPage.Navigation.PushModalAsync(createWaitGamePage(gameModel)).ConfigureAwait(true);
            });
            RestartGame = new Command(_ =>
            {
                var application = (App) Application.Current;
                application.BeginGame();
            });

            RefreshRoomsAsync().ContinueInParallel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool PageEnabled
        {
            get => pageEnabled;
            set
            {
                pageEnabled = value;
                OnPropertyChanged(nameof(PageEnabled));
            }
        }

        public Room[] Rooms
        {
            get => rooms;
            set
            {
                rooms = value;
                OnPropertyChanged(nameof(Rooms));
            }
        }

        public Command RestartGame { get; }
        public Command CreateRoomWithBot { get; }
        public Command CreateRoomWithPlayer { get; }

        public async Task JoinRoom(Room room, Func<Task> onFail)
        {
            var joinRoomRequestDto = new JoinRoomRequestDto {PlayerName = gameModel.PlayerName};
            var result = await client.JoinRoomAsync(joinRoomRequestDto, room.Id, gameModel.PlayerId).ConfigureAwait(true);
            if (result.Success)
            {
                gameModel.RoomId = room.Id;
                gameModel.AnotherPlayerName = room.PlayerName;
                await Application.Current.MainPage.Navigation.PushModalAsync(createGamePage(gameModel)).ConfigureAwait(true);
            }
            else
            {
                await onFail().ConfigureAwait(true);
                await RefreshRoomsAsync().ConfigureAwait(true);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task RefreshRoomsAsync()
        {
            var roomList = await client.GetOpenedRoomsAsync(gameModel.PlayerId).ConfigureAwait(true);
            Rooms = roomList.Rooms.Select(x => new Room {Id = x.Id, PlayerName = x.Players.First().Name}).ToArray();
        }
    }
}