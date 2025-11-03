using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

namespace Merge.UnlockMergeLine
{
    public class UnlockMergeLineManager : Singleton<UnlockMergeLineManager>
    {
        public enum UnlockType
        {
            unlockLine,
            finishOrder,
            level,
        }
        
        private StorageHome _storageHome
        {
            get
            {
                return StorageManager.Instance.GetStorage<StorageHome>();
            }
        }

        public bool UnLockMergeLine()
        {
            if (AdaptOldUser())
                return true;

            return UnLockLine(UnlockType.level);
        }
        private bool AdaptOldUser()
        {
            List<string> keys = new List<string>(_storageHome.RcoveryRecord.Keys);
            foreach (var key in keys)
            {
                if(!key.Contains("unlock"))
                    continue;

                string keyReplace = key.Replace("unlock", "");
                if(_storageHome.RcoveryRecord.ContainsKey($"success{keyReplace}"))
                    continue;
                
                _storageHome.RcoveryRecord.Add($"success{keyReplace}", true);
            }
            
            if (!_storageHome.RcoveryRecord.ContainsKey("1.0.36"))
            {
                _storageHome.RcoveryRecord.Add("1.0.36", true);
                
                return UnLockLine(UnlockType.unlockLine);
            }
            
            if (!_storageHome.RcoveryRecord.ContainsKey("1.0.51"))
            {
                _storageHome.RcoveryRecord.Add("1.0.51", true);
                
                return UnLockLine(UnlockType.unlockLine);
            }

            return false;
        }

        public bool FinishOrder(int orderId)
        {
            return UnLockLine(UnlockType.finishOrder, orderId);
        }
        
        private bool UnLockLine(UnlockType type, int param = -1)
        {
            foreach (var config in GameConfigManager.Instance.UnlockMergeLines)
            {
                if(_storageHome.RcoveryRecord.ContainsKey($"unlock{config.id}"))
                    continue;
                
                if(config.unlockType != (int)type)
                    continue;

                switch (type)
                {
                    case UnlockType.unlockLine:
                    {
                        bool isGet = false;
                        foreach (var id in config.unlockParam)
                        {
                            if(!MergeManager.Instance.IsGetItem(id))
                                continue;

                            isGet = true;
                            break;
                        }

                        if (!isGet)
                            continue;
                        
                        AddRewardItem(config.unlockMergeId);

                        _storageHome.RcoveryRecord.Add($"unlock{config.id}", true);
                        UIManager.Instance.OpenUI(UINameConst.UIPopupSyntheticChainUnlock, config, type);
                        
                        return true;
                    }
                    case UnlockType.finishOrder:
                    {
                        if(config.unlockParam[0] != param)
                            continue;
                        
                        _storageHome.RcoveryRecord.Add($"unlock{config.id}", true);
                        UIManager.Instance.OpenUI(UINameConst.UIPopupSyntheticChainUnlock, config, type);
                        
                        return true;
                    }
                    case UnlockType.level:
                    {
                        int level = ExperenceModel.Instance.GetLevel();
                        if(level < config.unlockParam[0])
                            continue;

                        AddRewardItem(config.unlockMergeId);
                        
                        _storageHome.RcoveryRecord.Add($"unlock{config.id}", true);
                        UIManager.Instance.OpenUI(UINameConst.UIPopupSyntheticChainUnlock, config, type);
                        
                        return true;
                    }
                }
            }

            return false;
        }

        private void AddRewardItem(int id)
        {
            var mergeItem = MergeManager.Instance.GetEmptyItem();
            mergeItem.Id = id;
            mergeItem.State = 1;
            MergeManager.Instance.AddRewardItem(mergeItem, MergeBoardEnum.Main,1, true);
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonAdaptOldUser,
                isChange = true,
            });
        }
    }
}