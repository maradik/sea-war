using System;

namespace SeaWar
{
    public class Logger : ILogger
    {
        private readonly string context;

        public Logger(string context)
        {
            this.context = context;
        }

        public void Info(string msg)
        {
            Console.WriteLine($"{context}:{msg}");
        }

        public ILogger WithContext(string newContext)
        {
            return new Logger($"{context}.{newContext}");
        }
    }
}