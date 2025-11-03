using Activity.FarmTimeOrder;
using DragonPlus;
using Farm.Order;
using UnityEngine;

namespace Farm.View
{
    public partial class OrderCell
    {
        private GameObject _normalBg;
        private GameObject _activityTimeOrderBg;

        private GameObject _activity_tiemGroup;
        private LocalizeTextMeshProUGUI _activity_time;
        
        private void Awake_TimeOrder()
        {
            _normalBg = transform.Find("BG").gameObject;
            _activityTimeOrderBg = transform.Find("BGTimeLimitOrder").gameObject;

            _activity_tiemGroup = transform.Find("TimeGroup").gameObject;
            _activity_time = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        }

        private void InitContentNotify()
        {
            if (_normalBg == null)
            {
                Awake_TimeOrder();
            }
            
            _normalBg?.gameObject.SetActive(_storage.Slot != (int)OrderSlot.Activity_TimeOrder);
            _activityTimeOrderBg?.gameObject.SetActive(_storage.Slot == (int)OrderSlot.Activity_TimeOrder);
        }

        private void InvokeUpdate()
        {
            _activity_tiemGroup.gameObject.SetActive(true);
            _activity_time.SetText(FarmTimeLimitOrderModel.Instance.GetJoinEndTimeString());
            
            
            FarmTimeLimitOrderModel.Instance.CheckJoinEnd();
        }
    }
}