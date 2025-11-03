using System;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;

public class UIMonopolyMiniGameRewardController:UIWindowController
{
    private Image Icon;
    private LocalizeTextMeshProUGUI NumText;
    private Button CloseBtn;
    private Action Callback;
    public override void PrivateAwake()
    {
        Icon = GetItem<Image>("Root/Reward/Image");
        NumText = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/TextNum");
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        var resId = (int) objs[0];
        var count = (int) objs[1];
        var resultId = (int) objs[2];
        Callback = objs[3] as Action;
        if (resId > 0)
            Icon.sprite = UserData.GetResourceIcon(resId, UserData.ResourceSubType.Big);
        NumText.SetText(count.ToString());
        var effect = transform.Find("Root/Reward/FX_0" + (resultId + 1));
        effect.gameObject.SetActive(false);
        effect.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound("sfx_easter2024_mini_win");
    }

    public void OnClickCloseBtn()
    {
        Callback();
        AnimCloseWindow();
    }
    public static UIMonopolyMiniGameRewardController Open(int resId,int count,int resultId,Action callback)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIMonopolyMiniGameReward, resId,count,resultId,callback) as
            UIMonopolyMiniGameRewardController;
    }
}