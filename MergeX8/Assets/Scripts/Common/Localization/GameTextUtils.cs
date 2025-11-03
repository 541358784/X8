using System.Collections.Generic;

namespace DragonPlus
{
    public static class GameTextUtils
    {
        private static Dictionary<string, List<string>> localTipsDict = new Dictionary<string, List<string>>();

        private static readonly List<string> tips = new List<string>
        {
            "&key.UI_tips_text1",
            "&key.UI_tips_text2",
            "&key.UI_tips_text3",
        };

        public static void PickupLocale()
        {
            //localTipsDict.Clear();
            //foreach (string locale in LocalizationManager.Instance.supportedLocale)
            //{
            //    List<string> localeTips = new List<string>();
            //    foreach (string key in tips)
            //    {
            //        string strNotFind = key.Substring(5);
            //        string text = LocalizationManager.Instance.GetLocalizedString(key, locale);
            //        localeTips.Add(text);
            //    }
            //    localTipsDict.Add(locale, localeTips);
            //}
        }

        public static string GetRandomTip()
        {
            if (localTipsDict.Count == 0)
            {
                return "";
            }

            System.Random rand = new System.Random();
            int idx = rand.Next(tips.Count);

            return localTipsDict[LocalizationManager.Instance.GetCurrentLocale()][idx];
        }
    }
}