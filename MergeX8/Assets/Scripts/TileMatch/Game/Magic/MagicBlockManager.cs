using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using GamePool;
using SomeWhere;
using SomeWhereTileMatch;
using TileMatch.Game.Block;
using UnityEngine;

namespace TileMatch.Game.Magic
{
    public class MagicBlockManager : Manager<MagicBlockManager>
    {
        Dictionary<int, List<Block.Block>> _blackBlocks = new Dictionary<int, List<Block.Block>>();
        Dictionary<int, List<Block.Block>> _brightBlocks = new Dictionary<int, List<Block.Block>>();
        Dictionary<int, List<Block.Block>> _collectBannerBlocks = new Dictionary<int, List<Block.Block>>();
        List<int> _blockIds = new List<int>();

        private List<Vector3> _movePosition = new List<Vector3>()
        {
            new Vector3(0, GameConst.BlockHeight/100f+2, -20f),
            new Vector3(-GameConst.BlockWidth/100f,2 ,-20f),
            new Vector3(GameConst.BlockWidth/100f, 2,-20f),
        };

        private List<string> _poolNames = new List<string>()
        {
            "TileViewMid",
            "TileViewLeft",
            "TileViewRight",
        };

        private bool _isMagic = false;

        public bool IsMagic
        {
            get { return _isMagic; }
        }
        
        public void Magic(List<Block.Block> blocks, List<Block.Block> bannerBlocks)
        {
            int selectBlockId = GetSelectBlockId(blocks, bannerBlocks);
            if(selectBlockId < 0)
                return;

            AudioManager.Instance.PlaySound(24+TileMatchRoot.AudioDistance);
            _isMagic = true;
            
            List<Block.Block> selectBlocks = new List<Block.Block>();
            if(_collectBannerBlocks.ContainsKey(selectBlockId))
                selectBlocks.AddRange(_collectBannerBlocks[selectBlockId]);

            int residueNum = 3 - selectBlocks.Count;
            if (residueNum > 0)
            {
                var randomBlocks = GetRandomBlocks(_blackBlocks, selectBlockId, residueNum);
                if(randomBlocks != null && randomBlocks.Count > 0)
                    selectBlocks.AddRange(randomBlocks);
                
                residueNum = 3 - selectBlocks.Count;
                if (residueNum > 0)
                {
                    randomBlocks = GetRandomBlocks(_brightBlocks, selectBlockId, residueNum);
                    if(randomBlocks != null && randomBlocks.Count > 0)
                        selectBlocks.AddRange(randomBlocks);
                }
            }
            
            StartCoroutine(PlayMagicAnim(selectBlocks));
        }
        
        public bool CanUseMagic(List<Block.Block> blocks, List<Block.Block> bannerBlocks)
        {
            if (IsMagic)
                return false;
            
            return GetSelectBlockId(blocks, bannerBlocks) > 0;
        }
        
        private int GetSelectBlockId(List<Block.Block> blocks, List<Block.Block> bannerBlocks)
        {
            InitBlocksData(blocks, bannerBlocks);
            FilterInvalid();

            int selectBlockId = GetRemoveBlockIdByBanner();
            if (selectBlockId < 0)
            {
                selectBlockId = GetRemoveBlockIdByBlock();
            }

            return selectBlockId;
        }
        
        private void InitBlocksData(List<Block.Block> blocks, List<Block.Block> bannerBlocks)
        {
            _blackBlocks.Clear();
            _brightBlocks.Clear();
            _collectBannerBlocks.Clear();
            _blockIds.Clear();
            
            foreach (var block in blocks)
            {
                if(!block.IsValidState())
                    continue;

                if(block._blockModel._blockData.blockType == (int)BlockTypeEnum.Funnel || block._blockModel._blockData.blockType == (int)BlockTypeEnum.Purdah || block._blockModel._blockData.id == GameConst.NOVAILDID)
                    continue;
                
                if(TileMatchGameManager.Instance.IsLock(block))
                    continue;
                
                if (block.GetBlockState() == BlockState.Black || block.GetBlockState() == BlockState.Overlap)
                {
                    if (!_blackBlocks.ContainsKey(block.BlockId))
                        _blackBlocks[block.BlockId] = new List<Block.Block>();
                    
                    _blackBlocks[block.BlockId].Add(block);
                }
                
                if (block.GetBlockState() == BlockState.Normal || block.GetBlockState() == BlockState.InCollection || block.GetBlockState() == BlockState.InCollectionOverlap)
                {
                    if (!_brightBlocks.ContainsKey(block.BlockId))
                        _brightBlocks[block.BlockId] = new List<Block.Block>();
                    
                    _brightBlocks[block.BlockId].Add(block);
                }
                
                if(!_blockIds.Contains(block.BlockId))
                    _blockIds.Add(block.BlockId);
            }

            foreach (var block in bannerBlocks)
            {
                if (!_collectBannerBlocks.ContainsKey(block.BlockId))
                    _collectBannerBlocks[block.BlockId] = new List<Block.Block>();
                
                _collectBannerBlocks[block.BlockId].Add(block);
                
                if(!_blockIds.Contains(block.BlockId))
                    _blockIds.Add(block.BlockId);
            }
        }

        private void FilterInvalid()
        {
            for (int i = 0; i < _blockIds.Count; i++)
            {
                int blockId = _blockIds[i];
                
                int num = 0;
                num += GetBlockNum(_collectBannerBlocks, blockId);
                num += GetBlockNum(_blackBlocks, blockId);
                num += GetBlockNum(_brightBlocks, blockId);
                
                if(num >= 3)
                    continue;
                
                _blockIds.RemoveAt(i);
                i--;

                RemoveBlock(_collectBannerBlocks, blockId);
                RemoveBlock(_blackBlocks, blockId);
                RemoveBlock(_brightBlocks, blockId);
            }
        }

        private int GetRemoveBlockIdByBanner()
        {
            if (_collectBannerBlocks.Count == 0)
                return -1;

            List<int> oneNumBlock = new List<int>();
            List<int> twoNumBlock = new List<int>();
            
            foreach (var kv in _collectBannerBlocks)
            {
                if(kv.Value.Count == 1)
                    oneNumBlock.Add(kv.Key);
                else
                    twoNumBlock.Add(kv.Key);
            }

            if (oneNumBlock.Count > 0)
                return oneNumBlock.RandomPickOne();

            if (twoNumBlock.Count > 0)
                return twoNumBlock.RandomPickOne();

            return -1;
        }

        private int GetRemoveBlockIdByBlock()
        {
            return _blockIds.RandomPickOne();
        }

        private List<Block.Block> GetRandomBlocks(Dictionary<int, List<Block.Block>> block, int blockId, int num)
        {
            if (!block.ContainsKey(blockId))
                return null;

            if (block[blockId].Count <= num)
                return block[blockId];

            List<Block.Block> randomBlocks = new List<Block.Block>();
            for (int i = 0; i < num; i++)
            {
                randomBlocks.Add(block[blockId].RandomPickOne());
                block[blockId].Remove(randomBlocks[randomBlocks.Count - 1]);
            }

            return randomBlocks;
        }
        private int GetBlockNum(Dictionary<int, List<Block.Block>> block, int blockId)
        {
            if (block.ContainsKey(blockId))
                return block[blockId].Count;

            return 0;
        }
        
        private void RemoveBlock(Dictionary<int, List<Block.Block>> block, int blockId)
        {
            if (!block.ContainsKey(blockId))
                return;

            block.Remove(blockId);
        }
        
        private IEnumerator PlayMagicAnim(List<Block.Block> blocks)
        {
            var magnet = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_AnimationMagnet);
            CommonUtils.AddChild(TileMatchRoot.Instance.transform, magnet.transform);
            StartCoroutine(CommonUtils.DelayWork(3, ()=>
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_AnimationMagnet, magnet);
            }));
            yield return new WaitForSeconds(1f);
            
            foreach (var block in blocks)
            {
                block.SetState(BlockState.Hided, false);
            }
            
            TileMatchGameManager.Instance.Magic_BeforeRemoveBlock(blocks);

            foreach (var block in blocks)
            {
                block.StartMagic();
            }

            float animTime = 0.5f;
            for (int i = 0; i < blocks.Count; i++)
            {
                Vector3 movePos = _movePosition[i];
                Vector3 orgPos = blocks[i]._blockView._root.transform.position;

                blocks[i]._blockView._root.transform.position = orgPos;
                blocks[i]._blockView._root.transform.DOMove(movePos, animTime);
            }

            yield return new WaitForSeconds(animTime);
            var animatorPoolObj = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_AnimationPool);
            for (int i = 0; i < blocks.Count; i++)
            {
                int index = i;
                StartCoroutine(CommonUtils.DelayWork(3, ()=>
                {
                    GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_AnimationPool, animatorPoolObj);
                }));
                
                CommonUtils.AddChild(TileMatchRoot.Instance.transform, animatorPoolObj.transform);

                var tileParent = animatorPoolObj.transform.Find(_poolNames[i]);
                tileParent.transform.position = _movePosition[i];
                CommonUtils.AddChild(tileParent.transform.Find("Parent"),blocks[i]._blockView._root.transform);
            }
            
            yield return new WaitForSeconds(1f);
            
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i]._blockView._root.SetActive(false);
            }
            
            foreach (var block in blocks)
                block.SetState(BlockState.Hided, false);
            
            TileMatchGameManager.Instance.Magic_AfterRemoveBlock(blocks);
            _isMagic = false;
        }

        public void CleanMagic()
        {
            StopAllCoroutines();
            _blackBlocks.Clear();
            _brightBlocks.Clear();
            _collectBannerBlocks.Clear();
            _blockIds.Clear();
            _isMagic = false;
        }
    }
}