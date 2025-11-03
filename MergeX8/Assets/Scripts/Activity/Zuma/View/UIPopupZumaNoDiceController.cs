using System;
using System.Linq;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIPopupZumaNoDiceController:UIWindowController
{
    private Button CloseBtn;
    private StorageZuma Storage;
    private Button EnterMergeBtn;
    private LocalizeTextMeshProUGUI BallCountText;
    private Button BuyBallBtn;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(()=>AnimCloseWindow());
        EnterMergeBtn = GetItem<Button>("Root/ButtonPlay");
        EnterMergeBtn.onClick.AddListener(() =>
        {
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIZumaMainController>(UINameConst.UIZumaMain);
            if (mainUI && !mainUI.IsPlaying())
                mainUI.AnimCloseWindow();
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                {
                    SceneFsm.mInstance.TransitionGame();
                }
            });
        });
        BallCountText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        EventDispatcher.Instance.AddEvent<EventZumaDiceCountChange>(OnBallCountChange);
        BuyBallBtn = GetItem<Button>("Root/ButtonBuy");
        BuyBallBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow(()=>UIZumaGiftBagController.Open());
        });
    }

    public void OnBallCountChange(EventZumaDiceCountChange evt)
    {
        BallCountText.SetText(evt.TotalValue.ToString());   
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventZumaDiceCountChange>(OnBallCountChange);
    }

    public void OnDiceCountChange(EventZumaDiceCountChange evt)
    {
        // DiceCountText.SetText(Storage.DiceCount.ToString());
    }
    private int LastDayId;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageZuma;
        BallCountText.SetText(Storage.BallCount.ToString());
    }
    
    public static UIPopupZumaNoDiceController Open(StorageZuma storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIZumaNoItem,storage) as
            UIPopupZumaNoDiceController;
    }
}