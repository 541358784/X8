// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-24 5:50 PM
// Ver : 1.0.0
// Description : TransformHolder.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MonoUpdateProxy : MonoBehaviour
{
    private Action updateAction;

    public void BindingAction(Action inUpdateAction)
    {
        updateAction = inUpdateAction;
    }

    private void Update()
    {
        updateAction?.Invoke();
    }
}

public class MonoDestroyProxy : MonoBehaviour
{
    private Action destoryAction;
    public void BindingAction(Action inDestroyAction)
    {
        destoryAction = inDestroyAction;
    }
    private void OnDestroy()
    {
        destoryAction?.Invoke();
    }
}
public class TransformHolder
{
    public Transform transform;
    public MonoUpdateProxy updateProxy;
    public MonoDestroyProxy destroyProxy;

    public TransformHolder(Transform inTransform)
    {
        transform = inTransform;
    }

    public void EnableUpdate()
    {
        if (updateProxy == null)
        {
            updateProxy = transform.gameObject.AddComponent<MonoUpdateProxy>();
            updateProxy.BindingAction(Update);
        }
    }

    public void DisableUpdate()
    {
        if (updateProxy)
        {
            updateProxy.BindingAction(null);
        }
    }

    public void EnableDestroy()
    {
        if (destroyProxy == null)
        {
            destroyProxy = transform.gameObject.AddComponent<MonoDestroyProxy>();
            destroyProxy.BindingAction(OnDestroy);
        }
    }
    public void DisableDestroy()
    {
        if (destroyProxy)
        {
            destroyProxy.BindingAction(null);
        }
    }

    public virtual void OnOpen()
    {
    }


    public async virtual Task OnClose()
    {
    }


    public virtual void Update()
    {
    }

    public virtual void OnDestroy()
    {
    }

    public virtual void Hide()
    {
        transform.gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        transform.gameObject.SetActive(true);
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(transform.gameObject);
    }
}