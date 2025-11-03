using System.Collections.Generic;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;

public class MergeTaskEntrance_CommonResourceLeaderBoard:MergeTaskEntranceBase
{
    private StorageCommonLeaderBoard Storage;

    public MergeTaskEntrance_CommonResourceLeaderBoard(StorageCommonLeaderBoard storage)
    {
        Storage = storage;
        CreatorDic[Storage] = this;
    }
    public static Dictionary<StorageCommonLeaderBoard, MergeTaskEntrance_CommonResourceLeaderBoard> CreatorDic =
        new Dictionary<StorageCommonLeaderBoard, MergeTaskEntrance_CommonResourceLeaderBoard>();
    public override bool CanCreateEntrance()
    {
        return Storage.IsResExist() && Storage.IsActive();
    }

    public MergeCommonResourceLeaderBoard TaskEntrance;
    public override MonoBehaviour CreateEntrance()
    {
        if (isRelease)
            return null;
        if (!CanCreateEntrance())
            return null;
        if (TaskEntrance != null)
            return TaskEntrance;
        var asset = ResourcesManager.Instance.LoadResource<GameObject>(
            Storage.TaskEntranceAssetPath);
        if (asset == null)
            return null;
        var obj = GameObject.Instantiate(asset);
        TaskEntrance = obj.AddComponent<MergeCommonResourceLeaderBoard>();
        TaskEntrance.SetStorage(Storage);
        return TaskEntrance;
    }
    private bool isRelease = false;
    public void Release()
    {
        isRelease = true;
        if (TaskEntrance)
        {
            GameObject.Destroy(TaskEntrance.gameObject);
        }
    }
}