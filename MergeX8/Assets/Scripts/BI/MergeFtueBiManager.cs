using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using BiEvent = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MergeFtueBiManager : Singleton<MergeFtueBiManager>
{
    public enum SendType
    {
        Product,
        Merge,
        FinishTask,
        TouchProduct,
    }

    private Dictionary<int, BiEvent.Types.GameEventType> productBi = new Dictionary<int, BiEvent.Types.GameEventType>()
    {
        // {102001, BiEvent.Types.GameEventType.GameEventFtue161},
        //
        // {102101, BiEvent.Types.GameEventType.GameEventFtue167},
        //
        // {102201, BiEvent.Types.GameEventType.GameEventFtue171},
    };

    private Dictionary<int, BiEvent.Types.GameEventType> mergeBi = new Dictionary<int, BiEvent.Types.GameEventType>()
    {
        // {102002, BiEvent.Types.GameEventType.GameEventFtue162},
        // {102003, BiEvent.Types.GameEventType.GameEventFtue163},
        // {102004, BiEvent.Types.GameEventType.GameEventFtue164},
        // {102005, BiEvent.Types.GameEventType.GameEventFtue165},
        // {102006, BiEvent.Types.GameEventType.GameEventFtue166},
        //
        // {102102, BiEvent.Types.GameEventType.GameEventFtue168},
        // {102103, BiEvent.Types.GameEventType.GameEventFtue169},
        // {102104, BiEvent.Types.GameEventType.GameEventFtue170},
        //
        // {102202, BiEvent.Types.GameEventType.GameEventFtue172},
        // {102203, BiEvent.Types.GameEventType.GameEventFtue173},
        // {102204, BiEvent.Types.GameEventType.GameEventFtue174},
        // {102205, BiEvent.Types.GameEventType.GameEventFtue175},
        // {102206, BiEvent.Types.GameEventType.GameEventFtue176},
        //
        // {101006, BiEvent.Types.GameEventType.GameEventFtue177},
    };

    private Dictionary<int, BiEvent.Types.GameEventType> finishTaskBi =
        new Dictionary<int, BiEvent.Types.GameEventType>()
        {
            // {100201, BiEvent.Types.GameEventType.GameEventFtue179},
            // {100202, BiEvent.Types.GameEventType.GameEventFtue180},
        };

    private Dictionary<int, BiEvent.Types.GameEventType> touchBi = new Dictionary<int, BiEvent.Types.GameEventType>()
    {
        // {101006, BiEvent.Types.GameEventType.GameEventFtue178},
    };


    public void SendFtueBi(SendType type, int param)
    {
        Dictionary<int, BiEvent.Types.GameEventType> biDictionary = null;
        switch (type)
        {
            case SendType.Merge:
            {
                biDictionary = mergeBi;
                break;
            }
            case SendType.Product:
            {
                biDictionary = productBi;
                break;
            }
            case SendType.FinishTask:
            {
                biDictionary = finishTaskBi;
                break;
            }
            case SendType.TouchProduct:
            {
                biDictionary = touchBi;
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