using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    private const string CardCollection = "卡牌";
    [Category(CardCollection)]
    [DisplayName("重置卡牌系统")]
    public void ResetCardCollection()
    {
        CardCollectionActivityModel.Instance.Storage.Clear();
        CardCollectionModel.Instance.StorageCardCollection.Clear();
        CardCollectionModel.Instance.ForceInitConfig();
        // var mainPopup = UIManager.Instance.GetOpenedUIByPath<UICardMainController>(UINameConst.UICardMain);
        // if (mainPopup)
        //     mainPopup.InitView();
        var guideIdList = new List<int>() {701,702,703,704,705,706,707,708};
        CleanGuideList(guideIdList);
        if (CardCollectionActivityModel.Instance.IsInitFromServer())
        {
            CardCollectionModel.Instance.StorageCardCollection.OpenThemeList.Add(CardCollectionActivityModel.Instance.CurStorage.ThemeId);
        }
        if (CardCollectionReopenActivityModel.Instance.IsInitFromServer())
        {
            CardCollectionModel.Instance.StorageCardCollection.OpenThemeList.Add(CardCollectionReopenActivityModel.Instance.CurStorage.ThemeId);
        }
    }

    private int _cardItemId;
    [Category(CardCollection)]
    [DisplayName("卡牌ID")]
    public int CardItemId
    {
        get=>_cardItemId;
        set=>_cardItemId=value;
    }
    
    [Category(CardCollection)]
    [DisplayName("获得卡牌")]
    public void GetCardCollection()
    {
        if (!CardCollectionModel.Instance.TableCardItem.ContainsKey(_cardItemId))
        {
            Debug.LogError("卡牌ID错误");
            CardItemId++;
            return;
        }
        if (CardCollectionModel.Instance.GetCardItemState(_cardItemId).CollectOneCard(GetCardSource.Debug,"Debug"))
        {
            CardCollectionModel.Instance.DoAllUndoAction();
        }
        CardItemId++;
    }
    
    private int _getCardCount;
    
    [Category(CardCollection)]
    [DisplayName("连续获得数量")]
    public int GetCardCount
    {
        get=>_getCardCount;
        set=>_getCardCount=value;
    }

    [Category(CardCollection)]
    [DisplayName("连续获得卡牌")]
    public void GetMultiCard()
    {
        for (var i = 0; i < GetCardCount; i++)
        {
            GetCardCollection();
        }
    }
    
    private int _wildCardItemId;
    [Category(CardCollection)]
    [DisplayName("万能牌ID")]
    public int WildCardItemId
    {
        get=>_wildCardItemId;
        set=>_wildCardItemId=value;
    }
    
    [Category(CardCollection)]
    [DisplayName("获得万能牌")]
    public void GetWildCardCollection()
    {
        if (!CardCollectionModel.Instance.TableCardWildCard.ContainsKey(_wildCardItemId))
        {
            Debug.LogError("万能牌ID错误");
            return;
        }
        CardCollectionModel.Instance.AddWildCard(_wildCardItemId,1,"Debug");
    }
    
    private int _cardPackageId;
    [Category(CardCollection)]
    [DisplayName("卡包ID")]
    public int CardPackageId
    {
        get=>_cardPackageId;
        set=>_cardPackageId=value;
    }
    
    [Category(CardCollection)]
    [DisplayName("获得卡包")]
    public void GetPackageCardCollection()
    {
        if (!CardCollectionModel.Instance.TableCardPackage.ContainsKey(_cardPackageId))
        {
            Debug.LogError("卡包ID错误");
            return;
        }
        CardCollectionModel.Instance.AddCardPackage(_cardPackageId,1,"Debug");
    }
    
    [Category(CardCollection)]
    [DisplayName("蓝星星")]
    public int CardBlueStarCount
    {
        get=>CardCollectionModel.Instance.StorageCardCollection.StarCount;
        set
        {
            CardCollectionModel.Instance.StorageCardCollection.StarCount = value;
        }
    }
}