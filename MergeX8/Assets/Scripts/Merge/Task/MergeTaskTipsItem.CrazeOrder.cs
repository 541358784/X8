using Activity.CrazeOrder.Model;
using Activity.TimeOrder;
using DragonPlus;
using DragonPlus.Config.CrazeOrder;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public partial class MergeTaskTipsItem
{
     private GameObject _crazeOrder;
    public LocalizeTextMeshProUGUI _crazeOrderText;
    public LocalizeTextMeshProUGUI _crazeOrderNumText;
    public Image _crazeOrderIcon;
    public Image _currentCrazeOrderIcon;
    private LocalizeTextMeshProUGUI _crazeText;

    private GameObject _crazeOrderBg;
    
    private void AwakeCrazeOrder()
    {
        _crazeOrderBg = transform.Find("BGCrazeOrder").gameObject;
        
        _crazeOrder = transform.Find("CrazeOrder").gameObject;
        _crazeOrderText = transform.Find("CrazeOrder/SliderText").GetComponent<LocalizeTextMeshProUGUI>();
        _crazeOrderIcon = transform.Find("CrazeOrder/RewardIcon").GetComponent<Image>();
        _currentCrazeOrderIcon= transform.Find("CrazeOrder/CurrentRewardIcon").GetComponent<Image>();
        
        _crazeOrderNumText = transform.Find("CrazeOrder/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        
        _crazeOrder.GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UICrazeOrderMain);
        });
    }

    private void RefreshRepeatingCrazeOrder()
    {
        if(storageTaskItem.Type != (int)MainOrderType.Craze)
            return;
        
        _cooldownText.SetText(CrazeOrderModel.Instance.GetJoinEndTimeString());
        
        CrazeOrderModel.Instance.CheckJoinEnd();
    }

    private void InitCrazeOrder(StorageTaskItem storageItem)
    {
        _crazeOrder.gameObject.SetActive(false);
        _crazeOrderBg.gameObject.SetActive(false);
        
        if(storageItem == null || storageItem.Type != (int)MainOrderType.Craze)
            return;
        
        var config = CrazeOrderModel.Instance.GetStageConfig();
        if (config != null)
        {
            _currentCrazeOrderIcon.sprite = UserData.GetResourceIcon(config.RewardIds[0]);
        }

        _crazeOrderIcon.sprite = UserData.GetResourceIcon(CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].RewardIds[0]);
        _crazeOrderNumText.SetText("x"+CrazeOrderConfigManager.Instance.CrazeOrderSettingList[0].RewardNums[0]);
        
        _crazeOrder.gameObject.SetActive(true);
        _timeCoolDown.gameObject.SetActive(true);
        _crazeOrderBg.gameObject.SetActive(true);
        
        RefreshCrazeInfo();
        RefreshRepeatingCrazeOrder();
    }

    private void RefreshCrazeInfo()
    {
        _crazeOrderText.SetText(CrazeOrderModel.Instance.GetOrderProgress());
    }
}