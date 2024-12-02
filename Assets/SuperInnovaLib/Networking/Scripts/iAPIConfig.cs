using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SuperInnovaLib
{
    [System.Serializable]
    public class iAPIInfo
    {
        public int    port;
        public string host;
        public string key;
        public bool   isDefault;
        public int    version; 

        public string endPoint
        {
            get
            {
                if (host.StartsWith("https"))
                {
                    return host;
                }
                else
                {
                    return $"{host}:{port}";
                }
            }
        }
    }


    public class iAPIConfig
    {
        public const string DEVELOPMENT = "development";
        public const string PRODUCTION  = "production";
        public const string LOCALHOST   = "localhost";

        private static string defaultKey = "";
        private static Dictionary<string, iAPIInfo> urls  = new Dictionary<string, iAPIInfo>();


        public static iAPIInfo Default
        {
            get
            {
                if (urls.ContainsKey(defaultKey))
                {
                    return urls[defaultKey];
                }

                return null;
            }
        }


        public static bool CreateFromConfig (string configPath = "iConfig.json")
        {
            if (!File.Exists(configPath)) return false;

            string json = File.ReadAllText(configPath);

            iConfig config = JsonConvert.DeserializeObject<iConfig>(json);

            if (config.api != null)
            {
                if (config.api.infos != null)
                {
                    foreach(var info in config.api.infos)
                    {
                        Create(info.host, info.port, info.version, info.key, info.isDefault);
                    }
                }
            }

            return true;
        }


        public static iAPIInfo Create(string host, int port, int version = 1, string key = DEVELOPMENT, bool isDefault = false)
        {
            if (isDefault || urls.Count == 0)
            {
                defaultKey = key;
            }

            while (host.EndsWith("/"))
            {
                host = host.Remove(host.Length - 1);
            }

            iAPIInfo info  = new iAPIInfo();
            info.host      = host;
            info.port      = port;
            info.key       = key;
            info.isDefault = isDefault;
            info.version   = (version == 0) ? 1 : version;

            urls[key] = info;
            return info;
        }


        public static bool SetDefault(string key)
        {
            if (urls.ContainsKey(key))
            {
                defaultKey = key;

                foreach(var kv in urls) { kv.Value.isDefault = kv.Key == key; }

                return true;
            }
            return false;
        }
    }
}