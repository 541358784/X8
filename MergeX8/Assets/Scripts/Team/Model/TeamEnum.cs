
namespace Scripts.UI
{
    public enum TeamState
    {
        Null,
        Lock,
        NotJoined,
        NotNetwork,
        Joined
    }

    public enum InitNetworkState
    {
        Nothing,
        Success,
        Fail,
    }

    public enum MyTeamRequestResult
    {
        Success,
        Fail,
        Kicked,
    }
}
