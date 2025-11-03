using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using Deco.Node;
using Decoration;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;

public class SuggestBubbleController : FollowTargetBase
{
    private RectTransform _buttonRectTransform;
    private RectTransform _canvasRectTransform;

    private Button _selectButton;
    
    public static string BUBBLE_PREFAB_PATH = "Prefabs/Home/UIWorldMainBubble";

    private Image _icon;
    public bool Visible
    {
        get
        {
            var leftOut = _buttonRectTransform.localPosition.x + _buttonRectTransform.sizeDelta.x / 2f <
                          -_canvasRectTransform.sizeDelta.x / 2f;
            if (leftOut) return false;
            var rightOut = _buttonRectTransform.localPosition.x - _buttonRectTransform.sizeDelta.x / 2f >
                           _canvasRectTransform.sizeDelta.x / 2f;
            if (rightOut) return false;
            var topOut = _buttonRectTransform.localPosition.y + _buttonRectTransform.sizeDelta.y / 2f >
                         _canvasRectTransform.sizeDelta.y / 2f;
            if (topOut) return false;
            var bottomOut = _buttonRectTransform.localPosition.y + _buttonRectTransform.sizeDelta.y / 2f <
                            -_canvasRectTransform.sizeDelta.y / 2f;
            if (bottomOut) return false;

            return true;
        }
    }

    void Awake()
    {
        var t = this.transform;
        _selectButton = t.GetComponent<Button>();
        _selectButton.onClick.AddListener(OnBtnSelectNode);

        _buttonRectTransform = transform as RectTransform;
        _canvasRectTransform = UIRoot.Instance.mRootCanvas.transform as RectTransform;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TouchBubble, transform as RectTransform, topLayer:topLayer);

        _icon = transform.Find("Root/Icon").GetComponent<Image>();
        
        Init();
    }

    public override void BindNode(DecoNode node)
    {
        base.BindNode(node);
        
        _node = node;
        gameObject.SetActive(false);

        Bind();
        
        if(_icon == null)
            _icon = transform.Find("Root/Icon").GetComponent<Image>();
        
        if(_icon == null)
            return;
        
        _icon.sprite = UserData.GetResourceIcon(node._data._config.costId, UserData.ResourceSubType.Big);
    }

    public override DecoNode GetNode()
    {
        return _node;
    }

    private void OnBtnSelectNode()
    { 
        if( SceneFsm.mInstance.GetCurrSceneType()!=StatusType.Home && SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome && SceneFsm.mInstance.GetCurrSceneType() != StatusType.EnterFarm) 
            return;
        DecoManager.Instance.SelectNode(_node);
    }

    public void HideNodeBuyUI()
    {
        gameObject.SetActive(false);
    }
}
