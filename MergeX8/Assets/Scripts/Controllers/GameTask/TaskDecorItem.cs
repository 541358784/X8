using System;
using System.Collections;
using System.Collections.Generic;
using Deco.Area;
using Deco.Node;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using Gameplay.UI.Capybara;
using UnityEngine;
using UnityEngine.UI;


public class TaskDecorItem : MonoBehaviour
{
    private int _areaId = 0;
    private DecoArea _decoArea;
    private List<DecoNode> _nodes = null;

    private LocalizeTextMeshProUGUI _title;
    private LocalizeTextMeshProUGUI _contentText;

    private Image _areaImage;
    

    private Button _infoBtn;
    
    private GameObject _taskCell;
    private GameObject _content;

    private List<TaskDecorItemCell> _decorItemCells = new List<TaskDecorItemCell>();
    public List<TaskDecorItemCell> DecorItemCells => _decorItemCells;
    
    private void Awake()
    {
        _taskCell = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Home/UITeskGroupCell");
        _content = transform.Find("TeskGroup").gameObject;
        
        _title = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        _contentText = transform.Find("ContentText").GetComponent<LocalizeTextMeshProUGUI>();

        _areaImage = transform.Find("Icon").GetComponent<Image>();
        
        _infoBtn = transform.Find("HelpButton").GetComponent<Button>();
        _infoBtn.onClick.AddListener(OpenInfo);
    }

    public void InitData(int areaId, List<DecoNode> nodes)
    {
        _areaId = areaId;
        _nodes = nodes;
        _decoArea = DecoManager.Instance.FindArea(_areaId);
        
        InitView();
        InitAllNodeCell();
    }

    public bool IsCoinNode()
    {
        if (_nodes == null || _nodes.Count == 0)
            return false;

        foreach (var node in _nodes)
        {
            if (node._data._config.costId == (int)UserData.ResourceId.Coin)
                return true;
        }

        return false;
    }
    

    private void OpenInfo()
    {
        UIPopupTaskController.CloseView(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupTaskHelp, _decoArea, _nodes);
        });
    }

    private void InitAllNodeCell()
    {
        if(_nodes == null || _nodes.Count == 0)
            return;
        
        foreach (var node in _nodes)
        {
            if (CapybaraManager.Instance.IsOpenCapybara())
            {
                if(node.Config.costId == (int)UserData.ResourceId.Seal)
                    continue;
            }
            else
            {
                if(node.Config.costId == (int)UserData.ResourceId.Capybara)
                    continue;
            }
            
            var obj = GameObject.Instantiate(_taskCell, _content.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            
            _decorItemCells.Add(obj.AddComponent<TaskDecorItemCell>());
            _decorItemCells[_decorItemCells.Count-1].InitData(node);
        }
        
        
        List<TaskDecorItemCell> firstList = new List<TaskDecorItemCell>();
        List<TaskDecorItemCell> secondList = new List<TaskDecorItemCell>();
        List<TaskDecorItemCell> lastList = new List<TaskDecorItemCell>();

        _decorItemCells.ForEach(a =>
        {
            if(a._node.Config.costId == (int)UserData.ResourceId.Coin)
                firstList.Add(a);
            else if(a._node.Config.costId == (int)UserData.ResourceId.Seal || a._node.Config.costId == (int)UserData.ResourceId.Dolphin || a._node.Config.costId == (int)UserData.ResourceId.Capybara)
                secondList.Add(a);
            else 
                lastList.Add(a);
        });
        _decorItemCells.Clear();
        _decorItemCells.AddRange(firstList);
        _decorItemCells.AddRange(secondList);
        _decorItemCells.AddRange(lastList);
        for (int i = 0; i < _decorItemCells.Count; i++)
        {
            _decorItemCells[i].transform.SetSiblingIndex(i);
        }
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