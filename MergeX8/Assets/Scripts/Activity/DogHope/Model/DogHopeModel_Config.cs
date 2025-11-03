using System;
using System.Collections.Generic;
using DragonPlus.Config.DogHope;

public partial class DogHopeModel
{
    public DogHopeGlobalConfig GlobalConfig => DogHopeConfigManager.Instance.GetConfig<DogHopeGlobalConfig>()[0];
    public List<DogHopeLeaderBoardRewardConfig> LeaderBoardRewardConfig=> DogHopeConfigManager.Instance.GetConfig<DogHopeLeaderBoardRewardConfig>();
}