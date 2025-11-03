using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Decoration.DaysManager;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Google.Protobuf;
using UnityEngine;
using BiEvent = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Network.BI;
using DragonU3DSDK.Storage;
using Facebook.Unity;
using Farm.Model;
using Framework;
using Gameplay;
using Gameplay.UI.EnergyTorrent;
using Screw;
using Screw.GameLogic;

namespace DragonPlus
{
    public partial class GameBIManager : MonoBehaviour
    {
        public struct ItemChangeReasonArgs
        {
            public BiEvent.Types.ItemChangeReason reason;
            public string data1;
            public string data2;
            public string data3;
            public uint boostId; //仅当 item 是道具的时候记录，其他 item 的日志不记录该项
            public ulong cardId; //卡片的 ID，获得或者分享卡片时记录；仅当 item 是 card 的时候记录，其他 item 的日志不记录该项

            public ItemChangeReasonArgs(BiEvent.Types.ItemChangeReason reason)
            {
                this.reason = reason;
                this.data1 = null;
                this.data2 = null;
                this.data3 = null;

                boostId = 0;
                cardId = 0;
            }
        }

        private static GameBIManager instance;
        public static GameBIManager Instance => instance;

        private Queue<IMessage> messages = new Queue<IMessage>();
        
        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (DragonU3DSDK.Storage.StorageManager.Instance.Inited && messages.Count > 0)
            {
                _sendEvent(messages.Dequeue());
            }
        }

        private static Dictionary<UserData.ResourceId, BiEventAdventureIslandMerge.Types.Item> _resourcesBiDict =
            new Dictionary<UserData.ResourceId, BiEventAdventureIslandMerge.Types.Item>()
            {
                {UserData.ResourceId.Coin, BiEventAdventureIslandMerge.Types.Item.Coin},
                {UserData.ResourceId.Diamond, BiEventAdventureIslandMerge.Types.Item.Gem},
                {UserData.ResourceId.Energy, BiEventAdventureIslandMerge.Types.Item.Energy},
                {UserData.ResourceId.Infinity_Energy, BiEventAdventureIslandMerge.Types.Item.Energyunlimited},
                {UserData.ResourceId.BagToken, BiEventAdventureIslandMerge.Types.Item.BagCoupon},
                {UserData.ResourceId.HappyGo_Energy, BiEventAdventureIslandMerge.Types.Item.EnergyHappygo},
                {UserData.ResourceId.HappyGo, BiEventAdventureIslandMerge.Types.Item.HappygoExp},
                {UserData.ResourceId.RareDecoCoin, BiEventAdventureIslandMerge.Types.Item.Crystal},
                {UserData.ResourceId.Exp, BiEventAdventureIslandMerge.Types.Item.Exp},
                {UserData.ResourceId.RecoverCoinStar, BiEventAdventureIslandMerge.Types.Item.Star},
                {UserData.ResourceId.Mermaid, BiEventAdventureIslandMerge.Types.Item.Mermaid},
                {UserData.ResourceId.Easter2024Egg, BiEventAdventureIslandMerge.Types.Item.Easter2024Egg},
                {UserData.ResourceId.SnakeLadderTurntable, BiEventAdventureIslandMerge.Types.Item.SnakeLadderTurntable},
                {UserData.ResourceId.ThemeDecorationScore, BiEventAdventureIslandMerge.Types.Item.ThemeDecorationScore},
                {UserData.ResourceId.SlotMachineScore, BiEventAdventureIslandMerge.Types.Item.SlotMachineScore},
                {UserData.ResourceId.MonopolyDice, BiEventAdventureIslandMerge.Types.Item.MonopolyDice},
                {UserData.ResourceId.KeepPetDogFrisbee, BiEventAdventureIslandMerge.Types.Item.KeepPetDogFrisbee},
                {UserData.ResourceId.KeepPetDogSteak, BiEventAdventureIslandMerge.Types.Item.KeepPetDogSteak},
                {UserData.ResourceId.KeepPetDogDrumstick, BiEventAdventureIslandMerge.Types.Item.KeepPetDogDrumstick},
                {UserData.ResourceId.KeepPetDogHead, BiEventAdventureIslandMerge.Types.Item.KeepPetDogHead},
                {UserData.ResourceId.Turntable, BiEventAdventureIslandMerge.Types.Item.Turntable},
                {UserData.ResourceId.GardenShovel, BiEventAdventureIslandMerge.Types.Item.GardentreasureShovel},
                {UserData.ResourceId.GardenBomb, BiEventAdventureIslandMerge.Types.Item.GardentreasureBomb},
                {UserData.ResourceId.MixMasterCoffee, BiEventAdventureIslandMerge.Types.Item.MixMasterCoffee},
                {UserData.ResourceId.MixMasterTea, BiEventAdventureIslandMerge.Types.Item.MixMasterTea},
                {UserData.ResourceId.MixMasterMilk, BiEventAdventureIslandMerge.Types.Item.MixMasterMilk},
                {UserData.ResourceId.MixMasterLemonJuice, BiEventAdventureIslandMerge.Types.Item.MixMasterLemonJuice},
                {UserData.ResourceId.MixMasterIceCream, BiEventAdventureIslandMerge.Types.Item.MixMasterIceCream},
                {UserData.ResourceId.MixMasterCream, BiEventAdventureIslandMerge.Types.Item.MixMasterCream},
                {UserData.ResourceId.MixMasterPearl, BiEventAdventureIslandMerge.Types.Item.MixMasterPearl},
                {UserData.ResourceId.MixMasterSugar, BiEventAdventureIslandMerge.Types.Item.MixMasterSugar},
                {UserData.ResourceId.MixMasterIce, BiEventAdventureIslandMerge.Types.Item.MixMasterIce},
                {UserData.ResourceId.MixMasterExtra1, BiEventAdventureIslandMerge.Types.Item.MixMasterExtra1},
                {UserData.ResourceId.Water, BiEventAdventureIslandMerge.Types.Item.SaveTheWhalesWater},
                {UserData.ResourceId.TurtlePangPackage, BiEventAdventureIslandMerge.Types.Item.TurtlePangPackage},
                {UserData.ResourceId.StarrySkyCompassRocket, BiEventAdventureIslandMerge.Types.Item.StarrySkyCompassRocket},
                {UserData.ResourceId.BlindBox1, BiEventAdventureIslandMerge.Types.Item.BlindBox1},
                {UserData.ResourceId.BlindBox2, BiEventAdventureIslandMerge.Types.Item.BlindBox2},
                {UserData.ResourceId.BlindBox3, BiEventAdventureIslandMerge.Types.Item.BlindBox3},
                {UserData.ResourceId.BlindBox4, BiEventAdventureIslandMerge.Types.Item.BlindBox4},
                {UserData.ResourceId.BlindBox5, BiEventAdventureIslandMerge.Types.Item.BlindBox5},
                {UserData.ResourceId.BuyDiamondTicket1, BiEventAdventureIslandMerge.Types.Item.BuyDiamondTicket1},
                {UserData.ResourceId.ZumaBall, BiEventAdventureIslandMerge.Types.Item.ZumaBall},
                {UserData.ResourceId.ZumaBomb, BiEventAdventureIslandMerge.Types.Item.ZumaBomb},
                {UserData.ResourceId.ZumaLine, BiEventAdventureIslandMerge.Types.Item.ZumaWild},
                {UserData.ResourceId.KapibalaReborn, BiEventAdventureIslandMerge.Types.Item.KapibalaReborn},
                {UserData.ResourceId.KapibalaLife, BiEventAdventureIslandMerge.Types.Item.KapibalaLife},
                {UserData.ResourceId.Farm_Exp, BiEventAdventureIslandMerge.Types.Item.ExpFarm},
                {UserData.ResourceId.Farm_Wood, BiEventAdventureIslandMerge.Types.Item.WoodFarm},
                {UserData.ResourceId.Farm_Brush, BiEventAdventureIslandMerge.Types.Item.BrushFarm},
                {UserData.ResourceId.Farm_Brick, BiEventAdventureIslandMerge.Types.Item.BrickFarm},
                {UserData.ResourceId.Farm_Nail, BiEventAdventureIslandMerge.Types.Item.NailFarm},
                {UserData.ResourceId.Farm_NFertilizer, BiEventAdventureIslandMerge.Types.Item.NfertilizerFarm},
                {UserData.ResourceId.Farm_SFertilizer, BiEventAdventureIslandMerge.Types.Item.SfertilizerFarm},
                {UserData.ResourceId.Farm_NKettle, BiEventAdventureIslandMerge.Types.Item.NkettleFarm},
                {UserData.ResourceId.Farm_SKettle, BiEventAdventureIslandMerge.Types.Item.SkettleFarm},
                {UserData.ResourceId.Farm_Gear, BiEventAdventureIslandMerge.Types.Item.GearFarm},
                {UserData.ResourceId.Farm_Clock, BiEventAdventureIslandMerge.Types.Item.ClockFarm},
                {UserData.ResourceId.KapiScrewReborn, BiEventAdventureIslandMerge.Types.Item.KapiScrewReborn},
                {UserData.ResourceId.KapiScrewLife, BiEventAdventureIslandMerge.Types.Item.KapiScrewLife},
                {UserData.ResourceId.FishCultureScore, BiEventAdventureIslandMerge.Types.Item.FishCultureScore},
                {UserData.ResourceId.Prop_Back, BiEvent.Types.Item.PropBack},
                {UserData.ResourceId.Prop_SuperBack, BiEvent.Types.Item.PropSuperback},
                {UserData.ResourceId.Prop_Magic, BiEvent.Types.Item.PropMagic},
                {UserData.ResourceId.Prop_Shuffle, BiEvent.Types.Item.PropShuffle},
                {UserData.ResourceId.Prop_Extend, BiEvent.Types.Item.PropExtend},
                {UserData.ResourceId.KapiTileLife, BiEventAdventureIslandMerge.Types.Item.KapiTileLife},
                {UserData.ResourceId.KapiTileReborn, BiEventAdventureIslandMerge.Types.Item.KapiTileReborn},
                {UserData.ResourceId.PhotoAlbumScore, BiEventAdventureIslandMerge.Types.Item.PhotoAlbum},
                {UserData.ResourceId.JungleAdventure, BiEventAdventureIslandMerge.Types.Item.JungleAdventure},
                {UserData.ResourceId.TeamLife, BiEventAdventureIslandMerge.Types.Item.TeamLife},
                {UserData.ResourceId.TeamCoin, BiEventAdventureIslandMerge.Types.Item.TeamCoin},
                {UserData.ResourceId.PillowWheel, BiEventAdventureIslandMerge.Types.Item.PillowWheel},
                {UserData.ResourceId.CatchFish, BiEventAdventureIslandMerge.Types.Item.CatchFish},
            };

        /// <summary>
        ///  道具id 转 BI 统计的item 
        /// </summary>
        /// <param name="id">ResourceId </param>
        /// <returns> BI Item </returns>
        public static BiEventAdventureIslandMerge.Types.Item ResourceId2BIItem(UserData.ResourceId id)
        {
            if (_resourcesBiDict.ContainsKey(id) == false)
                return BiEventAdventureIslandMerge.Types.Item.Gem;
            return _resourcesBiDict[id];   
        }

        public static bool TryParseGameEventType(string UPPER_UNDERLINE_BI_DEF,
            out BiEventAdventureIslandMerge.Types.GameEventType gameEventType)
        {
            try
            {
                var biName = StringUtils.UPPER_UNDERLINE_2_UpperCamelCase(UPPER_UNDERLINE_BI_DEF);

                if (Enum.TryParse(biName, out gameEventType))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            DebugUtil.LogError($"TryParseGameEventType, parse error, string : {UPPER_UNDERLINE_BI_DEF}");
            gameEventType = default;
            //gameEventType = BiEventAdventureIslandMerge.Types.GameEventType.GameEventDownloadFailure;
            return false;
        }

        public static bool TryParseGameEventType(BiEventAdventureIslandMerge.Types.GameEventType gameEventType,
            out string UPPER_UNDERLINE_BI_DEF)
        {
            try
            {
                var enumType = typeof(BiEvent.Types.GameEventType);
                var memberInfos = enumType.GetMember(gameEventType.ToString());
                var enumValueMemberInfo = Array.Find(memberInfos, (m => m.DeclaringType == enumType));
                if (enumValueMemberInfo != null)
                {
                    var valueAttributes =
                        enumValueMemberInfo.GetCustomAttributes(
                            typeof(Google.Protobuf.Reflection.OriginalNameAttribute), false);
                    if (valueAttributes.Length > 0)
                    {
                        UPPER_UNDERLINE_BI_DEF = ((Google.Protobuf.Reflection.OriginalNameAttribute) valueAttributes[0])
                            .Name;
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            DebugUtil.LogError($"TryParseGameEventType, parse error, event : {gameEventType}");
            UPPER_UNDERLINE_BI_DEF = "ERROR";
            return false;
        }

        /// <summary>
        /// Send Ops Event :发送活动事件，可选事件
        /// </summary>
        /// <param name="message">IMessage </param>
        /// [Obsolete("SendEvent is changed to private method,Use SendOpsEvent instead")]
        public void SendOpsEvent(IMessage message)
        {
            SendBIEvent(message);
        }


        public void SendCommonOpsEvent(BiEventCommon.Types.CommonOpsEventType type,
            BiEventCommon.Types.CommonOpsEventAction eventAction, string eventId, params string[] args)
        {
            try
            {
                BIManager.Instance.SendCommonOpsEvent(type.ToString(), eventId, eventAction, args);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());
            }
        }


        public class MergeEventArgs
        {
            public BiEventAdventureIslandMerge.Types.MergeEventType MergeEventType;
            public int itemAId;
            public int ItemALevel;
            public int itemBId;
            public int itemBLevel;
            public int itemCId;
            public int itemCLevel;
            public bool isLock;
            public bool isChange;
            public string data1 = "";
            public string data2 = "";
            public string data3 = "";
            public Dictionary<string, string> extras;
        }

        public void SendMergeEvent(MergeEventArgs args)
        {
            var mergeEvent = new BiEventAdventureIslandMerge.Types.MergeEvent()
            {
                //todo: 参数填充
                MergeEventType = args.MergeEventType,
                ItemAId = args.itemAId.ToString(),
                ItemALevel = (uint) args.ItemALevel,
                ItemBId = args.itemBId.ToString(),
                ItemBLevel = (uint) args.itemBLevel,
                ItemCId = args.itemCId.ToString(),
                ItemCLevel = (uint) args.itemCLevel,
                IsLock = args.isLock,
                IsChange = args.isChange,
                Data1 = args.data1,
                Data2 = args.data2,
                Data3 = args.data3
            };

            mergeEvent.BagRemainCell = (uint) MergeManager.Instance.GetLeftBagCount(MergeBoardEnum.Main);
            mergeEvent.BagTotalCell = (uint) MergeManager.Instance.GetBagCapacity(MergeBoardEnum.Main);
            mergeEvent.BoardRemainCell = (uint) MergeManager.Instance.GetEmptBoardItemCount(MergeBoardEnum.Main);
            mergeEvent.StoreCell = args.data1 == "1"
                ? (uint) MergeManager.Instance.GetStorBoardItemCount(MergeBoardEnum.Main)
                : (uint) MergeManager.Instance.GetStorBoardItemCount(MergeBoardEnum.Main);
            if (args?.extras != null && args.extras.Count > 0)
            {
                foreach (var kvp in args.extras)
                {
                    mergeEvent.Extras.Add(kvp.Key, kvp.Value);
                }
            }

            SendBIEvent(mergeEvent);
        }

        /// <summary>
        /// Send Game Event
        /// </summary>
        /// <param name="type">事件类型 : DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge.Types.GameEventType</param>
        /// <param name="data1">不同事件需要记录的特殊数据</param>
        /// <param name="data2">不同事件需要记录的特殊数据</param>
        /// <param name="data3">不同事件需要记录的特殊数据</param>
        /// <param name="status">不同事件的操作状态，是否成功等</param>
        /// <param name="duration">来自home，下载时，记录下载资源用时（秒）</param>
        /// <param name="isAuto">是否自动弹出：true（自动弹出），false （点击弹出</param>
        public void SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType type, string data1 = null, string data2 = null,
            string data3 = null, Dictionary<string, string> extras = null, string status = null, ulong duration = 0,
            bool isAuto = false)
        {
            try
            {
                Action action = () =>
                {
                    // var gameEvent = new BiEvent.Types.GameEvent {
                    //         GameEventType = type,
                    //         IsAuto = isAuto,
                    //     };
                    var gameEvent = new BiEvent.Types.GameEvent();
                    gameEvent.GameEventType = type;
                    gameEvent.IsAuto = isAuto;
                    if (!string.IsNullOrEmpty(data1))
                    {
                        gameEvent.Data1 = data1;
                    }

                    if (!string.IsNullOrEmpty(data2))
                    {
                        gameEvent.Data2 = data2;
                    }

                    if (!string.IsNullOrEmpty(data3))
                    {
                        gameEvent.Data3 = data3;
                    }

                    if (extras != null && extras.Count > 0)
                    {
                        foreach (var kvp in extras)
                        {
                            gameEvent.Extras.Add(kvp.Key, kvp.Value);
                        }
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        gameEvent.Status = status;
                    }

                    SendBIEvent(gameEvent);
                };

                if (_IsFirstBI(type))
                {
                    if (!_IsFirstBIFinished(type))
                    {
                        _SetFirstBIFinished(type);
                        action();

                        DebugUtil.LogWarning("FTE Event Type: \t" + type);
                    }
                }
                else
                    action();
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError("BI SendGameEvent:" + e.Message);
                DebugUtil.LogError("BI SendGameEvent:" + e.StackTrace);
            }
        }

        private void _sendEvent(IMessage message)
        {
             var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance
                .GetStorage<DragonU3DSDK.Storage.StorageCommon>();
            var storageHome =
                DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHome>();
            var storageGame =
                DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageGame>();
            ulong Coins = 0;
            string key = UserData.Instance.GetCurrencyKey(UserData.ResourceId.Coin);
            if (storageHome.Currency.TryGetValue(key, out var number))
            {
                if (number != null)
                    Coins = (ulong) number.GetValue();
            }

            ulong Diamonds = 0;
            string key1 = UserData.Instance.GetCurrencyKey(UserData.ResourceId.Diamond);
            if (storageHome.Currency.TryGetValue(key1, out var number1))
            {
                if (number1 != null)
                    Diamonds = (ulong) number1.GetValue();
            }

            var storageMarketing = StorageManager.Instance.GetStorage<StorageCommon>().Marketing;
            int decNum = DaysManager.Instance.TotalNodeNum;
            int rvNum = 0;
            uint isHappyGo = 0;
            var common = new BiEvent.Types.Common
            {
                Coin = Coins,
                Diamond = Diamonds,
                Energy = (ulong) storageHome.Energy,
                Level = (uint) storageHome.Level,
                TotalTask = (ulong) storageGame.TaskGroups.CompleteNormalNum,
                CurrentRoomId = (uint) storageHome.CurRoomId,
                CurrentRoomCompletion = (uint) decNum,
                Adcommonid = (uint) AdConfigHandle.Instance.GetCommonID(),
                Ishappygo = isHappyGo,
                Energyfrenzy = EnergyTorrentModel.Instance.IsOpen(),
                Subusergroup = (uint)UserGroupManager.Instance.SubUserGroup,
                Serverusergroup = (uint)UserGroupManager.Instance.UserGroup,
                PayLevel = (uint)PayLevelModel.Instance.PlayLevel,
                DogLevel = (uint)KeepPetModel.Instance.GetLevel(),
                FarmLevel = (uint)FarmModel.Instance.GetLevel(),
                ScrewLevel = (uint)StorageManager.Instance.GetStorage<StorageScrew>().MainLevelIndex,
                OrderRulesId =  (uint)StorageManager.Instance.GetStorage<StorageHome>().OrderRules,
                Campaignlocal = storageMarketing != null && storageMarketing.Campaign != null ? storageMarketing.Campaign : "",
                Networklocal = storageMarketing != null && storageMarketing.Network != null ? storageMarketing.Network : "",
                Fbinstallreferrerlocal = storageMarketing != null && storageMarketing.FbInstallReferrer != null ? storageMarketing.FbInstallReferrer : "",
            };

            var BiEvent = new BiEvent {Common = common};

            var messageName = message.GetType().Name;
            BiEvent.GetType().InvokeMember(messageName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder,
                BiEvent, new object[] {message});

            DragonU3DSDK.Network.BI.BIManager.Instance.SendEvent(BiEvent);
        }
        public void SendGameEventImmediately(BiEventAdventureIslandMerge.Types.GameEventType type, string data1 = null, string data2 = null,
            string data3 = null, Dictionary<string, string> extras = null, string status = null, ulong duration = 0,
            bool isAuto = false)
        {
            try
            {
                Action action = () =>
                {
                    // var gameEvent = new BiEvent.Types.GameEvent {
                    //         GameEventType = type,
                    //         IsAuto = isAuto,
                    //     };
                    var gameEvent = new BiEvent.Types.GameEvent();
                    gameEvent.GameEventType = type;
                    isAuto = isAuto;
                    if (!string.IsNullOrEmpty(data1))
                    {
                        gameEvent.Data1 = data1;
                    }

                    if (!string.IsNullOrEmpty(data2))
                    {
                        gameEvent.Data2 = data2;
                    }

                    if (!string.IsNullOrEmpty(data3))
                    {
                        gameEvent.Data3 = data3;
                    }

                    if (extras != null && extras.Count > 0)
                    {
                        foreach (var kvp in extras)
                        {
                            gameEvent.Extras.Add(kvp.Key, kvp.Value);
                        }
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        gameEvent.Status = status;
                    }

                    SendBIEventImmediately(gameEvent);
                };

                if (_IsFirstBI(type))
                {
                    if (!_IsFirstBIFinished(type))
                    {
                        _SetFirstBIFinished(type);
                        action();

                        DebugUtil.LogWarning("FTE Event Type: \t" + type);
                    }
                }
                else
                    action();
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError("BI SendGameEvent:" + e.Message);
                DebugUtil.LogError("BI SendGameEvent:" + e.StackTrace);
            }
        }

        public void SendHomeGameEvent(BiEventAdventureIslandMerge.Types.GameEventType type, string data1 = null, string data2 = null,
            string data3 = null, string data4 = null)
        {
            try
            {
                Action action = () =>
                {
                    // var gameEvent = new BiEvent.Types.GameEvent {
                    //         GameEventType = type,
                    //         IsAuto = isAuto,
                    //     };
                    var gameEvent = new BiEvent.Types.GameEvent();
                    gameEvent.GameEventType = type;
                    if (!string.IsNullOrEmpty(data1))
                    {
                        gameEvent.Data1 = data1;
                    }

                    if (!string.IsNullOrEmpty(data2))
                    {
                        gameEvent.Data2 = data2;
                    }

                    if (!string.IsNullOrEmpty(data3))
                    {
                        gameEvent.Data3 = data3;
                    }

                    SendBIEvent(gameEvent);
                };

                if (_IsFirstBI(type))
                {
                    if (!_IsFirstBIFinished(type))
                    {
                        _SetFirstBIFinished(type);
                        action();

                        DebugUtil.LogWarning("FTE Event Type: \t" + type);
                    }
                }
                else
                    action();
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError("BI SendGameEvent:" + e.Message);
                DebugUtil.LogError("BI SendGameEvent:" + e.StackTrace);
            }
        }

        /// <summary>
        ///  道具TYPE 转 BI 统计的item 
        /// </summary>
        /// <param name="type">ItemType </param>
        /// <returns> BI Item </returns>
        public static BiEventAdventureIslandMerge.Types.Item ResourceTMId2BIItem(TMatch.ItemType type)
        {
            if (type ==  TMatch.ItemType.TMCoin)
                return BiEventAdventureIslandMerge.Types.Item.CoinTm;
            if (type ==  TMatch.ItemType.TMEnergy)
                return BiEventAdventureIslandMerge.Types.Item.EnergyTm;
            if (type ==  TMatch.ItemType.TMStar)
                return BiEventAdventureIslandMerge.Types.Item.StarTm;
            if (type ==  TMatch.ItemType.TMMagnet)
                return BiEventAdventureIslandMerge.Types.Item.MagnetTm;
            if (type ==  TMatch.ItemType.TMBroom)
                return BiEventAdventureIslandMerge.Types.Item.BroomTm;
            if (type ==  TMatch.ItemType.TMWindmill)
                return BiEventAdventureIslandMerge.Types.Item.WindmillTm;
            if (type ==  TMatch.ItemType.TMFrozen)
                return BiEventAdventureIslandMerge.Types.Item.FrozenTm;
            if (type ==  TMatch.ItemType.TMLighting)
                return BiEventAdventureIslandMerge.Types.Item.LightingTm;
            if (type ==  TMatch.ItemType.TMClock)
                return BiEventAdventureIslandMerge.Types.Item.ClockTm;
            if (type ==  TMatch.ItemType.TMEnergyInfinity)
                return BiEventAdventureIslandMerge.Types.Item.EnergyInfinityTm;
            if (type ==  TMatch.ItemType.TMLightingInfinity)
                return BiEventAdventureIslandMerge.Types.Item.LightingInfinityTm;
            if (type ==  TMatch.ItemType.TMClockInfinity)
                return BiEventAdventureIslandMerge.Types.Item.ClockInfinityTm;
            if (type ==  TMatch.ItemType.TMInfinityActivityCollect)
                return BiEventAdventureIslandMerge.Types.Item.InfinityActivityCollectTm;
            if (type ==  TMatch.ItemType.TMWeeklyChallengeCollect)
                return BiEventAdventureIslandMerge.Types.Item.WeeklyChallengeCollectTm;
            if (type ==  TMatch.ItemType.TMWeeklyChallengeBuff)
                return BiEventAdventureIslandMerge.Types.Item.WeeklyChallengeBuffTm;
            return BiEventAdventureIslandMerge.Types.Item.CoinTm;
        }
        /// <summary>
        /// Send Item Change Event
        /// </summary>
        /// <param name="item">事件类型 : DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge.Types.Item</param>
        /// <param name="amount">变化数</param>
        /// <param name="current">当前数量</param>
        /// <param name="args">ItemChangeReasonArgs</param>
        public void SendTMItemChangeEvent(TMatch.ItemType itemType, long amount, ulong current,
            ItemChangeReasonArgs args)
        {
            try
            {
                var itemChangeEvent = new BiEvent.Types.ItemChange
                {
                    Item = ResourceTMId2BIItem(itemType),
                    Reason = args.reason,
                    Amount = amount,
                    Current = current,
                    BoostId = args.boostId,
                };
                if (!string.IsNullOrEmpty(args.data1))
                {
                    itemChangeEvent.Data1 = args.data1;
                }

                if (!string.IsNullOrEmpty(args.data2))
                {
                    itemChangeEvent.Data2 = args.data2;
                }

                if (!string.IsNullOrEmpty(args.data3))
                {
                    itemChangeEvent.Data3 = args.data3;
                }

                SendBIEvent(itemChangeEvent);
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError("BI SendItemChangeEvent:" + e.Message);
                DebugUtil.LogError("BI SendItemChangeEvent:" + e.StackTrace);
            }
        }
        /// <summary>
        /// Send Item Change Event
        /// </summary>
        /// <param name="item">事件类型 : DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge.Types.Item</param>
        /// <param name="amount">变化数</param>
        /// <param name="current">当前数量</param>
        /// <param name="args">ItemChangeReasonArgs</param>
        public void SendItemChangeEvent(UserData.ResourceId resId, long amount, ulong current,
            ItemChangeReasonArgs args)
        {
            try
            {
                var itemChangeEvent = new BiEvent.Types.ItemChange
                {
                    Item = ResourceId2BIItem(resId),
                    Reason = args.reason,
                    Amount = amount,
                    Current = current,
                    BoostId = args.boostId,
                };
                if (!string.IsNullOrEmpty(args.data1))
                {
                    itemChangeEvent.Data1 = args.data1;
                }

                if (!string.IsNullOrEmpty(args.data2))
                {
                    itemChangeEvent.Data2 = args.data2;
                }

                if (!string.IsNullOrEmpty(args.data3))
                {
                    itemChangeEvent.Data3 = args.data3;
                }

                SendBIEvent(itemChangeEvent);
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError("BI SendItemChangeEvent:" + e.Message);
                DebugUtil.LogError("BI SendItemChangeEvent:" + e.StackTrace);
            }
        }

        public void SendDecoEvent_enter_room(int roomId)
        {
            var decoEevent = new BiEventAdventureIslandMerge.Types.Decoration();
            decoEevent.RoomId = (uint) roomId;
            decoEevent.Feature = "enter_room";
            SendDecoEvent(decoEevent);
        }

        public void SendDecoEvent_purchase_style(int roomId, int nodeId, int keyCost)
        {
            var decoEevent = new BiEventAdventureIslandMerge.Types.Decoration();
            decoEevent.RoomId = (uint) roomId;
            decoEevent.RoomNodeId = (ulong) nodeId;
            decoEevent.RoomNodeKeyCost = (ulong) keyCost;
            decoEevent.Feature = "purchase_style";
            SendDecoEvent(decoEevent);
        }

        public void SendDecoEvent_ChangeItem(int roomId, int nodeId, string itemId, bool isDefault)
        {
            var decoEevent = new BiEventAdventureIslandMerge.Types.Decoration();
            decoEevent.RoomId = (uint) roomId;
            decoEevent.RoomNodeId = (uint) nodeId;
            decoEevent.RoomNodeItemId = itemId;
            decoEevent.Feature = "change_style";
            decoEevent.IsDefault = isDefault;
            SendDecoEvent(decoEevent);
        }

        public void SendDecoEvent_complete_room(int roomId)
        {
            var decoEevent = new BiEventAdventureIslandMerge.Types.Decoration();
            decoEevent.Feature = "complete_room";
            decoEevent.RoomId = (uint) roomId;
            SendDecoEvent(decoEevent);
        }

        public void SendDecoEvent(BiEvent.Types.Decoration decoration)
        {
            try
            {
                SendBIEvent(decoration);
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError("BI SendDecoEvent:" + e.Message);
                DebugUtil.LogError("BI SendDecoEvent:" + e.StackTrace);
            }
        }

        public void SendBIEvent(IMessage message)
        {
            if (!DragonU3DSDK.Storage.StorageManager.Instance.Inited)
            {
                messages.Enqueue(message);
                return;
            }

            _sendEvent(message);
        }
        
        private void SendBIEventImmediately(IMessage message)
        {
            var storageCommon = DragonU3DSDK.Storage.StorageManager.Instance
                .GetStorage<DragonU3DSDK.Storage.StorageCommon>();
            var storageHome =
                DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageHome>();
            var storageGame =
                DragonU3DSDK.Storage.StorageManager.Instance.GetStorage<DragonU3DSDK.Storage.StorageGame>();
            ulong Coins = 0;
            string key = UserData.Instance.GetCurrencyKey(UserData.ResourceId.Coin);
            if (storageHome.Currency.TryGetValue(key, out var number))
            {
                if (number != null)
                    Coins = (ulong) number.GetValue();
            }

            ulong Diamonds = 0;
            string key1 = UserData.Instance.GetCurrencyKey(UserData.ResourceId.Diamond);
            if (storageHome.Currency.TryGetValue(key1, out var number1))
            {
                if (number1 != null)
                    Diamonds = (ulong) number1.GetValue();
            }

            var storageMarketing = StorageManager.Instance.GetStorage<StorageCommon>().Marketing;
            int decNum = DaysManager.Instance.TotalNodeNum;
            int rvNum = 0;
            uint isHappyGo = 0;
            var common = new BiEvent.Types.Common
            {
                Coin = Coins,
                Diamond = Diamonds,
                Energy = (ulong) storageHome.Energy,
                Level = (uint) storageHome.Level,
                TotalTask = (ulong) storageGame.TaskGroups.CompleteNormalNum,
                CurrentRoomId = (uint) storageHome.CurRoomId,
                CurrentRoomCompletion = (uint) decNum,
                Adcommonid = (uint) AdConfigHandle.Instance.GetCommonID(),
                Ishappygo = isHappyGo,
                Energyfrenzy = EnergyTorrentModel.Instance.IsOpen(),
                Subusergroup = (uint)UserGroupManager.Instance.SubUserGroup,
                Serverusergroup = (uint)UserGroupManager.Instance.UserGroup,
                PayLevel = (uint)PayLevelModel.Instance.PlayLevel,
                DogLevel = (uint)KeepPetModel.Instance.GetLevel(),
                FarmLevel = (uint)FarmModel.Instance.GetLevel(),
                ScrewLevel = (uint)StorageManager.Instance.GetStorage<StorageScrew>().MainLevelIndex,
                OrderRulesId =  (uint)StorageManager.Instance.GetStorage<StorageHome>().OrderRules,
                Campaignlocal = storageMarketing != null && storageMarketing.Campaign != null ? storageMarketing.Campaign : "",
                Networklocal = storageMarketing != null && storageMarketing.Network != null ? storageMarketing.Network : "",
                Fbinstallreferrerlocal = storageMarketing != null && storageMarketing.FbInstallReferrer != null ? storageMarketing.FbInstallReferrer : "",
            };

            var BiEvent = new BiEvent {Common = common};

            var messageName = message.GetType().Name;
            BiEvent.GetType().InvokeMember(messageName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, Type.DefaultBinder,
                BiEvent, new object[] {message});
            SendEventImmediately(BiEvent);
            // DragonU3DSDK.Network.BI.BIManager.Instance.SendEvent(BiEvent);
        }

        public void SendEventImmediately(IMessage specificMsg)
        {
            var ev = new DragonU3DSDK.Network.API.Protocol.BiEvent { };
            //ev.Common = getCommon();
            ev.PayloadType = specificMsg.GetType().Name;
            ev.Payload = specificMsg.ToByteString();
            
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            var cSendEvents = new CSendEvents();
            if (ev.Common == null)
            {
                ev.Common = getCommon();
            }

            if (storageCommon != null)
            {
                ev.Common.PlayerId = storageCommon.PlayerId;
            }
            if (DragonU3DSDK.Asset.ChangeableConfig.Instance.IsTargetPlayer())
            {
                ev.Common.Abtests = DragonU3DSDK.Asset.ChangeableConfig.Instance.GetGroups();
            }

#if ABTEST_ENABLE
                        try
                        {
                            if (storageCommon.Abtests != null)
                            {
                                foreach (var kv in storageCommon.Abtests)
                                {
                                    ev.Common.AbtestMap.Add(kv.Key, kv.Value);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            DebugUtil.LogError(e.ToString());
                        }
                        
#endif

            cSendEvents.BiEvents.Add(ev);
            if (APIManager.Instance.HasNetwork)
            {
                cSendEvents.SentAt = DeviceHelper.CurrentTimeMillis();
                APIManager.Instance.Send(cSendEvents, (SSendEvents sSendEvents) =>
                {
                }, (errno, errmsg, resp) =>
                {
                });
            }
        }

        DragonU3DSDK.Network.API.Protocol.BiEvent.Types.Common getCommon()
        {
            if (!StorageManager.Instance.Inited)
            {
                return null;
            }
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            if (storageCommon == null)
            {
                return null;
            }

            var common = new DragonU3DSDK.Network.API.Protocol.BiEvent.Types.Common();
            common.PlayerId = storageCommon.PlayerId;
            common.InstalledAt = storageCommon.InstalledAt;
            common.RevenueUsdCents = storageCommon.RevenueUSDCents;
            common.Platform = DeviceHelper.GetPlatform();

            if (DragonU3DSDK.Account.AccountManager.Instance.HasBindFacebook())
            {
                common.PlayerType = PlayerType.Facebook;
            }
            else if (DragonU3DSDK.Account.AccountManager.Instance.HasBindEmail())
            {
                common.PlayerType = PlayerType.Email;
            }
            else
            {
                common.PlayerType = PlayerType.Guest;
            }

            if (!string.IsNullOrEmpty(storageCommon.Name))
            {
                common.PlayerName = storageCommon.Name;
            }

            if (!string.IsNullOrEmpty(storageCommon.Email))
            {
                common.PlayerEmail = storageCommon.Email;
            }

            if (!string.IsNullOrEmpty(storageCommon.FacebookId))
            {
                common.FacebookId = storageCommon.FacebookId;
            }

            if (!string.IsNullOrEmpty(storageCommon.FacebookEmail))
            {
                common.FacebookEmail = storageCommon.FacebookEmail;
            }

            if (!string.IsNullOrEmpty(storageCommon.FacebookName))
            {
                common.FacebookName = storageCommon.FacebookName;
            }

            if (!string.IsNullOrEmpty(storageCommon.Adid))
            {
                common.Adid = storageCommon.Adid;
            }
            if (!string.IsNullOrEmpty(storageCommon.Idfa))
            {
                common.Idfa = storageCommon.Idfa;
            }

            if (!string.IsNullOrEmpty(storageCommon.AdjustId))
            {
                common.AdjustId = storageCommon.AdjustId;
            }

            if (!string.IsNullOrEmpty(storageCommon.Country))
            {
                common.DeviceCountry = storageCommon.Country;
            }

            common.DeviceTimezone = TimeZoneInfo.Local.BaseUtcOffset.TotalHours.ToString();
            if (!string.IsNullOrEmpty(storageCommon.ResVersion))
            {
                common.ResourceVersion = storageCommon.ResVersion;
            }

            common.Member = false; // TODO
            common.MemberAge = 0; // TODO
            common.MemberTime = 0; // TODO
#if DEBUG
            common.Environment = "test";
#else
            common.Environment = "prod";
#endif
            common.ClientVersion = DragonNativeBridge.GetVersionCode().ToString();
            common.ClientVersionName = DragonNativeBridge.GetVersionName().ToString();

            //common.CreatedAt = DeviceHelper.CurrentTimeMillis();
            common.CreatedAt = APIManager.Instance.GetServerTime();
            common.DeviceId = DeviceHelper.GetDeviceId();
            common.DeviceOsName = DeviceHelper.GetOSName();
            common.DeviceOsVersion = DeviceHelper.GetOSVersion();
            common.DeviceScreenResolution = DeviceHelper.GetResolution();
            common.DeviceLanguage = DeviceHelper.GetLanguage();
            common.DeviceMemory = DeviceHelper.GetTotalMemory().ToString();
            common.DeviceModel = DeviceHelper.GetDeviceModel();
            common.DeviceType = DeviceHelper.GetDeviceType();
            common.NetworkType = DeviceHelper.GetNetworkStatus().ToString();
            common.Ip = DeviceHelper.GetLocalIp();

            common.LocalVersion = StorageManager.Instance.LocalVersion;
            common.RemoteVersionAck = StorageManager.Instance.RemoteVersionACK;
            common.RemoteVersionLocal = StorageManager.Instance.RemoteVersionSYN;

            common.HasLogin = DragonU3DSDK.Account.AccountManager.Instance.HasLogin;
            common.HasNetwork = APIManager.Instance.HasNetwork;

            common.UserGroupAd = (uint)storageCommon.AdsPredictUserGroup;

            //项目分层配置未使用SDK管理，无法获取用户分组信息，由项目自行设置
            common.UserGroups.AddAllKVPFrom(DragonPlus.ConfigHub.ConfigHubManager.Instance.GetStatusInfo());
            common.CampaignTypeCode = storageCommon.CampaignTypeCode;
            common.Sequence = 0;
            return common;
        }
        
        private void SendFirebase(BiEventAdventureIslandMerge.Types.GameEventType gameEventType, string data1)
        {
            var firebase = Dlugin.SDK.GetInstance().firebase;
            firebase?.TrackEvent(gameEventType.ToString(), 0, data1);
        }

        private void SendAdjust(string UPPER_UNDERLINE_BI_DEF)
        {
            var adjust = Dlugin.SDK.GetInstance().adjustPlugin;
            adjust?.TrackEvent(UPPER_UNDERLINE_BI_DEF, 0, "{}");
        }

        private bool _IsFirstBI(BiEventAdventureIslandMerge.Types.GameEventType et)
        {
            return et.ToString().Contains("Ftue");
        }

        private static bool _IsFirstBIFinished(BiEventAdventureIslandMerge.Types.GameEventType et)
        {
            return StorageManager.Instance.GetStorage<StorageHome>().BiEvents.ContainsKey((int) et);
        }

        private static void _SetFirstBIFinished(BiEventAdventureIslandMerge.Types.GameEventType et)
        {
            var biStorage = StorageManager.Instance.GetStorage<StorageHome>().BiEvents;
            if (!biStorage.ContainsKey((int) et))
            {
                biStorage.Add((int) et, (int) et);
            }
        }

        public void SendThirdPartyEvent(string evt)
        {
            DebugUtil.Log($"Try send ThirdParty {evt}");
            if (FB.IsInitialized) FB.LogAppEvent(evt);
            BIManager.Instance.onThirdPartyTracking(evt);
        }


        #region ThirdPartyTracking

        private Dictionary<int, string> _reachLevelBIDict = new Dictionary<int, string>()
        {
            {5, "GAME_EVENT_Reach_LV5"},
            {6, "GAME_EVENT_Reach_LV6"},
            {8, "GAME_EVENT_Reach_LV8"},
            {10, "GAME_EVENT_Reach_LV10"},
            {12, "GAME_EVENT_Reach_LV12"},
            {15, "GAME_EVENT_Reach_LV15"},
        };
        
        public void SendReachLevelThirdBI(int level)
        {
#if UNITY_EDITOR
            return;
#endif
            if (_reachLevelBIDict == null)
                return;
            if (_reachLevelBIDict.ContainsKey(level) == false)
                return;
            SendThirdPartyEvent(_reachLevelBIDict[level]);
        }
        
        private Dictionary<int, string> _completeAreaBIDict = new Dictionary<int, string>()
        {
            {101, "GAME_EVENT_Complete_area1"},
            {102, "GAME_EVENT_Complete_area2"},
            {103, "GAME_EVENT_Complete_area3"},
            {104, "GAME_EVENT_Complete_area4"},
        };

        public void SendAreaCompleteThirdBI(int areaIdx)
        {
            if (_completeAreaBIDict == null)
                return;
            if (_completeAreaBIDict.ContainsKey(areaIdx) == false)
                return;
            SendThirdPartyEvent(_completeAreaBIDict[areaIdx]);
        }

        private Dictionary<float, string> _completePurchaseBIDict = new Dictionary<float, string>()
        {
            {1.99f, "PURCHASE_ONETIME_1_99"},
            {2.99f, "PURCHASE_ONETIME_2_99"},
            {4.99f, "PURCHASE_ONETIME_4_99"},
            {9.99f, "PURCHASE_ONETIME_9_99"},
            {11.99f, "PURCHASE_ONETIME_11_99"},
            {13.99f, "PURCHASE_ONETIME_13_99"},
            {19.99f, "PURCHASE_ONETIME_19_99"},
            {24.99f, "PURCHASE_ONETIME_24_99"},
        };

        private Dictionary<float, string> _iapDayBi = new Dictionary<float, string>()
        {
            {299, "IAPLTV_7DAY_3"},
            {199, "IAPLTV_7DAY_2"},
            {99, "IAPLTV_7DAY_1"},
        };
       public void SendPurchaseCompleteThirdBI(float price)
       {
           //SendIapDayBi();");
           ulong priceValue = (ulong)(price * 100);
           BIManager.Instance.TrackIAPLTV("IAPLTV_7DAY_1",priceValue,99,7);
           BIManager.Instance.TrackIAPLTV("IAPLTV_7DAY_2",priceValue,199,7);
           BIManager.Instance.TrackIAPLTV("IAPLTV_7DAY_3",priceValue,299,7);
           
           if (_completePurchaseBIDict == null)
               return;
           if (_completePurchaseBIDict.ContainsKey(price) == false)
               return;
           SendThirdPartyEvent(_completePurchaseBIDict[price]);
       }

        private void SendIapDayBi()
        {
            if (UserGroupManager.Instance.TimeAfterInstall.TotalHours <= 168)
            {
                ulong RevenueUSDCents = StorageManager.Instance.GetStorage<StorageCommon>().RevenueUSDCents;
                foreach (var kv in _iapDayBi)
                {
                    if(RevenueUSDCents < kv.Key)
                        continue;
                    
                    SendThirdPartyEvent(kv.Value);
                }
            }
        }
        private Dictionary<int, string> _rvBiDict = new Dictionary<int, string>()
        {
            {12, "RV_DIDREWARD_STAGE_12"},
            {20, "RV_DIDREWARD_STAGE_20"},
        };

        public void SendRvThirdBI(int rvPlayCount)
        {
            if (_rvBiDict == null)
                return;
            if (_rvBiDict.ContainsKey(rvPlayCount) == false)
                return;
            SendThirdPartyEvent(_rvBiDict[rvPlayCount]);
        }

        #endregion
    }
}