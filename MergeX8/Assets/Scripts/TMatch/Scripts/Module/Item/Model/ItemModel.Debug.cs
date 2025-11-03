using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;

namespace TMatch
{


    public partial class ItemModel
    {
        public static List<DebugCfg> GetDebugCfg()
        {
            List<DebugCfg> cfgs = new List<DebugCfg>();
#if DEBUG || DEVELOPMENT_BUILD
            cfgs.Add(new DebugCfg()
            {
                TitleStr = "增加物品",
                ClickCallBack = (arg1, arg2) =>
                {
                    try
                    {
                        int id = Int32.Parse(arg1);
                        int num = Int32.Parse(arg2);
                        Instance.Add(id, num,
                            new DragonPlus.GameBIManager.ItemChangeReasonArgs()
                                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug}, true);

                        // if (Instance.IsPortrait(id))
                        // {
                        //     PlayerInfo.UIAddHead.Open(id);
                        // }
                        // else if (Instance.IsAvatar(id))
                        // {
                        //     PlayerInfo.UIFashionReward.Open(id);
                        // }
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogError("Add item failed! " + e.Message);
                    }
                }
            });

            cfgs.Add(new DebugCfg()
            {
                TitleStr = "扣除物品",
                ClickCallBack = (arg1, arg2) =>
                {
                    try
                    {
                        int id = Int32.Parse(arg1);
                        int num = Int32.Parse(arg2);

                        // if (Instance.IsAvatar(id))
                        // {
                        //     if (PlayerInfo.Model.Instance.GetAvatar(Instance.GetAvatarType(id)) == id)
                        //     {
                        //         DebugUtil.LogError($"You can't delete avatar {id} when you wore it.");
                        //         return;
                        //     }
                        // }

                        Instance.Cost(id, num,
                            new DragonPlus.GameBIManager.ItemChangeReasonArgs()
                                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug}, true);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogError("Cost item failed! " + e.Message);
                    }
                }
            });

            cfgs.Add(new DebugCfg()
            {
                TitleStr = "增加独立物品",
                ClickCallBack = (arg1, arg2) =>
                {
                    try
                    {
                        var id = int.Parse(arg1);
                        var count = int.Parse(arg2);
                        Instance.AddUniqueItem(id, count, APIManager.Instance.GetServerTime() + 60000,
                            new DragonPlus.GameBIManager.ItemChangeReasonArgs
                                {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug}, true);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogError("Cost item failed! " + e.Message);
                    }
                }
            });

            cfgs.Add(new DebugCfg()
            {
                TitleStr = "删除独立物品",
                ClickCallBack = (arg1, arg2) =>
                {
                    try
                    {
                        var id = int.Parse(arg1);
                        Instance.RemoveUniqueItem(id, true);
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogError("Cost item failed! " + e.Message);
                    }
                }
            });
#endif
            return cfgs;
        }
    }
}
