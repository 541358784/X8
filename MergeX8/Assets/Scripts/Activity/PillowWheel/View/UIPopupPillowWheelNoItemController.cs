using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupPillowWheelNoItemController:UIWindowController
{
    private Button CloseBtn;
    private Button StartBtn;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(()=>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PillowWheelToMerge);
            var mainUI = UIPillowWheelMainController.Instance;
            if (mainUI)
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(StartBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.PillowWheelToMerge, StartBtn.transform as RectTransform,
            topLayer: topLayer);
        if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.PillowWheelToMerge, null))
        {
            
        }
    }

    public static UIPopupPillowWheelNoItemController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupPillowWheelNoItem) as
            UIPopupPillowWheelNoItemController;
    }
}