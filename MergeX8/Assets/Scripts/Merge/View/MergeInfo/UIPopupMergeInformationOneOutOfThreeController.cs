using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Asset;
using System;
using System.Linq;
using DragonPlus;

public class UIPopupMergeInformationOneOutOfThreeController : UIWindow
{
    public class ChoiceBoxInformationMergeItem : MonoBehaviour
    {
        private Image Icon;
        private Button Btn;
        private TableMergeItem Config;

        private void Awake()
        {
            Icon = transform.Find("Icon").GetComponent<Image>();
            Btn = transform.Find("ButtonTip").GetComponent<Button>();
            Btn.onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(Config);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            });
        }

        public void Init(TableMergeItem config)
        {
            Config = config;
            Icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(Config.image);
        }
    }
    [NonSerialized] private TableMergeItem itemConfig;
    private Action CloseEvent;

    private List<ChoiceBoxInformationMergeItem> Items = new List<ChoiceBoxInformationMergeItem>();
    private Image Icon;
    private LocalizeTextMeshProUGUI NumText;
    private LocalizeTextMeshProUGUI titleText;
    private bool _isShowProbability;
    public override void PrivateAwake()
    {
        for (var i = 1; i <= 3; i++)
        {
            Items.Add(transform.Find("Root/ItemGroup/Item"+i).gameObject.AddComponent<ChoiceBoxInformationMergeItem>());
        }
        BindClick("Root/CloseButton", (go) => { ClickUIMask(); });
        Icon = transform.Find("Root/MergeInformationCell1/Icon").GetComponent<Image>();
        NumText = transform.Find("Root/MergeInformationCell1/Text").GetComponent<LocalizeTextMeshProUGUI>();
        NumText.gameObject.SetActive(false);
        titleText = transform.Find("Root/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, MergeInfoProductEvent);
        EventDispatcher.Instance.DispatchEvent(EventEnum.REWARD_POPUP);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, MergeInfoProductEvent);
    }

    public void SetInfomations(TableMergeItem config, Action close = null ,bool isShowProbability=false)
    {
        if (itemConfig == config)
        {
            CloseEvent = close;
            return;
        }

        _isShowProbability = isShowProbability;
        

        CloseEvent = close;
        this.itemConfig = config;

        InitTitleItem();
        InitItems();
    }

    public void InitTitleItem()
    {
        titleText.SetTerm(itemConfig.name_key);
        Icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
    }

    private void InitItems()
    {
        List<TableMergeItem> mergeItems = new List<TableMergeItem>();
        var choiceChest= GameConfigManager.Instance.GetChoiceChest(itemConfig.id);
        for (var i = 0; i < choiceChest.item.Length; i++)
        {
            mergeItems.Add(GameConfigManager.Instance.GetItemConfig(choiceChest.item[i]));
        }
        for (int i = 0; i < mergeItems.Count; i++)
        {
            Items[i].Init(mergeItems[i]);
        }
    }

    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        base.ClickUIMask();
        CloseEvent?.Invoke();
    }

    private void MergeInfoProductEvent(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length == 0)
            return;

        TableMergeItem config = (TableMergeItem) e.datas[0];
        if (config == itemConfig)
            return;

        itemConfig = config;

        InitTitleItem();
    }
}