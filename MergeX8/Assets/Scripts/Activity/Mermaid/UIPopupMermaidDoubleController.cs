using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupMermaidDoubleController : UIWindowController
{
    private LocalizeTextMeshProUGUI _coolTime;
    
    public override void PrivateAwake()
    {
        _coolTime = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        
        GetItem<Button>("Root/ButtonClose").onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        
        GetItem<Button>("Root/Button").onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
        
        InvokeRepeating("InvokeUpdate", 0, 1);
        
        _coolTime.SetText(MultipleScoreModel.Instance.GetActiveTime(MultipleScoreModel.InfluenceFuncType.Mermaid));
    }

    private void InvokeUpdate()
    {
        _coolTime.SetText(MultipleScoreModel.Instance.GetActiveTime(MultipleScoreModel.InfluenceFuncType.Mermaid));
    }
    
   private static string coolTimeKey = "UIPopupMermaidDouble";
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Mermaid))
            return false;

        if (!MermaidModel.Instance.IsOpened())
            return false;

        if (!MultipleScoreModel.Instance.IsOpenActivity())
            return false;

        if (MultipleScoreModel.Instance.GetMultiple(MultipleScoreModel.InfluenceFuncType.Mermaid) <= 1)
            return false;
        
        if (!CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, coolTimeKey))
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupMermaidDouble);
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, coolTimeKey, CommonUtils.GetTimeStamp());
            return true;
        }
        
        return false;
    }
}