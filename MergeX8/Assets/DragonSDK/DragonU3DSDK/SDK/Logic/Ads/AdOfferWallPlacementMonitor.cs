/*
 * 可以监控的状态：
 * Active【care parent】、Interactable、Custom
 */

using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;

namespace DragonPlus
{
    public class AdOfferWallPlacementMonitor : MonoBehaviour, AdPlacementMonitor
    {
        public delegate bool CustomCheck();
        public static void Bind(GameObject obj, string _placementId, CustomCheck _customCheck = null)
        {
            if (obj.GetComponent<AdOfferWallPlacementMonitor>() != null)
            {
                DebugUtil.LogError("$已经绑定了");
                return;
            }
            
            obj.AddComponent<AdOfferWallPlacementMonitor>().Bind(_placementId, _customCheck);
        }
        
        private string placementId;
        private CustomCheck customCheck;
        private bool validShould;
        private float lastCheckTime;
        
        private void Bind(string _placementId, CustomCheck _customCheck)
        {
            placementId = _placementId;
            customCheck = _customCheck;
            validShould = false;

            if (string.IsNullOrEmpty(placementId))
            {
                DebugUtil.LogError($"placementId为无效值");
            }

            if (GetComponent<Button>() == null)
            {
                DebugUtil.LogError($"需要绑定在button上");
            }
        }

        private void Awake()
        {
            AdLogicManager.Instance.RegisterPlacementMonitor(this);
        }

        private void OnDestroy()
        {
            customCheck = null;
            AdLogicManager.Instance.UnregisterPlacementMonitor(this);
        }
        
        public void Check()
        {
            if (Time.realtimeSinceStartup - lastCheckTime > 0.1f)
            {
                lastCheckTime = Time.realtimeSinceStartup;
                
                if (string.IsNullOrEmpty(placementId) || GetComponent<Button>() == null)
                {
                    return;
                }
            
                if (!validShould)
                {
                    if (Interactable() &&
                        GameObjectActiveCareParent(gameObject) &&
                        Custom())
                    {
                        validShould = true;
                    
                        DragonU3DSDK.Network.BI.BIManager.Instance.SendCommonMonetizationAdEvent(
                            BiEventCommon.Types.CommonMonetizationAdEventType.CommonMonetizationEventOfferwallAdShouldDisplay, placementId, 
                            BiEventCommon.Types.CommonMonetizationAdEventFailedReason.CommonMonetizationEventReasonAdNone);

                        AdLogicManager.Instance.ValidShouleTotalCnt++;
                    
                        DebugUtil.Log($"曝光检测 offerwall广告位：{placementId} 曝光一次");
                    }
                }
                else
                {
                    if (!Interactable() ||
                        !GameObjectActiveCareParent(gameObject) ||
                        !Custom())
                    {
                        validShould = false;
                    }
                }
            }
        }

        private bool Interactable()
        {
            return GetComponent<Button>().isActiveAndEnabled && GetComponent<Button>().IsInteractable();
        }

        private bool GameObjectActiveCareParent(GameObject obj)
        {
            bool activeSelf = obj.activeSelf;
            if (!activeSelf)
            {
                return false;
            }
            bool activeParent = true;
            if (null != obj.transform.parent)
            {
                activeParent = GameObjectActiveCareParent(obj.transform.parent.gameObject);
            }

            return activeParent;
        }

        private bool Custom()
        {
            if (null == customCheck)
            {
                return true;
            }

            return customCheck();
        }
    }
}