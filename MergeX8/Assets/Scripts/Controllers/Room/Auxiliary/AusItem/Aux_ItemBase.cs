using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public class Aux_ItemBase : MonoBehaviour
{
    protected Button _button;
    public UIHomeMainController mainController;
    public static Dictionary<Type,Aux_ItemBase> AuxItemDic = new Dictionary<Type, Aux_ItemBase>();
    protected virtual void Awake()
    {
        _button = transform.GetComponent<Button>();
        _button?.onClick.AddListener(OnButtonClick);
        AuxItemDic[GetType()] = this;
    }

    private void OnDestroy()
    {
        AuxItemDic.Remove(GetType());
    }

    public static Aux_ItemBase GetInstance<T>()where T:Aux_ItemBase
    {
        if (AuxItemDic.TryGetValue(typeof(T),out var auxItem))
            return auxItem;
        return null;
    }

    public virtual void Init(UIHomeMainController mainController)
    {
        this.mainController = mainController;
    }

    public virtual void UpdateUI()
    {
    }

    protected virtual void OnButtonClick()
    {
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
    }
    // public void SetItemAsFirstSibling()
    // {
    //     if (this && transform)
    //         transform.SetAsFirstSibling();
    // }
}