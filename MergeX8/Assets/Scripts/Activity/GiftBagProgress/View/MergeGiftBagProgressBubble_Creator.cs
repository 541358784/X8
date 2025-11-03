using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;

public class MergeGiftBagProgressBubble_Creator
{
    private StorageGiftBagProgress Storage;
    private Transform HangPoint => MergeMainController.Instance.BubbleHangPoint;

    public MergeGiftBagProgressBubble_Creator(StorageGiftBagProgress storage)
    {
        Storage = storage;
        CreatorDic[Storage] = this;
        TryCreate();
    }
    public static Dictionary<StorageGiftBagProgress, MergeGiftBagProgressBubble_Creator> CreatorDic =
        new Dictionary<StorageGiftBagProgress, MergeGiftBagProgressBubble_Creator>();
    public void TryCreate()
    {
        if (TaskEntrance == null && isRelease)
        {
            if (!CreateEntrance())
            {
                Task.Delay(1000).AddCallBack(TryCreate).WrapErrors();
            }
        }
    }
    public MergeGiftBagProgressBubble TaskEntrance;
    public MonoBehaviour CreateEntrance()
    {
        if (isRelease)
            return null;
        if (!Storage.ShowMergeBubble())
            return null;
        if (TaskEntrance != null)
            return TaskEntrance;
        if (HangPoint == null)
            return null;
        var asset = ResourcesManager.Instance.LoadResource<GameObject>(
            Storage.GetMergeBubbleAssetPath());
        if (asset == null)
            return null;
        var obj = GameObject.Instantiate(asset,HangPoint);
        TaskEntrance = obj.AddComponent<MergeGiftBagProgressBubble>();
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