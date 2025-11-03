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
    private const string MultipleScore = "美人鱼翻倍";
    [Category(MultipleScore)]
    [DisplayName("清空美人鱼翻倍记录")]
    public void CleanMultipleScoreStorage()
    {
        MultipleScoreModel.Instance.Storage.Clear();
        MultipleScoreModel.Instance.CreateStorage();
    }
}