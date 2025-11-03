using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;
using System;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using SomeWhere;
using  TMPro;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class HappyGoGameGiftTitleItem : MonoBehaviour
{
    private LocalizeTextMeshProUGUI title;
    private Button rvRefreshBtn;
    private GameObject _gary;
    private LocalizeTextMeshProUGUI _greyText;
    private Button dimondsRefreshBtn;
    private Image dimondsRefreshIcon;
    private LocalizeTextMeshProUGUI countdownText;
    private LocalizeTextMeshProUGUI rvRefreshText;
    private LocalizeTextMeshProUGUI diamondsRefreshText;
    private Transform refreshGroup;
    private Transform timeGroup;
    private Transform normalContent;
    private Transform specialContent;
    private Transform bundleContent;
    private LocalizeTextMeshProUGUI tagText;
    [NonSerialized] public ShowArea showArea;
    bool isInit = false;
    private StorageFlashSale flashSale => StorageManager.Instance.GetStorage<StorageHome>().FlashSale;
    public Transform RefreshBtnTransform => dimondsRefreshBtn.transform;
    public Transform NormalContentTransform => normalContent;
    public Transform BundleContentTransform => bundleContent;

    private float refreshTime = 0;

    private PageScrollView pageScrollView;
    private GameObject tagItem = null;
    private GameObject tagContent = null;
    private List<DailyTagData> pageTagDatas = new List<DailyTagData>();
    public void Init(ShowArea area=ShowArea.none)
    {
        if (isInit)
            return;
        
        title = transform.Find("TitleBg/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        tagText = transform.Find("TagBG/TagText").GetComponent<LocalizeTextMeshProUGUI>();
        tagText.transform.parent.gameObject.SetActive(false);
        dimondsRefreshBtn = transform.Find("UIShopRefresh/ButtonGroup/BuyButton").GetComponent<Button>();
        dimondsRefreshIcon = transform.Find("UIShopRefresh/ButtonGroup/BuyButton/Icon").GetComponent<Image>();
        _gary = transform.Find("UIShopRefresh/ButtonGroup/BuyButton/GreyBG").gameObject;
        _greyText = transform.Find("UIShopRefresh/ButtonGroup/BuyButton/GreyText").GetComponent<LocalizeTextMeshProUGUI>();
        diamondsRefreshText =transform.Find("UIShopRefresh/ButtonGroup/BuyButton/Text").GetComponent<LocalizeTextMeshProUGUI>(); 
        countdownText = transform.Find("UIShopRefresh/TimeBG/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        isInit = true;
        showArea = area;
        rvRefreshBtn = transform.Find("UIShopRefresh/ButtonGroup/FreeButton").GetComponent<Button>();
        refreshGroup=transform.Find("UIShopRefresh");
        normalContent=transform.Find("NormalContent");
        specialContent=transform.Find("SpecialContent");
        bundleContent=transform.Find("SpecialContent/Scroll View/Viewport/Content");
        specialContent.gameObject.SetActive(showArea==ShowArea.sale_of_day);
        refreshGroup.gameObject.SetActive(showArea==ShowArea.flash_sale);
        timeGroup=transform.Find("TimeGroup");
        timeGroup.gameObject.SetActive(false);
        
        
        tagItem = transform.Find("SpecialContent/ViewTag/tag").gameObject;
        tagItem.gameObject.SetActive(false);
        
        tagContent = transform.Find("SpecialContent/ViewTag").gameObject;
        
        SetTitleText(showArea.ToString());
        InvokeRepeating("OnRefreshDailyRefreshText", 0, 1);
    }
    
    void Start()
    {
        if (dimondsRefreshBtn != null)
        {
            dimondsRefreshBtn.onClick.AddListener(OnDimondsRefreshBtn);
       
            var price= GetDiamondsRefreshPrice();
            if (price != null)
            {
                dimondsRefreshIcon.sprite = UserData.GetResourceIcon(price.id);
                diamondsRefreshText.SetText(price.count.ToString());
                _greyText.SetText(price.count.ToString());

            }
            CommonUtils.SetShieldButUnEnable(dimondsRefreshBtn.gameObject);
            UpdateRefreshStatus();
        }
        
    }

    public void InitPageScrollView(int count)
    {
        if(pageScrollView != null)
            return;
        
        for(int i =0; i < count; i++)
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
        pageScrollView.UpdatePageCount();
        pageScrollView.ScrollPageImmediately(0);
        pageScrollView.IsAutoScroll = true;
        pageScrollView.pageScrollType = PageScrollType.Horizontal;
        pageScrollView.AutoScrollTime = 5;
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
    private void OnPageChange(int page)
    {
        if(pageTagDatas == null)
            return;
        
        for(int i = 0; i < pageTagDatas.Count; i++)
            pageTagDatas[i].TagShow(page == i);
    }
    
    private void Update()
    {
       
    }

    private void SetTitleText(string txt)
    {
        switch (showArea)
        {
            case ShowArea.sale_of_day:
                title.transform.parent.gameObject.SetActive(false);
                title.SetTerm("button_saleofday");
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
        }
    }

    private void OnRefreshDailyRefreshText()
    {
        if (countdownText == null)
            return;
        if (showArea == ShowArea.flash_sale)
        {
            long time=HappyGoStoreModel.Instance.GetFlashRefreshLeftTime();
            countdownText.SetText(TimeUtils.GetTimeString((int)time));
            if (time<=0)
            {
                HappyGoStoreModel.Instance.RefreshFlashShop(-1);
            }
        }
    }

    public void UpdateRefreshStatus()
    {
        _gary.SetActive(!HappyGoStoreModel.Instance.IsCanShowRefresh());
        dimondsRefreshBtn.interactable = HappyGoStoreModel.Instance.IsCanShowRefresh();
    }
    private void OnDimondsRefreshBtn()
    {
        if(Time.realtimeSinceStartup - refreshTime < 1)
            return;

        refreshTime = Time.realtimeSinceStartup;
        
        if (showArea == ShowArea.flash_sale)
        {


            var price = GetDiamondsRefreshPrice();
            //Debug.LogError("price:" + price);
            if (UserData.Instance.CanAford(price))
            {
                var reason = new GameBIManager.ItemChangeReasonArgs
                {
                    reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.HgVdShopFlashRefresh
                };
                UserData.Instance.ConsumeRes( price, reason);
                HappyGoStoreModel.Instance.RefreshFlashShop(1);
                UpdateRefreshStatus();
                HappyGoStoreModel.Instance.flashSale.RefreshCount++;
                GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventHgVdShopFlashRefresh, HappyGoStoreModel.Instance.flashSale.RefreshCount.ToString(),price.count.ToString());

            }
            else
            {
                if (price.id == (int) UserData.ResourceId.Diamond)
                {
                    BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Diamond, "",
                        "" ,"openSr",needCount:price.count);
                }
            }
        }
    }

    private ResData GetDiamondsRefreshPrice()
    {
        int count = HappyGoModel.Instance.HappyGoConfig == null
            ? 20
            : HappyGoModel.Instance.HappyGoConfig.flashShopFreshCost;
        ResData data=new ResData((int)UserData.ResourceId.Diamond,count);
        return data;
    }
    
    private void OnDestroy()
    {
        CancelInvoke("OnRefreshDailyRefreshText");
    }
}
