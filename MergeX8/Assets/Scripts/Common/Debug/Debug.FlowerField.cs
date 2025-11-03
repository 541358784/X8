using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string FlowerField = "花田";
    [Category(FlowerField)]
    [DisplayName("重置")]
    public void ResetFlowerField()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().FlowerField.Clear();
        foreach (var pair in StorageManager.Instance.GetStorage<StorageHome>().FlowerFieldLeaderBoard)
        {
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(pair.Value.ActivityId);
        }
        StorageManager.Instance.GetStorage<StorageHome>().FlowerFieldLeaderBoard.Clear();
        var guideIdList = new List<int>()
        {
            4570,
            4571,
            4572,
            4573,
            4574,
            4575,
            4576,
        };
        CleanGuideList(guideIdList);
        if (FlowerFieldModel.Instance.IsInitFromServer())
        {
            FlowerFieldModel.Instance.InitStorage();
            FlowerFieldLeaderBoardModel.Instance.InitFromServerData();
            XUtility.WaitFrames(1).AddCallBack(() =>
            {
                FlowerFieldLeaderBoardModel.Instance.CreateStorage(FlowerFieldModel.Instance.Storage); 
            }).WrapErrors();
            MergeFlowerField.Instance?.InitScore();
        }
    }

    public int _addFlowerFieldValue;
    [Category(FlowerField)]
    [DisplayName("加分")]
    public int AddFlowerFieldScore
    {
        get
        {
            return _addFlowerFieldValue;
        }
        set
        {
            _addFlowerFieldValue = value;

        }
    }
    
    [Category(FlowerField)]
    [DisplayName("加分")]
    public void AddAddFlowerFieldScoreBtn()
    {
        FlowerFieldModel.Instance.AddScore(_addFlowerFieldValue,"Debug");
        MergeTaskTipsController.Instance.MergeFlowerField?.PerformAddValue( _addFlowerFieldValue);
    }
   
}