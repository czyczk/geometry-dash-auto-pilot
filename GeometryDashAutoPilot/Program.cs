using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;

namespace GeometryDashAutoPilot
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.Error.WriteLine("The program takes an argument of the level to run.");
            }

            var isSuccess = uint.TryParse(args[0], out var level);
            if (!isSuccess || level <= 0)
            {
                Console.Error.WriteLine("Invalid level.");
            }

            ConfigureLogger();

            var levelProfile = new LevelProfile(level);
            var globalController = new GlobalController(levelProfile);
            globalController.SubscribeHandlers();
            Application.Run(new ApplicationContext()); // Need this to keep the program running as well as make the global key hook work.
        }

        private static void ConfigureLogger()
        {
            var loggerConfig = new LoggerConfiguration();
#if DEBUG
            loggerConfig = loggerConfig.MinimumLevel.Debug();
#endif
            Log.Logger = loggerConfig.
                WriteTo.Console().
                CreateLogger();

        }
    }
}
