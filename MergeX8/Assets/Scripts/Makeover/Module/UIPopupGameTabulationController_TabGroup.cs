using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGameTabulationController
{
    private Dictionary<int, TabGroup> TabGroupDic = new Dictionary<int, TabGroup>();
    private int _showTabIndex;
    public int ShowTabIndex
    {
        get
        {
            return _showTabIndex;
        }
        set
        {
            if (value != _showTabIndex)
            {
                if (TabGroupDic.TryGetValue(_showTabIndex, out var lastShowTab))
                {
                    lastShowTab.Hide();
                }
                _showTabIndex = value; 
                if (TabGroupDic.TryGetValue(_showTabIndex, out var curShowTab))
                {
                    curShowTab.Show();
                }
            }
        }
    }
    public void InitView()
    {
        var tab1 = transform.Find("Root/Scroll View").gameObject.AddComponent<TabGroup>();
        tab1.InitView(this,1);
        TabGroupDic.Add(1,tab1);
        var tab2 = transform.Find("Root/Scroll View2").gameObject.AddComponent<TabGroup>();
        tab2.InitView(this,2);
        TabGroupDic.Add(2,tab2);
        ShowTabIndex = 1;
    }
    public class TabGroup : MonoBehaviour
    {
        private UIPopupGameTabulationController MainUI;
        private Button TabButton;
        private GameObject _scrollView;
        private GameObject _item;
        private GameObject _rewardItem;
        private GameObject _content;

        private void Awake()
        {
            _scrollView = transform.gameObject;
            _item = transform.Find("Viewport/Content/Level").gameObject;
            _item.gameObject.SetActive(false);
            _rewardItem = transform.Find("Viewport/Content/Reward").gameObject;
            _rewardItem.gameObject.SetActive(false);
            _content = transform.Find("Viewport/Content").gameObject;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            TabButton.transform.Find("Normal").gameObject.SetActive(true);
            TabButton.transform.Find("Select").gameObject.SetActive(false);
            TabButton.transform.Find("Lock").gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            TabButton.transform.Find("Normal").gameObject.SetActive(false);
            TabButton.transform.Find("Select").gameObject.SetActive(true);
            TabButton.transform.Find("Lock").gameObject.SetActive(false);
            XUtility.WaitFrames(1, UpdateContentPosition);
        }
        private List<TableMiniGameGroup> TableMiniGameGroupList => MainUI.TableMiniGameGroupList;
        private Dictionary<int, TableMiniGameItem> TableMiniGameItemList => MainUI.TableMiniGameItemList;
        private List<UITabulationItemDigTrench> _itemsDigTrench => MainUI._itemsDigTrench;
        private List<UITabulationItemFishEatFish> _itemsFishEatFish => MainUI._itemsFishEatFish;
        private List<UITabulationItemOnePath> _itemsOnePath => MainUI._itemsOnePath;
        private List<UITabulationItemConnectLine> _itemsConnectLine => MainUI._itemsConnectLine;
        
        private List<UITabulationItemStimulate> _itemsStimulate => MainUI._itemsStimulate;
        private List<UITabulationItem> _items = new List<UITabulationItem>();
        public int TabPageIndex;
        public void InitView(UIPopupGameTabulationController mainUI,int pageId)
        {
            TabButton = mainUI.transform.Find("Root/Choose/" + pageId).GetComponent<Button>();
            if (TabButton)
            {
                TabButton.onClick.RemoveAllListeners();
                TabButton.onClick.AddListener(OnClickTabButton);
            }
            TabPageIndex = pageId;
            MainUI = mainUI;
            int index = 0;
            for (var i = 0; i < TableMiniGameGroupList.Count; i++)
            {
                var group = TableMiniGameGroupList[i];
                if (group.pageIndex != TabPageIndex)
                    continue;
                var itemConfigList = new List<TableMiniGameItem>();
                for (var i1 = 0; i1 < group.itemList.Length; i1++)
                {
                    var item = TableMiniGameItemList[group.itemList[i1]];
                    var i2 = 0;
                    for (i2 = 0; i2 < itemConfigList.Count; i2++)
                    {
                        if (!CompareOrderUnder(item, itemConfigList[i2]))
                        {
                            break;
                        }
                    }

                    itemConfigList.Insert(i2, item);
                }

                var startIndex = index;
                for (var i1 = 0; i1 < itemConfigList.Count; i1++)
                {
                    var item = itemConfigList[i1];
                    var cloneObj = Instantiate(_item, _content.transform);
                    cloneObj.gameObject.SetActive(true);
                    var typeTab = (MiniGameTypeTab) item.configType;

                    UITabulationItem itemData = null;
                    if (typeTab == MiniGameTypeTab.DigTrench)
                    {
                        itemData = new UITabulationItemDigTrench();
                        _itemsDigTrench.Add(itemData as UITabulationItemDigTrench);
                    }
                    else if (typeTab == MiniGameTypeTab.FishEatFish)
                    {
                        itemData = new UITabulationItemFishEatFish();
                        _itemsFishEatFish.Add(itemData as UITabulationItemFishEatFish);
                    }
                    else if (typeTab == MiniGameTypeTab.OnePath)
                    {
                        itemData = new UITabulationItemOnePath();
                        _itemsOnePath.Add(itemData as UITabulationItemOnePath);
                    }
                    else if (typeTab == MiniGameTypeTab.ConnectLine)
                    {
                        itemData = new UITabulationItemConnectLine();
                        _itemsConnectLine.Add(itemData as UITabulationItemConnectLine);
                    }
                    else if (typeTab == MiniGameTypeTab.Stimulate)
                    {
                        itemData = new UITabulationItemStimulate();
                        _itemsStimulate.Add(itemData as UITabulationItemStimulate);
                    }

                    itemData.Init(cloneObj, item.configId, index++);
                    _items.Add(itemData);
                }

                {
                    var rewardObj = Instantiate(_rewardItem, _content.transform);
                    rewardObj.gameObject.SetActive(true);
                    RewardItem rewardItem = new RewardItem();
                    rewardItem.Init(rewardObj, group, startIndex, index);
                }
            }

            {
                //结尾加一个comingSoon
                var comingObj = Instantiate(_item, _content.transform);
                comingObj.gameObject.SetActive(true);
                UITabulationItemDigTrench item = new UITabulationItemDigTrench();
                item.Init(comingObj, -1, index++);
            }
            Hide();
        }

        public void OnClickTabButton()
        {
            MainUI.ShowTabIndex = TabPageIndex;
        }
        public void UpdateContentPosition()
        {
        
            GameObject firstItem = _items[0]._gameObject;
            for (var i = 0; i < _items.Count; i++)
            {
                if (!_items[i].IsFinish())
                {
                    if (i > 0)
                        firstItem = _items[i-1]._gameObject;
                    break;
                }
            }
            if (firstItem == null)
                return;

            _scrollView.GetComponent<ScrollRect>().enabled = false;
        
            RectTransform rectContent = (RectTransform)_content.transform;
            float maxHeight = rectContent.rect.height;
            float moveHeight = -firstItem.transform.localPosition.y - ((RectTransform)(firstItem.transform)).rect.height/2;
            moveHeight = Mathf.Min(maxHeight, moveHeight);
        
            rectContent.DOAnchorPosY(moveHeight, 0f);
            XUtility.WaitFrames(1, () =>
            {
                _scrollView.GetComponent<ScrollRect>().enabled = true; 
            });
        }
    }
}