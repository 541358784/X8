using System.ComponentModel;
using DragonU3DSDK.Storage;
using UnityEngine;


public partial class SROptions
{
    [Category(Story)]
    [DisplayName("重置Story")]
    public void ResetStory()
    {
        StorageManager.Instance.GetStorage<StorageHome>().DialogData.Clear();
    }

    private int _debugStoryId = -1;
    [Category(Story)]
    [DisplayName("Debug Story Id")]
    public int DebugStoryId
    {
        get
        {
            return _debugStoryId;
        }
        set
        {
            _debugStoryId = value;
        }
    }
    
    [Category(Story)]
    [DisplayName("Start Story")]
    public void StartStory()
    {
        StorySubSystem.Instance.DEBUG_StartStory(_debugStoryId);
    }
    
    [Category(Story)]
    [DisplayName("Start StoryMovie")]
    public void StartStoryMovie()
    {
        StoryMovieSubSystem.Instance.DEBUG_StartStory(_debugStoryId);
    }
    
    [Category(Story)]
    [DisplayName("Clean StoryMovie")]
    public void CleanMovieStory()
    {
        StorageManager.Instance.GetStorage<StorageHome>().StoryMovieData.FinishedId.Clear();
    }
    
}