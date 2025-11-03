using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeGiftBagProgressBubble : MonoBehaviour
{
    private StorageGiftBagProgress Storage;
    private Animator Animator;
    public void SetStorage(StorageGiftBagProgress storage)
    {
        Storage = storage;
        RefreshView();
    }

    private void Awake()
    {
        Animator = transform.GetComponent<Animator>();
    }

    public void RefreshView()
    {
        if (Storage == null)
            return;
    }

    public void Show()
    {
        Animator.PlayAnimation("appear");
    }
    public void Shake()
    {
        Animator.PlayAnimation("shake");
    }
    public void Hide()
    {
        Animator.PlayAnimation("disappear");
    }
    private void OnDestroy()
    {
    }
}