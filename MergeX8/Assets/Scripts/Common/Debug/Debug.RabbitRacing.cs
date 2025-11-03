using System;
using System.Collections.Generic;
using System.ComponentModel;
using Activity.RabbitRacing.Dynamic;

public partial class SROptions
{
    private const string RabbitRacing = "0兔子竞速";
    [Category(RabbitRacing)]
    [DisplayName("重置竞速")]
    public void ResetRabbitRacing()
    {
        HideDebugPanel();
        RabbitRacingModel.Instance.DebugReset();
        RabbitRacingModel.Instance.Storage.Clear();
        RabbitRacingModel.Instance.CurRacing = null;
        ResetRabbitRacingGuide();
    }

    [Category(RabbitRacing)]
    [DisplayName("重置竞速引导")]
    public void ResetRabbitRacingGuide()
    {
        var guideIdList = new List<int>() {2301,2302,2303,2304,2305};
        CleanGuideList(guideIdList);
    }

    private int _rabbitRacingScore;

    [Category(RabbitRacing)]
    [DisplayName("需要增加的分数")]
    public int RabbitRacingScore
    {
        get => _rabbitRacingScore;
        set => _rabbitRacingScore = value;
    }

    [Category(RabbitRacing)]
    [DisplayName("增加分数")]
    public void SetRabbitRacingScore()
    {
        RabbitRacingModel.Instance.AddScoreDebug(RabbitRacingScore);
    }

    [Category(RabbitRacing)]
    [DisplayName("需要设置的机器人索引")]
    public int RabbitRobotScoreIndex
    {
        get => _rabbitRobotIndex;
        set => _rabbitRobotIndex = value;
    }

    private int _rabbitRobotIndex;

    [Category(RabbitRacing)]
    [DisplayName("设置机器人分数")]
    public int RabbitRobotScore
    {
        get => _rabbitRobotScore;
        set => _rabbitRobotScore = value;
    }

    private int _rabbitRobotScore;

    [Category(RabbitRacing)]
    [DisplayName("设置机器人")]
    public void SetRabbitRacingRobotScore()
    {
        RabbitRacingModel.Instance.SetRobotScore(RabbitRobotScoreIndex, RabbitRobotScore);
    }
}