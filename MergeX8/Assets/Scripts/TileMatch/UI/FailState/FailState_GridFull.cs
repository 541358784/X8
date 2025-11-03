using DragonU3DSDK;
using Framework;
namespace TileMatch.UI.FailState
{
    public class FailState_GridFull : FsmStateBase
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);
            var window =
                UIManager.Instance.OpenWindow(UINameConst.UIPopupTileMatchFail, FailTypeEnum.GridFull) as
                    UIPopupTileMatchFailController;
            window._OnBtnGiveUp = () =>
            {
                FailStateParamBase paramBase =stateParam as FailStateParamBase;
                paramBase.FailTypeEnum = FailTypeEnum.ReduceHp;
                Fsm.ChangeState<FailState_ReduceHp>(paramBase);
            };
        }

        public override void OnExit()
        {
            base.OnExit();
            

        }
    }
}