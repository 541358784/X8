using System;

namespace DragonPlus.Config
{
    public class AddedI18nConfig
    {
        public String Key { get; set; }
        public String En { get; set; }
        public String De { get; set; }
        public String Fr { get; set; }
        public String Kr { get; set; }
        public String Jp { get; set; }
        public String Pt { get; set; }
        public String Es { get; set; }
        public String It { get; set; }
        public String Id { get; set; }
        public String Ru { get; set; }
        public String Vi { get; set; }
        public String Tr { get; set; }
        public String Th { get; set; }
        public String Nl { get; set; }
        public String Zh { get; set; }
        public String Zht { get; set; }
        public String Ms { get; set; }
        public String Ar { get; set; }
        public String Hi { get; set; }

        public string GetI18nString(string lauage)
        {
            switch (lauage)
            {
                case Locale.ENGLISH:
                    return En;
                case Locale.GERMAN:
                    return De;
                case Locale.FRENCH:
                    return Fr;
                case Locale.KOREA:
                    return Kr;
                case Locale.JAPANESE:
                    return Jp;
                case Locale.PORTUGUESE:
                    return Pt;
                case Locale.SPANISH:
                    return Es;
                case Locale.ITALIAN:
                    return It;
                case Locale.INDONESIAN:
                    return Id;
                case Locale.RUSSIAN:
                    return Ru;
                case Locale.VIETNAMESE:
                    return Vi;
                case Locale.TURKISH:
                    return Tr;
                case Locale.THAI:
                    return Th;
                case Locale.DUTCH:
                    return Nl;
                case Locale.HINDI:
                    return Nl;
                case Locale.MALAYSIA:
                    return Ms;
                case Locale.ARABIC:
                    return Ar;
                case Locale.CHINESE_SIMPLIFIED:
                    return Zh;
                case Locale.CHINESE_TRADITION:
                    return Zht;
                default:
                    return En;
            }
        }
    }
}