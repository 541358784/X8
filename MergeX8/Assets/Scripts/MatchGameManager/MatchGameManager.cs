using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using Framework;
using Merge.Order;
using UnityEngine;

namespace MatchGameManager
{
    public class MatchGameManager : Manager<MatchGameManager>
    {
        private StorageHome _storageHome
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>();
            }
        }
        private Dictionary<int, int> _recoveryRes = new Dictionary<int, int>();
        private Dictionary<int, int> _recoveryItem = new Dictionary<int, int>();

        private string matchTaskKey = "MatchTaskKey";
        private string matchBoardKey = "MatchBoardKey";

        public IEnumerator MatchGame()
        {
            MatchTask();
            yield return MatchBoard();
            
            MainOrderManager.Instance.TryFillOrder();
        }
        
        private void MatchTask()
        {
             // if(_storageHome.RcoveryRecord.ContainsKey(matchTaskKey))
             //     return;
            
            //_storageHome.RcoveryRecord.Add(matchTaskKey, true);
            
            for(int i = 0; i < MainOrderManager.Instance.CurTaskList.Count; i++)
            {
                var storageTaskItem = MainOrderManager.Instance.CurTaskList[i];
                for(int j = 0; j < storageTaskItem.ItemIds.Count; j++)
                {
                    int itemId = storageTaskItem.ItemIds[j];
                    var recoverConfig = GameConfigManager.Instance.GetRecovery(itemId);
                    if(recoverConfig == null)
                        continue;

                    if (recoverConfig.task_replace < 0)
                    {
                        storageTaskItem.ItemIds.RemoveAt(j);
                        j--;
                        continue;
                    }

                    storageTaskItem.ItemIds[j] = recoverConfig.task_replace;
                }

                if (storageTaskItem.ItemIds.Count == 0)
                {
                    MainOrderManager.Instance.CurTaskList.RemoveAt(i);
                    i--;
                }
                
                if(storageTaskItem.Id == 80000001 && storageTaskItem.Type == (int)MainOrderType.Time)
                {
                    MainOrderManager.Instance.CurTaskList.RemoveAt(i);
                    i--;
                }
            }
        }

        private IEnumerator MatchBoard()
        {
             // if(_storageHome.RcoveryRecord.ContainsKey(matchBoardKey))
             //     yield break;
            
            //_storageHome.RcoveryRecord.Add(matchBoardKey, true);
            
            GameConfigManager.Instance.RecoveryList.ForEach(a=>RecoveryMergeItem(a));
            
            if(_recoveryRes.Count == 0)
                yield break;

            UIManager.Instance.OpenUI(UINameConst.UIPopupRecovery, _recoveryItem, _recoveryRes);
            
            while (true)
            {
                var dlg = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupRecovery);
                if (dlg == null)
                {
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void RecoveryMergeItem(TableRecovery recovery)
        {
            if (recovery == null)
                return;

            if(recovery.recovery_item_task_type <= 0)
                return;

            if (MergeMainController.Instance == null || MergeMainController.Instance.MergeBoard == null)
                return;

            for (int i = 0; i < MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Items.Count; i++)
            {
                int itemId = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Items[i].Id;
                if (itemId <= 0)
                    continue;

                if (itemId != recovery.id)
                    continue;

                int index = i;
                MergeManager.Instance.RemoveBoardItem(index,MergeBoardEnum.Main,"Recovery");

                AddRecoverRes(recovery);
            }
            
            for (int i = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards.Count-1; i >=0; i--)
            {
                if (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards[i].Id <= 0)
                    continue;

                if (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards[i].Id != recovery.id)
                    continue;
                
                AddRecoverRes(recovery);
                MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards.Remove(MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards[i]);
            }    
            
            for (int i = MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Bags.Count-1; i >=0; i--)
            {
                if (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Bags[i].Id <= 0)
                    continue;
                
                if (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Bags[i].Id != recovery.id)
                    continue;
                
                AddRecoverRes(recovery);
                MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Bags.Remove(MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Bags[i]);
            }
        }

        private void AddRecoverRes(TableRecovery recovery)
        {
            if(!_recoveryRes.ContainsKey(recovery.recovery_item_task_type))
                _recoveryRes.Add(recovery.recovery_item_task_type, 0);

            _recoveryRes[recovery.recovery_item_task_type] += recovery.recovery_item_task_num;
            
            if(!_recoveryItem.ContainsKey(recovery.id))
                _recoveryItem.Add(recovery.id, 0);
            
            _recoveryItem[recovery.id] += 1;
        }
    }
}