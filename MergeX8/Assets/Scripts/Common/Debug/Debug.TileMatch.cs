using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonPlus.Config.TileMatch;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using TileMatch.Event;
using TileMatch.Game;
using UnityEngine;
using BiEventTileGarden = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
public partial class SROptions
{
    private const string TileMatch = "TileMatch";
    
    [Sort(-28)]
    [Category(TileMatch)]
    [DisplayName("当前关卡")]
    public int CurrentLevel
    {
        get
        {
            return TileMatchGameManager.Instance.LevelId;
        }
    }
    [Sort(-29)]
    [Category(TileMatch)]
    [DisplayName("加载关卡id")]
    public int CurrentLevelIndex
    {
        get;
        set;
    }
    
    [Sort(-30)]
    [Category(TileMatch)]
    [DisplayName("加载关卡")]
    public void LoadLevel()
    {
        if(TileMatchGameManager.Instance == null)
            return;

        TileMatchGameManager.Instance.LoadLevel(CurrentLevelIndex);
        HideDebugPanel();
    }

    [Category(TileMatch)]
    [DisplayName("关卡成功")]
    public void LevelSuccess()
    {
        if(TileMatchGameManager.Instance == null)
            return;
        
        TileMatchGameManager.Instance.PlayHideAnim(() =>
        {
            TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_Success);
        });
        HideDebugPanel();
    }
    
    [Category(TileMatch)]
    [DisplayName("所有道具+5")]
    public void AddAllProp5()
    {
        UserData.Instance.AddRes((int)UserData.ResourceId.Prop_Back, 5, new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug), true);
        UserData.Instance.AddRes((int)UserData.ResourceId.Prop_SuperBack, 5, new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug), true);
        UserData.Instance.AddRes((int)UserData.ResourceId.Prop_Magic, 5, new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug), true);
        UserData.Instance.AddRes((int)UserData.ResourceId.Prop_Shuffle, 5, new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug), true);
        UserData.Instance.AddRes((int)UserData.ResourceId.Prop_Extend, 5, new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug), true);
    }
    
    [Category(TileMatch)]
    [DisplayName("清空所有道具")]
    public void CleanAllProp()
    {
        UserData.Instance.ConsumeRes(UserData.ResourceId.Prop_Back, UserData.Instance.GetRes(UserData.ResourceId.Prop_Back), new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
        UserData.Instance.ConsumeRes(UserData.ResourceId.Prop_SuperBack, UserData.Instance.GetRes(UserData.ResourceId.Prop_SuperBack), new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
        UserData.Instance.ConsumeRes(UserData.ResourceId.Prop_Magic, UserData.Instance.GetRes(UserData.ResourceId.Prop_Magic), new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
        UserData.Instance.ConsumeRes(UserData.ResourceId.Prop_Shuffle, UserData.Instance.GetRes(UserData.ResourceId.Prop_Shuffle), new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
        UserData.Instance.ConsumeRes(UserData.ResourceId.Prop_Extend, UserData.Instance.GetRes(UserData.ResourceId.Prop_Extend), new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
    }
    
    [Category(TileMatch)]
    [DisplayName("清空金币")]
    public void CleanCoin()
    {
        UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, UserData.Instance.GetRes(UserData.ResourceId.Coin), new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
    }
    
    [Category(TileMatch)]
    [DisplayName("清空体力")]
    public void CleanEnergy()
    {
        StorageManager.Instance.GetStorage<StorageHome>().Energy = 0;
        HideDebugPanel();
    }
    
    [Category(TileMatch)]
    [DisplayName("补满体力")]
    public void FullEnergy()
    {
        UserData.Instance.AddRes((int)UserData.ResourceId.Energy, EnergyModel.Instance.MaxEnergyNum, new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
        HideDebugPanel();
    }
    
    [Category(TileMatch)]
    [DisplayName("清空无限体力")]
    public void CleanUnlimitEnergy()
    {
        StorageManager.Instance.GetStorage<StorageHome>().UnlimitEnergyEndUTCTimeInSeconds = 0;
        HideDebugPanel();
    }
    
    [Category(TileMatch)]
    [DisplayName("增加3分钟无限体力")]
    public void AddUnlimitEnergy()
    {
        EnergyModel.Instance.AddEnergyUnlimitedTime(3*60*1000,new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.Debug));
        HideDebugPanel();
    }
    
    private bool _isUserTestFinderPath = false;
    [Category(TileMatch)]
    [DisplayName("程序使用-是否使用测试配置")]
    public bool IsUserTestFinderPath
    {
        get
        {
            return _isUserTestFinderPath;
        }
        set
        {
            _isUserTestFinderPath = value;
        }
    }

    private FailTypeEnum _failType = FailTypeEnum.Normal;
    [Category(TileMatch)]
    [DisplayName("关卡失败-类型")]
    public FailTypeEnum FailTypeEnum
    {
        get
        {
            return _failType;
        }
        set
        {
            _failType = value;
        }
    }
    
    private BlockTypeEnum _blockType = BlockTypeEnum.Normal;
    [Category(TileMatch)]
    [DisplayName("关卡失败-块类型")]
    public BlockTypeEnum BlockType
    {
        get
        {
            return _blockType;
        }
        set
        {
            _blockType = value;
        }
    }
    [Category(TileMatch)]
    [DisplayName("关卡失败")]
    public void LevelFailed()
    {
        if(TileMatchGameManager.Instance == null)
            return;
        
        TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_Fail,FailTypeEnum, BlockType);
        HideDebugPanel();
    }
    
    [Category(TileMatch)]
    [DisplayName("块块落下速度")]
    public float DropMoveSpeed
    {
        get
        {
            if(TileMatchGameManager.Instance == null)
                return 0f;

            return TileMatchGameManager.Instance._dropMoveSpeed;
        }
        set
        {
            if(TileMatchGameManager.Instance == null)
                return;

            TileMatchGameManager.Instance._dropMoveSpeed = value;
        }
    }
    
    [Category(TileMatch)]
    [DisplayName("块块移动速度")]
    public float NormalMoveSpeed
    {
        get
        {
            if(TileMatchGameManager.Instance == null)
                return 0f;

            return TileMatchGameManager.Instance._normalMoveSpeed;
        }
        set
        {
            if(TileMatchGameManager.Instance == null)
                return;

            TileMatchGameManager.Instance._normalMoveSpeed = value;
        }
    }
    
    
    [Category(TileMatch)]
    [DisplayName("效验关卡配置表")]
    public void VerifyLevelConfig()
    {
        Debug.LogError("------------开始效验所有配置表--------------");
        foreach (var levelConfig in TileMatchConfigManager.Instance.TileLevelList)
        {
            var _layoutGroup = TileMatchLayoutManager.Instance.GetLayoutGroup(levelConfig.layoutId);
            if (_layoutGroup == null)
            {
                Debug.LogError($"关卡:{levelConfig.id} 布局:{levelConfig.layoutId} 错误 布局为NULL");
                continue;
            }
            
            int blockNum = 0;
            int goldNum = 0;
            Dictionary<int, int> blocksType = new Dictionary<int, int>();
            Dictionary<int, int> blocksId = new Dictionary<int, int>();
            foreach (var tileLayout in _layoutGroup.layout)
            {
                foreach (var tileLayoutLayer in tileLayout.layers)
                {
                    foreach (var layerBlock in tileLayoutLayer.layerBlocks)
                    {
                        if(layerBlock.blockId == 65534)
                            continue;

                        if (!blocksType.ContainsKey(layerBlock.blockType))
                        {
                            blocksType.Add(layerBlock.blockType, 1);
                        }
                        else
                        {
                            blocksType[layerBlock.blockType]++;
                        }

                        if (layerBlock.blockType == (int)BlockTypeEnum.Gold)
                        {
                            goldNum++;
                        }
                        int num = 1;
                        if (layerBlock.blockType == (int)BlockTypeEnum.Funnel)
                        {
                           var spStr = layerBlock.blockParam.Split(',');
                           if (spStr != null)
                           {
                               num = spStr.Length;
                               foreach (var s in spStr)
                               {
                                   int id = int.Parse(s);
                                   if(!blocksId.ContainsKey(id))
                                       blocksId.Add(id, 1);
                                   else
                                   {
                                       blocksId[id]++;
                                   }
                               }
                           }
                        }
                        else
                        {
                            int id = layerBlock.blockId;
                            if(!blocksId.ContainsKey(id))
                                blocksId.Add(id, 1);
                            else
                            {
                                blocksId[id]++;
                            }
                        }
                        blockNum += num;
                    }
                }
            }

            if (blockNum % 3 != 0)
            {
                Debug.LogError($"关卡:{levelConfig.id} 布局:{levelConfig.layoutId} 错误 总块数:{blockNum}");
            }

            if (goldNum % 3 != 0)
            {
                Debug.LogError($"关卡:{levelConfig.id} 布局:{levelConfig.layoutId} 错误 黄金牌块数:{goldNum}");
            }
            
            foreach (var kv in blocksId)
            {
                if (kv.Value % 3 != 0)
                {
                    Debug.LogError($"关卡:{levelConfig.id} 布局:{levelConfig.layoutId} 错误  块id:{kv.Key} 块数:{kv.Value}");
                }
            }
        }
        Debug.LogError("------------结束效验所有配置表--------------");
    }
}