using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetriqusSdk
{
    public static class Backoff
    {
        private static readonly SynchronizationContext unityContext = SynchronizationContext.Current;

        public static async Task<T> DoAsync<T>(
            string id,
            Func<Task<T>> action,
            Func<T, bool> validateResult = null,
            Action<T, bool> onComplete = null,
            int maxRetries = 10, int maxDelayMilliseconds = 2000, int delayMilliseconds = 200,
            CancellationToken cancellationToken = default)
        {
            var backoff = new ExponentialBackoff(delayMilliseconds, maxDelayMilliseconds);

            var exceptions = new List<Exception>();

            for (var retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation

                    T result = default;
                    Exception executionException = null;

                    // Ensure action runs on the main thread
                    var actionTask = new TaskCompletionSource<T>();
                    unityContext.Post(async _ =>
                    {
                        try
                        {
                            result = await action();
                            actionTask.SetResult(result);
                        }
                        catch (Exception ex)
                        {
                            executionException = ex;
                            actionTask.SetException(ex);
                        }
                    }, null);

                    await actionTask.Task; // Wait for execution on the main thread

                    if (executionException != null)
                        throw executionException; // Ensure exception is handled

                    var isValid = validateResult?.Invoke(result);

                    if (isValid.HasValue && isValid.Value)
                    {
                        // Ensure onComplete runs on the main thread
                        unityContext.Post(_ => onComplete?.Invoke(result, true), null);
                        return result;
                    }
                }
                catch (OperationCanceledException)
                {
                    // Log cancellation and break out of the loop
                    /*if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"While processing BackoOff Operation {id}, operation canceled");*/

                    throw; // Optionally re-throw to propagate cancellation
                }
                catch (Exception ex)
                {
                    /*if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"While processing BackoOff Operation {id} an exception occured: " + ex.Message);*/

                    exceptions.Add(ex);

                    Metriqus.DebugLog($"Back off error occured: {ex.ToString()}", UnityEngine.LogType.Error);
                }

                try
                {
                    await backoff.Delay(cancellationToken).ConfigureAwait(false); // Delay with cancellation support
                }
                catch (OperationCanceledException)
                {
                    /*if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog("Operation canceled during delay.");*/
                    throw new AggregateException("Operation was canceled.", exceptions);
                }
                catch (Exception e)
                {
                    /*if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog("Operation canceled during delay.");*/
                    exceptions.Add(e);

                    Metriqus.DebugLog($"Back off Delay error occured: {e.ToString()}", UnityEngine.LogType.Error);
                }
            }

            // Ensure onComplete runs on the main thread if failure
            unityContext.Post(_ => onComplete?.Invoke((T)default, false), null);

            throw new AggregateException(exceptions);
        }

        private struct ExponentialBackoff
        {
            private readonly int _delayMilliseconds;
            private readonly int _maxDelayMilliseconds;
            private int _retries;
            private int _pow;

            public ExponentialBackoff(int delayMilliseconds, int maxDelayMilliseconds)
            {
                _delayMilliseconds = delayMilliseconds;
                _maxDelayMilliseconds = maxDelayMilliseconds;
                _retries = 0;
                _pow = 1;
            }

            public Task Delay(CancellationToken cancellationToken = default)
            {
                ++_retries;
                if (_retries < 31)
                {
                    _pow = _pow << 1; // m_pow = Pow(2, m_retries - 1)
                }

                var delay = Math.Min(_delayMilliseconds * (_pow - 1) / 2, _maxDelayMilliseconds);

                /*if (Metriqus.LogLevel != LogLevel.NoLog)
                    Metriqus.DebugLog($"BackOff Process waiting for {delay}");*/

                return Task.Delay(delay, cancellationToken);
            }
        }
    }
}