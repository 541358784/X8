using DragonU3DSDK.Storage;

public partial class ClimbTreeModel
{
    public static StorageDictionary<string, StorageClimbTree> StorageClimbTree =>
        StorageManager.Instance.GetStorage<StorageHome>().ClimbTree;
    public StorageClimbTree CurStorageClimbTreeWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageClimbTree.TryGetValue(ActivityId, out StorageClimbTree curWeek);
            return curWeek;
        }
    }
    public bool CreateStorage()
    {
        if (!SceneFsm.mInstance.ClientInited)
            return false;
        if (CurStorageClimbTreeWeek == null && IsInitFromServer()
                                             && ClimbTreeModel.GetFirstWeekCanGetReward() == null
                                             && IsOpened())
        {
            var newWeek = new StorageClimbTree()
            {
                ActivityId = ActivityId,
                StartTime = (long) StartTime,
                EndTime = (long) EndTime,
                ActivityResList = { },
            };
            var resMd5List = ActivityManager.Instance.GetActivityMd5List(ActivityId);
            newWeek.ActivityResList.Clear();
            foreach (var resMd5 in resMd5List)
            {
                var resPath = ActivityResHotUpdate.GetFilePath(resMd5);
                newWeek.ActivityResList.Add(resPath);
            }
            StorageClimbTree.Add(ActivityId, newWeek);
            ClimbTreeLeaderBoardModel.Instance.CreateStorage(newWeek.LeaderBoardStorage);
            _curLevel = ClimbTreeModel.Instance.GetLevelByScore(CurScore);
            _totalLevel = ClimbTreeModel.Instance.GetLevelByScore(TotalScore);
            _lastTotalLevel = TotalLevel;
            _lastCurLevel = CurLevel;
            return true;
        }

        return false;
    }
    public static StorageClimbTree GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageClimbTree)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.IsTimeOut() && storageWeek.LeaderBoardStorage.IsInitFromServer() && !storageWeek.LeaderBoardStorage.IsFinish)
                return storageWeek;
        }
        return null;
    }
}