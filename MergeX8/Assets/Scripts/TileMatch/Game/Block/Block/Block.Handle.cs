using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public partial class Block : IBlockHandle, IMagicHandle, IShake
    {
        //开始消除
        public virtual void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
            if(_blockState != BlockState.Overlap && _blockState != BlockState.Black)
                return;
            
            foreach (var block in blocks)
            {
                if(block.AreaType == AreaType.SuperBanner)
                    continue;
                
                if (!block._blockModel._blockData.parent.Contains(_id)) 
                    continue;

                if (!IsActiveBlock())
                    return;
                
                SetState(BlockState.Normal, isRefresh, isAnim:true);
            }
        }

        //消除完成
        public virtual void AfterRemoveBlock(List<Block> blocks){}
        
        //开始回退
        public virtual void BeforeRecoverBlock(List<Block> blocks, Action action)
        {
            RestBlockCollider(blocks);
            action?.Invoke();
        }

        public void RestBlockCollider(List<Block> blocks)
        {
            if(_blockState != BlockState.Normal)
                return;
            
            foreach (var block in blocks)
            {
                if(block.AreaType == AreaType.SuperBanner)
                    continue;
                
                if (!block._blockModel._blockData.parent.Contains(_id)) 
                    continue;
            
                _blockView.SetColliderEnable(false);
            }
        }
        
        //回退完成
        public virtual void AfterRecoverBlock(List<Block> blocks)
        {
            if(_blockState != BlockState.Normal)
                return;
            
            foreach (var block in blocks)
            {
                if(block.AreaType == AreaType.SuperBanner)
                    continue;
                
                if (!block._blockModel._blockData.parent.Contains(_id)) 
                    continue;
            
                if (_blockModel._layer != null && _blockModel._layer._layerModel._layoutData.isStackLayout)
                {
                    SetState(BlockState.Black, isAnim:true);
                }
                else
                {
                    SetState(BlockState.Overlap, isAnim:true);
                }
                
                return;
            }
            
            SetState(BlockState.Normal);
        }

        public virtual void Magic_BeforeRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                BeforeRemoveBlock(new List<Block>(){block});
            }
        }

        public virtual void Magic_AfterRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                AfterRemoveBlock(new List<Block>(){block});
            }
        }
        
        public virtual void DisappearBlock(List<Block> blocks, bool isMagic = false)
        {
        }
        
        public virtual void Shake()
        {
            ShakeTweener();
            _blockView.Shake();
        }
        
        public virtual Tweener ShakeTweener()
        {
            return _blockView._root.transform.DOShakePosition(0.2f, new Vector3(0.05f, 0.05f), 20);
        }
    }
}