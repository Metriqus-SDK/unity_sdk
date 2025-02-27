namespace MetriqusSdk
{
    /// <summary>
    /// Encironment of project operates
    /// Sandbox: App in testing/editor
    /// Production: App is in production
    /// </summary>
    [System.Serializable]
    public enum MetriqusEnvironment
    {
        Sandbox,
        Production
    }

    public static class MetriqusEnvironmentExtension
    {
        public static string ToLowercaseString(this MetriqusEnvironment metriqusEnvironment)
        {
            switch (metriqusEnvironment)
            {
                case MetriqusEnvironment.Sandbox:
                    return "sandbox";
                case MetriqusEnvironment.Production:
                    return "production";
                default:
                    return "unknown";
            }
        }
    }
}