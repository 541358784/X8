using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class StarrySkyCompassRollerElement : RollerElement
{
    public StarrySkyCompassRollerElement(Transform inTransform) : base(inTransform)
    {
        StarGroup = transform.Find("Star");
        StarText = transform.Find("Star/Text").GetComponent<LocalizeTextMeshProUGUI>();
        HappyGroup = transform.Find("JP");
        ArrowGroup = transform.Find("Arrow");
        EmptyGroup = transform.Find("Ren");
    }

    private Transform StarGroup;
    private Transform HappyGroup;
    private Transform ArrowGroup;
    private Transform EmptyGroup;
    
    private LocalizeTextMeshProUGUI StarText;
    private StarrySkyCompassRollerElementConfig Config;
    private bool IsHappy => Config.IsHappy;
    public override void RefreshState(RollerElementConfig config, params object[] param)
    {
        base.RefreshState(config);
        Config = (StarrySkyCompassRollerElementConfig) config;
        if (Config.Value == 0)
        {
            ArrowGroup.gameObject.SetActive(true);
            StarGroup.gameObject.SetActive(false);
            HappyGroup.gameObject.SetActive(false);
            EmptyGroup.gameObject.SetActive(false);
        }
        else
        {
            ArrowGroup.gameObject.SetActive(false);
            if (Config.IsCollect)
            {
                EmptyGroup.gameObject.SetActive(true);
                StarGroup.gameObject.SetActive(false);
                HappyGroup.gameObject.SetActive(false);
            }
            else
            {
                EmptyGroup.gameObject.SetActive(false);
                var itemConfig = StarrySkyCompassModel.Instance.ResultDicConfig[Config.Value];
                if (IsHappy)
                {
                    StarGroup.gameObject.SetActive(true);
                    HappyGroup.gameObject.SetActive(false);
                    StarText.SetText((itemConfig.HappyScore).ToString());   
                }
                else
                {
                    if (itemConfig.HappyValue > 0)
                    {
                        HappyGroup.gameObject.SetActive(true);
                        StarGroup.gameObject.SetActive(false);
                    }
                    else
                    {
                        StarGroup.gameObject.SetActive(true);
                        HappyGroup.gameObject.SetActive(false);
                        StarText.SetText((itemConfig.Score).ToString());
                    }
                }
            }
        }
    }
}

public class StarrySkyCompassRollerElementConfig : RollerElementConfig
{
    public int Value;
    public bool IsCollect;
    public bool IsHappy;
    public StarrySkyCompassRollerElementConfig(int inIndex, int value,bool isCollect,bool isHappy) :
        base(inIndex)
    {
        Value = value;
        IsCollect = isCollect;
        IsHappy = isHappy;
    }
}

public class StarrySkyCompassRollerView : RollerView
{
    public override void RefreshRewardState(params object[] param)
    {
        var configList = GetRollerElementConfigList();
        for (var i = 0; i < configList.Count; i++)
        {
            var config = configList[i];
            var element = rollerElementList[config.index];
            var tempConfig = (config as StarrySkyCompassRollerElementConfig);
            tempConfig.IsCollect = IsHappy && tempConfig.Value > 0 && Storage.IsInHappyTime() && Storage.HappySpinHistory.Contains(tempConfig.Value);
            element.RefreshState(tempConfig, param);
        }
    }
    public override RollerElement CreateRollerElement(Transform elementTransform)
    {
        return CreateRollerElement<StarrySkyCompassRollerElement>(elementTransform);
    }

    public override RollerController CreateRollingController()
    {
        return CreateRollingController<StarrySkyCompassRollerController>();
    }
    
    public override List<RollerElementConfig> BuildRollerElementConfigList()
    {
        var resultConfig = Config.TurntableResultList;
        var configList = new List<RollerElementConfig>();
        for (var i = 0; i < resultConfig.Count; i++)
        {
            var configId = resultConfig[i];
            bool isCollect = IsHappy && configId > 0 && Storage.IsInHappyTime() && Storage.HappySpinHistory.Contains(configId);
            configList.Add(new StarrySkyCompassRollerElementConfig(i,configId,isCollect,IsHappy));
        }
        return configList;
    }

    public override RollerControllerConfig BuildRollerConfig()
    {
        var globalConfig = StarrySkyCompassModel.Instance.GlobalConfig;
        return new RollerControllerConfig(Config.AddSpinSpeedTime, Config.MaxSpinSpeed, Config.ReduceSpinSpeedTime,
            Config.KeepMaxSpinSpeedTime, Config.BounceBackRotation, Config.BounceBackTime);
    }

    public override Transform GetWheelElementTransform(int elementIndex)
    {
        return transform.Find(elementIndex.ToString());
    }

    public override Transform GetWheelContentTransform()
    {
        return transform;
    }

    public override BoxCollider2D GetRollerStartBtn()
    {
        return transform.GetComponent<BoxCollider2D>();
    }
    
    public StarrySkyCompassTurntableConfig Config;
    public bool IsHappy;
    public StorageStarrySkyCompass Storage;
    public Transform WinEffect;
    public StarrySkyCompassRollerView(Transform inTransform,StarrySkyCompassTurntableConfig config,bool isHappy,StorageStarrySkyCompass storage,Transform winEffect) : base(inTransform)
    {
        Config = config;
        IsHappy = isHappy;
        Storage = storage;
        WinEffect = winEffect;
    }

    public override void ResetRollerOnShow()
    {
        base.ResetRollerOnShow();
    }

    public override async Task OnClickStartBtn()
    {
        
    }

    public override async Task GetDataReceiveTask()
    {
    }

    public override async Task OnRollingFinish()
    {
    }

    private int SpinResult;
    public override int GetStopIndex()
    {
        return (int) SpinResult;
    }

    public override async Task CollectReward()
    {
    }

    public override async Task Open()
    {
    }

    public override async Task Close()
    {
    }

    public override async Task PerformRoller()
    {
        // Show();
        // ResetRollerOnShow();
        // await Open();
        // await StartSpin();
        await OnClickStartBtn();
        var controller = CreateRollingController();
        var spinTask = controller.StartRolling();
        // await GetDataReceiveTask();
        controller.SetResult(GetStopRotation());
        await spinTask;
        // await OnRollingFinish();
        // await CollectReward();
    }

    public Task PerformTurntable(int resultIndex)
    {
        AudioManager.Instance.PlaySoundById(170+Config.Id);
        SpinResult = resultIndex;
        return PerformRoller();
    }

    public Transform GetElementTransform(int index)
    {
        return rollerElementList[index].transform;
    }
}