using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIEffects;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

class DailyBonusItem : MonoBehaviour
{
    private int dayIndex;

    private Transform _finishGroup;
    private Transform _canReceive;
    private Image boostIcon;
    private LocalizeTextMeshProUGUI boostNum;
    
    public void Awake()
    {
        _finishGroup = transform.Find("Finish");
        _canReceive = transform.Find("Claimed");
        boostIcon = transform.Find("Icon").GetComponent<Image>();
        boostNum = transform.Find("NumText").GetComponent<LocalizeTextMeshProUGUI>();

    }

    public void SetData(int day)
    {
        
        if (day < 0 || day > DailyBonusModel.DailyDays)
        {
            return;
        }
        dayIndex = day;
        var rewards = AdConfigHandle.Instance.GetDailyBonus(day);
        boostIcon.sprite=UserData.GetResourceIcon(rewards.ItemId[0],UserData.ResourceSubType.Big);
        boostNum.SetText(rewards.ItemNum[0].ToString());
     
    }
    
    public void SetState(DailyBonusModel.BonusState State, int showDay)
    {
        bool canRec = showDay == dayIndex && State == DailyBonusModel.BonusState.CanClaim;
        
        _canReceive.gameObject.SetActive(canRec);
        _finishGroup.gameObject.SetActive(showDay > dayIndex);

    }

}