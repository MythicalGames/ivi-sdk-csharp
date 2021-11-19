using Mythical.Game.IviSdkCSharp.Exception;

namespace Mythical.Game.IviSdkCSharp.Config
{
    public class IviConfiguration
    {
        public string? EnvironmentId { get; set; }
        
        public string? ApiKey { get; set; }

        public string Host { get; set; } = "sdk-api.iviengine.com";
        
        public int Port { get; set; } = 443;
        
        public int KeepAlive { get; set; } = 30;


        internal void Validate()
        {
            EnsureStringValue(EnvironmentId, "Environment Id not set!", IVIErrorCode.ENVIRONMENT_ID_NOT_SET);
            EnsureStringValue(ApiKey, "API Key not set!", IVIErrorCode.APIKEY_NOT_SET);
            EnsureStringValue(Host, "Host not set!", IVIErrorCode.HOST_NOT_SET);

            static void EnsureStringValue(string? value, string errorMessage, IVIErrorCode errorCode)
            {
                if (string.IsNullOrWhiteSpace(value)) throw new IVIException(errorMessage, errorCode);
            }
        }
    }
}