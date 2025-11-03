using System;
using UnityEngine;
using UnityEngine.UI;

public class OneselfChoseItem : MonoBehaviour
{
    public enum ItemStatus
    {
        Select,
        UnSelect
    }
    private Transform _normal;
    private Transform _select;
    private Image _icon;
    private Button _buttonTip;
    private ItemStatus _currentStatus = ItemStatus.UnSelect;
    private Button _button;
    private int _index;
    private TableMergeItem _mergeItem;
    private Action<int, TableMergeItem> _selectAction;
    
    private void Awake()
    {
        _normal = transform.Find("Normal");
        _select = transform.Find("Select");
        _icon = transform.Find("Icon").GetComponent<Image>();
        _buttonTip = transform.Find("ButtonTip").GetComponent<Button>();
        _button = gameObject.GetComponent<Button>();
        _button.onClick.AddListener(OnSelect);
        SetStatus(ItemStatus.UnSelect);
    }
    public void Init(TableMergeItem mergeItem, Action<int, TableMergeItem> selectAction,int index)
    {
        _mergeItem = mergeItem;
        _selectAction = selectAction;
        _index = index;
        _icon.sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(mergeItem.image);
        _buttonTip.onClick.AddListener(() =>
        {
            MergeInfoView.Instance.OpenMergeInfo(_mergeItem);
            UIPopupMergeInformationController controller =
                UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
            if (controller != null)
                controller.SetResourcesActive(false);
        });

    }
    private void OnSelect()
    {
        if (_currentStatus == ItemStatus.Select)
            return;
        
        _selectAction?.Invoke(_index,_mergeItem);
    }

    public void SetStatus(ItemStatus status)
    {
        _currentStatus = status;
        _select.gameObject.SetActive(status==ItemStatus.Select);
    }
    
}
