
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using Framework.UI;
using Framework.Utils;
using MiniGame;
using Spine.Unity;
using Spine.Unity.Playables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Scripts.UI
{
    /// <summary>
    /// 小游戏内的剧情界面
    /// </summary>
    public class UIMiniGameStory : UIView
    {
        public class Data : UIData
        {
            public int chapterId;

            public Data(int chapterId)
            {
                this.chapterId = chapterId;
            }
        }

        private UIMiniGameStoryStateNormal _state;

        private Transform _root;
        private Transform _mask;

        private Button _button;

        public PlayableDirector Director { get; private set; }

        private int _storyIndex = 1;

        private Data _data;

        public Transform HelpBtn => _button.transform;

        private const string Guide_key = "MiniGame_ButtonHelp";
        
        public static void Open(int chapterId)
        {
            Framework.UI.UIManager.Instance.Open<UIMiniGameStory>("NewMiniGame/UIMiniGame/Prefab/UIEnter", new Data(chapterId));
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _root = BindItem("Root");
            _mask = BindItem("Root/ImgMask");
            _button = BindButtonEvent("Root/ButtonHelp", OnBtnHelpClicked);

            var anim = _transform.GetComponent<Animator>();
            if (anim != null) anim.enabled = false;
            
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MiniGame_Button, _button.transform as RectTransform, targetParam:Guide_key, topLayer:transform);
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<UIMiniGameStoryStateNormal>(data);

            _data = data as Data;

            HideMask();
            ShowButton(false);
            InitStory();
            InitBGM();
            
            EventBus.Register<EventMiniGameStoryFinished>(OnEventMiniGameStoryFinished);

            if (_data.chapterId == 2)
            {
                transform.Find("Root/ButtonHelp/TextGroup/TextName").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("UI_minigame_button2");
            }
            else
            {
                transform.Find("Root/ButtonHelp/TextGroup/TextName").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("UI_minigame_button");
            }
        }

        protected override void OnClose()
        {
            base.OnClose();

            EventBus.UnRegister<EventMiniGameStoryFinished>(OnEventMiniGameStoryFinished);
        }

        /// <summary>
        /// 剧情故事播放完毕，退出
        /// </summary>
        private void OnEventMiniGameStoryFinished(EventMiniGameStoryFinished e)
        {
            Close();

            UIChapter.Open(_data.chapterId);

            if (_data.chapterId > 1)
            {
                UIHomeMainController.mainController?.AnimShowMainUI(true, false);
            }
            else
            {
                MiniGameModel.Instance.MiniGameHandle();
            }
        }

        public void SetState(UIMiniGameStoryStateNormal s)
        {
            _state = s;
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

        /// <summary>
        /// 关闭黑色 mask
        /// </summary>
        private void HideMask()
        {
            _mask?.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显隐按钮
        /// </summary>
        public void ShowButton(bool isShow)
        {
            _button.gameObject.SetActive(isShow);

            if (isShow)
            {
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, Guide_key, Guide_key);

                // Help her 按钮动画
                var anim = _button.GetComponent<Animator>();
                anim?.Play("MiniGameHelp");
            }
        }

        /// <summary>
        /// 实例化 Story 预制体
        /// </summary>
        private void InitStory()
        {
            var bundleId = MiniGameModel.Instance.GetResIdByChapterId(_data.chapterId, ChapterType.Normal);
            var resPath = $"NewMiniGame/MiniGame/Chapters/Chapter{bundleId}/Prefab/UIStory";
            var storyPrefab = ResourcesManager.Instance.LoadResource<GameObject>(resPath);
            var storyInstance = GameObject.Instantiate(storyPrefab, _root);

            storyInstance.transform.SetAsFirstSibling();
            Director = storyInstance.GetComponent<PlayableDirector>();

            var audioSource = storyInstance.GetComponent<AudioSource>();
            if (audioSource)
                audioSource.mute = SettingManager.Instance.SoundClose;
        }

        /// <summary>
        /// 点击帮助
        /// </summary>
        private void OnBtnHelpClicked()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MiniGame_Button, Guide_key);
            
            ShowButton(false);

            if (!TryChangeToNextStory())
            {
                EventBus.Send<EventMiniGameStoryFinished>();
            }

            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MiniGame_Button, "Click_MiniGame", "Click_MiniGame");
        }

        /// <summary>
        /// 尝试播放下一个故事
        /// </summary>
        private bool TryChangeToNextStory()
        {
            var playableAsset = MiniGameModel.Instance.LoadStoryPlayableAsset(_data.chapterId, ++_storyIndex, ChapterType.Normal);
            if (playableAsset is null)
            {
                return false;
            }

            Director.playableAsset = playableAsset;

            MiniGameModel.Instance.SetGenericBindingBySpine(Director);
            
            Director.Play();

            _state.SetEndTime(Director.duration);

            return true;
        }
    }
}