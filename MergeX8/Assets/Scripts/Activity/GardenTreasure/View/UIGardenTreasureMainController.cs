using System;
using Activity.GardenTreasure.Model;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

public partial class UIGardenTreasureMainController : UIWindowController
{
    public override void PrivateAwake()
    {
        AwakeOperation();
        AwakeBoard();
        AwakeTreasure();
        AwakeReward();
        AwakeAnim();
        
        EventDispatcher.Instance.AddEventListener(EventEnum.GARDEN_TREASURE_PURCHASE, GardenTreasureRefresh);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
        InitBoard();
        InitTreasure();
        InitReward();

        GardenTreasureModel.Instance.RecordEnterLevelCount();

        int levelId = GardenTreasureModel.Instance.GetCurrentLevelId();
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGardenTreasureLevelEnter,
            levelId.ToString(),GardenTreasureModel.Instance.GetEnterCurrentLevelCount().ToString(),GardenTreasureModel.Instance.GetTotalEnterLevelCount().ToString());
    }

    private void OnDestroy()
    {
        DestroyBoard();
        DestroyEffect();
        
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GARDEN_TREASURE_PURCHASE, GardenTreasureRefresh);
    }

    private void GardenTreasureRefresh(BaseEvent e)
    {
        UpdateValues();
    }
}