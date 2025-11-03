using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusBoxItem : MonoBehaviour
{
    private int dayIndex;

    private Transform _closeGroup;
    private Transform _openGroup;
    LocalizeTextMeshProUGUI _dayText;

    private Button _butClick;
    private GameObject _rewardItem;
    private DailyBonusChest _bonusChest;
    private GameObject _rewardGroup;
    private Coroutine coroutineAction = null;
    
    public void Awake()
    {
        _closeGroup = transform.Find("Close");
        _openGroup = transform.Find("Open");
        _dayText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _butClick = transform.GetComponent<Button>();
    
        
        _rewardGroup = transform.Find("BubbleGroup").gameObject;
        _rewardItem = transform.Find("BubbleGroup/Icon1").gameObject;
        
        _rewardGroup.gameObject.SetActive(false);
        _rewardItem.gameObject.SetActive(false);
    }

    public void SetData(DailyBonusChest config)
    {
        _bonusChest = config;
        
        StorageHome storageHome = StorageManager.Instance.GetStorage<StorageHome>();
        var showDay = storageHome.DailyBonus.TotalClaimDay -storageHome.DailyBonus.TotalClaimDay%30 + config.Day;
        _dayText.SetText(showDay.ToString());
        bool isClaim = storageHome.DailyBonus.TotalClaimDay >= showDay;
        _closeGroup.gameObject.SetActive(!isClaim);
        _openGroup.gameObject.SetActive(isClaim);
        
        InitRewardIcons();
    }

    public void SetState(DailyBonusModel.BonusState State, int day)
    {
        bool canRec = day == dayIndex && State == DailyBonusModel.BonusState.CanClaim;

    }

    public void OpenRewardBox(bool isShow)
    {
        if (coroutineAction != null)
        {
            StopCoroutine(coroutineAction);
            coroutineAction = null;
        }
        
        _rewardGroup.gameObject.SetActive(isShow);
        
        if(!isShow)
            return;

        coroutineAction = StartCoroutine(WaitClose());
    }

    private IEnumerator WaitClose()
    {
        yield return new WaitForSeconds(3f);
        _rewardGroup.gameObject.SetActive(false);
        coroutineAction = null;
    }
    
    private void InitRewardIcons()
    {
        for (int i = 0; i < _bonusChest.ItemId.Count; i++)
        {
            GameObject cloneObj = GameObject.Instantiate(_rewardItem, _rewardGroup.transform);
            cloneObj.gameObject.SetActive(true);
            cloneObj.GetComponent<Image>().sprite = UserData.GetResourceIcon(_bonusChest.ItemId[i]);
            cloneObj.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(_bonusChest.ItemNum[i].ToString());
        }
    }
    private void OnDestroy()
    {
    }
}