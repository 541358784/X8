using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Facebook.Unity;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;


public class UIPopupCompensateController : UIWindowController
{
    private class RewardData
    {
        public LocalizeTextMeshProUGUI textNum;
        public Image icon;
        public GameObject parentObj;
        public ResData resData = null;

        public void Init(GameObject parent)
        {
            parentObj = parent;

            textNum = parent.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            icon = parent.transform.Find("Icon").GetComponent<Image>();

            parentObj.gameObject.SetActive(false);
        }

        public void UpdateUI(ResData resData)
        {
            this.resData = resData;
            parentObj.gameObject.SetActive(true);

            textNum.SetText(resData.count.ToString());
            icon.sprite = UserData.GetResourceIcon(resData.id, UserData.ResourceSubType.Small);
        }

        public ResData GetReward()
        {
            if (resData == null)
                return null;

            List<ResData> tempResData = new List<ResData>();
            tempResData.Add(resData);

            UserData.Instance.AddRes(tempResData, new GameBIManager.ItemChangeReasonArgs());

            return resData;
        }
    }

    private List<RewardData> rewardDatas = new List<RewardData>();

    public override void PrivateAwake()
    {
        Button claimBut = transform.Find("Root/ButtonClaim").GetComponent<Button>();
        claimBut.onClick.AddListener(() =>
        {
            List<ResData> tempResData = new List<ResData>();
            foreach (var data in rewardDatas)
            {
                ResData resData = data.GetReward();

                if (resData != null)
                    tempResData.Add(resData);
            }

            CloseWindowWithinUIMgr(true);
        });

        for (int i = 0; i < 2; i++)
        {
            GameObject objItem = GetItem("Root/RewardGroup/Reward" + (i + 1));

            RewardData rewardData = new RewardData();
            rewardData.Init(objItem);
            rewardDatas.Add(rewardData);
        }

        string reward = GlobalConfigManager.Instance.GetGlobal_Config_Number_Value("compensate_item");
        if (reward == null || reward == "")
            return;

        string[] arrayReward = reward.Split(';');
        if (arrayReward == null || arrayReward.Length == 0)
            return;

        for (int i = 0; i < arrayReward.Length; i++)
        {
            string[] rewardStr = arrayReward[i].Split(',');
            if (rewardStr == null || rewardStr.Length < 2)
                continue;

            if (i >= rewardDatas.Count)
                continue;

            rewardDatas[i].UpdateUI(new ResData(int.Parse(rewardStr[0]), int.Parse(rewardStr[1])));
        }
    }
}