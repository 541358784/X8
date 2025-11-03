using TotalRecharge_New;

public partial class MergeTaskTipsController
{
    public MergeGarageCleanUp _mergeGarageCleanUp;
    public MergeRewardItem _mergeRewardItem;
    public MergeRVItem _mergeRVItem;
    public MergeDailyTaskItem _mergeDailyTaskItem;
    public MergeFarmBtn _mergeFarmBtn;
    public MergeRecoverCoin _mergeRecoverCoin;
    public MergeCoinLeaderBoard _mergeCoinLeaderBoard;
    public MergeSummerWatermelon _mergeSummerWatermelon;
    public MergeButterflyWorkShop _MergeButterflyWorkShop;
    public MergeTreasureHunt _MergeTreasureHunt;
    public MergeHappyGo _mergeHappyGo;
    public MergeEaster _mergeEaster;
    public MergeMermaid _MergeMermaid;
    public MergeTotalRecharge _MergeTotalRecharge;
    public MergeTotalRecharge_New _MergeTotalRecharge_New;
    public TotalRechargeDecoReward _totalRechargeNewDecoReward;
    public MergeExtraOrderRewardCouponShowView MergeExtraOrderRewardCouponShowView;
    public MergeDogPlay MergeDogPlay;
    
    private void InitFixEntry()
    {
        _mergeGarageCleanUp = content.Find("GarageCleanupBtn").gameObject.AddComponent<MergeGarageCleanUp>();
        _mergeGarageCleanUp.gameObject.SetActive(true);
        _mergeGarageCleanUp.gameObject.SetActive(false);
        
        _mergeRewardItem = content.Find("RewardBtn").gameObject.AddComponent<MergeRewardItem>();
        _mergeRewardItem.gameObject.SetActive(true);
        
        _mergeFarmBtn = content.Find("FarmBtn").gameObject.AddComponent<MergeFarmBtn>();
        _mergeFarmBtn.gameObject.SetActive(true);
        _mergeFarmBtn.gameObject.SetActive(false);
        
        _mergeDailyTaskItem = content.Find("TaskButton").gameObject.AddComponent<MergeDailyTaskItem>();
        _mergeDailyTaskItem.gameObject.SetActive(true);
        _mergeDailyTaskItem.gameObject.SetActive(false);
        
        _mergeRVItem = content.Find("RVBtn").gameObject.AddComponent<MergeRVItem>();
        _mergeRVItem.gameObject.SetActive(true);
        _mergeRVItem.gameObject.SetActive(false);
        
        _mergeRecoverCoin = content.Find("RecoverCoin").gameObject.AddComponent<MergeRecoverCoin>();
        _mergeRecoverCoin.gameObject.SetActive(true);
        _mergeRecoverCoin.gameObject.SetActive(false);   
        
        _mergeCoinLeaderBoard = content.Find("CoinLeaderBoard").gameObject.AddComponent<MergeCoinLeaderBoard>();
        _mergeCoinLeaderBoard.gameObject.SetActive(true);
        _mergeCoinLeaderBoard.gameObject.SetActive(false);
        
        _mergeSummerWatermelon = content.Find("SummerWatermelon").gameObject.AddComponent<MergeSummerWatermelon>();
        _mergeSummerWatermelon.gameObject.SetActive(true);
        _mergeSummerWatermelon.gameObject.SetActive(false);
        
        _MergeButterflyWorkShop = content.Find("ButterflyWorkShop").gameObject.AddComponent<MergeButterflyWorkShop>();
        _MergeButterflyWorkShop.gameObject.SetActive(true);
        _MergeButterflyWorkShop.gameObject.SetActive(false);
        
        _MergeTreasureHunt = content.Find("TreasureHunt").gameObject.AddComponent<MergeTreasureHunt>();
        _MergeTreasureHunt.gameObject.SetActive(true);
        _MergeTreasureHunt.gameObject.SetActive(false);
        
        _mergeHappyGo = content.Find("HappyGoBtn").gameObject.AddComponent<MergeHappyGo>();
        _mergeHappyGo.gameObject.SetActive(true);
        _mergeHappyGo.gameObject.SetActive(false);

        _mergeEaster = Instantiate(easterItem, content).AddComponent<MergeEaster>();
        _mergeEaster.gameObject.SetActive(true); 
        _mergeEaster.gameObject.SetActive(false); 
        
        _MergeMermaid=content.Find("MermaidGameButton").gameObject.AddComponent<MergeMermaid>();
        _MergeMermaid.gameObject.SetActive(true);    
        _MergeMermaid.gameObject.SetActive(false);        
        
        _MergeTotalRecharge=content.Find("CumulativeRechargeBtn").gameObject.AddComponent<MergeTotalRecharge>();
        _MergeTotalRecharge.gameObject.SetActive(true);  
        _MergeTotalRecharge.gameObject.SetActive(false);    
        
        _MergeTotalRecharge_New=content.Find("CumulativeRechargeBtn_New").gameObject.AddComponent<MergeTotalRecharge_New>();
        _MergeTotalRecharge_New.gameObject.SetActive(true);
        _MergeTotalRecharge_New.gameObject.SetActive(false);
            
        _totalRechargeNewDecoReward=content.Find("CumulativeRechargeBtn_New_Reward").gameObject.AddComponent<TotalRechargeDecoReward>();
        _totalRechargeNewDecoReward.gameObject.SetActive(true);
        _totalRechargeNewDecoReward.gameObject.SetActive(false);
        
        MergeExtraOrderRewardCouponShowView = transform.Find("ExtraOrderRewardCoupon").gameObject.AddComponent<MergeExtraOrderRewardCouponShowView>();
        MergeExtraOrderRewardCouponShowView.gameObject.SetActive(true);
        
        MergeDogPlay = MergeDogPlay.CreateEntrance(transform.Find("DogPlay"));
    }
}