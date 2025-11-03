
using System;
using DragonPlus;
using DragonU3DSDK;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class GarageCleanupRewardItem : MonoBehaviour
{
    private Image _icon;
    private Button _tipButton;
    private TableMergeItem _itemConfig;
    private LocalizeTextMeshProUGUI _rewardCount;
    private void Awake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _rewardCount = transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
        _tipButton = transform.Find("TipsBtn").GetComponent<Button>();
    }
    
    public void SetData( int itemId,int count)
    {
        if (UserData.Instance.IsResource(itemId))
        {
            _icon.sprite=UserData.GetResourceIcon(itemId, UserData.ResourceSubType.Big);
            _tipButton.gameObject.SetActive(false);
        }
        else
        {
            _itemConfig = GameConfigManager.Instance.GetItemConfig(itemId);
            if (_itemConfig==null)
            {
                DebugUtil.LogError("Item 错误 "+itemId);
            }
            _icon.sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(_itemConfig.image);
            _tipButton.onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(_itemConfig);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            });
        }
        _rewardCount.SetText("x"+count);
        
      
    }
}
