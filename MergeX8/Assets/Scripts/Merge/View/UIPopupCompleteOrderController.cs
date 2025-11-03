using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupCompleteOrderController:UIWindowController
{
    private Transform DefaultItem;
    private Button CancelButton;
    private Button OKButton;
    private Button CloseButton;
    public override void PrivateAwake()
    {
        DefaultItem = transform.Find("Root/Content/Item");
        DefaultItem.gameObject.SetActive(false);
        CancelButton = transform.Find("Root/ButtonGroup/ButtonBreak").GetComponent<Button>();
        CancelButton.onClick.AddListener(OnClickCloseButton);
        CloseButton = transform.Find("Root/BG/ButtonClose").GetComponent<Button>();
        CloseButton.onClick.AddListener(OnClickCloseButton);
        OKButton = transform.Find("Root/ButtonGroup/ButtonCancel").GetComponent<Button>();
        OKButton.onClick.AddListener(OnClickOKButton);
    }

    private Action Callback;
    List<int> Items;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Items = objs[0] as List<int>;
        Callback = objs[1] as Action;
        if (Items != null)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var itemObj = Instantiate(DefaultItem, DefaultItem.parent);
                itemObj.gameObject.SetActive(true);
                var itemConfig= GameConfigManager.Instance.GetItemConfig(Items[i]);
                if(itemConfig != null)
                    itemObj.Find("Icon").GetComponent<Image>().sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }   
        }
    }

    public void OnClickCloseButton()
    {
        AnimCloseWindow();
    }
    public void OnClickOKButton()
    {
        AnimCloseWindow(() =>
        {
            Callback();
        });
    }
    public static void ShowEnsurePopup(List<int> items,Action callback)
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupCompleteOrder, items,callback);
    }
}