namespace DragonPlus.Ad.UA
{
    internal static class UaSdkCallbackEvents
    {
        public const string OnInitialCallbackEvent = "OnInitialCallbackEvent";
        public const string OnSdkInitializedEvent  = "OnSdkInitializedEvent";

        public const string OnBannerAdLoadedEvent      = "OnBannerAdLoadedEvent";
        public const string OnBannerAdLoadFailedEvent  = "OnBannerAdLoadFailedEvent";
        public const string OnBannerAdClickedEvent     = "OnBannerAdClickedEvent";
        public const string OnBannerAdRevenuePaidEvent = "OnBannerAdRevenuePaidEvent";

        public const string OnInterstitialLoadedEvent            = "OnInterstitialLoadedEvent";
        public const string OnInterstitialLoadFailedEvent        = "OnInterstitialLoadFailedEvent";
        public const string OnInterstitialHiddenEvent            = "OnInterstitialHiddenEvent";
        public const string OnInterstitialDisplayedEvent         = "OnInterstitialDisplayedEvent";
        public const string OnInterstitialAdFailedToDisplayEvent = "OnInterstitialAdFailedToDisplayEvent";
        public const string OnInterstitialClickedEvent           = "OnInterstitialClickedEvent";
        public const string OnInterstitialAdRevenuePaidEvent     = "OnInterstitialAdRevenuePaidEvent";

        public const string OnRewardedAdLoadedEvent          = "OnRewardedAdLoadedEvent";
        public const string OnRewardedAdLoadFailedEvent      = "OnRewardedAdLoadFailedEvent";
        public const string OnRewardedAdDisplayedEvent       = "OnRewardedAdDisplayedEvent";
        public const string OnRewardedAdHiddenEvent          = "OnRewardedAdHiddenEvent";
        public const string OnRewardedAdClickedEvent         = "OnRewardedAdClickedEvent";
        public const string OnRewardedAdRevenuePaidEvent     = "OnRewardedAdRevenuePaidEvent";
        public const string OnRewardedAdFailedToDisplayEvent = "OnRewardedAdFailedToDisplayEvent";
        public const string OnRewardedAdReceivedRewardEvent  = "OnRewardedAdReceivedRewardEvent";
    }
}