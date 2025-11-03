using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string Parrot = "鹦鹉";
    [Category(Parrot)]
    [DisplayName("重置")]
    public void ResetParrot()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().Parrot.Clear();
        foreach (var pair in StorageManager.Instance.GetStorage<StorageHome>().ParrotLeaderBoard)
        {
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(pair.Value.ActivityId);
        }
        StorageManager.Instance.GetStorage<StorageHome>().ParrotLeaderBoard.Clear();
        var guideIdList = new List<int>()
        {
            4560,
            4561,
            4562,
            4563,
            4564,
            4565,
            4566,
        };
        CleanGuideList(guideIdList);
        if (ParrotModel.Instance.IsInitFromServer())
        {
            ParrotModel.Instance.InitStorage();
            ParrotLeaderBoardModel.Instance.InitFromServerData();
            XUtility.WaitFrames(1).AddCallBack(() =>
            {
                ParrotLeaderBoardModel.Instance.CreateStorage(ParrotModel.Instance.Storage); 
            }).WrapErrors();
            MergeParrot.Instance?.InitScore();
        }
    }

    public int _addParrotValue;
    [Category(Parrot)]
    [DisplayName("加分")]
    public int AddParrotScore
    {
        get
        {
            return _addParrotValue;
        }
        set
        {
            _addParrotValue = value;

        }
    }
    
    [Category(Parrot)]
    [DisplayName("加分")]
    public void AddAddParrotScoreBtn()
    {
        ParrotModel.Instance.AddScore(_addParrotValue,"Debug");
        MergeTaskTipsController.Instance.MergeParrot?.PerformAddValue( _addParrotValue);
    }
   
}