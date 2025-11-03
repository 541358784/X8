using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SomeWhereTileMatch;
using UnityEngine;
using UnityEngine.UI;

public class PathMoveItem 
{
    private int moveIndex;
    public GameObject moveItem;
    private PathMap pathMap;
    private int initMoveIndex;

    public bool isUpdate = true;

    private Image uiImage = null;

    private bool isHide = false;
    public void Init(Transform parent, int index, PathMap pathMap)
    {
        if (index >= pathMap.cuttingPointList.Count)
            index = index - pathMap.cuttingPointList.Count;
        
        initMoveIndex = moveIndex = index;
        this.pathMap = pathMap;

        moveItem = new GameObject(index.ToString());
        moveItem.transform.SetParent(parent);
        moveItem.transform.localScale = Vector3.one;
        moveItem.transform.position = pathMap.GetCuttingPoint(index);
    }

    public void Update()
    {
        if(!isUpdate)
            return;

        moveIndex++;
        moveIndex = moveIndex >= pathMap.cuttingPointList.Count ? 0 : moveIndex;
        moveItem.transform.position = pathMap.GetCuttingPoint(moveIndex);
    }

    public void Rest()
    {
        if(moveItem == null)
            return;
        
        moveIndex = initMoveIndex;
        moveItem.transform.localScale = Vector3.one;
        moveItem.transform.position = pathMap.GetCuttingPoint(moveIndex);
    }

    public void AddChild(GameObject childObj, float scale)
    {
        if(childObj == null)
            return;
        
        childObj.transform.SetParent(moveItem.transform);
        childObj.transform.localPosition = Vector3.zero;
        childObj.transform.localScale = new Vector3(scale, scale, scale);

        uiImage = childObj.transform.Find("Card").GetComponent<Image>();
        uiImage.color = Color.white;
    }

    public void InitImage(GameObject imageObj)
    {
        if(imageObj == null)
            return;
        
        uiImage = imageObj.transform.Find("Card").GetComponent<Image>();
        uiImage.color = Color.white;
    }
    
    public void Hide(float time)
    {
        if(isHide)
            return;

        isHide = true;
        uiImage.DOFade(0, time);
        
        if (!Application.isPlaying)
            uiImage.color = new Color(uiImage.color.r, uiImage.color.g, uiImage.color.b, 0);
    }
}
