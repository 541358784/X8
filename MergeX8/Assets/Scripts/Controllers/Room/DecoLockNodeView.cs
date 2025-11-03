using System;
using System.Collections.Generic;
using Deco.Area;
using Deco.Node;
using Deco.World;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class DecoLockNodeView : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _title;    
    private LocalizeTextMeshProUGUI _describeText;

    private Button _infoButton;

    private DecoNode _decoNode;
    private DecoArea _decoArea;
    private void Awake()
    {
        _title = transform.Find("Title/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _describeText = transform.Find("DescribeText").GetComponent<LocalizeTextMeshProUGUI>();

        transform.Find("HelpGroup").gameObject.SetActive(false);
    }

    public void SetNodeData(DecoNode nodeData, int areaId)
    {
        _decoNode = nodeData;

        DecoWorld.AreaLib.TryGetValue(areaId, out _decoArea);
        
        _title.SetTerm(_decoArea._data._config.areaName);
        _describeText.SetTerm(_decoArea._data._config.areaDesc);
    }

    private void OnInfoButton()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupTaskHelp, _decoArea, null);
    }
}