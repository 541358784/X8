using DragonU3DSDK;
using Framework;
namespace TileMatch.UI.FailState
{
    public class FailState_Special : FsmStateBase
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);
            FailStateParamBase stateParamBase = stateParam as FailStateParamBase;
            var window =
                UIManager.Instance.OpenWindow(UINameConst.UIPopupTileMatchFail, FailTypeEnum.Special,stateParamBase.BlockTypeEnum) as
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