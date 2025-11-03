using Activity.TimeOrder;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public partial class MergeTaskTipsItem
{
    private GameObject _timeOrder;
    public LocalizeTextMeshProUGUI _timeOrderText;
    public Image _timeOrderIcon;

    private GameObject _timeOrderBg;
    
    private void AwakeTimeOrder()
    {
        _timeOrderBg = transform.Find("BGLimitedTimeTask").gameObject;
        
        _timeOrder = transform.Find("StartActivity/LimitedTimeTask").gameObject;
        _timeOrderText = _timeOrder.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _timeOrderIcon = _timeOrder.transform.Find("Icon").GetComponent<Image>();
    }

    private void RefreshRepeatingTimeOrder()
    {
        if(storageTaskItem.Type != (int)MainOrderType.Time)
            return;
        
        _cooldownText.SetText(TimeOrderModel.Instance.GetJoinEndTimeString());
        
        TimeOrderModel.Instance.CheckJoinEnd();
    }

    private void InitTimeOrder(StorageTaskItem storageItem)
    {
        _timeOrder.gameObject.SetActive(false);
        _timeOrderBg.gameObject.SetActive(false);
        
        if(storageItem == null || storageItem.Type != (int)MainOrderType.Time)
            return;
        
        _timeOrder.gameObject.SetActive(true);
        _timeCoolDown.gameObject.SetActive(true);
        _timeOrderBg.gameObject.SetActive(true);
        
        _timeOrderText.SetText(storageTaskItem.ExtraRewardNums[0].ToString());;
        _timeOrderIcon.sprite = UserData.GetResourceIcon(storageTaskItem.ExtraRewardTypes[0]);
        RefreshRepeatingTimeOrder();
    }
}