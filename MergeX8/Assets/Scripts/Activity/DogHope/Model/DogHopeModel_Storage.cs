using DragonU3DSDK.Storage;

public partial class DogHopeModel
{
    public static StorageDictionary<string, StorageDogHope> StorageDogHope =>
        StorageManager.Instance.GetStorage<StorageHome>().DogHopes;
    public StorageDogHope CurStorageDogHopeWeek
    {
        get
        {
            if (ActivityId == null)
                return null;
            StorageDogHope.TryGetValue(ActivityId, out StorageDogHope curWeek);
            return curWeek;
        }
    }
    public bool CreateStorage()
    {
        if (!SceneFsm.mInstance.ClientInited)
            return false;
        if (CurStorageDogHopeWeek == null && IsInitFromServer()
                                             && DogHopeModel.GetFirstWeekCanGetReward() == null
                                             && IsOpened())
        {
            var newWeek = new StorageDogHope()
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
            StorageDogHope.Add(ActivityId, newWeek);
            DogHopeLeaderBoardModel.Instance.CreateStorage(newWeek.LeaderBoardStorage);
            return true;
        }

        return false;
    }
    public static StorageDogHope GetFirstWeekCanGetReward()
    {
        foreach (var storageWeekPair in StorageDogHope)
        {
            var storageWeek = storageWeekPair.Value;
            if (storageWeek.IsTimeOut() && storageWeek.LeaderBoardStorage.IsInitFromServer() && !storageWeek.LeaderBoardStorage.IsFinish)
                return storageWeek;
        }
        return null;
    }
}