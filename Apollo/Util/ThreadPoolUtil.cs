using Com.Ctrip.Framework.Apollo.Amib.Threading;

namespace Com.Ctrip.Framework.Apollo.Util
{
    public class ThreadPoolUtil
    {
        public static SmartThreadPool NewThreadPool(int minWorkerThreads, int maxWorkerThreads, 
                int idleTimeout, bool daemon)
        {
            STPStartInfo info = new STPStartInfo();
            info.MinWorkerThreads = minWorkerThreads;
            info.MaxWorkerThreads = maxWorkerThreads;
            info.IdleTimeout = idleTimeout;
            info.AreThreadsBackground = daemon;

            return new SmartThreadPool(info);
        }
    }
}
