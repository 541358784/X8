using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class FogMap : MonoBehaviour
{
    [System.Serializable]
    public class FogBlocks
    {
        public GameObject _gameObject;
        public string _imageName;
    }

[System.Serializable]
    public class FogChunk
    {
        public GameObject _gameObject;
        public List<FogBlocks> _blocks;
    }
    
    [System.Serializable]
    public class FogLayer
    {
        public int _areaId;
        public GameObject _gameObject;
        
        [HideInInspector]
        public GameObject _nodeReference;

        //[HideInInspector]
        public List<FogChunk> _fogChunks = new List<FogChunk>();
    }
    
    
    public List<FogLayer> _fogLayers = new List<FogLayer>();

    private void Awake()
    {
        FixLayer();
    }

    private void FixLayer()
    {
        for (int i = 0; i < _fogLayers.Count; i++)
        {
            if(transform.Find(_fogLayers[i]._areaId.ToString()) != null)
                continue;
            
            _fogLayers.RemoveAt(i);
            i--;
        }
    }
}
