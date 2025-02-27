namespace MetriqusSdk
{
    /// <summary>
    /// Represents ad revenue from applovin network, including source, earnings, currency, impressions, and network details.
    /// </summary>
    public class MetriqusApplovinAdRevenue : MetriqusAdRevenue
    {
        private const string source = "applovin";

        public MetriqusApplovinAdRevenue() : base(source)
        {
                   
        }

        public MetriqusApplovinAdRevenue(double revenue, string currency) : base(source, revenue, currency)
        {
        }
    }
}