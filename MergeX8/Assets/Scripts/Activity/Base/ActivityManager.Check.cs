using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using Framework;
using UnityEngine;

public enum ActiveityOpenType
{
    PassLevel = 1, //1通过关卡
    PlayerType = 2, //2玩家类型(1付费2免费)
    GreaterThanRegisterTime = 3, //3注册时长(大于秒)
    LessThanRegisterTime = 4, //4注册时长(小于秒)
    LessThanLevel = 5, //5小于关卡
    ConflictStarterPack = 6 //是否与新手礼包冲突
}

public partial class ActivityManager
{
    /// <summary>
    /// 基础筛选条件判断
    /// </summary>
    /// <param name="openType"></param>
    /// <param name="openTypeParam"></param>
    /// <param name="logStr"></param>
    /// <param name="log"></param>
    /// <returns></returns>
    public bool CheckOpenType(List<int> openType, List<int> openTypeParam, string logStr = "", bool log = true)
    {
        if (openType == null)
            return true;

        StorageCommon storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

        if (logStr == null) logStr = string.Empty;

        var result = true;
        for (var i = 0; i < openType.Count; i++)
        {
            var offerOpenType = (ActiveityOpenType) openType[i];
            switch (offerOpenType)
            {
                case ActiveityOpenType.PassLevel:
                    if (ExperenceModel.Instance.GetLevel() < openTypeParam[i])
                    {
                        if (log) logStr += "不满足开启条件: 未满足最大关卡";
                        result = false;
                    }

                    break;
                case ActiveityOpenType.GreaterThanRegisterTime:
                    if ((ulong) openTypeParam[i] >
                        ((ulong) TimeUtils.GetTimeStamp() - storageCommon.InstalledAt) / 1000)
                    {
                        if (log) logStr += "不满足开启条件: 不满足最大注册时间";

                        result = false;
                    }

                    break;
                case ActiveityOpenType.LessThanRegisterTime:
                    if ((ulong) openTypeParam[i] <
                        (TimeUtils.GetTimeStampMilliseconds() - storageCommon.InstalledAt) / 1000)
                    {
                        if (log) logStr += "不满足开启条件: 不满足最小注册时间";
                        result = false;
                    }

                    break;
                case ActiveityOpenType.LessThanLevel:
                    if (ExperenceModel.Instance.GetLevel() > openTypeParam[i])
                    {
                        if (log) logStr += "不满足开启条件: 小于某一关卡";
                        result = false;
                    }

                    break;
                case ActiveityOpenType.ConflictStarterPack:
                    //if (GameOfferStarterPackModel.Instance.IsOpenBuy())
                    //{
                    //    if (log) logStr += "不满足开启条件: 新手礼包开启状态";

                    //    result = false;
                    //}
                    break;
            }
        }

        if (!result && log) DebugUtil.Log(logStr);
        return result;
    }


    /// <summary>
    /// 基础筛选条件判断
    /// </summary>
    /// <param name="openType"></param>
    /// <param name="openTypeParam"></param>
    /// <param name="logStr"></param>
    /// <returns></returns>
    public bool CheckOpenType(string openType, string openTypeParam, string logStr = "")
    {
        if (string.IsNullOrEmpty(openType) || string.IsNullOrEmpty(openTypeParam))
            return true;

        return CheckOpenType(CommonUtils.StringToIntList(openType), CommonUtils.StringToIntList(openTypeParam), logStr);
    }

    #region Registered

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityEntityBase"></param>
    /// <returns></returns>
    public bool IsRegistered(ActivityEntityBase activityEntityBase)
    {
        if (activityEntityBase == null)
            return false;
        return _activityModules.ContainsKey(activityEntityBase.Guid);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityEntityBase"></param>
    public void Register(ActivityEntityBase activityEntityBase)
    {
        if (_activityModules.ContainsKey(activityEntityBase.Guid))
        {
            DebugUtil.LogError($"register {activityEntityBase.Guid} to config hub repeated.");
            return;
        }

        _activityModules[activityEntityBase.Guid] = activityEntityBase;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="activityEntityBase"></param>
    public void Unregister(ActivityEntityBase activityEntityBase)
    {
        if (!_activityModules.ContainsKey(activityEntityBase.Guid))
        {
            DebugUtil.LogError($"unregister {activityEntityBase.Guid} from config hub cannot be found.");
            return;
        }

        _activityModules.Remove(activityEntityBase.Guid);
    }

    #endregion
}