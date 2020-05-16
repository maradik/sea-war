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
        private volatile CancellationTokenSource stopCancellationTokenSource;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public PeriodicalTimer(Func<TimeSpan, Task> periodicalCallback, Func<Task> onTimeoutCallback, Func<Task> onStopCallback)
        {
            this.onTimeoutCallback = onTimeoutCallback;
            this.onStopCallback = onStopCallback;
            this.periodicalCallback = periodicalCallback;
        }

        public async Task StartAsync(TimeSpan period, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout < period)
            {
                throw new ArgumentException();
            }

            await semaphoreSlim.WaitAsync();
            
            stopCancellationTokenSource = new CancellationTokenSource();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, stopCancellationTokenSource.Token);
            Task.Run(async () => await StartInternal(period, timeout, cts.Token)).ContinueInParallel();
        }

        public async Task StopAsync()
        {
            stopCancellationTokenSource?.Cancel();
            await onStopCallback();

            semaphoreSlim.Release();
        }

        private async Task StartInternal(TimeSpan period, TimeSpan timeout, CancellationToken cancellation)
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