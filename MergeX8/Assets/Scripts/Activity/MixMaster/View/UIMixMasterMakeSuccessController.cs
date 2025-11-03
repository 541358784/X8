using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIMixMasterMakeSuccessController : UIWindowController
{
    public static UIMixMasterMakeSuccessController Open(MixMasterFormulaConfig formula,bool isFirst, Action callback = null)
    {
        var openView =
            UIManager.Instance.GetOpenedUIByPath<UIMixMasterMakeSuccessController>(UINameConst
                .UIMixMasterMakeSuccess);
        if (openView)
        {
            openView.CloseWindowWithinUIMgr();
        }

        return UIManager.Instance.OpenUI(UINameConst.UIMixMasterMakeSuccess, formula,isFirst, callback) as
            UIMixMasterMakeSuccessController;
    }

    public MixMasterFormulaConfig Formula;
    public Action Callback;
    private Button CloseBtn;
    private bool IsFirst;
    private Image FormulaIcon;
    private Transform FirstTag;
    private LocalizeTextMeshProUGUI FormulaName;
    private Image RewardIcon;
    private LocalizeTextMeshProUGUI RewardCount;
    

    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() => { AnimCloseWindow(Callback); });
        FormulaIcon = GetItem<Image>("Root/NewMake/Icon");
        FirstTag = transform.Find("Root/NewMake/Image");
        FormulaName = GetItem<LocalizeTextMeshProUGUI>("Root/NewMake/Text");
        RewardIcon = GetItem<Image>("Root/Reward/Icon");
        RewardCount = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/Text");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Formula = objs[0] as MixMasterFormulaConfig;
        IsFirst = (bool) objs[1];
        if (objs.Length > 2)
        {
            Callback = objs[2] as Action;
        }
        FirstTag.gameObject.SetActive(IsFirst);
        FormulaIcon.sprite = Formula.GetFormulaIcon();
        FormulaName.SetTerm(Formula.NameKey);
        var rewards = IsFirst
            ? CommonUtils.FormatReward(Formula.FirstRewardId, Formula.FirstRewardNum)
            : CommonUtils.FormatReward(Formula.RepeatRewardId, Formula.RepeatRewardNum);
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