using System.Linq;
using Framework.Utils;
using MiniGame;
using UnityEngine;
using UnityEngine.Playables;

namespace Scripts.UI
{
    public class UIChapterContentA : UIChapterContentBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

             EventBus.Register<EventMiniGameLevelFailRetryClicked>(OnEventMiniGameLevelFailRetryClicked); // 失败后点击重试，timeline复位
             EventBus.Register<EventMiniGameSelectionClicked>(OnClickedSelection); // 点击了三选一
             EventBus.Register<EventMiniGameSpecialHandleChapter2>(OnEventMiniGameSpecialHandleChapter2); // 第二章动画特殊处理
        }

        protected internal override void OnRemove()
        {
            base.OnRemove();

             EventBus.UnRegister<EventMiniGameLevelFailRetryClicked>(OnEventMiniGameLevelFailRetryClicked);
             EventBus.UnRegister<EventMiniGameSelectionClicked>(OnClickedSelection);
             EventBus.UnRegister<EventMiniGameSpecialHandleChapter2>(OnEventMiniGameSpecialHandleChapter2); // 第二章动画特殊处理
        }

        protected override void InitPlayable()
        {
            base.InitPlayable();

            // 先判断是不是可以领奖但还没有领
            var storage = MiniGameModel.Instance.GetChapterStorage(_param.chapterId);
            var levelIdList = MiniGameModel.Instance.GetChapterLevels(_param.chapterId);
            var allFinished = storage.LevelsDic.Values.All(c => c.Claimed || !levelIdList.Contains(c.Id)); //如果关卡列表内已经不包含存档内的数据,标为完成
            var unclaimed = allFinished && !storage.Claimed;

            if (unclaimed || allFinished && storage.Claimed)
            {
                _director.playableAsset = MiniGameModel.Instance.LoadChapterFinishIdleAsset(_param.chapterId, ChapterType.Normal);
            }
            else
            {
                var step = storage.LevelsDic.Values.Count(c => c.Claimed) + 1;
                var playableAsset = MiniGameModel.Instance.LoadLevelPlayableAsset(_param.chapterId, step, 0, ChapterType.Normal);
                _director.playableAsset = playableAsset;
            }
            
            MiniGameModel.Instance.SetGenericBindingBySpine(_director);
            
            _director.extrapolationMode = DirectorWrapMode.None;
            _director.Play();
        }


        private void OnEventMiniGameLevelFailRetryClicked(EventMiniGameLevelFailRetryClicked e)
        {
            InitPlayable();
        }

        private void OnClickedSelection(EventMiniGameSelectionClicked e)
        {
            _stateMachine.SetState<UIChapterContentAStatePlayingSelections>(new UIChapterContentAStatePlayingSelections.Data(this, e.selectionIndex, _param.chapterId, e.subLevelId, e.levelId));
        }

        private void OnEventMiniGameSpecialHandleChapter2(EventMiniGameSpecialHandleChapter2 e)
        {
            _stateMachine.SetState<UIChapterContentAStateSpecialHandle>(new UIChapterContentAStateSpecialHandle.Data(this, _param.chapterId));
        }
    }
}