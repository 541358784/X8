using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using TileMatch.Game.Block;
using UnityEngine;

namespace TileMatch.Game
{
    public partial class TileMatchGameManager
    {
        private List<List<Block.Block>> _superCollectBannerList = new List<List<Block.Block>>();
        private List<Vector3> _superCollectSeatPosition = new List<Vector3>();
        private const int maxStackNum = 4;
        private int _superStartIndex = 2;
        private int _extendSuperIndex = 0;
        
        public float GetSuperBannerOffsetY()
        {
            return _collectBannerWordPos.y + GameConst.Super_Banner_OffsetY;
        }
        
        private void InitSuperCollectSeatPosition()
        {
            for (int i = 0; i < GameConst.COLLECT_MAX_LENGHT; i++)
            {
                Vector3 seatPosition = new(_collectSeatPosition[i].x, _collectBannerWordPos.y + GameConst.Super_Banner_OffsetY, -1 - i * 0.01f);
                _superCollectSeatPosition.Add(seatPosition);
                
                _superCollectBannerList.Add(new List<Block.Block>());
            }
        }

        public void AddExtendSuperCollectBanner(Block.Block block, bool isBack = false, Action action = null)
        {
            if (_superStartIndex == 2)
            {
                _superStartIndex = 4;
                
                for(int i = _superCollectBannerList.Count-1; i >= _superStartIndex; i--)
                {
                    var tempList = _superCollectBannerList[i];
                    int swapIndex = _superStartIndex - (_superCollectBannerList.Count - 1 - i);
                    _superCollectBannerList[i] = _superCollectBannerList[swapIndex];
                    _superCollectBannerList[swapIndex] = tempList;
                }

                for (int i = _superCollectBannerList.Count - 1; i >= _superStartIndex; i--)
                {
                    RestCollectBlocksPosition(_superCollectBannerList[i], i, true);
                }
            }

            int index = -1;
            for (int i = 0; i < GameConst.SuperBackMaxLength; i++)
            {
                if (_superCollectBannerList[i].Count == 0)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                index = _extendSuperIndex++;
                _extendSuperIndex = _extendSuperIndex >= 3 ? 0 : _extendSuperIndex;
            }
            
            AddSuperCollectBanner(block, index, isBack, action, false);
        }
        
        private void AddSuperCollectBanner(Block.Block block, int index, bool isBack = false, Action action = null, bool isSuper = true)
        {
            int switchIndex = isSuper ? index + _superStartIndex : index;
            
            if(switchIndex < 0 || switchIndex >= _superCollectBannerList.Count)
                return;

            var collectList = _superCollectBannerList[switchIndex];
            
            block.SetAreaType(AreaType.SuperBanner);
            block._superBannerIndex = index;
            block._isSuperBanner = isSuper;
            
            int maxCount = Math.Min(collectList.Count, maxStackNum);//最多4层 其余重叠一起
            block._blockModel._convertPosition = GetSeatPosition(switchIndex);
            block._blockModel._convertPosition.y -= maxCount * 0.1f;
            block._blockModel._convertPosition.z -= collectList.Count * 0.2f;

            if (isBack)
            {
                block.BackMoveSuperBanner(block._blockModel._convertPosition, speed:_normalMoveSpeed, action:(b) =>
                {
                    UpdateBlockState(collectList);
                    block._blockView._root.transform.position = block._blockModel._convertPosition;
                    action?.Invoke();
                });
            }
            else
            {
                block.MoveSuperBanner(block._blockModel._convertPosition, speed:_normalMoveSpeed, action:(b) =>
                {
                    UpdateBlockState(collectList);
                    block._blockView._root.transform.position = block._blockModel._convertPosition;
                    action?.Invoke();
                });
            }
            collectList.Add(block);
        }

        private void RemoveSuperBlock(Block.Block block, bool isRestPosition = false)
        {
            for(int i = 0; i <_superCollectBannerList.Count; i++)
            {
                var blocks = _superCollectBannerList[i];
                if (!blocks.Contains(block))
                    continue;
                
                blocks.Remove(block);
                
                if(isRestPosition)
                    RestCollectBlocksPosition(blocks, i);
                
                UpdateBlockState(blocks);
                break;
            }
        }

        private void CleanSuperCollectBanner()
        {
            foreach (var blocks in _superCollectBannerList)
            {
                blocks.Clear();
            }
        }
        
        private void SuperCollectBannerDestroy()
        {
            CleanSuperCollectBanner();

            _superCollectBannerList = null;
        }
        
        private void UpdateBlockState(List<Block.Block> blocks)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (i == blocks.Count - 1)
                {
                    blocks[i].SetState(BlockState.InCollection);
                }
                else
                {
                    blocks[i].SetState(BlockState.InCollectionOverlap);
                }
            }
        }

        private void RestCollectBlocksPosition(List<Block.Block> blocks, int index, bool isAnim = false)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                Vector3 position = blocks[i]._blockView._root.transform.position;
                
                int maxCount = Math.Min(i, maxStackNum);//最多4层 其余重叠一起
                blocks[i]._blockModel._convertPosition = GetSeatPosition(index);
                blocks[i]._blockModel._convertPosition.y -= maxCount * 0.1f;
                blocks[i]._blockModel._convertPosition.z -= i * 0.2f;

                if(!isAnim)
                    blocks[i]._blockView._root.transform.position = blocks[i]._blockModel._convertPosition;
                else
                {
                    blocks[i]._blockView._root.transform.DOMove(blocks[i]._blockModel._convertPosition, 0.5f);
                }
            }
        }

        private Vector3 GetSeatPosition(int index)
        {
            return _superCollectSeatPosition[index];
        }
    }
}