/*
 * @file LanguageModel
 * 设置 - 语言
 * @author lu
 */

using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;

public class Language
{
    public string LKey { get; set; }

    public string DisStr { get; set; }

    // 是否已经开放这个语言,具体设置在  LocaleConfigManager.Instance.supportedLocale
    public bool IsEnabled { get; set; }
}

public class LanguageModel : Manager<LanguageModel>
{
    StorageCommon storageCommon
    {
        get { return StorageManager.Instance.GetStorage<StorageCommon>(); }
    }

    public void Init()
    {
    }


    public void SetLocale(string language)
    {
        storageCommon.Locale = language;
    }

    // 当前语言
    public string GetLocale()
    {
        if (storageCommon != null)
        {
            if (string.IsNullOrEmpty(storageCommon.Locale))
            {
                storageCommon.Locale = Locale.ENGLISH;
                storageCommon.OrigLocale = Locale.ENGLISH;
            }

            return storageCommon.Locale;
        }

        return Locale.ENGLISH;
    }

    // 原语言
    public string GetOrigLLocale()
    {
        if (string.IsNullOrEmpty(storageCommon.OrigLocale))
        {
            storageCommon.OrigLocale = Locale.ENGLISH;
        }

        return storageCommon.OrigLocale;
    }

    public Dictionary<string, Language> GetCfg()
    {
        var languageCfg = new Dictionary<string, Language>();
        languageCfg[Locale.ENGLISH] = new Language {LKey = Locale.ENGLISH, DisStr = "English"};
        languageCfg[Locale.FRENCH] = new Language {LKey = Locale.FRENCH, DisStr = "Français"};
        languageCfg[Locale.GERMAN] = new Language {LKey = Locale.GERMAN, DisStr = "Deutsch"};
        languageCfg[Locale.PORTUGUESE] = new Language {LKey = Locale.PORTUGUESE, DisStr = "Português"};
        languageCfg[Locale.SPANISH] = new Language {LKey = Locale.SPANISH, DisStr = "Español"};
        languageCfg[Locale.ITALIAN] = new Language {LKey = Locale.ITALIAN, DisStr = "Italiano"};
        languageCfg[Locale.INDONESIAN] = new Language {LKey = Locale.INDONESIAN, DisStr = "Bahasa Indonesia"};
        languageCfg[Locale.RUSSIAN] = new Language {LKey = Locale.RUSSIAN, DisStr = "Русский"};
        languageCfg[Locale.VIETNAMESE] = new Language {LKey = Locale.VIETNAMESE, DisStr = "Tiếng Việt"};
        languageCfg[Locale.TURKISH] = new Language {LKey = Locale.TURKISH, DisStr = "Türkçe"};
        languageCfg[Locale.THAI] = new Language {LKey = Locale.THAI, DisStr = "ภาษาไทย"};
        languageCfg[Locale.JAPANESE] = new Language {LKey = Locale.JAPANESE, DisStr = "日本語"};
        languageCfg[Locale.KOREA] = new Language {LKey = Locale.KOREA, DisStr = "한국어"};
        languageCfg[Locale.CHINESE_SIMPLIFIED] = new Language {LKey = Locale.CHINESE_SIMPLIFIED, DisStr = "简体中文"};
        languageCfg[Locale.CHINESE_TRADITION] = new Language {LKey = Locale.CHINESE_TRADITION, DisStr = "繁體中文"};
        languageCfg[Locale.HINDI] = new Language {LKey = Locale.HINDI, DisStr = "हिन्दी"};
        languageCfg[Locale.DUTCH] = new Language {LKey = Locale.DUTCH, DisStr = "Nederlands"};
        languageCfg[Locale.MALAYSIA] = new Language {LKey = Locale.MALAYSIA, DisStr = "Bahasa Melayu"};
        // languageCfg[Locale.ARABIC] = new Language { LKey = Locale.ARABIC, DisStr = "اللغة العربية" };

        var LanguageList = LocalizationManager.Instance.supportedLocale;
        foreach (string item in LanguageList)
        {
            languageCfg[item].IsEnabled = true;
        }

        return languageCfg;
    }
}