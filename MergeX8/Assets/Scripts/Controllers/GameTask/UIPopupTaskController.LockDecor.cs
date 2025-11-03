using System.Collections.Generic;
using Decoration;
using DragonU3DSDK.Asset;
using UnityEngine;


public partial class UIPopupTaskController : UIWindowController
{
    private GameObject _lockItem;
    private List<TaskLockItem> _taskLockCells = new List<TaskLockItem>();
    private void Awake_Lock()
    {
        _lockItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Home/UITaskGreyCell");
    }
    
    private void InitLockView()
    {
        var area = DecoManager.Instance.GetLockArea();
        if(area == null)
            return;
        
        var obj = GameObject.Instantiate(_lockItem, _content.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
            
        _taskLockCells.Add(obj.AddComponent<TaskLockItem>());
            
        _taskLockCells[_taskLockCells.Count-1].InitData(area, DecoManager.Instance.GetFirstNode(area));
    }
}