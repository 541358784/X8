using System.Collections.Generic;
using System.Linq;
using Framework.UI;
using Framework.UI;
using Framework.UI.fsm;
using Framework.Utils;
using MiniGame;
using UnityEngine;
using UnityEngine.Playables;

namespace Scripts.UI
{
    public class UIChapterContentBase : UIElement
    {
        public class Data : UIData
        {
            public int chapterId;

            public Data(int chapterId)
            {
                this.chapterId = chapterId;
            }
        }

        protected Transform _bubbleRoot;
        protected Data _param;

        public List<Transform> Bubbles = new List<Transform>();

        private List<UILevelBubble> _bubbles;

        public override float OpenTimelineTime => 0f;

        protected virtual void InitPlayable()
        {
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _bubbleRoot = BindItem("Root/BubblePoint");
            _director = BindItem<PlayableDirector>("");

            EventBus.Register<EventEnterMinigameLevel>(OnEventMiniGameEnterLevel); // 进关
            EventBus.Register<EventMiniGameLevelWinClaimed>(OnOnEventMiniGameLevelWinClaimed); // 胜利并退出
        }

        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            _param = data as Data;
            _bubbles = new List<UILevelBubble>();

            _stateMachine.SetState<UIChapterContentBaseStateBubble>(new UIStateData(this));

            InitBubbles();
            InitPlayable();
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();

            EventBus.UnRegister<EventEnterMinigameLevel>(OnEventMiniGameEnterLevel);
            EventBus.UnRegister<EventMiniGameLevelWinClaimed>(OnOnEventMiniGameLevelWinClaimed);
        }

        private void InitBubbles()
        {
            Bubbles.Clear();
            for (var index = 0; index < _bubbleRoot.childCount; index++)
            {
                var childObj = _bubbleRoot.GetChild(index).gameObject;
                if(!childObj.name.Contains("BubbleGroupItem"))
                    continue;
                
                var bubble = BindElement<UILevelBubble>(_bubbleRoot.GetChild(index).gameObject, new UILevelBubble.Data(_param.chapterId, index));
                bubble.Show(false);
                _bubbles.Add(bubble);

                Bubbles.Add(bubble.Transform);
            }
        }

        public void ShowBubble()
        {
            foreach (var uiLevelBubble in _bubbles)
            {
                uiLevelBubble.Show(true);
            }
        }

        public void TriggerGuide()
        {
            var minUnFinishedChapter = MiniGameModel.Instance.GetMinUnFinishedChapterId();
            var minLevel = MiniGameModel.Instance.GetMinUnfinishLevelInChapter(minUnFinishedChapter);
            
            //先注释
            //GuideTool.Instance.StartGuide(GuideTriggerType.OpenUI, $"BubbleCreate-{minUnFinishedChapter}-{minLevel}");
        }

        public void ShowBubbles(bool isShow)
        {
            if (_bubbleRoot && _bubbleRoot.gameObject)
                _bubbleRoot.gameObject.SetActive(isShow);
        }

        private void OnEventMiniGameEnterLevel(EventEnterMinigameLevel e)
        {
            ShowBubbles(false);
        }

        private void OnOnEventMiniGameLevelWinClaimed(EventMiniGameLevelWinClaimed e)
        {
            var storage = MiniGameModel.Instance.GetChapterStorage(_param.chapterId);
            var levelIdList = MiniGameModel.Instance.GetChapterLevels(_param.chapterId);
            if (storage.LevelsDic.Values.All(c => c.Claimed || !levelIdList.Contains(c.Id))) //如果关卡列表内已经不包含存档内的数据,标为完成  
            {
                return;
            }

            ShowBubbles(true);
            InitPlayable();
        }
    }
}