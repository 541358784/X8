using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using Framework;
using LayoutData;
using UnityEngine;

namespace TileMatch.Game.Block
{
    public class BockFunnel : Block
    {
        private Block _linkBlock;
        
        private BlockFunnelModel _selfModel;
        private BlockFunnelView _selfView;
        public BockFunnel(Layer.Layer layer, LayerBlock blockData, int index): base(layer, blockData, index){}
        public override void InitData(Layer.Layer layer, LayerBlock blockData)
        {
            _blockModel = new BlockFunnelModel(layer, blockData, this);
            _blockView = new BlockFunnelView(this);

            _selfModel = (BlockFunnelModel)_blockModel;
            _selfView = (BlockFunnelView)_blockView;
        }
        
        private Block LinkBlock
        {
            get
            {
                if (_linkBlock != null)
                    return _linkBlock;
            
                var neighbors = _blockModel._blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Down);
                var neighborsBlock = TileMatchGameManager.Instance.GetBlock(neighbors.id);

                _linkBlock = neighborsBlock;
                return _linkBlock;
            }
        }

        public override void RefreshView(bool isAnim = false)
        {
            base.RefreshView(isAnim);
            
            _selfView.RefreshView(_blockState);
        }
        
        public override void BeforeRemoveBlock(List<Block> blocks, bool isRefresh = true)
        {
            base.BeforeRemoveBlock(blocks, isRefresh);

            _selfView.RefreshView(_blockState);
            
            if(_blockState != BlockState.Normal && _blockState != BlockState.Hided)
                return;
            
            foreach (var block in blocks)
            {
                if(block.AreaType == AreaType.SuperBanner)
                    continue;
                
                var neighbors = _blockModel._blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Down);
                
                if (!block._blockModel._blockData.parent.Contains(_id) && !block._blockModel._blockData.parent.Contains(LinkBlock._id) && !_selfModel._newBlocks.Contains(block) && neighbors.id != block._id) 
                    continue;
                
                if (_selfModel._residueCount <= 0)
                {
                    if (_selfModel._newBlocks[_selfModel._index - 1] == block)
                    {
                        if (LinkBlock.BlockId == GameConst.NOVAILDID)
                        {
                            LinkBlock.SetBlockState(BlockState.Hided);
                            TileMatchGameManager.Instance.BeforeRemoveBlock(new List<Block>(){LinkBlock});
                        }
                    }
                    
                    continue;
                }
              
                if (LinkBlock.BlockId == GameConst.NOVAILDID)
                {
                    if (LinkBlock.GetBlockState() == BlockState.Inactive)
                    {
                        LinkBlock.SetBlockState(BlockState.Normal);
                        RefreshParentState();
                        CreateTileBlock();
                    }
                    else if(_selfModel._newBlocks.Count > 0 && _selfModel._newBlocks[_selfModel._newBlocks.Count-1] == block)
                    {
                        CreateTileBlock();
                    }
                }
                else if (!LinkBlock.IsInActiveState())
                {
                    if(_selfModel._newBlocks.Count > 0 && !_selfModel._newBlocks.Contains(block))
                        continue;
                  
                    if (_selfModel._index == 0)
                    {
                        CreateTileBlock();
                        RefreshParentState();
                    } 
                    else
                    {
                        if (_selfModel._newBlocks.Count > 0)
                        {
                            if (_selfModel._newBlocks[_selfModel._index-1] == block)
                                CreateTileBlock();
                        }
                    }
                }
            }
        }
        
        public override void BeforeRecoverBlock(List<Block> blocks, Action action)
        {
            RestBlockCollider(blocks);
            
            if (_blockState != BlockState.Normal && _blockState != BlockState.Hided)
            {
                action?.Invoke();
                return;
            }

            if (_selfModel._index == 0 || _selfModel._newBlocks.Count == 0)
            {
                action?.Invoke();
                return;
            }

            bool isSpawn = false;
            
            foreach (var block in blocks)
            {
                if(block.AreaType == AreaType.SuperBanner)
                    continue;
                
                var neighbors = _blockModel._blockData.neighbors.Find(a => a.neighborType == (int)NeighborEnum.Down);
                
                if (!block._blockModel._blockData.parent.Contains(_id) && !block._blockModel._blockData.parent.Contains(LinkBlock._id) && !_selfModel._newBlocks.Contains(block) && neighbors.id != block._id) 
                    continue;
                
                int removeIndex = -1;
                Block removeBlock = null;
                
                if (LinkBlock.BlockId == GameConst.NOVAILDID && _selfModel._index == 1 && _selfModel._blockData.children.Contains(block._id))
                {
                    removeIndex = 0;
                    LinkBlock.SetBlockState(BlockState.Inactive);
                }
                else
                {
                    if (_selfModel._index == 1 && _selfModel._blockData.children.Contains(block._id))
                    {
                        removeIndex = 0;
                    }
                    else
                    {
                        if(!_selfModel._newBlocks.Contains(block) && LinkBlock != block)
                            continue;
            
                        if (LinkBlock.BlockId != GameConst.NOVAILDID)
                        {
                            if (LinkBlock == block)
                                removeIndex = 0;
                        }

                        if (removeIndex < 0)
                        {
                            removeIndex = _selfModel._newBlocks.FindIndex(a => a == block);
                            if(removeIndex < 0)
                                continue;
                
                            removeIndex += 1;
                            if(removeIndex < 0 || removeIndex >= _selfModel._newBlocks.Count)
                                continue;
                        }
            
                        if (_blockState == BlockState.Hided)
                        {
                            RefreshParentState(); 
                            
                            _blockView._root.gameObject.SetActive(true);
                            _selfView.PlayAnimation("Respawn");

                            _blockState = BlockState.Normal;
                            TileMatchGameManager.Instance.AfterRecoverBlock(new List<Block>(){this});
                            
                            removeBlock = GetRemoveBlock(removeIndex);
                            RemoveBlock(removeBlock);
                            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.3f, () =>
                            {
                                removeBlock.SetState( BlockState.Normal);
                                TileMatchGameManager.Instance.AfterRecoverBlock(new List<Block>(){this});
                                RefreshParentState();
                                ReSpawn(removeBlock, action);
                            }));
                            return;
                        }
                    }
                }

                isSpawn = true;
                removeBlock = GetRemoveBlock(removeIndex);
                RemoveBlock(removeBlock);
                ReSpawn(removeBlock, action);
            }
            
            if(isSpawn)
                return;
            
            action?.Invoke();
        }

        private Block GetRemoveBlock(int index)
        {
            if (index < 0 || index >= _selfModel._newBlocks.Count)
                return null;
            
            return _selfModel._newBlocks[index];
        }

        private void RemoveBlock(Block removeBlock)
        {
            if(removeBlock == null)
                return;
            
            _selfModel._newBlocks.Remove(removeBlock);
            _selfModel._index = Math.Max(0, _selfModel._index-1);
        }
        
        private void ReSpawn(Block removeBlock, Action action)
        {
            if (removeBlock == null)
            {
                action?.Invoke();
                return;
            }

            float z = _selfView.GetSpawnZ();
            
            _selfView.PlayAnimation("TileMakerEat");

            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.2f, () =>
            {
                removeBlock._blockView._root.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.1f);
                removeBlock.MoveTo(new Vector3(_blockModel.position.x, _blockModel.position.y, z), speed:5, action:(b) =>
                {
                    removeBlock.Destroy();
                    _selfView.RefreshView();
                    action?.Invoke();
                    TileMatchGameManager.Instance.RemoveAppendBlock(removeBlock);
                });
            }));
        }
        public override void OnPointerExit(bool isRemove, Action action)
        {
            OnPointerExitAnim();
        }

        public override void OnPointerEnter()
        {
            if (_blockState != BlockState.Normal)
                return;

            Shake();
        }
        
        private void CreateTileBlock()
        {
            int index = LinkBlock._index;
            
            var blockData = new LayerBlock(LinkBlock._blockModel._blockData);
            blockData.blockId = ((BlockFunnelModel)_blockModel).GetNewBlockId();
            blockData.blockType = 0;
            
            var block = new Block(_layer, blockData, index+1);
            block.LoadView(_blockView._parent.transform);
            block.InitState();
            block.SetState(BlockState.Normal);
            
            _selfView.PlayAnimation("PrinterAnim");

            TileMatchGameManager.Instance.RegisterAppendBlock(block);
            TileMatchGameManager.Instance.AfterRecoverBlock(new List<Block>(){block});

            AudioManager.Instance.PlaySound(26+TileMatchRoot.AudioDistance);
            _selfView.RefreshView();
            float z = _selfView.GetSpawnZ();
            block._blockView.SetColliderEnable(false);
            block._blockView._root.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            block._blockView._root.transform.position = new Vector3(_blockModel.position.x, _blockModel.position.y, z);
            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.05f, () =>
            {
                block.MoveTo(new Vector3(block._blockModel.position.x, block._blockModel.position.y, z), speed:15, action:(b) =>
                {
                    block._blockView._root.transform.position = block._blockModel.position;
                    block.InitState();
                    block.SetState(block.GetBlockState(), isAnim:true);
                });
            }));
            
            _selfModel._newBlocks.Add(block);
            
            if (_selfModel._residueCount <= 0)
            {
                _blockState = BlockState.Hided;
                TileMatchGameManager.Instance.BeforeRemoveBlock(new List<Block>(){this});
                
                _selfView.PlayAnimation("PrinterAnimBlast", () =>
                {
                    _blockView._root.gameObject.SetActive(false);
                });
            }
        }

        private void RefreshParentState()
        {
            if (LinkBlock._blockModel._blockData.parent == null)
                return;
            
            foreach (var parentId in LinkBlock._blockModel._blockData.parent)
            {
                var childBlock = TileMatchGameManager.Instance.GetBlock(parentId);
                childBlock.AfterRecoverBlock(new List<Block>(){LinkBlock});
            }
        }
        
        public override bool CanShuffle()
        {
            return false;
        }
        
        public override int GetBlockNum()
        {
            return _selfModel.BlockNum();
        }
        
        public override List<Block> PreparationPreprocess()
        {
            return null;
        }
    }
}