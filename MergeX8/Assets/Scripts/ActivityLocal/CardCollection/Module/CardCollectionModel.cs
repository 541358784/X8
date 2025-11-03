using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityLocal.CardCollection.Home;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Scripts.UI;
using TMatch;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class CardCollectionModel : Manager<CardCollectionModel>
{
    // public bool ShowEntrance()
    // {
    //     return ThemeInUse != null;
    // }
    public bool IsUnlock => UnlockManager.IsOpen(UnlockManager.MergeUnlockType.CardCollection);

    public bool ShowAuxItem()
    {
        return IsUnlock && ThemeInUse != null;
    }

    public bool ShowTaskEntrance()
    {
        return ThemeInUse != null && IsResReady(ThemeInUse);
    }

    public string GetAuxItemAssetPath()
    {
        return "Prefabs/CardMain/Aux_CardCollection";
    }

    public string GetCardPackageTaskItemAssetPath()
    {
        return "Prefabs/CardMain/TaskList_CardPackage";
    }

    public StorageCardCollection StorageCardCollection =>
        StorageManager.Instance.GetStorage<StorageHome>().CardCollection;

    public Dictionary<int, TableCardCollectionTheme> TableCardTheme => CardCollectConfigManager.Instance.TableCardTheme;

    public Dictionary<int, TableCardCollectionCardBook> TableCardBook =>
        CardCollectConfigManager.Instance.TableCardBook;

    public Dictionary<int, TableCardCollectionCardItem> TableCardItem =>
        CardCollectConfigManager.Instance.TableCardItem;

    public Dictionary<int, TableCardCollectionCardPackage> TableCardPackage =>
        CardCollectConfigManager.Instance.TableCardPackage;

    public Dictionary<int, TableCardCollectionCardPackageItem> TableCardPackageItem =>
        CardCollectConfigManager.Instance.TableCardPackageItem;

    public Dictionary<int, TableCardCollectionWildCard> TableCardWildCard =>
        CardCollectConfigManager.Instance.TableCardWildCard;

    public Dictionary<int, TableCardCollectionRandomGroup> TableCardRandomGroup =>
        CardCollectConfigManager.Instance.TableCardRandomGroup;

    public CardCollectionCardThemeState ThemeInUse
    {
        get
        {
            // if (!StorageCardCollection.IsInit)
            //     return null;
            return CardCollectionActivityModel.Instance.GetThemeOnOpened();
        }
    }

    public CardCollectionCardThemeState ThemeReopenInUse
    {
        get
        {
            // if (!StorageCardCollection.IsInit)
            //     return null;
            return CardCollectionReopenActivityModel.Instance.GetThemeOnOpened();
        }
    }

    private bool InitFlag = false;

    public void InitConfig()
    {
        if (InitFlag)
        {
            ForceInitConfig();
            return;
        }

        InitFlag = true;
        foreach (var cardThemeConfig in TableCardTheme)
        {
            GetCardThemeState(cardThemeConfig.Key).AddUIWindowInfo();
            if (cardThemeConfig.Value.NewRandomLogic)
            {
                GetCardThemeState(cardThemeConfig.Key).InitNewRandomLogicStorage();
            }
        }

        InitUnViewedCardItemStateDictionary();
        CheckUnlockTheme();
    }

    public void ForceInitConfig()
    {
        CanCollectRewardCardBookStateList.Clear();
        CanCollectRewardCardThemeStateList.Clear();
        foreach (var cardThemeConfig in TableCardTheme)
        {
            GetCardThemeState(cardThemeConfig.Key).SetCardThemeId(cardThemeConfig.Key, true);
        }

        InitUnViewedCardItemStateDictionary();
    }

    public CardCollectionCardItemState GetCardItemState(int cardId)
    {
        if (!CardCollectionCardItemState.CardItemStateDictionary.ContainsKey(cardId))
        {
            var cardItem = new CardCollectionCardItemState(cardId);
        }

        return CardCollectionCardItemState.CardItemStateDictionary[cardId];
    }

    public CardCollectionCardBookState GetCardBookState(int cardBookId)
    {
        if (!CardCollectionCardBookState.CardBookStateDictionary.ContainsKey(cardBookId))
        {
            var cardBook = new CardCollectionCardBookState(cardBookId);
        }

        return CardCollectionCardBookState.CardBookStateDictionary[cardBookId];
    }

    public CardCollectionCardThemeState GetCardThemeState(int cardThemeId)
    {
        if (!CardCollectionCardThemeState.CardThemeStateDictionary.ContainsKey(cardThemeId))
        {
            var cardTheme = new CardCollectionCardThemeState(cardThemeId);
        }

        return CardCollectionCardThemeState.CardThemeStateDictionary[cardThemeId];
    }

    public void OpenCardPackage(StorageCardCollectionCardPackage package, string source, Action callback)
    {
        var packageId = package.Id;
        var packageConfig = TableCardPackage[packageId];
        var themeId = packageConfig.ThemeId;
        if (packageId == 999999)
            themeId = ThemeInUse != null ? ThemeInUse.GetUpGradeTheme().CardThemeConfig.Id : 1;
        List<CardCollectionCardItemState> collectCards = new List<CardCollectionCardItemState>();
        for (var i = 0; i < packageConfig.ItemList.Count; i++)
        {
            var packageItemId = packageConfig.ItemList[i];
            var packageItemConfig = TableCardPackageItem[packageItemId];
            var cardLevel = GetRandomCardLevel(packageItemConfig);
            var sameWeight = false;
            if (packageConfig.Level == 4)
            {
                sameWeight = GetCardThemeState(themeId).CardThemeConfig.Level4SameWeight >= cardLevel;
            }
            else if (packageConfig.Level == 5)
            {
                sameWeight = GetCardThemeState(themeId).CardThemeConfig.Level5SameWeight >= cardLevel;
            }

            var cardItemId = GetRandomCardWithLevel(cardLevel, themeId, (CardPackageWeightType)packageConfig.WeightType,
                package, sameWeight, collectCards);
            if (cardItemId != 0)
            {
                // collectCards.Add(GetCardItemState(cardItemId));
                collectCards.Insert(Random.Range(0, collectCards.Count), GetCardItemState(cardItemId)); //随机排序
            }
        }

        List<bool> newCardFlag = new List<bool>();
        for (var i = 0; i < collectCards.Count; i++)
        {
            newCardFlag.Add(collectCards[i]
                .CollectOneCard(GetCardSource.CardPackage, "CardPackage_" + packageId + "_" + source));
        }
        
        //开卡包弹窗
        UIOpenCardController.Open(packageConfig, collectCards, newCardFlag, callback);

        // if (packageConfig.TeamShare)
        {
            var cardIdList = new List<int>();
            foreach (var card in collectCards)
            {
                cardIdList.Add(card.CardItemConfig.Id);
            }
            TeamManager.Instance.CreateBattlePassGift("OpenCardPackage",
                new CardPackageExtra(cardIdList, packageConfig.Id));
        }
    }

    public int GetRandomCardLevel(TableCardCollectionCardPackageItem packageItemConfig)
    {
        var maxWeight = 0;
        for (var i = 0; i < packageItemConfig.Weight.Count; i++)
        {
            maxWeight += packageItemConfig.Weight[i];
        }

        var randomValue = Random.Range(0, maxWeight);
        var curValue = 0;
        for (var i = 0; i < packageItemConfig.Weight.Count; i++)
        {
            curValue += packageItemConfig.Weight[i];
            if (curValue > randomValue)
            {
                return i + 1;
            }
        }

        Debug.LogError("未随出卡牌等级,weight=" + packageItemConfig.Weight);
        return 1;
    }

    private int lastRandomPoolRefreshLevel = 0;

    public Dictionary<int, Dictionary<int, List<int>>> CardItemRandomPool =
        new Dictionary<int, Dictionary<int, List<int>>>();

    public void TryUpdateRandomPool(int themeId)
    {
        if (CardItemRandomPool.ContainsKey(themeId))
            return;
        var newPool = new Dictionary<int, List<int>>();
        foreach (var pair in GetCardThemeState(themeId).CardBookStateList)
        {
            var bookState = pair.Value;
            foreach (var itemPair in bookState.CardItemStateList)
            {
                var level = itemPair.Value.CardItemConfig.Level;
                if (!newPool.ContainsKey(level))
                {
                    newPool.Add(level, new List<int>());
                }

                newPool[level].Add(itemPair.Value.CardItemConfig.Id);
            }
        }

        CardItemRandomPool.Add(themeId, newPool);
    }

    public int GetRandomCardWithLevelNewRandomLogic(int level, int themeId,
        List<CardCollectionCardItemState> ignoreList)
    {
        var levelGroup = StorageCardCollection.ThemeRandomGroupDic[themeId].LevelRandomGroupDic[level];
        var weightList = new List<float>();
        var cardList = new List<int>();
        for (var i = 0; i < levelGroup.RandomGroups.Count; i++)
        {
            var randomGroup = levelGroup.RandomGroups[i];
            for (var j = 0; j < weightList.Count; j++)
            {
                weightList[j] *= randomGroup.LastLevelWeightScale;
            }

            foreach (var pair in randomGroup.Pool)
            {
                if (ignoreList.Find(a => a.CardItemConfig.Id == pair.Key) != null)
                    continue;
                weightList.Add(pair.Value);
                cardList.Add(pair.Key);
            }

            var leftCount = randomGroup.GetPoolLeftCardCount();
            if (leftCount > randomGroup.LevelUpLeftCount)
            {
                break;
            }
        }

        if (weightList.Count > 0)
        {
            var index = weightList.RandomIndexByWeightF();
            var cardId = cardList[index];
            for (var i = 0; i < levelGroup.RandomGroups.Count; i++)
            {
                var randomGroup = levelGroup.RandomGroups[i];
                if (randomGroup.Pool.ContainsKey(cardId))
                {
                    randomGroup.Pool[cardId]--;
                    if (randomGroup.Pool[cardId] == 0)
                    {
                        var keys = randomGroup.Pool.Keys.ToList();
                        foreach (var key in keys)
                        {
                            if (key == cardId)
                                continue;
                            if (randomGroup.Pool[key] > 1)
                            {
                                randomGroup.Pool[key]--;
                                randomGroup.Pool[cardId]++;
                                break;
                            }
                        }

                        if (randomGroup.Pool[cardId] == 0)
                        {
                            randomGroup.Pool.Remove(cardId);
                        }
                    }

                    break;
                }
            }

            Debug.LogError("新抽卡随机逻辑,cardId=" + cardId);
            return cardId;
        }
        else //全清
        {
            TryUpdateRandomPool(themeId);
            CardItemRandomPool[themeId].TryGetValue(level, out var randomCardItemIdList);
            if (randomCardItemIdList == null)
                return 0;
            var intWeightList = new List<int>();
            for (var i = 0; i < randomCardItemIdList.Count; i++)
            {
                if (ignoreList.Find(a => a.CardItemConfig.Id == randomCardItemIdList[i]) != null)
                {
                    intWeightList.Add(0);
                    continue;
                }

                intWeightList.Add(1);
            }

            var randomIndex = Utils.RandomByWeight(intWeightList);
            return randomCardItemIdList[randomIndex];
        }
    }

    public int GetRandomCardWithLevel(int level, int themeId, CardPackageWeightType weightType,
        StorageCardCollectionCardPackage package, bool sameWeight, List<CardCollectionCardItemState> ignoreList)
    {
        if (StorageCardCollection.ThemeRandomGroupDic.ContainsKey(themeId))
        {
            return GetRandomCardWithLevelNewRandomLogic(level, themeId, ignoreList);
        }

        TryUpdateRandomPool(themeId);
        CardItemRandomPool[themeId].TryGetValue(level, out var randomCardItemIdList);
        if (randomCardItemIdList == null)
            return 0;
        var weightList = new List<int>();
        if (sameWeight)
        {
            for (var i = 0; i < randomCardItemIdList.Count; i++)
            {
                if (ignoreList.Find(a => a.CardItemConfig.Id == randomCardItemIdList[i]) != null)
                {
                    weightList.Add(0);
                    continue;
                }

                weightList.Add(1);
            }
        }
        else
        {
            for (var i = 0; i < randomCardItemIdList.Count; i++)
            {
                if (ignoreList.Find(a => a.CardItemConfig.Id == randomCardItemIdList[i]) != null)
                {
                    weightList.Add(0);
                    continue;
                }

                var cardItemConfig = TableCardItem[randomCardItemIdList[i]];
                var addWeight = weightType == CardPackageWeightType.Free
                    ? (cardItemConfig.GetFreeWeight(package))
                    : cardItemConfig.GetPayWeight(package);
                weightList.Add(addWeight);
            }
        }

        var randomIndex = Utils.RandomByWeight(weightList);
        return randomCardItemIdList[randomIndex];
    }

    public List<CardCollectionCardBookState>
        CanCollectRewardCardBookStateList = new List<CardCollectionCardBookState>(); //待领奖的卡册列表

    public void PushCanCollectRewardCardBookStateList(CardCollectionCardBookState cardBookState)
    {
        if (!cardBookState.CanCollect || CanCollectRewardCardBookStateList.Contains(cardBookState))
            return;
        CanCollectRewardCardBookStateList.Add(cardBookState);
    }

    public List<CardCollectionCardThemeState> CanCollectRewardCardThemeStateList =
        new List<CardCollectionCardThemeState>(); //待领奖的卡册列表

    public void PushCanCollectRewardCardThemeStateList(CardCollectionCardThemeState cardThemeState)
    {
        if (!cardThemeState.CanCollect || CanCollectRewardCardThemeStateList.Contains(cardThemeState))
            return;
        CanCollectRewardCardThemeStateList.Add(cardThemeState);
    }

    public void CollectStar(int starCount)
    {
        StorageCardCollection.StarCount += starCount;
        EventDispatcher.Instance.SendEvent(new EventCardStarValueChange(starCount));
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryStars,
            starCount.ToString(), StorageCardCollection.StarCount.ToString());
    }

    private bool IsAutoCollecting = false;

    public bool DoAllUndoAction(Action callback = null)
    {
        if (IsAutoCollecting)
            return false;
        IsAutoCollecting = true;
        var result = CheckUndoCardCollectionAction(() =>
        {
            IsAutoCollecting = false;
            callback?.Invoke();
        });
        return result;
    }

    public bool DoAllUndoActionInAutoPopup()
    {
        return DoAllUndoAction();
    }

    public bool CheckUndoCardCollectionAction(Action callback = null)
    {
        if (CanCollectRewardCardThemeStateList.Count > 0)
        {
            GetAllCardThemeCompleteReward().AddCallBack(() => CheckUndoCardCollectionAction(callback)).WrapErrors();
            return true;
        }

        if (CanCollectRewardCardBookStateList.Count > 0)
        {
            GetAllCardBookCompleteReward().AddCallBack(() => CheckUndoCardCollectionAction(callback)).WrapErrors();
            return true;
        }

        // if (StorageCardCollection.UnOpenPackageList.Count > 0)
        // {
        //     TryOpenSingleCardPackage(StorageCardCollection.UnOpenPackageList[0].Id).AddCallBack(()=>CheckUndoCardCollectionAction(callback)).WrapErrors();
        //     return true;
        // }
        callback?.Invoke();
        return false;
    }

    public async Task TryGetCardBookCompleteReward()
    {
        if (CanCollectRewardCardBookStateList.Count > 0)
        {
            var collectCardBookState = CanCollectRewardCardBookStateList[0];
            CanCollectRewardCardBookStateList.RemoveAt(0);
            var task = new TaskCompletionSource<bool>();
            collectCardBookState.CollectCompleteReward(() => task.SetResult(true));
            await task.Task;
        }
    }

    public async Task TryGetCardThemeCompleteReward()
    {
        if (CanCollectRewardCardThemeStateList.Count > 0)
        {
            var collectCardThemeState = CanCollectRewardCardThemeStateList[0];
            CanCollectRewardCardThemeStateList.RemoveAt(0);
            var task = new TaskCompletionSource<bool>();
            collectCardThemeState.CollectCompleteReward(() => task.SetResult(true));
            await task.Task;
        }
    }

    public async Task GetAllCardBookCompleteReward()
    {
        var count = CanCollectRewardCardBookStateList.Count;
        for (var i = 0; i < count; i++)
        {
            await TryGetCardBookCompleteReward();
        }
    }

    public async Task GetAllCardThemeCompleteReward()
    {
        var count = CanCollectRewardCardThemeStateList.Count;
        for (var i = 0; i < count; i++)
        {
            await TryGetCardThemeCompleteReward();
        }
    }

    public async Task<bool> TryOpenSingleCardPackage(int cardPackageId)
    {
        for (var i = StorageCardCollection.UnOpenPackageList.Count - 1; i >= 0; i--)
        {
            var package = StorageCardCollection.UnOpenPackageList[i];
            if (package.Id == cardPackageId)
            {
                StorageCardCollection.UnOpenPackageList.RemoveAt(i);
                var task = new TaskCompletionSource<bool>();
                OpenCardPackage(package, package.Source, () => task.SetResult(true));
                await task.Task;
                var task1 = new TaskCompletionSource<bool>();
                DoAllUndoAction(() => task1.SetResult(true));
                await task1.Task;
                ;
                return true;
            }
        }

        return false;
    }

    public void ResumeWildCard(int wildCardId, int count)
    {
        StorageCardCollection.WildCards[wildCardId] -= count;
        EventDispatcher.Instance.SendEvent(new EventWildCardCountChange(TableCardWildCard[wildCardId], -count));
    }

    public void AddWildCardResources(UserData.ResourceId resourceId, int count, string biSource)
    {
        var wildCardId = 0;
        if (resourceId == UserData.ResourceId.WildCard3)
            wildCardId = 103;
        else if (resourceId == UserData.ResourceId.WildCard4)
            wildCardId = 104;
        else if (resourceId == UserData.ResourceId.WildCard5)
            wildCardId = 105;
        else
            return;
        AddWildCard(wildCardId, count, biSource);
    }

    public void AddWildCard(int wildCardId, int count, string biSource)
    {
        if (!StorageCardCollection.WildCards.ContainsKey(wildCardId))
        {
            StorageCardCollection.WildCards.Add(wildCardId, 0);
        }

        StorageCardCollection.WildCards[wildCardId] += count;
        EventDispatcher.Instance.SendEvent(new EventWildCardCountChange(TableCardWildCard[wildCardId], count));
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryCollectWildCardsObtain,
            wildCardId.ToString(), StorageCardCollection.WildCards[wildCardId].ToString(), biSource);
    }

    public void AddCardPackage(int packageId, int count, string biSource)
    {
        for (var i = 0; i < count; i++)
        {
            var newCardPackage = new StorageCardCollectionCardPackage()
            {
                Id = packageId,
                GetTime = APIManager.Instance.GetServerTime(),
                Source = biSource,
            };
            StorageCardCollection.UnOpenPackageList.Add(newCardPackage);
            EventDispatcher.Instance.SendEventImmediately<EventGetCardPackage>(
                new EventGetCardPackage(TableCardPackage[packageId]));
        }

        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryCollectCardsObtain,
            packageId.ToString(), StorageCardCollection.UnOpenPackageList.Count.ToString(), biSource);
    }

    public List<ResData> ExchangeCardPackageResource(UserData.ResourceId resourceId, int count)
    {
        if (CardCollectConfigManager.Instance.TableCardPackageResourceExchange.TryGetValue((int)resourceId,
                out var config))
        {
            var rewards = CommonUtils.FormatReward(config.RewardId, config.RewardNum);
            foreach (var reward in rewards)
            {
                reward.count *= count;
            }

            return rewards;
        }

        return null;
    }

    public int GetCardPackageCount(int packageId)
    {
        var count = 0;
        foreach (var cardPackage in StorageCardCollection.UnOpenPackageList)
            if (cardPackage.Id == packageId)
                count++;
        return count;
    }

    public Dictionary<int, CardCollectionCardItemState> UnViewedCardItemStateDictionary =
        new Dictionary<int, CardCollectionCardItemState>();

    public void InitUnViewedCardItemStateDictionary()
    {
        UnViewedCardItemStateDictionary.Clear();
        foreach (var pair in StorageCardCollection.CollectedCards)
        {
            var cardStorage = pair.Value;
            if (cardStorage.Count > 0 && !cardStorage.IsViewed)
            {
                AddUnViewedCardItem(GetCardItemState(cardStorage.Id));
            }
        }
    }

    public void AddUnViewedCardItem(CardCollectionCardItemState cardItemState)
    {
        UnViewedCardItemStateDictionary.Add(cardItemState.CardItemConfig.Id, cardItemState);
    }

    public void OnViewCard(CardCollectionCardItemState cardItemState)
    {
        var cardItemId = cardItemState.CardItemConfig.Id;
        if (UnViewedCardItemStateDictionary.ContainsKey(cardItemId))
        {
            if (StorageCardCollection.CollectedCards.TryGetValue(cardItemId, out var storageCardItem))
            {
                if (!storageCardItem.IsViewed)
                {
                    storageCardItem.IsViewed = true;
                    UnViewedCardItemStateDictionary.Remove(cardItemId);
                    EventDispatcher.Instance.SendEventImmediately(new EventViewNewCard(cardItemState));
                }
            }
        }
    }

    public int GetUnViewedCardCount(CardCollectionCardThemeState targetTheme = null)
    {
        var openThemeList = StorageCardCollection.OpenThemeList.DeepCopy();
        openThemeList.AddRange(StorageCardCollection.UnlockThemeList);
        if (openThemeList.Count == 0)
            return 0;
        // return UnViewedCardItemStateDictionary.Count;
        var count = 0;
        foreach (var pair in UnViewedCardItemStateDictionary)
        {
            var isTheme = false;
            foreach (var book in pair.Value.CardBookStateList)
            {
                foreach (var theme in book.CardThemeStateList)
                {
                    if (targetTheme != null)
                    {
                        if (theme == targetTheme)
                        {
                            isTheme = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        var upGradeTheme = theme.GetUpGradeTheme();
                        if (upGradeTheme != theme && openThemeList.Contains(upGradeTheme.CardThemeConfig.Id))
                        {
                            continue;
                        }
                        else
                        {
                            if (openThemeList.Contains(theme.CardThemeConfig.Id))
                            {
                                isTheme = true;
                                break;
                            }   
                        }   
                    }
                }

                if (isTheme)
                    break;
            }

            if (isTheme)
                count++;
        }

        return count;
    }

    public TableCardCollectionWildCard GetChangeableWildCard(CardCollectionCardItemState itemState)
    {
        TableCardCollectionWildCard bestConfig = null;
        foreach (var pair in StorageCardCollection.WildCards)
        {
            if (pair.Value > 0)
            {
                var config = TableCardWildCard[pair.Key];
                if (config.LevelRange.Contains(itemState.CardItemConfig.Level))
                {
                    if (bestConfig == null || config.Id < bestConfig.Id)
                    {
                        bestConfig = config;
                    }
                }
            }
        }

        return bestConfig;
    }

    public void OpenMainPopup()
    {
        // if (ThemeInUse != null)
        // {
        //     UICardMainController.Open(ThemeInUse);
        // }
        // else
        // {
        //     UICardMainController.Open(GetCardThemeState(1));
        // }

        UIManager.Instance.OpenUI(UINameConst.UIMainCard);
    }

    public int GetWildCardCount(int wildCardId)
    {
        if (StorageCardCollection.WildCards.TryGetValue(wildCardId, out var count))
            return count;
        return 0;
    }

    // public bool InGuideChain = false;
    // public static bool CheckGuide()
    // {
    //     // if (!Instance.StorageCardCollection.IsInit)
    //     // {
    //     //     Instance.StorageCardCollection.IsInit = true;
    //     //     Instance.AddCardPackage(999999,1);
    //     // }
    //     if (Instance.ThemeInUse != null && 
    //         !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.CardLobbyGuideEntrance) && 
    //         (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Home ||
    //          SceneFsm.mInstance.GetCurrSceneType() == StatusType.BackHome) &&
    //         !GuideSubSystem.Instance.IsShowingGuide())
    //     {
    //         if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.CardLobbyGuideEntrance, null))
    //         {
    //             // GuideSubSystem.Instance.InGuideChain = true;
    //             // CardCollectionModel.Instance.InGuideChain = true;
    //             CardCollectionModel.Instance.AddCardPackage(999999, 1,"Guide");
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    public int GetTotalStarCount()
    {
        var starCount = StorageCardCollection.StarCount;
        foreach (var cardThemeConfig in TableCardTheme)
        {
            starCount += GetCardThemeState(cardThemeConfig.Key).GetStarCount();
        }

        return starCount;
    }


    public bool TryCostStar(int costValue)
    {
        if (GetTotalStarCount() < costValue)
            return false;
        var tempCostValue = costValue;
        var costStarValue = Math.Min(tempCostValue, StorageCardCollection.StarCount);
        tempCostValue -= costStarValue;
        if (tempCostValue <= 0)
        {
            StorageCardCollection.StarCount -= costStarValue;
            return true;
        }

        Dictionary<CardCollectionCardItemState, int> costList = new Dictionary<CardCollectionCardItemState, int>();
        foreach (var cardThemeConfig in TableCardTheme)
        {
            var themeState = GetCardThemeState(cardThemeConfig.Key);
            var cardItemDic = themeState.GetCardItemConfigGroupByStarValue();
            var valueList = cardItemDic.Keys.ToList();
            valueList.Sort();
            for (var i = valueList.Count - 1; i >= 0; i--)
            {
                var value = valueList[i];
                var cardList = cardItemDic[value];
                var needCount = tempCostValue / value;
                if (needCount > 0)
                {
                    foreach (var cardItem in cardList)
                    {
                        var leftCount = cardItem.CollectCount - 1;
                        if (leftCount > 0)
                        {
                            var costCount = leftCount;
                            if (needCount < costCount)
                            {
                                costCount = needCount;
                            }

                            costList.Add(cardItem, costCount);
                            needCount -= costCount;
                            tempCostValue -= costCount * value;
                            if (needCount == 0)
                            {
                                break;
                            }
                        }
                    }
                }

                if (tempCostValue <= 0)
                {
                    break;
                }
            }

            if (tempCostValue > 0) //不浪费找不满,从低往高不过滤浪费情况找
            {
                for (var i = valueList.Count - 1; i >= 0; i--)
                {
                    var value = valueList[i];
                    var cardList = cardItemDic[value];
                    var needCount = tempCostValue / value;
                    if (tempCostValue % value != 0)
                        needCount++;
                    if (needCount > 0)
                    {
                        foreach (var cardItem in cardList)
                        {
                            var alreadyUseCount = costList.TryGetValue(cardItem, out var tempAlreadyUseCount)
                                ? tempAlreadyUseCount
                                : 0;
                            var leftCount = cardItem.CollectCount - alreadyUseCount - 1;
                            if (leftCount > 0)
                            {
                                var costCount = leftCount;
                                if (needCount < costCount)
                                {
                                    costCount = needCount;
                                }

                                if (costList.ContainsKey(cardItem))
                                {
                                    costList[cardItem] += costCount;
                                }
                                else
                                {
                                    costList.Add(cardItem, costCount);
                                }

                                needCount -= costCount;
                                tempCostValue -= costCount * value;
                                if (needCount == 0)
                                {
                                    break;
                                }
                            }
                        }

                        if (tempCostValue <= 0)
                        {
                            break;
                        }
                    }
                }
            }

            if (tempCostValue <= 0)
            {
                break;
            }
        }

        if (tempCostValue > 0)
        {
            return false;
        }
        else
        {
            StorageCardCollection.StarCount -= costStarValue;
            if (tempCostValue < 0)
            {
                StorageCardCollection.StarCount -= tempCostValue;
            }

            foreach (var pair in costList)
            {
                pair.Key.ConsumeCard(pair.Value);
            }

            return true;
        }
    }

    public List<string> GetCurrentMainPopupPath()
    {
        var paths = new List<string>();
        if (ThemeInUse != null)
        {
            paths.Add(ThemeInUse.GetCardUIName(CardUIName.UIType.UICardMain));
            paths.Add(ThemeInUse.GetCardUIName(CardUIName.UIType.UICardBook));
            paths.Add(ThemeInUse.GetCardUIName(CardUIName.UIType.UIOpenCard));
            paths.Add(ThemeInUse.GetCardUIName(CardUIName.UIType.UICardRewardBook));
            paths.Add(ThemeInUse.GetCardUIName(CardUIName.UIType.UICardRewardTheme));
        }

        if (ThemeReopenInUse != null)
        {
            paths.Add(ThemeReopenInUse.GetCardUIName(CardUIName.UIType.UICardMain));
            paths.Add(ThemeReopenInUse.GetCardUIName(CardUIName.UIType.UICardBook));
            paths.Add(ThemeReopenInUse.GetCardUIName(CardUIName.UIType.UIOpenCard));
            paths.Add(ThemeReopenInUse.GetCardUIName(CardUIName.UIType.UICardRewardBook));
            paths.Add(ThemeReopenInUse.GetCardUIName(CardUIName.UIType.UICardRewardTheme));
        }

        return paths;
    }

    public bool IsCardPackageResourceId(int id)
    {
        if ((id >= (int)UserData.ResourceId.CardPackageFreeLevel1 && id <= (int)UserData.ResourceId.WildCard5) ||
            (id >= (int)UserData.ResourceId.CardPackagePangLevel1 &&
             id <= (int)UserData.ResourceId.CardPackagePangLevel5))
        {
            return true;
        }

        return false;
    }

    public void SaveAbstractCardPackage(int resId, int count, string source)
    {
        Debug.LogError("暂存卡包" + resId + "*" + count + "_" + source);
        var resData = new StorageResData()
        {
            Id = resId,
            Count = count,
        };
        StorageCardCollection.AbstractPackages.Add(resData);
        StorageCardCollection.AbstractPackagesSource.Add(source);
    }

    public void CollectAbstractCardPackage()
    {
        if (!CardCollectionActivityModel.Instance.IsInitFromServer())
            return;
        if (StorageCardCollection.AbstractPackages.Count == 0)
            return;
        for (var i = 0; i < StorageCardCollection.AbstractPackages.Count; i++)
        {
            var resData = StorageCardCollection.AbstractPackages[i];
            var biSource = StorageCardCollection.AbstractPackagesSource[i];
            var themeState = GetCardThemeState(CardCollectionActivityModel.Instance.CurStorage.ThemeId).GetUpGradeTheme();
            var packageConfig = themeState.GetCardPackageConfig((UserData.ResourceId)resData.Id);
            AddCardPackage(packageConfig.Id, resData.Count, biSource);
            Debug.LogError("实例化卡包" + packageConfig.Id + "*" + resData.Count + "_" + biSource);
        }

        StorageCardCollection.AbstractPackages.Clear();
        StorageCardCollection.AbstractPackagesSource.Clear();
    }
    
    //检测通关开启的主题
    public int CheckUnlockTheme()
    {
        var openThemeList = StorageCardCollection.OpenThemeList.DeepCopy();
        openThemeList.AddRange(StorageCardCollection.UnlockThemeList);
        var starCount = 0;
        for (var i=0;i<openThemeList.Count ; i++)
        {
            var themeId = openThemeList[i];
            var theme = GetCardThemeState(themeId);
            if (theme.CardThemeConfig.UpGradeTheme > 0 && //拥有下一阶段主题
                !StorageCardCollection.UnlockThemeList.Contains(theme.CardThemeConfig.UpGradeTheme) && //未开启下一阶段主题
                theme.IsCompleted) //已完成该主题
            {
                var nextTheme = GetCardThemeState(theme.CardThemeConfig.UpGradeTheme);
                //解锁下一阶段主题
                Debug.LogError("已解锁下一阶段主题"+nextTheme.CardThemeConfig.Id);
                StorageCardCollection.UnlockThemeList.Add(nextTheme.CardThemeConfig.Id);
                openThemeList.Add(nextTheme.CardThemeConfig.Id);
                //回收已完成主题的星星
                starCount += RecycleThemeStar(themeId);
                //修改所有已完成主题未开启卡包的id
                foreach (var package in StorageCardCollection.UnOpenPackageList)
                {
                    var packageConfig = TableCardPackage[package.Id];
                    if (packageConfig.ThemeId == themeId && packageConfig.UpGrade > 0)
                    {
                        Debug.LogError("已修改卡包id "+package.Id+" -> "+packageConfig.UpGrade);
                        package.Id = packageConfig.UpGrade;   
                    }
                }
                EventDispatcher.Instance.SendEventImmediately(new EventCardThemeUnLock(nextTheme));
            }
        }
        return starCount;
    }

    public int RecycleThemeStar(int themeId)
    {
        var tempCostValue = 0;
        Dictionary<CardCollectionCardItemState, int> costList = new Dictionary<CardCollectionCardItemState, int>();
        var themeState = GetCardThemeState(themeId);
        var cardItemDic = themeState.GetCardItemConfigGroupByStarValue();
        var valueList = cardItemDic.Keys.ToList();
        valueList.Sort();
        for (var i = valueList.Count - 1; i >= 0; i--)
        {
            var value = valueList[i];
            var cardList = cardItemDic[value];
            foreach (var cardItem in cardList)
            {
                var leftCount = cardItem.CollectCount - 1;
                if (leftCount > 0)
                {
                    var costCount = leftCount;
                    costList.Add(cardItem, costCount);
                    tempCostValue += costCount * value;
                }
            }
        }
        StorageCardCollection.StarCount += tempCostValue;
        foreach (var pair in costList)
        {
            pair.Key.ConsumeCard(pair.Value);
        }
        Debug.LogError("已回收主题"+themeId+"所有星星，总计"+tempCostValue);
        return tempCostValue;
    }

    public string NormalCardThemeCollectStateStr()
    {
        var totalCount = 0;
        var collectCount = 0;
        foreach (var themeId in StorageCardCollection.OpenThemeList)
        {
            var themeState = GetCardThemeState(themeId);
            if (!themeState.CardThemeConfig.NeedUnLock)
            {
                totalCount++;
                if (themeState.IsCompleted)
                {
                    collectCount++;
                }
            }
        }
        return collectCount + "/" + totalCount;
    }
    public string GoldenCardThemeCollectStateStr()
    {
        var totalCount = 0;
        var collectCount = 0;
        foreach (var themeId in StorageCardCollection.OpenThemeList)
        {
            var themeState = GetCardThemeState(themeId);
            if (themeState.CardThemeConfig.UpGradeTheme > 0)
            {
                themeState = GetCardThemeState(themeState.CardThemeConfig.UpGradeTheme);
                totalCount++;
                if (themeState.IsCompleted)
                {
                    collectCount++;
                }
            }
        }
        return collectCount + "/" + totalCount;
    }

    public List<int> GetCardThemeLink(int themeId)
    {
        var themeState = GetCardThemeState(themeId);
        var link = new List<int>();
        link.Add(themeId);
        while (themeState.CardThemeConfig.DownGradeTheme > 0)
        {
            themeState = GetCardThemeState(themeState.CardThemeConfig.DownGradeTheme);
            link.Insert(0,themeState.CardThemeConfig.Id);
        }
        themeState = GetCardThemeState(themeId);
        while (themeState.CardThemeConfig.UpGradeTheme > 0)
        {
            themeState = GetCardThemeState(themeState.CardThemeConfig.UpGradeTheme);
            link.Add(themeState.CardThemeConfig.Id);
        }
        return link;
    }
}