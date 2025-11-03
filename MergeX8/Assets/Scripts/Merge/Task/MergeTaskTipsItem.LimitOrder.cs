using Activity.LimitTimeOrder;
using Activity.TimeOrder;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public partial class MergeTaskTipsItem
{
    private GameObject _limitOrder;
    public LocalizeTextMeshProUGUI _limitOrderText;
    public Image _limitOrderIcon;
    private LocalizeTextMeshProUGUI _rewardText;

    private GameObject _limitTimeOrderBg;
    
    private void AwakeLimitOrder()
    {
        _limitTimeOrderBg = transform.Find("BGLimitOrder").gameObject;
        
        _limitOrder = transform.Find("LimitOrder").gameObject;
        _limitOrderText = transform.Find("LimitOrder/SliderText").GetComponent<LocalizeTextMeshProUGUI>();
        _limitOrderIcon = transform.Find("LimitOrder/RewardIcon").GetComponent<Image>();
        
        _rewardText = transform.Find("LimitOrder/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        
        _limitOrder.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupLimitOrder);
        });
    }

    private void RefreshRepeatingLimitOrder()
    {
        if(storageTaskItem.Type != (int)MainOrderType.Limit)
            return;
        
        _cooldownText.SetText(LimitTimeOrderModel.Instance.GetJoinEndTimeString());
        
        LimitTimeOrderModel.Instance.CheckJoinEnd();
    }

    private void InitLimitOrder(StorageTaskItem storageItem)
    {
        _limitOrder.gameObject.SetActive(false);
        _limitTimeOrderBg.gameObject.SetActive(false);
        
        if(storageItem == null || storageItem.Type != (int)MainOrderType.Limit)
            return;
        
        var config = LimitTimeOrderModel.Instance.GetGroupConfig();
        if (config != null)
        {
            _limitOrderIcon.sprite = UserData.GetResourceIcon(config.RewardIds[0]);
            _rewardText.SetText("x"+config.RewardNums[0]);
        }
        
        _limitOrder.gameObject.SetActive(true);
        _timeCoolDown.gameObject.SetActive(true);
        _limitTimeOrderBg.gameObject.SetActive(true);
        
        RefreshLimitInfo();
        RefreshRepeatingLimitOrder();
    }

    private void RefreshLimitInfo()
    {
        _limitOrderText.SetText(LimitTimeOrderModel.Instance.GetOrderProgress());
    }
}