using System.Collections.Generic;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.Config.GiftBagBuyBetter;

public static class GiftBagBuyBetterUtils
{
    public static GiftBagBuyBetterList GetGiftBagList(int id)
    {
        List<GiftBagBuyBetterList> configData = GiftBagBuyBetterConfigManager.Instance.GetConfig<GiftBagBuyBetterList>();
        if (configData == null || configData.Count == 0)
            return null;

        return configData.Find(a => a.UserGroup == id);
    }

    public static Dictionary<GiftBagBuyBetterList, List<GiftBagBuyBetterResource>> DataPool =
        new Dictionary<GiftBagBuyBetterList, List<GiftBagBuyBetterResource>>();
    public static List<GiftBagBuyBetterResource> GetGiftBagBuyBetterDataList(int id,int curIndex)
    {
        GiftBagBuyBetterList GiftBagBuyBetterList = GetGiftBagList(id);
        if (GiftBagBuyBetterList == null)
            return null;

        List<GiftBagBuyBetterResource> configData =
            GiftBagBuyBetterConfigManager.Instance.GetConfig<GiftBagBuyBetterResource>();
        if (configData == null || configData.Count == null)
            return null;

        if (!DataPool.TryGetValue(GiftBagBuyBetterList, out var linkDatas))
        {
            linkDatas = new List<GiftBagBuyBetterResource>();   
            for (int i = 0; i < GiftBagBuyBetterList.ListData.Count; i++)
            {
                int dataId = GiftBagBuyBetterList.ListData[i];

                GiftBagBuyBetterResource data = configData.Find(a => a.Id == dataId);
                if (data == null)
                    continue;

                linkDatas.Add(data);
            }
            DataPool.Add(GiftBagBuyBetterList,linkDatas);
        }

        if (GiftBagBuyBetterList.RepeatPosition >= 0 && GiftBagBuyBetterList.RepeatPosition < GiftBagBuyBetterList.ListData.Count)
        {
            while (linkDatas.Count - 1 <= curIndex + 7)
            {
                for (int i = GiftBagBuyBetterList.RepeatPosition; i < GiftBagBuyBetterList.ListData.Count; i++)
                {
                    int dataId = GiftBagBuyBetterList.ListData[i];

                    GiftBagBuyBetterResource data = configData.Find(a => a.Id == dataId);
                    if (data == null)
                        continue;

                    linkDatas.Add(data);
                }
            }   
        }

        return linkDatas;
    }
}