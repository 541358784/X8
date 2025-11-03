using System.Collections.Generic;
using System.ComponentModel;
using Decoration;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string SlotMachine = "老虎机";
    [Category(SlotMachine)]
    [DisplayName("清存档")]
    public void CleanSlotMachineStorage()
    {
        SlotMachineModel.Instance.StorageDic.Clear();
        var guideIdList = new List<int>() {761,762,763,764,765};
        CleanGuideList(guideIdList);
    }
    [Category(SlotMachine)]
    [DisplayName("设置代币")]
    public int SetSlotMachineSpinCount
    {
        get
        {
            return SlotMachineModel.Instance.GetScore();
        }
        set
        {
            if (SlotMachineModel.Instance.IsOpened())
            {
                SlotMachineModel.Instance.CurStorage.SpinCount = value;
                EventDispatcher.Instance.SendEventImmediately(new EventSlotMachineScoreChange(0));
            }
        }
    }
}