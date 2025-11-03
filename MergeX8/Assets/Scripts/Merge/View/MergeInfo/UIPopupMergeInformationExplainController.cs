using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Asset;
using System;
using System.Linq;
using DragonPlus;

public class UIPopupMergeInformationExplainController : UIWindow
{
    private GameObject packageItem;
    [NonSerialized] private TableMergeItem itemConfig;
    private Action CloseEvent;

    private Transform contnet;
    private Transform reContnet;

    private GameObject reGameItem;

    private LocalizeTextMeshProUGUI titleText;
    private LocalizeTextMeshProUGUI topText;
    private LocalizeTextMeshProUGUI topNumText;
    private Image topImage;
    private bool _isShowProbability;
    public override void PrivateAwake()
    {
        contnet = transform.Find("Root/TopGroup/Content");
        reContnet = transform.Find("Root/Scroll View/Viewport/Content");

        BindClick("Root/CloseButton", (go) => { ClickUIMask(); });

        titleText = transform.Find("Root/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
        topText = transform.Find("Root/TopGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        topImage = transform.Find("Root/TopGroup/Image").GetComponent<Image>();
        topNumText = transform.Find("Root/TopGroup/Num").GetComponent<LocalizeTextMeshProUGUI>();
        packageItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergeInformationExplainCell");
        reGameItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/Item4");

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

        _isShowProbability = true;
        CommonUtils.DestroyAllChildren(contnet);
        CommonUtils.DestroyAllChildren(reContnet);

        CloseEvent = close;
        this.itemConfig = config;

        InitTitleItem();
        InitItems();
        InitProduct(GameConfigManager.Instance.GetMergeLine(itemConfig.in_line));
    }

    public void InitTitleItem()
    {
        titleText.SetTerm(itemConfig.name_key);
        // topNumText.gameObject.SetActive(_isShowProbability);
        // topText.gameObject.SetActive(_isShowProbability);
        // topImage.gameObject.SetActive(_isShowProbability);
        topNumText.SetText(itemConfig.output_amount.ToString());
    }

    private void InitItems()
    {
        List<TableMergeItem> mergeItems = null;

        if (itemConfig.in_line <= 0) //固定奖励 或者没有合成链的
        {
            mergeItems = new List<TableMergeItem>();
            mergeItems.Add(itemConfig);
        }
        else
        {
            mergeItems = GameConfigManager.Instance.GetMergeInLineItems(itemConfig.in_line);
        }

        if (mergeItems == null)
            return;

        for (int i = 0; i < mergeItems.Count; i++)
        {
            var config = mergeItems[i];
            if (config == null)
                continue;

            var item = Instantiate(packageItem, contnet);
            item.SetActive(true);

            var script = item.gameObject.GetComponentDefault<MergeInfoExplainItem>();
            script.SetItemInfomation(config, itemConfig);
        }
    }

    private void InitProduct(TableMergeLine mergeLine)
    {
        int[] outPut;
        if (MergeManager.Instance.IsBox(itemConfig) || itemConfig.in_line < 0) //固定产出
        {
            outPut = itemConfig.output;
        }
        else
        {
            outPut = mergeLine.output;
        }

        if (outPut == null)
            return;

        Dictionary<int, int> probability = new Dictionary<int, int>();
        if (itemConfig.output_probability != null)
        {
            for (int i = 0; i < outPut.Length; i++)
            {
                if(i >= itemConfig.output_probability.Length)
                    continue;
                
                probability.Add(outPut[i], itemConfig.output_probability[i]);
            }
        }
        
        Array.Sort(outPut, (x, y) => x - y);

        for (int i = 0; i < outPut.Length; i++)
        {
            var config = GameConfigManager.Instance.GetItemConfig(outPut[i]);
            if (config == null)
                continue;

            var item = Instantiate(reGameItem, reContnet);
            item.SetActive(true);
            var image = item.transform.Find("Icon").GetComponent<Image>();
            image.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(config.image);
            var text = item.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();

            bool haveProbability = probability.ContainsKey(outPut[i]);
            if(!_isShowProbability || !haveProbability)
                text.gameObject.SetActive(false);
            else{
                text.SetText(probability[outPut[i]]+"%");
            }
    
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
        CommonUtils.DestroyAllChildren(reContnet);

        InitTitleItem();
        InitProduct(GameConfigManager.Instance.GetMergeLine(config.in_line));
    }
}