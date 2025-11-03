using System;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using Deco.Node;
using Decoration;
using Decoration.Bubble;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;

public class PlayBubbleController : FollowTargetBase
{
    private RectTransform _buttonRectTransform;
    private RectTransform _canvasRectTransform;

    private Button _selectButton;

    private Animator _animator;
    public static string BUBBLE_PREFAB_PATH = "Prefabs/Home/UIWorldMainBubblePlay";

    void Awake()
    {
        var t = this.transform;
        _selectButton = t.GetComponent<Button>();
        _selectButton.onClick.AddListener(OnBtnSelectNode);

        _buttonRectTransform = transform as RectTransform;
        _canvasRectTransform = UIRoot.Instance.mRootCanvas.transform as RectTransform;

        _animator = t.Find("Root").GetComponent<Animator>();

        Init();
    }

    public override void BindNode(DecoNode node)
    {
        base.BindNode(node);
        gameObject.SetActive(false);
        Bind();
    }

    public override DecoNode GetNode()
    {
        return _node;
    }

    private void OnBtnSelectNode()
    {
        if( SceneFsm.mInstance.GetCurrSceneType()!=StatusType.Home && SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome) 
            return;
       
        NodeBubbleManager.Instance.UnLoadBubble(NodeBubbleManager.BubbleType.Play, _node);
        
        PlayerManager.Instance.PlayAnimation(_node);
    }
}
