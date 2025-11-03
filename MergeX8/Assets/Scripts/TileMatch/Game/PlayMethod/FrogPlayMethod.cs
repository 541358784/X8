using System;
using System.Collections.Generic;
using DragonPlus;
using GamePool;
using SomeWhere;
using SomeWhereTileMatch;
using UnityEngine;

namespace TileMatch.Game.PlayMethod
{
    public class FrogJumpRecord
    {
        public Block.Block _startBlock;
        public Block.Block _endBlock;
        public FrogMethodLogic _frogLogic;
    }
    
    public enum FrogStatus
    {
        None,
        RelayJump,
        Jumping,
    }
    
    public class FrogPlayMethod : PlayMethodBase
    {
        private List<Block.Block> _frogBlocks = new List<Block.Block>();
        private bool _isStart = false;
        private bool _isDie = false;

        private FrogStatus _frogStatus = FrogStatus.None;
        
        private List<FrogMethodLogic> _frogLogics = new List<FrogMethodLogic>();
        private Stack<List<FrogJumpRecord>> _frogRecord = new Stack<List<FrogJumpRecord>>();
        
        public override void Init(params object[] param)
        {
            _frogBlocks = (List<Block.Block>)param[0];
            
            foreach (var block in _frogBlocks)
            {
                var frog = GamePool.ObjectPoolManager.Instance.Spawn(ObjectPoolName.TileMatchBlock_Frog);
                var frogLogic = frog.GetComponent<FrogMethodLogic>();
                if(frogLogic == null)
                    frogLogic = frog.AddComponent<FrogMethodLogic>();
                frogLogic.Init(block);
                _frogLogics.Add(frogLogic);
            }

            _isDie = false;
        }

        public override void Start()
        {
            _isStart = true;
        }
        
        public override void Destroy()
        {
            _isStart = false;
            
            foreach (var frog in _frogLogics)
            {
                GamePool.ObjectPoolManager.Instance.DeSpawn(ObjectPoolName.TileMatchBlock_Frog, frog.gameObject);
            }
            _frogLogics.Clear();
            _frogBlocks.Clear();
        }

        public override void CleanRecord()
        {
            _frogRecord.Clear();
        }

        public override void AfterRecoverBlock(List<Block.Block> blocks)
        {
            if(!_isStart)
                return;
            
            if(_frogRecord.Count == 0)
                return;

            int jumpCompleteNum = 0;
            List<FrogJumpRecord> records =  _frogRecord.Pop();

            foreach (var frogJumpRecord in records)
            {
                frogJumpRecord._endBlock._isCanRemove = true;
            }
            foreach (var frogJumpRecord in records)
            {
                frogJumpRecord._frogLogic.Jump(frogJumpRecord._startBlock, () =>
                {
                    jumpCompleteNum++;
                    if (jumpCompleteNum >= _frogLogics.Count)
                        CheckDie();
                });
            }
        }

        private bool CheckDie()
        {
            List<Block.Block> canRemoveBlocks = TileMatchGameManager.Instance.GetCanRemoveBlocks(true);
            
            if (_frogLogics.Count >= canRemoveBlocks.Count + _frogLogics.Count)
            {
                _frogLogics.ForEach(a=>a.FrogDie());
                _frogLogics.Clear();

                CleanRecord();
                
                return true;
            }

            return false;
        }

        public override void BeforeRemoveBlock(List<Block.Block> blocks, bool isRefresh = true)
        {
            if(!_isStart)
                return;

            if (_frogStatus != FrogStatus.None)
                return;
            
            _frogStatus = FrogStatus.RelayJump;
            
            List<Block.Block> canRemoveBlocks = TileMatchGameManager.Instance.GetCanRemoveBlocks(true);

            _isDie = canRemoveBlocks.Count <= 1;
        }
        
        public override void AfterRemoveBlock(List<Block.Block> blocks)
        {
            if(!_isStart)
                return;

            if(_frogLogics.Count == 0)
                return;

            if(_frogStatus != FrogStatus.RelayJump)
                return;
            
            if (_isDie)
            {
                _frogLogics.ForEach(a=>a.FrogDie());
                _frogLogics.Clear();
                
                CleanRecord();
                return;
            }
            
            _frogStatus = FrogStatus.Jumping;
            List<Block.Block> canRemoveBlocks = TileMatchGameManager.Instance.GetCanRemoveBlocks(true);

            List<Block.Block> frogBlocks = new List<Block.Block>();
            foreach (var frogMethodLogic in _frogLogics)
            {
                frogMethodLogic._block._isCanRemove = true;
                
                if(canRemoveBlocks.Contains(frogMethodLogic._block))
                    canRemoveBlocks.Remove(frogMethodLogic._block);
                
                frogBlocks.Add(frogMethodLogic._block);
            }
            
            //AudioManager.Instance.PlaySound(30+TileMatchRoot.AudioDistance);
            int jumpCompleteNum = 0;
            List<FrogJumpRecord> records = new List<FrogJumpRecord>();
            foreach (var frogMethodLogic in _frogLogics)
            {
                if (canRemoveBlocks.Count == 0)
                {
                    canRemoveBlocks = frogBlocks;
                }
                
                var randomBlock = canRemoveBlocks.RandomPickOne();
                canRemoveBlocks.Remove(randomBlock);
                
                FrogJumpRecord record = new FrogJumpRecord();
                record._startBlock = frogMethodLogic._block;
                record._endBlock = randomBlock;
                record._frogLogic = frogMethodLogic;
                
                records.Add(record);

                frogMethodLogic.Jump(randomBlock, () =>
                {
                    jumpCompleteNum++;
                    if(jumpCompleteNum >= _frogLogics.Count)
                    {
                        CheckDie();
                        _frogStatus = FrogStatus.None;
                    }
                });
            }
            
            _frogRecord.Push(records);
        }

        public override void Magic_BeforeRemoveBlock(List<Block.Block> blocks)
        {
            foreach (var block in blocks)
            {
                for (int i = 0; i < _frogLogics.Count; i++)
                {
                    if (_frogLogics[i].Unbind(block))
                    {
                        _frogLogics[i].FrogDie();
                        _frogLogics.RemoveAt(i);
                        i--;
                        
                        CleanRecord();
                    }
                }
            }

            BeforeRemoveBlock(blocks);
        }

        public override void Magic_AfterRemoveBlock(List<Block.Block> blocks)
        {
            AfterRemoveBlock(blocks);
        }

        public override void Hide()
        {      
            foreach (var frog in _frogLogics)
            {
                frog.gameObject.gameObject.SetActive(false);
            }
        }

        public override void Show()
        {
            foreach (var frog in _frogLogics)
            {
                frog.gameObject.gameObject.SetActive(true);
            }
        }

        public override void StartShuffle()
        {
            Hide();
        }

        public override void StopShuffle()
        {
            Show();
        }

        public override bool IsLock(Block.Block block)
        {
            return false;
        }
    }
}