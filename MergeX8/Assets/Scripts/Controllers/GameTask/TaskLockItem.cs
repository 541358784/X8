using System;
using System.Collections.Generic;
using Deco.Area;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;


public class TaskLockItem : MonoBehaviour
{
    private DecoArea _decoArea;
    private DecoNode _decoNode;

    private LocalizeTextMeshProUGUI _title;
    private LocalizeTextMeshProUGUI _contentText;

    private Image _areaImage;
    
    private Button _startBtn;
    private Button _infoBtn;

    private void Awake()
    {
        _title = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        _contentText = transform.Find("ContentText").GetComponent<LocalizeTextMeshProUGUI>();

        _areaImage = transform.Find("Icon").GetComponent<Image>();
        
        _startBtn = transform.Find("Button").GetComponent<Button>();
        _startBtn.onClick.AddListener(OpenStart);
        _startBtn.gameObject.SetActive(false);
        
        _infoBtn = transform.Find("HelpButton").GetComponent<Button>();
        _infoBtn.onClick.AddListener(OpenInfo);
    }

    public void InitData(DecoArea area, DecoNode node)
    {
        _decoArea = area;
        _decoNode = node;
        
        InitView();
        InitAllNodeCell();
    }

    private void OpenInfo()
    {
        UIPopupTaskController.CloseView(() =>
        {
            List<DecoNode> lockNode = new List<DecoNode>();
            lockNode.Add(_decoNode);
            UIManager.Instance.OpenUI(UINameConst.UIPopupTaskHelp, _decoArea, lockNode);
        });
    }

    private void OpenStart()
    {
        
    }

    private void InitAllNodeCell()
    {
    }

    private void InitView()
    {
        if(_decoArea == null)
            return;
        
        _title.SetTerm(_decoArea._data._config.areaName);
        _contentText.SetTerm(_decoArea._data._config.areaDesc);
        
        _areaImage.sprite = CommonUtils.LoadAreaIconSprite(_decoArea._data._config.bigIcon);
    }
}