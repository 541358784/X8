using System;
using System.Collections.Generic;
using DragonU3DSDK.Storage;
using UnityEngine;

public class MixMasterEntranceRedPoint:MonoBehaviour
{
    private StorageMixMaster Storage;
    private void Awake()
    {
        EventDispatcher.Instance.AddEvent<EventMixMasterUpdateRedPoint>(UpdateRedPoint);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventMixMasterUpdateRedPoint>(UpdateRedPoint);
    }
    public void UpdateRedPoint(EventMixMasterUpdateRedPoint evt)
    {
        UpdateViewState();
    }

    public void Init(StorageMixMaster storage)
    {
        Storage = storage;
        UpdateViewState();
    }
    public void UpdateViewState()
    {
        gameObject.SetActive(CanShow());
    }

    private bool CheckTask = true;
    private bool CheckFormula = true;

    public void IgnoreTask()
    {
        CheckTask = false;
    }
    public void IgnoreFormula()
    {
        CheckFormula = false;
    }
    public bool CanShow()
    {
        if (CheckTask)
        {
            if (Storage.CanCollectLevels.Count > 0)
                return true;   
        }
        if (CheckFormula)
        {
            var bagState = new Dictionary<int, int>();
            foreach (var bag in Storage.Bag)
            {
                bagState.TryAdd(bag.Key,0);
                bagState[bag.Key] += bag.Value;
            }
            foreach (var desktop in Storage.Desktop)
            {
                bagState.TryAdd(desktop.Value.Id,0);
                bagState[desktop.Value.Id] += desktop.Value.Count;
            }
            foreach (var pair in Storage.History)
            {
                var formula = MixMasterModel.Instance.FormulaConfig.Find(a => a.Id == pair.Key);
                if (MixMasterModel.Instance.CheckFormula(formula, bagState))
                {
                    return true;
                }
            }   
        }
        return false;
    }
}