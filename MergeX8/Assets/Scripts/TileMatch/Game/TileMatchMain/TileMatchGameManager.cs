using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.TileMatch;
using DragonU3DSDK;
using TileMatch.Event;
using TileMatch.Game.Magic;
using TileMatch.Game.Shuffle;
using UnityEngine;
using Framework;
using Gameplay;
using TileMatch.UI.FailState;

namespace TileMatch.Game
{
    public partial class TileMatchGameManager : MonoBehaviour
    {
        public static TileMatchGameManager Instance;

        private int _levelId;

        public int LevelId
        {
            get { return _levelId; }
        }
        
        private Fsm _fsm;
        private void Awake()
        {
            Instance = this;
            AwakeLayout();
            AwakeCollectBanner();
            AwakeGuide();
            
            RegisterEvent();
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            RegisterGuide();
            
            yield return StartCoroutine(AdaptCollectionBanner());
            
            PlayShowAnim(() =>
            {
                StartPlayMethod();
                TriggerGuide(GuideType.GameStart);
            });
        }

        void Update()
        {
            UpdateInput();
            UpdatePlayMethod();
            _fsm?.Update();
        }

        private void FixedUpdate()
        {
            _fsm?.FixedUpdate(Time.deltaTime);
        }

        public void InitLevel(int levelId)
        {
            _levelId = levelId;
            
            LoadLayout();
            _fsm = new Fsm();
        }

        private void OnDestroy()
        {
            CollectDestroy();
            LayoutDestroy();
            InputDestroy();
            DestroyEvent();
            DestroyPlayMethod();
            MagicBlockManager.Instance.CleanMagic();
        }

        private void RegisterEvent()
        {
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_Fail, GameFail);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_Success, GameSuccess);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_Replay, ReplayGame);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_GoHome, GoHome);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_Operate_UseProp, OperateUseProp);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_PauseGame, PauseGame);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_RecoverGame, RecoverGame);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_BuyPropSuccess, BuyPropSuccess);
            TileMatchEventManager.Instance.AddEvent(GameEventConst.GameEvent_FailedContinuesGame, FailedContinuesGame);
        }

        private void DestroyEvent()
        {
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_Fail, GameFail);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_Success, GameSuccess);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_Replay, ReplayGame);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_GoHome, GoHome);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_PauseGame, PauseGame);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_RecoverGame, RecoverGame);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_Operate_UseProp, OperateUseProp);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_BuyPropSuccess, BuyPropSuccess);
            TileMatchEventManager.Instance.RemoveEvent(GameEventConst.GameEvent_FailedContinuesGame, FailedContinuesGame);
        }


        private void OnClean()
        {
            CollectClear();
            LayoutClean();
            ClearPlayMethod();
            InputClean();
        }

        private void GameSuccess(BaseEvent e)
        {
            KapiTileModel.Instance.DealWin();
            UIManager.Instance.OpenWindow(UINameConst.UITileMatchSuccess, TileMatchLevelModel.Instance.CollectCoin);
        }

        private void GameFail(BaseEvent e)
        {
            FailTypeEnum failTypeEnum = (FailTypeEnum) e.datas[0];
            BlockTypeEnum blockTypeEnum = BlockTypeEnum.Normal;
            if(failTypeEnum==FailTypeEnum.Special || failTypeEnum==FailTypeEnum.SpecialFail)
                blockTypeEnum=(BlockTypeEnum) e.datas[1];
            FailStateParamBase paramBase = new FailStateParamBase();
            paramBase.BlockTypeEnum = blockTypeEnum;
            paramBase.FailTypeEnum = failTypeEnum;

            if (failTypeEnum == FailTypeEnum.Time)
            {
                if (_flyBlockList.Count > 0 || MagicBlockManager.Instance.IsMagic)
                {
                    if(!_failedBlocks.ContainsKey(FailTypeEnum.Time))
                        _failedBlocks.Add(FailTypeEnum.Time,new FailData());
                    return;
                }
            }
            
            switch (failTypeEnum)
            {
                case FailTypeEnum.EnterBack:
                    _fsm.ChangeState<FailState_EnterBack>(paramBase);
                    break;
                case FailTypeEnum.GridFull :
                    _isGameFail = true;

                    _fsm.ChangeState<FailState_GridFull>(paramBase);
                    break;
                case FailTypeEnum.Special :
                    _isGameFail = true;

                    _fsm.ChangeState<FailState_Special>(paramBase);
                    break;             
                case FailTypeEnum.SpecialFail :
                    _isGameFail = true;

                    _fsm.ChangeState<FailState_SpecialFail>(paramBase);
                    break;
                case FailTypeEnum.Time:
                {
                    _isGameFail = true;

                    _fsm.ChangeState<FailState_Time>(paramBase);
                    
                    LevelFailedBi(FailTypeEnum.Time);
                    break;
                }
            }
            DebugUtil.Log((FailTypeEnum)e.datas[0]);
        }
        private void ReplayGame(BaseEvent e)
        {
            TileMatchLevelModel.Instance.CollectCoin = 0;
            PlayHideAnim(() =>
            {
                OnClean();
            
                LoadLayout();
                
                PlayShowAnim(() =>
                {
                    StartPlayMethod();
                });
            });
        }

        private void GoHome(BaseEvent e)
        {
            PlayHideAnim(() =>
            {
                SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
            });
        }

        private void OperateUseProp(BaseEvent e)
        {   
            UsePropBi((UserData.ResourceId)e.datas[0]);
            
            switch ((UserData.ResourceId)e.datas[0])
            {
                case UserData.ResourceId.Prop_Back:
                {
                    OnBack();
                    break;
                }
                case UserData.ResourceId.Prop_SuperBack:
                {
                    OnSuperBack();
                    break;
                }
                case UserData.ResourceId.Prop_Magic:
                {
                    var blocks = GetUnLockBlocks();

                    List<Block.Block> bannerBlocks = new List<Block.Block>(_collectBannerList);
                    foreach (var block in _flyBlockList)
                    {
                        bannerBlocks.Remove(block);
                    }
                    
                    foreach (var block in _collectBannerList)
                    {
                       var removeBlocks = GetContinuousBlocks(block, true);
                       if(removeBlocks == null || removeBlocks.Count < 3)
                           continue;

                       foreach (var removeBlock in removeBlocks)
                       {
                           bannerBlocks.Remove(removeBlock);
                       }
                    }
            
                    ClearRecord();
                    MagicBlockManager.Instance.Magic(blocks, bannerBlocks);
                    break;
                }
                case UserData.ResourceId.Prop_Shuffle:
                {
                    List<Block.Block> blocks = GetUnLockBlocks();
            
                    ClearRecord();
                    ShuffleBlockManager.Instance.Shuffle(blocks);
                    break;
                }
                case UserData.ResourceId.Prop_Extend:
                {
                    ExtendCollectBanner();
                    break;
                }
            }
        }

        public List<Block.Block> GetUnLockBlocks()
        {
            List<Block.Block> blocks = new List<Block.Block>();
            foreach (var blocksValue in _blocks.Values)
            {
                if(!IsLock(blocksValue))
                    blocks.Add(blocksValue);
            }
                    
            foreach (var kv in _appendBlocks)
            {
                blocks.AddRange(kv.Value);
            }

            return blocks;
        }
        
        public bool CanUseMagic()
        {
            List<Block.Block> blocks = new List<Block.Block>();
            blocks.AddRange(_blocks.Values);
            foreach (var kv in _appendBlocks)
            {
                blocks.AddRange(kv.Value);
            }

            List<Block.Block> bannerBlocks = new List<Block.Block>(_collectBannerList);
            foreach (var block in _flyBlockList)
            {
                bannerBlocks.Remove(block);
            }

            return MagicBlockManager.Instance.CanUseMagic(blocks, bannerBlocks);
        }
        
        private void PauseGame(BaseEvent e)
        {
            PausePlayMethod();
        }

        private void RecoverGame(BaseEvent e)
        {
            RecoverPlayMethod();
        }
        
        public void LoadLevel(int level)
        {
           OnClean();

            InitLevel(level);
            
            PlayShowAnim(() =>
            {
                StartPlayMethod();
                TriggerGuide(GuideType.GameStart);
            });
        }

        private void FailedContinuesGame(BaseEvent e)
        {
            _isGameFail = false;
            _failedBlocks.Clear();
            
            RecoverPlayMethod();
            FailTypeEnum type = (FailTypeEnum)e.datas[0];
            switch (type)
            {
                case FailTypeEnum.Time:
                {
                    LevelTimeResume();
                    AddTime(TileMatchConfigManager.Instance.GetInt("AddTime"));
                    break;
                }
                case FailTypeEnum.GridFull:
                {
                    LevelResume();
                    TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_AutoUseProp, UserData.ResourceId.Prop_SuperBack, false);
                    break;
                }
                case FailTypeEnum.EnterBack:
                {
                    break;
                }
                case FailTypeEnum.Special:
                {                   
                    TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_AutoUseProp, UserData.ResourceId.Prop_Back, false);
                    break;
                }
            }
        }
    }
}
