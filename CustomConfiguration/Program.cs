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

        public class MyApplication
        {
            private readonly IUserConfiguration _userConfiguration = null;
            private readonly ILogger _logger;


            /// <summary>
            /// Application initialization. 
            /// </summary>
            /// <param name="logger">Used to log information at runtime. Supplied by DI</param>
            /// <param name="userConfiguration">User configuration from appsettings.json/secrets.json. Supplied by DI</param>
            public MyApplication(ILogger<MyApplication> logger, IUserConfiguration userConfiguration)
            {
                _logger = logger;

                // Get the configuration needed for this demo. It assumes user secrets defined in the current entry assembly.
                _userConfiguration = userConfiguration;
            }

            /// <summary>
            /// This is the application entry point. 
            /// </summary>
            /// <returns></returns>
            internal async Task Run()
            {
                _logger.LogInformation("Application {applicationEvent} at {dateTime}", "Started", DateTime.UtcNow);

                await DoWork();

                _logger.LogInformation("Application {applicationEvent} at {dateTime}", "Ended", DateTime.UtcNow);

                Console.WriteLine("PRESS <ENTER> TO EXIT");
                Console.ReadKey();
            }

            /// <summary>
            /// All tests/work are performed here
            /// </summary>
            /// <returns></returns>
            internal async Task DoWork()
            {
                IUserConfiguration userConfiguration = ConfigFactory.Initialize<Program>();
                string result = JsonConvert.SerializeObject(userConfiguration, Formatting.Indented);

                _logger.LogInformation($"{result}");
            }
        }
    }
}
