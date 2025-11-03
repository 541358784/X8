
using System.Collections.Generic;
using System.Linq;
namespace DragonU3DSDK.Network.API {
    public class APIConfig {
        public static Dictionary<string, APIEntry> APIEntries = new Dictionary<string, APIEntry>(); 
        public static void Load() {
            APIEntries.Add("CHeartBeat", new APIEntry {
                uri = "/api/heart_beat",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetConfig", new APIEntry {
                uri = "/client_config",
                method = "GET",
                scheme = "http",
                timeout = 10,
                gzip = false,
                ignoreAuth = true,
            });
            APIEntries.Add("CSendEvents", new APIEntry {
                uri = "/bi",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = true,
                ignoreAuth = true,
            });
            APIEntries.Add("CLogin", new APIEntry {
                uri = "/api/account/login",
                method = "POST",
                scheme = "https",
                timeout = 20,
                gzip = false,
                ignoreAuth = true,
            });
            APIEntries.Add("CBindFacebook", new APIEntry {
                uri = "/api/account/facebook",
                method = "POST",
                scheme = "https",
                timeout = 20,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CUnbindFacebook", new APIEntry {
                uri = "/api/account/facebook/logout",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CBindEmail", new APIEntry {
                uri = "/api/account/email",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CUnbindEmail", new APIEntry {
                uri = "/api/account/email/logout",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CBindEmailVerify", new APIEntry {
                uri = "/api/account/email_verify",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CUnbindEmailVerify", new APIEntry {
                uri = "/api/account/email_verify/logout",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CSendEmailVerify", new APIEntry {
                uri = "/api/account/send_email_verify",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CBindApple", new APIEntry {
                uri = "/api/account/apple",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CUnbindApple", new APIEntry {
                uri = "/api/account/apple/logout",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CInviteFinished", new APIEntry {
                uri = "/api/account/invite_finished",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CListInvited", new APIEntry {
                uri = "/api/account/list_invited",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CBindFirebase", new APIEntry {
                uri = "/api/account/bind_firebase",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetPlayerProperties", new APIEntry {
                uri = "/api/account/player_properties",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CCheckDeleteAccount", new APIEntry {
                uri = "/api/account/check_delete",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CDeleteAccount", new APIEntry {
                uri = "/api/account/confirm_delete",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetProfile", new APIEntry {
                uri = "/api/profile",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CCreateProfile", new APIEntry {
                uri = "/api/profile",
                method = "POST",
                scheme = "https",
                timeout = 20,
                gzip = true,
                ignoreAuth = false,
            });
            APIEntries.Add("CUpdateProfile", new APIEntry {
                uri = "/api/profile",
                method = "PUT",
                scheme = "https",
                timeout = 60,
                gzip = true,
                ignoreAuth = false,
            });
            APIEntries.Add("CPreparePayment", new APIEntry {
                uri = "/api/payment/prepare",
                method = "POST",
                scheme = "https",
                timeout = 20,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CVerifyPayment", new APIEntry {
                uri = "/api/payment/verify",
                method = "POST",
                scheme = "https",
                timeout = 60,
                gzip = true,
                ignoreAuth = false,
            });
            APIEntries.Add("CFulfillPayment", new APIEntry {
                uri = "/api/payment/fulfill",
                method = "POST",
                scheme = "https",
                timeout = 20,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CListUnfulfilledPayments", new APIEntry {
                uri = "/api/payment/unfulfilled",
                method = "GET",
                scheme = "https",
                timeout = 20,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CCheckSubscription", new APIEntry {
                uri = "/api/payment/subscription",
                method = "GET",
                scheme = "https",
                timeout = 60,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CSendUserComplainMessage", new APIEntry {
                uri = "/api/user_complain",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CListUserComplainMessage", new APIEntry {
                uri = "/api/user_complain",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CCheckUserComplainMessage", new APIEntry {
                uri = "/api/user_complain/check",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CAutoSendUserComplainMessage", new APIEntry {
                uri = "/api/user_complain/auto",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CListMail", new APIEntry {
                uri = "/api/mail",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CClaimMail", new APIEntry {
                uri = "/api/mail/claim",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CCheckClaimMail", new APIEntry {
                uri = "/api/mail/check_claim",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CCheckClaimMails", new APIEntry {
                uri = "/api/mail/mul_check_claim",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CListGifts", new APIEntry {
                uri = "/api/gift",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CSendGifts", new APIEntry {
                uri = "/api/gift/send",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CClaimGifts", new APIEntry {
                uri = "/api/gift/claim",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CAskGifts", new APIEntry {
                uri = "/api/gift/ask",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CReplyGifts", new APIEntry {
                uri = "/api/gift/reply",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CClaimCoupon", new APIEntry {
                uri = "/api/coupon/claim",
                method = "POST",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetAdMediationConfigs", new APIEntry {
                uri = "/api/ad_mediation",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetActivities", new APIEntry {
                uri = "/api/activity",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetResActivities", new APIEntry {
                uri = "/api/activity/res_activity",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetActivityTimes", new APIEntry {
                uri = "/api/activity/time_activity",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CListAnnouncements", new APIEntry {
                uri = "/announcement",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = true,
            });
            APIEntries.Add("CUploadFile", new APIEntry {
                uri = "/api/helpers/upload_file",
                method = "POST",
                scheme = "https",
                timeout = 30,
                gzip = true,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetBundles", new APIEntry {
                uri = "/api/bundle_activity",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetResBundles", new APIEntry {
                uri = "/api/bundle_activity/res_bundles",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetNewResBundles", new APIEntry {
                uri = "/api/bundle_activity/new_res_bundles",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetShopConfig", new APIEntry {
                uri = "/api/shop",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CCheckSubPayments", new APIEntry {
                uri = "/api/payment/check_subscription",
                method = "GET",
                scheme = "https",
                timeout = 60,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetTestGroups", new APIEntry {
                uri = "/api/account/test_groups",
                method = "GET",
                scheme = "https",
                timeout = 60,
                gzip = false,
                ignoreAuth = false,
            });
            APIEntries.Add("CGetSegmentationConfig", new APIEntry {
                uri = "/api/segmentation_config",
                method = "GET",
                scheme = "https",
                timeout = 10,
                gzip = false,
                ignoreAuth = false,
            });
        }

        static APIConfig()
        {
            Load();

            var clazz = ConfigurationController.Instance.APIConfigClassName;
            if (!string.IsNullOrEmpty(clazz))
            {
                var type = System.Reflection.Assembly.GetExecutingAssembly().GetType("DragonU3DSDK.Network.API." + clazz);
                if (type != null && type.IsSubclassOf(typeof(APIConfig)))
                {
                    var method = type.GetMethod("Load");
                    if (method != null)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
            else
            {
                var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(APIConfig)));
                foreach (var type in types)
                {
                    var method = type.GetMethod("Load");
                    if (method != null)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
