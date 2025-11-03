using System.Collections.Generic;
using System.ComponentModel;
using Activity.JumpGrid;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string JumpGrid = "1跳格子";
    [Category(JumpGrid)]
    [DisplayName("重置跳格子")]
    public void RestJumpGrid()
    {
       JumpGridModel.Instance.Clear();
       
       var guideIdList = new List<int>() {4553, 4554, 4555, 4556};
       CleanGuideList(guideIdList);
    }

    private int _jumpStore;
    [Category(JumpGrid)] 
    [DisplayName("金币数量")]
    public int JumpStore
    {
        get
        {
            return _jumpStore;
        }
        set
        {
            _jumpStore=value;
        }
    }
    
    [Category(JumpGrid)]
    [DisplayName("增加金币")]
    public void AddCoinJump()
    {
        JumpGridModel.Instance.AddScore(_jumpStore);
    }
    
}