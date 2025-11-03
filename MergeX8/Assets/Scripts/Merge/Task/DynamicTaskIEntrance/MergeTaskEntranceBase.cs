using DragonU3DSDK.Asset;
using UnityEngine;

public abstract class MergeTaskEntranceBase
{
    public abstract bool CanCreateEntrance();
    public abstract MonoBehaviour CreateEntrance();
}