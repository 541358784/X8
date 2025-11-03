using System;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonPlus.Config.MiniGame;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Framework.UI;
using Framework.Utils;
using Gameplay;
using MiniGame;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class UILevelBubble : UIElement
    {
        public class Data : UIData
        {
            public int chapterId;
            public int bubbleIndex;

            public Data(int chapterId, int bubbleIndex)
            {
                this.chapterId = chapterId;
                this.bubbleIndex = bubbleIndex;
            }
        }

        private Transform _bubbleGroup;

        private Image _icon;

        private Transform _finishMark;
        private Transform _lockMark;
        private Transform _playBtn;
        private Transform _lockBtn;

        private LocalizeTextMeshProUGUI _lockText;
        private LocalizeTextMeshProUGUI _lockText2;
        private LocalizeTextMeshProUGUI _costText1;
        private LocalizeTextMeshProUGUI _costText2;
        
        private Data _param;

        private int _levelId;
        private int _subLevelId;
        private MiniGameLevel _config;

        private List<int> _levelsInChapter;
        private int _minUnFinishLevelId;

        protected override void OnCreate()
        {
            base.OnCreate();
            BindItems();
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            _param = data as Data;
            _levelsInChapter = MiniGameModel.Instance.GetChapterLevels(_param.chapterId);
            _levelId = _levelsInChapter[_param.bubbleIndex];
            _config = MiniGameModel.Instance.GetLevelConfig(_levelId);
            _subLevelId = _config.SubLevelId;

            InitBubble(true);

            _costText1.SetText(_config.CostCount.ToString());
            _costText2.SetText(_config.CostCount.ToString());
            
            EventBus.Register<EventMiniGameLevelWin>(OnEventMiniGameLevelWin); // 出关
        }

        protected override void OnClose()
        {
            base.OnClose();

            EventBus.UnRegister<EventMiniGameLevelWin>(OnEventMiniGameLevelWin);
        }

        private void BindItems()
        {
            _bubbleGroup = BindItem("BubbleGroup");
            _icon = BindItem<Image>("BubbleGroup/ImgIcon");

            _finishMark = BindItem("BubbleGroup/FinishGroup");
            _lockMark = BindItem("BubbleGroup/LockGroup");

            _playBtn = BindItem("ButtonPlay");
            _lockBtn = BindItem("ButtonLock");

            _lockText = BindItem<LocalizeTextMeshProUGUI>("ButtonLock/TextGroup/TextName");
            _lockText2 = BindItem<LocalizeTextMeshProUGUI>("ButtonLock/TextGroup/TextNameOutline");
            
            _costText1 = BindItem<LocalizeTextMeshProUGUI>("ButtonPlay/TextGroup/TextName");
            _costText2 = BindItem<LocalizeTextMeshProUGUI>("ButtonLock/TextGroup/TextName");
            
            BindButtonEvent(OnClick);
        }

        private void InitBubble(bool initRot = false)
        {
            // 只有第一次调整一下旋转，否则会再次旋转
            if (initRot) DealWithRotation(); // 旋转
        }

        public void Show(bool show)
        {
            if (MiniGameModel.Instance.GetMinUnFinishedChapterId() > _param.chapterId)
            {
                _gameObject.SetActive(false);
                return;
            }

            _minUnFinishLevelId = MiniGameModel.Instance.GetMinUnfinishLevelInChapter(_param.chapterId);
            var realShow = show && _minUnFinishLevelId == _levelId;
            _gameObject.SetActive(realShow);

            if (!realShow) return;

            var config = MiniGameModel.Instance.GetLevelConfig(_levelId);
            _icon.sprite = ResourcesManager.Instance.GetSpriteVariant(MiniGameModel.MiniGameAtlas, $"{config.Icon}");

            var status = GetBubbleStatus();

            _finishMark.gameObject.SetActive(status.Status is MiniGameCommonStatus.Finished);
            _lockMark.gameObject.SetActive(status.Status is MiniGameCommonStatus.Lock);

            _playBtn.gameObject.SetActive(status.Status is MiniGameCommonStatus.Processing);
            _lockBtn.gameObject.SetActive(status.Status is MiniGameCommonStatus.Lock && UserData.Instance.CanAford(_config.CostId, _config.CostCount));

            PlayAnim("MiniGameHelp");

            RefreshPlayButtonStatus();
        }

        private void RefreshPlayButtonStatus()
        {
            StorageLevel storage = MiniGameModel.Instance.GetLevelStorage(_param.chapterId, _levelId);
            _playBtn.gameObject.SetActive(!storage.IsBuy);
        }
        
        /// <summary>
        /// 处理气泡内容等的旋转角度
        /// </summary>
        private void DealWithRotation()
        {
            var rot = _bubbleGroup.GetComponent<RectTransform>().localRotation;
            var inverse = Quaternion.Inverse(rot);

            // bubble icon
            InverseTransformRot(_icon.transform, inverse);

            // finish mark
            InverseTransformRot(_finishMark, inverse, true);

            // lock mark
            InverseTransformRot(_lockMark, inverse, true);
        }

        /// <summary>
        /// 反向旋转
        /// </summary>
        private static void InverseTransformRot(Transform tr, Quaternion inverse, bool isFindImgIcon = false)
        {
            var t = isFindImgIcon ? tr.Find("ImgIcon") : tr;

            t.GetComponent<RectTransform>().localRotation = inverse * tr.GetComponent<RectTransform>().localRotation;
        }

        /// <summary>
        /// 获取当前气泡的解锁状态
        /// </summary>
        private MiniGameStatus GetBubbleStatus()
        {
            var result = new MiniGameStatus();

            if (_minUnFinishLevelId > _levelId)
            {
                result.Status = MiniGameCommonStatus.Finished;
                result.LockReason = MiniGameLockReason.None;
                return result;
            }

            
            if (_minUnFinishLevelId == _levelId)
            {
                result.Status = MiniGameCommonStatus.Processing;
            }
            else if (_minUnFinishLevelId < _levelId)
            {
                result.LockReason = MiniGameLockReason.NotFinishPre;
                result.Status = MiniGameCommonStatus.Lock;
            }
            
            return result;
        }

        /// <summary>
        /// 尝试更新玩家上一次进入某章节后所看到的最大解锁关卡ID
        /// </summary>
        private void TryUpdateMaxUnlockLevelIdLastEnter()
        {
             StorageManager.Instance.GetStorage<StorageMiniGameVersion>().LastEnterUnlockLevelId = _levelId;
            
             var finalLevelInThisChapter = MiniGameModel.Instance.GetTheLastLevelInChapter(_param.chapterId);
             var finalThreshold = MiniGameModel.Instance.GetLevelConfig(finalLevelInThisChapter);
             if (finalThreshold.Id == _levelId)
             {
                 StorageManager.Instance.GetStorage<StorageMiniGameVersion>().LastEnterUnlockLevelId = finalLevelInThisChapter;
             }
        }

        private void OnClick()
        {
            var status = GetBubbleStatus();

            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Bubble, UIChapter.Guide_key+(_param.bubbleIndex+1));

            Action action = () =>
            {
                UIRoot.Instance.EnableEventSystem = true;
                RefreshPlayButtonStatus();
                switch ((MiniGameLevelType)_config.LevelType)
                {
                    case MiniGameLevelType.ASRM:
                        EventBus.Send(new EventEnterASMRLevel(_subLevelId, _levelId));
                        break;
                    case MiniGameLevelType.STORY:
                        EventBus.Send(new EventEnterMinigameLevel(_subLevelId, _levelId));
                        break;
                    case MiniGameLevelType.Direct:
                        EventBus.Send(new EventMiniStoryDirectFinish(_subLevelId, _levelId));
                        _gameObject.SetActive(false);
                        break;
                }

                StorageManager.Instance.GetStorage<StorageMiniGameVersion>().CurrentLevel = _levelId;
            };
            
            StorageLevel storage = MiniGameModel.Instance.GetLevelStorage(_param.chapterId, _levelId);
            if (!storage.IsBuy)
            {
                if (!UserData.Instance.CanAford(_config.CostId, _config.CostCount))
                {
                    UIPopupNoMoneyController.ShowUI(_config.CostId);
                    return;
                }

                storage.IsBuy = true;
                UserData.Instance.ConsumeRes(new ResData(_config.CostId, _config.CostCount), new GameBIManager.ItemChangeReasonArgs(){});
                TryUpdateMaxUnlockLevelIdLastEnter();

                Vector3 srcPos = FlyGameObjectManager.Instance.GetResourcePosition((UserData.ResourceId)_config.CostId);
                UIRoot.Instance.EnableEventSystem = false;
                DecoBuyNodeView.FlyEffectTail(srcPos, _playBtn.transform.position, 0.5f, (UserData.ResourceId)_config.CostId, action);
                
                return;
            }

            action();  
        }

        private void OnEventMiniGameLevelWin(EventMiniGameLevelWin e)
        {
            InitBubble();
        }
    }
}