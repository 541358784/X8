using System;
using System.Collections.Generic;
using DG.Tweening;
using SomeWhere;
using SomeWhereTileMatch;
using TileMatch.Game.Block;
using UnityEngine;

namespace TileMatch.Game.PlayMethod
{
    public class PlanePlayMethod : PlayMethodBase
    {
        private List<Block.Block> _lockBlocks = new List<Block.Block>();
        private List<GameObject> _flyPlane = new List<GameObject>();
        
        public override void Init(params object[] param)
        {
        }

        public override void DisappearBlock(List<Block.Block> blocks, bool isMagic = false)
        {
            List<Block.Block> planeBlocks = new List<Block.Block>();
            foreach (var block in blocks)
            {
                if(block._blockModel._blockData.blockType != (int)BlockTypeEnum.Plane)
                    continue;
                
                planeBlocks.Add(block);
            }
            
            if(planeBlocks.Count == 0)
                return;

            PlaneLogic(planeBlocks);
        }
        
        public override bool IsLock(Block.Block block)
        {
            return _lockBlocks.Contains(block);
        }

        private void PlaneLogic(List<Block.Block> planeBlocks)
        {
            var unLockBlocks = TileMatchGameManager.Instance.GetUnLockBlocks();
            if(unLockBlocks == null || unLockBlocks.Count == 0)
                return;
            
            List<Block.Block> blackBlocks = new List<Block.Block>();
            List<Block.Block> overlapBlocks = new List<Block.Block>();

            for (int i = unLockBlocks.Count - 1; i >= 0; i--)
            {
                var block = unLockBlocks[i];
                if(block._blockModel._blockData.blockType == (int)BlockTypeEnum.Funnel || block._blockModel._blockData.id == GameConst.NOVAILDID)
                    continue;

                if (block.GetBlockState() == BlockState.Black)
                {
                    blackBlocks.Add(block);
                }
                else  if (block.GetBlockState() == BlockState.Overlap)
                {
                    overlapBlocks.Add(block);
                }
            }
            
            if(blackBlocks.Count + overlapBlocks.Count == 0)
                return;
            
            foreach (var planeBlock in planeBlocks)
            {
                var  randomBlock = RandomBlock(blackBlocks, overlapBlocks);
                if(randomBlock == null)
                    continue;

                var flyPlane = GameObject.Instantiate(planeBlock._blockView._obstacle);
                CommonUtils.AddChild(TileMatchGameManager.Instance.LayoutRoot, flyPlane.transform);
                Vector3 movePosition = planeBlock._blockView._obstacle.transform.position;
                movePosition.z = -45;
                
                flyPlane.transform.position = movePosition;

                movePosition = randomBlock._blockModel.position;
                movePosition.z = -45;
                _flyPlane.Add(flyPlane);
                flyPlane.transform.transform.DOMove(movePosition, 0.5f).OnComplete(() =>
                {
                    GameObject.Destroy(flyPlane);
                    _flyPlane.Remove(flyPlane);
                    
                    randomBlock.StartMagic();
                    
                    randomBlock.SetState(BlockState.Hided, false);
                    TileMatchGameManager.Instance.Magic_BeforeRemoveBlock(new List<Block.Block>(){randomBlock});
                    TileMatchGameManager.Instance.AddExtendSuperCollectBanner(randomBlock, false, () =>
                    {
                        _lockBlocks.Remove(randomBlock);
                        randomBlock._isCanRemove = true;
                    });
                });
            }
        }

        public override void Destroy()
        {
            foreach (var gameObject in _flyPlane)
            {
                gameObject.transform.DOKill();
                GameObject.Destroy(gameObject);
            }
            
            _flyPlane.Clear();
            _flyPlane = null;
            
            _lockBlocks.Clear();
            _lockBlocks = null;
        }

        private Block.Block RandomBlock(List<Block.Block> blackBlocks, List<Block.Block> overlapBlocks)
        {
            Block.Block block = null;
            if (blackBlocks.Count > 0)
            {
                block = blackBlocks.RandomPickOne();
                blackBlocks.Remove(block);
            }
            else if (overlapBlocks.Count > 0)
            {
                block = overlapBlocks.RandomPickOne();
                overlapBlocks.Remove(block);
            }

            if (block != null)
                block._isCanRemove = false;

            return block;
        }
    }
}