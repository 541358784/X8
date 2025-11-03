using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Activity.TreasureMap;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    private const string PhotoAlbum = "相册";
    [Category(PhotoAlbum)]
    [DisplayName("清档")]
    public void PhotoAlbumClearStorage()
    {
        HideDebugPanel();
        var storage = StorageManager.Instance.GetStorage<StorageHome>().PhotoAlbum;
   
        StorageManager.Instance.GetStorage<StorageHome>().PhotoAlbum.Clear();
        var guideIdList = new List<int>() {4448,4449,4501};
        CleanGuideList(guideIdList);
        var storyIdList = new List<int>()
        {
            9000101,
            9010101,
            9020101,
            9030101,
            9090101,
        };
        foreach (var storyId in storyIdList)
        {
            StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog.Remove(storyId);
        }
    }

    [Category(PhotoAlbum)]
    [DisplayName("积分")]
    public int PhotoAlbumScore
    {
        get
        {
            return PhotoAlbumModel.Instance.GetScore();
        }
        set
        {
            PhotoAlbumModel.Instance.AddScore(value - PhotoAlbumModel.Instance.GetScore(),"Debug");
        }
    }
    [Category(PhotoAlbum)]
    [DisplayName("结束")]
    public bool PhotoAlbumIsEnd
    {
        get
        {
            return PhotoAlbumModel.Instance.Storage.IsEnd;
        }
        set
        {
            PhotoAlbumModel.Instance.Storage.IsEnd = value;
        }
    }
}
