using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;

namespace Dlugin
{
    public static class AdsEventReporter
    {
        public static void ReportBidding(
            string       adProviderName,
            string       adUnitId,
            string       adUnitType,
            double       maxRevenue,
            double       levelPlayRevenue,
            double       uaRevenue,
            string       adCreativeId,
            double       countryEcpmFloor,
            double       noOpEcpmFloor,
            bool         uaBidding,
            double       splitterRate,
            string       extraData,
            string       winInstanceId,
            List<string> allInstanceId)
        {
            try
            {
                var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                var playerId      = storageCommon.PlayerId;
                var platform      = storageCommon.Platform;
                var country       = storageCommon.Country;

                var messages = new Dictionary<string, object>()
                {
                    { "name", adProviderName },
                    { "ad_unit_id", adUnitId },
                    { "ad_unit_type", adUnitType },
                    { "m_revenue", maxRevenue },
                    { "lp_revenue", levelPlayRevenue },
                    { "ua_revenue", uaRevenue },
                    { "ua_bidding", uaBidding },
                    { "creative_id", adCreativeId },
                    { "country_ecpm_floor", countryEcpmFloor },
                    { "no_op_ecpm_floor", noOpEcpmFloor },
                    { "player_id", playerId },
                    { "platform", platform },
                    { "country", country },
                    { "splitter_rate", splitterRate },
                    { "instance_id", winInstanceId },
                };

                if (allInstanceId != null)
                {
                    messages.Add("all_instance_id", allInstanceId);
                }

                try
                {
                    messages.Add("extra_data", !string.IsNullOrEmpty(extraData)
                        ? JsonConvert.DeserializeObject(extraData)
                        : new Dictionary<string, object>());
                }
                catch (Exception exception)
                {
                    DebugUtil.LogError(exception.Message);
                }

                var info = JsonConvert.SerializeObject(messages);
                SDK.GetInstance().adjustPlugin.TrackEvent("ua_event1", 0, info);

                DebugUtil.Log($"AdsManager : Report bidding: {info}");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsManager : Report bidding failed: {exception.Message}");
            }
        }

        public static void ReportAdsQueryDisposing(
            string      adProviderName,
            string      adUnitId,
            string      adUnitType,
            double      revenue,
            string      revenuePrecision,
            string      adCreativeId,
            string      networkName,
            double      userEcpmFloor,
            double      disposeRate,
            bool        disposed,
            double      countryEcpmFloor,
            AdExtraData extraData,
            string      instanceId)
        {
            try
            {
                var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
                var playerId      = storageCommon.PlayerId;
                var platform      = storageCommon.Platform;
                var country       = storageCommon.Country;

                var messages = new Dictionary<string, object>
                {
                    { "name", adProviderName },
                    { "ad_unit_id", adUnitId },
                    { "ad_unit_type", adUnitType },
                    { "revenue", revenue },
                    { "revenue_precision", revenuePrecision },
                    { "creative_id", adCreativeId },
                    { "network_name", networkName },
                    { "user_ecpm_floor", userEcpmFloor },
                    { "country_ecpm_floor", countryEcpmFloor },
                    { "player_id", playerId },
                    { "platform", platform },
                    { "country", country },
                    { "dispose_rate", disposeRate },
                    { "disposed", disposed },
                    { "instance_id", instanceId },
                };

                if (extraData != null)
                {
                    var dict = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(extraData.ModelId))
                    {
                        dict.Add("model_id", extraData.ModelId);
                    }

                    messages.Add("extra_data", dict);
                }
                else
                {
                    messages.Add("extra_data", new Dictionary<string, object>());
                }

                var info = JsonConvert.SerializeObject(messages);
                SDK.GetInstance().adjustPlugin.TrackEvent("ua_event2", 0, info);

                DebugUtil.Log($"AdsManager : Report ads query disposing: {info}");
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsManager : Report ads query disposing failed: {exception.Message}");
            }
        }
    }
}