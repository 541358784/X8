
using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DailyBundleController : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _numbertext;
    private LocalizeTextMeshProUGUI _daytext;

    private Button _buttonReceive;
    private GameObject _finish;

    private Transform _rewardItem;

    private List<DailyBonusItem> _rewardItems;
    
    private DailyBonusChest _bonusChest;
    private Button _bubbleBtn;
    private GameObject _bubbleGroup;
    private Coroutine coroutineAction = null;
    private Transform _bubbleItem;
    private List<GameObject> _bubbleItemList = new List<GameObject>();

    private Transform _vipFitmentReward;
    
    private void Awake()
    {
        _numbertext = transform.Find("RewardIcon/Num").GetComponent<LocalizeTextMeshProUGUI>();
        _daytext = transform.Find("Day/DayText").GetComponent<LocalizeTextMeshProUGUI>();

        _vipFitmentReward = transform.Find("Fitment");
        
        _buttonReceive = transform.Find("ButtonGroup/ReceiveButton").GetComponent<Button>();
        _buttonReceive.onClick.AddListener(OnReceive);
        _finish = transform.Find("ButtonGroup/Finish").gameObject;

        _rewardItem=transform.Find("RewardGroup/Reward");
        _rewardItem.gameObject.SetActive(false);
        _bubbleBtn = transform.Find("RewardIcon").GetComponent<Button>();
        _bubbleBtn.onClick.AddListener(OnBtnBox);
        _bubbleGroup=transform.Find("RewardIcon/BubbleGroup").gameObject;
        _bubbleGroup.SetActive(false);
        _bubbleItem=transform.Find("RewardIcon/BubbleGroup/Icon1");
        _bubbleItem.gameObject.SetActive(false);

    }

    private void OnBtnBox()
    {
        OpenBubble(true);
    }


    public void Init()
    {
        _numbertext.SetText("");
        _rewardItems = new List<DailyBonusItem>();
        for (int i = 0; i < 7; i++)
        {
            var item= Instantiate(_rewardItem, _rewardItem.parent);
            item.gameObject.SetActive(true);
            var rewardItem = item.GetOrCreateComponent<DailyBonusItem>();
            rewardItem.SetData(i + 1);
            _rewardItems.Add(rewardItem);
        }
        RefreshUI();
        RefreshBubbleItems();
        
        _vipFitmentReward.gameObject.SetActive(VipStoreModel.Instance.VipLevel() >= 2);
    }
    
    private void RefreshBubbleItems()
    {
        foreach (var item in _bubbleItemList)
        {
            GameObject.Destroy(item);
        }
        _bubbleItemList.Clear();
        
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        _bonusChest  =DailyBonusModel.Instance.GetDailyBoxConfig(storageHome.DailyBonus.TotalClaimDay);

        for (int i = 0; i < _bonusChest.ItemId.Count; i++)
        {
            GameObject cloneObj = GameObject.Instantiate(_bubbleItem.gameObject, _bubbleGroup.transform);
            cloneObj.gameObject.SetActive(true);
            cloneObj.GetComponent<Image>().sprite = UserData.GetResourceIcon(_bonusChest.ItemId[i]);
            cloneObj.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(_bonusChest.ItemNum[i].ToString());
            _bubbleItemList.Add(cloneObj);
        }
    }
    public void OpenBubble(bool isShow)
    {
        if (coroutineAction != null)
        {
            StopCoroutine(coroutineAction);
            coroutineAction = null;
        }
        
        _bubbleGroup.gameObject.SetActive(isShow);
        
        if(!isShow)
            return;

        coroutineAction = StartCoroutine(WaitClose());
    }

    private IEnumerator WaitClose()
    {
        yield return new WaitForSeconds(3f);
        _bubbleGroup.gameObject.SetActive(false);
        coroutineAction = null;
    }

    private int ShowDay;
    private DailyBonusModel.BonusState State;
    void RefreshUI()
    {
        ShowDay = 1;
        State = DailyBonusModel.BonusState.NotCanClaim;
        _buttonReceive.gameObject.SetActive(false);
        _finish.gameObject.SetActive(true);
        for (var i = 1; i <= DailyBonusModel.DailyDays; ++i)
        {
            var state = DailyBonusModel.Instance.GetDailyBonusState(i);
            if (state == DailyBonusModel.BonusState.CanClaim)
            {
                _buttonReceive.gameObject.SetActive(true);
                _finish.gameObject.SetActive(false);
                State = state;
                ShowDay = i;
            }
            else if (state == DailyBonusModel.BonusState.NextDayCanClaim)
            {
                State = state;
                ShowDay = i;
            }
        }
        for (int i = 0; i < _rewardItems.Count; ++i) 
        {
            DailyBonusItem item = _rewardItems[i];
            item.SetState (State, ShowDay);
            item.SetData(i + 1);
        }
       
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        _daytext.SetText(storageHome.DailyBonus.TotalClaimDay.ToString());
        var config =DailyBonusModel.Instance.GetDailyBoxConfig(storageHome.DailyBonus.TotalClaimDay);
        var showDay = storageHome.DailyBonus.TotalClaimDay -storageHome.DailyBonus.TotalClaimDay%30 + config.Day;
        _numbertext.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("UI_dailybonus_day_pic"),showDay));
    }

    private void OnReceive()
    {
        DailyBonusModel.Instance.ClaimBonus(ShowDay, false, 1, (bool isCanOpenBox) =>
        {
            if (isCanOpenBox)
            {
                StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
                DailyBonusModel.Instance.ClaimBonusChest(storageHome.DailyBonus.TotalClaimDay, (bool isClaim) =>
                {
                    RefreshBubbleItems();
                });
            }

        });
        RefreshUI();
    }
}
