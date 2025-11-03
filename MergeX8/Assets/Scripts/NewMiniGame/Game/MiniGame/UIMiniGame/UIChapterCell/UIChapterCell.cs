using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Decoration;
using DragonPlus;
using DragonPlus.Config.MiniGame;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Framework.UI;
using Framework.UI.fsm;
using Framework.Utils;
using Manager;
using MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class ChapterItemCellData : UIData
    {
        public int chapterId;
        public bool isComingSoon;
    }

    public class UIChapterCell : UIElement
    {
        private ChapterItemCellData _param;

        private UIElement _lockGroup;
        private UIElement _playGroup;
        private Transform _imgGroup;

        private Image _lockImg;
        private Image _playImg;

        private Transform _playIcon;
        private Transform _newMark;
        private Transform _finishMark;

        private LocalizeTextMeshProUGUI _title;
        private LocalizeTextMeshProUGUI _title2;

        private Slider _slider;
        private Transform _sliderRoot;
        private LocalizeTextMeshProUGUI _processText;

        private Transform _lineGroup;
        private Transform _sliderFirstLine;
        private Transform _sliderLastLine;

        private LocalizeTextMeshProUGUI _desText;
        private LocalizeTextMeshProUGUI _normalDesText;
        
        private MiniGameStatus _chapterStatus;

        private StorageChapter _storage;
        public StorageChapter Storage => _storage;

        private const string comingSoon = "UI_common_ComingSoon";
        private const string unlockAnimName = "open";

        public const string _guideKey = "PopupTask_";

        private Transform _okButton;
        
        public int ChapterId => _param.chapterId;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _lockGroup = BindElement<UIElement>("LockGroup");
            _playGroup = BindElement<UIElement>("PlayGroup");
            _imgGroup = BindItem("PlayGroup/ImgGroup");

            _lockImg = BindItem<Image>("LockGroup/ImgIcon");
            _playImg = BindItem<Image>("PlayGroup/ImgIcon");

            _playIcon = BindItem("PlayGroup/SliderGroup/ImgPlay");
            _newMark = BindItem("NewTagGroup");
            _finishMark = BindItem("ClearedTagGroup");

            _title = BindItem<LocalizeTextMeshProUGUI>("TitleGroup/TextGroup/TextName");
            _title2 = BindItem<LocalizeTextMeshProUGUI>("TitleGroup/TextGroup/TextNameOutline");
            _desText= BindItem<LocalizeTextMeshProUGUI>("LockGroup/Text");
            _normalDesText = BindItem<LocalizeTextMeshProUGUI>("PlayGroup/Text");

            _okButton = BindItem("PlayGroup/ButtonOk");
            
            _sliderRoot = BindItem("PlayGroup/SliderGroup");
            _slider = BindItem<Slider>("PlayGroup/SliderGroup/Slider");
            _processText = BindItem<LocalizeTextMeshProUGUI>("PlayGroup/SliderGroup/Slider/Text");

            _lineGroup = BindItem("PlayGroup/SliderGroup/Slider/LineGroup");
            _sliderFirstLine = BindItem("PlayGroup/SliderGroup/Slider/LineGroup/Line1");
            _sliderLastLine = BindItem("PlayGroup/SliderGroup/Slider/LineGroup/LineLast");

            BindButtonEvent(OnPlayBtnClicked);
            

        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<UIChapterCellStateNormal>(data);

            _param = data as ChapterItemCellData;

            _storage = MiniGameModel.Instance.GetChapterStorage(_param.chapterId);
            _chapterStatus = GetCurrChapterStatus();

            if (_param.isComingSoon)
            {
                _stateMachine.SetState<UIChapterCellStateComingSoon>(new UIStateData(this));
            }
            else
            {
                switch (_chapterStatus.Status)
                {
                    case MiniGameCommonStatus.Lock:
                        _stateMachine.SetState<UIChapterCellStateLock>(new UIStateData(this));
                        break;
                    case MiniGameCommonStatus.Processing:
                        _stateMachine.SetState<UIChapterCellStateNormal>(new UIStateData(this));
                        break;
                    case MiniGameCommonStatus.Finished:
                        _stateMachine.SetState<UIChapterCellStateFinish>(new UIStateData(this));
                        break;
                }
            }            
            
            
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Button, _transform as RectTransform, targetParam:(_guideKey+_param.chapterId), topLayer:_transform);
            
            if(_param.chapterId == 1)
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, _guideKey + _param.chapterId, _guideKey + _param.chapterId);
        }

        protected override void OnClose()
        {
            base.OnClose();
        }

        public void SetComingSoon()
        {
            _playImg.sprite = ResourcesManager.Instance.GetSpriteVariant(MiniGameModel.MiniGameAtlas, "ui_minigame_bg_icon_chapter_comingsoon");

            _title.SetTerm(comingSoon);
            _title2.SetTerm(comingSoon);

            _desText.SetTerm(comingSoon);
            _normalDesText.SetTerm(comingSoon);
            
            _newMark.gameObject.SetActive(false);
            _finishMark.gameObject.SetActive(false);
            _playIcon.gameObject.SetActive(false);
            _sliderRoot.gameObject.SetActive(false);
            _okButton.gameObject.SetActive(false);
            
            _playGroup.GameObject.SetActive(true); // 用来显示背景
            _imgGroup.gameObject.SetActive(false); // 隐藏 play 按钮
            _lockGroup.GameObject.SetActive(false);
        }

        public void SetLock()
        {
            InitNormalBg();
            InitNormalTitle();
            _finishMark.gameObject.SetActive(false);
            _playIcon.gameObject.SetActive(false);
            _sliderRoot.gameObject.SetActive(false);
            _lockGroup.GameObject.SetActive(true);
            _playGroup.GameObject.SetActive(true);
            _newMark.gameObject.SetActive(false);
            
            var config = MiniGameConfigManager.Instance.MiniGameChapterList.Find(a => a.Id == _param.chapterId);
            if (config != null)
            {
                _desText.SetTerm(config.DesKey);
                _normalDesText.SetTerm(config.DesKey);
            }
        }

        public void SetFinish()
        {
            InitNormalBg();
            InitNormalTitle();

            _finishMark.gameObject.SetActive(true);
            _playIcon.gameObject.SetActive(false);
            _sliderRoot.gameObject.SetActive(false);
            _lockGroup.GameObject.SetActive(false);
            _playGroup.GameObject.SetActive(true);
            _newMark.gameObject.SetActive(false);
            _normalDesText.gameObject.SetActive(false);
            _okButton.gameObject.SetActive(false);
        }

        public void SetNormal(bool firstUnlock)
        {
            InitNormalBg();
            InitNormalTitle();

            _newMark.gameObject.SetActive(MiniGameModel.Instance.HaveNewLevelUnlocked());
            _finishMark.gameObject.SetActive(false);
            _playIcon.gameObject.SetActive(true);
            _sliderRoot.gameObject.SetActive(false);
            _lockGroup.GameObject.SetActive(false || firstUnlock);
            _playGroup.GameObject.SetActive(true && !firstUnlock);

            var config = MiniGameConfigManager.Instance.MiniGameChapterList.Find(a => a.Id == _param.chapterId);
            if (config != null)
            {
                _desText.SetTerm(config.DesKey);
                _normalDesText.SetTerm(config.DesKey);
            }
            
            // Progress
            var levelIdList = MiniGameModel.Instance.GetChapterLevels(_param.chapterId);
            var levelCount = levelIdList.Count;

            _slider.wholeNumbers = true;
            _slider.maxValue = levelCount;

            var maxFinishedLevelId = MiniGameModel.Instance.GetMaxFinishLevelInChapter(_param.chapterId);
            if (maxFinishedLevelId == int.MinValue)
            {
                _slider.value = 0;
                _processText.SetText($"{_slider.value}/{_slider.maxValue}");
            }
            else
            {
                var levelIndex = levelIdList.FindIndex(c => c == maxFinishedLevelId);

                _slider.value = levelIndex + 1;
                _processText.SetText($"{_slider.value}/{_slider.maxValue}");
            }

            // 竖线 分割线
            while (_lineGroup.childCount < levelCount) GameObject.Instantiate(_sliderFirstLine, _lineGroup);
            _sliderLastLine.SetAsLastSibling();
        }

        private void InitNormalBg()
        {
            var chapterConfig = MiniGameModel.Instance.GetChapterConfig(_param.chapterId);
            var chapterIcon =  chapterConfig.Icon;
            var targetSprite = ResourcesManager.Instance.GetSpriteVariant(MiniGameModel.MiniGameAtlas, chapterIcon);

            _playImg.sprite = targetSprite;
            _lockImg.sprite = targetSprite;
        }

        private void InitNormalTitle()
        {
            const string chapter = "UI_minigame_chap";
            var thisChapter = _param.chapterId.ToString();

            var des = string.Format(LocalizationManager.Instance.GetLocalizedString(chapter), thisChapter);
             _title.SetTerm(des);
             _title2.SetTerm(des);
        }

        private MiniGameStatus GetCurrChapterStatus()
        {
            var result = new MiniGameStatus();

            result.LockReason = MiniGameLockReason.None;

            if (_param.isComingSoon)
            {
                result.Status = MiniGameCommonStatus.Lock;
            }
            else
            {
                var minUnFinishedChapterId = MiniGameModel.Instance.GetMinUnFinishedChapterId();

                if (minUnFinishedChapterId < _param.chapterId)
                {
                    result.LockReason = MiniGameLockReason.NotFinishPre;
                    result.Status = MiniGameCommonStatus.Lock;
                }
                else if (minUnFinishedChapterId == _param.chapterId)
                {
                    result.Status = MiniGameCommonStatus.Processing;
                }
                else if (minUnFinishedChapterId > _param.chapterId)
                {
                    result.Status = MiniGameCommonStatus.Finished;
                }
            }

            return result;
        }

        private void OnPlayBtnClicked()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, _guideKey + _param.chapterId);
            
            var status = GetCurrChapterStatus();
            if (_param.isComingSoon)
            {
                //先注释
                //UICommonTips.Open(comingSoon);
                return;
            }

            Action action = () =>
            {
                MiniGameModel.Instance.CurrentChapter = _param.chapterId;
                UIPopupTaskController.CloseView(async () =>
                {
                    GameModeManager.Instance.SetCurrentGameMode(GameModeManager.CurrentGameMode.MiniGame);
                    
                    await UniTask.WaitForEndOfFrame();
                    if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                    {
                        GameModeManager.Instance.RefreshGameStatus(false);
                        Action cb = () =>
                        {
                            if (GameModeManager.Instance.GetGameMode() == GameModeManager.GameMode.MiniAndMerge)
                            {
                                if (ExperenceModel.Instance.GetLevel() < UnlockManager.GetUnlockParam(UnlockManager.MergeUnlockType.MiniGame_Deo))
                                {
                                    GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, "Click_MiniGame");
                                    EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, true);
                                }
                            }
                        };
                        SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome, DecoOperationType.MiniGame, _param.chapterId, cb);
                    }
                    else
                    {
                        if (Framework.UI.UIManager.Instance.GetView<UIChapter>() == null)
                        {
                            GameModeManager.Instance.RefreshGameStatus(true);
                        }
                        else
                        {
                            GameModeManager.Instance.RefreshGameStatus(false);
                            EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, true);
                        }
                    }
                });
            };

            if (status.Status == MiniGameCommonStatus.Lock && status.LockReason is MiniGameLockReason.NotFinishPre)
            {
                //先注释
                //UICommonTips.Open(lockReason_NotFinishPre);
                return;
            }

            if (status.Status == MiniGameCommonStatus.Finished)
            {
                int chapterId = MiniGameModel.Instance.GetMinUnFinishedChapterId();
                int lastId = MiniGameConfigManager.Instance.MiniGameChapterList.Last().Id;
                if (chapterId > lastId && _param.chapterId == lastId)
                    action();
                
                return;
            }

            GameModeManager.Instance.SetCurrentGameMode(GameModeManager.CurrentGameMode.MiniGame);
            
            if (status.Status == MiniGameCommonStatus.Processing)
            {
                action();
            }
        }

        /// <summary>
        /// 刚刚完成前面一章，衔接一个解锁动画、bg 变化的过渡
        /// </summary>
        public void PlayUnlockAnimWhenFinishLastChapter()
        {
            var minUnfinishedChapter = MiniGameModel.Instance.GetMinUnFinishedChapterId();

            if (_param.isComingSoon || minUnfinishedChapter != _param.chapterId) return;

            UIChapter.JustFinishLastChapter = false;

            _stateMachine.SetState<UIChapterCellStateUnlock>(new UIStateData(this));
        }

        public float PlayUnlockAnim()
        {
            _playGroup.GameObject.SetActive(true);
            var time1 = _lockGroup.PlayAnim(unlockAnimName);
            var time2 = _playGroup.PlayAnim(unlockAnimName);

            return Mathf.Max(time1, time2);
        }
    }
}