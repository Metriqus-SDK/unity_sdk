namespace MetriqusSdk
{
    public static class MetriqusAdUnit
    {
        public const string Banner = "banner";
        public const string Interstitial = "interstitial";
        public const string Rewarded = "rewarded";
        public const string RewardedInterstitial = "rewarded_interstitial";
        public const string NativeAdvenced = "native_advenced";
        public const string AppOpen = "app_open";

        public static string AdUnitToString(MetriqusAdUnitType type)
        {
            switch (type)
            {
                case MetriqusAdUnitType.Banner:
                    return Banner;
                case MetriqusAdUnitType.Interstitial:
                    return Interstitial;
                case MetriqusAdUnitType.Rewarded:
                    return Rewarded;
                case MetriqusAdUnitType.RewardedInterstitial:
                    return RewardedInterstitial;
                case MetriqusAdUnitType.NativeAdvenced:
                    return NativeAdvenced;
                case MetriqusAdUnitType.AppOpen:
                    return AppOpen;
            }

            return "";
        }
    }

    public enum MetriqusAdUnitType
    {
        Banner, Interstitial, Rewarded, RewardedInterstitial, NativeAdvenced, AppOpen
    }
}