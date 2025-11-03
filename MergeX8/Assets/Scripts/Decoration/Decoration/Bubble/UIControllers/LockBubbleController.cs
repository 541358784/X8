using System;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using Deco.Node;
using Decoration;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;

public class LockBubbleController : FollowTargetBase
{
    private RectTransform _buttonRectTransform;
    private RectTransform _canvasRectTransform;

    private Button _selectButton;

    private Animator _animator;
    public static string BUBBLE_PREFAB_PATH = "Prefabs/Home/UIWorldMainBubbleLock";

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

    public void PlayUnLockAnim(Action action)
    {
        CoroutineManager.Instance.StartCoroutine(CommonUtils.PlayAnimation(_animator, "appear", "", action));
    }
    private void OnBtnSelectNode()
    {
        if( SceneFsm.mInstance.GetCurrSceneType()!=StatusType.Home && SceneFsm.mInstance.GetCurrSceneType() != StatusType.BackHome) 
            return;
        DecoManager.Instance.SelectNode(_node);
    }

    public void HideNodeBuyUI()
    {
        gameObject.SetActive(false);
    }
}
