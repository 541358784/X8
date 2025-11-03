using System;
using System.Collections.Generic;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class CardCollectionCardBookState
{
    public static Dictionary<int, CardCollectionCardBookState> CardBookStateDictionary = new Dictionary<int, CardCollectionCardBookState>();
    public TableCardCollectionCardBook CardBookConfig;
    public string NameKey => CardBookConfig.Name;
    public bool IsCompleted => CardCollectionModel.Instance.StorageCardCollection.CompletedCardBooks.ContainsKey(CardBookConfig.Id);
    public bool CanCollect => !IsCompleted && CollectCardItemCount == MaxCardItemCount;
    public Dictionary<int, CardCollectionCardItemState> CardItemStateList = new Dictionary<int, CardCollectionCardItemState>();
    public int CollectCardItemCount;
    public void UpdateCollectCardItemCount()
    {
        CollectCardItemCount = 0;
        foreach (var cardId in CardBookConfig.Cards)
        {
            if (CardItemStateList.TryGetValue(cardId, out var storageCardItem) && storageCardItem.CollectCount > 0)
            {
                if (storageCardItem.CardItemConfig.MergeView)
                {
                    if (storageCardItem.IsViewed)
                        CollectCardItemCount++;
                }
                else
                {
                    CollectCardItemCount++;
                }
            }
        }
    }
    public int MaxCardItemCount;
    
    public CardCollectionCardBookState(int cardBookId)
    {
        if (!CardBookStateDictionary.TryAdd(cardBookId, this))
            Debug.LogError("卡册对象初始化错误,已存在相同卡册id的对象,id="+cardBookId);
        SetCardBookId(cardBookId);
    }
    public void SetCardBookId(int cardBookId,bool force = false)
    {
        if (!force && CardBookConfig != null && CardBookConfig.Id == cardBookId)
            return;
        CardBookConfig = CardCollectionModel.Instance.TableCardBook[cardBookId];
        MaxCardItemCount = CardBookConfig.Cards.Count;
        CardItemStateList.Clear();
        foreach (var cardId in CardBookConfig.Cards)
        {
            var cardItem = CardCollectionModel.Instance.GetCardItemState(cardId);
            if (force)
                cardItem.SetCardItemId(cardId, true);
            cardItem.BindCardBookState(this);
            CardItemStateList.Add(cardId,cardItem);
        }
        UpdateCollectCardItemCount();
        CheckCanCollect();//初始化时把可领取的卡册完成奖励push到队列中
    }
    public List<CardCollectionCardThemeState> CardThemeStateList = new List<CardCollectionCardThemeState>();
    public void BindCardThemeState(CardCollectionCardThemeState cardThemeState)
    {
        if (!CardThemeStateList.Contains(cardThemeState))
        {
            // if (CardThemeStateList.Count > 0)
            //     Debug.LogError("卡册绑定多个卡册主题,已绑定卡册主题id为"+CardThemeStateList.ListToString()+",将要绑定的卡册主题id为"+cardThemeState.ToString());
            CardThemeStateList.Add(cardThemeState);
        }
    }

    public override string ToString()
    {
        return "卡册id=" + CardBookConfig.Id;
    }

    public void OnCollectNewCard()
    {
        UpdateCollectCardItemCount();
        CheckCanCollect();
    }

    public void CheckCanCollect()
    {
        if (CanCollect)
        {
            CardCollectionModel.Instance.PushCanCollectRewardCardBookStateList(this);
        }
    }

    public void CompleteCardBook()
    {
        if (IsCompleted)
        {
            Debug.LogError("重复完成卡册");
            return;   
        }
        var storageCardBook = new StorageCardCollectionCardBook()
        {
            Id = CardBookConfig.Id,
            CompletedTime = APIManager.Instance.GetServerTime()
        };
        CardCollectionModel.Instance.StorageCardCollection.CompletedCardBooks.Add(CardBookConfig.Id,storageCardBook);
        EventDispatcher.Instance.SendEvent(new EventCardBookComplete(this));
        foreach (var cardThemeState in CardThemeStateList)
        {
            cardThemeState.OnCollectNewCardBook();
        }
    }

    public Sprite GetIconSprite()
    {
        return ResourcesManager.Instance.GetSpriteVariant($"Card{CardBookConfig.ThemeId}Atlas", CardBookConfig.IconSprite);
    }
    public Sprite GetLockSprite()
    {
        return ResourcesManager.Instance.GetSpriteVariant($"Card{CardBookConfig.ThemeId}Atlas", CardBookConfig.IconSprite.Replace("Icon","Lock"));
    }

    public List<ResData> CompletedReward
    {
        get
        {
            var curStorage = CardCollectionActivityModel.Instance.CurStorage;
            var reopenStorage = CardCollectionReopenActivityModel.Instance.CurStorage;
            if (curStorage==null && reopenStorage == null)
                return new List<ResData>();
            if (curStorage != null)
            {
                var curThemeList = CardCollectionModel.Instance.GetCardThemeLink(curStorage.ThemeId);
                if (curThemeList.Contains(CardThemeStateList[0].CardThemeConfig.Id))
                {
                    return CommonUtils.FormatReward(CardBookConfig.RewardId, CardBookConfig.RewardNum);
                }   
            }
            if (reopenStorage != null)
            {
                 var reopenThemeList = CardCollectionModel.Instance.GetCardThemeLink(reopenStorage.ThemeId);
                 reopenThemeList.RemoveAt(0);//返场卡册基础主题没卡集奖励
                 if (reopenThemeList.Contains(CardThemeStateList[0].CardThemeConfig.Id))
                 {
                     return CommonUtils.FormatReward(CardBookConfig.RewardId, CardBookConfig.RewardNum);
                 }   
            }
            return new List<ResData>();
        }
    }

    public void CollectCompleteReward(Action callback)
    {
        if (!CanCollect)
            return;
        CompleteCardBook();
        var itemChangeReason = new GameBIManager.ItemChangeReasonArgs()
            {reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GetGallery};
        UserData.Instance.AddRes(CompletedReward,itemChangeReason);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryCollectSet,
            CardBookConfig.Id.ToString());
        //领奖弹窗
        UICardRewardController.Open(this,callback);
    }
    public int GetCardBookUnViewedCardCount()
    {
        var unViewCardCount = 0;
        foreach (var pair in CardItemStateList)
        {
            if (pair.Value.IsUnViewed())
            {
                unViewCardCount++;
            }
        }
        return unViewCardCount;
    }
}