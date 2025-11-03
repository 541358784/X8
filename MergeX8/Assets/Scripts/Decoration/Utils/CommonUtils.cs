using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Decoration;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;

public static partial class CommonUtils
{
    public static bool IsTouchUGUI(){
        if(EventSystem.current==null)return false;
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        if (Input.touchCount > 0 ? EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) : EventSystem.current.IsPointerOverGameObject())
#else
        if (EventSystem.current.IsPointerOverGameObject())
#endif
            return true;
        else
            return false;
    }
    
    
    public static void ShowTipAtTouchPosition(string cotentkey){

    }

    public static T GetOrCreateComponent<T>(GameObject owner) where T : MonoBehaviour
    {
        T targetT = owner.GetComponent<T>();
        if (targetT == null)
            targetT = owner.AddComponent<T>();

        return targetT;
    }
    

    public static int GetTaskValue(StorageTaskItem taskItem)
    {
        if (taskItem == null)
        {
            return 0;
        }

        int price = 0;
        for (var i = 0; i < taskItem.RewardTypes.Count; i++)
        {
            if (taskItem.RewardTypes[i] == (int)UserData.ResourceId.Coin ||
                taskItem.RewardTypes[i] == (int)UserData.ResourceId.RecoverCoinStar || 
                taskItem.RewardTypes[i] == (int)UserData.ResourceId.RareDecoCoin)
            {
                if (taskItem.RewardNums.Count > i)
                    price = taskItem.RewardNums[i];

                break;
            }
        }

        if (price == 0)
        {
            foreach (var itemId in taskItem.ExtraRewardTypes)
            {
                price += OrderConfigManager.Instance.GetOrderItemPrice(itemId);
            }
        }

        if (price == 0)
        {
            foreach (var itemId in taskItem.ItemIds)
            {
                price += OrderConfigManager.Instance.GetOrderItemPrice(itemId);
            }
        }

        return price;
    }
    
    public static Sprite LoadDecoItemIconSprite(int areaId, string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
        {
            return null;
        }
        else
        {
            var icon = ResourcesManager.Instance.GetSpriteVariant($"Area{areaId}Atlas", iconName);
            return icon;
        }
    }

    public static Sprite LoadAreaIconSprite(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
        {
            return null;
        }
        else
        {
            var icon = ResourcesManager.Instance.GetSpriteVariant($"DecoAreaIconAtlas", iconName);
            return icon;
        }
    }

    public static void AddChildTo(Transform parent, Transform obj)
    {
        if (obj != null && parent != null)
        {
            obj.transform.SetParent(parent);
            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;
            obj.localScale = Vector3.one;
        }
    }
    public static void DicSafeSet<T1, T2>(Dictionary<T1, T2> dic, T1 key, T2 value)
    {
        if (dic.ContainsKey(key))
        {
            dic[key] = value;
        }
        else
        {
            dic.Add(key, value);
        }
    }
    public static bool IsIdValid(int id)
    {
        return id > 0;
    }
    
    public static int GetInvalId()
    {
        return -1;
    }
    public static void ClearChildGameObject(Transform parent)
    {
        if (parent == null)
            return;

        if (parent.childCount <= 0)
            return;

        List<Transform> transList = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            transList.Add(parent.GetChild(i).GetComponent<Transform>());
        }

        parent.DetachChildren();
        foreach (var trans in transList)
        {
            GameObject.Destroy(trans.gameObject);
        }
    }
    
}