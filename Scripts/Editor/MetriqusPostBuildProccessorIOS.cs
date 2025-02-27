#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace MetriqusSdk
{
    public class MetriqusPostBuildProccessorIOS : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

#if UNITY_2018_1_OR_NEWER
        public void OnPostprocessBuild(BuildReport report)
        {
            OnPostprocessBuild(report.summary.platform, report.summary.outputPath);
        }
#endif

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.iOS)
            {
                RunPostProcessTasks(target, path);
            }
        }

        public static void RunPostProcessTasks(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
                return;

            // Path to Info.plist
            string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

            // Load the Info.plist file
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            if (MetriqusSettings.Instance.iOSUserTrackingDisabled == false)
                AddTrackingUsageDescription(plist);

            // Add Ad network keys
            AddNetworkKeys(plist);

            // Write the changes back to the plist file
            plist.WriteToFile(plistPath);
            
            Debug.Log("Modified Info.plist successfully.");

            string xcodeProjectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
            PBXProject xcodeProject = new PBXProject();
            xcodeProject.ReadFromFile(xcodeProjectPath);

            // add required frameworks to project
            AddFrameworks(xcodeProject);

            xcodeProject.WriteToFile(xcodeProjectPath);
        }

        private static void AddTrackingUsageDescription(PlistDocument plist)
        {
            // Ensure NSUserTrackingUsageDescription is set
            const string trackingDescriptionKey = "NSUserTrackingUsageDescription";
            if (!plist.root.values.ContainsKey(trackingDescriptionKey))
            {
                plist.root.SetString(trackingDescriptionKey,
                    string.IsNullOrEmpty(MetriqusSettings.Instance.iOSUserTrackingUsageDescription) ?
                    "We use your data to show personalized ads."
                    : MetriqusSettings.Instance.iOSUserTrackingUsageDescription);
            }
        }

        private static void AddNetworkKeys(PlistDocument plist)
        {
            // Ensure SKAdNetworkItems exists
            const string skAdNetworkItemsKey = "SKAdNetworkItems";
            if (!plist.root.values.ContainsKey(skAdNetworkItemsKey))
            {
                plist.root.CreateArray(skAdNetworkItemsKey);
            }

            var skAdNetworkArray = plist.root[skAdNetworkItemsKey].AsArray();

            AddAdNetworkKey(skAdNetworkArray, "ca-app-pub-3940256099942544/2934735716"); // Google AdMob
            AddAdNetworkKey(skAdNetworkArray, "v6w7j5b7xr.skadnetwork"); // TikTok
            AddAdNetworkKey(skAdNetworkArray, "fbadnetwork"); // Facebook
            AddAdNetworkKey(skAdNetworkArray, "4fzdc2evr5.skadnetwork"); // Twitter
            AddAdNetworkKey(skAdNetworkArray, "prcb7njmu6.skadnetwork"); // Unity Ads
        }

        // Helper method to add a single ad network key if not already present
        private static void AddAdNetworkKey(PlistElementArray skAdNetworkArray, string adNetworkIdentifier)
        {
            foreach (PlistElementDict item in skAdNetworkArray.values)
            {
                if (item["SKAdNetworkIdentifier"].AsString() == adNetworkIdentifier)
                {
                    // Ad network key already exists
                    return;
                }
            }

            // Add a new dictionary entry for the ad network
            var newAdNetworkDict = skAdNetworkArray.AddDict();
            newAdNetworkDict.SetString("SKAdNetworkIdentifier", adNetworkIdentifier);
        }

        /// <summary>
        /// Utility function to add required frameworks to XCode project
        /// </summary>
        /// <param name="pathToBuiltProject"></param>
        private static void AddFrameworks(PBXProject xcodeProject)
        {

#if UNITY_2019_3_OR_NEWER
            string xcodeTarget = xcodeProject.GetUnityMainTargetGuid();
#else
            string xcodeTarget = xcodeProject.TargetGuidByName("Unity-iPhone");
#endif
            //AddFrameworksToProject(xcodeProject, xcodeTarget);

            if (!string.IsNullOrEmpty(xcodeTarget))
                AddFrameworksToProject(xcodeProject, xcodeProject.GetUnityFrameworkTargetGuid());

        }

        private static void AddFrameworksToProject(PBXProject xcodeProject, string xcodeTarget)
        {
            /*if (MetriqusSettings.Instance.iOSFrameworkAdSupport)
            {*/
            Debug.Log("[Metriqus]: Adding AdSupport.framework to Xcode project.");
            xcodeProject.AddFrameworkToProject(xcodeTarget, "AdSupport.framework", true);
            Debug.Log("[Metriqus]: AdSupport.framework added successfully.");
            /*}
            else
            {
                Debug.Log("[Metriqus]: Skipping AdSupport.framework linking.");
            }
            if (MetriqusSettings.Instance.iOSFrameworkAdServices)
            {*/
            Debug.Log("[Metriqus]: Adding AdServices.framework to Xcode project.");
            xcodeProject.AddFrameworkToProject(xcodeTarget, "AdServices.framework", true);
            Debug.Log("[Metriqus]: AdServices.framework added successfully.");
            /*}
            else
            {
                Debug.Log("[Metriqus]: Skipping AdServices.framework linking.");
            }
            if (MetriqusSettings.Instance.iOSFrameworkStoreKit)
            {*/
            Debug.Log("[Metriqus]: Adding StoreKit.framework to Xcode project.");
            xcodeProject.AddFrameworkToProject(xcodeTarget, "StoreKit.framework", true);
            Debug.Log("[Metriqus]: StoreKit.framework added successfully.");
            /*}
            else
            {
                Debug.Log("[Metriqus]: Skipping StoreKit.framework linking.");
            }
            if (MetriqusSettings.Instance.iOSFrameworkAppTrackingTransparency)
            {*/
            Debug.Log("[Metriqus]: Adding AppTrackingTransparency.framework to Xcode project.");
            xcodeProject.AddFrameworkToProject(xcodeTarget, "AppTrackingTransparency.framework", true);
            Debug.Log("[Metriqus]: AppTrackingTransparency.framework added successfully.");
            /*}
            else
            {
                Debug.Log("[Metriqus]: Skipping AppTrackingTransparency.framework linking.");
            }*/
        }
    }
}
#endif