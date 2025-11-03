
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace TMatch
{
    public partial class eMapMode
    {
        public const string Main = "Main";
    }

    public enum OpenType
    {
        ArriveLevel = 1,
        PassLevel = 2, //1通过关卡
        PlayerType = 3, //2玩家类型(1付费2免费)
        GreaterThanRegisterTime = 4, //3注册时长(大于秒)
        LessThanRegisterTime = 5, //4注册时长(小于秒)
        LessThanLevel = 6, //5小于关卡
    }

    public static class GameUtils
    {
        public static int GetLevelIdCur()
        {
            return ClientMgr.Instance.CurLevelId;
        }
        public static string GetMapModeCur()
        {
            return eMapMode.Main;
        }
        
        /// <summary>
        /// 传入levelid，获得DisplayID, 例如  12-1
        /// </summary>
        // public static string GetDisplayIDByLevelId(int levelId)
        // {
        //     var localLevelID = Mathf.Max(levelId, 1011);
        //     var mapId = Utils.GetMapId(localLevelID);
        //     return mapId.ToString();
        // }
        
        public static bool IsMapCompleted(int mapId)
        {
            return true;
        }

        public static bool IsLevelPass(int levelId)
        {
            return ClientMgr.Instance.MainMaxLevel >= levelId;
        }

        public static bool IsLevelArrive(int levelId)
        {
            return ClientMgr.Instance.CurLevelId >= levelId;
        }

        public static bool IsLevelUnlock(int levelId)
        {
            return true;
        }
        
        // 基础筛选条件判断
        public static bool TryCheckOpenType(List<int> openType, List<int> openTypeParam, out string logStr)
        {
            logStr = string.Empty;
            
            if (openType == null || openType.Count == 0)
                return true;

            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();
            var result = true;
            for (var i = 0; i < openType.Count; i++)
            {
                var offerOpenType = (OpenType) openType[i];
                switch (offerOpenType)
                {
                    case OpenType.ArriveLevel:
                        if (!IsLevelArrive(openTypeParam[i]))
                        {
                            logStr += "不满足开启条件: 未满足最大关卡";
                            result = false;
                        }
                        break;
                    case OpenType.PassLevel:
                        if (!IsLevelPass(openTypeParam[i]))
                        {
                            logStr += "不满足开启条件: 未满足最大关卡";
                            result = false;
                        }
                        break;
                    case OpenType.PlayerType:
                        if (openTypeParam[i] == 1 && storageCommon.RevenueUSDCents <= 0
                            || openTypeParam[i] == 2 && storageCommon.RevenueUSDCents > 0)
                        {
                            logStr += "不满足开启条件: 是否未付费玩家不满足";
                            result = false;
                        }
                        break;
                    case OpenType.GreaterThanRegisterTime:
                        if ((ulong) openTypeParam[i] >
                            ((ulong) DragonU3DSDK.Utils.GetTimeStamp() - storageCommon.InstalledAt) / 1000)
                        {
                            logStr += "不满足开启条件: 不满足最大注册时间";
                            result = false;
                        }
                        break;
                    case OpenType.LessThanRegisterTime:
                        if ((ulong) openTypeParam[i] <
                            ((ulong) DragonU3DSDK.Utils.GetTimeStamp() - storageCommon.InstalledAt) / 1000)
                        {
                            logStr += "不满足开启条件: 不满足最小注册时间";
                            result = false;
                        }
                        break;
                    case OpenType.LessThanLevel:
                        if (GetLevelIdCur() >= openTypeParam[i])
                        {
                            logStr += "不满足开启条件: 小于某一关卡";
                            result = false;
                        }
                        break;
                }
            }

            return result;
        }

        // 基础筛选条件判断
        public static bool CheckOpenType(string openType, string openTypeParam, out string logStr)
        {
            logStr = string.Empty;
            
            if (string.IsNullOrEmpty(openType) || string.IsNullOrEmpty(openTypeParam))
                return true;

            return TryCheckOpenType(
                CommonUtils.StringToIntList(openType),
                CommonUtils.StringToIntList(openTypeParam),
                out logStr);
        }
        
        public static bool CheckOpenType(List<int> openType, List<int> openTypeParam)
        {
            return TryCheckOpenType(openType, openTypeParam, out _);
        }

        public static bool CheckOpenType(int openType, int openTypeParam)
        {
            return CheckOpenType(new List<int> {openType}, new List<int> {openTypeParam});
        }
    }
}