using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Activity;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.TMBP;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using IAPChecker;
using OutsideGuide;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TMatch
{
    public partial class TMBPModel : ActivityEntityBase
    {
        /// <summary>
        /// 活动类型
        /// </summary>
        public override string Guid => "OPS_EVENT_TYPE_TMBP";

        /// <summary>
        /// 活动是否可用
        /// </summary>
        public bool IsDataValid => IsOpened() || IsInReward();

        public override bool IsOpened(bool hasLog = false)
        {
            return IsUnlock && base.IsOpened(hasLog);
        }

        public override bool IsInReward(bool hasLog = false)
        {
            return IsUnlock && base.IsInReward(hasLog);
        }

        public bool IsUnlock => TMatchModel.Instance.GetMainLevel() >= 15;
        // public bool IsGetExpUnlock => TMatchModel.Instance.GetMainLevel() >= 16;

        /// <summary>
        /// 存档数据
        /// </summary>
        public StorageEventTMBP Data
        {
            get
            {
                var activity = StorageManager.Instance.GetStorage<StorageTMatch>();
                if (!activity.EventTMBP.ContainsKey(StorageKey))
                    activity.EventTMBP.Add(StorageKey, new StorageEventTMBP());
                return activity.EventTMBP[StorageKey];
            }
        }

        /// <summary>
        /// 所有的等级配置
        /// </summary>
        public static List<Base> LevelCfg => TMBPConfigManager.Instance.BaseList;

        /// <summary>
        /// 所有的奖励
        /// </summary>
        private static List<Rewards> RewardsCfg => TMBPConfigManager.Instance.RewardsList;

        /// <summary>
        /// 循环配置
        /// </summary>
        public Loop LoopCfg => IsDataValid ? TMBPConfigManager.Instance.LoopList[0] : null;

        /// <summary>
        /// 最大体力上限
        /// </summary>
        public int MaxEnergyNum => GetValue("MaxEnergy");

        /// <summary>
        /// 增加的等级
        /// </summary>
        public int AddLvNum => GetValue("BuyLevelAdd");

        /// <summary>
        /// 第一档商品id
        /// </summary>
        public int ShopId1 => GetValue("BuyShopIdLevel1");

        /// <summary>
        /// 第二档商品id
        /// </summary>
        public int ShopId2 => GetValue("BuyShopIdLevel2");

        /// <summary>
        /// 入口进度
        /// </summary>
        public float EntranceLastSliderValue;

        /// <summary>
        /// 入口是否可领取
        /// </summary>
        public bool EntranceLastCanCalim;

        /// <summary>
        /// 所有经验
        /// </summary>
        public int AllExp
        {
            get
            {
                int allExp = 0;
                if (LevelCfg != null)
                {
                    for (int i = 0; i < LevelCfg.Count; i++)
                    {
                        allExp += LevelCfg[i].exp;
                    }
                }

                return allExp;
            }
        }

        /// <summary>
        /// 游戏胜利获得的exp
        /// </summary>
        private int gameWinExp;

        /// <summary>
        /// 从服务器取到数据并初始化
        /// </summary>
        public override void InitFromServerData(string activityId, string activityType, ulong startTime, ulong endTime,
            ulong rewardEndTime,
            bool manualEnd, string configJson, string activitySubType)
        {
            base.InitFromServerData(activityId, activityType, startTime, endTime, rewardEndTime, manualEnd, configJson,
                activitySubType);

            TMBPConfigManager.Instance.InitConfig(configJson);

            //初始化完毕 初始化一次等级 安全起见
            if (Data != null)
            {
                Data.Level = GetCurLevelByExp();
                Data.StartActivityTime = (long)StartTime;
                Data.ActivityEndTime = (long)EndTime;
            }

            var curTime = APIManager.Instance.GetServerTime();
            var storage = StorageManager.Instance.GetStorage<StorageTMatch>().EventTMBP;
            var removeKeyList = new List<string>();
            foreach (var pair in storage)
            {
                if (pair.Value.ActivityEndTime < (long)curTime && pair.Value.UnCollectRewards.Count == 0)
                {
                    removeKeyList.Add(pair.Key);
                }
            }
            foreach (var key in removeKeyList)
            {
                storage.Remove(key);
            }
        }
        private static TMBPModel _instance;
        public static TMBPModel Instance => _instance ?? (_instance = new TMBPModel());
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitAuto()
        {
            Instance.Init();
            Instance.CustomInit();
        }

        public void CustomInit()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_WIN_BEFORE_ADD_MAIN_LEVEL, OnGameWin);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_START, OnGameStart);
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        /// <param name="evt"></param>
        private void OnGameStart(BaseEvent evt)
        {
            ClearValue();
        }

        /// <summary>
        /// 游戏胜利事件
        /// </summary>
        /// <param name="evt"></param>
        private void OnGameWin(BaseEvent evt)
        {
            if (!IsOpened())
                return;
            // if (!IsGetExpUnlock)
            //     return;
            int up = GetValue(IsLoopLevel() ? "LoopExpRangeUp" : "ExpRangeUp");
            int low = GetValue(IsLoopLevel() ? "LoopExpRangeLow" : "ExpRangeLow");

            EntranceLastSliderValue = GetProgress();
            EntranceLastCanCalim = HaveRewardClaim();
            gameWinExp = Random.Range(low, up + 1) * GetValue("ExpFactor");
            AddExp(gameWinExp);
        }

        #region get cfg

        public Base GetLevelConfig(int level)
        {
            return LevelCfg.Find(item => item.id == level);
        }

        public Rewards GetRewardsConfig(int id)
        {
            return RewardsCfg.Find(item => item.id == id);
        }

        public Rewards GetRewardsConfig(Base levelCfg, TM_BpType bpType)
        {
            int rewardId = bpType == TM_BpType.Normal ? levelCfg.freeReward : levelCfg.reward;
            return RewardsCfg.Find(item => item.id == rewardId);
        }

        #endregion

        #region get data

        /// <summary>
        /// 通过经验值获取等级
        /// </summary>
        /// <returns></returns>
        private int GetCurLevelByExp()
        {
            if (LevelCfg == null)
                return 0;
            //最大级
            if (Data.Exp >= AllExp)
            {
                return LevelCfg[LevelCfg.Count - 1].id;
            }

            int sumExp = 0;
            for (int i = 0; i < LevelCfg.Count - 1; i++)
            {
                sumExp += LevelCfg[i].exp;
                if (Data.Exp >= sumExp && Data.Exp < sumExp + LevelCfg[i + 1].exp)
                {
                    return LevelCfg[i].id;
                }
            }

            return 0;
        }

        /// <summary>
        /// 当前等级
        /// </summary>
        public int GetCurLevel()
        {
            if (!IsDataValid)
            {
                return 0;
            }

            return Data.Level;
        }

        /// <summary>
        /// 获取某一次需要的经验值
        /// </summary>
        /// <returns></returns>
        public int GetTargetLevelExp(int level)
        {
            int exp = 0;
            for (int i = 0; i < LevelCfg.Count; i++)
            {
                if (LevelCfg[i].id <= level)
                {
                    exp += LevelCfg[i].exp;
                }
            }

            return exp;
        }

        /// <summary>
        /// 是否是循环等级
        /// </summary>
        public bool IsLoopLevel()
        {
            return IsDataValid && Data.Exp >= AllExp;
        }

        /// <summary>
        /// 循环宝箱可以领取
        /// </summary>
        public bool IsLoopRewardsEnable()
        {
            return IsDataValid && HaveBuy() && IsLoopLevel() && Data.Exp >= AllExp + LoopCfg.exp;
        }

        #endregion

        /// <summary>
        /// 添加经验
        /// </summary>
        public void AddExp(int exp, bool dispatchEvent = true, bool isCliamLoopReward = false)
        {
            //循环奖励扣除奖励 不受活动是否开启影响
            if (!isCliamLoopReward && !IsOpened()) return;

            int levelOld = GetCurLevelByExp();
            int expOld = Data.Exp;
            Data.Exp += exp;
            if (Data.Exp < 0)
                Data.Exp = 0;
            if (dispatchEvent)
            {
                CommonEvent<Model, int, int, int>.DispatchEvent(EventEnum.TM_BattlePassOnExpChanged,
                    this, levelOld, expOld, exp);
            }

            int levelNow = GetCurLevelByExp();
            if (levelNow != levelOld)
            {
                Data.Level = levelNow;
                for (var i = levelOld+1; i <= levelNow; i++)
                {
                    Base cfg = GetLevelConfig(i);
                    Rewards rewardsFree = GetRewardsConfig(cfg, TM_BpType.Normal);
                    for (var j = 0; j < rewardsFree.itemIds.Length; j++)
                    {
                        var id = rewardsFree.itemIds[j];
                        var count = rewardsFree.itemCounts[j];
                        if (!Data.UnCollectRewards.ContainsKey(id))
                            Data.UnCollectRewards.Add(id,0);
                        Data.UnCollectRewards[id] += count;
                    }
                    if (Data.Status == (int)TM_BpType.Golden)
                    {
                        Rewards rewardsGolden = GetRewardsConfig(cfg, TM_BpType.Golden);
                        for (var j = 0; j < rewardsGolden.itemIds.Length; j++)
                        {
                            var id = rewardsGolden.itemIds[j];
                            var count = rewardsGolden.itemCounts[j];
                            if (!Data.UnCollectRewards.ContainsKey(id))
                                Data.UnCollectRewards.Add(id,0);
                            Data.UnCollectRewards[id] += count;
                        }
                    }
                }
            }

            EventDispatcher.Instance.DispatchEvent(new TM_BPRewardStatusChangeEvent());
        }

        /// <summary>
        /// 添加等级
        /// </summary>
        /// <param name="level"></param>
        public void AddLevel(int level)
        {
            if (!IsOpened(false)) return;

            int levelOld = GetCurLevel();
            int expOld = Data.Exp;
            int addExp = 0;
            for (var i = 0; i < level; i++)
            {
                if (!IsLoopLevel())
                {
                    int curLevel = GetCurLevel();
                    int tmpAddExp = GetLevelConfig(curLevel + 1).exp;
                    AddExp(tmpAddExp, false);
                    addExp += tmpAddExp;
                }
                else
                {
                    AddExp(LoopCfg.exp, false);
                    addExp += LoopCfg.exp;
                }
            }

            CommonEvent<Model, int, int, int>.DispatchEvent(EventEnum.TM_BattlePassOnExpChanged,
                this, levelOld, expOld, addExp);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <returns></returns>
        public int GetValue(string key)
        {
            if (!IsDataValid) return 0;
            Const cfg = TMBPConfigManager.Instance.ConstList.Find(item => item.key == key);
            return cfg?.value ?? 0;
        }

        #region claim

        /// <summary>
        /// 获取奖励状态
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="bpType">类型</param>
        /// <param name="viewedStatus">是否是查看状态</param>
        /// <param name="viewedLevel">是否是查看等级</param>
        /// <returns></returns>
        public TM_BPRewardStatus GetBpRewardStatus(int id, TM_BpType bpType = TM_BpType.Normal,
            bool viewedStatus = true, bool viewedLevel = true)
        {
            Base cfg = GetLevelConfig(id);
            switch (bpType)
            {
                case TM_BpType.Normal:
                    if (Data.ClaimedFree.Contains(id))
                    {
                        return TM_BPRewardStatus.Claimed;
                    }

                    int checkLevel = viewedLevel ? Data.LevelViewed : GetCurLevel();
                    return checkLevel >= cfg.id ? TM_BPRewardStatus.Available : TM_BPRewardStatus.Unlock;
                case TM_BpType.Golden:
                    if (Data.Claimed.Contains(id))
                    {
                        return TM_BPRewardStatus.Claimed;
                    }

                    TM_BpType type = viewedStatus ? (TM_BpType)Data.StatusView : (TM_BpType)Data.Status;
                    if (type == TM_BpType.Normal)
                        return TM_BPRewardStatus.Locked;

                    int checkLv = viewedLevel ? Data.LevelViewed : GetCurLevel();
                    return checkLv >= cfg.id ? TM_BPRewardStatus.Available : TM_BPRewardStatus.Unlock;
                default:
                    return TM_BPRewardStatus.Locked;
            }
        }

        /// <summary>
        /// 领取奖励
        /// </summary>
        /// <returns></returns>
        public void ClaimRewards(int level, TM_BpType bpType)
        {
            if (!IsDataValid) return;

            Base cfg = GetLevelConfig(level);
            Rewards rewards = GetRewardsConfig(cfg, bpType);
            for (var j = 0; j < rewards.itemIds.Length; j++)
            {
                var id = rewards.itemIds[j];
                var count = rewards.itemCounts[j];
                if (Data.UnCollectRewards.TryGetValue(id,out var pair))
                    if (pair <= 0)
                        Data.UnCollectRewards.Remove(id);
            }
            // BiUtil.ItemChangeReasonArgs changeReason = GetChangeReason(level);
            DragonPlus.GameBIManager.ItemChangeReasonArgs changeReason = GetChangeReason(level);

            switch ((TM_BpAwardType)rewards.awardType)
            {
                case TM_BpAwardType.Item:
                    ItemModel.Instance.Add(rewards.itemIds.ToList(), rewards.itemCounts.ToList(), changeReason, true);
                    break;
                case TM_BpAwardType.Chest:
                    UIItemBox.Open<UIItemBox>(new UIItemBox.Data()
                    {
                        boxType = bpType == TM_BpType.Normal?UIItemBox.BoxType.Blue1:UIItemBox.BoxType.Red1,
                        ItemIds = rewards.itemIds.ToList(),
                        ItemNums = rewards.itemCounts.ToList(),
                        HasReceive = true,
                    });
                    ItemModel.Instance.Add(rewards.itemIds.ToList(), rewards.itemCounts.ToList(), changeReason, true);
                    break;
            }

            if (bpType == TM_BpType.Normal)
                Data.ClaimedFree.Add(level);
            else
                Data.Claimed.Add(level);

            for (int i = 0; i < rewards.itemIds.Length; i++)
            {
                EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)rewards.itemIds[i]));
            }

            EventDispatcher.Instance.DispatchEvent(new TM_BPClaimRewardEvent(level, bpType));
            EventDispatcher.Instance.DispatchEvent(new TM_BPRewardStatusChangeEvent());
            SendClaimRewardsBI(rewards.itemIds.ToList(), rewards.itemCounts.ToList(), level);
            var data1 = bpType == TM_BpType.Golden ? "2" : "1";
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmBpReward,data1:data1,data2:cfg.id.ToString());
        }

        /// <summary>
        /// 领取循环奖励
        /// </summary>
        /// <returns></returns>
        public bool ClaimLoopRewards()
        {
            if (!IsDataValid || !IsLoopRewardsEnable())
                return false;

            Loop cfg = LoopCfg;
            int index = CommonUtils.RandomWeightIndex(cfg.rewardValue.ToList());
            Rewards rewards = GetRewardsConfig(cfg.reward[index]);
            if (rewards == null)
                return false;

            DragonPlus.GameBIManager.ItemChangeReasonArgs changeReason = GetChangeReason(31);

            UIItemBox.Open<UIItemBox>(new UIItemBox.Data()
            {
                boxType = UIItemBox.BoxType.Red1,
                ItemIds = rewards.itemIds.ToList(),
                ItemNums = rewards.itemCounts.ToList(),
                HasReceive = true,
            });

            ItemModel.Instance.Add(rewards.itemIds.ToList(), rewards.itemCounts.ToList(), changeReason, true);
            for (int i = 0; i < rewards.itemIds.Length; i++)
            {
                EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)rewards.itemIds[i]));
            }

            AddExp(-cfg.exp, true, true);

            SendClaimRewardsBI(rewards.itemIds.ToList(), rewards.itemCounts.ToList(), 31);
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmBpReward,data1:"0",data2:"100");
            EventDispatcher.Instance.DispatchEvent(new TM_BPClaimRewardEvent(true));

            return true;
        }

        /// <summary>
        /// 获取Bi change
        /// </summary>
        /// <returns></returns>
        private DragonPlus.GameBIManager.ItemChangeReasonArgs GetChangeReason(int lv)
        {
            DragonPlus.GameBIManager.ItemChangeReasonArgs changeReason =
                new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug);
            changeReason.data1 = GetCurLevel().ToString();
            changeReason.data2 = lv.ToString();
            changeReason.data3 = Data.Status.ToString();
            return changeReason;
        }

        /// <summary>
        /// 发送领奖BI
        /// </summary>
        private void SendClaimRewardsBI(List<int> itemIds, List<int> itemNums, int claimLv)
        {
            StringBuilder sbu = new StringBuilder();
            if (itemIds != null && itemNums != null && itemIds.Count == itemNums.Count)
            {
                for (int i = 0; i < itemIds.Count; i++)
                {
                    sbu.Append(itemIds[i].ToString());
                    sbu.Append("|");
                    sbu.Append(itemNums[i].ToString());
                    if (i != itemIds.Count - 1)
                    {
                        sbu.Append("|");
                    }
                }
            }

            // DragonPlus.GameBIManager.SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmClaimrewards,
            //     claimLv, Data.Status, sbu.ToString());
        }

        #endregion

        /// <summary>
        /// 购买
        /// </summary>
        public void Purchase(int shopId)
        {
            if (!IsOpened(false) || HaveBuy())
                return;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                FrameWorkUINotice.Open(new UINoticeData
                {
                    DescString =
                        LocalizationManager.Instance.GetLocalizedString("&key.UI_store_common_offlineerr_text"),
                    HasCloseButton = false
                });
                return;
            }
            // DragonPlus.GameBIManager.SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmGoldpassPurchase,
            //     GetCurLevel());
            StoreModel.Instance.Purchase(shopId);
        }

        /// <summary>
        /// 购买回调
        /// </summary>
        public void OnPurchase(TableShop shopConfigX8, bool isUnfulfilled)
        {
            if (!IsOpened(false))
            {
                if (isUnfulfilled) IAPChecker.UIMain.Open(new UIData { UIType = UIType.ContactUs });
                return;
            }

            Data.Status = (int)TM_BpType.Golden;

            if (isUnfulfilled)
            {
                IAPChecker.UIMain.Open(new UIData
                {
                    UIType = UIType.Vip,
                    UIVipType = UIVipType.TM_BpGoldenTicket
                });
            }

            var shopConfig = shopConfigX8.ChangeTableShopToTMatchShop();
            ShopLevel shopLevel = ShopLevel.Normal;
            if (shopConfig.id == ShopId2)
            {
                shopLevel = ShopLevel.AddLevel;
                AddLevel(AddLvNum);
            }
            else
            {
                EventDispatcher.Instance.DispatchEvent(new TM_BPRewardStatusChangeEvent());
            }

            // DragonPlus.GameBIManager.SendGameEventEx(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmGoldpassBuySuceed,
            //     GetCurLevel());
            EventDispatcher.Instance.DispatchEvent(new TM_BPBuyEvent(shopLevel));
            ItemModel.Instance.Add((int) ResourceId.TMEnergy, 8,new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
        }

        /// <summary>
        /// 获取购买战令 可能获得的奖励
        /// </summary>
        /// <returns></returns>
        public List<GameItemInfo> GetBuyCanGetItemInfo()
        {
            List<GameItemInfo> list = new List<GameItemInfo>();

            for (int i = 0; i < LevelCfg.Count; i++)
            {
                Rewards reward = GetRewardsConfig(LevelCfg[i].reward);
                for (int j = 0; j < reward.itemIds.Length; j++)
                {
                    AddToList(list, reward.itemIds[j], reward.itemCounts[j]);
                }
            }

            return list;
        }

        /// <summary>
        /// 添加到列表里面
        /// </summary>
        /// <param name="list"></param>
        /// <param name="itemId"></param>
        /// <param name="itemNum"></param>
        private void AddToList(List<GameItemInfo> list, int itemId, int itemNum)
        {
            bool find = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ItemId == itemId)
                {
                    list[i].ItemNum += itemNum;
                    find = true;
                    break;
                }
            }

            if (!find)
            {
                list.Add(new GameItemInfo(itemId, itemNum));
            }
        }

        /// <summary>
        /// 是否显示bp购买
        /// </summary>
        /// <returns></returns>
        public bool ShowBPBuy()
        {
            return IsOpened(false) && !HaveBuy();
        }

        /// <summary>
        /// 是否有奖励可领取
        /// </summary>
        /// <returns></returns>
        public bool HaveRewardClaim()
        {
            if (!IsDataValid) return false;
            for (int i = 0; i < LevelCfg.Count; i++)
            {
                if (GetBpRewardStatus(LevelCfg[i].id, TM_BpType.Normal, false, false) == TM_BPRewardStatus.Available
                    || GetBpRewardStatus(LevelCfg[i].id, TM_BpType.Golden) == TM_BPRewardStatus.Available)
                {
                    return true;
                }
            }

            if (IsLoopRewardsEnable())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 查找可领取的id
        /// </summary>
        /// <returns></returns>
        public int GetCanClaimId()
        {
            if (!IsDataValid) return -1;
            for (int i = 0; i < LevelCfg.Count; i++)
            {
                if (GetBpRewardStatus(LevelCfg[i].id, TM_BpType.Normal, false, false) == TM_BPRewardStatus.Available
                    || GetBpRewardStatus(LevelCfg[i].id, TM_BpType.Golden) == TM_BPRewardStatus.Available)
                {
                    return LevelCfg[i].id;
                }
            }

            return -1;
        }

        /// <summary>
        /// 获取Tm体力上限
        /// </summary>
        /// <returns></returns>
        public int GetTMEnergyMaxNum()
        {
            if (IsOpened(false) && Data != null && HaveBuy())
            {
                return MaxEnergyNum;
            }

            return ItemModel.Instance.GetConfigById((int)ResourceId.TMEnergy).max;
        }

        /// <summary>
        /// 获取上次游戏胜利获得的经验
        /// </summary>
        /// <returns></returns>
        public int GetLastGameWinExp()
        {
            int exp = gameWinExp;
            return exp;
        }

        /// <summary>
        /// 获取上次展示等级
        /// </summary>
        /// <returns></returns>
        public int GetViewdLevel()
        {
            return Data.LevelViewed;
        }

        /// <summary>
        /// 设置上次展示的等级
        /// </summary>
        public void SetLastShowLevel()
        {
            Data.LevelViewed = GetCurLevel();
        }

        /// <summary>
        /// 获取进度条
        /// </summary>
        /// <returns></returns>
        public float GetProgress()
        {
            bool haveReward = HaveRewardClaim();
            if (haveReward)
                return 1f;

            if (Data.Exp >= AllExp)
            {
                int haveGetExp = Data.Exp - AllExp;
                return (float)(haveGetExp) / LoopCfg.exp;
            }

            int curLevel = GetCurLevel();
            int curLvGetExp = Data.Exp - GetTargetLevelExp(curLevel);
            int nextLevelExp = GetLevelConfig(curLevel + 1).exp;
            return (float)curLvGetExp / nextLevelExp;
        }

        /// <summary>
        /// 状态是否变化
        /// </summary>
        /// <returns></returns>
        public bool StatusChanged()
        {
            return Data.Status != Data.StatusView;
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        public void StatusViewed()
        {
            Data.StatusView = Data.Status;
        }

        /// <summary>
        /// 是否已经购买
        /// </summary>
        /// <returns></returns>
        public bool HaveBuy()
        {
            if (!IsDataValid) return false;
            return (TM_BpType)Data.Status == TM_BpType.Golden;
        }

        /// <summary>
        /// 飞经验
        /// </summary>
        public void FlyExp(Vector3 startPos, GameObject entraceObj, Action callBack)
        {
            if (gameWinExp > 0)
            {
                GameObject prefab =
                    ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/TMatch/TMBP/TM_BPFlyExp");
                for (int i = 0; i < gameWinExp; i++)
                {
                    GameObject obj = GameObject.Instantiate(prefab);
                    obj.transform.parent = FlySystem.Instance.Root;
                    int index = i;
                    FlySystem.Instance.FlyObject(obj, startPos, entraceObj.transform.position, true, 0.8f, 0.15f * i,
                        () =>
                        {
                            FlySystem.Instance.PlayHintStarsEffect(entraceObj.transform.position);
                            if (index == gameWinExp - 1)
                            {
                                callBack?.Invoke();
                            }
                        });
                }
            }
        }

        /// <summary>
        /// 游戏结束 是否会弹出战令界面
        /// </summary>
        /// <returns></returns>
        public bool GameEndWellPopMainView()
        {
            if (IsOpened(false))
            {
                if (gameWinExp > 0 && GetCurLevel() > Data.LevelViewed)
                {
                    return true;
                }

                if (UIManager.Instance.GetOpenedWindow<TM_BPMainView>() != null ||
                    UIManager.Instance.GetOpenedWindow<TM_BPBuyView>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        public void ClearValue()
        {
            gameWinExp = 0;
            EntranceLastSliderValue = 0;
            EntranceLastCanCalim = false;
        }

        /// <summary>
        /// 参加活动
        /// </summary>
        public void JoinActivity()
        {
            if (IsOpened(false) && Data != null)
            {
                Data.IsJoinGame = true;
            }
        }

        /// <summary>
        /// 是否参加过活动
        /// </summary>
        /// <returns></returns>
        public bool IsJoinedActivity()
        {
            if (Data == null) return false;
            return Data.IsJoinGame;
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        public void OpenActivityMainUI()
        {
            TM_BPMainView.Open();
        }

        /// <summary>
        /// 下载完成
        /// </summary>
        public void OnDownloadFinished()
        {
            // 下载完成后主动加载一次图集
            ResourcesManager.Instance.LoadSpriteAtlas("Event/EventTMBP/SpriteAtlas/EventTMBP/EventTMBP");
        }
        

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_WIN_BEFORE_ADD_MAIN_LEVEL, OnGameWin);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_START, OnGameStart);
        }


        public static bool CanShowUnCollectRewardsUI()
        {
            var allUnCollectRewards = GetAllUnCollectRewards();
            if (allUnCollectRewards.Count > 0)
            {
                var itemIdList = new List<int>();
                var itemCountList = new List<int>();
                foreach (var reward in allUnCollectRewards)
                {
                    itemIdList.Add(reward.id);
                    itemCountList.Add(reward.count);
                }
                UIItemBox.Open<UIItemBox>(new UIItemBox.Data()
                {
                    boxType = UIItemBox.BoxType.Red1,
                    ItemIds = itemIdList,
                    ItemNums = itemCountList,
                    HasReceive = true,
                    OnDestroy = ()=>LobbyTaskSystem.Instance.FinishCurrentTask(),
                });
                var reasonArgs = new GameBIManager.ItemChangeReasonArgs()
                    {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug};
                ItemModel.Instance.Add(itemIdList, itemCountList, reasonArgs, true);
                CleanUnCollectRewards();
                for (int i = 0; i < itemIdList.Count; i++)
                {
                    EventDispatcher.Instance.DispatchEvent(new ResChangeEvent((ResourceId)itemIdList[i]));
                }
                return true;
            }
            return false;
        }
        public static List<ResData> GetAllUnCollectRewards()
        {
            var unCollectRewardsList = new List<ResData>();
            List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageTMatch>().EventTMBP.Keys);
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var storage = StorageManager.Instance.GetStorage<StorageTMatch>().EventTMBP[keys[i]];
            
                if (IsActivityStorageEnd(storage))
                {
                    foreach (var pair in storage.UnCollectRewards)
                    {
                        if (pair.Value > 0)
                        {
                            unCollectRewardsList.Add(new ResData(pair.Key,pair.Value));
                        }
                    }
                }
            }
            return unCollectRewardsList;
        }
        static bool IsActivityStorageEnd(StorageEventTMBP storage)
        {
            return (long) APIManager.Instance.GetServerTime() >= storage.ActivityEndTime ||
                   (long) APIManager.Instance.GetServerTime() < storage.StartActivityTime;
        }
        public static void CleanUnCollectRewards()
        {
            List<string> keys = new List<string>(StorageManager.Instance.GetStorage<StorageTMatch>().EventTMBP.Keys);
            for (int i = keys.Count - 1; i >= 0; i--)
            {
                var storage = StorageManager.Instance.GetStorage<StorageTMatch>().EventTMBP[keys[i]];
                if (IsActivityStorageEnd(storage))
                {
                    CompletedStorageActivity(storage);
                }
            }
        }
        static void CompletedStorageActivity(StorageEventTMBP storage)
        {
            storage.UnCollectRewards.Clear();
        }
        
        
        public override bool CanDownLoadRes()
        {
            return TMatchModel.Instance.GetMainLevel() >= 5;
        }
    }
}