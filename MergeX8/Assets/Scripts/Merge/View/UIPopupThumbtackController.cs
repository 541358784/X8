/****************************************************
    文件：MergeSpliteController.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： 2021-11-23-10:19:27
    功能：....
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupThumbtackController : UIWindowController
{
    private Action _callBack;
    private Image _image;
    private Button _buttonBreak;
    private Button _buttonCancel;
    private Button _buttonClose;
    public override void PrivateAwake()
    {
        _image= GetItem<Image>("Root/Content/Icon");
        _buttonBreak=GetItem<Button>("Root/ButtonGroup/ButtonBreak");
        _buttonBreak.onClick.AddListener(OnBtnBreak);
        _buttonCancel=GetItem<Button>("Root/ButtonGroup/ButtonCancel");
        _buttonCancel.onClick.AddListener(OnBtnClose);
        _buttonClose=GetItem<Button>("Root/BG/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
    }

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }

    private void OnBtnBreak()
    {
        AnimCloseWindow(() =>
        {
            _callBack?.Invoke();
        });
    }

    public void Init(Action callBack, TableMergeItem curMergeItem)
    {
        _callBack = callBack;
        _image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(curMergeItem.image);
    }
}