using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TTController.Service.Utils
{
    public static class Extensions
    {
        public static IEnumerable<Type> FindImplementations(this Type type)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract);

            return types.Where(t => type.IsAssignableOrSubclass(t));
        }

        public static bool IsAssignableOrSubclass(this Type type, Type c)
            => type.IsInterface
                ? type.IsAssignableFrom(c)
                : c.IsSubclassOf(type) || type == c;

        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            var registeredHandle = default(RegisteredWaitHandle);
            var tokenRegistration = default(CancellationTokenRegistration);
            try
            {
                var completionSource = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(
                    handle,
                    (state, timedOut) => ((TaskCompletionSource<bool>)state).TrySetResult(!timedOut),
                    completionSource,
                    millisecondsTimeout,
                    executeOnlyOnce: true
                );
                tokenRegistration = cancellationToken.Register(state => ((TaskCompletionSource<bool>)state).TrySetCanceled(), completionSource);
                return await completionSource.Task;
            }
            finally
            {
                registeredHandle?.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellationToken)
            => handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellationToken);

        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellationToken)
            => handle.WaitOneAsync(Timeout.Infinite, cancellationToken);
    }
}
