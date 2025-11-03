using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ConnectLine.Model;
using Ditch.Model;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Filthy.Game;
using Filthy.Model;
using Makeover;
using OnePath.Model;
using UnityEngine;


public partial class SROptions
{
    private const string MiniGame = "小游戏";
    
    [Category(MiniGame)]
    [DisplayName("强制开启小游戏")]
    public bool OpenMiniGame
    {
        get
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
                return Makeover.Utils.debugIsOpen;
            return false;
        }
        set
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
            {
                Makeover.Utils.debugIsOpen = value;
                
                if(UIHomeMainController.mainController != null)
                 UIHomeMainController.mainController.butGame.gameObject.SetActive(Makeover.Utils.debugIsOpen);
            }
        }
    }
    
    private string openScrewMergeKey = "ABTEST_MINIGAME_MERGE_FILTHY";
    [Category(MiniGame)]
    [DisplayName("强制开启SCREW")]
    public bool OpenScrewGame
    {
        get
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(openScrewMergeKey))
                return StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[openScrewMergeKey] ==  "1";

            return false;
        }
        set
        {
            if(!value)
                return;
            StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord["1.0.65"] = true;
            OpenMiniGame = true;
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(FilthyModel.AbKey))
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[FilthyModel.AbKey] =  "1";
            
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(openScrewMergeKey))
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(openScrewMergeKey, "1");
            
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[FilthyModel.AbKey] =  "1";
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[openScrewMergeKey] = "1";
        }
    }
    
    [Category(MiniGame)]
    [DisplayName("强制开启Merge")]
    public bool OpenScrewMerge
    {
        get
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(openScrewMergeKey))
                return StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[openScrewMergeKey] ==  "0";

            return false;
        }
        set
        {
            if(!value)
                return;
            
            OpenMiniGame = true;
            
            StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord["1.0.65"] = true;
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(FilthyModel.AbKey))
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[FilthyModel.AbKey] =  "1";
            
            if (!StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(openScrewMergeKey))
                StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.Add(openScrewMergeKey, "0");
            
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[FilthyModel.AbKey] =  "1";
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[openScrewMergeKey] = "0";
        }
    }
    
    [Category(MiniGame)]
    [DisplayName("强制开启新挖沟")]
    public bool OpenNewDitch
    {
        get
        {
            if (StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig.ContainsKey(DitchModel.AB_Ios_DitchPlan_D))
                return StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[DitchModel.AB_Ios_DitchPlan_D] ==  "1";

            return false;
        }
        set
        {
            if(value)
                OpenMiniGame = true;
            
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[DitchModel.AB_Ios_DitchPlan_D] = value ? "1" : "0";
        }
    }
    
    
    
    [Category(MiniGame)]
    [DisplayName("进入Screw Game")]
    public void OpenFilthy()
    {
        HideDebugPanel();
        SceneFsm.mInstance.ChangeState(StatusType.EnterFilthy);
    }
    
    [Category(MiniGame)]
    [DisplayName("小游戏组")]
    public string MiniGroup
    {
        get
        {
           var group = Utils.GetMiniGroup();
           switch (group)
           {
               case Makeover.MiniGroup.None:
                   return "None";
               case Makeover.MiniGroup.Old:
                   return "ASMR";
               case Makeover.MiniGroup.Puzzle:
                   return "拼图";
               case Makeover.MiniGroup.DigTrench:
                   return "挖沟";
           }
           
           return "None";
        }
    }

    
    [Category(MiniGame)]
    [DisplayName("小游戏显示类型")]
    public MiniGroup GetMiniGroup
    {
        get
        {
            return Utils.GetMiniGroup();
        }
        set
        {
            StorageManager.Instance.GetStorage<StorageHome>().AbTestConfig[Utils.MiniGameGroupKey] = ((int)value).ToString();
        }
    }
    
    [Category(MiniGame)]
    [DisplayName("挖沟-重置")]
    public void ResetDigTrench()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().DigTrench.Clear();
        var guideIdList = new List<int>() {558,559,560,561,562,563,564,565,566,567};
        CleanGuideList(guideIdList);
    }

    [Category(MiniGame)]
    [DisplayName("[新]挖沟-重置")]
    public void ResetDitch()
    {
        HideDebugPanel();
        DitchModel.Instance.Ditch.Clear();
        var storyIdList = new List<int>()
        {
            7890101,
            7890201,
            7890301,
            7890401,
        };
        foreach (var storyId in storyIdList)
        {
            StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog.Remove(storyId);
        }
    }


    [Category(MiniGame)]
    [DisplayName("一笔画-测试关卡")]
    public int DebugLevelId
    {
        get { return OnePathModel.Instance.debugLevelId; }
        set { OnePathModel.Instance.debugLevelId = value; }
    }
    
    [Category(MiniGame)]
    [DisplayName("一笔画-重置")]
    public void ResetOnePath()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().OnePath.Clear();
    }
    
    
    [Category(MiniGame)]
    [DisplayName("水管-测试关卡")]
    public int DebugConnectLevelId
    {
        get { return ConnectLineModel.Instance.debugLevelId; }
        set { ConnectLineModel.Instance.debugLevelId = value; }
    }
    
    [Category(MiniGame)]
    [DisplayName("水管-重置")]
    public void ResetConnect()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().ConnectLine.Clear();
    }
    
    
    [Category(MiniGame)]
    [DisplayName("刺激游戏-重置")]
    public void RestStorage()
    {
        StorageManager.Instance.GetStorage<StorageHome>().Stimulate.Clear();
        
        var guideIdList = new List<int>() {
            2001,
            2002,
            2005,
            2006,
            2010,
            2011,
            2012,
            2015,
            2016};
        CleanGuideList(guideIdList);

        StorageManager.Instance.GetStorage<StorageGame>().MergeBoards.Remove((int)MergeBoardEnum.Stimulate);
    }
    
    
    [Category(MiniGame)]
    [DisplayName("心理学-重置")]
    public void ResetPsychologyConnect()
    {
        HideDebugPanel();
        StorageManager.Instance.GetStorage<StorageHome>().Psychology.Clear();
    }
    
    
    [Category("MiniGame")]
    [DisplayName("清理全部小游戏完成标记")]
    public void CleanMiniGame()
    {
        StorageManager.Instance.GetStorage<StorageHome>().DigTrench.FinishInfo.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().FishEatFish.FinishInfo.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().OnePath.FinishInfo.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().ConnectLine.FinishInfo.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().Psychology.FinishInfo.Clear();
        StorageManager.Instance.GetStorage<StorageHome>().Stimulate.FinishInfo.Clear();
    }
}