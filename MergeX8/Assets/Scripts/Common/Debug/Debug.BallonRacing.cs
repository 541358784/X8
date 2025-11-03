using System;
using System.Collections.Generic;
using System.ComponentModel;
using Activity.BalloonRacing;

public partial class SROptions
{
    private const string BalloonRacing = "热气球竞速";
    [Category(BalloonRacing)]
    [DisplayName("重置热气球竞速")]
    public void ResetBalloonRacing()
    {
        HideDebugPanel();
        BalloonRacingModel.Instance.DebugReset();
        BalloonRacingModel.Instance.Storage.Clear();
        BalloonRacingModel.Instance.CurRacing = null;
        ResetBalloonRacingGuide();
    }

    [Category(BalloonRacing)]
    [DisplayName("重置热气球竞速引导")]
    public void ResetBalloonRacingGuide()
    {
        var guideIdList = new List<int>() {2201,2202,2203,2204,2205};
        CleanGuideList(guideIdList);
    }

    private int balloonRacingScore;

    [Category(BalloonRacing)]
    [DisplayName("需要增加的分数")]
    public int BalloonRacingScore
    {
        get => balloonRacingScore;
        set => balloonRacingScore = value;
    }

    [Category(BalloonRacing)]
    [DisplayName("增加分数")]
    public void SetBalloonRacingScore()
    {
        BalloonRacingModel.Instance.AddScoreDebug(BalloonRacingScore);
    }

    [Category(BalloonRacing)]
    [DisplayName("需要设置的机器人索引")]
    public int BalloonRobotScoreIndex
    {
        get => _balloonRobotIndex;
        set => _balloonRobotIndex = value;
    }

    private int _balloonRobotIndex;

    [Category(BalloonRacing)]
    [DisplayName("设置机器人分数")]
    public int BalloonRobotScore
    {
        get => _balloonRobotScore;
        set => _balloonRobotScore = value;
    }

    private int _balloonRobotScore;

    [Category(BalloonRacing)]
    [DisplayName("设置机器人")]
    public void SetBalloonRacingRobotScore()
    {
        BalloonRacingModel.Instance.SetRobotScore(BalloonRobotScoreIndex, BalloonRobotScore);
    }
}