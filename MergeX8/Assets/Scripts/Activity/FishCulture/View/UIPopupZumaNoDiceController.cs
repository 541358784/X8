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

public class UIPopupFishCultureNoDiceController : UIWindowController
{
    private Button CloseBtn;
    private StorageFishCulture Storage;
    private Button EnterMergeBtn;

    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() => AnimCloseWindow());
        EnterMergeBtn = GetItem<Button>("Root/Button");
        EnterMergeBtn.onClick.AddListener(() =>
        {
            var mainUI =
                UIManager.Instance.GetOpenedUIByPath<UIFishCultureMainController>(UINameConst.UIFishCultureMain);
            if (Storage.IsEnd)
            {
                UIPopupFishExitController.Open((b) =>
                {
                    if (b)
                    {
                        if (mainUI && !mainUI.IsPlaying())
                        {
                            mainUI.AnimCloseWindow();
                        }

                        AnimCloseWindow(() =>
                        {
                            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                            {
                                SceneFsm.mInstance.TransitionGame();
                            }
                        });
                    }
                });
            }
            else
            {
                if (mainUI && !mainUI.IsPlaying())
                {
                    mainUI.AnimCloseWindow();
                }

                AnimCloseWindow(() =>
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                    {
                        SceneFsm.mInstance.TransitionGame();
                    }
                });
            }
        });
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageFishCulture;
    }

    public static UIPopupFishCultureNoDiceController Open(StorageFishCulture storage)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupFishCultureNoItem, storage) as
            UIPopupFishCultureNoDiceController;
    }
}