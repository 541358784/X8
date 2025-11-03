using System;
using System.Collections.Generic;
using DragonPlus.Config.TileMatch;
using LayoutData;
using TileMatch.Game.Block;
using UnityEngine;

namespace TileMatch.Game
{
    public partial class TileMatchGameManager : IBlockHandle, IMagicHandle
    {
        private int _layoutId = -1;
        private List<Layout.Layout> _layouts = new List<Layout.Layout>();
        private Dictionary<int, Block.Block> _blocks = new Dictionary<int, Block.Block>();
        private List<Block.Block> _allBlocks = new List<Block.Block>();
        private Dictionary<int, List<Block.Block>> _appendBlocks = new Dictionary<int, List<Block.Block>>();
        
        private LayoutGroup _layoutGroup;
        
        private Transform _layoutRoot;

        public Transform LayoutRoot
        {
            get { return _layoutRoot; }
        }

        private Vector2 _offset = Vector2.zero;

        public List<Block.Block> GetAllBlocks
        {
            get { return _allBlocks; }
        }
        public Vector2 Offset
        {
            get { return _offset; }
        }
        private void AwakeLayout()
        {
            _layoutRoot = transform.Find("Board/LayoutRoot").transform;
        }

        private void LayoutDestroy()
        {
            _layouts.ForEach(a=>a.Destroy());
            _layouts.Clear();
            _layouts = null;
        }
        
        private void LoadLayout()
        {
            TileMatchLevelModel.Instance.CollectCoin = 0;
            
            var levelConfig = TileMatchConfigManager.Instance.TileLevelList.Find(a => a.id == _levelId);
            _layoutId = levelConfig.layoutId;
            
            _layoutGroup = TileMatchLayoutManager.Instance.GetLayoutGroup(_layoutId);

            for (int i = 0; i < _layoutGroup.layout.Count; i++)
            {
                var layer = new Layout.Layout(_layoutGroup.layout[i], i+1);
                layer.LoadView(_layoutRoot);
                _layouts.Add(layer);
            }

            InitState();
            InitLayoutRootPosition();
            InitPlayMethod();
            InitCollectionBanner();

            LevelStartBi();
        }

        private void InitLayoutRootPosition()
        {
            _offset = Vector2.zero;
            _offset.x -= (_layoutGroup.gridCol-1) * GameConst.BlockWidth / 100f / 2f;
            _offset.y += (_layoutGroup.gridRow-1) * GameConst.BlockHeight / 100f / 2f;
            
            _offset += GameConst.GlobalOffset;

            _layoutRoot.transform.localPosition = _offset + _layoutShowPosition;
        }
        
        private void LayoutClean()
        {
            _layouts.ForEach(a=>a.Destroy());
            _layouts.Clear();
            
            _blocks.Clear();
            _appendBlocks.Clear();
            _allBlocks.Clear();

            _isBack = false;
            _isGameFail = false;
            _failedBlocks.Clear();
        }

        private void InitState()
        {
            _allBlocks.ForEach(a=>a.InitState());
        }

        public void BeforeRemoveBlock(List<Block.Block> blocks, bool isRefresh = true)
        {
            for(int i =  _allBlocks.Count-1; i >= 0; i--)
            {
                var block = _allBlocks[i];
                block.BeforeRemoveBlock(blocks);
            }
        }

        public void ExecutePreprocess(List<Block.Block> blocks)
        {
            for(int i = _allBlocks.Count-1; i >= 0; i--)
            {
                var block = _allBlocks[i];
                block.ExecutePreprocess(blocks);
            }
            
            for(int i = _allBlocks.Count-1; i >= 0; i--)
            {
                var block = _allBlocks[i];
                var failedType = block.CheckFailure(blocks);
                if(failedType == FailTypeEnum.None)
                    continue;

                if (_failedBlocks.ContainsKey(failedType))
                    continue;

                FailData failData = new FailData();
                failData._failBlock = block;
                failData._blocks.AddRange(blocks);
                
                _failedBlocks[failedType] = failData;
            }
        }

        public void AfterRemoveBlock(List<Block.Block> blocks)
        {
            PlayMethod_AfterRemoveBlock(blocks);

            for(int i =  _allBlocks.Count-1; i >= 0; i--)
            {
                var block = _allBlocks[i];
                block.AfterRemoveBlock(blocks);
            }
        }

        public void BeforeRecoverBlock(List<Block.Block> blocks, Action action)
        {
            int blockNum = 0;
            for(int i =  _allBlocks.Count-1; i >= 0; i--)
            {
                var block = _allBlocks[i];
                block.BeforeRecoverBlock(blocks, () =>
                {
                    blockNum++;
                    if (blockNum >= _allBlocks.Count)
                    {
                        action?.Invoke();
                        PlayMethod_BeforeRecoverBlock(blocks);
                    }
                });
            }
        }
        
        public void AfterRecoverBlock(List<Block.Block> blocks)
        {
            PlayMethod_AfterRecoverBlock(blocks);

            for(int i =  _allBlocks.Count-1; i >= 0; i--)
            {
                var block = _allBlocks[i];
                block.AfterRecoverBlock(blocks);
            }
        }

        public Block.Block GetBlock(Collider2D collider2D)
        {
            for(int i = 0; i < _allBlocks.Count; i++)
            {
                var block = _allBlocks[i];
                if(block.IsSameCollider(collider2D))
                    return block;
            }

            return null;
        }

        public void RegisterBlock(Block.Block block)
        {
            if(block == null)
                return;
            
            _blocks.Add(block._id, block);
            _allBlocks.Add(block);
        }
        
        public Block.Block GetBlock(int id)
        {
            if (_blocks.ContainsKey(id))
                return _blocks[id];

            return null;
        }
        
        public List<Block.Block> GetBlocks(int id)
        {
            return _allBlocks.FindAll(a => a._id == id);
        }
        public void RegisterAppendBlock(Block.Block block)
        {
            if(!_appendBlocks.ContainsKey(block._id))
                _appendBlocks.Add(block._id, new List<Block.Block>());
            
            _appendBlocks[block._id].Add(block);
            _allBlocks.Add(block);
        }

        public void RemoveAppendBlock(Block.Block block)
        {
            _allBlocks.Remove(block);
            
            List<Block.Block> blocks = GetAppendBlock(block._id);
            if(blocks == null)
                return;

            blocks.Remove(block);
        }
        
        public List<Block.Block> GetAppendBlock(int id)
        {
            if (_appendBlocks.ContainsKey(id))
                return _appendBlocks[id];

            return null;
        }

        public int GetLayoutConfig(string key)
        {
            if (_layoutGroup == null)
                return 0;

            if (!_layoutGroup.config.ContainsKey(key))
                return 0;
            
            return int.Parse(_layoutGroup.config[key]);
        }

        public List<Block.Block> GetActiveBlocks()
        {
            List<Block.Block> activeBlock = new List<Block.Block>();
            for(int i = 0; i < _allBlocks.Count; i++)
            {
                var block = _allBlocks[i];
                if (block.IsInActiveState())
                    activeBlock.Add(block);
            }

            return activeBlock;
        }

        public bool HaveActiveBlock()
        {
            for(int i = 0; i < _allBlocks.Count; i++)
            {
                var block = _allBlocks[i];
                if (block.IsValidState())
                    return true;
            }
            
            return false;
        }
        
        public List<Block.Block> GetCanRemoveBlocks(bool filterSuperCollect = false)
        {
            List<Block.Block> removeBlocks = new List<Block.Block>();
            for(int i = 0; i < _allBlocks.Count; i++)
            {
                var block = _allBlocks[i];
                if(IsLock(block))
                    continue;
                
                if (block.GetBlockState() == BlockState.Normal && block.CanRemove())
                {
                    if(filterSuperCollect && block.AreaType == AreaType.SuperBanner)
                        continue;
                        
                    removeBlocks.Add(block);
                }
            }

            return removeBlocks;
        }

        public void Magic_BeforeRemoveBlock(List<Block.Block> blocks)
        {    
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].GetAreaType() == AreaType.SuperBanner)
                {
                    RemoveSuperBlock(blocks[i], true);
                }
                RemoveBlock(blocks[i]);
            }
            
            for (int i = 0; i < _collectBannerList.Count; i++)
            {
                if (_flyBlockList.Contains(_collectBannerList[i]))
                {
                    _collectBannerList[i].DropMove(seatPosition[i], seatScale, _dropMoveSpeed);
                }
                else
                {
                    _collectBannerList[i].MoveTo(seatPosition[i], seatScale, _normalMoveSpeed);
                }
            }
            
            for(int i = 0; i < _allBlocks.Count; i++)
            {
                _allBlocks[i].Magic_BeforeRemoveBlock(blocks);
            }

            PlayMethod_Magic_BeforeRemoveBlock(blocks);
        }

        public void Magic_AfterRemoveBlock(List<Block.Block> blocks)
        { 
            for(int i = 0; i < _allBlocks.Count; i++)
            {
                var block = _allBlocks[i];
                block.Magic_AfterRemoveBlock(blocks);
            }

            PlayMethod_Magic_AfterRemoveBlock(blocks);

            if(!CheckGameSuccess())
                CheckGameFailure();
        }
        
        public void DisappearBlock(List<Block.Block> blocks, bool isMagic = false)
        {
        }
    }
}