using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using System;
using ABTest;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using SomeWhere;
using TMPro;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class DailyTagData
{
    public GameObject gameOjbect;
    public GameObject tagFill;

    public void InitTagData(GameObject obj)
    {
        gameOjbect = obj;

        tagFill = gameOjbect.transform.Find("Fill").gameObject;

        TagShow(false);
    }

    public void TagShow(bool isShow)
    {
        tagFill.gameObject.SetActive(isShow);
    }

    public void Remove()
    {
        GameObject.DestroyImmediate(gameOjbect);
    }
}

public class GameGiftTitleItem : MonoBehaviour
{
    private Transform _bg;
    private Transform _bg2;
    private LocalizeTextMeshProUGUI title;
    private Button rvRefreshBtn;
    private Button dimondsRefreshBtn;
    private Image dimondsRefreshIcon;
    private LocalizeTextMeshProUGUI countdownText;
    private LocalizeTextMeshProUGUI rvRefreshText;
    private LocalizeTextMeshProUGUI diamondsRefreshText;
    private Transform refreshGroup;
    private Transform timeGroup;
    private Transform normalContent;
    private Transform diamondContent;
    
    private Transform bundleContent;
    private Transform specialContent;
    private Transform specialContentParent;
    private LocalizeTextMeshProUGUI tagText;
    [NonSerialized] public ShowArea showArea;
    bool isInit = false;
    private StorageFlashSale flashSale => StorageManager.Instance.GetStorage<StorageHome>().FlashSale;
    public Transform RefreshBtnTransform => dimondsRefreshBtn.transform;
    public Transform NormalContentTransform => normalContent;
    public Transform DiamondContentTransform => diamondContent;
    
    public Transform BundleContentTransform => bundleContent;
    public Transform SpecialContentTransform => specialContent;

    private float refreshTime = 0;

    private PageScrollView pageScrollView;
    private GameObject tagItem = null;
    private GameObject tagContent = null;
    private List<DailyTagData> pageTagDatas = new List<DailyTagData>();

    public void Init(ShowArea area = ShowArea.none)
    {
        if (isInit)
            return;
        _bg = transform.Find("BG");
        _bg2 = transform.Find("BG2");
        title = transform.Find("TitleBg/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        
        transform.Find("TitleBg_Vip").gameObject.SetActive(false);
        transform.Find("TitleBg").gameObject.SetActive(true);
        
        tagText = transform.Find("TagBG/TagText").GetComponent<LocalizeTextMeshProUGUI>();
        tagText.transform.parent.gameObject.SetActive(false);
        dimondsRefreshBtn = transform.Find("UIShopRefresh/ButtonGroup/BuyButton").GetComponent<Button>();
        dimondsRefreshIcon = transform.Find("UIShopRefresh/ButtonGroup/BuyButton/Icon").GetComponent<Image>();
        diamondsRefreshText = transform.Find("UIShopRefresh/ButtonGroup/BuyButton/Text")
            .GetComponent<LocalizeTextMeshProUGUI>();
        countdownText = transform.Find("UIShopRefresh/TimeBG/Text").GetComponent<LocalizeTextMeshProUGUI>();

        isInit = true;
        showArea = area;
        rvRefreshBtn = transform.Find("UIShopRefresh/ButtonGroup/FreeButton").GetComponent<Button>();
        refreshGroup = transform.Find("UIShopRefresh");
        normalContent = transform.Find("NormalContent");
        diamondContent = transform.Find("DiamondContent");
        specialContent = transform.Find("SpecialContent/Scroll View/Viewport/Content");
        specialContentParent = transform.Find("SpecialContent");
        bundleContent = transform.Find("BundleContent");
        specialContentParent.gameObject.SetActive(showArea == ShowArea.task_assist);
        refreshGroup.gameObject.SetActive(area == ShowArea.flash_sale);
        timeGroup = transform.Find("TimeGroup");
        timeGroup.gameObject.SetActive(area == ShowArea.daily_deals || area == ShowArea.sale_of_day || area == ShowArea.NewDailyPack);
        if (showArea == ShowArea.flash_sale)
        {
            Dictionary<string, string> extras = new Dictionary<string, string>();
            UIAdRewardButton.Create(ADConstDefine.RV_FLASH_REFRESH, UIAdRewardButton.ButtonStyle.Hide,
                rvRefreshBtn.gameObject,
                (s) =>
                {
                    GameBIManager.Instance.SendGameEvent(
                        BiEventCooking.Types.GameEventType.GameEventShopFlashsaleFleshWork, "rv");
            
                    if (s) StoreModel.Instance.RefreshFlashShop(0);
                }, true, () =>
                {
                    return !ABTestManager.Instance.IsOpenADTest();
                }, () =>
                {
                    GameBIManager.Instance.SendGameEvent(
                        BiEventCooking.Types.GameEventType.GameEventShopFlashsaleFleshClick, "rv");
                });
        }
        else if (showArea == ShowArea.task_assist)
        {
            _bg.gameObject.SetActive(false);
        }
        else if (showArea == ShowArea.daily_reward)
        {
            _bg.gameObject.SetActive(false);
            _bg2.gameObject.SetActive(true);
        }
        else if (showArea == ShowArea.daily_deals)
        {
            countdownText = transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }
        else if (showArea== ShowArea.sale_of_day)
        {
            _bg.gameObject.SetActive(false);
            countdownText = transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("OnRefreshDailyBundleText", 0, 1);
        }
        else if (showArea== ShowArea.NewDailyPack)
        {
            _bg.gameObject.SetActive(false);
            countdownText = transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            InvokeRepeating("OnRefreshNewDailyPackText", 0, 1);
        }
        else if (showArea== ShowArea.gem_shop)
        {
            var list = GlobalConfigManager.Instance.GetTableShop().FindAll(x => x.productType == (int)StoreModel.eProductType.Diamond);
            var isOpenTicket = BuyDiamondTicketModel.Instance.GetActiveTicket()!= null;
            transform.Find("TitleBg/BuyDiamondTicket").gameObject.SetActive(isOpenTicket);
            transform.Find("BuyDiamondTicket").gameObject.SetActive(isOpenTicket);
            InvokeRepeating("OnRefreshBuyDiamondExtraRewardTicketTimeText", 0, 1);
            var isOpen = false;
            foreach (var shopCfg in list)
            {
                if (ShopExtraRewardModel.Instance.GetExtraReward(shopCfg.id).Count > 0)
                {
                    isOpen = true;
                    break;
                }
            }
            transform.Find("TitleBg/ShopExtraReward").gameObject.SetActive(!isOpenTicket && isOpen);
            transform.Find("ShopExtraReward").gameObject.SetActive(!isOpenTicket && isOpen);
            InvokeRepeating("OnRefreshShopExtraRewardTimeText", 0, 1);
        }
        
        dimondsRefreshBtn.gameObject.SetActive(showArea == ShowArea.flash_sale);
        SetTitleText(showArea.ToString());
        
        if (showArea!= ShowArea.sale_of_day &&
            showArea!= ShowArea.NewDailyPack)
            InvokeRepeating("OnRefreshDailyRefreshText", 0, 1);


        tagItem = transform.Find("SpecialContent/ViewTag/tag").gameObject;
        tagItem.gameObject.SetActive(false);
        tagContent = transform.Find("SpecialContent/ViewTag").gameObject;
    }

    void Start()
    {
        if (dimondsRefreshBtn != null)
        {
            dimondsRefreshBtn.onClick.AddListener(OnDimondsRefreshBtn);

            var price = GetDiamondsRefreshPrice();
            if (price != null)
            {
                dimondsRefreshIcon.sprite = UserData.GetResourceIcon(price.id);
                diamondsRefreshText.SetText(price.count.ToString());
            }
        }
    }

    public void InitPageScrollView(int count)
    {
        if (pageScrollView != null)
            return;

        for (int i = 0; i < count; i++)
        {
            GameObject cloneTagItem = Instantiate(tagItem);
            cloneTagItem.SetActive(true);
            CommonUtils.AddChild(tagContent.transform, cloneTagItem.transform);

            DailyTagData dailyTagData = new DailyTagData();
            dailyTagData.InitTagData(cloneTagItem);
            pageTagDatas.Add(dailyTagData);
        }

        pageScrollView = transform.Find("SpecialContent/Scroll View").gameObject.AddComponent<PageScrollView>();
        pageScrollView.onPageChange = OnPageChange;
        ResetPageScrollView(0, true);
        pageScrollView.IsAutoScroll = true;
        pageScrollView.pageScrollType = PageScrollType.Horizontal;
        pageScrollView.AutoScrollTime = 5;
    }

    private void OnPageChange(int page)
    {
        if (pageTagDatas == null)
            return;

        for (int i = 0; i < pageTagDatas.Count; i++)
            pageTagDatas[i].TagShow(page == i);
    }

    public void ResetPageScrollView(int index = 0, bool isInit = false)
    {
        if (!isInit)
            UpdateTagData(pageScrollView.ChildCount());
        pageScrollView.UpdatePageCount();
        pageScrollView.ScrollPageImmediately(index);
    }

    private void UpdateTagData(int count)
    {
        if (count < 0 || count >= pageTagDatas.Count)
            return;

        for (int i = 0; i < pageTagDatas.Count; i++)
        {
            if (count > i)
                continue;

            pageTagDatas[i].Remove();
            pageTagDatas.RemoveAt(i);
            i--;
        }
    }

    private void Update()
    {
    }

    private void SetTitleText(string txt)
    {
        switch (showArea)
        {
            case ShowArea.sale_of_day:
                title.SetTerm("button_saleofday");
                break;
            case ShowArea.NewDailyPack:
                title.SetTerm("button_sale_for_today");
                break;
            case ShowArea.daily_deals:
                title.SetTerm("ui_shop_title_1");
                break;
            case ShowArea.gem_shop:
                title.SetTerm("button_gemshop");
                break;
            case ShowArea.coin_shop:
                title.SetTerm("button_coinshop");
                break;
            case ShowArea.daily_reward:
                title.SetTerm("button_dailydeals");
                break;
            case ShowArea.flash_sale:
                title.SetTerm("button_flashsale");
                break;
            case ShowArea.power_up:
                title.SetTerm("button_powerup");
                break;
            case ShowArea.energy:
                title.SetTerm("button_eneygy");  
                break;      
            case ShowArea.task_assist:
                title.SetTerm("ui_shop_title_task_assistant");
                break;
        }
    }

    private void OnRefreshDailyBundleText()
    {
        if (countdownText == null)
            return;
        int leftTime = DailyPackModel.Instance.GetPackCoolTime() ;
       if(leftTime<=0)
           DailyPackModel.Instance.RefreshPack();
       int showLeftTime = DailyPackModel.Instance.GetShowPackCoolTime() ;
       // timeGroup.gameObject.SetActive(leftTime > 0);
        countdownText.SetText( TimeUtils.GetTimeString(showLeftTime));
    }
    private void OnRefreshNewDailyPackText()
    {
        if (countdownText == null)
            return;
        int leftTime = NewDailyPackModel.Instance.GetPackCoolTime() ;
        if(leftTime<=0)
            NewDailyPackModel.Instance.RefreshPack();
        int showLeftTime = NewDailyPackModel.Instance.GetShowPackCoolTime() ;
        // timeGroup.gameObject.SetActive(leftTime > 0);
        countdownText.SetText( TimeUtils.GetTimeString(showLeftTime));
    }
    
    private void OnRefreshShopExtraRewardTimeText()
    {
        var isOpenTicket = BuyDiamondTicketModel.Instance.GetActiveTicket()!= null;
        var timeText = transform.Find("TitleBg/ShopExtraReward/TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        if (timeText == null)
            return;
        var list = GlobalConfigManager.Instance.GetTableShop().FindAll(x => x.productType == (int)StoreModel.eProductType.Diamond);
        var isOpen = false;
        foreach (var shopCfg in list)
        {
            if (ShopExtraRewardModel.Instance.GetExtraReward(shopCfg.id).Count > 0)
            {
                isOpen = true;
                break;
            }
        }
        transform.Find("TitleBg/ShopExtraReward").gameObject.SetActive(!isOpenTicket && isOpen);
        transform.Find("ShopExtraReward").gameObject.SetActive(!isOpenTicket && isOpen);
        timeText.SetText( ShopExtraRewardModel.Instance.GetActivityLeftTimeString());
    }
    private void OnRefreshBuyDiamondExtraRewardTicketTimeText()
    {
        var ticket = BuyDiamondTicketModel.Instance.GetActiveTicket();
        var isOpenTicket = ticket!= null;
        transform.Find("TitleBg/BuyDiamondTicket").gameObject.SetActive(isOpenTicket);
        transform.Find("BuyDiamondTicket").gameObject.SetActive(isOpenTicket);
        if (!isOpenTicket)
            return;
        var timeText = transform.Find("TitleBg/BuyDiamondTicket/TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
        if (timeText == null)
            return;
        timeText.SetText( ticket.GetTicketLeftTimeText());
    }

    private void OnRefreshDailyRefreshText()
    {
        if (countdownText == null)
            return;
        ulong severTime = APIManager.Instance.GetServerTime() / 1000;
        // DateTime serNow = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(severTime);
        DateTime serNow = new DateTime(1970, 1, 1).AddSeconds(severTime);
        DateTime refreshTime1 = serNow.AddDays(1);
        DateTime refreshTime2 = new DateTime(refreshTime1.Year, refreshTime1.Month, refreshTime1.Day, 0, 0, 0);
        TimeSpan span = refreshTime2 - serNow;

        int hours = span.Hours;
        int mins = span.Minutes;
        countdownText.SetText(TimeUtils.GetTimeString((int)span.TotalSeconds));
        if (span.TotalSeconds <= 1)
        {
            if (showArea == ShowArea.flash_sale)
            {
                StoreModel.Instance.RefreshFlashShop(-1);
            }
            else
            {
                StoreModel.Instance.RefreshDailyShop();
            }
        }
    }

    private void OnDimondsRefreshBtn()
    {
        if (Time.realtimeSinceStartup - refreshTime < 1)
            return;

        refreshTime = Time.realtimeSinceStartup;

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleFleshClick,
            "gem");
        if (showArea == ShowArea.flash_sale)
        {
            var price = GetDiamondsRefreshPrice();
            //Debug.LogError("price:" + price);
            if (UserData.Instance.CanAford(price))
            {
                var reason = new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.RefreshFlashSale
                };
                UserData.Instance.ConsumeRes(price, reason);
                StoreModel.Instance.RefreshFlashShop(1);
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventShopFlashsaleFleshWork,
                    "gem");
            }
            else
            {
                if (price.id == (int) UserData.ResourceId.Diamond)
                {
                    CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                    {
                        DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_info_text14"),
                        HasCloseButton = true,
                    });
                }
            }
        }
    }

    private ResData GetDiamondsRefreshPrice()
    {
        DailyShop shop = AdConfigHandle.Instance.GetDailyShops().Find(x => x.Type == 2);
        ResData data = null;
        if (shop.Refresh_gem_price != null && shop.Refresh_gem_price.Count > 1)
        {
            if (shop.Refresh_gem_price[0] == 1)
            {
                data = new ResData((int) UserData.ResourceId.Coin, shop.Refresh_gem_price[1]);
            }
            else if (shop.Refresh_gem_price[0] == 2)
            {
                data = new ResData((int) UserData.ResourceId.Diamond, shop.Refresh_gem_price[1]);
            }
        }

        return data;
    }

    private void OnDestroy()
    {
        CancelInvoke("OnRefreshDailyRefreshText");
    }
}