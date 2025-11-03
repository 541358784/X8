// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/19/17:35
// Ver : 1.0.0
// Description : UpdateScheduler.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpdateScheduler : Manager<UpdateScheduler>
{
    private  UnityAction updateAction;
    private  UnityAction secondUpdateAction;
    private  UnityAction halfSecondUpdateAction;

    private  UnityAction lateUpdateAction;
    private  UnityAction fixedUpdateAction;

    private float halfSecondIntervalUpdaterNextActionTime;
    private float secondIntervalUpdaterNextActionTime;

    public void DetachFromListener()
    {
        updateAction = null;
        secondUpdateAction = null;
        halfSecondUpdateAction = null;
        lateUpdateAction = null;
        fixedUpdateAction = null;
    }

    public void Update()
    {
        if (updateAction != null)
        {
            updateAction.Invoke();
        }

        if (Time.time > secondIntervalUpdaterNextActionTime)
        {
            secondIntervalUpdaterNextActionTime = Time.time + 1.0f;

            if (secondUpdateAction != null)
            {
                secondUpdateAction.Invoke();
            }
        }

        if (Time.time > halfSecondIntervalUpdaterNextActionTime)
        {
            halfSecondIntervalUpdaterNextActionTime = Time.time + 0.5f;

            if (halfSecondUpdateAction != null)
            {
                halfSecondUpdateAction?.Invoke();
            }
        }
    }

    public void LateUpdate()
    {
        if (lateUpdateAction == null)
            return;
        lateUpdateAction.Invoke();
    }

    public void FixedUpdate()
    {
        if (fixedUpdateAction == null)
            return;
        fixedUpdateAction?.Invoke();
    }

    #region Hook Update

    public void HookUpdate(UnityAction updateable)
    {
        updateAction += updateable;
    }

    public void HookSecondUpdate(UnityAction updateable)
    {
        secondUpdateAction += updateable;
    }

    public void HookHalfSecondUpdate(UnityAction updateable)
    {
        halfSecondUpdateAction += updateable;
    }

    public void UnhookUpdate(UnityAction updateable)
    {
        updateAction -= updateable;
        secondUpdateAction -= updateable;
        halfSecondUpdateAction -= updateable;
    }

    public void HookFixedUpdate(UnityAction fixedUpdateable)
    {
        fixedUpdateAction += fixedUpdateable;
    }

    public void UnhookFixedUpdate(UnityAction fixedUpdateable)
    {
        fixedUpdateAction -= fixedUpdateable;
    }

    public void HookLateUpdate(UnityAction lateUpdateable)
    {
       
        lateUpdateAction += lateUpdateable;
    }

    public void UnhookLateUpdate(UnityAction lateUpdateable)
    {
        lateUpdateAction -= lateUpdateable;
    }

    #endregion
}