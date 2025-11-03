
using System;
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using Screw;
using Screw.GameLogic;
using UnityEngine;
using UnityEngine.UI;

public class KapiScrewOptionalGiftSelectItem : MonoBehaviour
{
    private Button _addButton;
    private Button _replaceButton;
    private Transform _rewardItem;
    private void Awake()
    {
        _addButton = transform.Find("Add").GetComponent<Button>();
        _addButton.onClick.AddListener(OnBtnSelect);
        _replaceButton = transform.Find("Replace").GetComponent<Button>();
        _replaceButton.onClick.AddListener(OnBtnSelect);
        _rewardItem = transform.Find("Item");
        EventDispatcher.Instance.AddEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);

    }

    private void OnSelect(BaseEvent obj)
    {
        int index =(int) obj.datas[0];
        int select =(int) obj.datas[1];
        if (index == _index)
        {
            InitItem(_rewardItem,select,count[item.IndexOf(select)]);
            _rewardItem.gameObject.SetActive(true);
            _addButton.gameObject.SetActive(false);
            _replaceButton.gameObject.SetActive(true);
        }
    }

    int type = 1;
    List<int> item=null;
    List<int> count=null;
    private int _index = 0;
    public void Init(int index ,KapiScrewGiftBagConfig config)
    {
        _index = index;
        switch (index)
        {
            case 0:
                type = config.Type1;
                item = config.Item1;
                count = config.Count1;
                break;
            case 1:
                type = config.Type2;
                item = config.Item2;
                count = config.Count2;
                break;
            case 2:
                type = config.Type3;
                item = config.Item3;
                count = config.Count3;
                break;
        }

        if (type == 1)
        {
            InitItem(_rewardItem,item[0],count[0]);
            _addButton.gameObject.SetActive(false);
            _replaceButton.gameObject.SetActive(false);
        }
        else
        {
            int select= KapiScrewModel.Instance.GetSelect(index);
            if (select > 0)
            {
                InitItem(_rewardItem,select,count[item.IndexOf(select)]);
                _addButton.gameObject.SetActive(false);
                _replaceButton.gameObject.SetActive(true);
          
            }
            else
            {
                _rewardItem.gameObject.SetActive(false);
                _addButton.gameObject.SetActive(true);
                _replaceButton.gameObject.SetActive(false);
            }
        }
    }
    
    private void OnBtnSelect()
    {
        UIPopupKapiScrewOptionalGiftSelectController.Open(_index,item,count);
    }

    private void InitItem(Transform item,int itemID,int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID) || ScrewGameModel.Instance.IsScrewResId(itemID))
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID,UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else
            {
                Debug.LogError("Get MergeItemConfig---null " + itemID);
            }
        }

        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
        bool isRes = UserData.Instance.IsResource(itemID) || ScrewGameModel.Instance.IsScrewResId(itemID);
        var infoBtn = item.transform.Find("TipsBtn");
        if (infoBtn != null)
        {
            infoBtn.gameObject.SetActive(!isRes);
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemID, null,_isShowProbability:true);
                    UIPopupMergeInformationController controller =
                        UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                    if (controller != null)
                        controller.SetResourcesActive(false);
                });
            }
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.OPTIONAL_GIFT_SELECT, OnSelect);
    }
}
