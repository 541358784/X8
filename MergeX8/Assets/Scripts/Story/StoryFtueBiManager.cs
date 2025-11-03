using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using BiEvent = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class StoryFtueBiManager : Singleton<StoryFtueBiManager>
{
    public enum SendType
    {
        Start,
        TipContinue,
        Skip,
    }

    private Dictionary<int, BiEvent.Types.GameEventType> startStory = new Dictionary<int, BiEvent.Types.GameEventType>()
    {
        {101001, BiEvent.Types.GameEventType.GameEventFtue4},
        {102001, BiEvent.Types.GameEventType.GameEventFtue39},
        {103001, BiEvent.Types.GameEventType.GameEventFtue44},

    };


    private Dictionary<int, BiEvent.Types.GameEventType> continueStory = new Dictionary<int, BiEvent.Types.GameEventType>()
    {
        {101001, BiEvent.Types.GameEventType.GameEventFtue5},
        {102001, BiEvent.Types.GameEventType.GameEventFtue40},
        {103001, BiEvent.Types.GameEventType.GameEventFtue45},
    };
    
    private Dictionary<int, BiEvent.Types.GameEventType> skipStory = new Dictionary<int, BiEvent.Types.GameEventType>()
    {
        {101001, BiEvent.Types.GameEventType.GameEventFtue6},
        {102001, BiEvent.Types.GameEventType.GameEventFtue41},
        {103001, BiEvent.Types.GameEventType.GameEventFtue46},
    };


    public void SendFtueBi(SendType type, int param)
    {
        Dictionary<int, BiEvent.Types.GameEventType> biDictionary = null;
        switch (type)
        {
            case SendType.Start:
            {
                GameBIManager.Instance.SendGameEvent(BiEvent.Types.GameEventType.GameEventStoryShow, param.ToString());
                biDictionary = startStory;
                break;
            }
            case SendType.Skip:
            {                
                GameBIManager.Instance.SendGameEvent(BiEvent.Types.GameEventType.GameEventStorySkip, param.ToString());
                biDictionary = skipStory;
                break;
            }
            case SendType.TipContinue:
            {
                GameBIManager.Instance.SendGameEvent(BiEvent.Types.GameEventType.GameEventStoryContinue, param.ToString());
                biDictionary = continueStory;
                break;
            }
        }

        if (biDictionary == null)
            return;

        if (!biDictionary.ContainsKey(param))
            return;

        GameBIManager.Instance.SendGameEvent(biDictionary[param]);
    }
}