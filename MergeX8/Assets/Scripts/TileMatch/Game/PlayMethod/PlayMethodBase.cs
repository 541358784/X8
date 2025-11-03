using System;
using System.Collections.Generic;

namespace TileMatch.Game.PlayMethod
{
    public abstract class PlayMethodBase : IPlayMethod
    {
        public abstract void Init(params object[] param);

        public virtual void Start(){}

        public virtual void Pause(){}
        
        public virtual void Destroy(){}

        
        public virtual void BeforeRemoveBlock(List<Block.Block> blocks, bool isRefresh = true){}

        public virtual void AfterRemoveBlock(List<Block.Block> blocks){}

        public virtual void BeforeRecoverBlock(List<Block.Block> blocks, Action action){}

        public virtual void AfterRecoverBlock(List<Block.Block> blocks){}

        public virtual void DisappearBlock(List<Block.Block> blocks, bool isMagic = false){ }

        public virtual void Magic_BeforeRemoveBlock(List<Block.Block> blocks){}

        public virtual void Magic_AfterRemoveBlock(List<Block.Block> blocks){}


        public virtual void Recover() {}

        public virtual void Update(){}

        public virtual void CleanRecord(){}

        public virtual int GetTime(){return 0;}

        public virtual int GetLeftTime(){return 0;}

        public virtual void AddTime(int time){}

        public virtual void Hide(){}

        public virtual void Show(){ }

        public virtual void StartShuffle(){}

        public virtual void StopShuffle(){}

        public virtual bool IsLock(Block.Block block)
        {
            return false;
        }
    }
}