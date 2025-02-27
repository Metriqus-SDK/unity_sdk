using System;
using System.Text;

namespace MetriqusSdk
{
    public class AppInfoPackage
    {
        public const string PackageNameKey = "package_name";
        public const string AppVersionKey = "app_version";

        public string PackageName { get; set; }
        public string AppVersion { get; set; }

        public AppInfoPackage(string packageName, string appVersion)
        {
            this.PackageName = packageName;
            this.AppVersion = appVersion;
        }

        public string Serialize()
        {
            string result = "{\n";

            result += $"\"{PackageNameKey}\": \"{PackageName}\",\n";
            result += $"\"{AppVersionKey}\": \"{AppVersion}\"\n";

            result += "}";

            return result;
        }

        public static AppInfoPackage Deserialize(JSONNode jsonNode)
        {
            if (jsonNode == null)
            {
                return null;
            }

            string packageName, appVersion;

            try
            {
                packageName = MetriqusJSON.GetJsonString(jsonNode, PackageNameKey);
                appVersion = MetriqusJSON.GetJsonString(jsonNode, AppVersionKey);

                return new AppInfoPackage(packageName, appVersion);
            }
            catch (Exception e)
            {
                if (Metriqus.LogLevel != LogLevel.NoLog)
                {
                    Metriqus.DebugLog("Parsing AppInfoPackage Failed:\n" + e.ToString()
                        + "\n, json: " + jsonNode.ToString());
                }

                return null;
            }
        }
    }
}