using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class MergeRewardItem:MonoBehaviour
{
    private int curRewardCount = 0;
    public Button _mRewardBtn;
    public Image rewardIcon;
    public LocalizeTextMeshProUGUI rewardNumText;

    private void Start()
    {
        curRewardCount = MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main);
        RefreshRewards(false);
    }

    private void OnEnable()
    {
        curRewardCount = MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main);
        RefreshRewards(false);
    }
    void RefreshRewards(BaseEvent e)
    {
        if (e != null)
            curRewardCount = MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main);

        RefreshRewards(true);
        EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_BOARD_REFRESH);
    }
    void RefreshRewards(bool isCheckGuide)
    {
        bool isShow = curRewardCount > 0;

        bool openStore = UnlockManager.IsOpen(UnlockManager.MergeUnlockType.GameStore);
        bool openBag = UnlockManager.IsOpen(UnlockManager.MergeUnlockType.Bag);
       
        _mRewardBtn.gameObject.SetActive(isShow);

        rewardNumText.SetText(MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main).ToString());
        RefreshRewardIcon();

        if (isShow && isCheckGuide && gameObject.activeSelf)
        {
            MergeGuideLogic.Instance.CheckGetReward();
        }
    }
    private void RefreshRewardIcon()
    {
        if (MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main) > 0)
        {
            StorageMergeItem item = MergeManager.Instance.GetRewardItem(0,MergeBoardEnum.Main);
            var itemConfig = GameConfigManager.Instance.GetItemConfig(item.Id);
            if (itemConfig == null)
                return;
            rewardIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            rewardIcon.gameObject.SetActive(true);
        }
    }
    
    private void Awake()
    {
        curRewardCount = MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main);
        _mRewardBtn = gameObject.GetComponent<Button>();
        _mRewardBtn.onClick.AddListener(OnClickRewardBtn);
        XUtility.WaitFrames(1,()=>CommonUtils.SetShieldButTime(_mRewardBtn.gameObject));
        rewardIcon = transform.Find("Root/Icon").GetComponent<Image>();
        rewardNumText = transform.Find("Root/RedPoint/Label").GetComponent<LocalizeTextMeshProUGUI>();
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.GetReward, transform as RectTransform, topLayer:topLayer);
        
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_REWARD_REFRESH, RefreshRewards);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_REWARD_REFRESH, RefreshRewards);
    }

    private MergeBoard MergeBoard => MergeMainController.Instance.MergeBoard;
    
    public void OnClickRewardBtn()
    {
        if (curRewardCount <= 0)
            return;

        int emptyIndex = MergeManager.Instance.FindEmptyGrid(MergeBoard.activeIndex,MergeBoardEnum.Main);
        if (emptyIndex == -1)
        {
            MergePromptManager.Instance.ShowTextPrompt(_mRewardBtn.transform.position, 1.5f);
            return;
        }

        //引导需要特殊处理
        if (MergeManager.Instance.GetStorageBoard(MergeBoardEnum.Main).Rewards[0].Id == 1001)
        {
            if (!GuideSubSystem.Instance.isFinished(281))
            {
                foreach (var kv in MergeBoard.Grids)
                {
                    if(kv.id == 100004)
                       return;
                }
            }
        }
        // 3级前 加入1秒冷却时间
        // if (ExperenceModel.Instance.GetLevel() < 3)
        // {
        //     if (Time.time - clickRewardsTime < 1)
        //         return;
        //     clickRewardsTime = Time.time;
        // }

        curRewardCount--;
        StorageMergeItem storageMergeItem;
        MergeManager.Instance.RemoveRewardItem(0, out storageMergeItem, MergeBoardEnum.Main,false);
        if (MergeMainController.Instance.mergeClickTips!=null)
            MergeMainController.Instance.mergeClickTips.SetNoFocusStatus();

        rewardNumText.SetText(MergeManager.Instance.GetRewardCount(MergeBoardEnum.Main).ToString());
        MergeManager.Instance.SetNewBoardItem(emptyIndex, storageMergeItem.Id, 1, RefreshItemSource.rewards, MergeBoardEnum.Main,-1, false);
        RefreshRewards(true);
        AudioManager.Instance.PlaySound(13);
        TableMergeItem mergeItem = MergeResourceManager.Instance.ResourcesTableMerge;
        FlyGameObjectManager.Instance.FlyObject(storageMergeItem.Id, transform.position,
            MergeBoard.IndexToPosition(emptyIndex), 5f,
            () =>
            {
                ShakeManager.Instance.ShakeLight();
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardEnum.Main,emptyIndex, -1,
                    RefreshItemSource.rewards, storageMergeItem.Id);
                MergeGuideLogic.Instance.CheckMergeGuide();
                if (mergeItem != null)
                    MergeResourceManager.Instance.GetMergeResource(mergeItem,MergeBoardEnum.Main, true);
            });
        RefreshRewardIcon();
        SendUseAwardBagBi(storageMergeItem.Id);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GetReward, storageMergeItem.Id.ToString());
        MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Reward,MergeBoardEnum.Main);
    }
    private void SendUseAwardBagBi(int id)
    {
        var product = GameConfigManager.Instance.GetItemConfig(id);
        if (product == null)
            return;
        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
        {
            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeItemMoveStorageToGame,
            itemAId = product.id,
            ItemALevel = product.level,
            isChange = false,
            extras = new Dictionary<string, string>
            {
                {"from", "storage"},
                {"to", "game"},
            }
        });
        if (product.type == (int) MergeItemType.box)
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventChestPop);
        }
    }
}