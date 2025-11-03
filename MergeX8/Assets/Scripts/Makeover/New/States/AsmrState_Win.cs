using UnityEngine;

namespace MiniGame
{
    public class AsmrState_Win : AsmrState_Base
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            UIManager.Instance.GetOpenedUIByPath<UIGameMainController>(UINameConst.UIGameMain).SetFinishLevel();
        }
    }
}