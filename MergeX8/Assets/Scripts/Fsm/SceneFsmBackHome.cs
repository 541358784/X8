using System;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using Decoration.Bubble;
using UnityEngine;
using DragonPlus;
using Farm.Model;
using Game;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;


public class SceneFsmBackHome : IFsmState
{
    public StatusType Type => StatusType.BackHome;

    public void Enter(params object[] objs)
    {
        Input.multiTouchEnabled = true;
        UIHomeMainController.ShowUI();

        FarmModel.Instance.AnimShow(true);
        
        UIPopupRVRewardController.InitRvReward();
        BackHomeControl.doitNode = null;
        BackHomeControl.doitItems = null;
        BackHomeControl.doitAction = null;
        BackHomeControl.opType = DecoOperationType.None;

        if (objs != null && objs.Length > 0)
        {
            BackHomeControl.opType = (DecoOperationType) objs[0];
            switch (BackHomeControl.opType)
            {
                case DecoOperationType.Buy:
                {
                    int nodeId = (int) objs[1];

                    BackHomeControl.doitNode = DecoManager.Instance.FindNode(nodeId);
                    break;
                }
                case DecoOperationType.Install:
                case DecoOperationType.Preview:
                {
                    BackHomeControl.doitItems = (List<int>) objs[1];
                    if(objs.Length >= 3)
                        BackHomeControl.doitAction = (Action) objs[2];
                    break;
                }
                case DecoOperationType.MiniGame:
                {
                    if(objs.Length >= 2)
                        BackHomeControl.miniGameCharpterId = (int) objs[1];
                    if (objs.Length >= 3)
                        BackHomeControl.miniGameCallBackAction = (Action)objs[2];
                    break;
                }
            }
        }

        UIRoot.Instance.EnableEventSystem = false;
        MailDataModel.Instance.RequestMailList();

        if (BackHomeControl.EnterFarm)
        {
            BackHomeControl.EnterFarm = false;
            if (!FarmModel.Instance.IsFarmModel())
            {
                SceneFsm.mInstance.ChangeState(StatusType.EnterFarm);
                return;
            }
        }
        if (UIKapibalaMainController.Instance || 
            UIKapiScrewMainController.Instance || 
            UIKapiTileMainController.Instance)
        {
            UIRoot.Instance.EnableEventSystem = true;
        }
        else
        {
            BackHomeControl.BackHomeLogic();   
        }
    }


    public void Update(float deltaTime)
    {
        DecoManager.Instance?.Update(deltaTime);
    }

    public void LateUpdate(float deltaTime)
    {
        
    }
    public void Exit()
    {
        
    }
}