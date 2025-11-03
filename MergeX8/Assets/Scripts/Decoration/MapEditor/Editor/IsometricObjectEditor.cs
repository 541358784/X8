using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(IsometricObject))]
public class IsometricObjectEditor : Editor
{
    public void OnSceneGUI()
    {
        Handles.color = Color.black;
        var isometric = target as IsometricObject;
        var basePos = new Vector3(isometric.transform.position.x,
            isometric.transform.position.y - isometric._spriteLowerBound + isometric._floorHeight,
            isometric.transform.position.z);

        var newPos = Handles.Slider(basePos, Vector3.up);
        if (basePos != newPos)
        {
            Undo.RecordObject(isometric, "IsometricObject");
            var delta = newPos - basePos;
            isometric._floorHeight = isometric._floorHeight + delta.y;
            EditorUtility.SetDirty(target);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        if (GUILayout.Button("Reset Floor Height To: 0"))
        {
            var obj = target as IsometricObject;
            obj._floorHeight = 0;

            EditorUtility.SetDirty(target);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("自动排列子物体Z轴"))
        {
            var obj = target as IsometricObject;

            var delta = -0.0001f;
            var index = 1;
            foreach (Transform child in obj.transform)
            {
                child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, delta * index++);
            }

            EditorUtility.SetDirty(target);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("重新计算z值"))
        {
            var isometric = target as IsometricObject;
            isometric.CaculatePosition();
        }
    }
}