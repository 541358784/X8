using System;
using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.UI;

public class Main_LockController : SuperMainController
{
    private Image uiBgImage = null;

    private void Awake()
    {
        CommonUtils.NotchAdapte(transform.Find("Root") as RectTransform);
        uiBgImage = GetItem<Image>("UIBg");
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
    }
}