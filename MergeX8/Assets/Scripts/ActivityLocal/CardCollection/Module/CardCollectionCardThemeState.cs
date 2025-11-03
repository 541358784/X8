using System;
using System.Collections.Generic;
using System.Linq;
using ActivityLocal.CardCollection.Home;
using ActivityLocal.CardCollection.UI;
using Decoration;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Dynamic;
using Gameplay;
using SomeWhere;
using UnityEngine;

public class CardCollectionCardThemeState
{
    public static Dictionary<int, CardCollectionCardThemeState> CardThemeStateDictionary = new Dictionary<int, CardCollectionCardThemeState>();
    public TableCardCollectionTheme CardThemeConfig;
    public string NameKey => CardThemeConfig.Name;
    public bool IsCompleted => CardCollectionModel.Instance.StorageCardCollection.CompletedCardThemes.ContainsKey(CardThemeConfig.Id);
    public bool CanCollect => !IsCompleted && CollectCardBookCount == MaxCardBookCount;
    public Dictionary<int, CardCollectionCardBookState> CardBookStateList = new Dictionary<int, CardCollectionCardBookState>();
    public int CollectCardBookCount;
    public void UpdateCollectCardBookCount()
    {
        CollectCardBookCount = 0;
        foreach (var bookId in CardThemeConfig.CardBooks)
        {
            if (CardCollectionModel.Instance.StorageCardCollection.CompletedCardBooks.ContainsKey(bookId))
                CollectCardBookCount++;
        }
    }
    public int MaxCardBookCount;
    
    public CardCollectionCardThemeState(int cardThemeId)
    {
        if (!CardThemeStateDictionary.TryAdd(cardThemeId, this))
            Debug.LogError("卡册对象初始化错误,已存在相同卡册id的对象,id="+cardThemeId);
        SetCardThemeId(cardThemeId);
    }
    public void SetCardThemeId(int cardThemeId,bool force = false)
    {
        if (!force && CardThemeConfig != null && CardThemeConfig.Id == cardThemeId)
            return;
        CardThemeConfig = CardCollectionModel.Instance.TableCardTheme[cardThemeId];
        MaxCardBookCount = CardThemeConfig.CardBooks.Count;
        CardBookStateList.Clear();
        foreach (var bookId in CardThemeConfig.CardBooks)
        {
            var cardBook = CardCollectionModel.Instance.GetCardBookState(bookId);
            if (force)
                cardBook.SetCardBookId(bookId, true);
            cardBook.BindCardThemeState(this);
            CardBookStateList.Add(bookId,cardBook);
        }
        UpdateCollectCardBookCount();
        CheckCanCollect();//初始化时把可领取的卡册完成奖励push到队列中
    }

    public override string ToString()
    {
        return "卡册id=" + CardThemeConfig.Id;
    }

    public void OnCollectNewCardBook()
    {
        UpdateCollectCardBookCount();
        CheckCanCollect();
    }

    public void CheckCanCollect()
    {
        if (CanCollect)
        {
            CardCollectionModel.Instance.PushCanCollectRewardCardThemeStateList(this);
        }
    }

    public void CompleteCardTheme()
    {
        if (IsCompleted)
        {
            Debug.LogError("重复完成卡册");
            return;   
        }
        var storageCardTheme = new StorageCardCollectionCardTheme()
        {
            Id = CardThemeConfig.Id,
            CompletedTime = APIManager.Instance.GetServerTime()
        };
        CardCollectionModel.Instance.StorageCardCollection.CompletedCardThemes.Add(CardThemeConfig.Id,storageCardTheme);
        EventDispatcher.Instance.SendEvent(new EventCardThemeComplete(this));
    }

    public Sprite GetIconSprite()
    {
        // return ResourcesManager.Instance.GetSpriteVariant($"Card{CardThemeConfig.Id}Atlas", CardThemeConfig.IconSprite);
        return ResourcesManager.Instance.GetSpriteVariant("CardMainAtlas", CardThemeConfig.IconSprite);
        
    }
    public Sprite GetLockSprite()
    {
        // return ResourcesManager.Instance.GetSpriteVariant($"Card{CardThemeConfig.Id}Atlas", CardThemeConfig.IconSprite.Replace("Icon","Lock"));
        return ResourcesManager.Instance.GetSpriteVariant("CardMainAtlas", CardThemeConfig.IconSprite.Replace("Icon","Lock"));
    }

    public List<ResData> CompletedReward => CommonUtils.FormatReward(CardThemeConfig.RewardId, CardThemeConfig.RewardNum);
    public void CollectCompleteReward(Action callback)
    {
        if (!CanCollect)
            return;
        CompleteCardTheme();
        var itemChangeReason = new GameBIManager.ItemChangeReasonArgs()
            {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GetGallery};
        UserData.Instance.AddRes(CompletedReward,itemChangeReason);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryCollectBook,
            CardThemeConfig.Id.ToString());
        //领奖弹窗
        UICardRewardBookController.Open(this,callback);
    }
    public int GetCardThemeUnViewedCardCount()
    {
        var unViewCardCount = 0;
        foreach (var pair in CardBookStateList)
        {
            if (pair.Value.GetCardBookUnViewedCardCount() > 0)
            {
                unViewCardCount++;
            }
        }
        return unViewCardCount;
    }
    public Dictionary<string, string> GetNeedDownloadAssets()
    {
        List<AssetGroup> resGroupList = CardThemeConfig.GetDownloadAssetGroups();
        if (resGroupList == null) 
            return new Dictionary<string, string>();
        return AssetCheckManager.Instance.ResDownloadFitter(resGroupList,true);
    }
    public string GetCardUIName(CardUIName.UIType type)
    {
        return "Card" + CardThemeConfig.Id + "/" + CardUIName.CardUINameDic[type];
    }
    public void AddUIWindowInfo()
    {
        foreach (var pair in CardUIName.CardUINameDic)
        {
            UIManager.Instance._WindowMetaPublic(GetCardUIName(pair.Key), UIWindowLayer.Normal, false);
        }
    }
    public bool IsResReady()
    {
        return CardCollectionModel.Instance.IsResReady(this);
    }

    public void TryDownloadRes(Action<float, string> onProgressUpdate = null,Action<bool> onFinish = null)
    {
        CardCollectionModel.Instance.TryDownloadRes(this,onProgressUpdate,onFinish);
    }

    public void InitNewRandomLogicStorage()
    {
        if (CardCollectionModel.Instance.StorageCardCollection.ThemeRandomGroupDic.ContainsKey(CardThemeConfig.Id))
        {
            return;
        }

        var randomConfigDic = CardCollectionModel.Instance.TableCardRandomGroup;
        var themeRandomGroupStorage = new StorageCardCollectionThemeRandomGroup();
        var configList = new Dictionary<int, Dictionary<int,Tuple<TableCardCollectionRandomGroup, List<CardCollectionCardItemState>>>>();
        themeRandomGroupStorage.ThemeId = CardThemeConfig.Id;
        foreach (var pair in CardBookStateList)
        {
            var book = pair.Value;
            foreach (var pair2 in book.CardItemStateList)
            {
                var itemState = pair2.Value;
                var item = itemState.CardItemConfig;
                var group = randomConfigDic[item.GroupId];
                if (!configList.ContainsKey(group.Level))
                {
                    configList.Add(group.Level,new Dictionary<int,Tuple<TableCardCollectionRandomGroup, List<CardCollectionCardItemState>>>());
                }
                if (!configList[group.Level].ContainsKey(group.Id))
                {
                    configList[group.Level].Add(group.Id,new Tuple<TableCardCollectionRandomGroup, List<CardCollectionCardItemState>>(
                        group,new List<CardCollectionCardItemState>()));
                }
                configList[group.Level][group.Id].Item2.Add(itemState);
            }
        }

        foreach (var levelRandomConfig in configList)
        {
            var levelRandomGroup = new StorageCardCollectionLevelRandomGroup();
            levelRandomGroup.Level = levelRandomConfig.Key;
            themeRandomGroupStorage.LevelRandomGroupDic.Add(levelRandomConfig.Key,levelRandomGroup);
            var groupIdList = levelRandomConfig.Value.Keys.ToList();
            groupIdList.Sort();
            for (var i = 0; i < groupIdList.Count; i++)
            {
                var groupConfigPair = levelRandomConfig.Value[groupIdList[i]];
                var groupConfig = groupConfigPair.Item1;
                var cardItems = groupConfigPair.Item2;
                var randomGroup = new StorageCardCollectionRandomGroup();
                levelRandomGroup.RandomGroups.Add(randomGroup);
                randomGroup.LastLevelWeightScale = groupConfig.LastLevelWeightUp;
                randomGroup.LevelUpLeftCount = groupConfig.MaxCount - groupConfig.NextLevelCount;
                var cardCount = groupConfig.MaxCount;
                foreach (var cardItemState in cardItems)
                {
                    randomGroup.Pool.Add(cardItemState.CardItemConfig.Id,2);
                    cardCount-=2;
                }
                while (cardCount > 0)
                {
                    var randomCard = cardItems.RandomPickOne();
                    randomGroup.Pool[randomCard.CardItemConfig.Id]++;
                    cardCount--;
                }
            }
        }
        CardCollectionModel.Instance.StorageCardCollection.ThemeRandomGroupDic.Add(CardThemeConfig.Id,themeRandomGroupStorage);
        Debug.LogError("新抽卡随机逻辑初始化,themeId="+CardThemeConfig.Id);
    }
    public string GetTaskItemAssetPath()
    {
        return "Prefabs/Card" + CardThemeConfig.Id + "/TaskList_CardCollection";
    }
    public string GetAuxItemAssetPath()
    {
        return "Prefabs/Card" + CardThemeConfig.Id + "/Aux_CardCollection";
    }
    
    public void CreateTaskEntrance()
    {
        DynamicEntry_Game_CardCollection dynamicEntry = new DynamicEntry_Game_CardCollection(this);
        DynamicEntryManager.Instance.RegisterDynamicEntry<DynamicEntry_Game_CardCollection>(this, dynamicEntry);
    }
    public void CreateAuxEntrance()
    {
        DynamicEntry_Home_CardCollectionTheme dynamicEntry = new DynamicEntry_Home_CardCollectionTheme(this);
        DynamicEntryManager.Instance.RegisterDynamicEntry<DynamicEntry_Home_CardCollectionTheme>("Aux_CardTheme"+CardThemeConfig.Id, dynamicEntry);
    }
}