#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MetriqusSdk
{
    public class MetriqusPreBuildProccessorAndroid : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0; // Run early in the build process

        private readonly string gradlePath = "Assets/Plugins/Android/mainTemplate.gradle";

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
                return; // Only modify Gradle for Android builds

            if (!File.Exists(gradlePath))
            {
                Debug.LogError("[Metriqus] mainTemplate.gradle not found! Make sure 'Custom Main Gradle Template' is enabled in Player Settings/Publishing Settings.");
                return;
            }

            string gradleContent = File.ReadAllText(gradlePath);

            // Dependencies to add
            string[] dependencies =
            {
                "implementation 'com.android.installreferrer:installreferrer:2.2'",
                "implementation 'com.google.android.gms:play-services-ads-identifier:18.2.0'"
            };

            bool modified = false;

            foreach (string dependency in dependencies)
            {
                if (!gradleContent.Contains(dependency))
                {
                    gradleContent = gradleContent.Replace("dependencies {", $"dependencies {{\n    {dependency}");
                    modified = true;
                }
            }

            if (modified)
            {
                File.WriteAllText(gradlePath, gradleContent);
                Debug.Log("[Metriqus] Dependencies added to mainTemplate.gradle.");
            }
            else
            {
                Debug.Log("[Metriqus] Dependencies already exist in mainTemplate.gradle.");
            }
        }
    }
}
#endif