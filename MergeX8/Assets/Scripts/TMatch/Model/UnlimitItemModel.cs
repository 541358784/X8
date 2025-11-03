using DragonU3DSDK.Storage;
using Framework;

namespace TMatch
{


//限时Item
    public class UnlimitItemModel : GlobalSystem<UnlimitItemModel>
    {
        // 添加限时Item时间
        public void AddUnlimitedItemTime(ItemType unlimitedItemType, long timeMillSecond)
        {
            var unlimitItemEndUTCTimeInSecondsDict =
                StorageManager.Instance.GetStorage<StorageTMatch>().UnlimitItemEndUTCTimeInSecondsDict;
            var leftTime = UnlimitedItemLeftTime(unlimitedItemType);
            if (unlimitItemEndUTCTimeInSecondsDict.ContainsKey((int) unlimitedItemType))
            {
                if (leftTime > 0)
                {
                    unlimitItemEndUTCTimeInSecondsDict[(int) unlimitedItemType] += (long) (timeMillSecond * 0.001);
                }
                else
                {
                    unlimitItemEndUTCTimeInSecondsDict[(int) unlimitedItemType] =
                        (long) ((leftTime + timeMillSecond + CommonUtils.GetTimeStamp()) * 0.001);
                }
            }
            else
            {
                unlimitItemEndUTCTimeInSecondsDict.Add((int) unlimitedItemType,
                    (long) ((leftTime + timeMillSecond + CommonUtils.GetTimeStamp()) * 0.001));
            }
        }

        // 获取限时Item剩余时间(ms)
        public long UnlimitedItemLeftTime(ItemType unlimitedItemType)
        {
            var unlimitItemEndUTCTimeInSecondsDict =
                StorageManager.Instance.GetStorage<StorageTMatch>().UnlimitItemEndUTCTimeInSecondsDict;
            long left = 0;
            if (unlimitItemEndUTCTimeInSecondsDict.ContainsKey((int) unlimitedItemType))
            {
                left = unlimitItemEndUTCTimeInSecondsDict[(int) unlimitedItemType] * 1000 - CommonUtils.GetTimeStamp();
            }

            return left > 0L ? left : 0;
        }

        // 剩余时间格式化表示
        public string ParseRewardNumText(ItemType unlimitedItemType)
        {
            var leftTime = UnlimitedItemLeftTime(unlimitedItemType);
            return CommonUtils.GetUnlimiteLeftTimeString(leftTime);
        }

        // 是否处于限时中
        public bool IsUnlimitedItem(ItemType unlimitedItemType)
        {
            return UnlimitedItemLeftTime(unlimitedItemType) > 0;
        }
    }
}