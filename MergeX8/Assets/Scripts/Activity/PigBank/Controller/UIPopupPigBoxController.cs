using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using Gameplay;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupPigBoxController : UIWindowController
{
    private Canvas _canvas;

    private GameObject _pigBankPrefab;

    private LocalizeTextMeshProUGUI _collectValue;
    private LocalizeTextMeshProUGUI _countDown;
    private LocalizeTextMeshProUGUI _stage_1Text;
    private LocalizeTextMeshProUGUI _stage_2Text;  
    private Transform _stage1Finish;
    private Transform _stage2Finish;
    private Text _buyPriceText;

    private Image _sliderImage;

    private Button _helpButton;
    private Button _buyButton;
    private Button _closeButton;

    private PigBank _curPigBankTable;

    private float _sliderWidth = 0;
    private Transform _rewardItem1;

    private Image _boxImage;

    private GameObject _pigEffect;
    private int _curPigIndex = -1;

    private Animator _animator;

    public override void PrivateAwake()
    {
        PigBankModel.Instance.UpdateActivityState();
        _animator = transform.GetComponent<Animator>();
        
        _collectValue = transform.Find("Root/BubbleGroup/RewardTextGroup/Text")
            .GetComponent<LocalizeTextMeshProUGUI>();
        _countDown = transform.Find("Root/TimeGroup/TimeText")
            .GetComponent<LocalizeTextMeshProUGUI>();
        _stage_1Text = transform.Find("Root/Slider/RewardItem1/Text")
            .GetComponent<LocalizeTextMeshProUGUI>();
        _stage_2Text = transform.Find("Root/Slider/RewardItem2/Text")
            .GetComponent<LocalizeTextMeshProUGUI>();
        _buyPriceText = transform.Find("Root/ButtonBuy/Text").GetComponent<Text>();
        
        _stage1Finish = transform.Find("Root/Slider/RewardItem1/Finish");
        _stage2Finish = transform.Find("Root/Slider/RewardItem2/Finish");
        
        _boxImage = transform.Find("Root/StrongboxImage").GetComponent<Image>();

        _sliderImage = transform.Find("Root/Slider/Fill").GetComponent<Image>();

        _helpButton = transform.Find("Root/HelpButton").GetComponent<Button>();
        _helpButton.onClick.AddListener(OnClick_Help);

        _buyButton = transform.Find("Root/ButtonBuy").GetComponent<Button>();
        _buyButton.onClick.AddListener(OnClick_Buy);

        _closeButton = transform.Find("Root/CloseButton").GetComponent<Button>();
        _closeButton.onClick.AddListener(OnClick_Close);

        InvokeRepeating("UpdateCountDownTime", 0, 1);

        _sliderWidth = transform.Find("Root/Slider/Fill").GetComponent<RectTransform>()
            .sizeDelta.x;
        _sliderWidth = _sliderWidth / 2;
        _rewardItem1 = transform.Find("Root/Slider/RewardItem1");

        _pigEffect = transform.Find("Root/StrongboxImage/vfx_coin_stars").gameObject;
        _pigEffect.gameObject.SetActive(false);
        UpdateView();
       
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_PURCHASE_REFRESH, PurchaseRefresh);
        EventDispatcher.Instance.AddEventListener(EventEnum.PIGBANK_UI_REFRESH, UIRefresh);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_PURCHASE_REFRESH, PurchaseRefresh);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.PIGBANK_UI_REFRESH, UIRefresh);
    }


    private void UpdateView()
    {
        _curPigBankTable = PigBankModel.Instance.GetAdPigBankTable();
        if (_curPigBankTable != null)
        {
            _stage_1Text.SetText(_curPigBankTable.Stage_1.ToString());
            _stage_2Text.SetText(_curPigBankTable.Stage_2.ToString());
            _stage1Finish.gameObject.SetActive(PigBankModel.Instance.GetCurCollectValue() >= _curPigBankTable.Stage_1);
            _stage2Finish.gameObject.SetActive(PigBankModel.Instance.GetCurCollectValue() >= _curPigBankTable.Stage_2);
            if (_sliderImage != null)
            {
                int curValue = Math.Min(PigBankModel.Instance.GetCurCollectValue(), _curPigBankTable.Stage_2);

                _sliderImage.fillAmount = 1.0f * curValue / _curPigBankTable.Stage_2;

                float rate = 1.0f * _curPigBankTable.Stage_1 / _curPigBankTable.Stage_2;
                float posX = rate < 0.5f ? (-rate * _sliderWidth) : (rate - 0.5f) * _sliderWidth;

                _rewardItem1.transform.localPosition = new Vector3(posX, _rewardItem1.transform.localPosition.y,
                    _rewardItem1.transform.localPosition.z);


                if (_collectValue != null)
                {
                    if (PigBankModel.Instance.GetCurCollectValue() < _curPigBankTable.Stage_2)
                        _collectValue.SetText(PigBankModel.Instance.GetCurCollectValue().ToString());
                    else
                        _collectValue.SetTerm("UI_store_energy_full_text");
                }
            }

            _buyButton.interactable = PigBankModel.Instance.GetCurCollectValue() >= _curPigBankTable.Stage_1;

            string imageName = PigBankModel.Instance._boxImageName[0];
            int index = PigBankModel.Instance.GetCurIndex();
            imageName = PigBankModel.Instance._boxImageName[index];

            if (_curPigIndex > 0 && _curPigIndex != index)
                _pigEffect.gameObject.SetActive(true);

            _curPigIndex = index;

            if (_boxImage.sprite.name != imageName)
            {
                _boxImage.sprite = ResourcesManager.Instance.GetSpriteVariant("PigBoxAtlas", imageName);
            }
        }

        if (_buyPriceText != null && _curPigBankTable != null)
        {
            TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(_curPigBankTable.ShopId);
            if (tableShop != null)
            {
                _buyPriceText.text = StoreModel.Instance.GetPrice(tableShop.id);
            }
            
            transform.Find("Root/Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_curPigBankTable.ShopId));
        }
    }

    private void OnClick_Help()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPigBoxHelp);
    }    
    private void OnClick_Close()
    {
        OnCloseAnim();
    }

    private void OnCloseAnim()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, "disappear", "", () =>
            {
                CloseWindowWithinUIMgr(true);
            }
        ));
        
    }
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseAnim();
    }
    
    private void OnClick_Buy()
    {
        if (_curPigBankTable == null)
            return;

        if (PigBankModel.Instance.GetCurCollectValue() < _curPigBankTable.Stage_1)
            return;

        Dictionary<string, string> extras = new Dictionary<string, string>();
        extras.Add("pigIndex", PigBankModel.Instance.GetCurIndex().ToString());
        extras.Add("pigShopId", _curPigBankTable.ShopId.ToString());
        extras.Add("diamond", _curPigBankTable.Stage_2.ToString());
        int collectValue = Math.Min(PigBankModel.Instance.GetCurCollectValue(), _curPigBankTable.Stage_2);
        extras.Add("collectDiamond", collectValue.ToString());

        GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventPiggyBankBuy, null, null, null,
            extras);
        StoreModel.Instance.Purchase(_curPigBankTable.ShopId, "PigBank");
    }

    private void UpdateCountDownTime()
    {
        if (_countDown == null)
            return;

        _countDown.SetText(PigBankModel.Instance.GetActivityLeftTimeString());
    }

    private void PurchaseRefresh(BaseEvent e)
    {
        if (e == null || e.datas == null || e.datas.Length < 2)
            return;

        if (!PigBankModel.Instance.IsOpened())
        {
            UIManager.Instance.CloseUI(UINameConst.UIPopupPigBox, true);
            return;
        }

        int index = (int) e.datas[0];
        TableShop tableShop = (TableShop) e.datas[1];
        if (tableShop.id != _curPigBankTable.ShopId)
            return;

        UpdateView();
    }

    private void UIRefresh(BaseEvent e)
    {
        UpdateView();
    }
}