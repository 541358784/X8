using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;

public class Aux_NewNewIceBreakPack:Aux_ItemBase
{
    protected override void Awake()
    {
        base.Awake();
        TimeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("RedPoint");
        transform.Find("RedPoint/Label").gameObject.SetActive(false);
    }

    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        if (NewNewIceBreakPackModel.Instance.IsOpen())
        {
            UIPopupNewNewIceBreakPackController.Open();
            if (Storage.EndTime > (long)APIManager.Instance.GetServerTime())
            {
                UIPopupNewNewIceBreakPackController.Open();
            }
            else if(!Storage.ShowEndView)
            {
                Storage.ShowEndView = true;
                UIPopupNewNewIceBreakPackController.Open();
            }
        }
    }

    private StorageNewNewIceBreakPack Storage => NewNewIceBreakPackModel.Instance.Storage;
    private LocalizeTextMeshProUGUI TimeText;
    private Transform RedPoint;
    public override void UpdateUI()
    {
        base.UpdateUI();
        gameObject.SetActive(NewNewIceBreakPackModel.Instance.IsOpen());
        if (gameObject.activeSelf)
        {
            var curTime = (long)APIManager.Instance.GetServerTime();
            var leftTime = Storage.EndTime - curTime;
            if (leftTime < 0)
                leftTime = 0;
            var endTimeText = CommonUtils.FormatLongToTimeStr(leftTime);
            TimeText.SetText(endTimeText);
            var rewardList = NewNewIceBreakPackModel.Instance.NewNewIceBreakPackRewardList;
            var count = 0;
            foreach (var reward in rewardList)
            {
                if (NewNewIceBreakPackModel.Instance.CanCollectReward(reward))
                    count++;
            }
            RedPoint.gameObject.SetActive(count > 0);
        }
    }
}