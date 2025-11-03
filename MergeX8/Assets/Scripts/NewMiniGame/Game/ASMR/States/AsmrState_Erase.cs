using System;
using DG.Tweening;
using DragonU3DSDK;
using ScratchCardAsset;
using ScratchCardAsset.Core;
using UnityEngine;

namespace fsm_new
{
    public class AsmrState_Erase : AsmrState_ScratchCard_Base
    {
        public override void OnEnter(FsmStateParamBase stateParam)
        {
            base.OnEnter(stateParam);

            _cardManager.Card.Mode = ScratchMode.Erase;
            _cardManager.ClearScratchCard();
        }
    }
}