using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.CardCollect;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public class CardCollectionCardItemState
{
    public static Dictionary<int, CardCollectionCardItemState> CardItemStateDictionary = new Dictionary<int, CardCollectionCardItemState>();
    public CardCollectionCardItemState(int cardId)
    {
        if (!CardItemStateDictionary.TryAdd(cardId, this))
            Debug.LogError("卡牌对象初始化错误,已存在相同卡牌id的对象,id="+cardId);
        SetCardItemId(cardId);
    }

    public string NameKey => CardItemConfig.Name;
    public string Name => LocalizationManager.Instance.GetLocalizedString(NameKey);
    public string DescribeTextKey => CardItemConfig.Text;
    public string DescribeText => LocalizationManager.Instance.GetLocalizedString(DescribeTextKey);
    public TableCardCollectionCardItem CardItemConfig;
    public int CollectCount;
    public void UpdateCollectCount()
    {
        var oldValue = CollectCount;
        CollectCount = CardCollectionModel.Instance.StorageCardCollection.CollectedCards.TryGetValue(CardItemConfig.Id,out var storageCard)
                ? storageCard.Count-storageCard.ConsumeCount : 0;
        if (CollectCount != oldValue)
        {
            EventDispatcher.Instance.SendEvent(new EventCardCountChange(this));
        }
    }
    public void SetCardItemId(int cardItemId,bool force = false)
    {
        if (!force && CardItemConfig != null && CardItemConfig.Id == cardItemId)
            return;
        CardItemConfig = CardCollectionModel.Instance.TableCardItem[cardItemId];
        CardBookStateList.Clear();
        UpdateCollectCount();
    }

    public List<CardCollectionCardBookState> CardBookStateList = new List<CardCollectionCardBookState>();
    public void BindCardBookState(CardCollectionCardBookState cardBookState)
    {
        if (!CardBookStateList.Contains(cardBookState))
        {
            // if (CardBookStateList.Count > 0)
            //     Debug.LogError("卡牌绑定多个卡册,已绑定卡册id为"+CardBookStateList.ListToString()+",将要绑定的卡册id为"+cardBookState.ToString());
            CardBookStateList.Add(cardBookState);
        }
    }

    public bool ConsumeCard(int consumeCount)
    {
        if (!CardCollectionModel.Instance.StorageCardCollection.CollectedCards.TryGetValue(CardItemConfig.Id,
                out var storageCard))
        {
            Debug.LogError("消耗卡牌错误,找不到cardItem存档");
            return false;
        }
        if (storageCard.ConsumeCount + consumeCount > storageCard.Count - 1)
        {
            Debug.LogError("消耗卡牌错误,卡牌数量不足 存量="+(storageCard.Count-1-storageCard.ConsumeCount)+" 消耗量="+ consumeCount);
            return false;
        }
        storageCard.ConsumeCount += consumeCount;
        UpdateCollectCount();
        return true;
    }
    public bool CollectOneCard(GetCardSource source,string biSource)
    {
        if (!CardCollectionModel.Instance.StorageCardCollection.CollectedCards.TryGetValue(CardItemConfig.Id,out var storageCard))
        {
            storageCard = new StorageCardCollectionCardItem() {Id = CardItemConfig.Id};
            CardCollectionModel.Instance.StorageCardCollection.CollectedCards.Add(CardItemConfig.Id,storageCard);
        }
        var isNewCard = storageCard.Count == 0;
        storageCard.Count++;
        storageCard.GetTime.Add(APIManager.Instance.GetServerTime());
        UpdateCollectCount();
        if (isNewCard)
        {
            foreach (var cardBookState in CardBookStateList)
            {
                cardBookState.OnCollectNewCard();
            }
            // UserData.Instance.AddRes(UserData.ResourceId.Star,CardItemConfig.resourceStarValue,new GameBIManager.ItemChangeReasonArgs()
            // {
            //     reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GetGallery
            // });
            CardCollectionModel.Instance.AddUnViewedCardItem(this);
            EventDispatcher.Instance.SendEvent(new EventCollectNewCardItem(this,source));
        }
        else
        {
            // CardCollectionModel.Instance.CollectStar(CardItemConfig.starValue);
        }
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGalleryCollectCards,
            CardItemConfig.Id.ToString(),CollectCount.ToString(),biSource);
        return isNewCard;
    }
    public Sprite GetCardSprite()
    {
        return ResourcesManager.Instance.GetSpriteVariant($"Card{CardItemConfig.ThemeId}Atlas", CardItemConfig.Sprite);
    }

    public bool IsUnViewed()
    {
        return CardCollectionModel.Instance.UnViewedCardItemStateDictionary.ContainsKey(CardItemConfig.Id);
    }

    public StorageCardCollectionCardItem Storage =>
        CardCollectionModel.Instance.StorageCardCollection.CollectedCards.TryGetValue(CardItemConfig.Id,
            out var storageCard)
            ? storageCard
            : null;

    public bool IsViewed=>Storage != null && Storage.IsViewed;
}