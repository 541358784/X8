using System;
using System.Collections.Generic;
using Deco.Node;
using Decoration;
using Farm.Model;
using GamePool;
using Screw.UserData;
using UnityEngine;
using UnityEngine.UI;

namespace Screw.Module
{
    public class FlyModule : Singleton<FlyModule>
    {
        public void Fly(int id, int num, Vector3 srcPos, Action arriveAction = null, Action endAction = null, bool autoEvent = true)
        {
            GameObject clone = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.ResourceItem);
            UpdateImage(clone, id);
                    
            Fly(clone, num, srcPos, ResBarModule.Instance.GetFlyDestPosition(id), () =>
            {
                if (autoEvent)
                {
                    ResType resType = UserData.UserData.GetResType(id);
                    EventDispatcher.Instance.DispatchEventImmediately(ConstEvent.SCREW_REFRESH_RES, resType, UserData.UserData.Instance.GetRes(resType), num, true);
                }
                arriveAction?.Invoke();
            }, endAction);
                    
            GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.ResourceItem, clone);
        }
        
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

        private void UpdateImage(GameObject obj, int id)
        {
            Image icon = obj.transform.GetComponentDefault<Image>("Icon");
            if (icon == null)
                return;

            icon.sprite = UserData.UserData.GetResourceIcon(id);
        }
    }
}