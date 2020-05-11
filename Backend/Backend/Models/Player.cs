using System;

namespace Backend.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Map OwnMap { get; set; }
        public Map EnemyMap { get; set; }
    }
}