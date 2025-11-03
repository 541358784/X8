using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using Google.Protobuf;
using UnityEngine;
using BiEvent = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
using DragonU3DSDK;
using DragonU3DSDK.Account;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.BI;
using DragonU3DSDK.Storage;
using Facebook.Unity;
using Framework;
using Gameplay;

namespace DragonPlus
{
    public partial class GameBIManager : MonoBehaviour
    {
        public class GameEventArgs
        {
            public uint level_count; //当前关卡数
            public uint level_id; //当前关卡ID
            public uint level_difficulty; //关卡难度
            public uint enter_time;//进入次数
            public uint initial_thing_count;//原始物品数量
            public uint remove_thing_count;//消除物品数量
            public uint left_thing_count;//剩余物品数量
            public uint total_time;//关卡原始时长（秒）
            public uint left_time_count;//关卡结束剩余时长（秒）
            public uint level_time;//统计用户在关卡内的总时长(秒)
            public uint fail_reason;//返回关卡失败的原因，（1:中途退出，用户主动点击退出按钮退出；2:关卡失败，格子占满；3:倒计时结束导致的关卡失败；4:机关触发失败）
            public uint use_prop_back;//使用撤回数量
            public uint use_prop_superback;//使用超级撤回数量
            public uint use_prop_magic;//使用魔法棒数量
            public uint use_prop_shuffle;//使用洗牌数量
            public uint use_prop_extend;//使用格子+1数量
            public uint get_prop_back;//获得撤回数量
            public uint get_prop_superback;//获得超级撤回数量
            public uint get_prop_magic;//获得魔法棒数量
            public uint get_prop_shuffle;//获得洗牌数量
            public uint get_prop_extend;//获得格子+1数量
            public bool energy_infinite;//是否是无限体力
            public uint space_respawn_count;//格子满金币复活次数
            public uint space_respawn_ad_count;//格子满广告复活次数
            public uint time_respawn_count;//时间到金币复活次数
            public uint time_respawn_ad_count;//时间到广告复活次数
            public string data1;//不同事件需要记录的特殊数据
            public string data2;//不同事件需要记录的特殊数据
            public string data3;//不同事件需要记录的特殊数据
            public bool random_level;//是否随机关卡
        }
        public void SendGameMatchEvent(BiEvent.Types.GameEventType type, GameEventArgs args)
        {
            try
            {
                Action action = () =>
                {
                    var gameEvent = new BiEvent.Types.GameEvent();
                    gameEvent.GameEventType = type;
                    if (args != null)
                    {
                        // gameEvent.LevelCount = args.level_count;
                        // gameEvent.LevelId = args.level_id;
                        // gameEvent.LevelDifficulty = args.level_difficulty;
                        // gameEvent.EnterTime = args.enter_time;
                        // gameEvent.InitialThingCount = args.initial_thing_count;
                        // gameEvent.RemoveThingCount = args.remove_thing_count;
                        // gameEvent.LeftThingCount = args.left_thing_count;
                        // gameEvent.TotalTime = args.total_time;
                        // gameEvent.LeftTimeCount = args.left_time_count;
                        // gameEvent.LevelTime = args.level_time;
                        // gameEvent.FailReason = args.fail_reason;
                        // gameEvent.UsePropBack = args.use_prop_back;
                        // gameEvent.UsePropSuperback = args.use_prop_superback;
                        // gameEvent.UsePropMagic = args.use_prop_magic;
                        // gameEvent.UsePropShuffle = args.use_prop_shuffle;
                        // gameEvent.UsePropExtend = args.use_prop_extend;
                        // gameEvent.GetPropBack = args.get_prop_back;
                        // gameEvent.GetPropSuperback = args.get_prop_superback;
                        // gameEvent.GetPropMagic = args.get_prop_magic;
                        // gameEvent.GetPropShuffle = args.get_prop_shuffle;
                        // gameEvent.GetPropExtend = args.get_prop_extend;
                        // gameEvent.EnergyInfinite = args.energy_infinite;
                        // gameEvent.SpaceRespawnCount = args.space_respawn_count;
                        // gameEvent.SpaceRespawnAdCount = args.space_respawn_ad_count;
                        // gameEvent.TimeRespawnCount = args.time_respawn_count;
                        // gameEvent.TimeRespawnAdCount = args.time_respawn_ad_count;
                        // gameEvent.RandomLevel = args.random_level;
                        
                        if(!args.data1.IsEmptyString())
                            gameEvent.Data1 = args.data1;
                        
                        if(!args.data2.IsEmptyString())
                            gameEvent.Data2 = args.data2;
                        
                        if(!args.data3.IsEmptyString())
                            gameEvent.Data3 = args.data3; 
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
    }
}