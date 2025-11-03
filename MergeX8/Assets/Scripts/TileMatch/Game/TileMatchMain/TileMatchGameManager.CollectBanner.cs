using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using Framework;
using Gameplay;
using TileMatch.Event;
using TileMatch.Game.Block;
using UnityEngine;
using UnityEngine.UI;

namespace TileMatch.Game
{
    public partial class TileMatchGameManager
    {
        private Vector3 _collectCenterPosition;
        private Transform _collectBanner;
        private Camera _sceneCamera;
        private SpriteRenderer _collectSpriteRenderer;
        
        private List<Vector3> _collectSeatPosition = new List<Vector3>();
        private List<Vector3> _extendCollectSeatPosition = new List<Vector3>();
        
        private Vector3 _collectBannerWordPos = Vector3.zero;
        private List<Block.Block> _collectBannerList = new List<Block.Block>();
        private List<Block.Block> _flyBlockList = new List<Block.Block>();
        private List<List<Block.Block>> _recordList = new List<List<Block.Block>>();
        private Dictionary<int, List<Block.Block>> _sameTypeBlockDir = new Dictionary<int, List<Block.Block>>();
        private Dictionary<FailTypeEnum, FailData> _failedBlocks = new Dictionary<FailTypeEnum, FailData>();
        
        public float _dropMoveSpeed = 31f;
        public float _normalMoveSpeed = 20f;

        private bool _isGameFail = false;
        private bool _waitGameEndAnim = false;
        
        private bool _isBack = false;
        private bool CanBack
        {
            get { return _flyBlockList.Count == 0 && !_isBack; }
        }
        private List<Vector3> seatPosition
        {
            get
            {
                if (_isExtendBanner)
                    return _extendCollectSeatPosition;

                return _collectSeatPosition;
            }
        }

        private float seatScale
        {
            get
            {
                if (_isExtendBanner)
                    return GameConst.ExtendCollectBannerScale;

                return GameConst.InitCollectBannerScale;
            }
        }

        private int _maxCollectLenght = 0;

        private bool _isExtendBanner = false;

        private Animator _bannerAnimator;
        
        private void AwakeCollectBanner()
        {
            _bannerAnimator = transform.GetComponent<Animator>();
            
            _collectBanner = transform.Find("Board/CollectionBanner").transform;
            _collectSpriteRenderer = _collectBanner.GetComponent<SpriteRenderer>();

            _collectBanner.transform.position = _bannerShowPosition + Vector3.down * 10;
            _sceneCamera = TileMatchRoot.Instance._matchMainCamera;

            InitCollectBannerSpriteRender();
        }

        private void InitCollectBannerSpriteRender()
        {
            string animName = "Normal";
            
            // Vector2 size = GameConst.InitCollectBannerSize;
            // if (_isExtendBanner)
            //     size = GameConst.ExtendCollectBannerSize;
            //
            // _collectSpriteRenderer.size = size;

            if (_isExtendBanner)
                animName = "Tile_add";

            _bannerAnimator.Play(animName);
        }

        private void ExtendCollectBanner()
        {
            if (_isExtendBanner)
                return;
            
            _isExtendBanner = true;
            
            _maxCollectLenght = GameConst.COLLECT_EXTEND_MAX_LENGHT;
            InitCollectBannerSpriteRender();
            
            
            RestCollectBannerPositionAndScale();
        }
        
        private void CollectDestroy()
        {
            _collectBannerList.Clear();
            _collectBannerList = null;
            
            _flyBlockList.Clear();
            _flyBlockList = null;
            
            _recordList.Clear();
            _recordList = null;
            
            _sameTypeBlockDir.Clear();
            _sameTypeBlockDir = null;
            
            _collectSeatPosition.Clear();
            _collectSeatPosition = null;
            
            _extendCollectSeatPosition.Clear();
            _extendCollectSeatPosition = null;
            
            SuperCollectBannerDestroy();
        }

        private void CollectClear()
        {
            _collectBannerList.Clear();
            _flyBlockList.Clear();
            _recordList.Clear();
            _sameTypeBlockDir.Clear();

            CleanSuperCollectBanner();

            _isExtendBanner = false;
            _waitGameEndAnim = false;
            InitCollectBannerSpriteRender();
        }
        
        private IEnumerator AdaptCollectionBanner()
        {
            yield return new WaitForEndOfFrame();
            
            var uiWindow = UIManager.Instance.GetOpenedUIByPath(UINameConst.TileMatchMain);

            var collectionBanner = uiWindow.transform.Find("Root/BottomGroup/CollectionBanner").transform;
            
            var screenPos = UIRoot.Instance.mUICamera.WorldToScreenPoint(collectionBanner.position);
            _collectBannerWordPos = _sceneCamera.ScreenToWorldPoint(screenPos);
            _collectBannerWordPos.z = -0.1f;
            _collectBanner.transform.position = _collectBannerWordPos;

            _collectCenterPosition = _collectBanner.Find("CenterPoint").transform.position;
            
            InitCollectSeatPosition();
            InitSuperCollectSeatPosition();
        }

        private void InitCollectionBanner()
        {
            _maxCollectLenght = GameConst.COLLECT_MAX_LENGHT;

            InitCollectBannerSpriteRender();
        }

        private void InitCollectSeatPosition()
        {
            for (int i = 0; i < GameConst.Banner_MaxLength; i++)
            {
                _collectSeatPosition.Add(new Vector3(GameConst.Banner_StartX + i*GameConst.Banner_OffsetX, _collectCenterPosition.y, -40 - i * 0.01f));
            }
            
            for (int i = 0; i < GameConst.Banner_MaxLength; i++)
            {
                _extendCollectSeatPosition.Add(new Vector3(GameConst.Banner_Extend_StartX + i*GameConst.Banner_OffsetX*GameConst.ExtendCollectBannerScale, _collectCenterPosition.y, -40 - i * 0.01f));
            }
        }

        private bool IsFullCollectBanner()
        {
            return _collectBannerList.Count >= _maxCollectLenght && SameTypeGroupCount(3) <= 0;
        }

        private int SameTypeGroupCount(int targetCount)
        {
            int count = 0;
            foreach (var kv in _sameTypeBlockDir)
            {
                if(kv.Value == null)
                    continue;
                
                if (kv.Value.Count >= targetCount)
                {
                    count += 1;
                }
            }
            return count;
        }
        
        private void AddCollectBanner(List<Block.Block> blocks)
        {
            if(blocks == null || blocks.Count == 0)
                return;

            PlayMethod_BeforeRemoveBlock(blocks);

            int dropFinishNum = 0;
            foreach (var block in blocks)
            {
                int sameBlockSeatIndex = -1;
                sameBlockSeatIndex = _collectBannerList.FindLastIndex((a) => { return a.BlockId == block.BlockId; });
                if (sameBlockSeatIndex < 0)
                {
                    sameBlockSeatIndex = _collectBannerList.Count;
                    _collectBannerList.Add(block);
                }
                else
                {
                    int startIndex = sameBlockSeatIndex + 1;
                    sameBlockSeatIndex = startIndex;
                    
                    // 后面的牌移动
                    for (int i = startIndex; i < _collectBannerList.Count; i++)
                    {
                        int nextSeatIdx = i + 1;
                        if (_flyBlockList.Contains(_collectBannerList[i]))
                        {
                            _collectBannerList[i].DropMove(seatPosition[nextSeatIdx], seatScale,_dropMoveSpeed);
                        }
                        else
                        {
                            _collectBannerList[i].MoveTo(seatPosition[nextSeatIdx], seatScale, _normalMoveSpeed);
                        }
                    }

                    // 前面的牌调整层级
                    for (int i = 0; i < startIndex; i++)
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
                    _collectBannerList.Insert(startIndex, block);
                }
            
                // 该牌掉落
                if (block.GetAreaType() == AreaType.SuperBanner)
                    RemoveSuperBlock(block);

                Action<Block.Block> moveEnd = (b) =>
                {
                    dropFinishNum++;
                    OnBlockDropFinished(b);
                    
                    if(dropFinishNum == blocks.Count)
                        AfterRemoveBlock(blocks);
                    
                    TriggerGuide(GuideType.CollectBlock);
                };

                if (blocks.Count > 1)
                {
                    block.DropMoveLine(seatPosition[sameBlockSeatIndex], seatScale, _dropMoveSpeed, (b) =>
                    {
                        moveEnd(b);
                    });
                }
                else
                {
                    block.DropMove(seatPosition[sameBlockSeatIndex], seatScale, _dropMoveSpeed, (b) =>
                    {
                        moveEnd(b);
                    });
                }
            }
            
            BeforeRemoveBlock(blocks);

            if (!HaveActiveBlock())
            {
                PausePlayMethod();
            }
                
            if (IsFullCollectBanner())
            {
                if(!_failedBlocks.ContainsKey(FailTypeEnum.GridFull))
                    _failedBlocks[FailTypeEnum.GridFull] = new FailData();
            }
        }

        private void OnBlockDropFinished(Block.Block block)
        {
            _flyBlockList.Remove(block);

            var tmpItemList = GetContinuousBlocks(block);
            
            _collectBannerList.FindAll((a) => { return a.BlockId == block.BlockId&& a.GetBlockState() == BlockState.InBanner; });
            if (tmpItemList != null && tmpItemList.Count >= 3)
            {
                if (_failedBlocks.Count > 0)
                {
                    foreach (var kv in _failedBlocks)
                    {
                        foreach (var removeBlock in tmpItemList)
                        {
                            if(!kv.Value._blocks.Contains(removeBlock))
                                continue;
                            
                            CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.2f, () => { CheckGameFailure(); }));
                            return;
                        }
                    }
                }
                
                var vaildBlocks = _collectBannerList.FindAll((a) => { return a.GetBlockState() == BlockState.InBanner; });
                
                tmpItemList = tmpItemList.GetRange(0, 3);
                tmpItemList.ForEach(a=>vaildBlocks.Remove(a));
                
                AudioManager.Instance.PlaySound(19+TileMatchRoot.AudioDistance);

                bool isGameEnd = !HaveActiveBlock();
                if (vaildBlocks.Count == 0 && isGameEnd)
                {
                    WaitGameEndAnim(tmpItemList, () =>
                    {
                        RemoveCollectBlock(tmpItemList);
                    });
                }
                else
                {
                    PlayMethod_DisappearBlock(tmpItemList);
                    
                    foreach (var tmpItem in tmpItemList)
                    {
                        tmpItem.TweenHide();
                    }
                    
                    CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.3f, () => { RemoveCollectBlock(tmpItemList); }));
                }
            }
            else
            {
                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.2f, () => { CheckGameFailure(); }));
            }
        }

        private List<Block.Block> GetContinuousBlocks(Block.Block block, bool ignoreState = false)
        {
            if (_collectBannerList == null || _collectBannerList.Count == 0)
                return null;
            
            int index = -1;
            List<Block.Block> tmpItemList = new List<Block.Block>();
            for(int i = 0; i < _collectBannerList.Count; i++)
            {
                var bannerBlock = _collectBannerList[i];
                
                if (bannerBlock.BlockId != block.BlockId)
                    continue;
                
                if (!ignoreState && bannerBlock.GetBlockState() != BlockState.InBanner)
                    continue;
                
                if (index < 0)
                {
                    index = i;
                    tmpItemList.Add(_collectBannerList[i]);
                }
                else
                {
                    if (index + 1 == i)
                    {
                        tmpItemList.Add(_collectBannerList[i]);
                        index = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return tmpItemList;
        }
        
        private void RemoveCollectBlock(List<Block.Block> blocksList)
        {
            if(_collectBannerList == null || _collectBannerList.Count == 0)
                return;
            
            foreach (var block in blocksList)
            {
                if (_collectBannerList.Contains(block))
                    _collectBannerList.Remove(block);

                if (_sameTypeBlockDir[block.BlockId].Contains(block))
                    _sameTypeBlockDir[block.BlockId].Remove(block);
            }


            if (!_waitGameEndAnim)
            {
                for (int i = 0; i < _collectBannerList.Count; i++)
                {
                    if (i >= 0 && i < seatPosition.Count)
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
                }
            }
            
            RemoveBlockBi();
            
            if (IsFullCollectBanner())
            {
                if(!_failedBlocks.ContainsKey(FailTypeEnum.GridFull))
                    _failedBlocks[FailTypeEnum.GridFull] = new FailData();
            }
            
            if(!CheckGameSuccess())
                CheckGameFailure();
        }
        
        private void AddCollectRecord(List<Block.Block> blocks)
        {
            _recordList.Add(blocks);
        }

        private void ClearRecord()
        {
            ClearPlayMethodRecord();
            _recordList.Clear();
        }

        private void PreparationPreprocess(List<Block.Block> blocks)
        {
            bool isClearRecord = false;
            
            foreach (var block in blocks)
            {
                _flyBlockList.Add(block);
                
                if (!_sameTypeBlockDir.ContainsKey(block.BlockId))
                    _sameTypeBlockDir[block.BlockId] = new List<Block.Block>();
                _sameTypeBlockDir[block.BlockId].Add(block);
                
                if(_sameTypeBlockDir[block.BlockId].Count < 3)
                    continue;

                if(_failedBlocks.Count > 0)
                    continue;
                
                isClearRecord = true;
                ClearRecord();
            }

            if (isClearRecord)
            {
                if(_failedBlocks.ContainsKey(FailTypeEnum.Special))
                    _recordList.Add(_failedBlocks[FailTypeEnum.Special]._blocks);
                return;
            }
            
            AddCollectRecord(blocks);
        }

        private void OnBack()
        {
            if (_isBack)
                return;
            
            if (_recordList == null || _recordList.Count <= 0)
                return ;

            AudioManager.Instance.PlaySound(21+TileMatchRoot.AudioDistance);
            _isBack = true;
            
            var blocks = _recordList[_recordList.Count - 1];
            _recordList.RemoveAt(_recordList.Count - 1);
            
            BeforeRecoverBlock(blocks, () =>
            {
                int backToNum = 0;
                foreach (var block in blocks)
                {
                    if (block.GetAreaType() == AreaType.SuperBanner)
                    {
                        AddSuperCollectBanner(block, block._superBannerIndex, true, () =>
                        {
                            backToNum++;
                            if (backToNum == blocks.Count)
                              {
                                _isBack = false;
                                
                                AfterRecoverBlock(blocks);
                              }
                        }, block._isSuperBanner);
                        RemoveBlock(block);
                    }
                    else
                    {
                        block.BackMove(_dropMoveSpeed ,action:(b) =>
                            {
                                backToNum++;
                                if (backToNum == blocks.Count)
                                {
                                    _isBack = false;
                                    AfterRecoverBlock(blocks);
                                }
                            });
                            RemoveBlock(block);
                        }
                }

                RestCollectBannerPositionAndScale();
            });
            
        }

        private void RestCollectBannerPositionAndScale()
        {
            for (int i = 0; i < _collectBannerList.Count; i++)
            {
                if (_flyBlockList.Contains(_collectBannerList[i]))
                {
                    _collectBannerList[i].DropMove(seatPosition[i], seatScale,_dropMoveSpeed);
                }
                else
                {
                    _collectBannerList[i].MoveTo(seatPosition[i], seatScale, _normalMoveSpeed);
                }
            }
        }
        private void OnSuperBack()
        {
            if(_collectBannerList == null || _collectBannerList.Count == 0)
                return;
            
            AudioManager.Instance.PlaySound(22+TileMatchRoot.AudioDistance);
            
            int backCount = Mathf.Min(_collectBannerList.Count, GameConst.SuperBackMaxLength);
            List<Block.Block> blocks = new List<Block.Block>();
            for (int i = 0; i < backCount; i++)
            {
                blocks.Add(_collectBannerList[i]);
            }

            for (int i = 0; i < blocks.Count; i++)
            {
                AddSuperCollectBanner(blocks[i], i);
                RemoveBlock(blocks[i]);
            }

            RestCollectBannerPositionAndScale();
            
            ClearRecord();
        }
        
        private void RemoveBlock(Block.Block block)
        {
            if (_collectBannerList.Contains(block))
                _collectBannerList.Remove(block);   

            if (_sameTypeBlockDir.ContainsKey(block.BlockId) && _sameTypeBlockDir[block.BlockId].Contains(block))
                _sameTypeBlockDir[block.BlockId].Remove(block);
        }

        private bool CheckGameSuccess()
        {
            if (_collectBannerList.Count != 0 || _flyBlockList.Count != 0 || GetActiveBlocks().Count != 0)
                return false;
            
            foreach (var blocks in _superCollectBannerList)
            {
                if(blocks.Count > 0)
                    return false;
            }

            LevelSuccessBi();
            PlayHideAnim(() =>
            {
                TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_Success);
            });
            return true;
        }

        private void CheckGameFailure()
        {
            if(_isGameFail)
                return;

            if(_failedBlocks.Count == 0)
                return;
            
            if (_failedBlocks.ContainsKey(FailTypeEnum.Special))
            {
                var failedBlock = _failedBlocks[FailTypeEnum.Special]._failBlock;
                
                if(failedBlock._blockModel._blockData.blockType != (int)BlockTypeEnum.Bomb)
                    TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_Fail,FailTypeEnum.Special, (BlockTypeEnum)failedBlock._blockModel._blockData.blockType);
                
                _isGameFail = true;
                LevelFailedBi(FailTypeEnum.Special);
                return;
            }
            
            if (_failedBlocks.ContainsKey(FailTypeEnum.GridFull))
            {
                TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_Fail,FailTypeEnum.GridFull);
                
                LevelFailedBi(FailTypeEnum.GridFull);
                
                _isGameFail = true;
                return;
            }
            
            if (_failedBlocks.ContainsKey(FailTypeEnum.Time))
            {
                TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_Fail,FailTypeEnum.Time);
                
                LevelFailedBi(FailTypeEnum.Time);
                return;
            }
            
            if (_failedBlocks.ContainsKey(FailTypeEnum.SpecialFail))
            {
                var failedBlock = _failedBlocks[FailTypeEnum.SpecialFail]._failBlock;

                TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_Fail,FailTypeEnum.SpecialFail, (BlockTypeEnum)failedBlock._blockModel._blockData.blockType);
                _isGameFail = true;
                
                LevelFailedBi(FailTypeEnum.SpecialFail);
            }
        }

        public int GetBannerBlockNum()
        {
            return _collectBannerList.Count;
        }
        
        public bool IsEmptyBanner()
        {
            foreach (var kv in _sameTypeBlockDir)
            {
                if (kv.Value.Count % 3 != 0)
                    return false;
            }

            return true;
        }
        
        public int GetRecordNum()
        {
            return _recordList.Count;
        }

        public bool CanUseProp(UserData.ResourceId id)
        {
            if (!CanBack)
                return false;

            if (_isGameFail)
                return false;

            if (_failedBlocks.Count > 0)
                return false;
            
            return HaveActiveBlock();
        }

        public bool IsExtendBanner()
        {
            return _isExtendBanner;
        }
    }
}