using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConfigurationAssistant
{
    /// <summary>
    /// Every interface that the application needs is provided in a
    /// single interface that exposes each interface as a property.
    /// This way, all constructors simply have ONE parameter of type
    /// IApplicationRequirements, and every dependency injected interface you need
    /// is supplied for you. You don't need to specify each one
    /// individually in your constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IApplicationRequirements<T>
    {
        ILogger<T> ApplicationLogger { get; set; }
        IConfiguration ApplicationConfiguration { get; set; }
        IUserConfiguration UserConfiguration { get; set; }
    }

    /// <summary>
    /// Every constructor that needs application required interfaces
    /// should have IApplicationRequirements as a parameter. This
    /// one interface will provide the app with every other interface it needs. 
    /// </summary>
    /// <typeparam name="T">Type of the application that is running. Provides a mechanism to write logs to a specific filename</typeparam>
    public class ApplicationRequirements<T> : IApplicationRequirements<T>
    {
        public ILogger<T> ApplicationLogger { get; set; }
        public IUserConfiguration UserConfiguration { get; set; }
        public IConfiguration ApplicationConfiguration { get; set; }

        public ApplicationRequirements(ILogger<T> applicationLogger, IUserConfiguration userConfiguration, IConfiguration applicationConfiguration)
        {
            this.UserConfiguration = userConfiguration;
            this.ApplicationLogger = applicationLogger;
            this.ApplicationConfiguration = applicationConfiguration;
            TraceLoggerExtension._Logger = applicationLogger;
        }
    }

    /// <summary>
    /// Extension method that will allow ANY object to log it's information
    /// with a very simple syntax. Just append one of the ".TraceXXX" methods
    /// to ANY object, and the contents of that object will be output in the
    /// specified log locations.
    /// </summary>
    public static class TraceLoggerExtension
    {
        public static ILogger _Logger { get; set; }

        public static ObjectSerializationFormat _SerializationFormat { get; set; } = ObjectSerializationFormat.Json;

        public enum ObjectSerializationFormat
        {
            String,
            Json
        }

        public static void TraceInformation(this object objectToTrace, string message = null, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string MethodName = null, [CallerFilePath] string FileName = null)
        {
            _Logger.LogInformation($"{FileName}:{MethodName}:{LineNumber} {message ?? ""}\r\n{ConvertToString(objectToTrace)}");
        }
        public static void TraceCritical(this object objectToTrace, string message = null, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string MethodName = null, [CallerFilePath] string FileName = null)
        {
            _Logger.LogCritical($"{FileName}:{MethodName}:{LineNumber} {message ?? ""}\r\n{ConvertToString(objectToTrace)}");
        }
        public static void TraceDebug(this object objectToTrace, string message = null, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string MethodName = null, [CallerFilePath] string FileName = null)
        {
            _Logger.LogDebug($"{FileName}:{MethodName}:{LineNumber} {message ?? ""}\r\n{ConvertToString(objectToTrace)}");
        }
        public static void TraceError(this object objectToTrace, string message = null, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string MethodName = null, [CallerFilePath] string FileName = null)
        {
            _Logger.LogError($"{FileName}:{MethodName}:{LineNumber} {message ?? ""}\r\n{ConvertToString(objectToTrace)}");
        }
        public static void TraceWarning(this object objectToTrace, string message = null, [CallerLineNumber] int LineNumber = 0, [CallerMemberName] string MethodName = null, [CallerFilePath] string FileName = null)
        {
            _Logger.LogWarning($"{FileName}:{MethodName}:{LineNumber} {message ?? ""}\r\n{ConvertToString(objectToTrace)}");
        }

        static string ConvertToString(object objectToTrace)
        {
            JsonSerializerSettings jSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MaxDepth = 1
            };

            string retVal = "";
            if (objectToTrace != null)
            {
                switch (_SerializationFormat)
                {
                    case ObjectSerializationFormat.Json:
                        retVal = JsonConvert.SerializeObject(objectToTrace, Formatting.Indented, jSettings);
                        break;
                    case ObjectSerializationFormat.String:
                        retVal = retVal.ToString();
                        break;
                }
            }

            return (retVal);
        }
    }
}
