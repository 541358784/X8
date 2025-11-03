using System;
using System.Collections.Generic;
using UnityEngine;

public class VisibleOppositeController:MonoBehaviour
{
    public List<GameObject> ControlNodeList;
    
    private void OnEnable()
    {
        foreach (var node in ControlNodeList)
        {
            node.gameObject.SetActive(!gameObject.activeSelf);
        }
    }
    
    private void OnDisable()
    {
        foreach (var node in ControlNodeList)
        {
            node.gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}