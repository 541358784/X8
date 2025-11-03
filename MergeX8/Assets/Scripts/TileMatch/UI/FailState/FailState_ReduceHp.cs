using DragonU3DSDK;
using Framework;
namespace TileMatch.UI.FailState
{
    public class FailState_ReduceHp : FsmStateBase
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);
            UIManager.Instance.OpenWindow(UINameConst.UIPopupTileMatchFail, FailTypeEnum.ReduceHp);
        }

        public override void OnExit()
        {
            base.OnExit();
            

        }
    }
}