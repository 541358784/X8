using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


public class HospitalMapTools
{
    [MenuItem("GameObject/节点归零")]
    static void ResetZero()
    {
        var transforms = Selection.transforms;

        foreach (var t in transforms)
        {
            if (t.childCount > 0)
            {
                var child = t.GetChild(0);
                var pos = child.position;
                t.localPosition = Vector3.zero;
                child.position = pos;
            }
            
            EditorUtility.SetDirty(t.gameObject);
        }
    }


    [MenuItem("GameObject/数字命名")]
    static void Rename()
    {
        var transforms = Selection.transforms;

        var list = new List<Transform>(transforms);
        list.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));

        if (list.Count > 0)
        {
            if (int.TryParse(list[0].name, out var startIndex))
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var t = list[index];
                    if (index + startIndex < 10)
                        t.name = '0' + (index + startIndex).ToString();
                    else
                        t.name = (index + startIndex).ToString();
                }
            }
            
            EditorUtility.SetDirty(list[0].gameObject);
        }
    }

    [MenuItem("GameObject/前缀命名")]
    static void PreRename()
    {
        var transforms = Selection.transforms;

        var list = new List<Transform>(transforms);
        list.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));

        Debug.LogError(list.Count);

        if (list.Count > 0)
        {
            var prefix = list[0].name;
            for (var index = 1; index < list.Count; index++)
            {
                var t = list[index];
                t.name = $"{prefix}{index}";
            }
            
            EditorUtility.SetDirty(list[0].gameObject);
        }
    }

    [MenuItem("GameObject/数字命名倒叙")]
    static void Rename2()
    {
        var transforms = Selection.transforms;

        var list = new List<Transform>(transforms);
        list.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));

        if (list.Count > 0)
        {
            if (int.TryParse(list[0].name, out var startIndex))
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var t = list[index];
                    t.name = (startIndex - index).ToString();
                }
            }
            
            EditorUtility.SetDirty(list[0].gameObject);
        }
    }

    [MenuItem("GameObject/相同命名")]
    static void Rename3()
    {
        var transforms = Selection.transforms;

        var list = new List<Transform>(transforms);
        list.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));

        if (list.Count > 0)
        {
            if (int.TryParse(list[0].name, out var startIndex))
            {
                for (var index = 0; index < list.Count; index++)
                {
                    var t = list[index];
                    t.name = startIndex.ToString();
                }
            }
            
            EditorUtility.SetDirty(list[0].gameObject);
        }
    }

    private static List<Transform> _addParent_cache = new List<Transform>();

    [MenuItem("GameObject/包一层父节点")]
    static void AddParent()
    {
        var transforms = Selection.transforms;

        var list = new List<Transform>(transforms);
        list.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));

        if (list.Count > 0)
        {
            for (var index = 0; index < list.Count; index++)
            {
                var t = list[index];
                if (_addParent_cache.Contains(t)) continue;
                _addParent_cache.Add(t);

                var parentObj = new GameObject();
                parentObj.transform.SetParent(t.parent);
                parentObj.transform.localPosition = Vector3.zero;
                var pos = t.position;
                t.SetParent(parentObj.transform);
                t.position = pos;
                t.name = "01";
                parentObj.name = index.ToString();
            }
            
            EditorUtility.SetDirty(list[0].gameObject);
        }


        ClearCache();
    }

    private static async void ClearCache()
    {
        await Task.Delay(1000);
        
        _addParent_cache.Clear();
    }
}