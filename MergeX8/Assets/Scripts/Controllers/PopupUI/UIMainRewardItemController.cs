using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIMainRewardItemController : UIWindowController
{
    public LocalizeTextMeshProUGUI numText;
    public Button rewardBox;
    public Image rewardImage;
    public Animator animator;
    public Action onClickEnd;

    public override void PrivateAwake()
    {
        animator = transform.GetComponent<Animator>();

        numText = transform.Find("Root/Package/Text").GetComponent<LocalizeTextMeshProUGUI>();
        rewardBox = transform.Find("Root/ButtonStart").GetComponent<Button>();
        rewardBox.onClick.AddListener(() =>
        {
            if (onClickEnd == null)
                return;

            onClickEnd();
        });
        rewardImage = transform.Find("Root/Package").GetComponent<Image>();


        // rewardConfig = GoldCollectionModule.Instance.GetGoldenTileConfig();
        // UpdateUIInfo(rewardConfig);
    }

    public void RewardBoxShow(float moveTime)
    {
        Vector3 control = Vector3.zero;
        Vector3 targetPos = Vector3.zero;
        Vector3 srcPos = transform.position;
        Vector3 vDis = (targetPos - srcPos);

        control.x = srcPos.x + vDis.x / 2;
        control.y = srcPos.y + vDis.y / 2f;

        rewardImage.transform.DOPath(new Vector3[] {srcPos, control, targetPos}, moveTime, PathType.CatmullRom);
    }

    public void RewardBoxHide(Vector3 targetPos, float moveTime)
    {
        Vector3 control = Vector3.zero;
        Vector3 srcPos = transform.position;
        Vector3 vDis = (targetPos - srcPos);

        control.x = srcPos.x + vDis.x / 2;
        control.y = srcPos.y + vDis.y / 2f;

        rewardImage.transform.DOPath(new Vector3[] {srcPos, control, targetPos}, moveTime, PathType.CatmullRom);
    }

    public void UpdateUI()
    {
        //UpdateUIInfo(GoldCollectionModule.Instance.GetGoldenTileConfig());
    }

    // private void UpdateUIInfo(TableGoldenTileConfig rewardConfig)
    // {
    //     if(rewardConfig == null)
    //         return;
    //     
    //     Sprite iconSprite = UserData.GetResourceIcon(rewardConfig.bonusList[0], UserData.ResourceSubType.Gold);
    //     rewardImage.sprite = iconSprite;
    //     numText.SetText(rewardConfig.bonusList[1].ToString());
    // }
}