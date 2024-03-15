using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static class WaitForCondition
    {
        public static async Task WaitIsTrueAsync(Func<bool> conditionFunc)
        {
            await WaitAsync(conditionFunc, true);
        }

        public static async Task WaitAsyncIsFalse(Func<bool> conditionFunc)
        {
            await WaitAsync(conditionFunc, false);
        }

        public static async Task WaitIsTrueAsync(Func<bool> conditionFunc, CancellationToken token)
        {
            await WaitAsync(conditionFunc, true, token);
        }

        public static async Task WaitAsyncIsFalse(Func<bool> conditionFunc, CancellationToken token)
        {
            await WaitAsync(conditionFunc, false, token);
        }


        public static async Task WaitIsTrueAsync(Func<bool> conditionFunc, int millisecondsDelay)
        {
            using CancellationTokenSource cts = new(millisecondsDelay);
            await WaitAsync(conditionFunc, true, cts.Token);
        }

        public static async Task WaitAsyncIsFalse(Func<bool> conditionFunc, int millisecondsDelay)
        {
            using CancellationTokenSource cts = new(millisecondsDelay);
            await WaitAsync(conditionFunc, false, cts.Token);
        }

        public static async Task WaitIsTrueAsync(Func<bool> conditionFunc, int millisecondsDelay, CancellationToken token)
        {
            using CancellationTokenSource cts = new(millisecondsDelay);
            await WaitAsync(conditionFunc, true, cts.Token, token);
        }

        public static async Task WaitAsyncIsFalse(Func<bool> conditionFunc, int millisecondsDelay, CancellationToken token)
        {
            using CancellationTokenSource cts = new(millisecondsDelay);
            await WaitAsync(conditionFunc, false, cts.Token, token);
        }

        /// <summary>
        /// 等待當值被變更(訂閱功能)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task WaitChangeAsync<T>(Func<T> conditionFunc, params CancellationToken[] tokens)
        {
            await WaitAsync(conditionFunc, conditionFunc(), false, tokens);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        public static async Task WaitAsync<T>(Func<T> conditionFunc, T result)
        {
            await WaitAsync(conditionFunc, result, true);
        }
        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        public static async Task WaitNotAsync<T>(Func<T> conditionFunc, T result)
        {
            await WaitAsync(conditionFunc, result, false);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="millisecondsDelay"></param>
        /// <returns></returns>
        public static async Task WaitAsync<T>(Func<T> conditionFunc, T result, int millisecondsDelay)
        {
            using CancellationTokenSource cts = new(millisecondsDelay);
            await WaitAsync(conditionFunc, result, true, cts.Token);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="millisecondsDelay"></param>
        /// <returns></returns>
        public static async Task WaitNotAsync<T>(Func<T> conditionFunc, T result, int millisecondsDelay)
        {
            using CancellationTokenSource cts = new(millisecondsDelay);
            await WaitAsync(conditionFunc, result, false, cts.Token);
        }


        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        public static async Task WaitAsync<T>(Func<T> conditionFunc, T result, params CancellationToken[] tokens)
        {
            await WaitAsync(conditionFunc, result, true, tokens);
        }
        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        public static async Task WaitNotAsync<T>(Func<T> conditionFunc, T result, params CancellationToken[] tokens)
        {
            await WaitAsync(conditionFunc, result, false, tokens);
        }




        public static async Task<bool> WaitAsync<T>(Func<T> conditionFunc, T result, bool isEqual, params CancellationToken[] cancellationTokens)
        {
            var tcs = new TaskCompletionSource<bool>();
            await Task.Run(async () =>
            {
                try
                {
                    while (isEqual != Equals(conditionFunc(), result))
                    {
                        await Task.Delay(10);
                        
                        foreach (var token in cancellationTokens)
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();
                        }

                    }
                    tcs.SetResult(true);
                }
                catch (OperationCanceledException ocex)
                {
                    tcs.SetException(ocex);
                }
                catch(Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return await tcs.Task;
        }



        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        /*public static async Task<bool> WaitAsync<T>(Func<T> conditionFunc, T result, bool isEqual, params CancellationToken[] tokens)
        {
            // 创建 TaskCompletionSource
            var tcs = new TaskCompletionSource<bool>();
            // 啟動一個異步
            _ = MonitorConditionAsync(conditionFunc, result, isEqual, tcs, tokens);
            await Task.Yield();
            // 等待條件滿足
            return await tcs.Task;
        }

        static async Task<bool> MonitorConditionAsync<T>(Func<T> conditionFunc, T result, bool isEqual, TaskCompletionSource<bool> tcs, params CancellationToken[] tokens)
        {
            try
            {
                while (isEqual != Equals(conditionFunc(), result))
                {
                    if (tokens != null)
                    {
                        var canceledTokens = tokens.Where(token => token.IsCancellationRequested);
                        foreach (var canceledToken in canceledTokens)
                        {
                            canceledToken.ThrowIfCancellationRequested();
                        }
                    }
                    await Task.Delay(1);
                    await Task.Yield();
                }
                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task.Result;
        }*/


    }
}
