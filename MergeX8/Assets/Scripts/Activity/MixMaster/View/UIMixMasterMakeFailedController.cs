using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIMixMasterMakeFailedController : UIWindowController
{
    public static UIMixMasterMakeFailedController Open(Action callback = null)
    {
        var openView =
            UIManager.Instance.GetOpenedUIByPath<UIMixMasterMakeFailedController>(UINameConst
                .UIMixMasterMakeFailed);
        if (openView)
        {
            openView.CloseWindowWithinUIMgr();
        }

        return UIManager.Instance.OpenUI(UINameConst.UIMixMasterMakeFailed, callback) as
            UIMixMasterMakeFailedController;
    }

    public Action Callback;
    private Button CloseBtn;
    private Image RewardIcon;
    private LocalizeTextMeshProUGUI RewardCount;
    

    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() => { AnimCloseWindow(Callback); });
        RewardIcon = GetItem<Image>("Root/Reward/Icon");
        RewardCount = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/Text");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (objs.Length > 0)
        {
            Callback = objs[0] as Action;
        }
        var rewards = CommonUtils.FormatReward(MixMasterModel.Instance.GlobalConfig.FailRewardId, MixMasterModel.Instance.GlobalConfig.FailRewardNum);
        if (rewards.Count == 0)
        {
            transform.Find("Root/Reward").gameObject.SetActive(false);
        }
        else
        {
            RewardIcon.sprite = UserData.GetResourceIcon(rewards[0].id,UserData.ResourceSubType.Big);
            RewardCount.SetText(rewards[0].count.ToString());
        }
    }
}