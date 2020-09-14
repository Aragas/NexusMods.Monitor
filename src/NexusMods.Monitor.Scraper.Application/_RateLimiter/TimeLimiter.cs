using ComposableAsync;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter
{
    /// <summary>
    /// TimeLimiter implementation
    /// </summary>
    public class TimeLimiter : IDispatcher
    {
        private readonly IAwaitableConstraint _AwaitableConstraint;

        internal TimeLimiter(IAwaitableConstraint awaitableConstraint)
        {
            _AwaitableConstraint = awaitableConstraint;
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// returning the result of given function
        /// </summary>
        /// <param name="perform"></param>
        /// <returns></returns>
        public Task Enqueue(Func<Task> perform)
        {
            return Enqueue(perform, CancellationToken.None);
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// returning the result of given function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="perform"></param>
        /// <returns></returns>
        public Task<T> Enqueue<T>(Func<Task<T>> perform)
        {
            return Enqueue(perform, CancellationToken.None);
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// </summary>
        /// <param name="perform"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Enqueue(Func<Task> perform, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (await _AwaitableConstraint.WaitForReadiness(cancellationToken))
            {
                await perform();
            }
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// returning the result of given function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="perform"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> Enqueue<T>(Func<Task<T>> perform, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (await _AwaitableConstraint.WaitForReadiness(cancellationToken))
            {
                return await perform();
            }
        }

        public IDispatcher Clone() => new TimeLimiter(_AwaitableConstraint.Clone());

        private static Func<Task> Transform(Action act)
        {
            return () => { act(); return Task.FromResult(0); };
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// returning the result of given function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compute"></param>
        /// <returns></returns>
        private static Func<Task<T>> Transform<T>(Func<T> compute)
        {
            return () =>  Task.FromResult(compute());
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// </summary>
        /// <param name="perform"></param>
        /// <returns></returns>
        public Task Enqueue(Action perform)
        {
            var transformed = Transform(perform);
            return Enqueue(transformed);
        }

        /// <summary>
        ///  Perform the given task respecting the time constraint
        /// </summary>
        /// <param name="action"></param>
        public void Dispatch(Action action)
        {
            Enqueue(action);
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// returning the result of given function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="perform"></param>
        /// <returns></returns>
        public Task<T> Enqueue<T>(Func<T> perform)
        {
            var transformed = Transform(perform);
            return Enqueue(transformed);
        }

        /// <summary>
        /// Perform the given task respecting the time constraint
        /// returning the result of given function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="perform"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<T> Enqueue<T>(Func<T> perform, CancellationToken cancellationToken)
        {
            var transformed = Transform(perform);
            return Enqueue(transformed, cancellationToken);
        }

            /// <summary>
        /// Perform the given task respecting the time constraint
        /// </summary>
        /// <param name="perform"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Enqueue(Action perform, CancellationToken cancellationToken)
        {
           var transformed = Transform(perform);
           return Enqueue(transformed, cancellationToken);
        }

        /// <summary>
        /// Returns a TimeLimiter based on a maximum number of times
        /// during a given period
        /// </summary>
        /// <param name="maxCount"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static TimeLimiter GetFromMaxCountByInterval(int maxCount, TimeSpan timeSpan)
        {
            return new TimeLimiter(new CountByIntervalAwaitableConstraint(maxCount, timeSpan));
        }

        /// <summary>
        /// Compose various IAwaitableConstraint in a TimeLimiter
        /// </summary>
        /// <param name="constraints"></param>
        /// <returns></returns>
        public static TimeLimiter Compose(params IAwaitableConstraint[] constraints)
        {
            var composed = constraints.Aggregate(default(IAwaitableConstraint), (accumulated, current) => accumulated is null ? current : accumulated.Compose(current));
            return new TimeLimiter(composed);
        }
    }
}