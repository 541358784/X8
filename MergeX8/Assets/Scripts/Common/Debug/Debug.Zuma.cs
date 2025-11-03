using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string Zuma = "祖玛";
    [Category(Zuma)]
    [DisplayName("重制")]
    public void ResetZuma()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().Zuma.Clear();
        foreach (var pair in StorageManager.Instance.GetStorage<StorageHome>().ZumaLeaderBoard)
        {
            CommonLeaderBoardUtils.StorageWeekInitStateDictionary.Remove(pair.Value.ActivityId);
        }
        StorageManager.Instance.GetStorage<StorageHome>().ZumaLeaderBoard.Clear();
        var guideIdList = new List<int>() {4331,4332,4333,4334,4335,4336};
        CleanGuideList(guideIdList);
        ZumaModel.Instance.CreateStorage();
    }

    [Category(Zuma)]
    [DisplayName("球数")]
    public int ZumaBallCount
    {
        get
        {
            return ZumaModel.Instance.GetBallCount();
        }
        set
        {
            ZumaModel.Instance.AddBall(value - ZumaModel.Instance.GetBallCount(),"Debug");
        }
    }
    [Category(Zuma)]
    [DisplayName("炸弹数")]
    public int ZumaBombCount
    {
        get
        {
            return ZumaModel.Instance.GetBombCount();
        }
        set
        {
            ZumaModel.Instance.AddBomb(value - ZumaModel.Instance.GetBombCount(),"Debug");
        }
    }
    [Category(Zuma)]
    [DisplayName("激光数")]
    public int ZumaLightCount
    {
        get
        {
            return ZumaModel.Instance.GetLineCount();
        }
        set
        {
            ZumaModel.Instance.AddLine(value - ZumaModel.Instance.GetLineCount(),"Debug");
        }
    }
    [Category(Zuma)]
    [DisplayName("关内分数")]
    public int ZumaScoreCount
    {
        get
        {
            var storage = ZumaModel.Instance.CurStorageZumaWeek;
            if (storage == null)
                return 0;
            return storage.LevelScore;
        }
        set
        {
            var storage = ZumaModel.Instance.CurStorageZumaWeek;
            if (storage != null)
            {
                storage.AddScore(value - storage.LevelScore,"Debug");
            }
        }
    }

    [Category(Zuma)]
    [DisplayName("关卡")]
    public int ZumaChangeLevel
    {
        get
        {
            var storage = ZumaModel.Instance.CurStorageZumaWeek;
            if (storage == null)
                return 0;
            return storage.LevelId;
        }
        set
        {
            var storage = ZumaModel.Instance.CurStorageZumaWeek;
            if (storage == null)
                return;
            var level = ZumaModel.Instance.GetLevel(value);
            if (level == null)
                return;
            storage.CompleteTimes = value - 1;
            storage.StartLevel(level);
        }
    }
}