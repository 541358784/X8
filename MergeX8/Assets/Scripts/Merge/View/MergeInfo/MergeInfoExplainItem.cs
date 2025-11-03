using System;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;


public class MergeInfoExplainItem : MonoBehaviour
{
    private GameObject _normalObj;
    private GameObject _currentObj;
    private GameObject _questionObj;
    private Image _imageIcon;
    private GameObject _arrowObj;
    private Button _button;
    private TableMergeItem _itemConfig;
    private int _parentId;
    private bool _isUnLock;
    
    private void Awake()
    {
        _normalObj = transform.Find("Normal").gameObject;
        _currentObj = transform.Find("Current").gameObject;
        _questionObj = transform.Find("QuestionMark").gameObject;
        _arrowObj = transform.Find("Arrow").gameObject;

        _imageIcon = transform.Find("Icon").GetComponent<Image>();

        _button = transform.GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
        
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, MergeInfoProductEvent);
    }

    public void SetItemInfomation(TableMergeItem itemConfig, TableMergeItem parentConfig)
    {
        _itemConfig = itemConfig;

        _imageIcon.sprite = UserData.GetResourceIcon(itemConfig.id);

        _isUnLock = true;//MergeManager.Instance.IsShowedItemId(_itemConfig.id);
        
        UpdateSelectedStatus(parentConfig);
    }

    private void OnClick()
    {
        if(!_isUnLock)
            return;
        
        EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, _itemConfig);
    }
    
    private void MergeInfoProductEvent(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length == 0)
            return;

        TableMergeItem config = (TableMergeItem) e.datas[0];
        UpdateSelectedStatus(config);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, MergeInfoProductEvent);
    }

    private void UpdateSelectedStatus(TableMergeItem selectConfig)
    {
        bool isSelect = _itemConfig == selectConfig;
        
        _currentObj.gameObject.SetActive(isSelect);
        _arrowObj.gameObject.SetActive(isSelect);
        _normalObj.gameObject.SetActive(!isSelect);

        if (!isSelect)
        {
            _questionObj.gameObject.SetActive(!_isUnLock);
            _imageIcon.gameObject.SetActive(_isUnLock);
        }
    }
}