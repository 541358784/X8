using System;
using ActivityLocal.DecoBuildReward;
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

public class GetBubbleController : FollowTargetBase
{
    private RectTransform _buttonRectTransform;
    private RectTransform _canvasRectTransform;

    private Button _selectButton;

    private Animator _animator;
    public static string BUBBLE_PREFAB_PATH = "Prefabs/Home/UIWorldMainBubbleGet";

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
        var reward = DecoBuildRewardManager.Instance.GetReward(_node.Id.ToString());
        if(reward == null)
            return;

        var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TotalRechargeRewardGet);
        CommonRewardManager.Instance.PopCommonReward(reward, CurrencyGroupManager.Instance.GetCurrencyUseController(),
            true, reasonArgs, animEndCall:
            () =>
            {
                NodeBubbleManager.Instance.UnLoadBubble(_node);
                NodeBubbleManager.Instance.OnLoadBubble(_node);
            });
    }
}
