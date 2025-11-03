using System.Collections.Generic;
using System.Text;
using Dlugin.PluginStructs;
using DragonU3DSDK;

namespace Dlugin
{
    //广告调度器　给定已加载的广告内容，返回应播放的广告
    public static class AdDispatcher
    {
        public static AdsUnitDefine PickAdsUnitDefine(
            List<AdsUnitDefine> sources,
            double              ecpmFloor,
            bool                uaBidding,
            bool                uaSplitterEnabled,
            bool                removeOriginAdsUnit = true)
        {
            if (sources == null || sources.Count == 0)
            {
                return null;
            }

            SortAdUnits(sources);

            var targetAd = sources[0];
            if (targetAd.m_EcpmFloor < ecpmFloor)
            {
                return null;
            }

            // check uaBidding and usSplitterEnabled when ua ads found
            if (targetAd.m_PluginName == Constants.UA)
            {
                if (!uaBidding || !uaSplitterEnabled)
                {
                    
#if DEVELOPMENT_BUILD || DEBUG
                    DebugUtil.Log($"AdsManager : Try find other ads because of usBiding => {uaBidding}, uaSplitterEnabled => {uaSplitterEnabled}"); 
#endif
                    
                    // find other valid ads except UA
                    var count = sources.Count;
                    for (var i = 1; i < count; i++)
                    {
                        if (sources[i].m_PluginName != Constants.UA &&
                            sources[i].m_EcpmFloor  >= ecpmFloor)
                        {
                            targetAd = sources[i];
                            sources.RemoveAt(i);
                            
#if DEVELOPMENT_BUILD || DEBUG
                            DebugUtil.Log($"AdsManager : Found other valid ads: {targetAd.m_PluginName} => {targetAd.m_EcpmFloor}"); 
#endif
                            
                            return targetAd;
                        }
                    }

                    if (!uaSplitterEnabled)
                    {
                        // null if no other ads found and uaSplitterEnabled is false
                        
#if DEVELOPMENT_BUILD || DEBUG
                        DebugUtil.Log($"AdsManager : No other valid ads found, and uaSplitterEnabled is false, return null"); 
#endif 
                        
                        return null;
                    }

                    // no valid other unit found, play found targetAd
                   
#if DEVELOPMENT_BUILD || DEBUG
                    DebugUtil.Log($"AdsManager : No other valid ads found, choose {targetAd.m_PluginName} => {targetAd.m_EcpmFloor}"); 
#endif 
                }
            }


            if (removeOriginAdsUnit)
            {
                sources.RemoveAt(0);
            }

            return targetAd;
        }

        public static void SortAdUnits(List<AdsUnitDefine> sources)
        {
            if (sources is not { Count: > 1 })
            {
                return;
            }

#if DEVELOPMENT_BUILD || DEBUG
            PrintAdsUnitDefines("Simon 广告 V2 before sorting", sources);
#endif

            sources.Sort((a, b) =>
            {
                var result = b.m_EcpmFloor.CompareTo(a.m_EcpmFloor);

                return result != 0 ? result : b.m_Weight.CompareTo(a.m_Weight);
            });

#if DEVELOPMENT_BUILD || DEBUG
            PrintAdsUnitDefines("Simon 广告 V2 after sorting", sources);
#endif
        }

#if DEVELOPMENT_BUILD || DEBUG
        private static void PrintAdsUnitDefines(string title, List<AdsUnitDefine> sources)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(title);

            foreach (var ad in sources)
            {
                stringBuilder.AppendLine(
                    "\t name " + ad.m_PluginName +
                    " place "  + ad.m_Placement  +
                    " ecpm "   + ad.m_EcpmFloor  +
                    " weight " + ad.m_Weight);
            }

            DebugUtil.Log(stringBuilder.ToString());
        }
#endif
    }
}