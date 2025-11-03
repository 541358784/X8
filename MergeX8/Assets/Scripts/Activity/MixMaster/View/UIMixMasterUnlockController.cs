using System;
using DragonPlus;
using UnityEngine.UI;

public class UIMixMasterUnlockController : UIWindowController
{
    public static UIMixMasterUnlockController Open(MixMasterFormulaConfig formula,Action callback = null)
    {
        var openView =
            UIManager.Instance.GetOpenedUIByPath<UIMixMasterUnlockController>(UINameConst.UIMixMasterUnlock);
        if (openView)
        {
            openView.CloseWindowWithinUIMgr();
        }

        return UIManager.Instance.OpenUI(UINameConst.UIMixMasterUnlock, formula, callback) as
            UIMixMasterUnlockController;
    }

    public MixMasterFormulaConfig Formula;
    public Action Callback;
    private Button CloseBtn;
    private Image FormulaIcon;
    private LocalizeTextMeshProUGUI FormulaName;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(Callback);
        });
        FormulaIcon = GetItem<Image>("Root/Reward/Icon");
        FormulaName = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/Text");
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Formula = objs[0] as MixMasterFormulaConfig;
        if (objs.Length > 1)
        {
            Callback = objs[1] as Action;
        }
        FormulaIcon.sprite = Formula.GetFormulaIcon();
        FormulaName.SetTerm(Formula.NameKey);
    }
    
}