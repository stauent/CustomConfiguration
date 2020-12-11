using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using ConfigurationAssistant;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace CustomConfiguration
{
    class Program
    {
        public static ConfigurationResults<MyApplication> configuredApplication = null;

        static async Task Main(string[] args)
        {
            configuredApplication = ConsoleHostBuilderHelper.CreateApp<MyApplication>(args);
            await configuredApplication.myService.Run();
        }
    }


    public class MyApplication
    {
        private readonly IApplicationRequirements<MyApplication> _requirements;

        /// <summary>
        /// Application initialization.
        /// </summary>
        /// <param name="requirements">IApplicationRequirements contains all the other interfaces that the application needs</param>
        public MyApplication(IApplicationRequirements<MyApplication> requirements)
        {
            _requirements = requirements;
        }

        /// <summary>
        /// This is the application entry point. 
        /// </summary>
        /// <returns></returns>
        internal async Task Run()
        {
            $"Application Started at {DateTime.UtcNow}".TraceInformation("Run initiated");

            await DoWork();

            $"Application Ended at {DateTime.UtcNow}".TraceInformation();

            Console.WriteLine("PRESS <ENTER> TO EXIT");
            Console.ReadKey();
        }

        /// <summary>
        /// All tests/work are performed here
        /// </summary>
        /// <returns></returns>
        internal async Task DoWork()
        {
            _requirements.UserConfiguration.TraceInformation("Here's my user configuration");

            string cs = _requirements.UserConfiguration.ConnectionString("CareerCloud");
            Console.WriteLine($"CareerCloud connection string:\r\n{cs}");
        }
    }
}
