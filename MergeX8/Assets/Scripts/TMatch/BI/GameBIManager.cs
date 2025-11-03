using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Network.BI;
using Google.Protobuf;

namespace DragonPlus
{
    public partial class GameBIManager
    {
        public static void SendDecoGuideEvent(BiEventAdventureIslandMerge.Types.GameEventType type, string data1 = null, string data2 = null, string data3 = null)
        {
            var biEvent = new BiEventAdventureIslandMerge.Types.GameEvent
            {
                GameEventType = type,
            };
            if (!string.IsNullOrEmpty(data1)) biEvent.Data1 = data1;
            if (!string.IsNullOrEmpty(data1)) biEvent.Data2 = data2;
            if (!string.IsNullOrEmpty(data1)) biEvent.Data3 = data3;
            GameBIManager.Instance.SendBIEvent(biEvent);
        }
        public static void SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType type, object data1 = null,
            object data2 = null, object data3 = null, string status = null, ulong duration = 0,
            bool isAuto = false)
        {
            GameBIManager.Instance.SendGameEvent(type, data1?.ToString(), data2?.ToString(), data3?.ToString(), null, status,
                duration, isAuto);
        }

        public void SendEvent(IMessage message)
        {
            SendBIEvent(message);
        }
        
        // static public void SendItemChangeEvent(BiEventAdventureIslandMerge.Types.Item item, long amount, ulong current,
        //     ItemChangeReasonArgs args)
        // {
        //     var itemChangeEvent = new BiEventAdventureIslandMerge.Types.ItemChange
        //     {
        //         Item = ("ITEM_"+item).ToUpper(),
        //         Reason = args.reason,
        //         Amount = amount,
        //         Current = current,
        //         BoostId = args.boostId,
        //         CardId = args.cardId
        //     };
        //     if (!string.IsNullOrEmpty(args.data1))
        //     {
        //         itemChangeEvent.Data1 = args.data1;
        //     }
        //
        //     if (!string.IsNullOrEmpty(args.data2))
        //     {
        //         itemChangeEvent.Data2 = args.data2;
        //     }
        //
        //     if (!string.IsNullOrEmpty(args.data3))
        //     {
        //         itemChangeEvent.Data3 = args.data3;
        //     }
        //
        //     SendEvent(itemChangeEvent);
        // }
        public static void onThirdPartyTracking(string gameEventType)
        {
            DebugUtil.Log($"onThirdPartyTracking: {gameEventType}");
            BIManager.Instance.onThirdPartyTracking(gameEventType);
        }
    }
}