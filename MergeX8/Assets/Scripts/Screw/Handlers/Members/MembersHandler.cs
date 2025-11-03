using System.Collections.Generic;

namespace Screw
{
    public class MembersHandler : ILogicEventHandler
    {
        private ScrewGameContext context;
        private readonly Dictionary<ScrewShape, MemberHandler> _members;

        public MembersHandler(ScrewGameContext inContext)
        {
            context = inContext;

            _members = new Dictionary<ScrewShape, MemberHandler>();
            
            _members.Add(ScrewShape.Phillips, new PhillipsHandler(inContext));
            _members.Add(ScrewShape.Star, new StarHandler(inContext));
            _members.Add(ScrewShape.Diamond, new DiamondHandler(inContext));
            _members.Add(ScrewShape.Triangle, new TriangleHandler(inContext));
        }
        
        public void HandleScrewDown(ScrewModel model, ScrewView view)
        {
            if (context.IsFirstClicked && context.levelIndex == 1)
            {
                context.IsFirstClicked = false;
                //迁移报错注释
                //BIHelper.SendGameEvent(BiEventScrewscapes.Types.GameEventType.GameEventFunnelLevel1Click);
            }

            if (IsScrewDownValid(model))
            {
                _members[model.ScrewShape].HandleScrewDown(model, view);
            }
        }
        public void HandleScrewUp(ScrewModel model, ScrewView view)
        {
            if (IsScrewUpValid(model))
            {
                _members[model.ScrewShape].HandleScrewUp(model, view);
            }
        }

        public bool IsScrewDownValid(ScrewModel model)
        {
            return _members.ContainsKey(model.ScrewShape) && _members[model.ScrewShape].IsScrewDownValid(model);
        }

        public bool IsScrewUpValid(ScrewModel model)
        {
            if (_members.ContainsKey(model.ScrewShape))
                return _members[model.ScrewShape].IsScrewUpValid(model);

            return false;
        }

        public int GetExecuteOrder()
        {
            return ExecuteOrder.MemberHandlerOrder;
        }

        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams)
        {
            switch (logicEvent)
            {
                case LogicEvent.PreMove:
                    OnPreMove(((MoveParams) eventParams).moveAction);
                    break;
                case LogicEvent.PostMove:
                    OnPostMove(((MoveParams) eventParams).moveAction);
                    break;
                case LogicEvent.PrePairMove:
                    OnPrePairMove(((PairMoveParams) eventParams).moveAction);
                    break;
                case LogicEvent.PostPairMove:
                    OnPostPairMove(((PairMoveParams) eventParams).moveAction);
                    break;
                case LogicEvent.BlockCheckFail:
                    CheckLevelFailed();
                    break;
            }
        }
        
        private void OnPreMove(MoveAction moveAction)
        {
            if (_members.ContainsKey(moveAction.Model.ScrewShape))
            {
                _members[moveAction.Model.ScrewShape].OnPreMove(moveAction);
            }
        }

        private void OnPostMove(MoveAction moveAction)
        {
            if (_members.ContainsKey(moveAction.Model.ScrewShape))
            {
                _members[moveAction.Model.ScrewShape].OnPostMove(moveAction);
            }
        }

        private void OnPrePairMove(PairMoveAction moveAction)
        {
            // 执行最后一个Model
            if (_members.ContainsKey(moveAction.Models[moveAction.Models.Count - 1].ScrewShape))
            {
                _members[moveAction.Models[moveAction.Models.Count - 1].ScrewShape].OnPrePairMove(moveAction);
            }
        }

        private void OnPostPairMove(PairMoveAction moveAction)
        {
            // 执行最后一个Model
            if (_members.ContainsKey(moveAction.Models[moveAction.Models.Count - 1].ScrewShape))
            {
                _members[moveAction.Models[moveAction.Models.Count - 1].ScrewShape].OnPostPairMove(moveAction);
            }
        }

        private void CheckLevelFailed()
        {
            foreach (var member in _members.Values)
            {
                member.CheckLevelFailed();
            }
        }
    }
}