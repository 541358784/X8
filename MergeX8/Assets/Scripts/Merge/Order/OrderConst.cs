namespace Merge.Order
{
    public enum MainOrderType
    {
        None=-1,
        Fixed = 0,
        Random1 =1,
        Random2 = 2,
        Random3 = 3,
        Random4 = 4,
        Special = 5,
        SpecialGold = 6,
        Branch = 7,
        Urgency = 8,
        Random5New = 9,
        ReturnUserFree = 13,
        Random6 = 10,
        Recomment = 12,
        SecondRecycle = 15,
        
        Append = 20,
        Time = 30,
        Limit = 31,
        Craze = 32,
        KeepPet = 50,
        Team = 60,
    }

    public enum SlotDefinition
    {
        Slot1 = 1, 
        Slot2 = 2, 
        Slot3 = 3, 
        Slot4 = 4, 
        Slot5 = 5, 
        SpecialSlot = 6, 
        BranchSlot = 7, 
        Slot8 = 8, 
        Slot10 = 10, 
        Recomment = 12,
        UserFree = 13,
        SlotCount = 14,
        
        SecondRecycle = 15,
        
        Append = 20,
        Time = 30,
        Limit = 31,
        Craze = 32,
        KeepPet = 50,
        Special=100, //海豚海豹
        Team = 60,
    }


    public enum EItemMultipleTypeOrderState
    {
        NoExitMap = 1000, 
        NoExitMapLevelUp = 10000, 
        ExitMapFinishEnough = 100, 
        NoMultipleExitMap = 1  
    }

    public enum CreateOrderType
    {
        Level, //等级生成
        Difficulty,//难度生成
    }


    public enum Difficulty
    {
        maxDifficulty = 100000 - 5
    }
}