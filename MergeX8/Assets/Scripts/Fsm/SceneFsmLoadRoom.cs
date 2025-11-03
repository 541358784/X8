using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Decoration;
using Decoration.DynamicMap;
using DragonPlus;
using DG.Tweening;
using DragonU3DSDK.Asset;
using DragonPlus.Config;
using DragonU3DSDK;
using Framework;
using Gameplay;
using GamePool;
using Manager;
using MiniGame;

public class SceneFsmLoadRoom : IFsmState
{
    public StatusType Type => StatusType.LoadRoom;

    private StatusType nextSceneType;
    private List<object> param = new List<object>();
    private bool isUpdate = true;
    private bool isClientInited = false;

    private float _curTime = 0;
    public void Enter(params object[] objs)
    {
        isUpdate = true;

        if (objs.Length <= 0)
            return;

        param.Clear();
        nextSceneType = (StatusType) objs[0];
        for (int i = 0; i < objs.Length; i++)
        {
            if (i == 0)
                continue;

            param.Add(objs[i]);
        }
        
        GameModeManager.Instance.InitGameMode();

        isClientInited = SceneFsm.mInstance.ClientInited;
        if (!isClientInited)
        {
            LoadingController loadingCtr = LoadingController.ShowLoading();
            loadingCtr.StartLoading();
            EventDispatcher.Instance.AddEventListener(EventEnum.LOADING_FINISH, LoadingFinish);
        }
        else
        {
            TransitionLogic();
        }
    }
    public void LateUpdate(float deltaTime)
    {
        
    }
    public void Exit()
    {
        DOTween.KillAll(true);

        LoadingController.HideLoading();
        UIManager.Instance.CloseUI(UINameConst.UILogin, true);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.LOADING_FINISH, LoadingFinish);
        UIHomeMainController.mainController = null;
        CoroutineManager.Instance.StopAllCoroutines();
    }

    public void Update(float deltaTime)
    {
        if (!isUpdate)
            return;

        if (!isClientInited)
            return;

        switch (nextSceneType)
        {
            case StatusType.Home:
            {
                if (!DecoManager.Instance.IsWorldReady)
                    return;

                _curTime += deltaTime;
                if(_curTime < 0.3f)
                    return;
                
                SceneFsm.mInstance.ChangeState(nextSceneType, param.ToArray());
                isUpdate = false;
                break;
            }
        }
    }

    private void TransitionLogic()
    {
        switch (nextSceneType)
        {
            case StatusType.Home:
            {
                if (param.Count <= 0)
                    return;

                int roomId = (int) param[0];
                param.RemoveAt(0);
                
                Action DecoAction = () =>
                {
                    DecoManager.Instance.EnableUpdate = true;
                    UIRoot.Instance.mWorldUIRoot.gameObject.SetActive(true);
                    DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(true);
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
                };
                
                var gameMode = GameModeManager.Instance.GetGameMode();
                switch (gameMode)
                {
                    case GameModeManager.GameMode.DecoAndMerge:
                    {
                        DecoAction();
                        break;
                    }
                    case GameModeManager.GameMode.MiniAndMerge:
                    {
                        var currentGameMode = GameModeManager.Instance.GetCurrenGameMode();

                        switch (currentGameMode)
                        {
                            case GameModeManager.CurrentGameMode.Deco:
                            {
                                DecoAction();
                                break;
                            }
                            case GameModeManager.CurrentGameMode.MiniGame:
                            {
                                MiniGameModel.Instance.LoadMiniGame();
                                DecoManager.Instance.CurrentWorld?.HideByPosition();
                                break;
                            }
                        }

                        break;
                    }
                }
                break;
            }
            default:
                SceneFsm.mInstance.ChangeState(nextSceneType, param.ToArray());
                break;
        }
    }

    private void LoadingFinish(BaseEvent e)
    {
        if (isClientInited)
            return;

        isClientInited = true;

        TransitionLogic();
    }
}