namespace IviSdkCsharp.Config
{
    public class IviConfiguration
    {
        private static string _environmentId;
        private static string _apiKey;
        private static string _host = "sdk-api.iviengine.com";
        private static int _port = 443;
        private static int _keepAlive = 30;
        public static string GetEnvironmentId()
        {
            return IviConfiguration._environmentId;
        }

        public static void SetEnvironmentId(string environmentId)
        {
            IviConfiguration._environmentId = environmentId;
        }

        public static string GetApiKey()
        {
            return IviConfiguration._apiKey;
        }

        public static void SetApiKey(string apiKey)
        {
            IviConfiguration._apiKey = apiKey;
        }

        public static string GetHost()
        {
            return IviConfiguration._host;
        }

        public static void SetHost(string host)
        {
            IviConfiguration._host = host;
        }

        public static int GetPort()
        {
            return IviConfiguration._port;
        }

        public static void SetPort(int port)
        {
            IviConfiguration._port = port;
        }

        public static int GetKeepAlive()
        {
            return _keepAlive;
        }

        public static void SetKeepAlive(int keepAlive)
        {
            IviConfiguration._keepAlive = keepAlive;
        }
    }
}