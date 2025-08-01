namespace MetriqusSdk
{
    /// <summary>
    /// Represents ad revenue data, including source, earnings, currency, impressions, and network details.
    /// </summary>
    public class MetriqusAdRevenue
    {
        /// <summary>
        /// The source of the ad revenue data (e.g., platform or provider).
        /// </summary>
        protected string Source { get; private set; }

        /// <summary>
        /// The revenue generated from ads, if available.
        /// </summary>
        public double? Revenue { get; private set; }

        /// <summary>
        /// The currency in which the revenue is reported.
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// The total number of ad impressions recorded, if available.
        /// </summary>
        public int? AdImpressionsCount { get; set; }

        /// <summary>
        /// The network through which the ad revenue was generated.
        /// </summary>
        public string AdRevenueNetwork { get; set; }

        /// <summary>
        /// The specific ad unit generating the revenue.
        /// </summary>
        public string AdRevenueUnit { get; set; }

        /// <summary>
        /// The placement of the ad that contributed to the revenue.
        /// </summary>
        public string AdRevenuePlacement { get; set; }

        public MetriqusAdRevenue()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusAdRevenue"/> class with the specified source.
        /// </summary>
        /// <param name="source">The source of the ad revenue data.</param>
        public MetriqusAdRevenue(string source, MetriqusAdUnitType adUnitType)
        {
            this.Source = source;

            this.AdRevenueUnit = MetriqusAdUnit.AdUnitToString(adUnitType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusAdRevenue"/> class with revenue details.
        /// </summary>
        /// <param name="source">The source of the ad revenue data.</param>
        /// <param name="revenue">The revenue generated from ads.</param>
        /// <param name="currency">The currency in which the revenue is reported.</param>
        public MetriqusAdRevenue(string source, MetriqusAdUnitType adUnitType, double revenue, string currency)
        {
            this.Source = source;
            this.Revenue = revenue;
            this.Currency = currency;

            this.AdRevenueUnit = MetriqusAdUnit.AdUnitToString(adUnitType);
        }

        public string GetSource() => Source;

        /// <summary>
        /// Sets or updates the revenue and currency for this ad revenue record.
        /// </summary>
        /// <param name="revenue">The revenue generated from ads.</param>
        /// <param name="currency">The currency in which the revenue is reported.</param>
        public void SetRevenue(double revenue, string currency)
        {
            this.Revenue = revenue;
            this.Currency = currency;
        }
    }
}