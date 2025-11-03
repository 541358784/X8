using System.Linq;
using Framework.UI;
using Framework.UI.fsm;
using Framework.Utils;
using MiniGame;
using NewMiniGame.Fsm;
using Spine.Unity;
using Spine.Unity.Playables;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Scripts.UI
{
    public class UIChapterContentAStatePlayingSelections : UIStateBase
    {
        public class Data : UIStateData
        {
            public int index;
            public int chapterId;
            public int SubLevelId;
            public int levelId;

            public Data(UIElement view, int index, int chapterId, int subLevelId, int levelId) : base(view)
            {
                this.index = index;
                this.chapterId = chapterId;
                this.SubLevelId = subLevelId;
                this.levelId = levelId;
            }
        }

        private Data _data;
        private float _endTime;
        private bool _selectIsRight;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _data = param as Data;

            var clickIndex = _data.index;
            var resultList = MiniGameModel.Instance.GetLevelAnswerBySubLevelId(_data.SubLevelId);

            _selectIsRight = resultList[clickIndex] > 0; // 1 为正确，0 为错误
            _endTime = PlaySelectionTimeline(clickIndex, _selectIsRight); // second

            //先注释
            //BITool.SendGameEvent(BiEventMatchRush3D.Types.GameEventType.GameEventMinigameLevelIconChoose, _data.levelId.ToString(), (clickIndex + 1).ToString(), _selectIsRight ? "Right" : "Wrong");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_elapsedTime >= _endTime)
            {
                Finish();
            }
        }

        private float PlaySelectionTimeline(int clickIndex, bool isRight)
        {
            var storage = MiniGameModel.Instance.GetChapterStorage(_data.chapterId);
            var step = storage.LevelsDic.Values.Count(c => c.Claimed) + 1;

            var playableAsset = MiniGameModel.Instance.LoadLevelPlayableAsset(_data.chapterId, step, clickIndex + 1, ChapterType.Normal);

            _data.view.Director.playableAsset = playableAsset;
            _data.view.Director.extrapolationMode = isRight ? DirectorWrapMode.None : DirectorWrapMode.Hold;

            // 特殊处理
            // 第一章第四关的正确选项，点击后需进入 hold 模式，否则动画会还原成初始状态
            if (_data.chapterId == 1 && step == 4 && isRight)
            {
                _data.view.Director.extrapolationMode = DirectorWrapMode.Hold;
            }

            MiniGameModel.Instance.SetGenericBindingBySpine(_data.view.Director);
            
            _data.view.Director.Play();
            return (float)MiniGameUtil.GetEndTimeWithSignal(_data.view.Director);
        }

        private void Finish()
        {
            if (_selectIsRight)
            {
                var rewardData = MiniGameModel.Instance.ClaimLevelReward(_data.chapterId, _data.levelId);

                UIMiniGameWin.Open(_data.chapterId, _data.levelId, rewardData);
            }
            else
            {
                EventBus.Send<EventMiniGameLevelFailed>();

                UIMiniGameFail.Open();
            }

            ChangeState<UIChapterContentBaseStateBubble>(_data);
        }
    }
}