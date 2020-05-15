using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SeaWar.Extensions;

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
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await semaphoreSlim.WaitAsync();
            
            stopCancellationTokenSource = new CancellationTokenSource();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, stopCancellationTokenSource.Token);
            Task.Run(async () => await StartInternal(cts.Token)).ContinueInParallel();
        }

        public async Task StopAsync()
        {
            stopCancellationTokenSource?.Cancel();
            await onStopCallback();

            semaphoreSlim.Release();
        }

        private async Task StartInternal(CancellationToken cancellation)
        {
            var timeoutCancellation = new CancellationTokenSource(timeout).Token;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            while (!cancellation.IsCancellationRequested)
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