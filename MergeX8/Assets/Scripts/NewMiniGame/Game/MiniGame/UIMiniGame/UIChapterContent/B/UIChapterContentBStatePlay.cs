using Framework.UI;
using Framework.UI.fsm;
using MiniGame;
using NewMiniGame.Fsm;

namespace Scripts.UI
{
    public class UIChapterContentBStatePlay : UIStateBase
    {
        public class Data : UIStateData
        {
            public int chapterId;
            public int levelId;

            public Data(UIElement view, int chapterId, int levelId) : base(view)
            {
                this.chapterId = chapterId;
                this.levelId = levelId;
            }
        }

        private Data _data;

        private UIChapterContentB _ui;

        private float _endTime;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _data = param as Data;

            _ui = _data.view as UIChapterContentB;

            var resId = MiniGameModel.Instance.GetResIdByChapterId(_data.chapterId, ChapterType.New);
            var levelIdList = MiniGameModel.Instance.GetChapterLevels(_data.chapterId);
            var timelineIndex = levelIdList.IndexOf(_data.levelId) + 1;
            var filePath = $"NewMiniGame/MiniStory/Chapters/Chapter{resId}/Prefab/{timelineIndex}-play";

            _ui.DirectorPlay(filePath);

            _endTime = (float)_ui.Director.duration;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_elapsedTime >= _endTime)
            {
                Finish();
            }
        }

        private void Finish()
        {
            ChangeState<UIChapterContentBaseStateBubble>(_data);

            var rewardData = MiniGameModel.Instance.ClaimLevelReward(_data.chapterId, _data.levelId);
            UIMiniGameWin.Open(_data.chapterId, _data.levelId, rewardData);
        }
    }
}