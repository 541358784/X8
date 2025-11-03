using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSeaRacingEndController:UIWindowController
{
    private Button CloseBtn;
    private Button CollectBtn;
    private Transform DefaultRewardNode;
    private Transform RewardGroup1;
    private Transform RewardGroup2;
    public override void PrivateAwake()
    {
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        CollectBtn = GetItem<Button>("Root/Button");
        CollectBtn.onClick.AddListener(OnClickCloseBtn);
        DefaultRewardNode = transform.Find("Root/RewardGroup/1-6/RewardNode");
        DefaultRewardNode.gameObject.SetActive(false);
        RewardGroup1 = transform.Find("Root/RewardGroup/1-6/");
        RewardGroup2 = transform.Find("Root/RewardGroup/7-12/");
    }
    
    public void OnClickCloseBtn()
    {
        CloseBtn.enabled = false;
        CollectBtn.enabled = false;
        Storage.CompletedStorageActivity();
        AnimCloseWindow();
    }

    private StorageSeaRacing Storage;
    public void BindStorage(StorageSeaRacing storage)
    {
        if (storage != null)
        {
            Storage = storage;
            InitRewardGroup();
        }
        else
        {
            Debug.LogError("预热弹窗绑定storage为null");
        }
    }

    public void InitRewardGroup()
    {
        var rewardCount = 0;
        for (var i = 1; i <= Storage.SeaRacingRoundList.Count; i++)
        {
            if (Storage.SeaRacingRoundList.TryGetValue(i, out var round))
            {
                if (round.IsFinish)
                {
                    rewardCount++;
                    var rank = round.SortController().MyRank;
                    var rewardNodeObj = Instantiate(DefaultRewardNode.gameObject, rewardCount>6?RewardGroup2:RewardGroup1);
                    rewardNodeObj.gameObject.SetActive(true);
                    for (var j = 1; j <= 3; j++)
                    {
                        rewardNodeObj.transform.Find(j.ToString()).gameObject.SetActive(j == rank);
                    }   
                }
            }
        }
        RewardGroup2.gameObject.SetActive(rewardCount > 6);
    }
    public static UIPopupSeaRacingEndController Open(StorageSeaRacing storage)
    {
        var popup = UIManager.Instance.OpenUI(UINameConst.UIPopupSeaRacingEnd) as UIPopupSeaRacingEndController;
        if (!popup)
            return null;
        popup.BindStorage(storage);
        return popup;
    }
}