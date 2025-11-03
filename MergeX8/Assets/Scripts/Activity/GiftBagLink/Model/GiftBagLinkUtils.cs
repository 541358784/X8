using System.Collections.Generic;
using DragonPlus.Config.AdConfigExtend;
using DragonPlus.Config.GiftBagLink;

public static class GiftBagLinkUtils
{
    public static GiftBagLinkList GetGiftBagList(int id)
    {
        List<GiftBagLinkList> configData = GiftBagLinkConfigManager.Instance.GetConfig<GiftBagLinkList>();
        if (configData == null || configData.Count == 0)
            return null;

        return configData.Find(a => a.UserGroup == id);
    }

    public static Dictionary<GiftBagLinkList, List<GiftBagLinkResource>> DataPool =
        new Dictionary<GiftBagLinkList, List<GiftBagLinkResource>>();
    public static List<GiftBagLinkResource> GetGiftBagLinkDataList(int id,int curIndex)
    {
        GiftBagLinkList giftBagLinkList = GetGiftBagList(id);
        if (giftBagLinkList == null)
            return null;

        List<GiftBagLinkResource> configData =
            GiftBagLinkConfigManager.Instance.GetConfig<GiftBagLinkResource>();
        if (configData == null || configData.Count == null)
            return null;

        if (!DataPool.TryGetValue(giftBagLinkList, out var linkDatas))
        {
            linkDatas = new List<GiftBagLinkResource>();   
            for (int i = 0; i < giftBagLinkList.ListData.Count; i++)
            {
                int dataId = giftBagLinkList.ListData[i];

                GiftBagLinkResource data = configData.Find(a => a.Id == dataId);
                if (data == null)
                    continue;

                linkDatas.Add(data);
            }
            DataPool.Add(giftBagLinkList,linkDatas);
        }

        if (giftBagLinkList.RepeatPosition >= 0 && giftBagLinkList.RepeatPosition < giftBagLinkList.ListData.Count)
        {
            while (linkDatas.Count - 1 <= curIndex + 7)
            {
                for (int i = giftBagLinkList.RepeatPosition; i < giftBagLinkList.ListData.Count; i++)
                {
                    int dataId = giftBagLinkList.ListData[i];

                    GiftBagLinkResource data = configData.Find(a => a.Id == dataId);
                    if (data == null)
                        continue;

                    linkDatas.Add(data);
                }
            }   
        }

        return linkDatas;
    }
}