using System.Collections.Generic;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public partial class MergeTaskTipsItem
{
    private GameObject debugInfoObj = null;
    private Text debugStarText;
    private Text debugRewardText;
    private Text debugIdText;

    private void AwakeDebug()
    {
        debugInfoObj = transform.Find("DebugInfo").gameObject;
        debugInfoObj.gameObject.SetActive(MainOrderManager.Instance.OpenDebugModule);

        debugStarText = debugInfoObj.transform.Find("star").GetComponent<Text>();
        debugRewardText = debugInfoObj.transform.Find("reward").GetComponent<Text>();
        debugIdText = debugInfoObj.transform.Find("id").GetComponent<Text>();
    }
    
    public void RefreshDebugModule()
    {
        if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
            return;
        UpdateDebugInfo();
        debugInfoObj.gameObject.SetActive(MainOrderManager.Instance.OpenDebugModule);
    }
    
    
    private void UpdateDebugInfo()
    {
        if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
            return;

        if (!MainOrderManager.Instance.OpenDebugModule)
            return;

        string type = "";
        if (storageTaskItem.Type == (int)MainOrderType.Fixed)
            type = "固";
        else if(storageTaskItem.Type == (int)MainOrderType.Random1)
            type = "随1";
        else if(storageTaskItem.Type ==  (int)MainOrderType.Random2)
            type = "随2";
        else if(storageTaskItem.Type ==  (int)MainOrderType.Random3)
            type = "随3";
        else if(storageTaskItem.Type == (int)MainOrderType.Random4)
            type = "随4";
        else if(storageTaskItem.Type == (int)MainOrderType.Random5New)
            type = "随9";
        else if(storageTaskItem.Type == (int)MainOrderType.Random6)
            type = "随6";
        else if(storageTaskItem.Type == (int)MainOrderType.ReturnUserFree)
            type = "免";
        else if(storageTaskItem.Type == (int)MainOrderType.Branch)
            type = "特";
        else if(storageTaskItem.Type == (int)MainOrderType.Append)
            type = "加";
        else if(storageTaskItem.Type == (int)MainOrderType.Time)
            type = "限";
        else if(storageTaskItem.Type == (int)MainOrderType.Limit)
            type = "链";
        else if(storageTaskItem.Type == (int)MainOrderType.Craze)
            type = "狂";
        else if(storageTaskItem.Type == (int)MainOrderType.SecondRecycle)
            type = "二回";
        if (storageTaskItem.IsHard || storageTaskItem.Type == (int)MainOrderType.Recomment)
            type += " 回";
        
        
        debugIdText.text = string.Format("ID:{0}", storageTaskItem.Id + " Type: " + type + "  Slot:" + storageTaskItem.Slot);
        debugInfoObj.gameObject.SetActive(MainOrderManager.Instance.OpenDebugModule);

        debugStarText.text = "";
        for (int i = 0; i < storageTaskItem.RewardTypes.Count; i++)
        {
            debugStarText.text += string.Format("奖励{0}: I:{1}  N:{2}", i, storageTaskItem.RewardTypes[i], storageTaskItem.RewardNums[i]);
            if (i < storageTaskItem.RewardTypes.Count)
                debugStarText.text += "\n";
        }
        
        debugRewardText.text = "";
        for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
        {
            int seatIndex = 1;
            if (storageTaskItem.ItemSeatIndex != null && storageTaskItem.ItemSeatIndex.Count == storageTaskItem.ItemIds.Count)
                seatIndex = storageTaskItem.ItemSeatIndex[i];
            
            debugRewardText.text += string.Format("物品{0}: R:{1}  S:{2}", i, storageTaskItem.ItemIds[i], seatIndex);
            if (i < storageTaskItem.ItemIds.Count)
                debugRewardText.text += "\n";
        }
    }

}


public static partial class XUtility
{
    public static TableMergeItem GetNextNeedItemConfig(this StorageTaskItem storageTaskItem)
    {
        Dictionary<int, int> mergeItemCounts = MergeManager.Instance.GetMergeItemCounts(MergeBoardEnum.Main);

        Dictionary<int, int> needItemCounts = new Dictionary<int, int>();
        for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
        {
            int id = storageTaskItem.ItemIds[i];
            if (!mergeItemCounts.ContainsKey(id))
                continue;

            if (!needItemCounts.ContainsKey(id))
                needItemCounts.Add(id, mergeItemCounts[id]);
        }
        for (int i = 0; i < storageTaskItem.ItemIds.Count; i++)
        {
            int id = storageTaskItem.ItemIds[i];
            int totalCount = storageTaskItem.ItemNums[i];
            if (needItemCounts.ContainsKey(id))
            {
                needItemCounts[id] -= totalCount;
                if (needItemCounts[id] < 0)
                {
                    return GameConfigManager.Instance.GetItemConfig(id);
                }   
            }
            else
            {
                return GameConfigManager.Instance.GetItemConfig(id);
            }
        }
        return null;
    }
}
