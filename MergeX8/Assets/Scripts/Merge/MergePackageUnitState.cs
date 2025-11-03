using UnityEngine;
using UnityEngine.UI;
using System;
using DragonPlus;
using Gameplay;

public class MergePackageUnitState : MonoBehaviour
{
    private Image icon
    {
        get { return this.transform.Find("Icon")?.GetComponent<Image>(); }
    }

    private Button tipsBtn;
    private Button tipBtn;

    private Transform tagText,
        taskBg,
        taskBgNone,
        nameText,
        taskText,
        levelText,
        tipTran,
        arrow,
        arrowGray,
        iconNone,
        numText,
        currency,
        tip,
        levelBGNone,
        selectedBG;

    private TableMergeItem itemConfig;
    private MergePackageUnitType mergePackageUnitType;
    private int parentItemId = 0; // 父的itemid

    private void Awake()
    {
        tipTran = this.transform.Find("TipsBtn");
        if (tipTran != null)
            tipsBtn = tipTran.GetComponent<Button>();
        if (tipsBtn != null)
            tipsBtn.onClick.AddListener(OnClickTips);
        tip = this.transform.Find("Tip");
        if (tip != null)
            tipBtn = tip.GetComponent<Button>();

        taskBg = this.transform.Find("TaskBG");
        taskBgNone = this.transform.Find("TaskBGNone");

        nameText = this.transform.Find("NameText");
        taskText = this.transform.Find("TaskText");
        levelText = this.transform.Find("LevelText");
        arrow = this.transform.Find("Arrow");
        arrow?.gameObject.SetActive(false);

        arrowGray = this.transform.Find("ArrowGray");
        arrowGray?.gameObject.SetActive(true);

        iconNone = this.transform.Find("IconNone");
        numText = this.transform.Find("NumText");
        currency = this.transform.Find("Currency");
        levelBGNone = this.transform.Find("LevelBGNone");

        selectedBG = transform.Find("SelectedBG");
        tagText = transform.Find("TagGroup/Text");
    }

    public void SetItemStatus(int itemId, MergePackageUnitType type)
    {
        mergePackageUnitType = type;
        switch (type)
        {
            case MergePackageUnitType.notMergeIcon:
                levelText?.gameObject.SetActive(true);
                nameText?.gameObject.SetActive(false);
                tipTran?.gameObject.SetActive(false);
                SetNotMergeIcon(itemId);
                break;
        }
    }

    private void SetNotMergeIcon(int itemId)
    {
        //Items itemCfg = HotelGameConfigManager.Instance.ItemsList.Find(x => x.Id == itemId);
        //icon.sprite =  MergeConfigManager.Instance.mergeIcon.GetSprite(itemCfg.ImageSmall);
    }

    public void SetItemStatus(TableMergeItem itemConfig, int choosedIndex, MergePackageUnitType type,
        int parentItemId = 0, int count = 0)
    {
        mergePackageUnitType = type;
        this.parentItemId = parentItemId;
        this.itemConfig = itemConfig;
        if (selectedBG != null)
            selectedBG.gameObject.SetActive(false);
        if (icon != null)
            if (icon.enabled == false)
                icon.enabled = true;
        switch (type)
        {
            case MergePackageUnitType.bag:
            case MergePackageUnitType.buildBag:
            case MergePackageUnitType.bigInfo:
                nameText.GetComponent<LocalizeTextMeshProUGUI>().SetTerm(itemConfig.name_key);
                levelText.GetComponent<LocalizeTextMeshProUGUI>().SetText(
                    LocalizationManager.Instance.GetLocalizedString("UI_choose_progress_level_text1") + " " +
                    itemConfig.level);
                taskBg.gameObject.SetActive(false);
                taskBgNone.gameObject.SetActive(false);
                icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                break;
            case MergePackageUnitType.bagUnlock:
            case MergePackageUnitType.buildBagUnlock:

                break;
            case MergePackageUnitType.baglock:
            case MergePackageUnitType.buildBaglock:
                currency.gameObject.SetActive(true);
                break;
            case MergePackageUnitType.bagMax:
            case MergePackageUnitType.buildBagMax:
                currency.gameObject.SetActive(false);
                break;
            case MergePackageUnitType.info:
                selectedBG.gameObject.SetActive(itemConfig.id == parentItemId);
                SetInfoIconImage();
                levelText.GetComponent<LocalizeTextMeshProUGUI>().SetTerm(itemConfig.level.ToString());
                break;
            case MergePackageUnitType.productExplain:
            {
                bool isParent = itemConfig.id == parentItemId;
                selectedBG.gameObject.SetActive(isParent);
                SetInfoExplainIconImage();
                break;
            }
            case MergePackageUnitType.buildBundle:
            case MergePackageUnitType.taskRewards:
                SetInfoIconImage();
                nameText.GetComponent<LocalizeTextMeshProUGUI>().SetTerm(itemConfig.name_key);
                levelText.gameObject.SetActive(false);
                // levelText.GetComponent<LocalizeTextMeshProUGUI>().SetText(
                //     LocalizationManager.Instance.GetLocalizedString("UI_choose_progress_level_text1") + " " +
                //     itemConfig.level.ToString());
                // if (count > 1)
                // {
                //     
                // }
                tagText.parent.gameObject.SetActive(true);
                tagText.GetComponent<LocalizeTextMeshProUGUI>().SetText("x" + count);
                nameText.gameObject.SetActive(false);
                tipTran.gameObject.SetActive(true);
                break;
            case MergePackageUnitType.taskXpRewards:
                nameText.GetComponent<LocalizeTextMeshProUGUI>().SetTerm(itemConfig.name_key);
                levelText.gameObject.SetActive(true);
                nameText.gameObject.SetActive(false);
                tipTran.gameObject.SetActive(false);
                break;
            case MergePackageUnitType.infoTips:
                nameText.GetComponent<LocalizeTextMeshProUGUI>().SetTerm(itemConfig.name_key);
                nameText.gameObject.SetActive(false);
                tipTran.gameObject.SetActive(true);
                break;
            case MergePackageUnitType.product:
            {
                selectedBG.gameObject.SetActive(true);
                tipTran.gameObject.SetActive(true);
                break;
            }
            case MergePackageUnitType.splite:
                SetInfoIconImage();
                levelText.gameObject.SetActive(true);
                levelText.GetComponent<LocalizeTextMeshProUGUI>().SetText(
                    LocalizationManager.Instance.GetLocalizedString("UI_choose_progress_level_text1") + " " +
                    itemConfig.level.ToString());
                nameText.GetComponent<LocalizeTextMeshProUGUI>().SetTerm(itemConfig.name_key);
                nameText.gameObject.SetActive(false);
                taskText.gameObject.SetActive(false);
                taskBg.gameObject.SetActive(false);
                taskBgNone.gameObject.SetActive(false);
                tipTran.gameObject.SetActive(false);
                break;
            case MergePackageUnitType.increasItem:
                SetInfoIconImage(true);
                levelText.gameObject.SetActive(true);
                levelText.GetComponent<LocalizeTextMeshProUGUI>().SetText(
                    LocalizationManager.Instance.GetLocalizedString("UI_choose_progress_level_text1") + " " +
                    itemConfig.level.ToString());
                nameText.GetComponent<LocalizeTextMeshProUGUI>().SetTerm(itemConfig.name_key);
                nameText.gameObject.SetActive(false);
                taskText.gameObject.SetActive(false);
                taskBg.gameObject.SetActive(false);
                taskBgNone.gameObject.SetActive(false);
                tipTran.gameObject.SetActive(false);
                break;
            case MergePackageUnitType.warning:
                SetInfoIconImage();
                break;
        }

        if (itemConfig == null)
        {
            return;
        }

        if (type != MergePackageUnitType.info)
            if (icon != null)
                icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
    }

    public void SetNameText(string txt)
    {
        nameText.GetComponent<LocalizeTextMeshProUGUI>().SetText(txt);
    }

    public void SetLevelText(string txt)
    {
        levelText.GetComponent<LocalizeTextMeshProUGUI>().SetText(txt);
    }

    public void SetBagCost(int type, int count)
    {
        if (count != 0)
        {
            currency.GetComponent<Image>().sprite = UserData.GetResourceIcon(type);
            numText.GetComponent<LocalizeTextMeshProUGUI>().SetText(count.ToString());
        }
        else
            numText.GetComponent<LocalizeTextMeshProUGUI>().SetText("Max");

        tipBtn.interactable = UserData.Instance.CanAford((UserData.ResourceId)type, count);
    }

    private void SetInfoIconImage(bool forceShowIcon = false) //是否可以显示icon
    {
        if (mergePackageUnitType == MergePackageUnitType.taskRewards)
            return;
        bool isShowedProduct = MergeManager.Instance.IsShowedItemId(itemConfig.id);
        if (levelBGNone != null)
            levelBGNone.gameObject.SetActive(!isShowedProduct);
        if (isShowedProduct || itemConfig.id == parentItemId || forceShowIcon)
        {
            icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            icon.gameObject.SetActive(true);
            if (iconNone != null)
                iconNone.gameObject.SetActive(false);
        }
        else
        {
            icon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite("question_mark");
            icon.gameObject.SetActive(false);
            if (iconNone != null)
                iconNone.gameObject.SetActive(true);
            icon.SetNativeSize();
        }

        if (itemConfig.next_level == -1)
        {
            if (arrow != null)
                arrow.gameObject.SetActive(false);
            if (arrowGray != null)
                arrowGray.gameObject.SetActive(false);
        }
    }

    private void SetInfoExplainIconImage()
    {
        if (itemConfig.next_level == -1)
        {
            if (arrow != null)
                arrow.gameObject.SetActive(false);
            if (arrowGray != null)
                arrowGray.gameObject.SetActive(false);
        }
    }

    private void OnClickTips()
    {
        switch (mergePackageUnitType)
        {
            case MergePackageUnitType.infoTips:
            case MergePackageUnitType.product:
            {
                if (!GuideSubSystem.Instance.IsShowingGuide())
                    MergeInfoView.Instance.OpenMergeInfo(itemConfig);
                break;
            }
            case MergePackageUnitType.vipBag:
            case MergePackageUnitType.bag:
            {
                UIManager.Instance.CloseUI(UINameConst.UIPopupMergePackage, true);
                MergeInfoView.Instance.OpenMergeInfo(itemConfig,
                    () => { UIManager.Instance.OpenUI(UINameConst.UIPopupMergePackage); });
                break;
            }
            case MergePackageUnitType.buildBag:
            {
                UIManager.Instance.CloseUI(UINameConst.UIPopupMergePackage, true);
                MergeInfoView.Instance.OpenMergeInfo(itemConfig,
                    () =>
                    {
                        UIManager.Instance.OpenUI(UINameConst.UIPopupMergePackage,BagType.Building);
                    });
                break;
            }
            case MergePackageUnitType.warning:
            case MergePackageUnitType.taskRewards:
            {
                MergeInfoView.Instance.OpenMergeInfo(itemConfig, () => { },_isShowProbability:true);
                break;
            }
            case MergePackageUnitType.productExplain:
            {
                EventDispatcher.Instance.DispatchEvent(MergeEvent.MERGE_INFO_PRODUCT_EXPLAIN, itemConfig);
                break;
            }
        }
        // else if(mergePackageUnitType == MergePackageUnitType.buildBundle)
        // {
        //     MergeInfoView.Instance.OpenMergeInfo(itemConfig,()=>
        //     {
        //         UIManager.Instance.OpenUI("MergePurchaseController");
        //     });
        // }
    }

    public void SetTaskStatus(int curCount, int needCount)
    {
        var text = taskText.GetComponent<LocalizeTextMeshProUGUI>();
        text.SetText(string.Format("{0}/{1}", curCount, needCount));
        bool isFinish = curCount >= needCount;
        if (isFinish)
        {
            taskBg.gameObject.SetActive(true);
            taskBgNone.gameObject.SetActive(false);
            // text.SetColor(Color.green);
        }
        else
        {
            taskBg.gameObject.SetActive(false);
            taskBgNone.gameObject.SetActive(true);
            // text.SetColor(Color.white);
        }
    }

    public void SetTaskText(string msg)
    {
        taskBg.gameObject.SetActive(true);
        taskBgNone.gameObject.SetActive(false);
        var text = taskText.GetComponent<LocalizeTextMeshProUGUI>();
        text.SetText(msg);
        levelText?.gameObject.SetActive(false);
    }

    public void SetGalleryAwards()
    {
        if (itemConfig == null)
            return;
        Image image = tipsBtn.transform.GetComponentDefault<Image>("Diamonds");
        if (itemConfig.gallery_award == null || MergeManager.Instance.IsItemGetGalleryAwards(itemConfig.id) ||
            !MergeManager.Instance.IsShowedItemId(itemConfig.id))
        {
            tipsBtn.gameObject.SetActive(false);
            return;
        }

        if (itemConfig.gallery_award.Length != 2)
        {
            throw new Exception("当前图鉴奖励配置出错:" + itemConfig.id);
        }

        tipsBtn.gameObject.SetActive(true);
        // Items itemCfg = HotelGameConfigManager.Instance.ItemsList.Find(x => x.Id == itemConfig.Gallery_award[0]);
        // image.sprite =  MergeConfigManager.Instance.mergeIcon.GetSprite(itemCfg.ImageSmall);
    }

    public void GetGalleryAwards()
    {
        tipsBtn.gameObject.SetActive(false);
    }

    public void UpdateSelectedBg(TableMergeItem config)
    {
        selectedBG.gameObject.SetActive(config == itemConfig);
    }
}