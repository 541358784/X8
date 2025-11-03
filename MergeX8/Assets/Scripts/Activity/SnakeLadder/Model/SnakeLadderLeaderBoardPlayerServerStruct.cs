using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Newtonsoft.Json;

public class SnakeLadderLeaderBoardPlayerServerStruct
{
    public bool IsFake;
    public int StarCount;
    public ulong StarUpdateTime;
    public ulong PlayerId;
    public string PlayerName = "";
    public int AvatarIconId;
    public int AvatarIconFrameId;
    public AvatarViewState ViewState;
    public void SetValue(SnakeLadderLeaderBoardPlayerServerStruct dstPlayer)
    {
        IsFake = dstPlayer.IsFake;
        PlayerId = dstPlayer.PlayerId;
        PlayerName = dstPlayer.PlayerName;
        StarCount = dstPlayer.StarCount;
        AvatarIconId = dstPlayer.AvatarIconId;
        StarUpdateTime = dstPlayer.StarUpdateTime;
        AvatarIconFrameId = dstPlayer.AvatarIconFrameId;
        ViewState = dstPlayer.ViewState;
    }
    public SnakeLadderLeaderBoardPlayerServerStruct Copy()
    {
        var newObject = new SnakeLadderLeaderBoardPlayerServerStruct();
        newObject.SetValue(this);
        return newObject;
    }

    public bool SameAs(SnakeLadderLeaderBoardPlayerServerStruct dstPlayer)
    {
        return IsFake == dstPlayer.IsFake &&
               PlayerId == dstPlayer.PlayerId &&
               PlayerName == dstPlayer.PlayerName &&
               StarCount == dstPlayer.StarCount &&
               AvatarIconId == dstPlayer.AvatarIconId &&
               StarUpdateTime == dstPlayer.StarUpdateTime && 
               AvatarIconFrameId == dstPlayer.AvatarIconFrameId &&
               ViewState.SameAs(dstPlayer.ViewState);
    }
    
    public static SnakeLadderLeaderBoardPlayerServerStruct HandleServerRobotData(LeaderboardEntry serverData)
    {
        var serverDataStruct =
            JsonConvert.DeserializeObject<SnakeLadderLeaderBoardPlayerServerStruct>(serverData.Extra);
        if (serverDataStruct.IsFake)
        {
            serverDataStruct.StarCount = (int)serverData.Score;
            serverDataStruct.StarUpdateTime = ulong.MinValue;
            serverDataStruct.PlayerId = serverData.LeaderboardEntryId.ToULong();
            serverDataStruct.PlayerName = "Robot"+serverData.LeaderboardEntryId;
            serverDataStruct.AvatarIconId = HeadIconUtils.GetRandomHead();
            serverDataStruct.AvatarIconFrameId = HeadIconUtils.GetRandomHeadFrame();
        }

        if (serverDataStruct.ViewState == null || serverDataStruct.ViewState.SameAs(default(AvatarViewState)))
        {
            serverDataStruct.ViewState = new AvatarViewState(serverDataStruct.AvatarIconId, serverDataStruct.AvatarIconFrameId, serverDataStruct.PlayerName, 
                serverDataStruct.PlayerId == StorageManager.Instance.GetStorage<StorageCommon>().PlayerId);
        }
        return serverDataStruct;
    }
}