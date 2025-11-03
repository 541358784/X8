using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RollerControllerConfig
{
    public float addSpeedTime = 0.5f;
    public float loopSpeed = 1000f;
    public float reduceSpeedTime = 4f;
    public float leastLoopSpeedTime = 0f;
    public float bonuceBackRotation = 0f;
    public float bounceBackTime = 0f;

    public RollerControllerConfig(float inAddSpeedTime, float inLoopSpeed, float inReduceSpeedTime,
        float inLeastLoopSpeedTime, float inExtraRotation, float inBounceBackTime)
    {
        addSpeedTime = inAddSpeedTime;
        loopSpeed = inLoopSpeed;
        reduceSpeedTime = inReduceSpeedTime;
        leastLoopSpeedTime = inLeastLoopSpeedTime;
        bonuceBackRotation = inExtraRotation;
        bounceBackTime = inBounceBackTime;
    }
}

public class RollerStateDealClass
{
    public RollerState rollerState;
    public float stateDuration;
    public float lastEndTime = 0;
    public float lastEndRotation = 0;
    private Func<float, float> getStateChangeRotation = f => 0f;
    private Func<RollerStateDealClass, float> getStateUsingTime = (dealClass) => 0f;
    private Action startCallback;
    private Action finishCallback;
    public float stateUsingTime => getStateUsingTime.Invoke(this);

    public virtual void SetLastStateInfo(float endTime, float endRotation)
    {
        lastEndTime = endTime;
        lastEndRotation = endRotation;
        startCallback?.Invoke();
    }

    public RollerStateDealClass(RollerState inRollerState,
        Func<float, float> getStateChangeRotationFunc = null,
        Func<RollerStateDealClass, float> getStateUsingTimeFunc = null,
        Action inStartCallback = null, Action inFinishCallback = null)
    {
        rollerState = inRollerState;
        if (getStateChangeRotationFunc != null)
            getStateChangeRotation = getStateChangeRotationFunc;
        if (getStateUsingTimeFunc != null)
            getStateUsingTime = getStateUsingTimeFunc;
        startCallback = inStartCallback;
        finishCallback = inFinishCallback;
    }

    public void ReplaceRotationFunc(Func<float, float> replaceFunc)
    {
        getStateChangeRotation = replaceFunc;
    }

    public void ReplaceDurationFunc(Func<RollerStateDealClass, float> replaceFunc)
    {
        getStateUsingTime = replaceFunc;
    }

    public void ReplaceStartCallback(Action replaceFunc)
    {
        startCallback = replaceFunc;
    }

    public void ReplaceFinishCallback(Action replaceFunc)
    {
        finishCallback = replaceFunc;
    }

    public virtual float GetRotation(float accumulateTime, out float rotation)
    {
        accumulateTime -= lastEndTime;
        stateDuration = accumulateTime;
        rotation = lastEndRotation;
        if (accumulateTime > stateUsingTime)
        {
            rotation = lastEndRotation + getStateChangeRotation.Invoke(stateUsingTime);
            finishCallback?.Invoke();
            return stateUsingTime + lastEndTime;
        }

        rotation = lastEndRotation + getStateChangeRotation.Invoke(accumulateTime);
        return -1f;
    }
}

public enum RollerState
{
    Idle = 0,
    BounceUp = 1,
    AddSpeed = 2,
    Loop = 3,
    ReduceSpeed = 4,
    BounceBack = 5,
    Stop = 6,
}

public class RollerController
{
    float addSpeedTime = 0.5f;
    float loopSpeed = 1000f;
    float roundValue = 360f;
    float reduceSpeedTime = 4f;
    private float leastLoopSpeedTime = 0f;
    private float bounceBackRotation = 0f;
    float loopSpeedTime = -1f;
    private float bounceBackTime = 0f;
    bool receiveData = false;
    float resultRotation = 0f;
    private float reduceSpeedRotation = 0f;
    private Transform roller;
    private int directuion = -1;
    private float initRotation;

    public RollerController(Transform rollerTarget, RollerControllerConfig config)
    {
        roller = rollerTarget;
        UsingConfig(config);
        InitState();
    }

    public void UsingConfig(RollerControllerConfig config)
    {
        addSpeedTime = config.addSpeedTime;
        loopSpeed = config.loopSpeed;
        reduceSpeedTime = config.reduceSpeedTime;
        leastLoopSpeedTime = config.leastLoopSpeedTime;
        bounceBackRotation = config.bonuceBackRotation;
        bounceBackTime = config.bounceBackTime;
    }

    private float rotationMulti = 1f;

    public void InitState()
    {
        receiveData = false;
        resultRotation = 0f;

        // reduceSpeedRotation = loopSpeed * reduceSpeedTime / 2;
        reduceSpeedRotation = (float) Math.Pow(reduceSpeedTime, 3) * loopSpeed /
                              (float) (3 * Math.Pow(reduceSpeedTime, 2));
        initRotation = roller.localEulerAngles.z / directuion;
        BuildRollerStateList();
    }

    private Action<float> updateSpeedProgress;

    public void BindUpdateSpeedProgress(Action<float> bindingAction)
    {
        updateSpeedProgress = bindingAction;
    }

    public void SetResult(float rotation)
    {
        receiveData = true;
        resultRotation = (rotation + bounceBackRotation) % roundValue;
    }

    private List<RollerStateDealClass> usingRollerStateList;

    public virtual void BuildRollerStateList()
    {
        usingRollerStateList = new List<RollerStateDealClass>()
        {
            new RollerStateDealClass(RollerState.Idle),
            new RollerStateDealClass(RollerState.AddSpeed,
                time =>
                {
                    var currentSpeed = loopSpeed * time / addSpeedTime;
                    return currentSpeed * time / 2;
                },
                dealClass => addSpeedTime),
            new RollerStateDealClass(RollerState.Loop,
                time => loopSpeed * time,
                dealClass =>
                {
                    if (receiveData)
                    {
                        if (loopSpeedTime < 0)
                        {
                            var forceWaitTime = Math.Max(dealClass.stateDuration, leastLoopSpeedTime);
                            var nowRotation =
                                (dealClass.lastEndRotation + loopSpeed * forceWaitTime + reduceSpeedRotation) %
                                roundValue;
                            var extraLoopRotation = 0f;
                            if (nowRotation <= resultRotation)
                            {
                                extraLoopRotation = resultRotation - nowRotation;
                            }
                            else
                            {
                                extraLoopRotation = roundValue + resultRotation - nowRotation;
                            }

                            var extraLoopTime = extraLoopRotation / loopSpeed;
                            loopSpeedTime = forceWaitTime + extraLoopTime;
                        }

                        return loopSpeedTime;
                    }

                    return 999999999f;
                }),
            new RollerStateDealClass(RollerState.ReduceSpeed,
                time =>
                    reduceSpeedRotation * GetRotationProgress(time / reduceSpeedTime),
                dealClass => reduceSpeedTime),
            new RollerStateDealClass(RollerState.BounceBack,
                time => bounceBackTime == 0?-bounceBackRotation:-bounceBackRotation * GetBounceBackProgress(time / bounceBackTime),
                dealClass => bounceBackTime),
            new RollerStateDealClass(RollerState.Stop)
        };
    }

    public RollerStateDealClass GetState(RollerState rollerState, int index = 0)
    {
        var count = 0;
        for (var i = 0; i < usingRollerStateList.Count; i++)
        {
            if (usingRollerStateList[i].rollerState == rollerState)
                if (count == index)
                    return usingRollerStateList[i];
                else
                    count++;
        }

        return null;
    }

    public bool ReplaceState(RollerState rollerState, RollerStateDealClass dealClass, int index = 0)
    {
        var count = 0;
        for (var i = 0; i < usingRollerStateList.Count; i++)
        {
            if (usingRollerStateList[i].rollerState == rollerState)
                if (count == index)
                {
                    usingRollerStateList[i] = dealClass;
                    return true;
                }
                else
                    count++;
        }

        return false;
    }

    private int stateIndex;

    public RollerStateDealClass GetNextState()
    {
        stateIndex++;
        if (stateIndex == usingRollerStateList.Count)
            return null;
        return usingRollerStateList[stateIndex];
    }

    private float lastRotation;
    private float lastAccumulateTime;

    public void Update(SimpleUpdater usingUpdater)
    {
        var accumulateTime = usingUpdater.accumulateTime;
        var rotation = 0f;
        var stopFlag = false;
        while (true)
        {
            var nowState = usingRollerStateList[stateIndex];
            var endTime = nowState.GetRotation(accumulateTime, out rotation);
            if (endTime >= 0)
            {
                var nextState = GetNextState();
                if (nextState == null)
                {
                    stopFlag = true;
                    break;
                }

                nextState.SetLastStateInfo(endTime, rotation);
            }
            else
            {
                break;
            }
        }

        var changeTime = accumulateTime - lastAccumulateTime;
        var changeRotation = rotation - lastRotation;
        var currentSpeed = changeRotation / changeTime;
        updateSpeedProgress?.Invoke(currentSpeed / loopSpeed);
        lastAccumulateTime = accumulateTime;
        lastRotation = rotation;

        float targetAngle = rotation * directuion;
        roller.localEulerAngles = new Vector3(0, 0, targetAngle);

        if (stopFlag)
        {
            usingUpdater.StopUpdater();
            StopRolling();
        }
    }

    private TaskCompletionSource<bool> rollingTask;

    public void StopRolling()
    {
        rollingTask.SetResult(true);
    }

    public async Task StartRolling()
    {
        rollingTask = new TaskCompletionSource<bool>();
        lastRotation = initRotation;
        lastAccumulateTime = 0f;
        stateIndex = 0;
        usingRollerStateList[stateIndex].SetLastStateInfo(0, initRotation);
        loopSpeedTime = -1f;
        var updater = new SimpleUpdater();
        updater.BindingUpdateAction(Update);
        await rollingTask.Task;
    }

    public float GetRotationProgress(float timeProgress)
    {
        var rotationProgress = (float) (1 / 3f - Math.Pow((1 - timeProgress), 3) / 3) / (1 / 3f);
        return rotationProgress;
    }

    public float GetBounceBackProgress(float timeProgress)
    {
        return timeProgress;
    }
}