using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class Main_HomeController : SuperMainController
{
    private void Awake()
    {
        InitUI();
    }

    private void InitUI()
    {
        CurrencyGroupManager.Instance.Show();

      
    }

    public override void Show()
    {
        UIHomeMainController.UpdateAuixControllers();
        base.Show();
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        base.Hide();
        // _groupController?.Hide();
        //gameObject.SetActive(false);
    }
}