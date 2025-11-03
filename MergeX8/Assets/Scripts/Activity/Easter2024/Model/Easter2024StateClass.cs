
using DragonU3DSDK.Asset;
using UnityEngine;

public enum Easter2024CardType
{
    NoCard = 0,
    Score = 1,
    MultiScore = 2,
    ExtraBall = 3,
}

public enum Easter2024BallType
{
    Normal,
    Multi,
    Extra,
    DataCollection,
}

public struct Easter2024CardState
{
    public static Easter2024CardState NoCard = new Easter2024CardState() {CardType = Easter2024CardType.NoCard};
    public Easter2024CardType CardType;
    public int Score;
    public int MultiValue;
    public int BallCount;
    
    public Easter2024CardState(Easter2024CardConfig cardConfig)
    {
        CardType = (Easter2024CardType) cardConfig.CardType;
        Score = cardConfig.Score;
        MultiValue = cardConfig.MultiValue;
        BallCount = cardConfig.BallCount;
    }
    public Easter2024CardState(Easter2024CardType cardType,int value)
    {
        CardType = cardType;
        Score = CardType == Easter2024CardType.Score?value:0;
        MultiValue = CardType == Easter2024CardType.MultiScore?value:0;
        BallCount = CardType == Easter2024CardType.ExtraBall?value:0;
    }
    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;
        if (obj is Easter2024CardState)
        {
            Easter2024CardState other = (Easter2024CardState)obj;
            return this == other;
        }
        return false;
    }
    
    public static bool operator ==(Easter2024CardState a, Easter2024CardState b)
    {
        return a.GetHashCode() == b.GetHashCode();
    }
    public static bool operator !=(Easter2024CardState a, Easter2024CardState b)
    {
        return !(a == b);
    }
    public override int GetHashCode()
    {
        var typeWeight = 100000000;
        switch (CardType)
        {
            case Easter2024CardType.Score:
                return CardType.GetHashCode()*typeWeight + Score.GetHashCode();
            case Easter2024CardType.MultiScore:
                return CardType.GetHashCode()*typeWeight + MultiValue.GetHashCode();
            case Easter2024CardType.ExtraBall:
                return CardType.GetHashCode()*typeWeight + BallCount.GetHashCode();
        }
        return CardType.GetHashCode() * typeWeight;
    }

    public Sprite GetSprite()
    {
        if (CardType != Easter2024CardType.ExtraBall &&
            CardType != Easter2024CardType.MultiScore)
            return null;
        var assetName = "Card_" + 
            (CardType == Easter2024CardType.ExtraBall?"Extra":"Multi") +"_" +
            (CardType == Easter2024CardType.ExtraBall?BallCount:MultiValue);
        return ResourcesManager.Instance.GetSpriteVariant(AtlasName.Easter2024Atlas, assetName);
    }
}