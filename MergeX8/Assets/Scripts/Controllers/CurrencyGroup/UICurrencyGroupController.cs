using System;
using System.Collections;
using System.Collections.Generic;
using ActivityLocal.ClimbTower.Model;
using DG.Tweening;
using DragonPlus;
using Game;
using Gameplay;
using SRF;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public partial class EventEnum
{
    public static string EventOpenBuyItemPopup = "EventOpenBuyItemPopup";
    public static string EventCloseBuyItemPopup = "EventCloseBuyItemPopup";
}
public class UICurrencyGroupController : UIWindowController
{
    private enum EnergyShowType
    {
        Times, // 次数
        Infinite, // 无限体力
        NextAddCountDown, // 下次获得体力倒计时
        None,
    }

    private Dictionary<UserData.ResourceId, LocalizeTextMeshProUGUI> resTexts =
        new Dictionary<UserData.ResourceId, LocalizeTextMeshProUGUI>();

    private Dictionary<UserData.ResourceId, Image> iconDic =
        new Dictionary<UserData.ResourceId, Image>();

    private Dictionary<UserData.ResourceId, Vector3> resPositions = new Dictionary<UserData.ResourceId, Vector3>();
    private Dictionary<UserData.ResourceId, Animator> resAnimators = new Dictionary<UserData.ResourceId, Animator>();
    private Dictionary<UserData.ResourceId, Transform> resGroupTrans = new Dictionary<UserData.ResourceId, Transform>();
    private Dictionary<UserData.ResourceId, Tween> _resTweens = new Dictionary<UserData.ResourceId, Tween>();
    
    private List<GameObject> addbuttons = new List<GameObject>();
    private Button goGemButton;
    private Button goCoinButton;
    private Button goDecoCoinButton;


    //---ENERGY
    private LocalizeTextMeshProUGUI energyTimeText;
    private Button _goEnergyButton;
    private GameObject _goEnergyIcon;
    private EnergyShowType _energyShowType = EnergyShowType.None;
    private GameObject energyTimeBG;
    private GameObject energyInfinity;
    //---HgENERGY
    private LocalizeTextMeshProUGUI hgEnergyTimeText;
    private Button _hgGoEnergyButton;
    private GameObject _hgGoEnergyIcon;
    private EnergyShowType _hgEnergyShowType = EnergyShowType.None;
    private GameObject _hgEnergyTimeBG;

    //---EXP
    public RectTransform expTrans;

    private UserData.ResourceId energyIconType = UserData.ResourceId.None;
    public ExperienceProgress _experience;

    private int orgSortOrder = 0;

    //---Shop
    private Button goShopButton;
    private GameObject storeRedPoint;

    private Button _happyGoShop;
    public Transform ShopTransform
    {
        get { return goShopButton?.transform; }
    }
    public override void PrivateAwake()
    {
        InitUI();
        isPlayDefaultAudio = false;
        EventDispatcher.Instance.AddEventListener(EventEnum.UserDataUpdate, UserDataUpdate);
        //EventDispatcher.Instance.AddEventListener(EventEnum.OnIAPItemPaid, UserDataUpdate);
        EventDispatcher.Instance.AddEventListener(EventEnum.EnergyChanged, UserEnergyUpdate);
        EventDispatcher.Instance.AddEventListener(EventEnum.HGEnergyChanged, HGUserEnergyUpdate);
        EventDispatcher.Instance.AddEventListener(MergeEvent.DAILYDEALS_PURCHASE_SUCCESS, OnDailyDealsPurchase);
        AnimControlManager.Instance.InitAnimControl(AnimKey.Main_ResBar, gameObject, true);
        EventDispatcher.Instance.AddEventListener(EventEnum.EventOpenBuyItemPopup,ShowBar3);
        EventDispatcher.Instance.AddEventListener(EventEnum.EventCloseBuyItemPopup,HideBar3);
    }

    public void ShowBar3(BaseEvent evt)
    {
        var groupBar3 = transform.Find("Root/ResourcesGroup/CurrencyBarGroup3");
        groupBar3.gameObject.SetActive(true);
    }
    public void HideBar3(BaseEvent evt)
    {
        var groupBar3 = transform.Find("Root/ResourcesGroup/CurrencyBarGroup3");
        groupBar3.gameObject.SetActive(false);
    }


    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        orgSortOrder = canvas.sortingOrder;

        foreach (var kv in iconDic)
        {
            if (kv.Value == null)
                continue;

            if(kv.Key == UserData.ResourceId.HappyGo_Energy)
                resPositions[kv.Key] = resPositions[UserData.ResourceId.Energy];
            else
            {
                resPositions[kv.Key] = kv.Value.transform.position;
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        orgSortOrder = canvas.sortingOrder;
        UpdateEnergyPerSecond();
        HGUpdateEnergyPerSecond();
    }

    private void InitUI()
    {
        expTrans = CommonUtils.Find<RectTransform>(transform, "Root/ResourcesGroup/Head");
        _experience = expTrans.gameObject.AddComponent<ExperienceProgress>();
        iconDic.Add(UserData.ResourceId.Exp, _experience.ExpBackGround);
        resAnimators.Add(UserData.ResourceId.Exp, expTrans.GetComponent<Animator>());
        resGroupTrans.Add(UserData.ResourceId.Exp,expTrans);
        
       var redPoint = CommonUtils.Find<RectTransform>(transform, "Root/ResourcesGroup/Head/StarNode/RedPoint");
       var redPointScript = redPoint.gameObject.AddComponent<UIPopupSetRedPoint>();
       redPointScript.Init();
       
        InitResourceData("Root/ResourcesGroup/CurrencyBarGroup2", UserData.ResourceId.Diamond);
        InitResourceData("Root/ResourcesGroup/CurrencyBarGroup0", UserData.ResourceId.Coin);
        InitResourceData("Root/ResourcesGroup/CurrencyBarGroup1", UserData.ResourceId.Energy);
        InitResourceData("Root/ResourcesGroup/CurrencyBarGroup3", UserData.ResourceId.RareDecoCoin);
        InitResourceData("Root/ResourcesGroup/CurrencyBarGroup4", UserData.ResourceId.HappyGo_Energy);
        
        InitAddButton("Root/ResourcesGroup/Head/StarNode/Button");
        // InitAddButton("Root/ResourcesGroup/CurrencyBarGroup0/ButtonAdd");
        InitAddButton("Root/ResourcesGroup/CurrencyBarGroup1/ButtonAdd");
        InitAddButton("Root/ResourcesGroup/CurrencyBarGroup2/ButtonAdd");
        InitAddButton("Root/ResourcesGroup/CurrencyBarGroup3/ButtonAdd");
        InitAddButton("Root/ResourcesGroup/CurrencyBarGroup4/ButtonAdd");
        var showBarGroupBtn = transform.Find("Root/ButtonShowBarGroup").GetComponent<Button>();
        var groupBar3 = transform.Find("Root/ResourcesGroup/CurrencyBarGroup3");
        showBarGroupBtn.onClick.AddListener(() =>
        {
            if (groupBar3.gameObject.activeSelf)
            {
                groupBar3.gameObject.SetActive(false);
            }
            else
            {
                groupBar3.gameObject.SetActive(true);
            }
        });
        groupBar3.gameObject.SetActive(false);
        XUtility.WaitFrames(2, () =>
        {
            groupBar3.gameObject.SetActive(false);
            showBarGroupBtn.GetComponent<ShieldButtonOnClick>().isUse = false;
        });

        //钻石
        goGemButton = CommonUtils.Find<Button>(transform, "Root/ResourcesGroup/CurrencyBarGroup2/ButtonAdd");
        goGemButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(SfxNameConst.button_s);
            string openType = SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game
                ? "diamond_game"
                : "diamond_main";
            if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIStore) == null)
            {
                UIStoreController.OpenUI(openType,ShowArea.gem_shop);
            }
        });
        //金币
        goCoinButton = CommonUtils.Find<Button>(transform, "Root/ResourcesGroup/CurrencyBarGroup0/ButtonAdd");
        goCoinButton.gameObject.SetActive(false);
        goCoinButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(SfxNameConst.button_s);
            string openType = SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game ? "coin_in" : "coin_out";
            string openType2 = SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game ? "2" : "1";
            if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIStoreGame) == null)
            {
                BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Coin, openType2,
                    "", openType);
            }
        });

        //装修币
        goDecoCoinButton = CommonUtils.Find<Button>(transform, "Root/ResourcesGroup/CurrencyBarGroup3/ButtonAdd");
        goDecoCoinButton.onClick.AddListener(() => { UIPopupNoMoneyController.ShowUI((int) UserData.ResourceId.Coin); });

        //体力
        energyTimeText =
            CommonUtils.Find<LocalizeTextMeshProUGUI>(transform,
                "Root/ResourcesGroup/CurrencyBarGroup1/TimeBG/TimeText");
        energyTimeBG =transform.Find("Root/ResourcesGroup/CurrencyBarGroup1/TimeBG").gameObject;
        energyInfinity =transform.Find("Root/ResourcesGroup/CurrencyBarGroup1/Infinity").gameObject;

        _goEnergyIcon = transform.Find("Root/ResourcesGroup/CurrencyBarGroup1/ButtonAdd/Icon").gameObject;
        _goEnergyButton = CommonUtils.Find<Button>(transform, "Root/ResourcesGroup/CurrencyBarGroup1/ButtonAdd");
        _goEnergyButton.onClick.AddListener(() =>
        {
            string openType = SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game ? "2" : "1";

            BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.Energy, openType, "",
                "diamond_lack_energy");
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventLackEnergyPop, "handle");
            
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.AddEnergy);
        });
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_goEnergyButton.transform.parent);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.AddEnergy, (RectTransform)_goEnergyButton.transform, topLayer:topLayer, isReplace:false);
        //Happy go  体力
        hgEnergyTimeText =
            CommonUtils.Find<LocalizeTextMeshProUGUI>(transform,
                "Root/ResourcesGroup/CurrencyBarGroup4/TimeBG/TimeText");
        _hgEnergyTimeBG =transform.Find("Root/ResourcesGroup/CurrencyBarGroup4/TimeBG").gameObject;

        _hgGoEnergyIcon = transform.Find("Root/ResourcesGroup/CurrencyBarGroup4/ButtonAdd/Icon").gameObject;
        _hgGoEnergyButton = CommonUtils.Find<Button>(transform, "Root/ResourcesGroup/CurrencyBarGroup4/ButtonAdd");
        _hgGoEnergyButton.onClick.AddListener(OnHappyGoEnergyAdd);
         
        goShopButton = CommonUtils.Find<Button>(transform, "Root/ResourcesGroup/ButtonShop");
        goShopButton.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ClickStore);
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TeamShopEntrance1);
            UIStoreController.OpenUI();
        });
        storeRedPoint = goShopButton.transform.Find("Tips").gameObject;
        
        _happyGoShop= CommonUtils.Find<Button>(transform, "Root/ResourcesGroup/ButtonHappyGoShop");
        _happyGoShop.onClick.AddListener(() =>
        {
            HappyGoUIStoreGameController.OpenUI("");
            GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventHgVdShop);
        });
        InvokeRepeating("RefreshRepeating", 0, 1f);
        List<Transform> topLayer1 = new List<Transform>();
        topLayer1.Add(goShopButton.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ClickStore, (RectTransform)goShopButton.transform, topLayer:topLayer1, isReplace:false);
        SetEnergyShowType(true);
    }
    private void RefreshRepeating()
    {
        RefreshShopRedPoint();
    }


    private void OnDailyDealsPurchase(BaseEvent evt)
    {
        RefreshShopRedPoint();
    }
    private void RefreshShopRedPoint()
    {
        if (storeRedPoint == null)
            return;
        int itemId = 0;
        bool show =  ClimbTowerModel.Instance.IsCanEnter();
        storeRedPoint.SetActive(show);
    }
    public void UpdateShowType(CurrencyGroupManager.CurrencyShowType showType)
    {
        goShopButton.gameObject.SetActive(true);
        // goDecoCoinButton.transform.parent.gameObject.SetActive(showType == CurrencyGroupManager.CurrencyShowType.Home);
    }
    private void InitResourceData(string path, UserData.ResourceId resourceId)
    {
        LocalizeTextMeshProUGUI diamondText = GetItem<LocalizeTextMeshProUGUI>(path + "/Text");
        Image diamondIcon = GetItem<Image>(path + "/Icon");
        resTexts.Add(resourceId, diamondText);
        iconDic.Add(resourceId, diamondIcon);
        resAnimators.Add(resourceId, GetItem<Animator>(path));
        resGroupTrans.Add(resourceId,transform.Find(path));
    }

    private void InitAddButton(string path)
    {
        GameObject addBtn = GetItem(path);
        if (addBtn == null)
            return;

        addbuttons.Add(addBtn);
    }

    public Transform GetIconTransform(UserData.ResourceId resId)
    {
        if (!iconDic.ContainsKey(resId))
            return null;

        if (iconDic[resId] == null)
            return null;

        return iconDic[resId].transform;
    }

    public Vector3 GetResourcePosition(UserData.ResourceId resId)
    {
        if (!resPositions.ContainsKey(resId))
            return Vector3.zero;

        return resPositions[resId];
    }

    public void PlayShakeAnim(UserData.ResourceId resId)
    {
        if (!resAnimators.ContainsKey(resId))
            return;

        resAnimators[resId].Play("shake", 0, 0f);
    }
    
    public void UpdateText(UserData.ResourceId resId, int subNum, int resNum, float time, System.Action callBack = null)
    {
        if (!resTexts.ContainsKey(resId))
        {
            if (resId == UserData.ResourceId.Exp)
            {
                _experience.UpdateText(subNum, time, callBack);
            }
            else
            {
                callBack?.Invoke();
            }

            return;
        }

        int newValue = resNum;
        int oldValue = newValue - subNum;
        oldValue = Math.Max(oldValue, 0);
        newValue = Math.Max(newValue, 0);

        resTexts[resId].SetText(oldValue.ToString());

        if (_resTweens.ContainsKey(resId) && _resTweens[resId] != null)
        {
            _resTweens[resId].Kill();
            _resTweens[resId] = null;
        }
        
        _resTweens[resId] = DOTween.To(() => oldValue, x => oldValue = x, newValue, time).OnUpdate(() =>
        {
            resTexts[resId].SetText(oldValue.ToString());
        }).OnComplete(() =>
        {
            _resTweens[resId] = null;
            callBack?.Invoke();
        });
    }

    public void InitText(UserData.ResourceId resId, int money)
    {
        if (!resTexts.ContainsKey(resId))
            return;

        money = Math.Max(money, 0);
        resTexts[resId].SetText(money.ToString());
    }
    
    public void UpdateUI()
    {
        resTexts[UserData.ResourceId.Coin].SetText(UserData.Instance.GetRes(UserData.ResourceId.Coin).ToString());
        resTexts[UserData.ResourceId.RareDecoCoin]
            .SetText(UserData.Instance.GetRes(UserData.ResourceId.RareDecoCoin).ToString());
        resTexts[UserData.ResourceId.Diamond].SetText(UserData.Instance.GetRes(UserData.ResourceId.Diamond).ToString());
        resTexts[UserData.ResourceId.Energy].SetText(EnergyModel.Instance.EnergyNumber().ToString());
        resTexts[UserData.ResourceId.HappyGo_Energy].SetText(HappyGoEnergyModel.Instance.EnergyNumber().ToString());

        _UpdateInfiniteEnergy();
        _HGUpdateInfiniteEnergy();
    }

    public void NotchAdapte()
    {
        CommonUtils.NotchAdapte(GetItem("Root").transform);
    }

    private void UserDataUpdate(BaseEvent e)
    {
        UpdateUI();
    }

    #region 体力

    private void UserEnergyUpdate(BaseEvent obj)
    {
        _UpdateInfiniteEnergy();
        resTexts[UserData.ResourceId.Energy].SetText(EnergyModel.Instance.EnergyNumber().ToString());
    }

    void UpdateEnergyPerSecond()
    {
        _UpdateInfiniteEnergy();
        if (this.gameObject.activeSelf)
            this.StartCoroutine(CommonUtils.DelayWork(1f, () => { UpdateEnergyPerSecond(); }));
    }

    private void _UpdateInfiniteEnergy()
    {
        //刷新体力
        var leftTime = EnergyModel.Instance.EnergyUnlimitedLeftShowTime();
        if (leftTime > 0)
        {
            if (_energyShowType != EnergyShowType.Infinite)
            {
                if (energyIconType != UserData.ResourceId.Infinity_Energy)
                {
                    iconDic[UserData.ResourceId.Energy].sprite =
                        UserData.GetResourceIcon(UserData.ResourceId.Energy, UserData.ResourceSubType.Big);
                    energyIconType = UserData.ResourceId.Infinity_Energy;
                }

                _goEnergyIcon.gameObject.SetActive(false);
                energyInfinity.SetActive(true);
                energyTimeBG.gameObject.SetActive(false);
                energyTimeText.gameObject.SetActive(false);
            }

            var str = DragonU3DSDK.Utils.GetTimeString("%mm:%ss", (int) (leftTime * 0.001f));
            resTexts[UserData.ResourceId.Energy].SetText(str);
            _energyShowType = EnergyShowType.Infinite;
        }
        else
        {
            energyInfinity.SetActive(false);
            if (_energyShowType != EnergyShowType.Times || _energyShowType != EnergyShowType.NextAddCountDown)
            {
                resTexts[UserData.ResourceId.Energy].gameObject.SetActive(true);

                if (energyIconType != UserData.ResourceId.Energy)
                {
                    iconDic[UserData.ResourceId.Energy].sprite =
                        UserData.GetResourceIcon(UserData.ResourceId.Energy, UserData.ResourceSubType.Big);
                    energyIconType = UserData.ResourceId.Energy;
                }
                // _goInfiniteEnergy.SetActive(false);
                // EventDispatcher.Instance.DispatchEvent(new EnergyChangedEvent(EnergyModel.Instance.IsEnergyUnlimited(), EnergyModel.Instance.EnergyNumber(), 0));
            }

            int currentCount = EnergyModel.Instance.EnergyNumber();
            if (currentCount >= EnergyModel.Instance.MaxEnergy())
            {
                _goEnergyIcon.gameObject.SetActive(false);
                energyTimeBG.gameObject.SetActive(false);
                energyTimeText.gameObject.SetActive(false);
            }
            else
            {
                _goEnergyIcon.gameObject.SetActive(true);
                energyTimeBG.gameObject.SetActive(true);
                energyTimeText.gameObject.SetActive(true);
                energyTimeText?.SetText(DragonU3DSDK.Utils.GetTimeString("%mm:%ss",
                    (int) (EnergyModel.Instance.LeftAutoAddEnergyTime() * 0.001)));
                _energyShowType = EnergyShowType.NextAddCountDown;
            }
        }
    }

    #endregion
    
    #region HappyGo体力
    private void HGUserEnergyUpdate(BaseEvent obj)
    {
        _HGUpdateInfiniteEnergy();
        resTexts[UserData.ResourceId.HappyGo_Energy].SetText(HappyGoEnergyModel.Instance.EnergyNumber().ToString());
    }

    void HGUpdateEnergyPerSecond()
    {
        if (!HappyGoModel.Instance.IsOpened())
            return;
        _HGUpdateInfiniteEnergy();
        if (this.gameObject.activeSelf)
            this.StartCoroutine(CommonUtils.DelayWork(1f, () => { HGUpdateEnergyPerSecond(); }));
    }

    private void _HGUpdateInfiniteEnergy()
    {
        if (!HappyGoModel.Instance.IsOpened())
            return;
        //刷新体力
        var leftTime = HappyGoEnergyModel.Instance.EnergyUnlimitedLeftShowTime();
        if (leftTime > 0)
        {
            if (_hgEnergyShowType != EnergyShowType.Infinite)
            {
                resTexts[UserData.ResourceId.HappyGo_Energy].gameObject.SetActive(false);

                if (energyIconType != UserData.ResourceId.HappyGo_Infinity_Energy)
                {
                    iconDic[UserData.ResourceId.HappyGo_Energy].sprite =
                        UserData.GetResourceIcon(UserData.ResourceId.HappyGo_Infinity_Energy, UserData.ResourceSubType.Big);
                    energyIconType = UserData.ResourceId.HappyGo_Infinity_Energy;
                }

                _hgGoEnergyIcon.gameObject.SetActive(false);
            }

            var str = DragonU3DSDK.Utils.GetTimeString("%mm:%ss", (int) (leftTime * 0.001f));
            hgEnergyTimeText?.SetText(str);
            _hgEnergyShowType = EnergyShowType.Infinite;
        }
        else
        {
            if (_hgEnergyShowType != EnergyShowType.Times || _hgEnergyShowType != EnergyShowType.NextAddCountDown)
            {
                resTexts[UserData.ResourceId.HappyGo_Energy].gameObject.SetActive(true);

                if (energyIconType != UserData.ResourceId.HappyGo_Energy)
                {
                    iconDic[UserData.ResourceId.HappyGo_Energy].sprite =
                        UserData.GetResourceIcon(UserData.ResourceId.HappyGo_Energy, UserData.ResourceSubType.Big);
                    energyIconType = UserData.ResourceId.HappyGo_Energy;
                }
                // _goInfiniteEnergy.SetActive(false);
                // EventDispatcher.Instance.DispatchEvent(new EnergyChangedEvent(EnergyModel.Instance.IsEnergyUnlimited(), EnergyModel.Instance.EnergyNumber(), 0));
            }

            int currentCount = HappyGoEnergyModel.Instance.EnergyNumber();
            if (currentCount >= HappyGoEnergyModel.Instance.MaxEnergy())
            {
                _hgGoEnergyIcon.gameObject.SetActive(false);
                _hgEnergyTimeBG.gameObject.SetActive(false);
                hgEnergyTimeText.gameObject.SetActive(false);
            }
            else
            {
                _hgGoEnergyIcon.gameObject.SetActive(true);
                _hgEnergyTimeBG.gameObject.SetActive(true);
                hgEnergyTimeText.gameObject.SetActive(true);
                hgEnergyTimeText?.SetText(DragonU3DSDK.Utils.GetTimeString("%mm:%ss",
                    (int) (HappyGoEnergyModel.Instance.LeftAutoAddEnergyTime() * 0.001)));
                _hgEnergyShowType = EnergyShowType.NextAddCountDown;
            }
        }
        
    }

    public void OnHappyGoEnergyAdd()
    {
        BuyResourceManager.Instance.TryShowBuyResource(UserData.ResourceId.HappyGo_Energy, "", "",
            "diamond_lack_energy");
    }
    public void SetEnergyShowType(bool isMain)
    {
        if (isMain)
        {
            resTexts[UserData.ResourceId.Energy].transform.parent.gameObject.SetActive(true);
            resTexts[UserData.ResourceId.HappyGo_Energy].transform.parent.gameObject.SetActive(false);
            _happyGoShop.gameObject.SetActive(false);
        }
        else
        {
            HGUpdateEnergyPerSecond();
            resTexts[UserData.ResourceId.Energy].transform.parent.gameObject.SetActive(false);
            resTexts[UserData.ResourceId.HappyGo_Energy].transform.parent.gameObject.SetActive(true);
            _happyGoShop.gameObject.SetActive(true);
        }
    }
    #endregion

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.UserDataUpdate, UserDataUpdate);
        //EventDispatcher.Instance.RemoveEventListener(EventEnum.OnIAPItemPaid, UserDataUpdate);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.EnergyChanged, UserEnergyUpdate);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HGEnergyChanged, HGUserEnergyUpdate);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.DAILYDEALS_PURCHASE_SUCCESS, OnDailyDealsPurchase);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.EventOpenBuyItemPopup,ShowBar3);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.EventCloseBuyItemPopup,HideBar3);
        //AnimControlManager.Instance.RemoveAnim(AnimKey.Main_ResBar);
    }

    public void HideShopButton()
    {
        goGemButton.gameObject.SetActive(false);
        goCoinButton.gameObject.SetActive(false);
        _goEnergyButton.gameObject.SetActive(false);
    }

    public override void UpdateCanvasSortOrder()
    {
        orgSortOrder = canvas.sortingOrder;
    }

    public void SetCanvasSortOrder(int order)
    {
        canvas.sortingOrder = order;
    }
    
    public void SetMultiGroupCanvasSortOrder(List<UserData.ResourceId> resourceIdList,int order)
    {
        foreach (var resId in resourceIdList)
        {
            if (resGroupTrans.TryGetValue(resId, out var group))
            {
                if (group == null)
                    continue;
                var groupCanvas = group.GetComponent<Canvas>();
                if (groupCanvas == null)
                {
                    groupCanvas = group.gameObject.AddComponent<Canvas>();
                    groupCanvas.gameObject.AddComponent<GraphicRaycaster>();
                }
                groupCanvas.overrideSorting = true;
                groupCanvas.sortingOrder = order;
            }
        }
    }

    public void RecoverMultiGroupCanvasSortOrder(List<UserData.ResourceId> resourceIdList)
    {
        foreach (var resId in resourceIdList)
        {
            if (resGroupTrans.TryGetValue(resId, out var group))
            {
                if (group == null)
                    continue;
                {
                    var component = group.gameObject.GetComponent<GraphicRaycaster>();
                    if (component != null)
                        DestroyImmediate(component);
                }
                {
                    var component = group.gameObject.GetComponent<Canvas>();
                    if (component != null)
                        DestroyImmediate(component);
                }
            }
        }
        Canvas.ForceUpdateCanvases();
    }
    public void SetCoinCanvasSortOrder(int order)
    {
        SetMultiGroupCanvasSortOrder(new List<UserData.ResourceId>() {UserData.ResourceId.Coin}, order);
    }
    public void RecoverCoinCanvasSortOrder()
    {
        RecoverMultiGroupCanvasSortOrder(new List<UserData.ResourceId>() {UserData.ResourceId.Coin});
    }
    public void SetDiamondCanvasSortOrder(int order)
    {
        SetMultiGroupCanvasSortOrder(new List<UserData.ResourceId>() {UserData.ResourceId.Diamond}, order);
    }
    public void RecoverDiamondCanvasSortOrder()
    {
        RecoverMultiGroupCanvasSortOrder(new List<UserData.ResourceId>() {UserData.ResourceId.Diamond});
    }

    public void RecoverCanvasSortOrder()
    {
        if (canvas == null)
            return;

        canvas.sortingOrder = orgSortOrder;
    }

    public void SetAddButtonsActive(bool active)
    {
        if (canvas == null)
            return;

        addbuttons?.ForEach(a => a.SetActive(active));
    }
    
    public void TriggerGuide()
    {
        if (_experience == null)
            return;

        _experience.TriggerGuide();
    }

    public void CheckLevelUp(Action callFunc)
    {
        _experience?.CheckLevelUp(callFunc);
    }
}