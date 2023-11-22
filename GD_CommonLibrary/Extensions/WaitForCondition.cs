using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_CommonLibrary.Extensions
{
    public static class WaitForCondition<T>
    {

        public static async Task<T> WaitAsync(Func<T> conditionFunc, T result)
        {
            // 创建 TaskCompletionSource
            var tcs = new TaskCompletionSource<T>();
            // 啟動一個異步
            _ = MonitorConditionAsync(conditionFunc, result, tcs);
            // 等待條件滿足
            return await tcs.Task;
        }


        static async Task MonitorConditionAsync(Func<T> conditionFunc,   T result, TaskCompletionSource<T> tcs)
        {
            try
            {
                while (!Equals(conditionFunc(), result))
                {
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
