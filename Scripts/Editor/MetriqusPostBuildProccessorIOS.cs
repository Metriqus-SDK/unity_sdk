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

            AddAdvertisingAttributionReportEndpoint(plist);

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
            // Ensure NSUserTrackingUsageDescription is set or updated
            const string trackingDescriptionKey = "NSUserTrackingUsageDescription";

            string description = string.IsNullOrEmpty(MetriqusSettings.Instance.iOSUserTrackingUsageDescription)
                ? "We use your data to show personalized ads."
                : MetriqusSettings.Instance.iOSUserTrackingUsageDescription;

            plist.root.SetString(trackingDescriptionKey, description);
        }

        private static void AddAdvertisingAttributionReportEndpoint(PlistDocument plist)
        {
            // Ensure NSUserTrackingUsageDescription is set
            const string attributionReportEndpoint = "NSAdvertisingAttributionReportEndpoint";
            plist.root.SetString(attributionReportEndpoint, "https://mtrqs.com");
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

            // clear existing elements
            skAdNetworkArray.values.Clear();

            AddAdNetworkKey(skAdNetworkArray, "4fzdc2evr5.skadnetwork"); // Aarki
            AddAdNetworkKey(skAdNetworkArray, "4pfyvq9l8r.skadnetwork"); // AdColony Inc.
            AddAdNetworkKey(skAdNetworkArray, "ydx93a7ass.skadnetwork"); // Adikteev
            AddAdNetworkKey(skAdNetworkArray, "tmhh9296z4.skadnetwork"); // Adkomo
            AddAdNetworkKey(skAdNetworkArray, "488r3q3dtq.skadnetwork"); // Adtiming technology PTE. LTD.
            AddAdNetworkKey(skAdNetworkArray, "nzq8sh4pbs.skadnetwork"); // Adzmedia
            AddAdNetworkKey(skAdNetworkArray, "v72qych5uu.skadnetwork"); // Appier
            AddAdNetworkKey(skAdNetworkArray, "6xzpu9s2p8.skadnetwork"); // Applift
            AddAdNetworkKey(skAdNetworkArray, "ludvb6z3bs.skadnetwork"); // AppLovin
            AddAdNetworkKey(skAdNetworkArray, "mlmmfzh3r3.skadnetwork"); // Appreciate
            AddAdNetworkKey(skAdNetworkArray, "lr83yxwka7.skadnetwork"); // Apptimus LTD
            AddAdNetworkKey(skAdNetworkArray, "cp8zw746q7.skadnetwork"); // Arpeely
            AddAdNetworkKey(skAdNetworkArray, "c6k4g5qg8m.skadnetwork"); // Beeswax
            AddAdNetworkKey(skAdNetworkArray, "wg4vff78zm.skadnetwork"); // Bidmachine
            AddAdNetworkKey(skAdNetworkArray, "3sh42y64q3.skadnetwork"); // Centro
            AddAdNetworkKey(skAdNetworkArray, "f38h382jlk.skadnetwork"); // Chartboost
            AddAdNetworkKey(skAdNetworkArray, "hs6bdukanm.skadnetwork"); // Criteo
            AddAdNetworkKey(skAdNetworkArray, "9rd848q2bz.skadnetwork"); // Criteo SA (Manage.com)
            AddAdNetworkKey(skAdNetworkArray, "prcb7njmu6.skadnetwork"); // CrossInstall
            AddAdNetworkKey(skAdNetworkArray, "52fl2v3hgk.skadnetwork"); // Curate
            AddAdNetworkKey(skAdNetworkArray, "m8dbw4sv7c.skadnetwork"); // Dataseat
            AddAdNetworkKey(skAdNetworkArray, "m5mvw97r93.skadnetwork"); // Discipline Digital
            AddAdNetworkKey(skAdNetworkArray, "v9wttpbfk9.skadnetwork"); // Facebook Audience Network 1
            AddAdNetworkKey(skAdNetworkArray, "n38lu8286q.skadnetwork"); // Facebook Audience Network 2
            AddAdNetworkKey(skAdNetworkArray, "fz2k2k5tej.skadnetwork"); // FeedMob
            AddAdNetworkKey(skAdNetworkArray, "g2y4y55b64.skadnetwork"); // GlobalWide Media Ltd
            AddAdNetworkKey(skAdNetworkArray, "cstr6suwn9.skadnetwork"); // Google AdMob
            AddAdNetworkKey(skAdNetworkArray, "w9q455wk68.skadnetwork"); // Hybrid
            AddAdNetworkKey(skAdNetworkArray, "wzmmz9fp6w.skadnetwork"); // InMobi
            AddAdNetworkKey(skAdNetworkArray, "su67r6k2v3.skadnetwork"); // ironSource from Unity
            AddAdNetworkKey(skAdNetworkArray, "yclnxrl5pm.skadnetwork"); // Jampp
            AddAdNetworkKey(skAdNetworkArray, "4468km3ulz.skadnetwork"); // Kayzen
            AddAdNetworkKey(skAdNetworkArray, "v79kvwwj4g.skadnetwork"); // Kidoz Ltd.
            AddAdNetworkKey(skAdNetworkArray, "t38b2kh725.skadnetwork"); // Lifestreet
            AddAdNetworkKey(skAdNetworkArray, "7ug5zh24hu.skadnetwork"); // Liftoff
            AddAdNetworkKey(skAdNetworkArray, "5lm9lj6jb7.skadnetwork"); // Loopme
            AddAdNetworkKey(skAdNetworkArray, "zmvfpc5aq8.skadnetwork"); // Maiden Marketing Pvt Ltd.
            AddAdNetworkKey(skAdNetworkArray, "kbd757ywx3.skadnetwork"); // Mintegral
            AddAdNetworkKey(skAdNetworkArray, "ns5j362hk7.skadnetwork"); // Mobrand
            AddAdNetworkKey(skAdNetworkArray, "275upjj5gd.skadnetwork"); // Mobupps
            AddAdNetworkKey(skAdNetworkArray, "9t245vhmpl.skadnetwork"); // Moloco
            AddAdNetworkKey(skAdNetworkArray, "cad8qz2s3j.skadnetwork"); // MYAPPFREE S.P.A.
            AddAdNetworkKey(skAdNetworkArray, "a2p9lx4jpn.skadnetwork"); // Opera
            AddAdNetworkKey(skAdNetworkArray, "238da6jt44.skadnetwork"); // Pangle/Bytedance-China
            AddAdNetworkKey(skAdNetworkArray, "22mmun2rn5.skadnetwork"); // Pangle/Bytedance-Non china
            AddAdNetworkKey(skAdNetworkArray, "44jx6755aq.skadnetwork"); // Persona.ly
            AddAdNetworkKey(skAdNetworkArray, "tl55sbb4fm.skadnetwork"); // PubNative GMBH
            AddAdNetworkKey(skAdNetworkArray, "24zw6aqk47.skadnetwork"); // Qverse
            AddAdNetworkKey(skAdNetworkArray, "2u9pt9hc89.skadnetwork"); // Remerge
            AddAdNetworkKey(skAdNetworkArray, "8s468mfl3y.skadnetwork"); // RTB House
            AddAdNetworkKey(skAdNetworkArray, "glqzh8vgby.skadnetwork"); // Sabio Mobile Inc.
            AddAdNetworkKey(skAdNetworkArray, "av6w8kgt66.skadnetwork"); // ScaleMonk
            AddAdNetworkKey(skAdNetworkArray, "klf5c3l5u5.skadnetwork"); // Sift
            AddAdNetworkKey(skAdNetworkArray, "ppxm28t8ap.skadnetwork"); // Smadex
            AddAdNetworkKey(skAdNetworkArray, "424m5254lk.skadnetwork"); // Snap Inc.
            AddAdNetworkKey(skAdNetworkArray, "44n7hlldy6.skadnetwork"); // Spyke Media
            AddAdNetworkKey(skAdNetworkArray, "ecpz2srf59.skadnetwork"); // TapJoy
            AddAdNetworkKey(skAdNetworkArray, "pwa73g5rt2.skadnetwork"); // Tremor
            AddAdNetworkKey(skAdNetworkArray, "4dzt52r2t5.skadnetwork"); // Unity Technologies
            AddAdNetworkKey(skAdNetworkArray, "bvpn9ufa9b.skadnetwork"); // Unity Technologies
            AddAdNetworkKey(skAdNetworkArray, "gta9lk7p23.skadnetwork"); // Vungle
            AddAdNetworkKey(skAdNetworkArray, "3rd42ekr43.skadnetwork"); // YouAppi

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