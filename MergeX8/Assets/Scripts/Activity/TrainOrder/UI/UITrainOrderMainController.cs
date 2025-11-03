using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.TrainOrder
{
    public partial class UITrainOrderMainController : UIWindowController
    {
        private int _oldSelectIndex = -1;
        private int _selectGridIndex = -1;
        private int _selectGridId = -1;


        private float _sellColdTime = 0;


        private TableMergeItem curMergeItem = null;
        private StorageMergeItem boardItem = null;

        public static UITrainOrderMainController Instance;


        public TrainOrderMergeBoard MergeBoard;
        private Button _buttonClose;

        private Transform _orderSmallItem;
        private Transform _orderSmallItemRoot;

        private Transform _orderBigItem;
        private Transform _orderBigItemRoot;


        private Transform _tranOrderGroupRoot;

        private Slider _sliderProgress;
        private LocalizeTextMeshProUGUI _textProgress;


        private LocalizeTextMeshProUGUI _textTime;


        private Transform _tranSell;
        private Transform _tranNoSell;

        private Button _buttonSell;
        private LocalizeTextMeshProUGUI _textSell;


        private List<TrainOrderItem> _orderItems = new List<TrainOrderItem>();

        private List<TrainOrderGroupItem> _orderGroupItems = new List<TrainOrderGroupItem>();


        public override void PrivateAwake()
        {
            Instance = this;
            _buttonClose = GetItem<Button>("Root/ButtonClose");
            _buttonClose.onClick.AddListener(delegate { AnimCloseWindow(); });

            _orderSmallItemRoot = transform.Find("Root/Up");
            _orderSmallItem = transform.Find("Root/Up/Scroll View");
            _orderSmallItem.gameObject.SetActive(false);

            _orderBigItemRoot = transform.Find("Root/Down");
            _orderBigItem = transform.Find("Root/Down/Scroll View");
            _orderBigItem.gameObject.SetActive(false);


            _tranOrderGroupRoot = transform.Find("Root/Slider/Reward");

            _sliderProgress = transform.Find("Root/Slider").GetComponent<Slider>();

            _textProgress = transform.Find("Root/Progress/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _textTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();

            _tranSell = transform.Find("GameObject/Full");
            _tranNoSell = transform.Find("GameObject/Normal");
            _buttonSell = GetItem<Button>("GameObject/Full/SellBtn");
            _textSell = GetItem<LocalizeTextMeshProUGUI>("GameObject/Full/SellBtn/Text");
            _buttonSell.onClick.AddListener(OnClickSell);
        }

        protected override void OnCloseWindow(bool destroy = false)
        {
            base.OnCloseWindow(destroy);
            MergeBoard.StopAllTweenAnim();
            MergeManager.Instance.Refresh((int)MergeBoardEnum.Main);
            CancelInvoke(nameof(UpdateTime));
            // CurrencyGroupManager.Instance?.currencyController?.RecoverCanvasSortOrder();
            // CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(true);
            EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BORAD_SELECTED_GRID, OnSelectGrid);
            // EventDispatcher.Instance.RemoveEventListener(EventEnum.REWARD_POPUP, RefreshOrder);
            // EventDispatcher.Instance.RemoveEventListener(EventEnum.NOTICE_POPUP, RefreshOrder);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            if (TrainOrderModel.Instance.Storage.NeedDelayRefreshOrder)
            {
                TrainOrderModel.Instance.RefreshOrder();
                TrainOrderModel.Instance.Storage.NeedDelayRefreshOrder = false;
            }

            MergeManager.Instance.Refresh(MergeBoardEnum.TrainOrder);
            MergeBoard = transform.Find("Root").GetComponentDefault<TrainOrderMergeBoard>("Board");
            MergeBoard.activeIndex = 20;
            RefreshView();

            InvokeRepeating(nameof(UpdateTime), 0, 1);
            
            EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_SELECTED_GRID, OnSelectGrid);
            // EventDispatcher.Instance.AddEventListener(EventEnum.REWARD_POPUP, RefreshOrder);
            // EventDispatcher.Instance.AddEventListener(EventEnum.NOTICE_POPUP, RefreshOrder);
            //
            // RefreshOrder(null);
            InitEnergyTorrentBtn();
        }


        void RefreshOrder(BaseEvent e)
        {
            CurrencyGroupManager.Instance?.currencyController?.SetCanvasSortOrder(canvas.sortingOrder + 1);
            CurrencyGroupManager.Instance?.currencyController?.SetAddButtonsActive(false);
        }
        
        void OnSelectGrid(BaseEvent e)
        {
            if (e == null || e.datas == null || e.datas.Length == 0)
                return;
            if ((MergeBoardEnum)e.datas[1] != MergeBoardEnum.TrainOrder)
                return;
            Vector2Int arg = (Vector2Int)e.datas[0];
            OnSelectGrid(arg);
        }

        //0 index
        //1 id
        void OnSelectGrid(Vector2Int arg)
        {
            int index = arg[0];
            int id = arg[1];

            _oldSelectIndex = _selectGridIndex;
            _selectGridIndex = index;
            _selectGridId = id;
            MergeMainController.Instance.selcedItemId = _selectGridId;

            curMergeItem = GameConfigManager.Instance.GetItemConfig(_selectGridId);
            boardItem = MergeManager.Instance.GetBoardItem(_selectGridIndex, MergeBoardEnum.TrainOrder);

            bool isExist = MergeManager.Instance.IsBoardItemExist(_selectGridIndex, MergeBoardEnum.TrainOrder);
            if (curMergeItem == null || !isExist || (boardItem != null && boardItem.State == (int)MergeItemStatus.box))
            {
                _tranSell.gameObject.SetActive(false);
                _tranNoSell.gameObject.SetActive(true);
                return;
            }

            if (curMergeItem.sold_gold >= 0 && !TrainOrderModel.Instance.CurLevel.BuildId.Contains(_selectGridId))
            {
                _tranSell.gameObject.SetActive(true);
                _tranNoSell.gameObject.SetActive(false);
                _textSell.SetText(curMergeItem.sold_gold.ToString());
            }
            else
            {
                _tranSell.gameObject.SetActive(false);
                _tranNoSell.gameObject.SetActive(true);
            }
        }


        private void UpdateTime()
        {
            _textTime.SetText(TrainOrderModel.Instance.GetActivityLeftTimeString());
        }

        public void RefreshView()
        {
            TryRefreshLevelBuild();
            InitOrderItem(TrainOrderModel.Instance.CurOrders);
            InitOrderGroupItem(TrainOrderModel.Instance.CurGroups);

            RefreshProgress(0f);

            OnSelectGrid(Vector2Int.zero);
        }


        public void InitOrderItem(List<TrainOrderOrder> orders, bool fromOutSide = false)
        {
            Transform tran = Instantiate(_orderSmallItem, _orderSmallItemRoot);
            tran.gameObject.SetActive(true);
            TrainOrderItem item = tran.GetComponentDefault<TrainOrderItem>();
            item.Init(orders[0], fromOutSide);


            tran = Instantiate(_orderBigItem, _orderBigItemRoot);
            tran.gameObject.SetActive(true);
            item = tran.GetComponentDefault<TrainOrderItem>();
            item.Init(orders[1], fromOutSide);
        }

        private bool _initGroup = false;

        private void InitOrderGroupItem(List<TrainOrderOrderGroup> groups)
        {
            if (_initGroup)
            {
                for (var i = 0; i < groups.Count; i++)
                {
                    _orderGroupItems[i].Init(groups[i]);
                }
            }
            else
            {
                for (var i = 0; i < groups.Count; i++)
                {
                    Transform tran = _tranOrderGroupRoot.Find((i + 1).ToString());
                    if (tran != null)
                    {
                        TrainOrderGroupItem item = tran.GetComponentDefault<TrainOrderGroupItem>();
                        item.Init(groups[i]);
                        _orderGroupItems.Add(item);
                    }
                }

                _initGroup = true;
            }
        }

        /// <summary>
        /// 尝试刷新当前关卡建筑
        /// </summary>
        private void TryRefreshLevelBuild()
        {
            if (!TrainOrderModel.Instance.Storage.IsInitLevelBuild)
            {
                for (var i = 0; i < TrainOrderModel.Instance.CurLevel.BuildId.Count; i++)
                {
                    int buildId = TrainOrderModel.Instance.CurLevel.BuildId[i];
                    int pos = TrainOrderModel.Instance.BuildPos[i];
                    MergeManager.Instance.SetNewBoardItem(pos, buildId, 1, RefreshItemSource.rewards,
                        MergeBoardEnum.TrainOrder);
                }

                TrainOrderModel.Instance.Storage.IsInitLevelBuild = true;
            }
        }

        public void RefreshProgress(float duration = 0.3f)
        {
            _sliderProgress.DOValue(TrainOrderModel.Instance.Storage.Progress, duration);
            _textProgress.SetText(TrainOrderModel.Instance.Storage.Progress + "/" + TrainOrderModel.MAX_PROGRESS);
        }


        public TrainOrderGroupItem GetGroupItem(int index)
        {
            return _orderGroupItems[index];
        }

        private void OnClickSell()
        {
            if (Time.time - _sellColdTime < 0.8f)
                return;
            if (!MergeManager.Instance.IsBoardItemExist(_selectGridIndex, MergeBoardEnum.TrainOrder))
                return;
            var itemConfig = GameConfigManager.Instance.GetItemConfig(_selectGridId);
            bool isOpen = MergeManager.Instance.IsOpen(_selectGridIndex, MergeBoardEnum.TrainOrder);
            if (!isOpen)
                return;
            if (itemConfig.sold_confirm)
            {
                var warningView =
                    UIManager.Instance.OpenUI(UINameConst.UIPopupMergeWarning) as UIPopupMergeWarningController;
                warningView.InitPackageUnit(_selectGridId);
                warningView.OnSellItem = SellItem;
            }
            else
            {
                SellItem();
            }
        }

        private void SellItem()
        {
            int sellIndex = _selectGridIndex;
            OnSellItem(sellIndex);
            _sellColdTime = Time.time;
            var itemConfig = GameConfigManager.Instance.GetItemConfig(_selectGridId);
            if (itemConfig.sold_gold > 0)
            {
                AudioManager.Instance.PlaySound(19);
            }
            else
            {
                AudioManager.Instance.PlaySound(170);
            }

            SendSellBi(_selectGridId);
            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BORAD_SELL_ITEM,
                MergeBoardEnum.TrainOrder, _selectGridIndex);
            MergeManager.Instance.RemoveBoardItem(_selectGridIndex, MergeBoardEnum.TrainOrder, "Sell");
            OnSelectGrid(Vector2Int.zero);
        }
        
        private void OnSellItem(int index)
        {
            var grid = MergeBoard.GetGridByIndex(index);
            GameObject clone = grid.board.CloneIconGameObject();
            clone.transform.SetParent(grid.board.transform.parent, false);
            clone.transform.transform.position = grid.board.GetIconPosition();
            Destroy(clone, 0.2f);
        }

        private void SendSellBi(int id)
        {
            var config = GameConfigManager.Instance.GetItemConfig(id);
            if (config == null)
                return;
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemChangeSaleItem,
                itemAId = config.id,
                ItemALevel = config.level,
                isChange = true,
                extras = new Dictionary<string, string>
                {
                    { "coin", config.sold_gold.ToString() },
                },
                data1 = "0",
            });
        }


        public static void Open()
        {
            UIManager.Instance.OpenWindow(UINameConst.UITrainOrderMain);
        }
    }
}