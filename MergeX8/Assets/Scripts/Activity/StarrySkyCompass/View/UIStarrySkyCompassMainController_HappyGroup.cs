using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using UnityEngine;

public partial class UIStarrySkyCompassMainController
{
    public List<Transform> HappyPointList = new List<Transform>();
    public Transform HappyTimeGroup;
    public LocalizeTextMeshProUGUI HappyTimeText;
    private Transform HappyEffect;
    public void InitHappyGroup()
    {
        for (var i = 0; i < Model.GlobalConfig.HappyMaxCount; i++)
        {
            var point = transform.Find("Root/JPGroup/" + i + "/Full");
            point.gameObject.SetActive(false);
            HappyPointList.Add(point);
        }
        HappyEffect = transform.Find("Root/JPGroup/Image (1)/fx_reward");
        UpdateHappyProgress();
        HappyTimeGroup = transform.Find("Root/HappyTimeGroup");
        HappyTimeGroup.gameObject.SetActive(Storage.IsInHappyTime());
        HappyTimeText = GetItem<LocalizeTextMeshProUGUI>("Root/HappyTimeGroup/TimeText");
        InvokeRepeating("UpdateHappyTime",0f,1f);
    }

    public void UpdateHappyTime()
    {
        HappyTimeText.SetText(Storage.GetHappyTimeText());
        if (!IsPerforming)
        {
            var isHappyTime = Storage.IsInHappyTime();
            if (LastIsHappySpin != isHappyTime)
            {
                LastIsHappySpin = isHappyTime;
                HappyTimeGroup.gameObject.SetActive(LastIsHappySpin);
                HappyRollerView.gameObject.SetActive(LastIsHappySpin);
                RollerView.gameObject.SetActive(!LastIsHappySpin);
                UpdateHappyProgress();
                // if (LastIsHappySpin)
                // {
                //     AudioManager.Instance.PlayMusic("bgm_star_1");
                // }
                // else
                // {
                //     AudioManager.Instance.PlayMusic("bgm_star_1");
                // }
            }
        }
    }
    public void UpdateHappyProgress()
    {
        if (Storage.IsInHappyTime())
        {
            foreach (var point in HappyPointList)
            {
                point.gameObject.SetActive(true);
            }
            HappyEffect.gameObject.SetActive(true);
        }
        else
        {
            for (var i = 0; i < HappyPointList.Count; i++)
            {
                HappyPointList[i].gameObject.SetActive(i < Storage.HappyValue);
            }
            HappyEffect.gameObject.SetActive(false);
        }
    }
    public void FlyHappyPoint(Vector3 startPos,int addValue,Action callback = null)
    {
        int count = addValue/5;
        if (count == 0)
            count = 1;
        if (count > 10)
            count = 10;
        float delayTime = 0.05f;
        for (int i = 0; i < count; i++)
        {
            int index = i;
            Transform target = null;
            var emptyCount = 0;
            for (var i1 = 0; i1 < HappyPointList.Count; i1++)
            {
                if (HappyPointList[i1].gameObject.activeSelf)
                    continue;
                if (emptyCount == index)
                {
                    target = HappyPointList[i1];
                    break;
                }
                emptyCount++;
            }
            Vector3 position = target.position;
            target.gameObject.SetActive(true);
            FlyGameObjectManager.Instance.FlyObject(target.gameObject, startPos, position, true, 0.5f,
                delayTime * i, () =>
                {
                    target.gameObject.SetActive(true);
                    if (target == HappyPointList.Last())
                    {
                        HappyEffect.gameObject.SetActive(true);
                    }
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(position);
                    ShakeManager.Instance.ShakeLight();
                    if (index == count - 1)
                    {
                        callback?.Invoke();
                    }
                });
            target.gameObject.SetActive(false);
        }
    }
}