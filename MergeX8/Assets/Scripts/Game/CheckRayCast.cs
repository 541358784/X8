/****************************************************
    文件：CheckRayCast.cs
    作者：Cys
    邮箱: yongsheng.chen@dragonplus.com
    日期： 2022-01-11-11:05:09
    功能：....
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckRayCast : MonoBehaviour, IPointerClickHandler
{
    public Action OnClickAction;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickAction?.Invoke();
    }
}