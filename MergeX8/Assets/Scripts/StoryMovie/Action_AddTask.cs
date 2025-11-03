using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using Merge.Order;
using TMPro;
using UnityEngine;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace StoryMovie
{
    public class Action_AddTask: ActionBase
    {
        private int taskId = -1;
        protected override void Init()
        {
        }

        protected override void Start()
        {
            var config = OrderConfigManager.Instance.GetOrderCreateConfig(taskId);
            if (config == null)
                return;

            StorageTaskItem taskItem = MainOrderManager.Instance.AddTask(config, SlotDefinition.Special);
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventAppearTask,taskItem.OrgId + "_" + taskItem.Type, "seal");
        }

        protected override void Stop()
        {
        }
        
        protected override void ParsingParam()
        {
            int.TryParse(_config.actionParam, out taskId);
        }
    }
}