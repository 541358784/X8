using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Merge.Order;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreTaskAssistBundleItem : MonoBehaviour
{
    public UIStoreItemType ItemType { get; } = UIStoreItemType.Bundle;

    protected Button BuyButton
    {
        get { return _btnBuy; }
    }

    private TableShop _cfg;

    public TableShop Cfg
    {
        get { return _cfg; }
    }

    private ShowArea _showArea;

    public ShowArea showArea
    {
        get { return _showArea; }
    }

    private string _openSrc;

    public string OpenSrc
    {
        get { return openSrc; }
    }

    public string openSrc;

    private TaskAssist _taskAssist;
    private Button _btnBuy;

    private Transform _rewardItem;
    private Transform _rewardParent;
    private Text _priceText;

    private LocalizeTextMeshProUGUI _contentText;

    private Transform _tagGroup;
    private LocalizeTextMeshProUGUI _tagText;
    private LocalizeTextMeshProUGUI _timeText;
    private StorageTaskAssistPackItem _packItem;

    private GameGiftTitleItem _gameGiftTitleItem;

    protected void Awake()
    {
        _btnBuy = transform.Find("BuyButton").GetComponent<Button>();
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
        _contentText = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();

        _tagGroup = transform.Find("TipGroup");
        _tagText = transform.Find("TipGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _rewardParent = transform.Find("RewardGroup");
        _rewardItem = transform.Find("Item1 (1)");
        _rewardItem.gameObject.SetActive(false);
        BuyButton.onClick.AddListener(OnClickBuy);

        InvokeRepeating("UpdateTimeText", 0, 1);
        EventDispatcher.Instance.AddEventListener(EventEnum.TASKASSIST_PURCHASE_REFRESH, PurchseRefresh);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.TASKASSIST_PURCHASE_REFRESH, PurchseRefresh);
        CancelInvoke("UpdateTimeText");
    }

    public void UpdateTimeText()
    {
        if (_packItem == null)
            return;
        long leftTime = _packItem.EndTime - CommonUtils.GetTimeStamp();
        if (leftTime < 0)
        {
            DeleteBundle();
            leftTime = 0;
        }

        _timeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
    }

    private void PurchseRefresh(BaseEvent obj)
    {
        if (!TaskAssistPackModel.Instance.IsCanBuy(_taskAssist.Id))
            DeleteBundle();
        Refresh();
    }

    public void Init(TableShop cfg, TaskAssist taskAssist, GameGiftTitleItem gameGiftTitleItem,
        ShowArea area = ShowArea.none, string src = "")
    {
        _showArea = area;
        openSrc = src;
        if (cfg == null)
            return;
        _cfg = cfg;
        _taskAssist = taskAssist;
        _gameGiftTitleItem = gameGiftTitleItem;
        _packItem = TaskAssistPackModel.Instance.GetTaskAssistPackItem(_taskAssist.Id);
        Refresh();
    }

    public void DeleteBundle()
    {
        Destroy(gameObject);
        if (TaskAssistPackModel.Instance.GetCanBuyCount() > 0)
            CommonUtils.DelayedCall(0.1f, DeLayRefresh);
        else
        {
            _gameGiftTitleItem.gameObject.SetActive(false);
            UIStoreController.Instance?.ReBuild();
        }
    }

    private void DeLayRefresh()
    {
        _gameGiftTitleItem.ResetPageScrollView();
    }

    public void Refresh()
    {
        CommonUtils.DestroyAllChildren(_rewardParent);
        _priceText.text = (StoreModel.Instance.GetPrice(Cfg.id));

        _tagText.SetText((1 - Cfg.showDiscount) * 100 + "% \n" +
                         LocalizationManager.Instance.GetLocalizedString("ui_common_off"));
        string txt = LocalizationManager.Instance.GetLocalizedString("UI_info_text11");
        _tagText.SetText(string.Format(txt, TaskAssistPackModel.Instance.GetLeftCount(_taskAssist.Id)));
        _contentText.SetTerm(_taskAssist.Name);
        List<int> itemIds = new List<int>(_taskAssist.Content);
        List<int> itemCounts = new List<int>(_taskAssist.Count);

        for (int i = 0; i < itemIds.Count; i++)
        {
            var item = Instantiate(_rewardItem, _rewardParent);
            item.gameObject.SetActive(true);
            if (UserData.Instance.IsResource(itemIds[i]))
            {
                item.transform.Find("Icon").GetComponent<Image>().sprite =
                    UserData.GetResourceIcon(itemIds[i], UserData.ResourceSubType.Reward);
            }
            else
            {
                var itemConfig = GameConfigManager.Instance.GetItemConfig(itemIds[i]);
                item.transform.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }

            item.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(itemCounts[i].ToString());
            var _helpButon = item.transform.Find("HelpButton").GetComponent<Button>();
            int index = i;
            _helpButon.gameObject.SetActive(!UserData.Instance.IsResource(itemIds[index]));
            _helpButon.onClick.AddListener(() =>
            {
                MergeInfoView.Instance.OpenMergeInfo(itemIds[index],_isShowProbability:true);
                UIPopupMergeInformationController controller =
                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                if (controller != null)
                    controller.SetResourcesActive(false);
            });
        }
    }

    protected void OnClickBuy()
    {
        var extras = new Dictionary<string, string>();
        extras.Clear();
        extras.Add("type", "iap");
        extras.Add("amount", Cfg.price.ToString());
        extras.Add("packID", _taskAssist.Id.ToString());
        string type = "bundle";
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventStoreBuy, OpenSrc,
            MainOrderManager.Instance.GetCurMaxTaskID().ToString(), type, extras);

        int buyID = Cfg.id;

        AppConst.tempButRemoveAdsType = "store";
        AppConst.tempBuyStarPackType = "store";
        AppConst.tempBuySpecialPackType = "store";
        AppConst.tempBuyWeeklyCardType = "store";
        StoreModel.Instance.nowItemPos = this.transform.position;
        StoreModel.Instance.Purchase(buyID, openSrc, _taskAssist.Id);
    }
}