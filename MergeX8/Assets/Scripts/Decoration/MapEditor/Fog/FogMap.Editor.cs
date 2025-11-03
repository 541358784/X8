using System.Collections.Generic;
using UnityEngine;

public partial class FogMap : MonoBehaviour
{
    public FogLayer CreateFogLayer(int areaId)
    {
        var layer = _fogLayers.Find(a => a._areaId == areaId);
        if (layer != null)
            return layer;

        
        var item = new GameObject(areaId.ToString());
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;

        layer = new FogLayer();
        layer._gameObject = item;
        layer._areaId = areaId;
        _fogLayers.Add(layer);

        return layer;
    }

    public FogChunk CreateFogChunk(int areaId)
    {
        var layer = _fogLayers.Find(a => a._areaId == areaId);
        if (layer == null)
            layer = CreateFogLayer(areaId);
        
        var item = new GameObject("ck_" + layer._fogChunks.Count);
        item.transform.SetParent(layer._gameObject.transform);
        item.transform.localPosition = Vector3.zero;
        
        var chunk = new FogChunk();
        chunk._gameObject = item;
        layer._fogChunks.Add(chunk);

        {
            var render = new GameObject("sp");
            render.transform.SetParent(item.transform);
            render.transform.localPosition = Vector3.zero;
            var spRender = render.AddComponent<SpriteRenderer>();
        }
        return chunk;
    }
    
    public bool DeleteFogLayer(int areaId)
    {
        var layer = _fogLayers.Find(a => a._areaId == areaId);
        if (layer == null)
            return false;
            
        _fogLayers.Remove(layer);
        return true;
    }

    public void ShowFogLayer(int areaId, bool isShow)
    {
        var layer = _fogLayers.Find(a => a._areaId == areaId);
        if (layer == null)
            return;
        
        layer._gameObject.SetActive(isShow);
    }

    public bool IsShow(int areaId)
    {
        var layer = _fogLayers.Find(a => a._areaId == areaId);
        if (layer == null)
            return false;

        return layer._gameObject.activeSelf;
    }

    public void Save()
    {
        FixLayer();
        
        _fogLayers.ForEach(a =>
        {
            a._fogChunks.Clear();

            foreach (Transform child in a._gameObject.transform)
            {
                var chunk = new FogChunk();
                chunk._gameObject = child.gameObject;
                chunk._blocks = new List<FogBlocks>();

                foreach (Transform subChild in child.transform.transform)
                {
                    var sp = subChild.GetComponent<SpriteRenderer>();
                    if(sp == null || sp.sprite == null)
                        continue;

                    var block = new FogBlocks();
                    block._gameObject = subChild.gameObject;
                    block._imageName = sp.sprite.name;
                    
                    DestroyImmediate(sp);
                    
                    chunk._blocks.Add(block);
                }
                
                a._fogChunks.Add(chunk);
            }
        });
    }
}