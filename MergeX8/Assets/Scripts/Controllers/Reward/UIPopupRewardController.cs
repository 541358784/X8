using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public enum ShowRewardType
{
    Common,
    RoomBox,
    TaskBox,
    HappyGo,
}

public class UIPopupRewardController : UIWindowController
{
    private List<RewardData> rewardDatas = new List<RewardData>();
    private List<RewardData> showRewardDatas = new List<RewardData>();

    private Button getRewardButton = null;

    private List<ResData> resDatas = null;
    private ShowRewardType rewardType;
    private Action animEndCall;
    private Action clickGetCall;

    private float animStep = 0.05f;
    private float waitTime = 0.1f;

    private Animator _animator;

    private UICurrencyGroupController resController = null;

    private int[][] showOrder = new[]
    {
        new[] {6},
        new[] {1, 3},
        new[] {1, 2, 3},
        new[] {1, 2, 3, 6},
        new[] {0, 1, 2, 3, 4},
        new[] {0, 1, 2, 3, 4, 6},
        new[] {0, 1, 2, 3, 4, 5, 7},
        new[] {0, 1, 2, 3, 4, 5, 6, 7},
    };

    private bool animFinished = false;

    public override void PrivateAwake()
    {
        getRewardButton = GetItem<Button>("Root/getRward");
        getRewardButton.onClick.AddListener(OnClickGetReward);

        _animator = GetItem<Animator>(gameObject);

        for (int i = 1; i < 9; i++)
        {
            RewardData rdData = new RewardData();

            GameObject rdObj = GetItem("Root/rewardGroup/reward" + i);
            rdData.gameObject = rdObj;
            rdData.image = GetItem<Image>("Icon", rdObj);
            rdData.tipIcon = GetItem<Image>("Icon/TipIcon", rdObj);
            rdData.numText = GetItem<LocalizeTextMeshProUGUI>("Text", rdObj);
            rdData.animator = GetItem<Animator>(rdObj);
            rdData.SetActive(false);

            rewardDatas.Add(rdData);
        }
    }

    //param
    //1 list<restdata>
    //2 autoAddRes
    //3 showType
    //4 bi
    //5 animEndCall
    //6 clickGetCall
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow();

        if (objs == null || objs.Length == 0)
            return;

        resDatas = GetParamsValue<List<ResData>>(0, objs);
        resController = GetParamsValue<UICurrencyGroupController>(1, objs);
        bool autoAddRes = GetParamsValue<bool>(2, objs);
        rewardType = GetParamsValue<ShowRewardType>(3, objs);
        GameBIManager.ItemChangeReasonArgs bi = GetParamsValue<GameBIManager.ItemChangeReasonArgs>(4, objs);
        animEndCall = GetParamsValue<Action>(5, objs);
        clickGetCall = GetParamsValue<Action>(6, objs);

        string animName = "appearCommon";
        switch (rewardType)
        {
            case ShowRewardType.RoomBox:
            {
                animName = "appear";
                break;
            }
            case ShowRewardType.TaskBox:
            {
                animName = "appearCommon02";
                break;
            }
        }

        AudioManager.Instance.PlaySound(30);
        _animator.Play(animName, 0, 0);

        if (resDatas == null || resDatas.Count == 0)
            return;
        // for(int i =0; i < 7; i++)
        //     resDatas.Add(resDatas[0]);

        int count = Math.Min(resDatas.Count, showOrder.Length);
        count = count - 1;
        int[] order = showOrder[count];

        if (resDatas.Count > count+1)
        {
            var otherRes = new List<ResData>();
            for (var i = count + 1; i < resDatas.Count; i++)
            {
                otherRes.Add(resDatas[i]);
            }
            CommonRewardManager.Instance.PopCommonReward(otherRes, resController, false, bi, animEndCall, clickGetCall);
        }
        for (int i = 0; i <= count; i++)
        {
            int index = order[i];
            rewardDatas[index].UpdateReward(resDatas[i]);

            showRewardDatas.Add(rewardDatas[index]);

            // if (!autoAddRes || resDatas[i].isBuilding)
            //     continue;
            
            // UserData.Instance.AddRes(resDatas[i], bi, false, rewardType);
        }

        if (autoAddRes)
        {
            for (var i = 0; i < resDatas.Count; i++)
            {
                if (resDatas[i].isBuilding)
                    continue;
                UserData.Instance.AddRes(resDatas[i], bi, false, rewardType);
            }   
        }

        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.REWARD_POPUP);
    }

    public void OnClickGetReward()
    {
        if (!animFinished)
            return;

        animFinished = false;

        getRewardButton.gameObject.SetActive(false);
        _animator.Play("disappear", 0, 0);

        clickGetCall?.Invoke();

        FlyGameObjectManager.Instance.FlyObject(showRewardDatas, resController, () =>
        {
            if (ShowRewardType.HappyGo == rewardType)
            {
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
            }
            else
            {
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            }

            CloseWindowWithinUIMgr(true);
            
            if(CommonRewardManager.Instance.PopupCacheReward() == null)
                animEndCall?.Invoke();
            
            foreach (var resData in showRewardDatas)
            {
                GameObject.Destroy(resData.gameObject);
            }
        });
    }

    public void OnAnimationFinished()
    {
        StartCoroutine(PlayRewardAnimation());
    }

    private IEnumerator PlayRewardAnimation()
    {
        if (showRewardDatas == null || showRewardDatas.Count == 0)
        {
            animFinished = true;
            yield break;
        }

        ShakeManager.Instance.ShakeMedium();

        for (int i = 0; i < showRewardDatas.Count; i++)
        {
            showRewardDatas[i].SetActive(true);
            showRewardDatas[i].PlayAnimation("appear");

            yield return new WaitForSeconds(animStep);
        }

        yield return new WaitForSeconds(waitTime);
        animFinished = true;
    }

    private T GetParamsValue<T>(int index, object[] objects)
    {
        if (objects == null || objects.Length == 0)
            return default(T);

        if (index < 0 || index >= objects.Length)
            return default(T);

        return (T) objects[index];
    }
}