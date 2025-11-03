using System;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIGiftBagRewardCell : MonoBehaviour
{
    private Image iconImage;
    private LocalizeTextMeshProUGUI lockNumText;
    private LocalizeTextMeshProUGUI normalNumText;
    private Button _infoButton;

    private TableMergeItem _tableMerge;
    private void Awake()
    {
        iconImage = transform.Find("Icon").GetComponent<Image>();
        lockNumText = transform.Find("BlueText").GetComponent<LocalizeTextMeshProUGUI>();
        normalNumText = transform.Find("YellowText").GetComponent<LocalizeTextMeshProUGUI>();
        _infoButton = transform.Find("TipsBtn").GetComponent<Button>();
        _infoButton.onClick.AddListener(() =>
        {
            MergeInfoView.Instance.OpenMergeInfo(_tableMerge, isShowGetResource:false,_isShowProbability:true);
        });
        
        _infoButton.gameObject.SetActive(false);
    }

    public void InitData(int itemId, int num)
    {
        lockNumText.SetText("x" + num.ToString());
        normalNumText.SetText("x" + num.ToString());

        string iconName = UserData.GetResourceIconName(itemId, UserData.ResourceSubType.Reward);
        if (!UserData.Instance.IsResource(itemId))
        {
            _tableMerge = GameConfigManager.Instance.GetItemConfig(itemId);
            if (_tableMerge != null)
                iconName = _tableMerge.image;
            
            _infoButton.gameObject.SetActive(true);
        }
        else
        {
            _infoButton.gameObject.SetActive(false);
        }

        if (iconImage.sprite == null || iconImage.sprite.name != iconName)
        {
            if (UserData.Instance.IsResource(itemId))
                iconImage.sprite = UserData.GetResourceIcon(iconName);
            else
                iconImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(iconName);
        }
    }

    public void UpdateStatus(bool isUnLock)
    {
        lockNumText.gameObject.SetActive(!isUnLock);
        normalNumText.gameObject.SetActive(isUnLock);
    }
}