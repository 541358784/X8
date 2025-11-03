using System.Collections.Generic;
using System.ComponentModel;
using DragonU3DSDK.Storage;


public partial class SROptions
{
    private const string FishCulture = "养鱼";
    [Category(FishCulture)]
    [DisplayName("清档")]
    public void FishCultureClearStorage()
    {
        HideDebugPanel();
        var storage = StorageManager.Instance.GetStorage<StorageHome>().FishCulture;
        StorageManager.Instance.GetStorage<StorageHome>().FishCulture.Clear();
        foreach (var pair in StorageManager.Instance.GetStorage<StorageHome>().FishCultureLeaderBoard)
        {
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(pair.Value.ActivityId);
        }
        StorageManager.Instance.GetStorage<StorageHome>().FishCultureLeaderBoard.Clear();
        var guideIdList = new List<int>() {4445,4446};
        CleanGuideList(guideIdList);
    }

    [Category(FishCulture)]
    [DisplayName("积分")]
    public int FishCultureScore
    {
        get
        {
            return FishCultureModel.Instance.GetScore();
        }
        set
        {
            FishCultureModel.Instance.AddScore(value - FishCultureModel.Instance.GetScore(),"Debug");
        }
    }
    [Category(FishCulture)]
    [DisplayName("结束")]
    public bool FishCultureIsEnd
    {
        get
        {
            return FishCultureModel.Instance.CurStorageFishCultureWeek.IsEnd;
        }
        set
        {
            FishCultureModel.Instance.CurStorageFishCultureWeek.IsEnd = value;
        }
    }
}
