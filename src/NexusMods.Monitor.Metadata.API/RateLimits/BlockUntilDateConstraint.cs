﻿using RateLimiter;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API.RateLimits
{
    public class BlockUntilDateConstraint : IAwaitableConstraint
    {
        private class DummyDisposable : IDisposable
        {
            public void Dispose() { }
        }

        private readonly DateTime _blockUntil;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public BlockUntilDateConstraint(DateTime blockUntil)
        {
            _blockUntil = blockUntil;
        }

        public async Task<IDisposable> WaitForReadiness(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            var timeToWait = _blockUntil - DateTime.Now;
            if (timeToWait < TimeSpan.Zero)
                return new DummyDisposable();

            try
            {
                await Task.Delay(timeToWait, cancellationToken);
                return new DummyDisposable();

            }
            finally
            {
                _semaphore.Release();
            }
        }

        public IAwaitableConstraint Clone() => new BlockUntilDateConstraint(_blockUntil);
    }
}