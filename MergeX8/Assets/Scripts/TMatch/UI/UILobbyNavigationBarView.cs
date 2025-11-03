using System.Net.Mime;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace TMatch
{
    public class NavigationBarParam : UIViewParam
    {
        public Transform offsetRoot;
    }

    public class UILobbyNavigationBarView : UIView
    {
        private static Transform topView;

        public static Transform GetMainTopView()
        {
            return topView;
        }

        public enum UIType
        {
            Shop,
            Energy,
            Main,
            Team,
            Rank,
        }

        [ComponentBinder("Root")] private Transform root;
        [ComponentBinder("BGSelect")] private RectTransform BGSelect;
        [ComponentBinder("Root/Energy/Tag")] private Transform livesBankRedTag;
        [ComponentBinder("Root/Team/Tag")] private Transform teamRedTag;
        [ComponentBinder("Root/Main/BG")] public Transform main;

        private const float unselectWidth = 125.0f;
        private const float selectWidth = 256.0f;
        private const float gapSize = 3.0f;
        private UIType lastNavigationType = UIType.Main;

        public int lastNavigationIndex
        {
            get { return (int)lastNavigationType; }
        }

        private TweenerCore<float, float, FloatOptions> tween;

        private NavigationBarParam paramData;

        protected override bool IsChildView => true;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            topView = main;

            paramData = param as NavigationBarParam;

            foreach (UIType itemType in Enum.GetValues(typeof(UIType)))
            {
                Button button = root.Find($"{itemType.ToString()}").GetComponent<Button>();
                button.onClick.AddListener(() => { NavigationBarOnClick(itemType, true); });

                button.GetComponent<RectTransform>().sizeDelta = GetNavigationButtonSize(itemType, UIType.Main);
                button.GetComponent<RectTransform>().localPosition = GetNavigationButtonPos(itemType, UIType.Main);
            }

            root.Find($"{UIType.Main}").GetComponent<Animator>().Play("select", 0, 1.0f);
            RefreshLivesBankRedTag();

            EventDispatcher.Instance.DispatchEvent(new LobbyNavigationActiveTypeEvent(UIType.Main));

            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyMainShowState, Show);
            EventDispatcher.Instance.AddEventListener(EventEnum.JumpToLobbyNavigationType, JumpToType);
            EventDispatcher.Instance.AddEventListener(EventEnum.LivesBankDataRefresh, LivesBankDataRefresh);
            EventDispatcher.Instance.AddEventListener(EventEnum.RED_POINT, onRedPointChange);
        }

        public override Task OnViewClose()
        {
            topView = null;

            //camera
            Rect rect = CameraManager.MainCamera.rect;
            CameraManager.MainCamera.rect = new Rect(0, rect.y, rect.width, rect.height);

            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyMainShowState, Show);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.JumpToLobbyNavigationType, JumpToType);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LivesBankDataRefresh, LivesBankDataRefresh);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RED_POINT, onRedPointChange);
            return base.OnViewClose();
        }

        public void NavigationBarOnClick(UIType uiType, bool isClicked = false)
        {
            float offset = ((int)UIType.Main - (int)uiType + 0.5f) * UIRoot.Instance.mRootCanvas.GetComponent<RectTransform>().sizeDelta.x;
            DOTween.Kill(paramData.offsetRoot);
            paramData.offsetRoot.DOLocalMove(new Vector3(offset, 0.0f, 0.0f), 0.5f).SetEase(Ease.OutQuart);

            if (uiType == lastNavigationType) return;
            // 点击的bi
            if (isClicked)
            {
                AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
                if (uiType == UIType.Energy)
                {
                    // DragonPlus.GameBIManager.SendGameEvent(BiEventMatchFrenzy.Types.GameEventType.GameEventLivesbankClick, 
                    //     data1: LivesBankModel.Instance.GetStorageHelpDict().Count.ToString());
                }

                if (uiType == UIType.Shop)
                {
                    // IAPController.Instance.SetIAPBiParaPlacement(BiEventMatchFrenzy.Types.MonetizationIAPEventPlacement.PlacementLobbyClickShop);
                }
            }

            //button anim
            {
                root.Find($"{lastNavigationType.ToString()}").GetComponent<Animator>().Play("unselect", 0, 1.0f);
                root.Find($"{uiType.ToString()}").GetComponent<Animator>().Play("select", 0, 0.0f);
            }
            //camera
            {
                Rect rect = CameraManager.MainCamera.rect;
                float cur = rect.x;
                float target = Mathf.Clamp(((int)UIType.Main - (int)uiType), -0.999f, 0.999f);
                tween?.Kill();
                tween = DOTween.To(() => cur,
                    value => { CameraManager.MainCamera.rect = new Rect(value, rect.y, rect.width, rect.height); },
                    target,
                    0.5f).SetEase(Ease.OutQuart);
            }

            //button pos & width
            foreach (UIType itemType in Enum.GetValues(typeof(UIType)))
            {
                Button button = root.Find($"{itemType.ToString()}").GetComponent<Button>();
                DOTween.Kill(button.GetComponent<RectTransform>());
                button.GetComponent<RectTransform>().DOSizeDelta(GetNavigationButtonSize(itemType, uiType), 0.5f).SetEase(Ease.OutQuart);
                button.GetComponent<RectTransform>().DOLocalMove(GetNavigationButtonPos(itemType, uiType), 0.5f).SetEase(Ease.OutQuart);
            }

            //bg
            {
                DOTween.Kill(BGSelect);
                Vector3 pos = GetNavigationButtonPos(uiType, uiType);
                BGSelect.DOLocalMove(new Vector3(pos.x, BGSelect.localPosition.y, BGSelect.localPosition.z), 0.5f).SetEase(Ease.OutQuart);
            }

            lastNavigationType = uiType;

            EventDispatcher.Instance.DispatchEvent(new LobbyNavigationActiveTypeEvent(uiType));
        }

        private Vector2 GetNavigationButtonSize(UIType targetType, UIType selectType)
        {
            Vector2 sizeDelta = root.Find($"{targetType.ToString()}").GetComponent<Button>().GetComponent<RectTransform>().sizeDelta;
            return (int)targetType - (int)selectType == 0 ? new Vector2(selectWidth, sizeDelta.y) : new Vector2(unselectWidth, sizeDelta.y);
        }

        private Vector3 GetNavigationButtonPos(UIType targetType, UIType selectType)
        {
            Button button = root.Find($"{targetType.ToString()}").GetComponent<Button>();
            Vector3 localPosition = button.GetComponent<RectTransform>().localPosition;
            float x = -UIRoot.Instance.mRoot.GetComponent<CanvasScaler>().referenceResolution.x * 0.5f;
            for (int i = 0; i < (int)targetType; i++)
            {
                x += ((int)selectType == i ? selectWidth : unselectWidth) + gapSize;
            }

            x += ((int)selectType == (int)targetType ? selectWidth : unselectWidth) * 0.5f;
            Vector3 pos = new Vector3(x, localPosition.y, localPosition.z);
            return pos;
        }

        public void Show(BaseEvent evt)
        {
            LobbyMainShowStateEvent realEvt = evt as LobbyMainShowStateEvent;
            if (realEvt.enable)
            {

                DOTween.Kill(transform.GetComponent<RectTransform>());
                transform.GetComponent<RectTransform>().DOAnchorPosY(0.0f, 0.3f);
            }
            else
            {
                DOTween.Kill(transform.GetComponent<RectTransform>());
                transform.GetComponent<RectTransform>().DOAnchorPosY(-462f, 0.3f);
            }
        }

        public void JumpToType(BaseEvent evt)
        {
            JumpToLobbyNavigationTypeEvent realEvt = evt as JumpToLobbyNavigationTypeEvent;
            NavigationBarOnClick(realEvt.type);
        }

        public void RefreshLivesBankRedTag()
        {
            // var redCount = LivesBankModel.Instance.GetStorageHelpDict().Count;
            // livesBankRedTag.gameObject.SetActive(redCount > 0);
            // if (redCount > 0) {
            //     livesBankRedTag.Find("NumberText").GetComponent<TextMeshProUGUI>().SetText(redCount.ToString());
            // }
        }

        public void LivesBankDataRefresh(BaseEvent evt)
        {
            RefreshLivesBankRedTag();
        }

        private void onRedPointChange(BaseEvent evt)
        {
            var eventData = evt as RedPointEvent;
            if (eventData.Index == (int)UIType.Team)
            {
                if (teamRedTag)
                {
                    teamRedTag.gameObject.SetActive(eventData.Value != 0);
                    if (eventData.Value > 0)
                    {
                        teamRedTag.Find("NumberText").GetComponent<TextMeshProUGUI>().SetText(eventData.Value.ToString());
                        teamRedTag.Find("Tips").GetComponent<Image>().gameObject.SetActive(false);
                    }
                    else if (eventData.Value < 0)
                    {
                        teamRedTag.Find("NumberText").GetComponent<TextMeshProUGUI>().SetText("");
                        teamRedTag.Find("Tips").GetComponent<Image>().gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}