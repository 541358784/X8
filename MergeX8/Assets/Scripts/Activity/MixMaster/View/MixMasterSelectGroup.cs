
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class MixMasterSelectGroup : MonoBehaviour
{
    private int _index;
    private int _selectId;
    private Transform _item;
    private List<SelectItem> _selectItems;
    private Button sureBtn;
    public void Awake()
    {
        _item = transform.Find("Grid/1");
        _item.gameObject.SetActive(false);
        sureBtn = transform.Find("Button").GetComponent<Button>();
        sureBtn.onClick.AddListener(() =>
        {
            if (_selectId > 0)
            {
                MixMasterModel.Instance.AddSelect(_index,_selectId);
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.OPTIONAL_GIFT_SELECT,_index,_selectId);   
            }
            gameObject.SetActive(false);
        });
    }

    public void Init(int index,List<int> rewards,List<int> counts)
    {
        _index = index;
        _selectItems = new List<SelectItem>();
        for (int i = 0; i < rewards.Count; i++)
        {
            var item=Instantiate(_item, _item.parent);
            item.gameObject.SetActive(true);
            SelectItem selectItem = new SelectItem();
            selectItem._normal=item.Find("Normal");
            selectItem._normalReward=item.Find("Normal/Item");
            selectItem._select=item.Find("Selected");
            selectItem._select.gameObject.SetActive(false);
            selectItem._selectReward = item.Find("Selected/Item");
            selectItem._button = item.GetComponent<Button>();
            selectItem._index = i;
            InitItem(selectItem._normalReward, rewards[i], counts[i]);
            InitItem(selectItem._selectReward, rewards[i], counts[i]);
            _selectItems.Add(selectItem);
            selectItem._button.onClick.AddListener(() =>
            {
                _selectId = rewards[selectItem._index];
         
                for (int j = 0; j < _selectItems.Count; j++)
                {
                    _selectItems[j]._select.gameObject.SetActive(_selectItems[j]._index == selectItem._index);
                }
            });
        }
    }
    private void InitItem(Transform item,int itemID,int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID))
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
        bool isRes = UserData.Instance.IsResource(itemID);
        var infoBtn = item.transform.Find("TipsBtn");
        if (infoBtn != null)
        {
            infoBtn.gameObject.SetActive(!isRes);
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
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

    public class SelectItem
    {
        public Button _button;
        public int _index;
        public Transform _normal;
        public Transform _normalReward;
        public Transform _select;
        public Transform _selectReward;
    }
}
