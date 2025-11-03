using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;


public partial class SROptions
{
    [Category(CoinCompetition)]
    [DisplayName("重置金币挑战")]
    public void RestCoinCompetition()
    {
       CoinCompetitionModel.Instance.Clear();
       
       var guideIdList = new List<int>() {553, 554, 555, 556};
       var guideFinish = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
       var cacheGuideFinished = GuideSubSystem.Instance.CacheGuideFinished;
       for (var i = 0; i < guideIdList.Count; i++)
       {
           var guideId = guideIdList[i];
           if (guideFinish.ContainsKey(guideId))
           {
               guideFinish.Remove(guideId);
           }
           if (cacheGuideFinished.ContainsKey(guideId))
           {
               cacheGuideFinished.Remove(guideId);
           }
       }
    }

    private int _store;
    [Category(CoinCompetition)] 
    [DisplayName("分数")]
    public int Store
    {
        get
        {
            return _store;
        }
        set
        {
            _store=value;
        }
    }
    
    [Category(CoinCompetition)]
    [DisplayName("金币挑战ADD")]
    public void AddCoinCompetition()
    {
        CoinCompetitionModel.Instance.AddScore(Store);
    }

    
    
}