using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using GamePool;
using UnityEngine;

public class RewardTipsManager : Manager<RewardTipsManager>
{
    public void ShowRewardTipsAndUpdateRes(Vector3 position,List<ResData> resDatas,bool IsUpdateRes,GameBIManager.ItemChangeReasonArgs _bi,Action onEnd=null)
    {
        if (IsUpdateRes)
        {
            UserData.Instance.AddRes(resDatas, _bi);
        }

        StartCoroutine(DeilyShowReward(position,resDatas,onEnd));
    }
    public void ShowRewardTips(Vector3 position,List<ResData> resDatas)
    {

        StartCoroutine(DeilyShowReward(position,resDatas));
    }
    IEnumerator DeilyShowReward(Vector3 position,List<ResData> resDatas,Action onEnd=null)
    {
        for (int i = 0; i < resDatas.Count; i++)
        {
            ShowRewardTip(position,resDatas[i].id,resDatas[i].count);
            yield return new WaitForSeconds(0.5f);
        }
        onEnd?.Invoke();
    }
    private void ShowRewardTip(Vector3 position,int rewardID,int rewardCount)
    {
   
        //GameObject tips = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.PopRewardTips);
        GameObject tips = null;
        if(tips == null)
            return;

        tips.GetOrCreateComponent<PopRewardTipsController>().PlayAnim(rewardID,rewardCount, () =>
        {
            //GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.PopRewardTips, tips);
        });

        tips.transform.parent = UIRoot.Instance.mUIRoot.transform;
        tips.transform.position = position;
        tips.gameObject.transform.localScale = Vector3.one;
        tips.gameObject.SetActive(true);
    }
}