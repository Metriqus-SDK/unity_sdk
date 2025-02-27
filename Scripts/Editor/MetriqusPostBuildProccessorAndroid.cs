#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MetriqusSdk
{
    public class MetriqusPostBuildProccessorAndroid : IPostprocessBuildWithReport
    {
        public int callbackOrder => 999;

#if UNITY_2018_1_OR_NEWER
        public void OnPostprocessBuild(BuildReport report)
        {
            bool isExportingProject = (report.summary.options & BuildOptions.AcceptExternalModificationsToPlayer) != 0;

            if (isExportingProject)
                OnPostprocessBuild(report.summary.platform, report.summary.outputPath);
        }
#endif

        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                string gradlePath = Path.Combine(path, "launcher", "build.gradle");

                if (File.Exists(gradlePath))
                {
                    string gradleContent = File.ReadAllText(gradlePath);

                    // Dependency to add
                    string installreferrerDep = "implementation 'com.android.installreferrer:installreferrer:2.2'";
                    string playServicesAdsIdentifierDep = "implementation 'com.google.android.gms:play-services-ads-identifier:18.2.0'";

                    AddAndroidDependency(ref gradleContent, installreferrerDep);
                    AddAndroidDependency(ref gradleContent, playServicesAdsIdentifierDep);

                    File.WriteAllText(gradlePath, gradleContent);

                }
                else
                {
                    Debug.Log("[Metriqus] Could not find build.gradle at " + gradlePath);
                }
            }
        }

        private static void AddAndroidDependency(ref string gradleContent, string dependency)
        {
            // Check if dependency exists
            if (!gradleContent.Contains(dependency))
            {
                gradleContent = gradleContent.Replace("dependencies {", $"dependencies {{\n    {dependency}\n");
                Debug.Log($"[Metriqus] Android dependency({dependency}) added to build.gradle");
            }
            else
            {
                Debug.Log($"[Metriqus] Android dependency({dependency}) already exist");
            }
        }
    }
}
#endif