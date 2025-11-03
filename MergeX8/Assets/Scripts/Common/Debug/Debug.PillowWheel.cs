using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    private const string PillowWheel = "0枕头";
    [Category(PillowWheel)]
    [DisplayName("重置")]
    public void ResetPillowWheel()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().PillowWheel.Clear();
        foreach (var pair in StorageManager.Instance.GetStorage<StorageHome>().PillowWheelLeaderBoard)
        {
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(pair.Value.ActivityId);
        }
        StorageManager.Instance.GetStorage<StorageHome>().PillowWheelLeaderBoard.Clear();
        var guideIdList = new List<int>()
        {
            4600,
            4601,
            4602,
            4603,
            4604,
            4605,
            4606,
            4607,
        };
        CleanGuideList(guideIdList);
        if (PillowWheelModel.Instance.IsInitFromServer())
        {
            PillowWheelModel.Instance.InitStorage();
            PillowWheelLeaderBoardModel.Instance.InitFromServerData();
            XUtility.WaitFrames(1).AddCallBack(() =>
            {
                PillowWheelLeaderBoardModel.Instance.CreateStorage(PillowWheelModel.Instance.Storage); 
            }).WrapErrors();
        }
    }
    
    [Category(PillowWheel)]
    [DisplayName("枕头")]
    public int AddPillowWheelScore
    {
        get
        {
            return PillowWheelModel.Instance.GetItem();
        }
        set
        {
            PillowWheelModel.Instance.AddItem(value - PillowWheelModel.Instance.GetItem(),new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }
    }
}