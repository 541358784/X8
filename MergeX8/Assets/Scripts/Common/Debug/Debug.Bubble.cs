using System.ComponentModel;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;
using Google.Protobuf.WellKnownTypes;
using UnityEngine;


public partial class SROptions
{
    private const string Bubble = "气泡";
    public string bubbleCreateInfo;
    public string bubbleFaileInfo;
    [Category(Bubble)]
    [DisplayName("气泡debug信息")]
    public string BubbleInfo
    {
        get
        {
            string info = "";
            info += "泡产生的随机概率: \t" + bubbleCreateInfo + '\n';
            info += "今日已经产生气泡: \t" + MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).TodayBubbleCount + "，Max:" + GlobalConfigManager.Instance.GetNumValue("TodayMaxBubbleCount") + '\n';
            info += "剩余CD时间: \t" + MergeManager.Instance.GetCreateBubbleCD() + '\n';
            info += "失败原因: \t" + bubbleFaileInfo + '\n';
            return info;
        }
    }
    public bool isOpenBubblRate = false;
    public bool isSetBubblRateOpen = false;
    [Category(Bubble)]
    [DisplayName("关闭气泡概率")]
    public bool DebugBubblRate
    {
        get
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
                return isOpenBubblRate;
            return false;
        }
        set
        {
            if (ConfigurationController.Instance.version == VersionStatus.DEBUG)
            {
                isSetBubblRateOpen = true;
                isOpenBubblRate = value;
            }
        }
    }
    
    [Category(Bubble)]
    [DisplayName("清除今日气泡总数")]
    public void DebugClearTotalBubbleNumber()
    {
        MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).TodayBubbleCount = 0;
    }
    
    [Category(Bubble)]
    [DisplayName("清除单气泡总数")]
    public void DebugClearBubbleNumber()
    {
        MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).TodayBubbleInfo.Clear();
    }
}