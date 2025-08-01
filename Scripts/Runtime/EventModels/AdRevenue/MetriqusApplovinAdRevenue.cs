namespace MetriqusSdk
{
    /// <summary>
    /// Represents ad revenue from applovin network, including source, earnings, currency, impressions, and network details.
    /// </summary>
    public class MetriqusApplovinAdRevenue : MetriqusAdRevenue
    {
        private const string source = "applovin";

        public MetriqusApplovinAdRevenue(MetriqusAdUnitType adUnitType) : base(source, adUnitType)
        {
                   
        }

        public MetriqusApplovinAdRevenue(MetriqusAdUnitType adUnitType, double revenue, string currency) : base(source, adUnitType, revenue, currency)
        {
        }
    }
}