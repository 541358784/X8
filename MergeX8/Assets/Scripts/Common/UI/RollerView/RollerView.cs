using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RollerElement : TransformHolder
{
    public RollerElement(Transform inTransform) : base(inTransform)
    {
    }

    public virtual void RefreshState(RollerElementConfig config, params object[] param)
    {
    }
}

public class RollerElementConfig
{
    public int index;

    public RollerElementConfig(int inIndex)
    {
        index = inIndex;
    }
}

public abstract class RollerView : TransformHolder
{
    public List<RollerElementConfig> rollerElementConfigList;

    public virtual void InitRollerElementConfigList()
    {
        rollerElementConfigList = BuildRollerElementConfigList();
    }

    public virtual List<RollerElementConfig> BuildRollerElementConfigList()
    {
        return new List<RollerElementConfig>();
    }

    public virtual List<RollerElementConfig> GetRollerElementConfigList()
    {
        return rollerElementConfigList;
    }

    public RollerControllerConfig rollerConfig;

    public virtual void InitRollerConfig()
    {
        rollerConfig = BuildRollerConfig();
    }

    public virtual RollerControllerConfig BuildRollerConfig()
    {
        return new RollerControllerConfig(1.5f, 1000f, 4f, 0, 15f, 0.3f);
    }

    public virtual RollerControllerConfig GetRollerConfig()
    {
        return rollerConfig;
    }

    public virtual int GetElementCount()
    {
        return GetRollerElementConfigList().Count;
    }

    public abstract Transform GetWheelElementTransform(int elementIndex);

    public virtual TRollerController CreateRollingController<TRollerController>()
        where TRollerController : RollerController
    {
        var constructor =
            typeof(TRollerController).GetConstructor(new[] {typeof(Transform), typeof(RollerControllerConfig)});
        return (TRollerController) constructor.Invoke(
            new object[] {GetWheelContent(), GetRollerConfig()});
    }

    public virtual RollerController CreateRollingController()
    {
        return CreateRollingController<RollerController>();
    }

    public virtual TRollerElement CreateRollerElement<TRollerElement>(Transform elementTransform)
        where TRollerElement : RollerElement
    {
        var constructor = typeof(TRollerElement).GetConstructor(new[] {typeof(Transform)});
        var rollerElement = (TRollerElement) constructor.Invoke(
            new object[] {elementTransform});
        return rollerElement;
    }

    public virtual RollerElement CreateRollerElement(Transform elementTransform)
    {
        return CreateRollerElement<RollerElement>(elementTransform);
    }

    public List<RollerElement> rollerElementList;

    public virtual void InitRollerElementList()
    {
        rollerElementList = new List<RollerElement>();
        for (var i = 0; i < GetElementCount(); i++)
        {
            rollerElementList.Add(CreateRollerElement(GetWheelElementTransform(i)));
        }
    }

    public Transform wheelContent;

    public abstract Transform GetWheelContentTransform();

    // {
    //     return transform.Find("Root/AnimCenter/Wheel");
    // }
    public virtual void InitWheelContent()
    {
        wheelContent = GetWheelContentTransform();
    }

    public virtual Transform GetWheelContent()
    {
        return wheelContent;
    }

    private const float roundRotationZ = 360f;

    public virtual float GetStopRotation()
    {
        return GetStopIndex() * roundRotationZ / GetElementCount();
        // return (GetElementCount() - GetStopIndex()) * roundRotationZ / GetElementCount();
    }

    public virtual int GetStopIndex()
    {
        return 0;
    }

    public RollerView(Transform inTransform) : base(inTransform)
    {
    }

    public void Init()
    {
        InitRollerElementConfigList();
        InitRollerConfig();
        InitRollerElementList();
        InitWheelContent();
    }

    public virtual BoxCollider2D GetRollerStartBtn()
    {
        return transform.GetComponent<BoxCollider2D>();
    }

    public virtual async Task StartSpin()
    {
        var clickTask = new TaskCompletionSource<bool>();
        var pointerHandler = GetRollerStartBtn().gameObject.AddComponent<PointerEventCustomHandler>();
        pointerHandler.BindingPointerClick((evt) => { clickTask.SetResult(true); });
        await clickTask.Task;
        GameObject.Destroy(pointerHandler);
    }

    public virtual void RefreshRewardState(params object[] param)
    {
        var configList = GetRollerElementConfigList();
        for (var i = 0; i < configList.Count; i++)
        {
            var config = configList[i];
            var element = rollerElementList[config.index];
            element.RefreshState(config, param);
        }
    }

    public virtual void ResetRotation()
    {
        GetWheelContent().localEulerAngles = new Vector3(0, 0, 0);
    }

    public virtual void ResetRollerOnShow()
    {
        RefreshRewardState();
        ResetRotation();
    }

    public virtual async Task PerformRoller()
    {
        Show();
        ResetRollerOnShow();
        await Open();
        await StartSpin();
        await OnClickStartBtn();
        var controller = CreateRollingController();
        var spinTask = controller.StartRolling();
        await GetDataReceiveTask();
        controller.SetResult(GetStopRotation());
        await spinTask;
        await OnRollingFinish();
        await CollectReward();
    }

    public virtual async Task GetDataReceiveTask()
    {
    }

    public virtual async Task CollectReward()
    {
        await Close();
    }

    public virtual async Task Open()
    {
        Show();
    }

    public virtual async Task Close()
    {
        Hide();
    }

    public virtual async Task OnClickStartBtn()
    {
    }

    public virtual async Task OnRollingFinish()
    {
    }
}