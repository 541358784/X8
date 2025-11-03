using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(DogHope)]
    [DisplayName("重置小狗")]
    public void ResetDogHope()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().DogHopes.Clear();
        DogHopeLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        DogHopeModel.Instance.CreateStorage();
        var guideIdList = new List<int>() {731,732,733};
        CleanGuideList(guideIdList);
    }
    [Category(DogHope)]
    [DisplayName("显示小狗")]
    public void ShowDogHope()
    {
        HideDebugPanel();
        
        UIManager.Instance.OpenUI(UINameConst.UIDogMain);
    }


    [Category(DogHope)]
    [DisplayName("DogHope")]
    public int AddDogValue
    {
        get
        {
            return DogHopeModel.Instance.GetScore();
        }
        set
        {
            DogHopeModel.Instance.AddScore(value);
        }
    }
    
    [Category(DogHope)]
    [DisplayName("DogHope")]
    public int CurIndex
    {
        get
        {
            return DogHopeModel.Instance.GetCurIndex();
        }
    }
    
    [Category(DogHope)]
    [DisplayName("设置结束时间")]
    public int SetDogHopeLeaderBoardCurWorldEndTime
    {
        get
        {
            if (DogHopeModel.Instance.CurStorageDogHopeWeek == null)
                return 0;
            return (int)DogHopeModel.Instance.CurStorageDogHopeWeek.GetLeftTime()/1000;
        }
        set
        {
            DogHopeModel.Instance.CurStorageDogHopeWeek?.SetLeftTime((long)value*1000);
        }
    }
   
}