
using DragonU3DSDK.Asset;
using UnityEngine;

public enum MonopolyCardType
{
    NoCard = 0,
    Score = 2,
    MultiScore = 3,
    MultiStep = 4,
    Wild = 5,
}

public enum MonopolyBlockType
{
    Score=0,
    Reward=1,
    MiniGame=2,
    Card=3,
    Start=4,
}

public struct MonopolyCardState
{
    public static MonopolyCardState NoCard = new MonopolyCardState() {CardType = MonopolyCardType.NoCard};
    public MonopolyCardType CardType;
    public int Score;
    public int MultiScore;
    public int MultiStep;
    
    public MonopolyCardState(MonopolyCardConfig cardConfig)
    {
        CardType = (MonopolyCardType) cardConfig.CardType;
        Score = cardConfig.Score;
        MultiScore = cardConfig.ScoreMultiValue;
        MultiStep = cardConfig.StepMultiValue;
    }
    public MonopolyCardState(MonopolyCardType cardType,int value)
    {
        CardType = cardType;
        Score = CardType == MonopolyCardType.Score?value:0;
        MultiScore = CardType == MonopolyCardType.MultiScore?value:0;
        MultiStep = CardType == MonopolyCardType.MultiStep?value:0;
    }
    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;
        if (obj is MonopolyCardState)
        {
            MonopolyCardState other = (MonopolyCardState)obj;
            return this == other;
        }
        return false;
    }
    
    public static bool operator ==(MonopolyCardState a, MonopolyCardState b)
    {
        return a.GetHashCode() == b.GetHashCode();
    }
    public static bool operator !=(MonopolyCardState a, MonopolyCardState b)
    {
        return !(a == b);
    }
    public override int GetHashCode()
    {
        var typeWeight = 100000000;
        switch (CardType)
        {
            case MonopolyCardType.Score:
                return CardType.GetHashCode()*typeWeight + Score.GetHashCode();
            case MonopolyCardType.MultiScore:
                return CardType.GetHashCode()*typeWeight + MultiScore.GetHashCode();
            case MonopolyCardType.MultiStep:
                return CardType.GetHashCode()*typeWeight + MultiStep.GetHashCode();
        }
        return CardType.GetHashCode() * typeWeight;
    }
}