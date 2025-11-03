using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using Gameplay;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupRVRewardItem : MonoBehaviour
{
    private Transform UIBgFinish;
    private Transform UIBgNormal;
    private Transform UIBgLock;
    private Image Icon;
    private Image IconMagic;
    private LocalizeTextMeshProUGUI normalText;

    private LocalizeTextMeshProUGUI lockText;

    //private TableRvReward rvRewardConfig;
    private LocalizeTextMeshProUGUI numNormalText;
    private LocalizeTextMeshProUGUI numLockText;

    private Transform uiBoubble;
    private Image Icon1;
    private Image Icon2;
    private LocalizeTextMeshProUGUI text1;
    private LocalizeTextMeshProUGUI text2;
    private bool isShowTip = false;

    private int getIndex = 0;

    private GameObject numNoramlObj;
    private GameObject numLockObj;

    private enum RewardType
    {
        None,
        Normal,
        Finish,
        Lock,
    }

    private RewardType rewardType = RewardType.None;

    private void Awake()
    {
        UIBgFinish = transform.Find("UIBgFinish");
        UIBgNormal = transform.Find("UIBgNormal");
        UIBgLock = transform.Find("UIBgLock");
        Icon = transform.Find("Icon").GetComponent<Image>();
        IconMagic = transform.Find("IconMagicWand").GetComponent<Image>();

        normalText = transform.Find("UIBgNormal/Text").GetComponent<LocalizeTextMeshProUGUI>();
        lockText = transform.Find("UIBgLock/Text").GetComponent<LocalizeTextMeshProUGUI>();

        numNormalText = transform.Find("NumGroup/Normal/Text").GetComponent<LocalizeTextMeshProUGUI>();
        numLockText = transform.Find("NumGroup/Lock/Text").GetComponent<LocalizeTextMeshProUGUI>();

        numNoramlObj = transform.Find("NumGroup/Normal").gameObject;
        numLockObj = transform.Find("NumGroup/Lock").gameObject;
    }
    //
    // public void SetData(TableRvReward rvRewardConfig,int index, int progress)
    // {
    //     getIndex = index;
    //     this.rvRewardConfig = rvRewardConfig;
    //
    //     normalText.SetText("X" + rvRewardConfig.rewardNum[0].ToString());
    //     lockText.SetText("X" + rvRewardConfig.rewardNum[0].ToString());
    //     
    //     if (rvRewardConfig.rewardType == 1)
    //     {
    //         IconMagic.gameObject.SetActive(false);
    //         Icon.gameObject.SetActive(true);
    //         Icon.sprite =  UserData.GetResourceIcon(rvRewardConfig.rewardId[0], UserData.ResourceSubType.Big);
    //     }
    //     else
    //     {
    //         normalText.SetText("X1");
    //         lockText.SetText("X1");
    //         
    //         uiBoubble=transform.Find("UIBoubble");
    //         uiBoubble.gameObject.SetActive(false);
    //         
    //         Icon1=transform.Find("UIBoubble/Icon").GetComponent<Image>();
    //         Icon2=transform.Find("UIBoubble/Icon2").GetComponent<Image>();
    //         text1=transform.Find("UIBoubble/Text1").GetComponent<LocalizeTextMeshProUGUI>();
    //         text2=transform.Find("UIBoubble/Text2").GetComponent<LocalizeTextMeshProUGUI>();
    //
    //         bool isGlodBox = (index+1) % 10 == 0;
    //         GameObject item2 =transform.Find("Icon2").gameObject;
    //         item2.SetActive(!isGlodBox);
    //         Icon.gameObject.SetActive(isGlodBox);
    //         
    //         Icon1.sprite =  UserData.GetResourceIcon(rvRewardConfig.rewardId[0], UserData.ResourceSubType.Big);
    //         Icon2.sprite =  UserData.GetResourceIcon(rvRewardConfig.rewardId[1], UserData.ResourceSubType.Big);
    //         text1.SetText("X" + rvRewardConfig.rewardNum[0].ToString());
    //         text2.SetText("X" + rvRewardConfig.rewardNum[1].ToString());
    //         Icon.transform.GetComponent<Button>().onClick.AddListener(() =>
    //         {
    //             ShowUIBoubble();
    //         });
    //         
    //         item2.transform.GetComponent<Button>().onClick.AddListener(() =>
    //         {
    //             ShowUIBoubble();
    //         });
    //     }
    //
    //     numNormalText.SetText((getIndex+1).ToString());
    //     numLockText.SetText((getIndex+1).ToString());
    //     
    //     UpdateProgress(progress);
    // }

    public void UpdateProgress(int progress)
    {
        UpdateRewardType(progress);
        UpdateUI();
    }

    private void ShowUIBoubble()
    {
        if (!isShowTip)
        {
            isShowTip = true;
            uiBoubble.gameObject.SetActive(true);

            StartCoroutine(CommonUtils.DelayWork(2, () =>
            {
                isShowTip = false;
                uiBoubble.gameObject.SetActive(false);
            }));
        }
    }

    private void UpdateUI()
    {
        UIBgFinish.gameObject.SetActive(false);
        UIBgNormal.gameObject.SetActive(false);
        UIBgLock.gameObject.SetActive(false);
        numNoramlObj.gameObject.SetActive(false);
        numLockObj.gameObject.SetActive(false);


        switch (rewardType)
        {
            case RewardType.Normal:
            {
                numNoramlObj.gameObject.SetActive(true);
                UIBgNormal.gameObject.SetActive(true);
                break;
            }
            case RewardType.Lock:
            {
                numLockObj.gameObject.SetActive(true);
                UIBgLock.gameObject.SetActive(true);
                break;
            }
            case RewardType.Finish:
            {
                numNoramlObj.gameObject.SetActive(true);
                UIBgNormal.gameObject.SetActive(true);
                UIBgFinish.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void UpdateRewardType(int progress)
    {
        if (progress == getIndex)
            rewardType = RewardType.Normal;
        else if (progress >= getIndex)
            rewardType = RewardType.Finish;
        else
            rewardType = RewardType.Lock;
    }

    public void RestUI()
    {
        if (!isShowTip)
            return;

        isShowTip = false;
        StopAllCoroutines();
        uiBoubble.gameObject.SetActive(false);
    }
}