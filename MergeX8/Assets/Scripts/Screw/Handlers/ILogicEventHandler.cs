
namespace Screw
{
    public enum LogicEvent
    {
        ShowHardLevel,
        EnterLevel,
        PreMove,
        PrePairMove,
        PostMove,
        PostPairMove,
        BlockCheckFail,
        ExitLevel,
        TimeOut,
        DebugWin,
        CheckTask,
        ActionFinish,
        EnterBreakPanel,
        ExitBreakPanel,
        TwoTaskEnd,
        RefreshExtraSlot,
        RefreshTaskStatus,
        RefreshBooster,
        RefreshShield,
    }

    public class LogicEventParams
    {
        
    }
    
    public class MoveParams:LogicEventParams
    {
        public MoveAction moveAction;

        public MoveParams(MoveAction inMoveAction)
        {
            moveAction = inMoveAction;
        }
    }

    public class PairMoveParams : LogicEventParams
    {
        public PairMoveAction moveAction;

        public PairMoveParams(PairMoveAction inMoveAction)
        {
            moveAction = inMoveAction;
        }
    }

    public class MoveEndParams : LogicEventParams
    {
        public ScrewModel ScrewModel;

        public MoveEndParams(ScrewModel inScrewModel)
        {
            ScrewModel = inScrewModel;
        }
    }


    public class ExecuteOrder
    {
        public const int MemberHandlerOrder = 0;
        public const int GuideHandlerExecuteOrder = 5;
        public const int BoosterHandlerOrder = 10;
        public const int LevelTaskHandler = 15;
        public const int LevelStateHandler = 20;
        public const int GameTimerHandler = 30;
        public const int ShieldHandler = 40;
    }
    
    public interface ILogicEventHandler
    {
        public int GetExecuteOrder();
        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams);
    }
}