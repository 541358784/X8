
using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class WeeklyCardItem : MonoBehaviour
{
    private Image _icon;
    private LocalizeTextMeshProUGUI _numbertext;

    private Button _buttonBuy;
    private Text _priceText;
    private Button _buttonReceive;
    private GameObject _finish;

    private Transform _rewardItem;
    private TableWeeklyCard _weeklyCardCfg;

    private List<WeeklyCardRewardItem> _rewardItems;
    private void Awake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _numbertext = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();

        _buttonReceive = transform.Find("ButtonGroup/ReceiveButton").GetComponent<Button>();
        _buttonReceive.onClick.AddListener(OnReceive);
        _buttonBuy = transform.Find("ButtonGroup/BuyButton").GetComponent<Button>();
        _buttonBuy.onClick.AddListener(OnBuy);
        _priceText = transform.Find("ButtonGroup/BuyButton/Text").GetComponent<Text>();
        _finish = transform.Find("ButtonGroup/Finish").gameObject;

        _rewardItem=transform.Find("RewardGroup/Reward");
        _rewardItem.gameObject.SetActive(false);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.WEEKLYCARD_PURCHASE,OnPurchase);
        EventDispatcher.Instance.AddEventListener(EventEnum.WEEKLYCARD_GETREWARD,OnGetReward);
    }



    public void Init(TableWeeklyCard weeklyCardCfg)
    {
        _weeklyCardCfg = weeklyCardCfg;
        if (weeklyCardCfg.id == 1)
        {
            _numbertext.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_weeklycard_desc1"),weeklyCardCfg.firstRewardNum[0]));
        }
        else
        {
            _numbertext.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_weeklycard_desc2"),weeklyCardCfg.firstRewardNum[0],weeklyCardCfg.firstRewardNum[1]));
        }
        _rewardItems = new List<WeeklyCardRewardItem>();
        for (int i = 0; i < weeklyCardCfg.RewardDays; i++)
        {
            var item= Instantiate(_rewardItem, _rewardItem.parent);
            item.gameObject.SetActive(true);
            var rewardItem = item.GetOrCreateComponent<WeeklyCardRewardItem>();
            _rewardItems.Add(rewardItem);
        }
        Refresh();

        _priceText.text = StoreModel.Instance.GetPrice(weeklyCardCfg.shopId);
        
        transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(weeklyCardCfg.shopId));
    }

    
    private void OnGetReward(BaseEvent obj)
    {
        Refresh();
    }

    private void OnPurchase(BaseEvent obj)
    {
        Refresh();
    }
    public void Refresh()
    {
        _buttonBuy.gameObject.SetActive(!WeeklyCardModel.Instance.IsPurchase(_weeklyCardCfg.id));
        _buttonReceive.gameObject.SetActive(WeeklyCardModel.Instance.IsCanClaim(_weeklyCardCfg));
        _finish.gameObject.SetActive(WeeklyCardModel.Instance.IsPurchase(_weeklyCardCfg.id)&&!WeeklyCardModel.Instance.IsCanClaim(_weeklyCardCfg));
        for (int i = 0; i < _weeklyCardCfg.RewardDays; i++)
        {
            if (WeeklyCardModel.Instance.IsOld(_weeklyCardCfg))
            {
                _rewardItems[i].SetStatus(_weeklyCardCfg.oldReward,_weeklyCardCfg.oldCount,
                    WeeklyCardModel.Instance.IsFinish(_weeklyCardCfg,i)   ,WeeklyCardModel.Instance.IsCanClaim(_weeklyCardCfg,i) );
            }
            else
            {
                _rewardItems[i].SetStatus(_weeklyCardCfg.everydayReward[i],_weeklyCardCfg.everydayRewardNum[i],
                    WeeklyCardModel.Instance.IsFinish(_weeklyCardCfg,i)   ,WeeklyCardModel.Instance.IsCanClaim(_weeklyCardCfg,i) );
            }
   
        }
    }
    private void OnBuy()
    {
        StoreModel.Instance.Purchase(_weeklyCardCfg.shopId);
    }

    private void OnReceive()
    {
        WeeklyCardModel.Instance.Claim(_weeklyCardCfg);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.WEEKLYCARD_PURCHASE,OnPurchase);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.WEEKLYCARD_GETREWARD,OnGetReward);
    }
}
