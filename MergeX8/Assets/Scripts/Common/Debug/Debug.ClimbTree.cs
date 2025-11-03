using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(ClimbTree)]
    [DisplayName("重置猴子爬树")]
    public void ResetClimbTree()
    {
        HideDebugPanel();
        ClimbTreeModel.StorageClimbTree.Clear();
        var guideIdList = new List<int>() {520, 521, 522, 523, 524,525,526,527,734,735,736};
        CleanGuideList(guideIdList);
        ClimbTreeLeaderBoardUtils.StorageWeekInitStateDictionary.Clear();
        ClimbTreeModel.Instance.CreateStorage();
    }
    [Category(ClimbTree)]
    [DisplayName("显示猴子爬树")]
    public void ShowClimbTree()
    {
        HideDebugPanel();
        
        UIManager.Instance.OpenUI(UINameConst.UIClimbTreeMain);
    }


    [Category(ClimbTree)]
    [DisplayName("猴子爬树改分")]
    public int AddClimbTreeValue
    {
        get
        {
            if (!ClimbTreeModel.Instance.IsPrivateOpened())
                return 0;
            return ClimbTreeModel.Instance.TotalScore;
        }
        set
        {
            if (!ClimbTreeModel.Instance.IsPrivateOpened())
                return;
            var addValue = value - ClimbTreeModel.Instance.TotalScore;
            var addValueTask = new List<TaskCompletionSource<bool>>();
            ClimbTreeModel.Instance.AddScore(addValue,addValueTask);
            while (addValueTask.Count > 0)
            {
                addValueTask[0].SetResult(true);
            }
        }
    }

    // [Category(ClimbTree)]
    // [DisplayName("ClimbTree")]
    // public int ClimbTreeCurIndex
    // {
    //     get
    //     {
    //         return ClimbTreeModel.Instance.CurScore;
    //     }
    // }
    [Category(ClimbTree)]
    [DisplayName("设置结束时间")]
    public int SetClimbTreeLeaderBoardCurWorldEndTime
    {
        get
        {
            if (ClimbTreeModel.Instance.CurStorageClimbTreeWeek == null)
                return 0;
            return (int)ClimbTreeModel.Instance.CurStorageClimbTreeWeek.GetLeftTime()/1000;
        }
        set
        {
            ClimbTreeModel.Instance.CurStorageClimbTreeWeek?.SetLeftTime((long)value*1000);
        }
    }
   
}