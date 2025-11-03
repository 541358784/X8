using System;
using System.Collections.Generic;
using System.Text;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.SDKEvents;
using DragonU3DSDK.Storage;
using TMPro;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace DragonPlus
{
    public sealed class
        LocalizationManager /*: IEventHandler<ProfileReplacedEvent>, IEventHandler<ProfileFetchedEvent>*/
    {
        // 拉取到远端存档后，切换语言的委托事件
        public delegate void LocalizationAction();
        // public static event LocalizationAction OnLocalization;

        private string current_locale = Locale.ENGLISH;

        private Dictionary<string, Material> m_CacheMaterial = new Dictionary<string, Material>();
        private Dictionary<string, TMP_FontAsset> m_CacheFont = new Dictionary<string, TMP_FontAsset>();

        private bool isForceShowKey = false;


        // TMPSettings
        private static bool m_IsTMPSettingsLoaded = false;
        private static Dictionary<string, TMPSettingParam> TMPSettingDic = new Dictionary<string, TMPSettingParam>();

        public const string STR_TMP_SETTINGS_NAME = "TMP Settings {0}";
        public const string STR_TMP_FONT_ASSET_PATH = "Export/Fonts/{0}/{1}";
        public const string STR_TMP_SETTING_PATH = "Export/Fonts/{0}";
        public const string STR_FONT_ASSET_NAME = "LocaleFont_{0} SDF";
        public const string STR_MATERIAL_ASSET_NAME = "LocaleFont_{0} SDF {1}";
        public const string ENGLISH = "En";
        public const string FRENCH = "Fr";
        public const string GERMAN = "De";
        public const string PORTUGUESE = "Pt";
        public const string SPANISH = "Es";
        public const string ITALIAN = "It";
        public const string INDONESIAN = "Id";
        public const string RUSSIAN = "Ru";
        public const string VIETNAMESE = "Vi";
        public const string TURKISH = "Tr";
        public const string THAI = "Th";
        public const string JAPANESE = "Jp";
        public const string KOREA = "Kr";
        public const string CHINESE_SIMPLIFIED = "Zh";
        public const string CHINESE_TRADITION = "Zht";
        public const string HINDI = "Hi";
        public const string DUTCH = "Nl"; // 荷兰
        public const string MALAYSIA = "Ms"; // 马来西亚
        public const string ARABIC = "Ar"; // 阿拉伯

        // 配置已经支持的语言
        public readonly List<string> supportedLocale = new List<string>
        {
            Locale.ENGLISH,
            Locale.GERMAN,
            Locale.FRENCH,
            Locale.JAPANESE,
            Locale.CHINESE_SIMPLIFIED,
            Locale.KOREA,
            Locale.SPANISH,
            Locale.PORTUGUESE,
            Locale.RUSSIAN,
            Locale.TURKISH,
            
            //Locale.CHINESE_TRADITION,
            //Locale.ITALIAN,
            // Locale.INDONESIAN,
            // Locale.VIETNAMESE,
            // Locale.THAI,
            // Locale.HINDI,
            // Locale.DUTCH,
            // Locale.MALAYSIA,
            // Locale.ARABIC,
        };

        public static readonly Dictionary<string, string> Local2Suffix = new Dictionary<string, string>
        {
            {Locale.ENGLISH, ENGLISH},
            {Locale.FRENCH, FRENCH},
            {Locale.GERMAN, GERMAN},
            {Locale.PORTUGUESE, PORTUGUESE},
            {Locale.JAPANESE, JAPANESE},
            {Locale.KOREA, KOREA},
            {Locale.CHINESE_SIMPLIFIED, CHINESE_SIMPLIFIED},
            {Locale.CHINESE_TRADITION, CHINESE_TRADITION},
            {Locale.SPANISH, SPANISH},
            {Locale.ITALIAN, ITALIAN},
            {Locale.INDONESIAN, INDONESIAN},
            {Locale.RUSSIAN, RUSSIAN},
            {Locale.VIETNAMESE, VIETNAMESE},
            {Locale.TURKISH, TURKISH},
            {Locale.THAI, THAI},
            {Locale.HINDI, HINDI},
            {Locale.DUTCH, DUTCH},
            {Locale.MALAYSIA, MALAYSIA},
        };

        /// 主动切换多语言，选择的目标语言记录【en,1;zh,2;de,3;fr,4;ko,5;ja,6;pt,7;es,8;ru,9;it,10;nl,11;tr,12;id,13;th,14;vi,15;hi,16;ms,17zht,18;ar,19] 
        public static readonly Dictionary<string, int> localeBIValues = new Dictionary<string, int>
        {
            {"en", 1}, {"zh", 2}, {"de", 3}, {"fr", 4}, {"kr", 5}, {"jp", 6}, {"pt", 7}, {"es", 8}, {"ru", 9},
            {"it", 10},
            {"nl", 11}, {"tr", 12}, {"id", 13}, {"th", 14}, {"vi", 15}, {"hi", 16}, {"ms", 17}, {"zht", 18}, {"ar", 19}
        };

        public static int GetLocaleBIValue(string key)
        {
            int value = 0;
            localeBIValues.TryGetValue(key, out value);
            return value;
        }

        private static readonly LocalizationManager instance = new LocalizationManager();

        static LocalizationManager()
        {
        }

        private LocalizationManager()
        {
            //EventManager.Instance.Subscribe<ProfileReplacedEvent>(this);
            //EventManager.Instance.Subscribe<ProfileFetchedEvent>(this);
        }

        public static LocalizationManager Instance
        {
            get { return instance; }
        }

        private static readonly StringBuilder m_builder = new StringBuilder();
        private static readonly object m_lock = new object(); // thread safe

        private static string Format(string s, params object[] values)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (values.Length < 1) return s; // params is empty

            int vi = 0,
                start = 0,
                now = 0,
                len = s.Length; // value index, current start position, current search position

            lock (m_lock)
            {
                m_builder.Clear();
                while (now < len)
                {
                    if (s[now++] != '%') continue;
                    var c = s[now++];
                    if (c == 's' || c == 'S')
                    {
                        var v = values[vi++];
                        m_builder.Append(s.Substring(start, now - start - 2))
                            .Append(v == null ? string.Empty : v.ToString());
                        start = now;

                        if (vi >= values.Length) break;
                    }
                }

                if (start < len) m_builder.Append(s.Substring(start));

                return m_builder.ToString();
            }
        }

        private static string FormatCSharp(string s, params object[] values)
        {
            try
            {
                return string.Format(s, values);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
                DebugUtil.LogError($"FormatCSharp string error : " + s);
            }

            return s;
        }

        // 多个%s情况下的不定参替换
        public string GetLocalizedStringWithFormats(string key, params object[] values)
        {
            var newStr = GetLocalizedString(key);

            if (newStr == string.Empty || newStr.Equals(key)) return newStr;

            //newStr = Format(newStr, values);
            newStr = FormatCSharp(newStr, values);

            return newStr;
        }

        public string GetLocalizedStringWithFormat(string key, string content)
        {
            return GetLocalizedStringWithFormats(key, content);
        }

        public string GetLocalizedString(string key)
        {
            string _key = key.Trim();

            if (String.IsNullOrEmpty(_key))
            {
                return String.Empty;
            }

            if (_key.StartsWith("&key", StringComparison.InvariantCulture))
            {
                _key = _key.Substring(5);
            }

            if (isForceShowKey)
                return _key;

            if (!LocaleConfigManager.Instance.TryGetLocalString(_key, current_locale, out var result))
            {
                DebugUtil.LogError($"GetLocalString error, key = {key}, locale = {current_locale}");
            }

            return result;
        }

        public bool TryGetLocalizedString(string key, out string result)
        {
            string _key = key.Trim();

            if (String.IsNullOrEmpty(_key))
            {
                result = String.Empty;
                return true;
            }

            if (_key.StartsWith("&key", StringComparison.InvariantCulture))
            {
                _key = _key.Substring(5);
            }

            if (isForceShowKey)
            {
                result = _key;
                return true;
            }

            return LocaleConfigManager.Instance.TryGetLocalString(_key, current_locale, out result);
        }


        public string GetLocalizedString(string key, string givenLocale)
        {
            string _key = key.Trim();

            if (String.IsNullOrEmpty(_key))
            {
                return String.Empty;
            }

            if (_key.StartsWith("&key", StringComparison.InvariantCulture))
            {
                _key = _key.Substring(5);
            }

            if (isForceShowKey)
                return _key;

            if (!LocaleConfigManager.Instance.TryGetLocalString(_key, givenLocale, out var result))
            {
                DebugUtil.LogError($"GetLocalString error, key = {key}, locale = {givenLocale}");
            }

            return result;
        }


        /// <summary>
        /// 匹配当前用户的语言
        /// 1、存档里Local字段为空:根据操作系统语言匹配多语言
        /// 2、存档里Local字段不为空:
        ///    2.1、Local字段在supportedLocale里，直接切换到存档里的语言
        ///    2.2、Local字段不在supportedLocale里，执行1
        /// </summary>
        public void MatchLanguage()
        {
            StorageCommon storage = StorageManager.Instance.GetStorage<StorageCommon>();
            string userLanguage = GetSystemLanguage();
            if (string.IsNullOrEmpty(storage.Locale))
            {
                storage.Locale = userLanguage;
                storage.OrigLocale = userLanguage;
                SetCurrentLocale(userLanguage);
            }
            else
            {
                if (supportedLocale.Contains(storage.Locale))
                {
                    SetCurrentLocale(storage.Locale);
                }
                else
                {
                    storage.Locale = userLanguage;
                    storage.OrigLocale = userLanguage;
                    SetCurrentLocale(userLanguage);
                }
            }
        }

        public bool SetCurrentLocale(string locale, bool sendBi = false)
        {
            if (sendBi)
            {
                // CKBI.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLanguageChange,data1: GetLocaleBIValue(locale).ToString(),
                //     data2:GetLocaleBIValue(current_locale).ToString(),data3: GetLocaleBIValue(GetSystemLanguage()).ToString());
            }

            if (string.IsNullOrEmpty(locale))
            {
                return false;
            }

            if (current_locale == locale)
            {
                return false;
            }

            if (!supportedLocale.Contains(locale))
            {
                return false;
            }

            current_locale = locale;
            StorageManager.Instance.GetStorage<StorageCommon>().Locale = locale;
            return true;
        }

        public string GetCurrentLocale()
        {
            return current_locale;
        }

        public TMP_FontAsset GetLocaleFont()
        {
            string locale = CommonUtils.FirstCharToUpper(current_locale);
            string key = string.Format("LocaleFont_{0} SDF", locale);
            TMP_FontAsset fontAsset = null;

            if (m_CacheFont.TryGetValue(key, out fontAsset))
            {
                return fontAsset;
            }
            else
            {
                string path = string.Format("Export/Fonts/{0}/{1}", locale, key);
                //fontAsset = ResourcesManager.Instance.LoadResource<TMP_FontAsset>(path);
                fontAsset = Resources.Load<TMP_FontAsset>(path);
                m_CacheFont.Add(key, fontAsset);
                return fontAsset;
            }
        }

        // todo:默认材质的加载路径，目前不对
        public Material GetLocaleMaterial(string suffix)
        {
            string locale = CommonUtils.FirstCharToUpper(current_locale);
            string key = string.Format("LocaleFont_{0} SDF {1}", locale, suffix);
            Material mat = null;

            if (m_CacheMaterial.TryGetValue(key, out mat))
            {
                return mat;
            }
            else
            {
                string path = string.Format("Export/Fonts/{0}/{1}", locale, key);
                //mat = ResourcesManager.Instance.LoadResource<Material>(path);
                mat = Resources.Load<Material>(path);
                m_CacheMaterial.Add(key, mat);
                return mat;
            }
        }

        public void ForceShowKey()
        {
            isForceShowKey = true;
        }

        #region 匹配操作系统的语言

        private string GetSystemLanguage()
        {
            string osLanguage = GetSystemLanguage(Application.systemLanguage);
            return GetLocal(osLanguage);
        }

        private string GetSystemLanguage(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Afrikaans: return "af";
                case SystemLanguage.Arabic: return "ar";
                case SystemLanguage.Basque: return "eu";
                case SystemLanguage.Belarusian: return "be";
                case SystemLanguage.Bulgarian: return "bg";
                case SystemLanguage.Catalan: return "ca";
                case SystemLanguage.Chinese: return "zh";
                case SystemLanguage.ChineseSimplified: return "zh";
                case SystemLanguage.ChineseTraditional: return "zht";
                case SystemLanguage.Czech: return "cs";
                case SystemLanguage.Danish: return "da";
                case SystemLanguage.Dutch: return "nl";
                case SystemLanguage.English: return "en";
                case SystemLanguage.Estonian: return "et";
                case SystemLanguage.Faroese: return "fo";
                case SystemLanguage.Finnish: return "fi";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Greek: return "el";
                case SystemLanguage.Hebrew: return "he";
                case SystemLanguage.Icelandic: return "is";
                case SystemLanguage.Indonesian: return "id";
                case SystemLanguage.Japanese: return "jp";
                case SystemLanguage.Korean: return "kr";
                case SystemLanguage.Latvian: return "lv";
                case SystemLanguage.Lithuanian: return "lt";
                case SystemLanguage.Norwegian: return "no";
                case SystemLanguage.Polish: return "pl";
                case SystemLanguage.Portuguese: return "pt";
                case SystemLanguage.Romanian: return "ro";
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.SerboCroatian: return "hr";
                case SystemLanguage.Slovak: return "sk";
                case SystemLanguage.Slovenian: return "sl";
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.Swedish: return "sv";
                case SystemLanguage.Thai: return "th";
                case SystemLanguage.Turkish: return "tr";
                case SystemLanguage.Ukrainian: return "uk";
                case SystemLanguage.Vietnamese: return "vi";
                case SystemLanguage.Hungarian: return "hu";
                case SystemLanguage.Italian: return "it";
                case SystemLanguage.Unknown: return "en";
            }

            return "en";
        }

        // 过滤不支持的语言
        private string GetLocal(string language)
        {
            if (supportedLocale.Contains(language))
                return language;

            return Locale.ENGLISH;
        }

        //public void OnNotify(ProfileReplacedEvent message)
        //{
        //    this.MatchLanguage();
        //    //OnLocalization();
        //}

        //public void OnNotify(ProfileFetchedEvent message)
        //{
        //    this.MatchLanguage();
        //    //OnLocalization();
        //}

        #endregion

        #region TMPSettings

        private void LoadTMPSettings()
        {
            if (!m_IsTMPSettingsLoaded)
            {
                List<TMPSettingParam> paramList = new List<TMPSettingParam>();
                foreach (var locale in supportedLocale)
                {
                    string fileName = string.Format(STR_TMP_SETTINGS_NAME, Local2Suffix[locale]);
                    string filePath = string.Format(STR_TMP_SETTING_PATH, fileName);
                    TMPSettings tmpSetting = Resources.Load<TMPSettings>(filePath);
                    if (tmpSetting && tmpSetting.List != null)
                        paramList.AddRange(tmpSetting.List);
                }

                TMPSettingDic.Clear();
                for (int i = 0; i < paramList.Count; i++)
                {
                    TMPSettingParam param = paramList[i];
                    TMPSettingDic[param.Name] = param;
                }

                m_IsTMPSettingsLoaded = true;
            }
        }

        public TMPSettingParam GetLocaleParam(string suffix)
        {
            LoadTMPSettings();
            string paramName = string.Format(STR_MATERIAL_ASSET_NAME, Local2Suffix[current_locale], suffix);
            TMPSettingParam findedParam;
            if (TMPSettingDic.ContainsKey(paramName))
                findedParam = TMPSettingDic[paramName];
            else
            {
                findedParam = new TMPSettingParam();
                // DebugUtil.LogError(string.Format("{0} ### GetSettingParam Error ### {1}", current_locale, suffix));
            }

            return findedParam;
        }

        #endregion
    }
}