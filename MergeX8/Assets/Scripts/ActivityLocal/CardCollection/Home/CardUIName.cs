using System.Collections.Generic;
using Dlugin;

namespace ActivityLocal.CardCollection.Home
{
    public static class CardUIName
    {
        public enum UIType
        {
            UICardMain,
            UICardBook,
            UICardRewardTheme,
            UICardRewardBook,
            UIOpenCard,
            UIPopupCardStart,
            UIPopupCardReopenStart,
            UICardHelp,
            UIUpGradeCard,
        }

        public static Dictionary<UIType, string> CardUINameDic = new Dictionary<UIType, string>()
        {
            {UIType.UICardMain,"UICardMain"},
            {UIType.UICardBook,"UICardBook"},
            {UIType.UICardRewardTheme,"UICardRewardBook"},
            {UIType.UICardRewardBook,"UICardReward"},
            {UIType.UIOpenCard,"UIOpenCard"},
            {UIType.UIPopupCardStart,"UIPopupCardStart"},
            {UIType.UIPopupCardReopenStart,"UIPopupCardReopenStart"},
            {UIType.UICardHelp,"UICardHelp"},
            {UIType.UIUpGradeCard,"UIUpGradeCard"},
        };

        public static string[] CardUINames()
        {
            List<string> names = new List<string>();
            if (CardCollectionModel.Instance.ThemeInUse != null && CardCollectionModel.Instance.ThemeInUse.CardThemeConfig != null)
            {
                names.Add(CardCollectionModel.Instance.ThemeInUse.GetCardUIName(UIType.UIPopupCardStart));
                names.Add(CardCollectionModel.Instance.ThemeInUse.GetCardUIName(UIType.UIPopupCardReopenStart));
                names.Add(UINameConst.UIMainCard);
                names.Add(CardCollectionModel.Instance.ThemeInUse.GetCardUIName(UIType.UICardMain));
                names.Add(CardCollectionModel.Instance.ThemeInUse.GetCardUIName(UIType.UICardBook));
                names.Add(CardCollectionModel.Instance.ThemeInUse.GetCardUIName(UIType.UIOpenCard));
            }
            return names.ToArray();
        }
        public static string[] CardReopenUINames()
        {
            List<string> names = new List<string>();
            if (CardCollectionModel.Instance.ThemeReopenInUse != null && CardCollectionModel.Instance.ThemeReopenInUse.CardThemeConfig != null)
            {
                names.Add(CardCollectionModel.Instance.ThemeReopenInUse.GetCardUIName(UIType.UIPopupCardStart));
                names.Add(UINameConst.UIMainCard);
                names.Add(CardCollectionModel.Instance.ThemeReopenInUse.GetCardUIName(UIType.UICardMain));
                names.Add(CardCollectionModel.Instance.ThemeReopenInUse.GetCardUIName(UIType.UICardBook));
                names.Add(CardCollectionModel.Instance.ThemeReopenInUse.GetCardUIName(UIType.UIOpenCard));
            }
            return names.ToArray();
        }

        public static string[] CardAllUINames()
        {
            var uiNames = CardUINames().ToList();
            uiNames.AddRange(CardReopenUINames().ToList());
            return uiNames.ToArray();
        }
    }
}