using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;
using Gameplay;
using Framework;
using Gameplay.UI.Store.Vip.Model;

public class UIDailyRVItemController : MonoBehaviour
{
    private GameObject _bg_1;
    private GameObject _bg_2;
    private GameObject _bg_3;
    private GameObject _unlockIcon;

    private Button _buttonAd;
    private Transform _buttonAdChild;
    GameObject _loadObj;
    public RVshopResource Data { get; private set; }

    public int Index { get; private set; }

    private Animator _itemAnimator;
    private Animator _bgAnimator;
    private Animator _lockAnimator;

    private int sortingOrder = 0;

    private LocalizeTextMeshProUGUI _buttonTextGreen;
    private LocalizeTextMeshProUGUI _buttonTextBlue;
    private Image _adIconImage;
    private Image _itemIcon;

    private bool canClick = false;
    private Image bgImage;

    private LocalizeTextMeshProUGUI _vipText;
    private void Awake()
    {
        _bg_1 = transform.Find("UIBgNormal/BGGroup/BG2").gameObject;
        _bg_2 = transform.Find("UIBgNormal/BGGroup/BG1").gameObject;
        _bg_3 = transform.Find("UIBgNormal/BGGroup/BG3").gameObject;

        _buttonAd = transform.Find("UIBgNormal/ReplayButton").GetComponent<Button>();
        _buttonAdChild = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim");
        _loadObj = transform.Find("UIBgNormal/ReplayGreyButton").gameObject;
        _loadObj.gameObject.SetActive(false);

        bgImage = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim/BG").GetComponent<Image>();

        _vipText = transform.Find("UIBgNormal/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _loadObj.GetComponent<Button>().onClick.AddListener(() =>
        {
            CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
            {
                DescString = LocalizationManager.Instance.GetLocalizedString("&key.UI_button_loading_ADS"),
            });
        });

        _unlockIcon = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim/LockIcon").gameObject;
        _buttonAd.onClick.AddListener(OnBtnAd);

        _itemAnimator = transform.Find("UIBgNormal").GetComponent<Animator>();
        _bgAnimator = transform.Find("UIBgNormal/BGGroup").GetComponent<Animator>();
        _lockAnimator = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim/LockIcon").GetComponent<Animator>();

        _buttonTextGreen = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim/TextGreen").GetComponent<LocalizeTextMeshProUGUI>();
        _buttonTextBlue = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim/TextBlue").GetComponent<LocalizeTextMeshProUGUI>();
        
        _adIconImage = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim/Icon/Image").GetComponent<Image>();
        _itemIcon = transform.Find("UIBgNormal/ReplayButton/ReplayButtonAnim/ItemIcon/Image").GetComponent<Image>();
    }

    private void OnBtnAd()
    {
        if (DailyRVModel.Instance.CurRvShopElementIdx != Index)
            return;

        if (!canClick)
            return;

        canClick = !UIDailyRVController.Instance.OnRVAdClicked(Data);
        if (Data.ConsumeType == 4)
            canClick = true;
    }

    public void Init(RVshopResource data, int elementIdx, int sortingOrder)
    {
        Data = data;
        Index = elementIdx;
        InitReward();
        RefreshState();
        this.sortingOrder = sortingOrder;

        if (data.ConsumeType != 1)
        {
            bgImage.sprite =
                ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, "Common_Button_Green");
        }
    }

    public void RefreshState()
    {
        if (DailyRVModel.Instance.CurRvShopElementIdx == Index)
        {
            PlayAnimator(_bgAnimator, "NormalBG2");
            canClick = true;
        }
        else if (DailyRVModel.Instance.CurRvShopElementIdx + 1 == Index)
        {
            PlayAnimator(_bgAnimator, "NormalBG1");
        }
        else if (DailyRVModel.Instance.CurRvShopElementIdx + 2 == Index)
        {
            PlayAnimator(_bgAnimator, "NormalBG3");
        }
        else
        {
            PlayAnimator(_bgAnimator, "NormalBG3");
            PlayAnimator(_itemAnimator, "hide");
        }

        _vipText.transform.parent.gameObject.SetActive(false);
        
        if (Data.ConsumeType == 1) //rv
        {
            _buttonTextBlue.SetText(LocalizationManager.Instance.GetLocalizedString("UI_button_watch"));
            _buttonTextBlue.gameObject.SetActive(true);
            _buttonTextGreen.gameObject.SetActive(false);
            _adIconImage.transform.parent.gameObject.SetActive(true);
            _itemIcon.transform.parent.gameObject.SetActive(false);
        }
        else if (Data.ConsumeType == 2) //钻石
        {
            _buttonTextGreen.SetText(Data.ConsumeAmount.ToString());
            _buttonTextBlue.gameObject.SetActive(false);
            _buttonTextGreen.gameObject.SetActive(true);
            _adIconImage.transform.parent.gameObject.SetActive(false);
            _itemIcon.transform.parent.gameObject.SetActive(true);
            _itemIcon.sprite = UserData.GetResourceIcon(UserData.ResourceId.Diamond);
        }
        else if (Data.ConsumeType == 3) //free
        {
            _buttonTextBlue.gameObject.SetActive(false);
            _buttonTextGreen.gameObject.SetActive(true);
            _buttonTextGreen.SetText(LocalizationManager.Instance.GetLocalizedString("button_free"));
            _adIconImage.transform.parent.gameObject.SetActive(false);
            _itemIcon.transform.parent.gameObject.SetActive(false);
        }
        else if (Data.ConsumeType == 4) //充值
        {
            TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(Data.ConsumeAmount);
            _buttonTextBlue.gameObject.SetActive(false);
            _buttonTextGreen.gameObject.SetActive(true);
            if (tableShop != null)
                _buttonTextGreen.SetText(StoreModel.Instance.GetPrice(tableShop.id));

            _vipText.transform.parent.gameObject.SetActive(true);
            _vipText.SetText(VipStoreModel.Instance.GetVipScoreString(tableShop.price));
            
            _adIconImage.transform.parent.gameObject.SetActive(false);
            _itemIcon.transform.parent.gameObject.SetActive(false);
        }
        else if (Data.ConsumeType == 5) //金币
        {
            _buttonTextBlue.gameObject.SetActive(false);
            _buttonTextGreen.gameObject.SetActive(true);
            _buttonTextGreen.SetText(Data.ConsumeAmount.ToString());
            _adIconImage.transform.parent.gameObject.SetActive(false);
            _itemIcon.transform.parent.gameObject.SetActive(true);
            _itemIcon.sprite= UserData.GetResourceIcon(UserData.ResourceId.Coin);
        }

        if (DailyRVModel.Instance.CurRvShopElementIdx == Index)
        {
            if (Data.ConsumeType == 1)
            {
                AdRewardedVideoPlacementMonitor.Bind(_buttonAd.gameObject, ADConstDefine.RV_TV_REWARD);
                bool isCanPlayRV = AdSubSystem.Instance.CanPlayRV(ADConstDefine.RV_TV_REWARD);
     
                _buttonAdChild.gameObject.SetActive(isCanPlayRV);
                _loadObj.SetActive(isCanPlayRV == false);
       
            }
            else
            {
                _buttonAd.gameObject.SetActive(true);
                _loadObj.gameObject.SetActive(false);
            }

            _unlockIcon.SetActive(false);
        }
        else
        {
            _unlockIcon.SetActive(true);
            _buttonAd.gameObject.SetActive(true);
            _loadObj.SetActive(false);
        }
    }

    private void InitReward()
    {
        GameObject rewards = transform.Find("UIBgNormal/Rewards").gameObject;
        GameObject item = transform.Find("UIBgNormal/Rewards/Item").gameObject;
        item.gameObject.SetActive(false);

        foreach (Transform trans in rewards.transform)
        {
            if(trans.gameObject != item)
                Destroy(trans.gameObject);
        }
        
        for (int i = 0; i < Data.RewardID.Count; i++)
        {
            var im = Instantiate(item, rewards.transform);
            im.gameObject.SetActive(true);

            Image iconImg = CommonUtils.Find<Image>(im.transform, "RewardIcon");
            Button _tipsBtn = CommonUtils.Find<Button>(im.transform, "TipsBtn");
            _tipsBtn.gameObject.SetActive(false);
            if (iconImg != null)
            {
                if (UserData.Instance.IsResource(Data.RewardID[i]))
                    iconImg.sprite = UserData.GetResourceIcon(Data.RewardID[i], UserData.ResourceSubType.Reward);
                else
                {
                    TableMergeItem mergeConfig = GameConfigManager.Instance.GetItemConfig(Data.RewardID[i]);
                    if (mergeConfig != null)
                    {
                        iconImg.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(mergeConfig.image);
                        if (mergeConfig.type == 2)
                        {
                            _tipsBtn.gameObject.SetActive(true);
                            _tipsBtn.onClick.AddListener(() =>
                            {
                                MergeInfoView.Instance.OpenMergeInfo(mergeConfig,_isShowProbability:true);
                                UIPopupMergeInformationController controller =
                                    UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as
                                        UIPopupMergeInformationController;
                                if (controller != null)
                                    controller.SetResourcesActive(false);
                            });
                        }
                    }
                }
            }

            LocalizeTextMeshProUGUI text = CommonUtils.Find<LocalizeTextMeshProUGUI>(im.transform, "Text");
            text.SetText(CommonUtils.NumString(Data.RewardType, Data.Amount[i], "X"));
            float scale = 1f;
            if (Data.RewardID.Count == 1)
                scale = 1.12f;
            iconImg.transform.localScale = new Vector3(scale, scale, 1);
        }
       
    }

    public void ToDone()
    {
        PlayAnimator(_itemAnimator, "disappear");
    }

    public void ToUnLock(int index)
    {
        if (index == 1)
        {
            PlayAnimator(_bgAnimator, "BG2");
            StartCoroutine(CommonUtils.PlayAnimation(_lockAnimator, "disappear", "",
                () => { _unlockIcon.gameObject.SetActive(false); }));

            Canvas lockCanvas = _unlockIcon.gameObject.GetComponent<Canvas>();
            if (lockCanvas == null)
                lockCanvas = _unlockIcon.gameObject.AddComponent<Canvas>();
            
            lockCanvas.overrideSorting = true;
            lockCanvas.sortingOrder = sortingOrder;
        }
        else if (index == 2)
        {
            PlayAnimator(_bgAnimator, "BG1");
        }
        else if (index == 3)
        {
            PlayAnimator(_itemAnimator, "appear");
        }
    }

    private void PlayAnimator(Animator animator, string name)
    {
        if (animator == null)
            return;

        animator.Play(name, 0, 0f);
    }
}