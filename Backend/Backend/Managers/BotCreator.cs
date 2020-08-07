using System;
using Backend.Models;

namespace Backend.Managers
{
    public class BotCreator
    {
        private static readonly Random random = new Random();
        private static readonly string[] botNames = {"Федот-стрелец", "Иван Петрович", "Михалыч", "Super killer", "Marika-38", "Непотопляемый", "Капитан", "Джонни Дэп"};

        public Bot Create(Room room) =>
            new Bot(Guid.NewGuid(), botNames[random.Next(botNames.Length)], room);
    }
}