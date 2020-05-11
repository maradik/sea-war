using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SeaWar.ViewModels
{
    public class PeriodicalTimer
    {
        private readonly Func<Task> onTimeoutCallback;
        private readonly Func<Task> onStopCallback;
        private readonly Func<TimeSpan, Task> periodicalCallback;
        private readonly TimeSpan period;
        private readonly TimeSpan timeout;
        private volatile CancellationTokenSource stopCancellationTokenSource;

        public PeriodicalTimer(TimeSpan period, Func<TimeSpan, Task> periodicalCallback, TimeSpan timeout, Func<Task> onTimeoutCallback, Func<Task> onStopCallback)
        {
            if (timeout < period)
            {
                throw new ArgumentException();
            }

            this.onTimeoutCallback = onTimeoutCallback;
            this.onStopCallback = onStopCallback;
            this.periodicalCallback = periodicalCallback;
            this.period = period;
            this.timeout = timeout;
        }

        public Task Start()
        {
            Task.Run(async () => await StartInternal());
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            stopCancellationTokenSource?.Cancel();
            onStopCallback();
            return Task.CompletedTask;
        }

        private async Task StartInternal()
        {
            stopCancellationTokenSource = new CancellationTokenSource();
            var timeoutCancellation = new CancellationTokenSource(timeout).Token;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            while (!stopCancellationTokenSource.Token.IsCancellationRequested)
            {
                if (timeoutCancellation.IsCancellationRequested)
                {
                    await onTimeoutCallback();
                    return;
                }

                var remain = timeout - stopwatch.Elapsed;
                if (remain < TimeSpan.Zero)
                {
                    remain = TimeSpan.Zero;
                }

                await periodicalCallback(remain);
                await Task.Delay(period);
            }
        }
    }
}