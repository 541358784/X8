using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Screw
{
    public class ScrewActionController
    {
        private ScrewGameContext context;

        private int actionId = 0;

        private List<int> activeAction;

        public ScrewActionController(ScrewGameContext inContext)
        {
            activeAction = new List<int>();
            context = inContext;
        }

        public void ResetActionId()
        {
            actionId = 0;
            activeAction.Clear();
        }

        private int AllocationId()
        {
            return actionId++;
        }

        public async UniTaskVoid AddMoveAction(ScrewModel model, ScrewView view)
        {
            var tempActionId = AllocationId();
            //Log.Info($"Move:{tempActionId}");

            activeAction.Add(tempActionId);
            var moveAction = new MoveAction(context);
            moveAction.Model = model;
            moveAction.View = view;

            bool success = await moveAction.ExecuteAction();
            activeAction.Remove(tempActionId);
            if (success)
                context.hookContext.OnLogicEvent(LogicEvent.CheckTask, null);
        }
        
        public async UniTaskVoid AddPairMoveAction(List<ScrewModel> cellModels, List<ScrewView> views)
        {
            var tempActionId = AllocationId();
            activeAction.Add(tempActionId);
            var pairMoveAction = new PairMoveAction(context);
            pairMoveAction.Models = cellModels;
            pairMoveAction.Views = views;

            bool success = await pairMoveAction.ExecuteAction();
            activeAction.Remove(tempActionId);
            if (success)
                context.hookContext.OnLogicEvent(LogicEvent.CheckTask, null);
            else
                context.hookContext.OnLogicEvent(LogicEvent.ActionFinish, null);
        }

        public bool HasActionInExecute()
        {
            return activeAction.Count > 0;
        }
    }
}