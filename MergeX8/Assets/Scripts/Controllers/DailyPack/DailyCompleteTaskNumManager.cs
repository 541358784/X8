using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine;

public class DailyCompleteTaskNumManager : Singleton<DailyCompleteTaskNumManager>
{
    private StorageHome _home;
    private const int _saveNum = 10;
    private bool _isInit = false;

    private int _statisticsDayNum = 3;
    private int _defaultTaskNum = 25;
    private int _minTaskNum = 10;

    private StorageHome storageHome
    {
        get
        {
            if (_home == null)
                _home = StorageManager.Instance.GetStorage<StorageHome>();

            return _home;
        }
    }

    private void InitGlobalData()
    {
        if (_isInit)
            return;

        _isInit = true;
        _statisticsDayNum = GlobalConfigManager.Instance.GetNumValue("daily_package_task_num_days");
        _defaultTaskNum = GlobalConfigManager.Instance.GetNumValue("daily_package_task_num");
        _minTaskNum = GlobalConfigManager.Instance.GetNumValue("daily_package_task_num_min");
    }

    public int GetAverageTaskNum()
    {
        InitGlobalData();

        if (storageHome.DailyCompleteTaskNums.Count < _statisticsDayNum || _statisticsDayNum == 0)
            return _defaultTaskNum;

        int totalTaskNum = 0;
        for (int i = 0; i < _statisticsDayNum; i++)
        {
            int count = storageHome.DailyCompleteTaskNums.Count - 1 - i;
            totalTaskNum += storageHome.DailyCompleteTaskNums[count].CompleteTaskNum;
        }

        int taskNum = totalTaskNum / _statisticsDayNum;
        return taskNum < _minTaskNum ? _minTaskNum : taskNum;
    }

    public void SaveCompleteTaskNum()
    {
        int dayNum = GetDayNum();

        StorageDailyCompleteTaskNum data = null;
        if (storageHome.DailyCompleteTaskNums.Count == 0)
        {
            data = new StorageDailyCompleteTaskNum();
            data.DayNum = dayNum;
            data.CompleteTaskNum = 0;
            storageHome.DailyCompleteTaskNums.Add(data);
        }

        if (storageHome.DailyCompleteTaskNums.Count >= _saveNum)
            storageHome.DailyCompleteTaskNums.RemoveAt(0);

        data = storageHome.DailyCompleteTaskNums[storageHome.DailyCompleteTaskNums.Count - 1];
        if (data.DayNum == dayNum)
        {
            data.CompleteTaskNum++;
            return;
        }

        data = new StorageDailyCompleteTaskNum();
        data.DayNum = dayNum;
        data.CompleteTaskNum = 1;
        storageHome.DailyCompleteTaskNums.Add(data);
    }

    private int GetDayNum()
    {
        var sCommon = StorageManager.Instance.GetStorage<StorageCommon>();
        int dayNum = Utils.GetDayInterval((long) sCommon.InstalledAt / 1000, Utils.TotalSeconds()) + 1;

        return dayNum;
    }
}