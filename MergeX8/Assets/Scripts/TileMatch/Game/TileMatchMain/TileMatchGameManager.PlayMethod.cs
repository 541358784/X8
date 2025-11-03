using System.Collections.Generic;
using TileMatch.Game.PlayMethod;
using UnityEngine;

namespace TileMatch.Game
{
    public partial class TileMatchGameManager
    {
        private List<IPlayMethod> _playMethods = new List<IPlayMethod>();
        private List<Block.Block> _frogBlocks = new List<Block.Block>();
        private List<Block.Block> _purdahBlocks = new List<Block.Block>();

        private void InitPlayMethod()
        {
            InitTimePlayMethod();
            InitFrogPlayMethod();
            InitPurdahPlayMethod();
            
            
            IPlayMethod playMethod = new PlanePlayMethod();
            playMethod.Init();
            _playMethods.Add(playMethod);
        }

        private void InitTimePlayMethod()
        {
            int time = GetLayoutConfig(GameConst.TimeKey);
            if(time <= 0)
                return;

            IPlayMethod playMethod = new TimeLimitPlayMethod();
            playMethod.Init(time);
            
            _playMethods.Add(playMethod);
        }

        private void InitFrogPlayMethod()
        {
            if(_frogBlocks == null || _frogBlocks.Count == 0)
                return;
            
            IPlayMethod playMethod = new FrogPlayMethod();
            playMethod.Init(_frogBlocks);
            
            _playMethods.Add(playMethod);
        }

        private void InitPurdahPlayMethod()
        {
            if(_purdahBlocks == null || _purdahBlocks.Count == 0)
                return;
            
            IPlayMethod playMethod = new PurdahPlayMethod();
            playMethod.Init(_purdahBlocks);
            
            _playMethods.Add(playMethod);
        }
        public void AddFrogBlock(Block.Block block)
        {
            _frogBlocks.Add(block);
        }
        
        public void AddPurdahBlock(Block.Block block)
        {
            _purdahBlocks.Add(block);
        }
        
        private void StartPlayMethod()
        {
            _playMethods.ForEach(a=>a.Start());
        }

        private void PausePlayMethod()
        {
            _playMethods.ForEach(a=>a.Pause());
        }

        private void RecoverPlayMethod()
        {
            _playMethods.ForEach(a=>a.Recover());
        }
        private void UpdatePlayMethod()
        {
            _playMethods.ForEach(a=>a.Update());
        }
        
        private void DestroyPlayMethod()
        {
            _playMethods.ForEach(a=>a.Destroy());
            _playMethods.Clear();
            _frogBlocks.Clear();
            _purdahBlocks.Clear();
        }

        private void ClearPlayMethod()
        {
            DestroyPlayMethod();
        }

        private void ClearPlayMethodRecord()
        {
            _playMethods.ForEach(a=>a.CleanRecord());
        }

        private void PlayMethod_BeforeRemoveBlock(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;
            
            _playMethods.ForEach(a=>a.BeforeRemoveBlock(blocks));
        }

        private void PlayMethod_AfterRemoveBlock(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;
            
            _playMethods.ForEach(a=>a.AfterRemoveBlock(blocks));
        }

        private void PlayMethod_BeforeRecoverBlock(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;
            
            _playMethods.ForEach(a=>a.BeforeRecoverBlock(blocks, null));
        }

        private void PlayMethod_AfterRecoverBlock(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;
            
            _playMethods.ForEach(a=>a.AfterRecoverBlock(blocks));
        }

        private void PlayMethod_Magic_BeforeRemoveBlock(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;
            
            _playMethods.ForEach(a=>a.Magic_BeforeRemoveBlock(blocks));
        }
           
        private void PlayMethod_Magic_AfterRemoveBlock(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;
            
            _playMethods.ForEach(a=>a.Magic_AfterRemoveBlock(blocks));
        }

        public void StartShuffle()
        {
            _playMethods.ForEach(a=>a.StartShuffle());
        }

        public void StopShuffle()
        {
            _playMethods.ForEach(a=>a.StopShuffle());
        }
        private int GetTime()
        {
            foreach (var playMethod in _playMethods)
            {
                if (playMethod is TimeLimitPlayMethod)
                    return playMethod.GetTime();
            }

            return 0;
        }

        private int GetLeftTime()
        {
            foreach (var playMethod in _playMethods)
            {
                if (playMethod is TimeLimitPlayMethod)
                    return playMethod.GetLeftTime();
            }

            return 0;
        }
        
        private void AddTime(int time)
        {
            foreach (var playMethod in _playMethods)
            {
                if (playMethod is TimeLimitPlayMethod)
                    playMethod.AddTime(time);
            }
        }

        public bool IsLock(Block.Block block)
        {
            foreach (var playMethod in _playMethods)
            {
                if (playMethod is PurdahPlayMethod)
                    return playMethod.IsLock(block);
            }

            return false;
        }
        public void PlayMethod_DisappearBlock(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;
            
            _playMethods.ForEach(a=>a.DisappearBlock(blocks));
        }
        
        public IPlayMethod GetPlayMethod<T>() where T : IPlayMethod
        {
            foreach (var playMethod in _playMethods)
            {
                if (playMethod is T)
                    return playMethod;
            }

            return null;
        }
    }
}