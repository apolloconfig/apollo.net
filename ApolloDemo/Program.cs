using ApolloDemo;
using System;
using System.Net;
using Com.Ctrip.Framework.Apollo.Logging;

namespace Apollo.NetCoreApp.Demo
{
    class Program
    {
        private static void Main()
        {
            Console.WriteLine(typeof(WebRequest).Assembly.Location);

            LogManager.Provider = new ConsoleLoggerProvider(LogLevel.Trace);

            Console.WriteLine($"请输入 0：测试Configuration；其他：测试ConfigurationManagerDemo");

            Func<string, string> func;
            if (Console.ReadLine() == "0")
                func = new ConfigurationDemo().GetConfig;
            else
                func = new ConfigurationManagerDemo().GetConfig;

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
                func(input);
            }
        }
    }
}
