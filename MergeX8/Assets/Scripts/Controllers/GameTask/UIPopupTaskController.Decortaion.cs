using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Asset;
using UnityEngine;


public partial class UIPopupTaskController : UIWindowController
{
    private GameObject _decoItem;
    private List<TaskDecorItem> _taskDecoCells = new List<TaskDecorItem>();
    public List<TaskDecorItem> TaskDecorItems => _taskDecoCells;
    private void Awake_Deco()
    {
        _decoItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Home/UITaskCell");
    }
    
    private void InitDecoView()
    {
        var areaNodes = DecoManager.Instance.GetDecoWorld(1).GetUnlockAndNotOwnedNodes();
        foreach (var area in areaNodes)
        {
            if(area.Value == null || area.Value.Count == 0)
                continue;
            
            var obj = GameObject.Instantiate(_decoItem, _content.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            
            _taskDecoCells.Add(obj.AddComponent<TaskDecorItem>());
            
            _taskDecoCells[_taskDecoCells.Count-1].InitData(area.Key, area.Value);
        }
        
        _taskDecoCells.ForEach(a =>
        {
            if(!a.IsCoinNode())
                a.transform.SetAsLastSibling();
        });
    }

    private void GetDecorationReward(BaseEvent e)
    {
        var cell = _taskDecoCells.Find(a => a == e.datas[0]);
        if(cell == null)
            return;

        _taskDecoCells.Remove(cell);
        GameObject.DestroyImmediate(cell.gameObject);
        
        _comingSoon.gameObject.SetActive(_taskDecoCells.Count == 0 && _taskLockCells.Count == 0);
    }
}