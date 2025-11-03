using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;


public partial class SROptions
{
    [Category(GarageCleanup)]
    [DisplayName("重置仓库整理")]
    public void RestGarageClean()
    {
       StorageManager.Instance.GetStorage<StorageHome>().GarageCleanup.Clear();
       GarageCleanupModel.Instance._storageGarageCleanup = null;
    }

    private bool isHaveAll;
    [Category(GarageCleanup)] 
    [DisplayName("忽略物品")]
    public bool IsHaveAll
    {
        get
        {
            return isHaveAll;
        }
        set
        {
            isHaveAll=value;
        }
    }
    [Sort(-10)]
    [Category(GarageCleanup)]
    [DisplayName("修改鱼塘锩")]
    public int Fishpond_token
    {
        get { return UserData.Instance.GetRes(UserData.ResourceId.Fishpond_token); }
        set
        {
            GameBIManager.ItemChangeReasonArgs reason = new GameBIManager.ItemChangeReasonArgs()
                {reason = BiEventCooking.Types.ItemChangeReason.Debug};
            UserData.Instance.SetRes(UserData.ResourceId.Fishpond_token, value, reason);
        }
    }
    
}