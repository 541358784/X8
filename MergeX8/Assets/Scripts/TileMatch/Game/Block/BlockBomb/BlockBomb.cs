using System;
using System.Collections.Generic;
using LayoutData;
using TileMatch.Event;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BlockBomb : Block
    {
        private BlockBombModel _selfModel;
        private BlockBombView _selfView;

        private int _preprocessBrokenNum = 0;
        public BlockBomb(Layer.Layer layer, LayerBlock blockData, int index) : base(layer, blockData, index){}

        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockBombModel(layer, blockData, this);
            _blockView = new BlockBombView(this);
            
            _selfModel = (BlockBombModel)_blockModel;
            _selfView = (BlockBombView)_blockView;
        }

        public override void SetState(BlockState blockState, bool isRefresh = true, bool isAnim = false)
        {
            base.SetState(blockState, isRefresh, isAnim);
            
            _selfView.UpdateView(GetBlockState());
        }

        public override void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
            base.BeforeRemoveBlock(blocks, isRefresh);
            
            if(_blockState != BlockState.Normal)
                return;

            if (_selfModel.IsCollect)
                return;
            
            foreach (var block in blocks)
            {
                if(IgnoreRemoveBlockHandle(block))
                    continue;

                if (block.AreaType != AreaType.SuperBanner)
                {
                    bool isChildern = false;
                    if (_blockModel._blockData.children != null)
                    {
                        foreach (var block1 in blocks)
                        {
                            if (_blockModel._blockData.children.Contains(block1._id))
                            {
                                isChildern = true;
                                break;
                            }
                        }
                    }
                    
                    if(isChildern)
                        continue;
                }

                if(_selfModel._brokenNum <= 0)
                    return;
                
                _selfModel._brokenNum--;
                _selfView.UpdateView();

                _isCanRemove = _selfModel._brokenNum > 0;

                if (_selfModel._brokenNum == 0)
                    UIRoot.Instance.EnableEventSystem = false;
                break;
            }
        }

        public override void AfterRemoveBlock(List<Block> blocks)
        {
            if(_blockState != BlockState.Normal)
                return;
            
            if(_blockModel.IsCollect)
                return;
            
            if(_selfModel._brokenNum > 0)
                return;
            
            _selfView.Bomb(() =>
            {
                TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_Fail,FailTypeEnum.Special,BlockTypeEnum.Bomb);
            });
        }
        
        public override void BeforeRecoverBlock(List<Block> blocks, Action action)
        {
            RestBlockCollider(blocks);
            if (_selfModel.IsCollect)
            {
                action?.Invoke();
                return;
            }

            if (_selfModel._brokenNum > 0)
            {
                if (GetBlockState() == BlockState.Normal)
                {
                    _selfModel._brokenNum++;
                    _preprocessBrokenNum--;
                    _preprocessBrokenNum = Math.Max(_preprocessBrokenNum, 0);
                    _selfModel._brokenNum = Math.Min(_selfModel._brokenNum, _selfModel._initBrokenNum);
                    
                    _selfView.UpdateView();
                }
                
                action?.Invoke();
                return;
            }

            _blockModel.IsCollect = true;
            _selfView.ShowIcon();
            SetState(BlockState.Normal);
            //_selfView.HideBomb();
            _isCanRemove = true;
            action?.Invoke();
        }
        
        public override void RemoveAnim(Action action)
        {
            if (_selfModel.IsCollect)
            {
                base.RemoveAnim(action);
                return;
            }
            
            base.RemoveAnim(action);
            _selfView.HideBomb();
        }
        
        public override void StartShuffle()
        {
            base.StartShuffle();
            
            if(_selfModel.IsCollect)
                return;
            
            _selfView.StartShuffle();
        }

        public override void StopShuffle()
        {
            base.StopShuffle();
            
            if(_selfModel.IsCollect)
                return;
            
            _selfView.StopShuffle();
        }
        
        public override void Magic_BeforeRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                base.BeforeRemoveBlock(new List<Block>(){block});
            }
        }

        public override void Magic_AfterRemoveBlock(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                base.AfterRemoveBlock(new List<Block>(){block});
            }
        }
        
        public override void StartMagic()
        {  
            base.StartMagic();
            
            if(_selfModel.IsCollect)
                return;

            _selfView.HideBomb();
        }

        public override void ExecutePreprocess(List<Block> blocks)
        {
            if (_blockState != BlockState.Normal)
                return;
            
            if (_blockModel.IsCollect)
                return;

            foreach (var block in blocks)
            {
                if (IgnoreRemoveBlockHandle(block))
                    continue;
                
                if (block == this)
                    continue;

                _preprocessBrokenNum++;
                break;
            }
        }
        
        public override FailTypeEnum CheckFailure(List<Block> blocks)
        {
            if (_blockState != BlockState.Normal)
                return FailTypeEnum.None;

            if (_blockModel.IsCollect)
                return FailTypeEnum.None;

            foreach (var block in blocks)
            {
                if (block == this)
                    continue;

                if (_selfModel._initBrokenNum - _preprocessBrokenNum <= 0)
                    return FailTypeEnum.Special;
            }

            return FailTypeEnum.None;
        }
    }
}