
using DragonU3DSDK.Asset;
using UnityEngine;

public enum SnakeLadderCardType
{
    NoCard = 0,
    Step = 1,
    Score = 2,
    MultiScore = 3,
    MultiStep = 4,
    Wild = 5,
    Defense = 6,
}

public enum SnakeLadderBlockType
{
    Score,
    Reward,
    Ladder,
    Snake,
    Start,
    End,
}

public struct SnakeLadderCardState
{
    public static SnakeLadderCardState NoCard = new SnakeLadderCardState() {CardType = SnakeLadderCardType.NoCard};
    public SnakeLadderCardType CardType;
    public int Score;
    public int Step;
    public int MultiScore;
    public int MultiStep;
    
    public SnakeLadderCardState(SnakeLadderCardConfig cardConfig)
    {
        CardType = (SnakeLadderCardType) cardConfig.CardType;
        Score = cardConfig.Score;
        Step = cardConfig.Step;
        MultiScore = cardConfig.ScoreMultiValue;
        MultiStep = cardConfig.StepMultiValue;
    }
    public SnakeLadderCardState(SnakeLadderCardType cardType,int value)
    {
        CardType = cardType;
        Score = CardType == SnakeLadderCardType.Score?value:0;
        Step = CardType == SnakeLadderCardType.Step?value:0;
        MultiScore = CardType == SnakeLadderCardType.MultiScore?value:0;
        MultiStep = CardType == SnakeLadderCardType.MultiStep?value:0;
    }
    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;
        if (obj is SnakeLadderCardState)
        {
            SnakeLadderCardState other = (SnakeLadderCardState)obj;
            return this == other;
        }
        return false;
    }
    
    public static bool operator ==(SnakeLadderCardState a, SnakeLadderCardState b)
    {
        return a.GetHashCode() == b.GetHashCode();
    }
    public static bool operator !=(SnakeLadderCardState a, SnakeLadderCardState b)
    {
        return !(a == b);
    }
    public override int GetHashCode()
    {
        var typeWeight = 100000000;
        switch (CardType)
        {
            case SnakeLadderCardType.Score:
                return CardType.GetHashCode()*typeWeight + Score.GetHashCode();
            case SnakeLadderCardType.MultiScore:
                return CardType.GetHashCode()*typeWeight + MultiScore.GetHashCode();
            case SnakeLadderCardType.MultiStep:
                return CardType.GetHashCode()*typeWeight + MultiStep.GetHashCode();
            case SnakeLadderCardType.Step:
                return CardType.GetHashCode()*typeWeight + Step.GetHashCode();
        }
        return CardType.GetHashCode() * typeWeight;
    }
}