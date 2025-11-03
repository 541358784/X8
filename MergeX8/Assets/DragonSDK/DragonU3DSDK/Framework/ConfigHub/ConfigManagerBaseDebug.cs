/************************************************
 * ConfigManagerBase
 * If there's any problem, ask yunhan.zeng@dragonplus.com
 ************************************************/
using DragonU3DSDK;
using DragonU3DSDK.Storage;

namespace DragonPlus.ConfigHub
{
    public abstract partial class ConfigManagerBase
    {
        protected string getModuleString()
        {
            return getModuleInfoString(MetaData.VersionIOS, MetaData.VersionAndroid, MetaData.GroupId,
                ConfigHubManager.Instance.GetGroupIdSpecified(Guid), GetCacheGroupId(false));
        }
        
        public string getCacheString()
        {
            var cache = getCache();
            if (cache == null) return "cache:[null]";
            var gc = MetaData?.GroupId ?? -1;
            var leftTime = cache.Duration < 0 ? "permanent" : $"{Utils.GetTimeStamp() - (cache.CreateTime + cache.Duration)}";
            return "cache:" + getModuleInfoString(cache.VersionIOS, cache.VersionAndroid, gc,
                ConfigHubManager.Instance.GetGroupIdSpecified(Guid), cache.UserGroup, leftTime);
        }
        
        private string getModuleInfoString(int vi, int va, int gc, int gr, int gCache, string leftTime = null)
        {
            var leftTimeString = string.IsNullOrEmpty(leftTime) ? "" : $" left<{leftTime}>";
            return $"[{Guid} - version<i({vi})/a({va})> group<current({gc})/remote({gr})/cache({gCache})>{leftTimeString}]";
        }
        
        public string DebugCacheClear()
        {
            return clearCache("from debug");
        }
    }
}