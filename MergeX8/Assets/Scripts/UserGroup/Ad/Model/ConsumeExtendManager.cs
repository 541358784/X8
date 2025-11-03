using System;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Storage;

public class ConsumeExtendManager : Manager<ConsumeExtendManager>
{
    private StorageConsumeExtend m_Storage => StorageManager.Instance.GetStorage<StorageHome>().ConsumeExtendData;

    bool IsFailedByCDOK(int id)
    {
        var c = AdConfigHandle.Instance.GetConsumeExtendID(id);
        //没有次数限制
        if (c == null)
        {
            return false;
        }

        if (!m_Storage.ConsumeRecords.ContainsKey(c.PlaceId))
            return false;

        if (Math.Abs(CommonUtils.GetCurTime() - m_Storage.ConsumeRecords[c.PlaceId].LastPlayTime) <
            (c.ShowInterval * 1000))
        {
            return true;
        }

        return false;
    }

    public bool IsFailedWithinLimit(int id)
    {
        var c = AdConfigHandle.Instance.GetConsumeExtendID(id);
        //没有次数限制
        if (c == null)
        {
            return false;
        }

        var pl = c.PlaceId;
        if (!m_Storage.ConsumeRecords.ContainsKey(pl))
        {
            StorageConsumeExtendRecord pr = new StorageConsumeExtendRecord();
            pr.LastPlayTime = 0;
            pr.PlayCount = 0;
            m_Storage.ConsumeRecords.Add(pl, pr);
        }

        System.DateTime l = CommonUtils.ConvertFromUnixTimestamp((ulong) m_Storage.ConsumeRecords[pl].LastPlayTime);
        if (c.LimitPerDay > 0 && (System.DateTime.Now.Year > l.Year || System.DateTime.Now.DayOfYear > l.DayOfYear))
        {
            m_Storage.ConsumeRecords[pl].PlayCount = 0;
            return false;
        }

        return m_Storage.ConsumeRecords[pl].PlayCount >= c.LimitPerDay;
    }

    public void ADConsumeRecord(int consumeExtendId)
    {
        var c = AdConfigHandle.Instance.GetConsumeExtendID(consumeExtendId);
        //没有次数限制
        if (c == null)
        {
            return;
        }

        ADConsumeRecord(c.PlaceId);
    }

    public void ADConsumeRecord(string pl)
    {
        StorageConsumeExtendRecord pr = null;
        if (!m_Storage.ConsumeRecords.TryGetValue(pl, out pr))
        {
            pr = new StorageConsumeExtendRecord();
            m_Storage.ConsumeRecords.Add(pl, pr);
        }

        pr.LastPlayTime = CommonUtils.GetCurTime();
        pr.PlayCount++;
    }

    public bool CanShowConsumeExtend(int consumeExtendID)
    {
        if (IsFailedByCDOK(consumeExtendID))
            return false;

        if (IsFailedWithinLimit(consumeExtendID))
            return false;


        return true;
    }
}