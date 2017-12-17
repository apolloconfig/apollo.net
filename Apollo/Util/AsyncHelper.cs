using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Com.Ctrip.Framework.Apollo.Util
{
    internal static class AsyncHelper
    {
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        /// <summary>在同步方法中不死锁方式调用异步方法</summary>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var currentUiCulture = CultureInfo.CurrentUICulture;
            var currentCulture = CultureInfo.CurrentCulture;

            return MyTaskFactory.StartNew(() =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;

                    return func();
                })
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>在同步方法中不死锁方式调用异步方法</summary>
        public static void RunSync(Func<Task> func)
        {
            var currentUiCulture = CultureInfo.CurrentUICulture;
            var currentCulture = CultureInfo.CurrentCulture;

            MyTaskFactory.StartNew(() =>
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentUiCulture;

                    return func();
                })
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}
