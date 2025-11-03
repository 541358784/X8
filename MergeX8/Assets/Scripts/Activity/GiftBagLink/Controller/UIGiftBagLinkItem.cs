using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using Framework;
using Gameplay.UI.Store.Vip.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIGiftBagLinkItem : MonoBehaviour
{
    private int _index;
    private GiftBagLinkResource _giftBagLinkResource;

    private Button getButton;
    private LocalizeTextMeshProUGUI getButtonText;

    private GameObject bigGroup;
    private GameObject smallGroup;

    private List<UIGiftBagRewardCell> _rewardCells = new List<UIGiftBagRewardCell>();

    private GameObject lockObj;

    private GameObject lockBgObj;
    private GameObject normalBgObj;

    public Animator _commonAnimator;
    public Animator _buttonAnimator;
    public Animator _lockAnimator;
    public Animator _bgAnimator;

    private bool _isUnlock = false;

    public int index
    {
        get { return _index; }
    }

    public GiftBagLinkResource giftBagLinkResource
    {
        get { return _giftBagLinkResource; }
    }

    public int ConsumeType
    {
        get { return _giftBagLinkResource.ConsumeType; }
    }

    private void Awake()
    {
        _commonAnimator = gameObject.GetComponent<Animator>();
        _bgAnimator = transform.Find("BGGroup").GetComponent<Animator>();
        ;
        _buttonAnimator = transform.Find("FreeButton/Button").GetComponent<Animator>();
        ;
        _lockAnimator = transform.Find("FreeButton/LockIcon").GetComponent<Animator>();
        ;

        lockObj = transform.Find("FreeButton/LockIcon").gameObject;

        bigGroup = transform.Find("RewardGroup/LittleGroup").gameObject;
        smallGroup = transform.Find("RewardGroup/MoreGroup").gameObject;

        getButton = transform.Find("FreeButton").GetComponent<Button>();
        getButton.onClick.AddListener(GetRewardClick);
        getButtonText = transform.Find("FreeButton/Button/BG/Text").GetComponent<LocalizeTextMeshProUGUI>();

        lockObj = transform.Find("FreeButton/LockIcon").gameObject;

        normalBgObj = transform.Find("BGGroup/YellowBG").gameObject;
        lockBgObj = transform.Find("BGGroup/BlueBG").gameObject;

        UIGiftBagRewardCell bigCellSp = bigGroup.transform.Find("Item1").gameObject.AddComponent<UIGiftBagRewardCell>();
        _rewardCells.Add(bigCellSp);

        for (int i = 1; i <= 3; i++)
        {
            UIGiftBagRewardCell smallCellSp =
                smallGroup.transform.Find("Item" + i).gameObject.AddComponent<UIGiftBagRewardCell>();
            _rewardCells.Add(smallCellSp);
        }

        bigGroup.gameObject.SetActive(false);
        smallGroup.gameObject.SetActive(false);
    }

    public void UpdateIndex(int index)
    {
        _index = index;
    }

    public void UpdateData(GiftBagLinkResource data, int index)
    {
        _index = index;
        _giftBagLinkResource = data;

        bigGroup.gameObject.SetActive(false);
        smallGroup.gameObject.SetActive(false);
        getButtonText.SetText("");

        if (data.RewardID == null || data.RewardID.Count == 0)
            return;

        UpdateStatus(GiftBagLinkModel.Instance.GetCurIndex() == _index);
        UpdateGetButtonText();

        if (data.RewardID.Count == 1)
        {
            bigGroup.gameObject.SetActive(true);
            _rewardCells[0].InitData(data.RewardID[0], data.Amount[0]);
            return;
        }

        smallGroup.gameObject.SetActive(true);
        for (int i = 1; i <= 3; i++)
        {
            _rewardCells[i].gameObject.SetActive(false);
            if (i > data.RewardID.Count)
                continue;

            _rewardCells[i].gameObject.SetActive(true);
            _rewardCells[i].InitData(data.RewardID[i - 1], data.Amount[i - 1]);
        }
    }

    public void UpdateStatus(bool isUnLock)
    {
        _isUnlock = isUnLock;

        foreach (var kv in _rewardCells)
        {
            kv.UpdateStatus(isUnLock);
        }

        lockObj.gameObject.SetActive(!isUnLock);
        lockBgObj.gameObject.SetActive(!isUnLock);
        normalBgObj.gameObject.SetActive(isUnLock);
    }

    private void UpdateGetButtonText()
    {
        if (_giftBagLinkResource == null)
            return;

        switch (_giftBagLinkResource.ConsumeType)
        {
            case 1:
            {
                getButtonText.SetText(LocalizationManager.Instance.GetLocalizedString("button_free"));
                transform.Find("Vip").gameObject.SetActive(false);
                break;
            }
            case 2:
            {
                TableShop tableShop = GlobalConfigManager.Instance.GetTableShopByID(_giftBagLinkResource.ConsumeAmount);
                if (tableShop != null)
                    getButtonText.SetText(StoreModel.Instance.GetPrice(tableShop.id));
                
                transform.Find("Vip").gameObject.SetActive(true);
                transform.Find("Vip/Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(VipStoreModel.Instance.GetVipScoreString(_giftBagLinkResource.ConsumeAmount));

                break;
            }
            default:
            {
                getButtonText.SetText("");
                transform.Find("Vip").gameObject.SetActive(false);
                break;
            }
        }
    }

    private void GetRewardClick()
    {
        if (!_isUnlock)
            return;

        EventDispatcher.Instance.DispatchEvent(EventEnum.GIFTBAGLINK_GET_REWARD, this);
    }

    public void CompleteAnim()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_buttonAnimator, "appear", "",
            () => { _commonAnimator.Play("disappear", 0, 0); }));
    }

    public void ChangeAnim()
    {
        lockBgObj.gameObject.SetActive(true);
        normalBgObj.gameObject.SetActive(true);
        StartCoroutine(CommonUtils.PlayAnimation(_bgAnimator, "appear01", "", () =>
            {
                foreach (var kv in _rewardCells)
                {
                    kv.UpdateStatus(true);
                }
            }
        ));
    }

    public void UnLockAnim()
    {
        _lockAnimator.Play("appear", 0, 0);
    }
}