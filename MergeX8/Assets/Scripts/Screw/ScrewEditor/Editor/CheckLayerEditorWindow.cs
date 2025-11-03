using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CheckLayerEditorWindow : EditorWindow
{
    string[] _layerNames = new string[]
    {
        "Item",
        "Room",
        "Floor",
        "RoomView",
        "SnapShot",
        "Post Process",
        "BehindItem",
        "Car",
        "Clickable",
        "Area",
        //"Grid",
        //"HightLight",
        //"GameBG",
        //"Clicker"
    };
    
    private List<int> _targetLayerName = new List<int>(); 
    
    [MenuItem("Tools/Screw/LayerCheck Window")]
    public static void ShowWindow()
    {
        // Opens the window, or brings it to focus if it's already open.
        GetWindow<CheckLayerEditorWindow>("LayerCheck Window");
    }

    void OnGUI()
    {
        GUILayout.Label("LayerCheck Window", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Check"))
        {
            CheckGameObjectsInLayer();
        }
    }

    private void CheckGameObjectsInLayer()
    {
        string[] ids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets" });
        for (int i = 0; i < ids.Length; i++)
        {
            bool found = false;
            string tempPath = AssetDatabase.GUIDToAssetPath(ids[i]);
            if(tempPath == null)
                continue;
            
          
                //根据路径获取Prefab(GameObject)
                GameObject tempObj = AssetDatabase.LoadAssetAtPath(tempPath, typeof(GameObject)) as GameObject;
                if (tempObj == null)
                    continue;
        
                if (CheckLayer(tempObj.transform))
                {
                    Debug.Log($"Object parent {tempObj.name} is on layer");
                    found = true;
                }
        
            if (found)
                return;
        }
    }

    private bool CheckLayer(Transform parent)
    {
        if (_targetLayerName.Count == 0)
        {
            foreach (var layerName in _layerNames)
            {
                _targetLayerName.Add( LayerMask.NameToLayer(layerName));
            }
        }
        if (_targetLayerName.Contains(parent.gameObject.layer))
        {
            Debug.Log($"Object {parent.name} is on layer");
            return true;
        }
        
        foreach (Transform child in parent.transform)
        {
            if (CheckLayer(child))
                return true;
        }
    
        return false;
    }
}