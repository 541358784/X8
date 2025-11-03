using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIStorePigItem : UIStoreBaseItem
{
    public override UIStoreItemType ItemType { get; } = UIStoreItemType.Bundle;

    protected override Button BuyButton
    {
        get { return _btnBuy; }
    }

    private Button _btnBuy;

    private Text _priceText;
    
    private LocalizeTextMeshProUGUI _contentText;

    // private Transform _tagGroup;
    // private LocalizeTextMeshProUGUI _tagText;

    private LocalizeTextMeshProUGUI _timeText;

    private Image _rewardIcon;
    private LocalizeTextMeshProUGUI _rewardText;

    private List<GameObject> _pigBg = new List<GameObject>();
    // private LocalizeTextMeshProUGUI _rareText;
    Dictionary<string, string> extras = new Dictionary<string, string>();

    // private Transform _rareBG;
    // private LocalizeTextMeshProUGUI _tagText;
    protected override void PrivateAwake()
    {
        _btnBuy = transform.Find("BuyButton").GetComponent<Button>();
        _priceText = transform.Find("BuyButton/Text").GetComponent<Text>();
 
        // _tagGroup = transform.Find("TagGroup");
        // _tagText = transform.Find("TagGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        // _tagText.gameObject.SetActive(false);
        _rewardIcon = transform.Find("RewardGroup/Icon").GetComponent<Image>();
        _rewardText = transform.Find("RewardGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _timeText = transform.Find("TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        // _rareText = transform.Find("RareText").GetComponent<LocalizeTextMeshProUGUI>();
        // _rareText.gameObject.SetActive(false);
        // _rareBG = transform.Find("RareBG");
        // _rareBG.gameObject.SetActive(false);

        for (int i = 1; i <= 3; i++)
        {
            _pigBg.Add(transform.Find("BG/BG"+i).gameObject);
            _pigBg[i-1].SetActive(PigBankModel.Instance.GetCurIndex() == i-1);
        }

        transform.Find("HelpButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupPigBox);
        });
    }

    protected override void PrivateStart()
    {
       InvokeRepeating("RefreshTime",0,1);
    }

    public void RefreshTime()
    {
        if (PigBankModel.Instance.GetActivityLeftTime() <= 0 || !PigBankModel.Instance.IsOpened() || !PigBankModel.Instance.IsCanBuy())
        {
            gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
        }
        _timeText.SetText(PigBankModel.Instance.GetActivityLeftTimeString());
    }
    public override void Refresh()
    {
        var _curPigBankTable = PigBankModel.Instance.GetAdPigBankTable();
        TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(_curPigBankTable.ShopId);
        if (null == tableShop)
        {
            DebugUtil.LogError("shopConfig is Null " + ID);
            return;
        }
 
        _rewardText.SetText(PigBankModel.Instance.GetCanCollectValue().ToString());
        _priceText.text = (StoreModel.Instance.GetPrice(_curPigBankTable.ShopId));

        transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(tableShop.price));
    }

}