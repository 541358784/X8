using System.Collections.Generic;

namespace Screw
{
    public class BlockersHandler
    {
        private Dictionary<ScrewBlocker, BlockerHandler> _blockerHandlers;
        
        private ScrewGameContext context;

        public BlockersHandler(ScrewGameContext inContext)
        {
            context = inContext;

            RegisterBlockers();
        }
        
        private void RegisterBlockers()
        {
            _blockerHandlers = new Dictionary<ScrewBlocker, BlockerHandler>();

            _blockerHandlers.Add(ScrewBlocker.ConnectBlocker, new ConnectBlockerHandler(context));
            _blockerHandlers.Add(ScrewBlocker.IceBlocker, new IceBlockerHandler(context));
            _blockerHandlers.Add(ScrewBlocker.ShutterBlocker, new ShutterBlockerHandler(context));
            _blockerHandlers.Add(ScrewBlocker.BombBlocker, new BombBlockerHandler(context));
            _blockerHandlers.Add(ScrewBlocker.LockBlocker, new LockBlockerHandler(context));
            _blockerHandlers.Add(ScrewBlocker.TieBlocker, new TieBlockerHandler(context));
        }

        public bool IsScrewDownValid(ScrewModel screwModel, List<ScrewBlocker> ignoreList = null)
        {
            var result = true;
            foreach (var handler in _blockerHandlers)
            {
                if (ignoreList == null)
                {
                    result &= handler.Value.IsScrewDownValid(screwModel);
                }
                else if(!ignoreList.Contains(handler.Key))
                {
                    result &= handler.Value.IsScrewDownValid(screwModel);
                }
            }

            return result;
        }
        
        public bool HandleScrewDown(ScrewModel screwModel, ScrewView view)
        {
            var result = false;
            foreach (var handler in _blockerHandlers)
            {
                result |= handler.Value.HandleScrewDown(screwModel, view);
            }

            return result;
        }

        public bool HandleScrewUp(ScrewModel screwModel, ScrewView view)
        {
            var result = false;
            foreach (var handler in _blockerHandlers)
            {
                result |= handler.Value.HandleScrewUp(screwModel, view);
            }

            return result;
        }
        
        public bool OnPreMove(MoveAction moveAction)
        {
            var result = false;
            foreach (var handler in _blockerHandlers)
            {
                result |= handler.Value.OnPreMove(moveAction);
            }

            return result;
        }
        
        public bool OnPostMove(MoveAction moveAction)
        {
            var result = false;
            foreach (var handler in _blockerHandlers)
            {
                result |= handler.Value.OnPostMove(moveAction);
            }

            return result;
        }

        public bool OnPrePairMove(PairMoveAction moveAction)
        {
            var result = false;
            foreach (var handler in _blockerHandlers)
            {
                result |= handler.Value.OnPrePairMove(moveAction);
            }

            return result;
        }
        
        public bool OnPostPairMove(PairMoveAction moveAction)
        {
            var result = false;
            foreach (var handler in _blockerHandlers)
            {
                result |= handler.Value.OnPostPairMove(moveAction);
            }

            return result;
        }
        
        public bool CompleteBlockerInRevive(ScrewModel screwModel)
        {
            if (screwModel.HasBlocker)
            {
                foreach (var handler in _blockerHandlers)
                {
                    foreach (var keyValue in screwModel.ScrewBlockers)
                    {
                        if (handler.Key == keyValue.Key)
                        {
                            return _blockerHandlers[keyValue.Key].CompleteBlockerInRevive(screwModel);
                        }
                    }
                }
            }

            return false;
        }

        public void CheckLevelFailed()
        {
            foreach (var handler in _blockerHandlers)
                handler.Value.CheckLevelFailed();
        }
    }
}