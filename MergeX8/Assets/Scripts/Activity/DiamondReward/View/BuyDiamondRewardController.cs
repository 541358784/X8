using System.Collections.Generic;
using Activity.DiamondRewardModel.Model;
using DragonPlus;
using DragonPlus.Config.DiamondReward;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class BuyDiamondRewardController : UIWindowController
{
    private Button _buyButton;
    private Button _unableButton;
    private Button _ignorePopUI;
    private GameObject _ignoreSelect;
    private int _index;

    private int _consumeValue = 0;
    public override void PrivateAwake()
    {
        var closeBtn = GetItem<Button>("Root/ButtonClose");
        closeBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });

        _buyButton = transform.Find("Root/Button").GetComponent<Button>();
        _buyButton.onClick.AddListener(OpenBox);
        
        _unableButton = transform.Find("Root/ButtonGrey").GetComponent<Button>();
        _unableButton.onClick.AddListener(() =>
        {
            AnimCloseWindow();
            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, 
                "diamondReward","", "",true,_consumeValue);
        });
        
        _ignorePopUI = transform.Find("Root/ButtonNotDisplay").GetComponent<Button>();
        _ignorePopUI.onClick.AddListener(() =>
        {
            DiamondRewardModel.Instance.IsIgnorePopUI = !DiamondRewardModel.Instance.IsIgnorePopUI;
            UpdateIgnoreStatus();
            Dictionary<string, string> ex = new Dictionary<string, string>();
            ex.Add("level",DiamondRewardModel.Instance.DiamondReward.Level.ToString());
            if(DiamondRewardModel.Instance.IsIgnorePopUI)
                GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventDiamondrewardCancelTips, DiamondRewardModel.Instance.DiamondReward.PoolId.ToString(),_index.ToString(), DiamondRewardModel.Instance.DiamondReward.PoolIndex.ToString(),extras:ex);
        });
        _ignoreSelect = transform.Find("Root/ButtonNotDisplay/Select").gameObject;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);

        _index = (int)objs[0];
        _consumeValue = DiamondRewardModel.Instance.GetConsume();
        _buyButton.transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>().SetText(_consumeValue.ToString());
        _unableButton.transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>().SetText(_consumeValue.ToString());
        UpdateIgnoreStatus();

        bool isEnough = UserData.Instance.CanAford(UserData.ResourceId.Diamond, _consumeValue);
        _buyButton.gameObject.SetActive(isEnough);
        _unableButton.gameObject.SetActive(!isEnough);
    }

    private void UpdateIgnoreStatus()
    {
        _ignoreSelect.gameObject.SetActive(DiamondRewardModel.Instance.IsIgnorePopUI);
    }
    private void OpenBox()
    {
        AnimCloseWindow(() =>
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BuyDiamondReward, _index);
        });
    }
}