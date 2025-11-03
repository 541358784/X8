using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Newtonsoft.Json;
using UnityEngine;

namespace DragonPlus.Config
{
    public class LocaleConfigManager : Manager<LocaleConfigManager>
    {
        Dictionary<string, Dictionary<string, string>> localeConfigs =
            new Dictionary<string, Dictionary<string, string>>();

        ////////////////
        // 备份版多语言，从app里读取的
        Dictionary<string, Dictionary<string, string>> bakLocaleConfigs =
            new Dictionary<string, Dictionary<string, string>>();

        private bool loadedFromApp = false;
        string sep = " = ";

        public bool IsInit
        {
            get { return localeConfigs.Count > 0; }
        }

        /// <summary>
        /// 读取一份本地的多语言
        /// </summary>
        public void LoadBakLocale()
        {
            if (loadedFromApp)
            {
                return;
            }

            foreach (string locale in LocalizationManager.Instance.supportedLocale)
            {
                var configPath = "Export/Configs/LocaleConfig/locale_loading_" + locale;
                var ta = Resources.Load<TextAsset>(configPath);
                string content = ta.text;

                content = ChangeableConfig.Instance.AddChangeableConfig("locale_" + locale, content);

                List<LocaleItemConfig> localeConfig = JsonConvert.DeserializeObject<List<LocaleItemConfig>>(content);
                if (localeConfig != null && localeConfig.Count > 0)
                {
                    Dictionary<string, string> configs = new Dictionary<string, string>();
                    foreach (LocaleItemConfig c in localeConfig)
                    {
                        configs[c.Key] = c.Value;
                    }

                    ////add home
                    //if(locale.Equals(Locale.ENGLISH))
                    //{
                    //    TextAsset textAssetHome = Resources.Load<TextAsset>("Export/Configs/global_localization");
                    //    if (textAssetHome.text != null)
                    //    {
                    //        List<TableGlobal_Localization> configHome = DragonU3DSDK.Config.TableManager.DeSerialize<TableGlobal_Localization>(textAssetHome.text);
                    //        foreach (TableGlobal_Localization kv in configHome)
                    //        {
                    //            configs[kv.key] = kv.en;
                    //        }
                    //    }
                    //}
                    bakLocaleConfigs[locale] = configs;
                }

                Resources.UnloadAsset(ta);
            }

            // 初始包里的多语言读取完后，设置下tips的内容
            GameTextUtils.PickupLocale();

            loadedFromApp = true;
        }

        public void InitConfigs(bool isClear = false)
        {
            if (isClear)
                localeConfigs.Clear();

            foreach (string locale in LocalizationManager.Instance.supportedLocale)
            {
                if (localeConfigs.ContainsKey(locale))
                    continue;

                //DebugUtil.Log("Prepare to init config for locale " + locale);
                var configPath = PathManager.Configs + "/LocaleConfig/locale_" + locale;
                var ta = ResourcesManager.Instance.LoadResource<TextAsset>(configPath, addToCache: false);
                string content = ta.text;
                //DebugUtil.Log(ta);
                List<LocaleItemConfig> localeConfig = JsonConvert.DeserializeObject<List<LocaleItemConfig>>(content);

                //DebugUtil.Log("localeConfigs count : " + localeConfig.Count);
                if (localeConfig != null && localeConfig.Count > 0)
                {
                    Dictionary<string, string> configs = new Dictionary<string, string>();
                    foreach (LocaleItemConfig c in localeConfig)
                    {
                        if (string.IsNullOrEmpty(c.Key))
                        {
                            DebugUtil.LogError("key is " + c.Key + "  value is " + c.Value + "\t local " + locale);
                            continue;
                        }

                        //DebugUtil.Log("key : "+c.Key);
                        if (configs.ContainsKey(c.Key))
                        {
                            configs[c.Key] = c.Value;
                        }
                        else
                        {
                            configs.Add(c.Key, c.Value);
                        }
                    }

                    localeConfigs[locale] = configs;
                }

                ResourcesManager.Instance.ReleaseRes(configPath, true);
            }

            // 远端的多语言读取完后，更新下loading条上的tips
            GameTextUtils.PickupLocale();
        }

        // 对嵌套的key进行处理
        string ReplaceKeyVlaue(Dictionary<string, Dictionary<string, string>> tempLocaleConfigs, string locale,
            string key)
        {
            string value = tempLocaleConfigs[locale][key];
            if (string.IsNullOrEmpty(value))
                return "";

            MatchCollection Matches = Regex.Matches(value, "%{(.+?)}");
            foreach (Match match in Matches)
            {
                TryGetLocalString(match.Groups[1].Value, locale, out var result);
                value = value.Replace("%{" + match.Groups[1].Value + "}", result);
            }

            return value;
        }

        public bool TryGetLocalString(string key, string locale, out string result)
        {
            var replaceLocalConfig = ReplaceLocalConfigManager.Instance.GetLocaleConfigs();
            if (replaceLocalConfig.ContainsKey(locale)
                && replaceLocalConfig[locale].ContainsKey(key)) //是否有热更的内容
            {
                result = ReplaceKeyVlaue(replaceLocalConfig, locale, key);
                return true;
            }

            if (localeConfigs.ContainsKey(locale) && localeConfigs[locale].ContainsKey(key)) //从远端多语言里get到了
            {
                result = ReplaceKeyVlaue(localeConfigs, locale, key);
                return true;
            }

            if (bakLocaleConfigs.ContainsKey(locale) && bakLocaleConfigs[locale].ContainsKey(key)) //从备份版多语言里get到了
            {
                result = ReplaceKeyVlaue(bakLocaleConfigs, locale, key);
                return true;
            }

            result = key;
            return false;
        }

        public string AllConfigs(int index, int total)
        {
            if (index < 1) index = 1;
            if (index > total) index = total;
            var locale = LanguageModel.Instance.GetLocale();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            foreach (var localeConfig in localeConfigs)
            {
                if (localeConfig.Key == locale)
                {
                    var dict = localeConfig.Value;
                    var firstHalf = new Dictionary<string, string>();
                    int start = dict.Count * (index - 1) / total;
                    int end = dict.Count * index / total;
                    int count = 0;
                    foreach (var item in dict)
                    {
                        count++;
                        if (count >= start && count <= end)
                        {
                            firstHalf.Add(item.Key, item.Value);
                        }
                    }

                    foreach (var item in firstHalf)
                    {
                        sb.Append(item.Key)
                            .Append(sep)
                            .Append(item.Value)
                            .AppendLine();
                    }

                    return $"<start>{sb}</end>";
                }
            }

            return $"<start>{sb}</end>";
        }

        public string match(string str)
        {
            Regex regex = new Regex("([\\u4e00-\\u9fa5]{1,})");
            return regex.Replace(str, $"<color=green>{str}</color>");
        }
    }
}