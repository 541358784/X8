using System;
using Activity.SaveTheWhales;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Farm.FarmFly;
using Farm.Model;
using Farm.View;
using Gameplay;
using GamePool;
using UnityEngine;

namespace Farm.Order
{
    public partial class FarmOrderManager
    {
        private void FlyReward(OrderCell order, Action endAction = null)
        {
            int index = 0;
            for (int i = 0; i < order._storage.RewardIds.Count; i++)
            {
                int id = order._storage.RewardIds[i];
                int num = order._storage.RewardNums[i];

                if (id == (int)UserData.ResourceId.Farm_Exp)
                {
                    GameObject flyItem = order.GetRewardIcon(id);
                    FarmFlyManager.Instance.FlyItem(id, num, flyItem.transform.position, () =>
                    {
                        EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_REFRESH_LEVEL_EXP);
                    }, () =>
                    {
                        index++;
                        if (index >= order._storage.RewardIds.Count-1)
                            endAction?.Invoke();
                    });
                }
                else
                {
                    GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ResourceItem);
                    FlyGameObjectManager.Instance.UpdateMergeItemImage(clone,(int)id);

                    GameObject flyItem = order.GetRewardIcon(id);

                    Vector3 destPos = FarmModel.Instance.WarehouseTransform.position;
                    if (!UserData.Instance.IsFarmRes(id))
                        destPos = FarmModel.Instance.MainPlayTransform.position;
                    
                    FarmFlyManager.Instance.Fly(clone, num, flyItem.transform.position, destPos, () =>
                    {
                        index++;
                        if (index >= order._storage.RewardIds.Count-1)
                            endAction?.Invoke();
                    });
            
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ResourceItem, clone);
                }
            }
        }
    }
}