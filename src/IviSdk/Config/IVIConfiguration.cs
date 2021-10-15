namespace IviSdkCsharp.Config
{
    public class IviConfiguration
    {
        public static string? EnvironmentId { get; set; }
        
        public static string? ApiKey { get; set; }

        public static string Host { get; set; } = "sdk-api.iviengine.com";
        
        public static int Port { get; set; } = 443;
        
        public static int KeepAlive { get; set; } = 30;
    }
}