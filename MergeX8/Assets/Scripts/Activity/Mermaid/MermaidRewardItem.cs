
using System;
using Deco.Item;
using Deco.World;
using Decoration;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public class MermaidRewardItem: MonoBehaviour
{
    private Transform _bgInsufficient;
    private Transform _bgNormal;
    private LocalizeTextMeshProUGUI _text;
    private Image _icon;
    private LocalizeTextMeshProUGUI _consumeText;
    private Transform _complete;
    private ExchangeReward _config;
    private UIPopupMermaidMainController _controller;
    private void Awake()
    {
        _bgInsufficient = transform.Find("BGInsufficient");
        _bgNormal = transform.Find("BGNormal");
        _text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _icon = transform.Find("Icon").GetComponent<Image>();
        _consumeText = transform.Find("Consume/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _complete = transform.Find("Complete");
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
    }

    private void OnBtnClick()
    {
        if (MermaidModel.Instance.IsClaimed(_config.RewardId))
            return;
        _controller.AnimCloseWindow((() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.Buy, DecoManager.Instance.FindNodeByItem(_config.RewardId).Id);
            }
            else
            {
                DecoManager.Instance.SelectNode(DecoManager.Instance.FindNodeByItem(_config.RewardId));
            }
        }));
    }

    public void Init(ExchangeReward config,UIPopupMermaidMainController controller)
    {
        _config = config;
        _controller = controller;
        _consumeText.SetText((config.ExchangeScore*MermaidModel.Instance.GetExtendMultiple()).ToString());
        UpdateState();
    }

    public void UpdateState()
    {
        bool isClaimed = MermaidModel.Instance.IsClaimed(_config.RewardId);
        _bgNormal.gameObject.SetActive(MermaidModel.Instance.GetScore()>=_config.ExchangeScore*MermaidModel.Instance.GetExtendMultiple()||isClaimed);
        _complete.gameObject.SetActive(isClaimed);
        _consumeText.transform.parent.gameObject.SetActive(!isClaimed);
        
        
        DecoItem item = null;
        if (DecoWorld.ItemLib.ContainsKey(_config.RewardId))
            item = DecoWorld.ItemLib[_config.RewardId];

        if (item != null)
        {
            _text.SetTerm(item.Config.name);
            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant("MermaidAtlas", item.Config.buildingIcon);
        } 
    }
}
