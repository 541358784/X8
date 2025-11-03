using Framework.UI;
using Framework.Utils;
using MiniGame;
using UnityEngine;
using UnityEngine.Playables;

namespace Scripts.UI
{
    public class UIChapterContentB : UIChapterContentBase
    {
        protected internal override void OnOpen<T>(UIData data)
        {
            base.OnOpen<T>(data);

            EventBus.Register<EventMiniStoryDirectFinish>(OnEventMiniStoryDirectFinish);
        }

        protected override void OnClose()
        {
            base.OnClose();

            EventBus.UnRegister<EventMiniStoryDirectFinish>(OnEventMiniStoryDirectFinish);
        }

        protected override void InitPlayable()
        {
            base.InitPlayable();

            var resId = MiniGameModel.Instance.GetResIdByChapterId(_param.chapterId, ChapterType.New);
            var levelId = MiniGameModel.Instance.GetMinUnfinishLevelInChapter(_param.chapterId);
            var levelIdList = MiniGameModel.Instance.GetChapterLevels(_param.chapterId);
            var filePath = string.Empty;
            if (levelId == int.MaxValue) //全部完成
            {
                var timelineIndex = levelIdList.Count;
                filePath = $"NewMiniGame/MiniStory/Chapters/Chapter{resId}/Prefab/{timelineIndex}-finish";
            }
            else
            {
                var index = levelIdList.IndexOf(levelId);
                var timelineIndex = index + 1;
                filePath = $"NewMiniGame/MiniStory/Chapters/Chapter{resId}/Prefab/{timelineIndex}-before";
            }

            DirectorPlay(filePath);
        }

        private void OnEventMiniStoryDirectFinish(EventMiniStoryDirectFinish e)
        {
            _stateMachine.SetState<UIChapterContentBStatePlay>(new UIChapterContentBStatePlay.Data(this, _param.chapterId, e.levelId));
        }
    }
}