using Activity.BalloonRacing;
using Activity.RabbitRacing.Dynamic;

namespace AutoPopupManager
{
    public partial class AutoPopupManager
    {
        private bool _isPause = true;
        private AutoPopUI[] timeAutoPopUIArray;
        private void InitTimeAutoPopUI()
        {
            timeAutoPopUIArray = new AutoPopUI[]
            {
                new AutoPopUI(Activity.BattlePass.UIPopupBattlePassRefreshController.AutoShow, new[] { UINameConst.UIPopupBattlePassRefresh }),
                new AutoPopUI(Activity.BattlePass_2.UIPopupBattlePassRefreshController.AutoShow, new[] { UINameConst.UIPopupBattlePassRefresh }),
                
                new AutoPopUI(BalloonRacingModel.Instance.TimeCheckRacingEnding, new[]
                {
                    UINameConst.UIBalloonRacingMain, UINameConst.UIBalloonRacingFail, UINameConst.UIBalloonRacingReward, UINameConst.UIBalloonRacingOpenBox,
                    UINameConst.UIBalloonRacingStart
                }),
                //气球竞速
                new AutoPopUI(RabbitRacingModel.Instance.TimeCheckRacingEnding,
                    new[]{
                        UINameConst.UIPopupRabbitRacingStart, UINameConst.UIRabbitRacingMain, UINameConst.UIRabbitRacingReward, UINameConst.UIRabbitRacingOpenBox
                    }),
            };
        }
        
        protected override void InitImmediately()
        {
            base.InitImmediately();

            InvokeRepeating("InvokeUpdate", 0, 2);
        }

    
        public void SetPause(bool isPause)
        {
            _isPause = isPause;
        }
        
        private void InvokeUpdate()
        {
            if(_isPause)
                return;

            StartCoroutine(PopUIViewLogic(timeAutoPopUIArray, GetPopUINum(), printLog:false));
        }
    }
}