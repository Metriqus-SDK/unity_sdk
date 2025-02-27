namespace MetriqusSdk
{
    /// <summary>
    /// Represents ad revenue from applovin network, including source, earnings, currency, impressions, and network details.
    /// </summary>
    public class MetriqusAdmobAdRevenue : MetriqusAdRevenue
    {
        private const string source = "google admob";
        public MetriqusAdmobAdRevenue() : base(source)
        {
                   
        }

        public MetriqusAdmobAdRevenue(double revenue, string currency) : base(source, revenue, currency)
        {
        }
    }
}