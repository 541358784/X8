using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DragonU3DSDK.Asset;
using DragonPlus;

public class MapDownloadMgr : Manager<MapDownloadMgr>
{
    private RectTransform ContentTr;
    private Dictionary<string, MapDownloadItem> DownloadTask = new Dictionary<string, MapDownloadItem>();

    private void Awake()
    {
        Transform tsks = this.transform.Find("Tasks");
        if (tsks == null)
            return;

        ContentTr = tsks.GetComponent<RectTransform>();
    }

    public bool HaveTask()
    {
        return DownloadTask.Count > 0;
    }

    public void AddTask(string tag, List<DownloadInfo> tasks, Action<float> onprogress)
    {
        if (DownloadTask.ContainsKey(tag)) return;

        var item = new GameObject(tag);
        item.transform.parent = ContentTr;
        var script = item.AddComponent<MapDownloadItem>();
        script.AttachTask(tag, tasks, onprogress);
        DownloadTask.Add(tag, script);
    }

    public void RemoveTask(string tag)
    {
        MapDownloadItem item = null;
        foreach (var kv in DownloadTask)
        {
            if (kv.Key.Equals(tag))
            {
                item = kv.Value;
                break;
            }
        }

        if (item != null)
        {
            DownloadTask.Remove(tag);
            item.transform.parent = null;
            Destroy(item.gameObject);
        }
    }
}