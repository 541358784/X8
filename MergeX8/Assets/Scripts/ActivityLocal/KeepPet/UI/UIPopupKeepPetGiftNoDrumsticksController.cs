using System.Collections.Generic;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKeepPetGiftNoDrumsticksController:UIWindowController
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
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KeepPetFeed2);
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIKeepPetMainController>(UINameConst.UIKeepPetMain);
            if (mainUI)
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                {
                    MainOrderManager.Instance.LocationOrder(MainOrderType.KeepPet);
                }
                else
                {
                    SceneFsm.mInstance.TransitionGame(type:MainOrderType.KeepPet);
                }
            });
        });
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.KeepPetFeed2))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(StartBtn.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.KeepPetFeed2, StartBtn.transform as RectTransform,
                topLayer: topLayer);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.KeepPetFeed2, "");
        }
    }

    public static UIPopupKeepPetGiftNoDrumsticksController Open()
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetGiftNoDrumsticks) as
            UIPopupKeepPetGiftNoDrumsticksController;
    }
}