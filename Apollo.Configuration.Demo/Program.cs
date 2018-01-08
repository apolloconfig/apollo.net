using Com.Ctrip.Framework.Apollo.Logging;
using System;

namespace Apollo.Configuration.Demo
{
    class Program
    {
        private static void Main()
        {
            LogManager.Provider = new ConsoleLoggerProvider(LogLevel.Trace);

            var demo = new ConfigurationDemo();

            Console.WriteLine("Apollo Config Demo. Please input key to get the value. Input quit to exit.");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }
                input = input.Trim();
                if (input.Equals("quit", StringComparison.CurrentCultureIgnoreCase))
                {
                    Environment.Exit(0);
                }
                demo.GetConfig(input);
            }
        }
    }
}
