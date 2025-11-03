//this code is auto generated, DO NOT CHANGE IT MANUALLY, please use "node GenConstant.js".

namespace Dlugin
{
    public static class Constants
	{
		//all service define
		public const int kUnknownService = 0;
		public const int kOSUtilService = 1;
		public const int kPayService = 2;
		public const int kUserLoginService = 3;
		public const int kAdsService = 4;
		public const int kDataStatService = 5;
		public const int kLogService = 6;
		
		//all common error code
		public const int kErrorSuccess = 0;
		public const int kErrorHasInProgress = 1;
		public const int kErrorNetworkError = 2;
		public const int kErrorUserCanceled = 3;
		public const int kErrorParameterError = 4;
		public const int kErrorUnknown = 5;
		public const int kErrorPluginMalformed = 6;
		public const int kErrorNotSupported = 7;
		public const int kErrorChannelError = 8;
		public const int kErrorNotInitialize = 9;
		public const int kErrorNotLogin = 10;
		public const int kErrorTokenExpired = 11;
		public const int kErrorTargetNotFound = 12;
		public const int kErrorDelayed = 13;
		public const int kErrorNotReady = 14;
		public const int kErrorAppleCredentialsRevoked = 15;

		//all user login status
		public const int kLoginStatusNotLogin = 0;
		public const int kLoginStatusGuestLogin = 1;
		public const int kLoginStatusUserLogin = 2;
		public const int kLoginStatusUnknown = 3;
		
		//all user permission define
		public const int kUserPermissionLoginToken = 1;
		public const int kUserPermissionBasicInfo = 2;
		public const int kUserPermissionPublish = 3;
		public const int kUserPermissionFriendList = 4;
		
		//all iap product types
		public const int kProductTypeSubscript = 1;
		public const int kProductTypeConsume = 2;
		public const int kProductTypeUnknown = 3;
		
		//all delimiters
		public const char kStructDelimiter = (char)1;
		public const char kUnitySendMessageDelimiter = (char)2;
		
		//for UnitySendMessage
		public const string kMessageReceiverName = "__PLUGIN_RECEIVER__";
		public const string kMessageReceiverMethod = "OnPluginMessage";
		public const int kMessageTypeLoginOver = 1;
		public const int kMessageTypeLogoutOver = 2;
		public const int kMessageTypeRefreshUserInfoOver = 3;
		public const int kMessageTypeChangePermissionOver = 4;
		public const int kMessageTypePayOver = 5;
		public const int kMessageTypeRestoreOver = 6;
		public const int kMessageTypeConsumeOver = 7;
		public const int kMessageTypeGetUnfinishedOver = 8;
		public const int kMessageTypeGetProductListOver = 9;
		public const int kMessageTypeAdsLoaded = 10;
		public const int kMessageTypeAdsWatched = 11;
		public const int kMessageTypeAdsPlayFinished = 12;
		public const int kMessageTypeAdsAvailabilityChanged = 13;
		public const int kMessageTypeNetworkStatusChanged = 14;
		
		//for ads interface, the type of ads
		public const int kAdsTypeUnknown = 0;
		public const int kAdsTypeInterstitial = 1;
		public const int kAdsTypeRewardVideo = 2;
		public const int kAdsTypeCrossPromotion = 3;
		public const int kAdsTypeBanner = 4;
		public const int kAdsTypeOfferWall = 5;
		public const int kAdsTypeCount = 6;

		//for event statistic interface, the type of common event
		public const string kEventTypePurchase = "inAppPurchase";
		public const string kEventTypeLogin = "login";
		public const string kEventTypeLaunch = "launchApp";
		
		//for log service, all urgent level of the log
		public const int kUrgentLevelCrash = 1;
		public const int kUrgentLevelError = 2;
		public const int kUrgentLevelWarning = 3;
		public const int kUrgentLevelDefect = 4;
		
		//the enum for network conditions
		public const int kNetworkStatusUnknown = 0;
		public const int kNetworkStatusNoNetwork = 1;
		public const int kNetworkStatusCellular = 2;
		public const int kNetworkStatusWifi = 3;
		
		//the privacy for social feature
		public const int kSocialPrivacyEveryone = 1;
		public const int kSocialPrivacyFriendOnly = 2;
		public const int kSocialPrivacyMeOnly = 3;

        //plugin Name
        public const string Admob = "Admob";
        public const string AdColony = "AdColony";
        public const string Audience = "AudienceNetWork";
        public const string Chartboost = "Chartboost";
        public const string AppLovin = "AppLovin";
        public const string Adjust = "Adjust";
        public const string IronSource = "IronSource";
        public const string UnityAds = "UnityAds";
        public const string FaceBook = "FaceBook";
        public const string FireBase = "FireBase";
        public const string OneSignal = "OneSignal";
        public const string MAX = "MAX";
        public const string UA = "UA";
        public const string HybridAds = "HybridAds";
		public const string Apple = "Apple";
		public const string Tapjoy = "Tapjoy";
		public const string APT = "AndroidPerformanceTuner";
		public const string APS = "APS";
		
		//for ad plugins
		public const float GLOBAL_AD_REQUEST_CD = 30.0f;

        public static string[] Placements = { "InTheMap", "GameFailed" }; 
    }
}
