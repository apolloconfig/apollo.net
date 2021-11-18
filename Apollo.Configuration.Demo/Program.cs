using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Logging;

namespace Apollo.Configuration.Demo;

internal class Program
{
    private static void Main()
    {
        LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);

        YamlConfigAdapter.Register();

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
