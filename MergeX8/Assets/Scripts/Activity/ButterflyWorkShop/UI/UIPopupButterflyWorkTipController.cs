using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupButterflyWorkTipController:UIWindowController
{
    private Button _playBtn;
    private Button _closeBtn;
    public override void PrivateAwake()
    {
        _playBtn = GetItem<Button>("Root/Button");
        _playBtn.onClick.AddListener(OnPlayBtn);
        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnCloseBtn);
    
    }

    public void OnPlayBtn()
    {
        AnimCloseWindow(() =>
        {
        });
    }

    public void OnCloseBtn()
    {
        AnimCloseWindow(() =>
        {
           
        });
    }
    
    
}