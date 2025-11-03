using System;
using Deco.Node;
using Decoration;
using Farm.Model;
using GamePool;
using UnityEngine;

namespace Farm.FarmFly
{
    public class FarmFlyManager : Singleton<FarmFlyManager>
    {
        public void Fly(GameObject flyItem, int count, Vector3 srcPos, Vector3 destPos, Action arriveAction = null, Action endAction = null, float flyStep = 0.1f, float flyTime = 1f)
        {
            srcPos.z = 0;
            destPos.z = 0;
            
            count = Math.Min(count, 10);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                
                FlyGameObjectManager.Instance.FlyObject(flyItem, srcPos, destPos, true, flyTime, flyStep * i, () =>
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(destPos);
                    ShakeManager.Instance.ShakeLight();
                        
                    if(index == 0)
                        arriveAction?.Invoke();
                    
                    if(index == count-1)
                        endAction?.Invoke();
                });
            }
        }

        public void FlyProductItemToWarehouse(DecoNode node, int id, int count)
        {
            if(node.IconTipTransform == null)
                return;
            
            var position = DecoSceneRoot.Instance.mSceneCamera.WorldToScreenPoint(node.IconTipTransform.transform.position);
            var screenPos = UIRoot.Instance.mUICamera.ScreenToWorldPoint(position);
            screenPos.z = 0;
            
            GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ResourceItem);
            FlyGameObjectManager.Instance.UpdateMergeItemImage(clone,(int)id);

            Fly(clone, count, screenPos, FarmModel.Instance.WarehouseTransform.position);
            
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ResourceItem, clone);
        }

        public void FlyItem(int id, int num, Vector3 srcPos, Action arriveAction = null, Action endAction = null)
        {
            GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ResourceItem);
            FlyGameObjectManager.Instance.UpdateMergeItemImage(clone,id);
                    
            Fly(clone, num, srcPos, FarmModel.Instance.LevelUpTransform.transform.position,arriveAction, endAction);
                    
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ResourceItem, clone);
        }
    }
}