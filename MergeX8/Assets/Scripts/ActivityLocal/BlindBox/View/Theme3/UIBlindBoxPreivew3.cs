using System.Collections.Generic;
using Dlugin;
using DragonPlus;
using UnityEngine.UI;

public class UIBlindBoxPreview3 : UIBlindBoxPreviewBase
{
    private Image Icon;
    private Image NoIcon;
    private LocalizeTextMeshProUGUI TitleText;
    private LocalizeTextMeshProUGUI TipText;
    public override void PrivateAwake()
    {
        Icon = GetItem<Image>("Root/Icon");
        NoIcon = GetItem<Image>("Root/NoIcon");
        TitleText = GetItem<LocalizeTextMeshProUGUI>("Root/TitleText");
        TipText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
    }

    public override void InitCloseBtn()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Icon.sprite = Config.GetItemSprite();
        NoIcon.sprite = Config.GetItemSprite(true);
        var count = Storage.CollectItems.GetValueOrDefault(Config.Id);
        var showNoIcon = count == 0;
        Icon.gameObject.SetActive(!showNoIcon);
        NoIcon.gameObject.SetActive(showNoIcon);
        TitleText.SetTerm(Config.NameKey);
        TipText.SetTerm(Config.TipKey);
    }
}