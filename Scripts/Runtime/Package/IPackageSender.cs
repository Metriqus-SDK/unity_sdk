namespace MetriqusSdk
{
    internal interface IPackageSender
    {
        public void SendCustomPackage(MetriqusCustomEvent customEvent);
        internal void SendSessionStartPackage();
        internal void SendSessionBeatPackage();
        public void SendIAPEventPackage(MetriqusInAppRevenue metriqusEvent);
        public void SendAdRevenuePackage(MetriqusAdRevenue adRevenue);
        internal void SendAttributionPackage(MetriqusAttribution attribution);
    }
}