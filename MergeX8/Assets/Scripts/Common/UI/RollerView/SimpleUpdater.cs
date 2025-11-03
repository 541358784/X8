using System;
using UnityEngine;


public class NewMachineSimpleUpdater : MonoBehaviour
{
    private Action lateUpdateAction;
    private Action updateAction;

    private void LateUpdate()
    {
        lateUpdateAction?.Invoke();
    }

    private void Update()
    {
        updateAction?.Invoke();
    }

    public void AddLateUpdateAction(Action inLateUpdateAction)
    {
        lateUpdateAction = inLateUpdateAction;
    }

    public void AddUpdateAction(Action inUpdateAction)
    {
        updateAction = inUpdateAction;
    }
}

public class SimpleUpdater : IUpdateable, ILateUpdateable
{
    public float accumulateTime = 0;
    private Action<SimpleUpdater> updateAction;
    private Action<SimpleUpdater> lateUpdateAction;
    private NewMachineSimpleUpdater newUpdater;

    public SimpleUpdater()
    {
        UpdateScheduler.Instance.HookUpdate(Update);
        UpdateScheduler.Instance.HookLateUpdate(LateUpdate);
    }

    public virtual void Update()
    {
        accumulateTime += Time.deltaTime;
        if (updateAction != null)
        {
            updateAction(this);
        }
    }

    public virtual void LateUpdate()
    {
        if (lateUpdateAction != null)
        {
            lateUpdateAction(this);
        }
    }

    public float GetAccumulateTime()
    {
        return accumulateTime;
    }

    // public virtual void OnContextDestroy()
    // {
    //     if (newUpdater != null)
    //         UnityEngine.Object.Destroy(newUpdater);
    //     UpdateScheduler.UnhookUpdate(this);
    //     UpdateScheduler.UnhookLateUpdate(this);
    //     // updateAction = null;
    //     // lateUpdateAction = null;
    // }

    public virtual void StopUpdater()
    {
        if (newUpdater != null)
            UnityEngine.Object.Destroy(newUpdater);
        UpdateScheduler.Instance.UnhookUpdate(Update);
        UpdateScheduler.Instance.UnhookLateUpdate(LateUpdate);
    }

    public void BindingUpdateAction(Action<SimpleUpdater> inUpdateAction)
    {
        updateAction = inUpdateAction;
    }

    public void BindingLateUpdateAction(Action<SimpleUpdater> inLateUpdateAction)
    {
        lateUpdateAction = inLateUpdateAction;
    }

    public void BindingNewLateUpdateAction(Action<SimpleUpdater> inNewLateUpdateAction, Transform bindingTransform)
    {
        newUpdater = bindingTransform.gameObject.AddComponent<NewMachineSimpleUpdater>();
        newUpdater.AddLateUpdateAction(() => inNewLateUpdateAction(this));
    }
}