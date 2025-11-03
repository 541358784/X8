using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UILobbyMain")]
    public class UILobbyView : UIView
    {
        // public override UIViewLayer ViewLayer => UIViewLayer.Lobby;

        [ComponentBinder("TopPanelHome")] private Transform topPanelHome;
        [ComponentBinder("UINavigationBar")] private Transform navigationBar;
        [ComponentBinder("InputController")] private Transform inputController;

        [ComponentBinder("ScrollView/Viewport/Root/UIShopMain")]
        private Transform shop;

        [ComponentBinder("ScrollView/Viewport/Root/UILivesBank")]
        private Transform livesBank;

        [ComponentBinder("ScrollView/Viewport/Root/Main")]
        private Transform main;

        [ComponentBinder("ScrollView/Viewport/Root/UIGuildMain")]
        private Transform guild;

        [ComponentBinder("ScrollView/Viewport/Root/UIRankingList")]
        private Transform rank;

        public UILobbyMainView MainView;
        UILobbyNavigationBarView LobbyNavigationBarView;

        public static Transform CurrencyGroup;

        // private bool rootGridLayoutAdapt;
        // private int viewCount;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            // rootGridLayout.cellSize = UIRoot.Instance.mRootCanvas.GetComponent<RectTransform>().sizeDelta;
            Show();

            AddChildView<UILobbyTopView>(topPanelHome.gameObject);
            // LobbyNavigationBarView = AddChildView<UILobbyNavigationBarView>(navigationBar.gameObject, new NavigationBarParam() { offsetRoot = rootGridLayout.transform });
            MainView = AddChildView<UILobbyMainView>(main.gameObject);
            // AddChildView<UiTeamMainController>(guild.gameObject);
            // AddChildView<UiLeaderBoardMainView>(rank.gameObject);
            // AddChildView<UILivesBankMainView>(livesBank.gameObject);
            AddChildView<LobbyShopView>(shop.gameObject, new ShopBaseViewParam() { onlyPopUp = false });
            // viewCount = rootGridLayout.transform.childCount;

            // InputProxy inputProxy=  inputController.GetOrCreateComponent<InputProxy>();
            // inputProxy.onCheckDrag = (data) =>
            // {
            //     var delta = data.delta;
            //     return Mathf.Abs(delta.x) > Mathf.Abs(1.2f * delta.y);
            // };

            CurrencyGroup = transform.Find("TopPanelHome/Root/CurrencyGroup");

            // inputProxy.onBeginDrag = (data) =>
            // {
            //     ExecuteEvents.ExecuteHierarchy(rootGridLayout.gameObject, data, ExecuteEvents.beginDragHandler);
            // };
            //
            // inputProxy.onDrag = (data) =>
            // {
            //     ExecuteEvents.ExecuteHierarchy(rootGridLayout.gameObject, data, ExecuteEvents.dragHandler);
            // };
            //
            // inputProxy.onEndDrag = (data) =>
            // {
            //     ExecuteEvents.ExecuteHierarchy(rootGridLayout.gameObject, data, ExecuteEvents.endDragHandler);
            // };

            // ScrollRect scrollRect = rootGridLayout.GetComponentInParent<ScrollRect>();
            // if (!scrollRect.GetComponent<EventTrigger>())
            // {
            //     EventTrigger eventTrigger = scrollRect.AddComponent<EventTrigger>();
            //     EventTrigger.Entry entry = new EventTrigger.Entry();
            //     entry.eventID = EventTriggerType.EndDrag;
            //     entry.callback.AddListener((data) => MoveNearIndex());
            //     eventTrigger.triggers.Add(entry);
            // }
        }

        float GetIndex()
        {
            // Vector3 rootPos = (rootGridLayout.transform as RectTransform).anchoredPosition3D;
            // return (viewCount - 1) * 0.5f - rootPos.x / rootGridLayout.cellSize.x;
            return 0;
        }

        int GetNearIndex()
        {
            // int lastIndex = LobbyNavigationBarView.lastNavigationIndex;
            // float index = GetIndex();
            // int newIndex = Mathf.RoundToInt(index);
            //
            // float direction = Mathf.Sign(index - lastIndex);
            // float offset = index - newIndex;
            // if (direction * offset > 0.15f)
            // {
            //     newIndex += Mathf.RoundToInt(direction);
            // }
            //
            // newIndex = Mathf.Clamp(newIndex, 0, viewCount - 1);
            // return newIndex;
            return 0;
        }

        void MoveNearIndex()
        {
            LobbyNavigationBarView.NavigationBarOnClick((UILobbyNavigationBarView.UIType)GetNearIndex());
        }

        public void Hide()
        {
            RectTransform rectTransform = transform.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, -10000.0f);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -10000.0f);
        }

        public void Show()
        {
            RectTransform rectTransform = transform.GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, CommonUtils.GetSafeAreaOffset());
            AdaptRootGridLayout();

            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.LobbyRefreshShow);
        }

        private async void AdaptRootGridLayout()
        {
            // if (rootGridLayoutAdapt) return;
            // await Task.Yield();
            // rootGridLayout.cellSize += new Vector2(0.0f, CommonUtils.GetSafeAreaOffset());
            // rootGridLayoutAdapt = true;
        }

        protected override void OnBackButtonCallBack()
        {
            CommonUtils.PopExitGame();
        }
    }
}