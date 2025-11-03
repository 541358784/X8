using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Decoration;
using DragonPlus;
using DG.Tweening;
using DragonU3DSDK.Asset;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Game;
using Gameplay;
using GamePool;
using Merge.Order;

public class SceneFsmTransitionHappyGo : IFsmState
{
    public StatusType Type => StatusType.Transition;

    private StatusType nextSceneType;
    private List<object> param = new List<object>();
    private bool isClientInited = false;

    public void Enter(params object[] objs)
    {
        // AudioManager.Instance.StopAllMusic();
        AudioManager.Instance.PlaySound(SfxNameConst.sfx_change_scene);
        if (objs.Length <= 0)
            return;

        param.Clear();
        nextSceneType = (StatusType) objs[0];

        for (int i = 1; i < objs.Length; i++)
        {
            param.Add(objs[i]);
        }

        DragonU3DSDK.Device.Instance.RemoveBackButtonCallback(BackHomeControl.BackButtonQuickApp);

        GuideSubSystem.Instance.CloseCurrent(true);

        MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.None, MergeBoardEnum.Main,true);
        UserGroupManager.Instance.UpdateSubUserGroup();
        
        EnterHomeLogic();
        CurrencyGroupManager.Instance.UpdateShowType(CurrencyGroupManager.CurrencyShowType.Home);
    }

    private void EnterGameLogic()
    {
        UIRoot.Instance.EnableEventSystem = false;
        GuideSubSystem.Instance.FinishCurrent();
        
        if (AnimControlManager.Instance.IsShow(AnimKey.Der_Select))
        {
            UIHomeMainController.HideUI(true, true);

            MainDecorationController.mainController.Hide();
            AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Der_Select, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false, true);

            PlayMergeAppearAim();
        }
        else if (AnimControlManager.Instance.IsShow(AnimKey.Main_Bottom))
        {
            UIHomeMainController.HideUI(true);
            
            MainDecorationController.mainController.Hide();
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, false, false);

            PlayMergeAppearAim();
        }
        else
        {
            UIHomeMainController.HideUI(true);
            
            MainDecorationController.mainController.Hide();
            AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Der_Select, false);
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false, true);

            PlayMergeAppearAim();
        }
    }

    private void PlayMergeAppearAim()
    {
        if (param != null && param.Count > 0)
        {
            MergeBoardEnum type =(MergeBoardEnum) param[0] ;
            if (type == MergeBoardEnum.HappyGo)
            {
                HappyGo_MergeAppearAim();
                CurrencyGroupManager.Instance.UpdateShowType(CurrencyGroupManager.CurrencyShowType.HappyGo);

                return;
            }
        }
        CurrencyGroupManager.Instance.UpdateShowType(CurrencyGroupManager.CurrencyShowType.Game);
        Main_MergeAppearAim();
    }

    private void Main_MergeAppearAim()
    {
        MainOrderManager.Instance.AutoTryFillOrder();
        
        MergeMainController mergeController =
            UIManager.Instance.OpenUI(UINameConst.MergeMain) as MergeMainController;
        if (mergeController == null)
        {
            UIRoot.Instance.EnableEventSystem = true;
            return;
        }

        MergeMainController.Instance.MergeBoard?.RefreshGridsStatus();

        EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate);
        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f,
            () => { AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true); }));
        mergeController.PlayAnimation(true, () => { MergeAppearAimCall(); });
    }
    private void HappyGo_MergeAppearAim()
    {
        MainOrderManager.Instance.AutoTryFillOrder();
        
        HappyGoMainController mergeController =
            UIManager.Instance.OpenUI(UINameConst.HappyGoMain) as HappyGoMainController;
        if (mergeController == null)
        {
            UIRoot.Instance.EnableEventSystem = true;
            return;
        }

        HappyGoMainController.Instance.mMergeBoard?.RefreshGridsStatus();
        CurrencyGroupManager.Instance.currencyController.SetEnergyShowType(false);
        EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate);
        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f,
            () => { AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true); }));
        mergeController.PlayAnimation(true, () => { MergeAppearAimCall(); });
    }
    private void MergeAppearAimCall()
    {
        CameraManager.MainCamera.gameObject.SetActive(false);
        
        Input.multiTouchEnabled = false;
        SceneFsm.mInstance.ChangeState(nextSceneType, param.ToArray());
        UIRoot.Instance.EnableEventSystem = true;
        if (nextSceneType == StatusType.HappyGoGame)
        {
            HappyGoMergeGuideLogic.Instance.CheckMergeGuide();
        }
        else
        {
            MergeGuideLogic.Instance.CheckMergeGuide();
        }
    }

    private void EnterHomeLogic()
    {
        UIManager.Instance.CloseUI(UINameConst.UIStore);
        MergeInfoView.Instance.CloseAllMergeInfo();

        UIRoot.Instance.EnableEventSystem = false;

        PlayMergeDisAppearAim();
    }

    private void PlayMergeDisAppearAim()
    {
        CameraManager.MainCamera.gameObject.SetActive(true);
        UIHomeMainController.HideUI(true, true);
        Input.multiTouchEnabled = true;

        EventDispatcher.Instance.DispatchEvent(EventEnum.UserDataUpdate);
        
        AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false);
        
        MergeMainController mergeController = UIManager.Instance.GetOpenedUIByPath(UINameConst.MergeMain) as MergeMainController;
        mergeController.PlayAnimation(false, () => { MergeDisAppearAimCall(); });
    }

    private void MergeDisAppearAimCall()
    {
        DragonU3DSDK.Device.Instance.AddBackButtonCallback(BackHomeControl.BackButtonQuickApp);
        UIManager.Instance.CloseUI(UINameConst.MergeMain);
        UIManager.Instance.CloseUI(UINameConst.HappyGoMain);
        CurrencyGroupManager.Instance.currencyController.SetEnergyShowType(true);
        UIManager.Instance.CloseUI(UINameConst.HappyGoUIStoreGam);
        UIManager.Instance.CloseUI(UINameConst.UIPopupHappyGoReward);
        //DragonPlus.ConfigHub.ConfigHubManager.Instance.UpdateRemoteConfig(true);
        ActivityManager.Instance.RequestActivityInfosFromServer();
        
        SceneFsm.mInstance.ChangeState(nextSceneType, param.ToArray());
    }

    public void Exit()
    {
    }

    public void Update(float deltaTime)
    {
    }
    public void LateUpdate(float deltaTime)
    {
        
    }
}