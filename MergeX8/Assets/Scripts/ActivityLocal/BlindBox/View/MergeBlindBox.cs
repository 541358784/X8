using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.CoinRush;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MergeBlindBox : MonoBehaviour
{
    private Button _btn;

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        
        InvokeRepeating("RefreshView", 0, 1f);
    }

    public void OnClick()
    {
       
    }

    private void RefreshView()
    {
        gameObject.SetActive(BlindBoxModel.Instance.IsUnlock);
    }
}