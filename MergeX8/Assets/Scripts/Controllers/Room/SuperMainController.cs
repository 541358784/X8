using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class SuperMainController : MonoBehaviour, IMainController
{
    public UIHomeMainController mainController { get; set; }


    public void Init(UIHomeMainController mainController)
    {
        this.mainController = mainController;
    }

    public virtual void Show()
    {
    }

    public virtual void Hide()
    {
    }

    public virtual void MoneyAnim(UserData.ResourceId resId, int subNum, float time)
    {
    }

    public virtual void InitMoney(UserData.ResourceId resId, int money)
    {
    }

    public virtual bool IsShow()
    {
        return gameObject.activeSelf;
    }

    public GameObject GetItem(string key, GameObject parObj = null)
    {
        if (parObj == null)
        {
            parObj = this.gameObject;
        }

        var obj = FindObj(key, parObj);
        if (obj == null)
        {
            DragonU3DSDK.DebugUtil.LogError("GetItem failed, window controller name : {0},  key = {1}",
                GetType().ToString(), key);
        }

        return obj;
    }

    public bool TryGetItem(string key, out GameObject go, GameObject parObj = null)
    {
        if (parObj == null)
        {
            parObj = this.gameObject;
        }

        var trans = parObj.transform.Find(key);
        go = trans ? trans.gameObject : null;
        return go != null;
    }

    public T GetItem<T>(string key, GameObject parObj = null)
    {
        var go = GetItem(key, parObj);
        return GetItem<T>(go);
    }

    public T GetItem<T>(GameObject go)
    {
        if (go != null)
        {
            var com = go.GetComponent<T>();
            if (com == null)
            {
                DragonU3DSDK.DebugUtil.LogError(
                    "GetItem failed, window controller name : {0},  game object name = {1}, Component type:{2}",
                    GetType().ToString(), go.name, typeof(T).ToString());
            }

            return com;
        }

        return default(T);
    }

    public GameObject FindObj(string path, GameObject par = null)
    {
        if (par == null)
        {
            par = this.gameObject;
        }

        GameObject obj = par.transform.Find(path)?.gameObject;
        return obj;
    }    
    
    public virtual Transform GetTransform()
    {
        return transform;
    }
}