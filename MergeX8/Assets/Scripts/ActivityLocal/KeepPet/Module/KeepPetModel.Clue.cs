
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;

public partial class KeepPetModel
{
    public bool HaveClue(int id)
    {
        return Storage.ClueDictionary.ContainsKey(id);
    }

    public void GetClue(int id)
    {
        if(HaveClue(id))
            return;
        
        Storage.ClueDictionary.Add(id, id);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventKeepPetClueCollect,id.ToString());
    }
}