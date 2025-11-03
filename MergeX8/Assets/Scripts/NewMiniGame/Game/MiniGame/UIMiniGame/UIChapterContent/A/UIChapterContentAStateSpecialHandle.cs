using Framework.UI;
using Framework.UI.fsm;
using Framework.Utils;
using MiniGame;
using NewMiniGame.Fsm;
using UnityEngine.Playables;

namespace Scripts.UI
{
    public class UIChapterContentAStateSpecialHandle : UIStateBase
    {
        public class Data : UIStateData
        {
            public int chapterId;

            public Data(UIElement view,int chapterId) : base(view)
            {
                this.chapterId = chapterId;
            }
        }

        private Data _data;
        private float _endTime;

        public override void Enter(StateData param)
        {
            base.Enter(param);

            _data = param as Data;


            if (ExperenceModel.Instance.IsCanLevelUp())
            {
                _endTime = -1;
                
                var playableAsset = MiniGameModel.Instance.LoadChapterSpecialAsset8(_data.chapterId, ChapterType.Normal);
                _data.view.Director.playableAsset = playableAsset;
                _data.view.Director.extrapolationMode = DirectorWrapMode.None;
                
                MiniGameModel.Instance.SetGenericBindingBySpine(_data.view.Director);
                
                _data.view.Director.Play();
            }
            else
            {
                _endTime = PlaySelectionTimeline();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(_endTime < 0)
                return;
            
            if (_elapsedTime >= _endTime)
            {
                Finish();
            }
        }

        private float PlaySelectionTimeline()
        {

            var playableAsset = MiniGameModel.Instance.LoadChapterSpecialAsset(_data.chapterId, ChapterType.Normal);

            _data.view.Director.playableAsset = playableAsset;
            _data.view.Director.extrapolationMode = DirectorWrapMode.None;
                
            MiniGameModel.Instance.SetGenericBindingBySpine(_data.view.Director);
            
            _data.view.Director.Play();

            return (float)MiniGameUtil.GetEndTimeWithSignal(_data.view.Director);
        }

        private void Finish()
        {
            EventBus.Send<EventMiniGameSpecialHandleFinishChapter2>();

            ChangeState<UIChapterContentBaseStateBubble>(_data);
        }
    }
}