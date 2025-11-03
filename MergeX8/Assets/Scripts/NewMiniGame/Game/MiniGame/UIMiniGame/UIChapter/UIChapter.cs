using System;
using System.Collections.Generic;
using System.Linq;
using ASMR;
using DragonPlus;
using DragonPlus.Config.MiniGame;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework.UI;
using Framework.UI.fsm;
using Framework.Utils;
using Gameplay;
using MiniGame;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Scripts.UI
{
    public class UIChapter : UIView
    {
        public class Data : UIData
        {
            public int chapterId;
            public ChapterType type;

            public Data(int chapterId, ChapterType type)
            {
                this.chapterId = chapterId;
                this.type = type;
            }
        }

        protected override bool _fitIphoneX => true;

        private Transform _root;

        public UIChapterTop TopGroup;
        public UISelections SelectGroup;

        public Transform ExitBtn;
        public Transform SelectArea => SelectGroup.Transform;

        public static bool JustFinishLastChapter;

        private StorageChapter _storage;
        private Data _data;
        private List<int> _levelIdList;

        private Transform _topStepPrefab;
        private Transform _topStepRoot;
        private List<UIChapterTopCell> _topCells;
        private Transform _previewRewardPoint;
        private Slider _progressSlider;

        private List<Transform> _bubbles;
        public const string Guide_key = "MiniBubble_";
        
        private UIChapterContentBase _chapterContent;

        public UIChapterContentBase ChapterContent => _chapterContent;

        private Transform _rewardTipObj;
        private Image _rewardIcon;
        private Animator _rewardAnimator;
        private Coroutine _coroutine;
        private LocalizeTextMeshProUGUI _rewardText;
        
        public int ChapterId => _data.chapterId;
        
        public static void Open(int chapterId)
        {
            var chapterConfig = MiniGameModel.Instance.GetChapterConfig(chapterId);
            var type = (ChapterType) chapterConfig.Type;
            var filePath = "NewMiniGame/UIMiniGame/Prefab/UIMiniGameLevelMain";
            //if (type == ChapterType.New) filePath = "MiniGame/UIMiniStory/Prefab/UIMiniStory";

            Framework.UI.UIManager.Instance.Open<UIChapter>(filePath, new Data(chapterId, type), UIWindowLayer.Min);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _root = BindItem("Root");
            TopGroup = BindElement<UIChapterTop>("Root/TopGroup");
            SelectGroup = BindElement<UISelections>("Root/SelectGroup");
            _topStepPrefab = BindItem("Root/TopGroup/SliderGroup/TargetGroup/TargetItem1");
            _topStepRoot = BindItem("Root/TopGroup/SliderGroup/TargetGroup");
            _previewRewardPoint = BindItem("Root/TopGroup/SliderGroup/ButtonBox/BubblePoint");
            _progressSlider = BindItem<Slider>("Root/TopGroup/SliderGroup/Slider");

            BindButtonEvent("Root/TopGroup/UICurrencyColumn/Root/TopGroup/CoinGroup", OnCoinIconClicked);
            ExitBtn = BindButtonEvent("Root/TopGroup/ButtonQuit", OnCloseBtnClick).transform;
            BindButtonEvent("Root/TopGroup/SliderGroup/ButtonBox", OnPreviewClick);
                
            _rewardTipObj = BindItem("Root/TopGroup/SliderGroup/ButtonBox/Tip");
            _rewardIcon = BindItem<Image>("Root/TopGroup/SliderGroup/ButtonBox/Tip/Icon");
            _rewardText = BindItem<LocalizeTextMeshProUGUI>("Root/TopGroup/SliderGroup/ButtonBox/Tip/Icon/Num");
            _rewardAnimator = BindItem<Animator>("Root/TopGroup/SliderGroup/ButtonBox/Tip");
            
            TopGroup.Show(false);
            SelectGroup?.Show(false);
            
                    
            MiniGameModel.Instance.MiniGameHandle();
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<UIChapterStateNormal>(data);

            _data = data as Data;

            _storage = MiniGameModel.Instance.GetChapterStorage(_data.chapterId);
            _levelIdList = MiniGameModel.Instance.GetChapterLevels(_data.chapterId);

            InitTopCells();
            LoadLevel();
            InitBGM();

            var config = MiniGameModel.Instance.GetChapterConfig(_data.chapterId);
            if (config != null)
            {
                _rewardIcon.sprite = UserData.GetResourceIcon(config.ChapterRewardId);
                _rewardText.SetText("x"+config.ChapterRewardCnt);
            }
            
            CheckChapterFinishReward();

            EventBus.Register<EventEnterMinigameLevel>(OnEnterLevel); // 进关
            EventBus.Register<EventMiniGameLevelWinClaimed>(OnEventMiniGameLevelWinClaimed); // 章节最后一关胜利，直接退出此 shell
            EventBus.Register<EventEnterASMRLevel>(OnEventEnterASMRLevel);
            EventBus.Register<EventMinigameProgressChanged>(OnEventMinigameProgressChanged);
            EventBus.Register<EventMiniGameSpecialHandleFinishChapter2>(OnEventMiniGameSpecialHandleFinishChapter2);
            
            EventDispatcher.Instance.AddEventListener(EventMiniGame.MINIGAME_SETSHOWSTATUS, RefreshMiniGameStatus);
            
            EventDispatcher.Instance.AddEventListener(EventMiniGame.MINIGAME_BGM, RefreshBGM);
        }

        public override void OnOpenFinish()
        {
            base.OnOpenFinish();

            var minUnFinishedChapter = MiniGameModel.Instance.GetMinUnFinishedChapterId();
            var minLevel = MiniGameModel.Instance.GetMinUnfinishLevelInChapter(minUnFinishedChapter);
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();

            EventBus.UnRegister<EventEnterMinigameLevel>(OnEnterLevel);
            EventBus.UnRegister<EventMiniGameLevelWinClaimed>(OnEventMiniGameLevelWinClaimed);
            EventBus.UnRegister<EventEnterASMRLevel>(OnEventEnterASMRLevel);
            EventBus.UnRegister<EventMinigameProgressChanged>(OnEventMinigameProgressChanged);
            EventBus.UnRegister<EventMiniGameSpecialHandleFinishChapter2>(OnEventMiniGameSpecialHandleFinishChapter2);
            EventDispatcher.Instance.RemoveEventListener(EventMiniGame.MINIGAME_SETSHOWSTATUS, RefreshMiniGameStatus);
            EventDispatcher.Instance.RemoveEventListener(EventMiniGame.MINIGAME_BGM, RefreshBGM);
        }

        private void InitTopCells()
        {
            _topCells = new();
            var objs = new List<Transform>();
            objs.Add(_topStepPrefab);
            
            for(int i = 0;i < _levelIdList.Count - 2; i++)
            {
                var prfab = Object.Instantiate(_topStepPrefab, _topStepRoot);
                objs.Add(prfab);
            }
            for (int i = 0; i < _levelIdList.Count - 1; i++)
            {
                var cell = BindElement<UIChapterTopCell>(objs[i].gameObject);
                cell.Init(i, _levelIdList[i]);
                _topCells.Add(cell);
            }

            SetTopCellProgress();
        }

        private void SetTopCellProgress()
        {
            var totalCount = _levelIdList.Count;
            var finishedCount = _storage.LevelsDic.Values.Count(c => c.Claimed);

            _progressSlider.minValue = 0;
            _progressSlider.maxValue = totalCount;
            _progressSlider.value = finishedCount;
            
            for (var i = 0; i < _topCells.Count; i++)
            {
                _topCells[i].SetFinishStatus(i < finishedCount);
            }
        }

        private void OnEventEnterASMRLevel(EventEnterASMRLevel e)
        {
            var config = MiniGameModel.Instance.GetLevelConfig(e.levelId);
            ASMRModel.Instance.LoadLevelbyLevelId(config.SubLevelId, new ASMRModel.AttachData(config.MiniGameId, e.subLevelId, e.levelId));

            Close();
        }

        private void OnCloseBtnClick()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, "Click_BackHome");
            EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, false);
            NoAdsGiftBagModel.Instance.TryShow();
        }

        private void InitBGM()
        {
            var bgmPath = MiniGameModel.Instance.GetBGMPath(_data.chapterId);
            if (bgmPath.EndsWith('/'))
            {
                AudioManager.Instance.StopAllMusic();
                return;
            }

            var clip = ResourcesManager.Instance.LoadResource<AudioClip>(MiniGameModel.Instance.GetBGMPath(_data.chapterId));
            AudioManager.Instance.PlayMusic(clip, true);
        }

        private void LoadLevel()
        {
            switch (_data.type)
            {
                case ChapterType.New:
                {
                    var resId = MiniGameModel.Instance.GetResIdByChapterId(_data.chapterId, _data.type);
                    var levelPath = $"NewMiniGame/MiniStory/Chapters/Chapter{resId}/Prefab/Level";
                    GameObject levelPrefab;

                    if (CommonUtils.IsLE_16_10())
                    {
                        levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>($"{levelPath}_Pad");
                        if (levelPrefab == null)
                            levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>(levelPath);
                    }
                    else
                    {
                        levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>(levelPath);
                    }

                    var level = Object.Instantiate(levelPrefab, _root);
                    level.transform.SetAsFirstSibling();

                    var ui = BindElement<UIChapterContentB>(level, new UIChapterContentBase.Data(_data.chapterId));

                    _chapterContent = ui;
                    break;
                }
                case ChapterType.Normal:
                {
                    var resId = MiniGameModel.Instance.GetResIdByChapterId(_data.chapterId, _data.type);
                    var levelPath = $"NewMiniGame/MiniGame/Chapters/Chapter{resId}/Prefab/UILevel";
                    GameObject levelPrefab;

                    if (CommonUtils.IsLE_16_10())
                    {
                        levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>($"{levelPath}_Pad");
                        if (levelPrefab == null)
                            levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>(levelPath);
                    }
                    else
                    {
                        levelPrefab = ResourcesManager.Instance.LoadResource<GameObject>(levelPath);
                    }

                    var level = Object.Instantiate(levelPrefab, _root);
                    level.transform.SetAsFirstSibling();

                    var ui = BindElement<UIChapterContentA>(level, new UIChapterContentBase.Data(_data.chapterId));
                    _chapterContent = ui;
                    break;
                }
            }

            for (var i = 0; i < _chapterContent?.Bubbles.Count; i++)
            {
                int index = i + 1;
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Bubble, _chapterContent.Bubbles[i].transform as RectTransform, targetParam:(Guide_key+index), topLayer:_chapterContent.Bubbles[i]);
            }
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Button, ExitBtn.transform as RectTransform, targetParam:("Click_BackHome"), topLayer:ExitBtn);

            _chapterContent?.ShowBubbles(false);
        }

        private void OnEnterLevel(EventEnterMinigameLevel e)
        {
            _stateMachine.SetState<UIChapterStateSelect>(new UIStateData(this));

            SelectGroup?.InitSelections(e.subLevelId, e.levelId);
        }

        private void OnEventMiniGameLevelWinClaimed(EventMiniGameLevelWinClaimed e)
        {
            CheckChapterFinishReward();
        }

        private void OnEventMinigameProgressChanged(EventMinigameProgressChanged obj)
        {
            SetTopCellProgress();
        }

        private void OnEventMiniGameSpecialHandleFinishChapter2(EventMiniGameSpecialHandleFinishChapter2 e)
        {
            OnImplementFinish();
        }

        private void CheckChapterFinishReward()
        {
            if (_storage.Claimed) return;

            var allFinish = _storage.LevelsDic.Values.All(c => c.Claimed || !_levelIdList.Contains(c.Id)); //如果关卡列表内已经不包含存档内的数据,标为完成

            if (allFinish)
            {
                if (_data.chapterId == 3)
                {
                    EventBus.Send<EventMiniGameSpecialHandleChapter2>();
                }
                else
                {
                    OnImplementFinish();
                }
            }
        }

        private void OnImplementFinish()
        {
            var rewardItems = MiniGameModel.Instance.ClaimChapterReward(_data.chapterId);

            CommonRewardManager.Instance.PopCommonReward(new List<ResData>(){rewardItems}, CurrencyGroupManager.Instance.currencyController, false, new GameBIManager.ItemChangeReasonArgs(){},
                animEndCall: () =>
                {
                    CurrencyGroupManager.Instance.currencyController.CheckLevelUp(() =>
                    {
                        JustFinishLastChapter = true;
                        OnCloseBtnClick();
                    
                        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, "Click_BackHome", "Click_BackHome");
                        UIMiniGameMain.Open();
                    });
                    
                },clickGetCall:() =>
                {
                });
        }

        private void OnCoinIconClicked()
        {
        }

        private void OnPreviewClick()
        {
            if(_coroutine == null)
                _rewardTipObj.gameObject.SetActive(false);
                
            _rewardTipObj.gameObject.SetActive(true);

            if(_coroutine != null)
                StopCoroutine(_coroutine);
            
            _coroutine = StartCoroutine(CommonUtils.DelayWork(3, () =>
            {
                _rewardAnimator.Play("disappear", -1, 0);
                _coroutine = null;
            }));

            //先注释
            // var config = MiniGameModel.Instance.GetChapterConfig(_data.chapterId);
            // var rewardCell = UICommonRewardBubble.Create(1);
            // var rewardList = new List<RewardData>();
            //
            //
            // var item = new RewardData();
            // item.itemId = config.ChapterRewardId;
            // item.count = config.ChapterRewardCnt;
            // rewardList.Add(item);
            //
            // var data = new UICommonRewardBubble.Data(rewardList, UICommonRewardBubble.Dir.BottomCenter);
            // AddElement<UICommonRewardBubble>(rewardCell, _previewRewardPoint, data);
        }

        void RefreshBGM(BaseEvent e)
        {
            InitBGM();
        }
        private void RefreshMiniGameStatus(BaseEvent e)
        {
            bool isMiniShow = (bool)e.datas[0];
            bool isImmediately = false;
            if(e.datas.Count() >= 2)
                isImmediately= (bool)e.datas[1];

            bool isTriggerGuide = true;
            if(e.datas.Length >= 3)
                isTriggerGuide= (bool)e.datas[2];
            if (isMiniShow)
            {
                InitBGM();
                AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true, true);
                TopGroup?.Show(true);
            
                _chapterContent?.ShowBubbles(true);

                if (isTriggerGuide)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Bubble, Guide_key+i, Guide_key+i);
                    }
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, "Click_BackHome", "Click_BackHome");
                }
            }
            else
            {
                TopGroup?.Show(false);
            
                _chapterContent?.ShowBubbles(false);
            }
        }
    }
}