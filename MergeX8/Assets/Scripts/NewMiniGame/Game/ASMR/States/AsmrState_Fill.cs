using ScratchCardAsset.Core;

namespace fsm_new
{
    public class AsmrState_Fill : AsmrState_ScratchCard_Base
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _cardManager.Card.Mode = ScratchMode.Restore;
            _cardManager.FillScratchCard();
        }
    }
}