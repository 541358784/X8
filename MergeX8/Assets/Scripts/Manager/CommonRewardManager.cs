using System;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;


public class CommonRewardManager : Singleton<CommonRewardManager>
{
    private class CacheRewardData
    {
        public List<ResData> reward;
        public UICurrencyGroupController resController;
        public bool autoAddRes;
        public ShowRewardType type;
        public GameBIManager.ItemChangeReasonArgs bi;
        public System.Action animEndCall;
        public Action clickGetCall;
        public string uiPath;
        public UIWindowType windowType;
    }

    private Queue<CacheRewardData> cacheRewardDatas = new Queue<CacheRewardData>();

    public UIPopupRewardController PopRoomBoxReward(List<ResData> rewards, UICurrencyGroupController resController,
        System.Action animEndCall = null, Action clickGetCall = null)
    {
        return PopReward(rewards, resController, false, ShowRewardType.RoomBox,
            new GameBIManager.ItemChangeReasonArgs(), animEndCall, clickGetCall) as UIPopupRewardController;
    }

    public UIPopupRewardController PopCommonReward(List<ResData> rewards, UICurrencyGroupController resController,
        bool autoAddRes, GameBIManager.ItemChangeReasonArgs bi = new GameBIManager.ItemChangeReasonArgs(),
        System.Action animEndCall = null, Action clickGetCall = null,string uiPath = null, UIWindowType windowType = UIWindowType.Normal)
    {
        return PopReward(rewards, resController, autoAddRes, ShowRewardType.Common, bi, animEndCall, clickGetCall,uiPath, windowType) as
            UIPopupRewardController;
    }

    public UIPopupRewardController PopHappyGoReward(List<ResData> rewards, UICurrencyGroupController resController,
        bool autoAddRes, GameBIManager.ItemChangeReasonArgs bi = new GameBIManager.ItemChangeReasonArgs(),
        System.Action animEndCall = null, Action clickGetCall = null)
    {
        return PopReward(rewards, resController, autoAddRes, ShowRewardType.HappyGo, bi, animEndCall, clickGetCall) as
            UIPopupRewardController;
    }

    public UIPopupRewardController PopTaskBoxReward(List<ResData> rewards, UICurrencyGroupController resController,
        bool autoAddRes, GameBIManager.ItemChangeReasonArgs bi = new GameBIManager.ItemChangeReasonArgs(),
        System.Action animEndCall = null, Action clickGetCall = null)
    {
        return PopReward(rewards, resController, autoAddRes, ShowRewardType.TaskBox, bi, animEndCall, clickGetCall) as
            UIPopupRewardController;
    }

    public UIPopupActivityUnCollectRewardController PopActivityUnCollectReward(List<ResData> rewards,
        GameBIManager.ItemChangeReasonArgs bi, Action callback = null,Action onBtnClick = null)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIPopupActivityUnCollectReward, rewards, bi, callback,onBtnClick)
            as UIPopupActivityUnCollectRewardController;
    }

    public UIPopupRewardController PopReward(List<ResData> rewards, UICurrencyGroupController resController,
        bool autoAddRes, ShowRewardType type, GameBIManager.ItemChangeReasonArgs bi, Action animEndCall,
        Action clickGetCall,string uiPath = null, UIWindowType windowType = UIWindowType.Normal)
    {
        if (uiPath == null)
            uiPath = UINameConst.UIPopupReward;
        if (UIManager.Instance.GetOpenedUIByPath<UIPopupRewardController>(uiPath))
        {
            CacheRewardData cacheData = new CacheRewardData();
            cacheData.reward = rewards;
            cacheData.resController = resController;
            cacheData.autoAddRes = autoAddRes;
            cacheData.type = type;
            cacheData.bi = bi;
            cacheData.clickGetCall = clickGetCall;
            cacheData.animEndCall = animEndCall;
            cacheData.uiPath = uiPath;
            cacheData.windowType = windowType;
            cacheRewardDatas.Enqueue(cacheData);
            return null;
        }

        return UIManager.Instance.OpenUI(uiPath, forceType:windowType, rewards, resController, autoAddRes, type,
            bi, animEndCall, clickGetCall) as UIPopupRewardController;
    }

    public UIPopupRewardController PopupCacheReward()
    {
        if (cacheRewardDatas == null || cacheRewardDatas.Count == 0)
            return null;

        CacheRewardData cacheData = cacheRewardDatas.Dequeue();

        return PopReward(cacheData.reward, cacheData.resController, cacheData.autoAddRes, cacheData.type, cacheData.bi,
            cacheData.clickGetCall, cacheData.animEndCall,cacheData.uiPath, cacheData.windowType);
    }
    
    public enum TipDirection
    {
        Top,
        Bottom,
        Left,
        Right,
    }
    
    public void ShowNormalBoxReward(Vector3 pos, List<ResData> resDatas,
        TipDirection dir = TipDirection.Top)
    {
        UIManager.Instance.OpenWindow(UINameConst.UICommonNormalBox, resDatas, pos, dir);
    }
}