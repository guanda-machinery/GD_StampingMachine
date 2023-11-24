using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static class WaitForCondition
    {
        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="conditionFunc"></param>
        /// <param name="result"></param>
        /// <param name="isEqual"></param>
        /// <returns></returns>
        public static async Task<T> WaitAsync<T>(Func<T> conditionFunc, T result , CancellationToken token,  bool isEqual = true)
        {
            // 创建 TaskCompletionSource
            var tcs = new TaskCompletionSource<T>();
            // 啟動一個異步
            _ = MonitorConditionAsync(conditionFunc, result, isEqual, tcs , token);
            // 等待條件滿足
            return await tcs.Task;
        }


        static async Task MonitorConditionAsync<T>(Func<T> conditionFunc,   T result, bool isEqual, TaskCompletionSource<T> tcs , CancellationToken token)
        {


            try
            {
                while (isEqual != Equals(conditionFunc(), result))
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    await Task.Delay(10);
                }

                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }


    }
}
