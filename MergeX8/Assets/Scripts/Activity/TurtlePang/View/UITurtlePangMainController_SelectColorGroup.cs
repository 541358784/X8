using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangMainController
{
    public Transform SelectColorGroup;
    public List<SelectColorBtn> SelectColorBtnList = new List<SelectColorBtn>();
    public Button SelectColorSureBtn;
    public SelectColorBtn CurSelectColorBtn;
    public TaskCompletionSource<TurtlePangItemConfig> SelectColorTask;
    public void InitSelectColorGroup()
    {
        SelectColorGroup = GetItem<Transform>("SelectColour");
        var temp = SelectColorGroup.gameObject.AddComponent<Canvas>();
        temp.overrideSorting = true;
        temp.sortingOrder = canvas.sortingOrder + 4;
        SelectColorGroup.gameObject.AddComponent<GraphicRaycaster>();
        SelectColorSureBtn = GetItem<Button>("SelectColour/Root/Button");
        SelectColorSureBtn.onClick.AddListener(() =>
        {
            if (CurSelectColorBtn == null)
                return;
            SelectColorTask.SetResult(CurSelectColorBtn.Config);
        });
        foreach (var itemConfig in Model.ItemConfig)
        {
            var node = transform.Find("SelectColour/Root/Content/" + itemConfig.Id);
            if (node)
            {
                var selectBtn = node.gameObject.AddComponent<SelectColorBtn>();
                selectBtn.Init(itemConfig, OnClickSelectBtn);
                SelectColorBtnList.Add(selectBtn);
            }
        }
        SelectColorGroup.gameObject.SetActive(false);
    }

    public async Task SelectColor()
    {
        if (Storage.LuckyColor > 0)
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TurtlePangColor))
        {
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TurtlePangColor, null))
            {
                
            }
        }
        IsPerforming = true;
        SelectColorTask = new TaskCompletionSource<TurtlePangItemConfig>();
        SelectColorGroup.gameObject.SetActive(true);
        SelectColorGroup.GetComponent<Canvas>().overrideSorting = true;
        LuckyColorIcon.gameObject.SetActive(false);
        foreach (var selectBtn in SelectColorBtnList)
        {
            selectBtn.SelectEffect.gameObject.SetActive(false);
        }
        CurSelectColorBtn = null;
        SelectColorSureBtn.interactable = CurSelectColorBtn;
        var result = await SelectColorTask.Task;
        Storage.LuckyColor = result.Id;
        SelectColorGroup.gameObject.SetActive(false);
        IsPerforming = false;
        var luckyColor = Model.ItemConfig.Find(a => a.Id == Storage.LuckyColor);
        LuckyColorIcon.sprite = luckyColor.GetTurtleIcon();
        LuckyColorIcon.gameObject.SetActive(true);
        //刷新界面
    }

    public void OnClickSelectBtn(SelectColorBtn selectBtn)
    {
        if (CurSelectColorBtn != selectBtn)
        {
            if (CurSelectColorBtn)
                CurSelectColorBtn.SelectEffect.gameObject.SetActive(false);
            CurSelectColorBtn = selectBtn;
            if (CurSelectColorBtn)
                CurSelectColorBtn.SelectEffect.gameObject.SetActive(true);
            LuckyColorIcon.sprite = CurSelectColorBtn.Config.GetTurtleIcon();
            LuckyColorIcon.gameObject.SetActive(true);
        }
        else
        {
            if (CurSelectColorBtn)
                CurSelectColorBtn.SelectEffect.gameObject.SetActive(false);
            CurSelectColorBtn = null;
            LuckyColorIcon.gameObject.SetActive(false);
        }
        SelectColorSureBtn.interactable = CurSelectColorBtn;
    }
    public class SelectColorBtn : MonoBehaviour
    {
        public Button Btn;
        public Image Icon;
        public TurtlePangItemConfig Config;
        public Action<SelectColorBtn> Callback;
        public Transform SelectEffect;
        
        public void Init(TurtlePangItemConfig config,Action<SelectColorBtn> callback)
        {
            Config = config;
            Callback = callback;
            Btn = transform.GetComponent<Button>();
            Btn.onClick.AddListener(()=>Callback(this));
            Icon = transform.Find("Image").GetComponent<Image>();
            Icon.sprite = Config.GetTurtleIcon();
            SelectEffect = transform.Find("Selected");
            SelectEffect.gameObject.SetActive(false);
        }
    }
}
