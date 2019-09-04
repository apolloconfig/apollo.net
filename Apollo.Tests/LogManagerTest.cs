using Com.Ctrip.Framework.Apollo.Logging;
using System;
using Xunit;

namespace Apollo.Tests
{
    public class LogManagerTest
    {
        [Fact]
        public void DefaultLogger()
        {
            Assert.NotNull(LogManager.LogFactory);
            Assert.NotNull(LogManager.CreateLogger(typeof(LogManagerTest)));
        }

        [Fact]
        public void Exception_Fallback()
        {
            LogManager.LogFactory = s => throw new NotSupportedException();

            Assert.NotNull(LogManager.CreateLogger(typeof(LogManagerTest)));
        }
    }
}
