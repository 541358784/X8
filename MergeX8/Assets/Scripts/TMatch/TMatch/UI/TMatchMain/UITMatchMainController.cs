using System.Threading.Tasks;
using DragonPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UITMatchMain")]
    public partial class UITMatchMainController : UIView
    {
        private TMatchBaseItem _tMatchBaseItem;
        private static Transform levelTopView;

        public static Transform GetLevelTopView()
        {
            return levelTopView;
        }

        [ComponentBinder("Root/TOPGroup/Prop/Viewport/Content")]
        private Transform propRoot;
        
        [ComponentBinder("Root/TOPGroup/CurrencyCoin")]private Transform coinGroup;
        [ComponentBinder("Root/TOPGroup/CurrencyLive")] private Transform liveGroup;

        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);
            coinGroup.gameObject.AddComponent<CoinNum>();
            liveGroup.gameObject.AddComponent<LifeNumber>();
            levelTopView = transform.Find("Root/TOPGroup/LevelText");

            CommonUtils.NotchAdapte(transform.GetComponent<RectTransform>());
            transform.Find("Root/PropGroup/Halt").GetComponent<Button>().onClick.AddListener(PauseOnClick);
            AddChildView<UITMatchMainCollectItemView>(propRoot.gameObject);

            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_START, GameStart);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_FINISH, GameFinish);
            EventDispatcher.Instance.AddEventListener(EventEnum.OnApplicationPause, OnApplicationPauseEvent);
        }

        public override async Task OnViewClose()
        {
            levelTopView = null;

            DestoryTask();
            DestoryTime();
            DestoryBoost();
            DestoryLowSpace();
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_START, GameStart);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_FINISH, GameFinish);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.OnApplicationPause, OnApplicationPauseEvent);
            await base.OnViewClose();
        }

        public override void OnViewUpdate(float deltaTime)
        {
            base.OnViewUpdate(deltaTime);
            UpdateTime(Time.deltaTime);
            UpdateGuideTMItem();
        }

        private void PauseOnClick()
        {
            if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                UIViewSystem.Instance.Open<UIKapibalaLevelPauseController>();
            }
            else
            {
                UIViewSystem.Instance.Open<UITMatchLevelPauseController>();   
            }
        }

        protected override void OnBackButtonCallBack()
        {
            if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                UIViewSystem.Instance.Open<UIKapibalaLevelPauseController>();
            }
            else
            {
                UIViewSystem.Instance.Open<UITMatchLevelPauseController>();   
            }
        }

        private void GameStart(BaseEvent evt)
        {
            if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                var levelTextStr = LocalizationManager.Instance.GetLocalizedStringWithFormats($"&key.UI_return_reward_help_target",
                    (KapibalaModel.Instance.Storage.BigLevel+1)+"-"+(KapibalaModel.Instance.Storage.PlayingSmallLevel+1));
                transform.Find("Root/TOPGroup/LevelText").GetComponent<LocalizeTextMeshProUGUI>().SetText(levelTextStr);
            }
            else
            {
                var levelTextStr = LocalizationManager.Instance.GetLocalizedStringWithFormats($"&key.UI_return_reward_help_target",
                    TMatchModel.Instance.GetMainLevel().ToString());
                transform.Find("Root/TOPGroup/LevelText").GetComponent<LocalizeTextMeshProUGUI>().SetText(levelTextStr);
            }
            InitTask();
            InitTime();
            InitBoost();
            InitLowSpace();
            StartGuide();

            // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchCenter, transform.Find("Root/GuideTipsCenter"));
        }

        private void GameFinish(BaseEvent evt)
        {
            OnTimeFinishEvent();
        }

        private void StartGuide()
        {
            if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
                return;
            if (TMatchModel.Instance.TryGuideGame())
                return;

            if (TMatchModel.Instance.TryGuideWeekCollect())
                return;

            if (TMatchModel.Instance.TryGuideCollectLighting())
                return;

            if (TMatchModel.Instance.TryGuideCollectClock())
                return;

            if (TMatchModel.Instance.TryGuideMagnet())
                return;

            if (TMatchModel.Instance.TryGuideCover())
                return;

            if (TMatchModel.Instance.TryGuideBroom())
                return;

            if (TMatchModel.Instance.TryGuideWindmill())
                return;

            if (TMatchModel.Instance.TryGuideFrozen())
                return;
        }

        public void SetGuideTMItem(TMatchBaseItem tmItem)
        {
            _tMatchBaseItem = tmItem;
        }

        private void UpdateGuideTMItem()
        {
            if (_tMatchBaseItem == null || GuideTipsCenter == null)
                return;

            GuideTipsCenter.position = _tMatchBaseItem.ToUIPosition();
        }

        private void OnApplicationPauseEvent(BaseEvent evt)
        {
            OnApplicationPauseEvent realEvt = evt as OnApplicationPauseEvent;
            if (realEvt.pause)
            {
                // if (GuideSubSystem.Instance.IsShowingGuide()) return;
                PauseOnClick();
            }
        }
    }
}