namespace DigTrench
{
    public enum ConnectState
    {
        Wall = 0,
        Tile = 1,
        Trap = 2,
        Target = 3
    }

    public enum ConnectDirection
    {
        XPositive,
        XNegative,
        YPositive,
        YNegative,
        SamePosition,
        DisConnect
    }

    public enum DigEndType
    {
        PointerUp,
        Wall,
        Trap,
        Target,
    }

    public enum TileType
    {
        Tile,
        Trap,
        StartPoint,
        EndPoint,
        // SideQuest,
    }

    public enum DigResult
    {
        Success,
        VideoSuccess,
        Failed
    }

    public enum TouchType
    {
        None,
        Background,
        Road,
    }
}