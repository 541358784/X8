using System.Collections.Generic;
using DragonPlus.Config;
using DragonU3DSDK;

namespace DragonPlus
{
    public class ReplaceLocalConfigManager
    {
        Dictionary<string, Dictionary<string, string>> localeConfigs =
            new Dictionary<string, Dictionary<string, string>>();

        // AdsManager 单例化
        private static ReplaceLocalConfigManager instance = null;
        private static readonly object syslock = new object();

        public static ReplaceLocalConfigManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new ReplaceLocalConfigManager();
                        }
                    }
                }

                return instance;
            }
        }

        private ReplaceLocalConfigManager()
        {
        }

        public Dictionary<string, Dictionary<string, string>> GetLocaleConfigs()
        {
            return localeConfigs;
        }

        // 额外的覆盖多语言的方法,一般用于活动单独下发的多语言
        public void AddedI18nConfigs(List<AddedI18nConfig> addedI18nConfigs)
        {
            foreach (AddedI18nConfig addedI18nConfig in addedI18nConfigs)
            {
                foreach (string locale in LocalizationManager.Instance.supportedLocale)
                {
                    if (!localeConfigs.ContainsKey(locale))
                    {
                        localeConfigs[locale] = new Dictionary<string, string>();
                    }

                    var addValue = addedI18nConfig.GetI18nString(locale);
                    if (!string.IsNullOrEmpty(addValue))
                    {
                        DebugUtil.Log("新增多语言: " + locale + "," + addedI18nConfig.Key + "," + addValue);
                        localeConfigs[locale][addedI18nConfig.Key] = addValue;
                    }
                    else
                    {
                        DebugUtil.Log("新增多语言 错误: " + locale + "," + addedI18nConfig.Key);
                    }
                }
            }
        }
    }
}