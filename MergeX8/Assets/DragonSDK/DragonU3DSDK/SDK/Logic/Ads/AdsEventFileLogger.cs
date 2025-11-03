#if DEBUG || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dlugin.PluginStructs;
using DragonU3DSDK;
using Newtonsoft.Json;
using UnityEngine.Device;

namespace Dlugin
{
    public class AdsEventFileLogger
    {
        private const string     LogFilePath = "ads_event_logs";
        private       FileStream _logFileStream;

        public void Startup()
        {
            var now          = DateTime.Now;
            var logDirectory = Path.Combine(Application.persistentDataPath, LogFilePath);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var filename = Path.Combine(
                logDirectory,
                $"events_{now.Year:0000}{now.Month:00}{now.Day:00}_{now.Hour:00}{now.Minute:00}{now.Second:00}.log");

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            _logFileStream = new FileStream(filename, FileMode.CreateNew);
        }

        public void Shutdown()
        {
            _logFileStream?.Close();
            _logFileStream = null;
        }

        public void LogSplitterSettings(
            bool   uaSplitterEnabled,
            double uaSplitterRate,
            double uaSplitterEcpmFloor)
        {
            try
            {
                var message = new Dictionary<string, object>
                {
                    { "event_type", "ads_splitter_settings" },
                    { "splitter_enabled", uaSplitterEnabled },
                    { "splitter_rate", uaSplitterRate },
                    { "splitter_ecpm_floor", uaSplitterEcpmFloor }
                };

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogBiddingSettings(
            bool   adsDisposeEnabled,
            double adNoOpEcpmFloor,
            double reloadDelayWhenDisposed)
        {
            try
            {
                var message = new Dictionary<string, object>
                {
                    { "event_type", "ads_bidding_settings" },
                    { "dispose_enabled", adsDisposeEnabled },
                    { "no_op_ecpm_floor", adNoOpEcpmFloor },
                    { "reload_delay", reloadDelayWhenDisposed }
                };

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsLoaded(AdsUnitDefine adsUnitDefine)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type", "ads_loaded");

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsLoadFailed(AdsUnitDefine adsUnitDefine)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type", "ads_load_failed");

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsQueryBidding(AdsUnitDefine adsUnitDefine, double revenue, int playedCount)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type",      "ads_query_bidding");
                message.Add("bidding_revenue", revenue);
                message.Add("played_count",    playedCount);

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsQueryBiddingResult(
            AdsUnitDefine adsUnitDefine,
            bool          disposed,
            double        disposeRate,
            double        userEcpmFloor)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type",      "ads_query_bidding_result");
                message.Add("disposed",        disposed);
                message.Add("dispose_rate",    disposeRate);
                message.Add("user_ecpm_floor", userEcpmFloor);

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsDisposed(AdsUnitDefine adsUnitDefine)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type", "ads_disposed");

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsNoOp(
            double maxRevenue,
            double levelPlayRevenue,
            double uaSplitterEcpmFloor,
            double uaSplitterRate)
        {
            try
            {
                var message = new Dictionary<string, object>
                {
                    { "event_type", "ads_no_op" },
                    { "max_revenue", maxRevenue },
                    { "level_play_revenue", levelPlayRevenue },
                    { "splitter_ecpm_floor", uaSplitterEcpmFloor },
                    { "splitter_rate", uaSplitterRate }
                };

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsTryPlay(
            AdsUnitDefine adsUnitDefine,
            double        maxRevenue,
            double        levelPlayRevenue,
            double        uaRevenue,
            double        uaSplitterEcpmFloor,
            double        uaSplitterRate,
            double        noOpEcpmFloor,
            bool          uaBidding,
            bool          played)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type",          "ads_try_play");
                message.Add("max_revenue",         maxRevenue);
                message.Add("level_play_revenue",  levelPlayRevenue);
                message.Add("ua_revenue",          uaRevenue);
                message.Add("ua_bidding",          uaBidding);
                message.Add("splitter_ecpm_floor", uaSplitterEcpmFloor);
                message.Add("splitter_rate",       uaSplitterRate);
                message.Add("no_op_ecpm_floor",    noOpEcpmFloor);
                message.Add("played",              played);

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsDisplayed(AdsUnitDefine adsUnitDefine)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type", "ads_displayed");

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        public void LogAdsClicked(AdsUnitDefine adsUnitDefine)
        {
            try
            {
                var message = new Dictionary<string, object>();
                FillGenericMessage(adsUnitDefine, message);

                message.Add("event_type", "ads_clicked");

                Log(JsonConvert.SerializeObject(message));
            }
            catch (Exception exception)
            {
                DebugUtil.LogError($"AdsEventFileLogger : exception -> {exception.Message}");
            }
        }

        private void Log(string message)
        {
            var now = DateTime.Now;
            _logFileStream?.Write(
                Encoding.UTF8.GetBytes(
                    $"[{now.Year:0000}-{now.Month:00}-{now.Day:00} {now.Hour}:{now.Minute}:{now.Second}] {message}\n"));
            _logFileStream?.Flush();
        }

        private static void FillGenericMessage(AdsUnitDefine adsUnitDefine, Dictionary<string, object> message)
        {
            message.Add("plugin",            adsUnitDefine.m_PluginName);
            message.Add("placement",         adsUnitDefine.m_Placement);
            message.Add("network",           adsUnitDefine.m_NetworkName);
            message.Add("network_placement", adsUnitDefine.m_NetworkPlacement);
            message.Add("ad_unit_type",      adsUnitDefine.m_Type.ToString());
            message.Add("revenue",           adsUnitDefine.m_EcpmFloor);
            message.Add("revenue_precision", adsUnitDefine.m_RevenuePrecision);
            message.Add("ad_unit_id",        adsUnitDefine.m_AdUnitIdentifier);
            message.Add("creative_id",       adsUnitDefine.m_CreativeIdentifier);
            message.Add("dsp_name",          adsUnitDefine.m_DspName);
            message.Add("instance_id",       adsUnitDefine.m_InstanceId);
        }
    }
}
#endif