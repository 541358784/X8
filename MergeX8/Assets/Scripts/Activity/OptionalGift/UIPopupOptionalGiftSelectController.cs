
using System.Collections.Generic;
using DragonPlus;
using Gameplay;
using OptionalGift;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupOptionalGiftSelectController : UIWindowController
{
    private Button _buttonClose;
    private Button _sureBtn;
    private int _index;
    private int _selectId;
    private Transform _item;
    private List<SelectItem> _selectItems;
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/ButtonClose");
        _buttonClose.onClick.AddListener(OnBtnClose);
        _sureBtn = GetItem<Button>("Root/Button");
        _sureBtn.onClick.AddListener(OnBtnSure);
        _item = GetItem<Transform>("Root/Scroll View/Viewport/Content/1");
        _item.gameObject.SetActive(false);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _index = (int) objs[0];
        List<int> rewards = (List<int>) objs[1];
        List<int> counts = (List<int>) objs[2];
        _selectItems = new List<SelectItem>();
        for (int i = 0; i < rewards.Count; i++)
        {
            var item=Instantiate(_item, _item.parent);
            item.gameObject.SetActive(true);
            SelectItem selectItem = new SelectItem();
            selectItem._normal=item.Find("Normal");
            selectItem._normalReward=item.Find("Normal/Item");
            selectItem._select=item.Find("Select");
            selectItem._select.gameObject.SetActive(false);
            selectItem._selectReward = item.Find("Select/Item");
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
    private void OnBtnSure()
    {
        if (_selectId > 0)
        {
            OptionGiftModel.Instance.AddSelect(_index,_selectId);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.OPTIONAL_GIFT_SELECT,_index,_selectId);
        } 
        AnimCloseWindow();
    }

    private void OnBtnClose()
    {
        AnimCloseWindow();
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
