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

public class UIPopupGoldRewardItem : MonoBehaviour
{
    private Transform UIBgFinish;
    private Transform UIBgNormal;
    private Transform UIBgLock;
    private Image Icon;
    private LocalizeTextMeshProUGUI normalText;

    private LocalizeTextMeshProUGUI lockText;

    //private TableGoldenTileConfig rewardConfig;
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


        normalText = transform.Find("UIBgNormal/Text").GetComponent<LocalizeTextMeshProUGUI>();
        lockText = transform.Find("UIBgLock/Text").GetComponent<LocalizeTextMeshProUGUI>();

        numNormalText = transform.Find("NumGroup/Get/Text").GetComponent<LocalizeTextMeshProUGUI>();
        numLockText = transform.Find("NumGroup/Normal/Text").GetComponent<LocalizeTextMeshProUGUI>();

        numLockObj = transform.Find("NumGroup/Normal").gameObject;
        numNoramlObj = transform.Find("NumGroup/Get").gameObject;
    }

    // public void SetData(TableGoldenTileConfig rewardConfig,int index)
    // {
    //     Sprite iconSprite = UserData.GetResourceIcon(rewardConfig.bonusList[0], UserData.ResourceSubType.Gold);
    //     
    //     Icon=UIBgFinish.Find("Icon").GetComponent<Image>();
    //     Icon.sprite = iconSprite;
    //     
    //     Icon =UIBgNormal.Find("Icon").GetComponent<Image>();
    //     Icon.sprite = iconSprite;
    //     
    //     Icon =UIBgNormal.Find("Icon").GetComponent<Image>();
    //     Icon.sprite = iconSprite;
    //     
    //     getIndex = index;
    //     this.rewardConfig = rewardConfig;
    //     
    //     normalText.SetText("X" + rewardConfig.bonusList[1].ToString());
    //     lockText.SetText("X" + rewardConfig.bonusList[1].ToString());
    //     
    //     numNormalText.SetText((getIndex+1).ToString());
    //     numLockText.SetText((getIndex+1).ToString());
    //     
    //     UpdateProgress();
    // }

    public void UpdateProgress()
    {
        UpdateRewardType();
        UpdateUI();
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

    private void UpdateRewardType()
    {
        // if(rewardConfig.id == curId)
        //     rewardType = RewardType.Normal;
        // else if (curId >= curId)
        //     rewardType = RewardType.Finish;
        // else
        //     rewardType = RewardType.Lock;
    }
}