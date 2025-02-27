using System;

namespace MetriqusSdk
{
    internal class MetriqusPackageSender : IPackageSender
    {

        void IPackageSender.SendCustomPackage(MetriqusCustomEvent customEvent)
        {
            PackageBuilder builder = new PackageBuilder(Metriqus.GetMetriqusSettings(), Metriqus.GetDeviceInfo(), DateTime.UtcNow);
            Package package = builder.BuildCustomEventPackage(customEvent);

            MetriqusLogger.LogEvent(package);

            if (Metriqus.LogLevel == LogLevel.Verbose)
                Metriqus.DebugLog("Sending Custom Event Package. \n" + package.ToString());
        }

        void IPackageSender.SendSessionStartPackage()
        {
            PackageBuilder builder = new PackageBuilder(Metriqus.GetMetriqusSettings(), Metriqus.GetDeviceInfo(), DateTime.UtcNow);
            Package package = builder.BuildSessionStartPackage();

            MetriqusLogger.LogEvent(package);

            if (Metriqus.LogLevel == LogLevel.Verbose)
                Metriqus.DebugLog("Sending Session Package. \n" + package.ToString());
        }

        void IPackageSender.SendSessionBeatPackage()
        {
            PackageBuilder builder = new PackageBuilder(Metriqus.GetMetriqusSettings(), Metriqus.GetDeviceInfo(), DateTime.UtcNow);
            Package package = builder.BuildSessionBeatPackage();

            MetriqusLogger.LogEvent(package);

            if (Metriqus.LogLevel == LogLevel.Verbose)
                Metriqus.DebugLog("Sending Session Beat Package. \n" + package.ToString());
        }

        void IPackageSender.SendIAPEventPackage(MetriqusInAppRevenue metriqusEvent)
        {
            PackageBuilder builder = new PackageBuilder(Metriqus.GetMetriqusSettings(), Metriqus.GetDeviceInfo(), DateTime.UtcNow);
            Package package = builder.BuildIAPEventPackage(metriqusEvent);

            MetriqusLogger.LogEvent(package);

            if (Metriqus.LogLevel == LogLevel.Verbose)
                Metriqus.DebugLog("Sending IAP Event Package. \n" + package.ToString());
        }

        void IPackageSender.SendAdRevenuePackage(MetriqusAdRevenue adRevenue)
        {
            PackageBuilder builder = new PackageBuilder(Metriqus.GetMetriqusSettings(), Metriqus.GetDeviceInfo(), DateTime.UtcNow);
            Package package = builder.BuildAdRevenueEventPackage(adRevenue);

            MetriqusLogger.LogEvent(package);

            if (Metriqus.LogLevel == LogLevel.Verbose)
                Metriqus.DebugLog("Sending Event Package. \n" + package.ToString());
        }

        void IPackageSender.SendAttributionPackage(MetriqusAttribution attribution)
        {
            PackageBuilder builder = new PackageBuilder(Metriqus.GetMetriqusSettings(), Metriqus.GetDeviceInfo(), DateTime.UtcNow);
            Package package = builder.BuildAttributionPackage(attribution);

            MetriqusLogger.LogEvent(package);

            if (Metriqus.LogLevel == LogLevel.Verbose)
                Metriqus.DebugLog("Sending attribution Package. \n" + package.ToString());
        }
    }
}