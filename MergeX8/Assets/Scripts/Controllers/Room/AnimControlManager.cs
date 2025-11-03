using System;
using System.Collections;
using System.Collections.Generic;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.Model;
using Framework;
using Gameplay;
using Gameplay.UI;
using GamePool;
using UnityEngine;
using UnityEngine.UI;

public enum AnimKey
{
    Main_Bottom,
    Main_Group,
    Main_ResBar,
    Der_Select,
    Der_BuyNode,
    Der_Lockode,
    
    Farm_Contrl,
    Farm_Seed,
    Farm_Machine,
    Farm_Buy,
    Farm_Top,
}

public class AnimStruct
{
    public GameObject animGameObj;
    public float moveTime = 0.3f;
    public Vector2 initPositon;
    public Vector2 endPosition;
    public RectTransform rectTransform;
    public bool isShow;
    public Ease ease = Ease.Linear;
    public bool isAnimator = false;
    public Animator animator;
}

public class AnimControlManager : Manager<AnimControlManager>
{
    private Dictionary<AnimKey, AnimStruct> animControlDatas = new Dictionary<AnimKey, AnimStruct>();

    public AnimStruct InitAnimControl(AnimKey animKey, GameObject animObj, bool isAnimator = false,
        float moveTime = 0.3f)
    {
        if (animObj == null)
            return null;

        if (animControlDatas.ContainsKey(animKey))
            return animControlDatas[animKey];

        AnimStruct animStruct = new AnimStruct();
        animStruct.animGameObj = animObj;
        animStruct.moveTime = moveTime;
        animStruct.rectTransform = animObj.transform as RectTransform;
        animStruct.initPositon = animStruct.rectTransform.anchoredPosition;
        animStruct.endPosition = animStruct.rectTransform.anchoredPosition;
        animStruct.isShow = true;
        animStruct.isAnimator = isAnimator;
        if (isAnimator)
            animStruct.animator = animObj.GetComponent<Animator>();

        switch (animKey)
        {
            case AnimKey.Main_Bottom:
            {
                animStruct.endPosition.y = -460;
                break;
            }
            case AnimKey.Der_Select:
            {
                animStruct.endPosition.y = -460;
                break;
            }
            case AnimKey.Farm_Buy:
            {
                animStruct.endPosition.y = -260;
                break;
            }
            case AnimKey.Farm_Top:
            {
                animStruct.endPosition.y = 400;
                break;
            }
            case AnimKey.Farm_Seed:
            case AnimKey.Farm_Machine:
            case AnimKey.Farm_Contrl:
            {
                animStruct.endPosition.y = -460;
                break;
            }
            case AnimKey.Der_BuyNode:
            case AnimKey.Der_Lockode:
            {
                animStruct.endPosition.y = -265;
                animStruct.initPositon.y += CommonUtils.SafeAreaOffset();
                break;
            }
            
            
        }

        animControlDatas.Add(animKey, animStruct);

        return animStruct;
    }

    public void AnimShow(AnimKey animKey, bool isShow, bool isImmediately = false, Action endCall = null, bool isForce = false)
    {
        if (!isForce && FarmModel.Instance.IsFarmModel())
        {
            if (animKey == AnimKey.Main_Bottom)
                animKey = AnimKey.Farm_Contrl;
        }
        
        if (!animControlDatas.ContainsKey(animKey))
        {
            if (endCall != null)
                endCall();
            return;
        }

        if (animControlDatas[animKey].isShow == isShow)
        {
            if (endCall != null)
                endCall();
            return;
        }

        animControlDatas[animKey].isShow = isShow;

        animControlDatas[animKey].animGameObj.transform.DOKill();
        Vector2 position = isShow ? animControlDatas[animKey].initPositon : animControlDatas[animKey].endPosition;

        if (animControlDatas[animKey].isAnimator)
        {
            string animName = "appear";
            if (!isShow)
                animName = "disappear";

            if (animControlDatas[animKey].animator == null)
            {
                if (endCall != null)
                    endCall();
                return;
            }

            CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(animControlDatas[animKey].animator,
                animName, null,
                () =>
                {
                    if (endCall != null)
                        endCall();
                }));
            return;
        }

        if (isImmediately)
        {
            animControlDatas[animKey].rectTransform.anchoredPosition = position;

            if (endCall != null)
                endCall();
            return;
        }

        animControlDatas[animKey].rectTransform.DOAnchorPos(position, animControlDatas[animKey].moveTime)
            .SetEase(animControlDatas[animKey].ease).OnComplete(() =>
            {
                if (endCall != null)
                    endCall();
            });
    }

    public void RemoveAnim(AnimKey animKey)
    {
        if (!animControlDatas.ContainsKey(animKey))
            return;

        animControlDatas.Remove(animKey);
    }

    public bool IsShow(AnimKey animKey)
    {
        if (!animControlDatas.ContainsKey(animKey))
            return false;

        return animControlDatas[animKey].isShow;
    }

    public void Destory()
    {
        if (animControlDatas == null)
            return;

        animControlDatas.Clear();
    }
}