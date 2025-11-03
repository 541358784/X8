using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using UnityEngine;

public class SnakeLadderRollerElement : RollerElement
{
    public SnakeLadderRollerElement(Transform inTransform) : base(inTransform)
    {
        Text = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
    }
    public int StepMultiValue = 1;
    private LocalizeTextMeshProUGUI Text;
    public override void RefreshState(RollerElementConfig config, params object[] param)
    {
        base.RefreshState(config);
        var configPrivate = (SnakeLadderRollerElementConfig) config;
        if (configPrivate.Value == 0)
        {
            Text.SetText("?");
        }
        else
        {
            var stepNum = StepMultiValue * configPrivate.Value;
            Text.SetText(stepNum.ToString());
        }
    }
}

public class SnakeLadderRollerElementConfig : RollerElementConfig
{
    public int Value;

    public SnakeLadderRollerElementConfig(int inIndex, int value) :
        base(inIndex)
    {
        Value = value;
    }
}

public class SnakeLadderRollerView : RollerView
{
    public int StepMultiValue = 1;
    public override void RefreshRewardState(params object[] param)
    {
        var doubleBack = transform.Find("Double");
        doubleBack.gameObject.SetActive(StepMultiValue > 1);
        var configList = GetRollerElementConfigList();
        for (var i = 0; i < configList.Count; i++)
        {
            var config = configList[i];
            var element = rollerElementList[config.index];
            (element as SnakeLadderRollerElement).StepMultiValue = StepMultiValue;
            element.RefreshState(config, param);
        }
    }
    public override RollerElement CreateRollerElement(Transform elementTransform)
    {
        return CreateRollerElement<SnakeLadderRollerElement>(elementTransform);
    }

    public override RollerController CreateRollingController()
    {
        return CreateRollingController<SnakeLadderRollerController>();
    }

    public override List<RollerElementConfig> BuildRollerElementConfigList()
    {
        var resultConfig = SnakeLadderModel.Instance.GlobalConfig.TurntableResultList;
        var configList = new List<RollerElementConfig>();
        for (var i = 0; i < 8; i++)
        {
            configList.Add(new SnakeLadderRollerElementConfig(i,resultConfig[i]));
        }
        return configList;
    }

    public override RollerControllerConfig BuildRollerConfig()
    {
        var globalConfig = SnakeLadderModel.Instance.GlobalConfig;
        return new RollerControllerConfig(globalConfig.AddSpinSpeedTime, globalConfig.MaxSpinSpeed, globalConfig.ReduceSpinSpeedTime,
            globalConfig.KeepMaxSpinSpeedTime, globalConfig.BounceBackRotation, globalConfig.BounceBackTime);
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
    

    public SnakeLadderRollerView(Transform inTransform) : base(inTransform)
    {
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
        SpinResult = resultIndex;
        return PerformRoller();
    }
}