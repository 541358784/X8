using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;


public partial class SROptions
{
    [Category(HappyGo)]
    [DisplayName("清理HappyGo")]
    public void ClearHappyGO()
    {
        MergeManager.Instance.GetStorageBoard(MergeBoardEnum.HappyGo).Clear();
        StorageManager.Instance.GetStorage<StorageGame>().HappyGo.Clear();
        
        AreaId = 888;
        ResetArea();
    }

     

     [Category(HappyGo)]
     [DisplayName("清理HappyGo引导")]
     public void ClearHappyGOGuide()
     {
         var guideIdList = new List<int>();
         for(int i = 1001; i <1900; i++)
             guideIdList.Add(i);
         
         CleanGuideList(guideIdList);
     }


     [Category(HappyGo)]
     [DisplayName("测试延期")]
     public void BuyExtend()
     {
         UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoExtend);
     }   
     [Category(HappyGo)]
     [DisplayName("测试结束")]
     public void TestEnd()
     {
         UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoEnd);
     }
     
     private int addMergeId1 = 0;
     [Sort(100)]
     [Category(HappyGo)]
     [DisplayName("mergeId")]
     public int AddMergeId1
     {
         get { return addMergeId1 ;}
         set { addMergeId1= value; }
     }
    
     [Sort(110)]
     [Category(HappyGo)]
     [DisplayName("增加merge元素")]
     public void AddMergeItem1()
     {
         if (GameConfigManager.Instance.GetItemConfig(addMergeId1) == null)
             return;

         var mergeItem = MergeManager.Instance.GetEmptyItem();
         mergeItem.Id = addMergeId1;
         mergeItem.State = 1;
         MergeManager.Instance.AddRewardItem(mergeItem,MergeBoardEnum.HappyGo, 1);
         EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.HAPPYGO_MERGE_REWARD_REFRESH);
        
     }
//     /*
//     [Category(HappyGo)]
//     [DisplayName("结束HappyGo")]
//     public void FinishHappyGO()
//     {
//         StorageManager.Instance.GetStorage<StorageGame>().HappyGo.StartTime =
//             StorageManager.Instance.GetStorage<StorageGame>().HappyGo.StartTime - 3 * 24 * 60 * 60;
//     }
//
     [Category(HappyGo)]
     [DisplayName("修改Happy经验")]
     public int HappyGoExpNum
     {
         get { return HappyGoModel.Instance.GetExp(); }
         set
         {
             
             HappyGoModel.Instance.storageHappy.Exp = value;
         }
     }
//
//     public bool IsOpenHappyGo = false;
//
//     [Category(HappyGo)]
//     [DisplayName("开启HappyGo")]
//     public bool DebugMHG
//     {
//         get
//         {
//             if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
//                 return IsOpenHappyGo;
//             return false;
//         }
//         set
//         {
//             if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
//                 IsOpenHappyGo = value;
//         }
//     }
//
//     [Category(HappyGo)]
//     [DisplayName("HappyGo Request 效验")]
//     public void HappyGoRequest()
//     {
//         for (int i = 0; i < 200; i++)
//         {
//             Debug.LogError("storageHappy.RequestId " +
//                            StorageManager.Instance.GetStorage<StorageGame>().HappyGo.RequestId + "\t" +
//                            StorageManager.Instance.GetStorage<StorageGame>().HappyGo.RequestIndex);
//             HappyGoModel.Instance.AddRequestIndex();
//         }
//     }
//
//     [Category(HappyGo)]
//     [DisplayName("显示 HappyGo")]
//     public void ShowHappyGo()
//     {
//         foreach (var kv in HappyGoMergeBoard.Instance.Grids)
//         {
//             if (kv.board == null)
//                 continue;
//
//             kv.board.Debug_ShowIcon();
//         }
//     }*/
}