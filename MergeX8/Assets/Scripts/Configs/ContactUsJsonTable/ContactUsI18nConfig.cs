
using System.Collections.Generic;

namespace DragonPlus.Config
{
    public class ContactUsI18nConfig
    {
        public string Key { get; set; }
        
        public string Zh { get; set; }
        
        public string En { get; set; }
        
        public string De { get; set; }
        
        public string Fr { get; set; }
        
        public string Kr { get; set; }
        
        public string Jp { get; set; }
        
        public string Zht { get; set; }
        
        public string Pt { get; set; }
        
        public string Es { get; set; }
        
        public string Ru { get; set; }
        
        public string It { get; set; }
        
        public string Nl { get; set; }
        
        public string Tr { get; set; }
        
        public string Id { get; set; }
        
        public string Th { get; set; }
        
        public string Vi { get; set; }

        public Dictionary<string,string> I18nDic { get; set; }
        
        public void InitI18n()
        {
            I18nDic = new Dictionary<string, string>();
            I18nDic.Add(Locale.ENGLISH,En); // 英语
            I18nDic.Add(Locale.FRENCH,Fr);  // 法语
            I18nDic.Add(Locale.GERMAN,De);  // 德语
            I18nDic.Add(Locale.PORTUGUESE,Pt);  // 葡萄牙
            I18nDic.Add(Locale.JAPANESE,Jp);    // 日语
            I18nDic.Add(Locale.KOREA,Kr);       // 韩语
            I18nDic.Add(Locale.CHINESE_SIMPLIFIED,Zh); // 简体中文
            I18nDic.Add(Locale.CHINESE_TRADITION,Zht); // 繁体中文
            I18nDic.Add(Locale.SPANISH,Es);    // 西班牙
            I18nDic.Add(Locale.ITALIAN,It);    // 意大利
            I18nDic.Add(Locale.INDONESIAN,Id); // 印度尼西亚语
            I18nDic.Add(Locale.RUSSIAN,Ru);    // 俄罗斯
            I18nDic.Add(Locale.VIETNAMESE,Vi); // 越南语
            I18nDic.Add(Locale.TURKISH,Tr);    // 土耳其
            I18nDic.Add(Locale.THAI,Th);       // 泰国
            I18nDic.Add(Locale.DUTCH,Nl);      // 荷兰
        }
    }
}


