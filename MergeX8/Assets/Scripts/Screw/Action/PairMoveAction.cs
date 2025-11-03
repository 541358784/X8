using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Screw
{
    public class PairMoveAction : GameAction
    {
        public List<ScrewModel> Models;
        public List<ScrewView> Views;

        public PairMoveAction(ScrewGameContext inContext) : base(inContext)
        {
        }

        public override async UniTask<bool> ExecuteAction()
        {
            bool isFull = false;
            bool isCollect = false;
            bool lose = false;

            List<Transform> collectTargets = new List<Transform>();
            for (int i = 0; i < Models.Count; i++)
            {
                var collectTarget = context.StorageScrew(Models[i]);

                if (collectTarget == null)
                    isFull = true;
                else
                    isCollect = true;
                
                collectTargets.Add(collectTarget);
            }

            context.hookContext.OnLogicEvent(LogicEvent.PrePairMove, new PairMoveParams(this));

            var connectBlockerView = context.GetBlockerView<ConnectBlockerView>(ScrewBlocker.ConnectBlocker, Models[0]);
            lose = isFull && isCollect;

            for (int i = 0; i < Views.Count; i++)
            {
                if (collectTargets[i] != null)
                {
                    if (i == Views.Count - 1)
                    {
                        await Views[i].DoMove(collectTargets[i], () =>
                        {
                            if (!lose)
                                connectBlockerView.DisConnect();
                        });
                    }
                    else
                    {
                        Views[i].DoMove(collectTargets[i]).Forget();
                    }
                }
            }

            context.hookContext.OnLogicEvent(LogicEvent.PostPairMove, new PairMoveParams(this));

            if (lose)
            {
                // TODO 直接判断为输
                // context.gameState = ScrewGameState.Fail;
                context.failReason = LevelFailReason.ConnectFailed;
                context.AddFailBlocker(LevelFailReason.ConnectFailed, Models[Models.Count - 1]);
            }

            return true;
        }
        
    }
}