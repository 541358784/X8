using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupShopRvController : UIWindowController
{
    private Button _buttonClose;
    private Button _buttonAd;

    private CommonRewardItem _rewardItem;

    private Transform _inTag;

    
    private bool _canUseActiveIn = false;
    private string _activeInPlaceId;

    public override void PrivateAwake()
    {
        _buttonClose = transform.Find("Root/ButtonClose").GetComponent<Button>();
        _buttonAd = transform.Find("Root/ItemGroup/WatchButton").GetComponent<Button>();

        _inTag = transform.Find("Root/ItemGroup/WatchButton/Tip");

        
        _buttonClose.onClick.AddListener((() => AnimCloseWindow()));

        _rewardItem = transform.Find("Root/ItemGroup/Item").gameObject.AddComponent<CommonRewardItem>();
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _canUseActiveIn = (bool)objs[0];
        _activeInPlaceId = (string)objs[1];
        _inTag.gameObject.SetActive(_canUseActiveIn);
        
        var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, ADConstDefine.RV_SHOP_SOURCE);
        if (rv != null && rv.Bonus > 0)
        {
            var bs = AdConfigHandle.Instance.GetBonus(rv.Bonus, true);
            if (bs != null)
            {
                _rewardItem.Init(bs[0]);
            }
        }
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip, AdLocalOperate.Open);

        if (_canUseActiveIn)
        {
            _buttonAd.onClick.AddListener((() =>
            {
                AdSubSystem.Instance.PlayInterstital(_activeInPlaceId, (b =>
                        {
                            if (b)
                            {
                                AdSubSystem.Instance.ActiveInToRvGet(ADConstDefine.RV_SHOP_SOURCE);
                                PopReward();
                            }
                        }
                    ));
            }));
        }
        else
        {
            UIAdRewardButton.Create(ADConstDefine.RV_SHOP_SOURCE, UIAdRewardButton.ButtonStyle.Disable,
                _buttonAd.gameObject,
                (s) =>
                {
                    if (s)
                        PopReward();
                }
                , true);
        }
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip, AdLocalOperate.Close,AdLocalSkipScene.ShopSource);
    }


    private void PopReward()
    {
        var rv = AdConfigHandle.Instance.GetRvAd(UserGroupManager.Instance.SubUserGroup, ADConstDefine.RV_SHOP_SOURCE);
        if (rv != null && rv.Bonus > 0)
        {
            var bs = AdConfigHandle.Instance.GetBonus(rv.Bonus, true);
            if (bs != null)
            {
                CommonRewardManager.Instance.PopCommonReward(bs,
                    CurrencyGroupManager.Instance.GetCurrencyUseController(), true,
                    new GameBIManager.ItemChangeReasonArgs()
                    {
                        reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Adv,
                        data2 = _canUseActiveIn ? rv.ActiveInterAdsPlaceId : ADConstDefine.RV_SHOP_SOURCE,
                    });
            }
        }

        AdLocalConfigHandle.Instance.RefreshSceneOperate(AdLocalOperateScene.Skip, AdLocalOperate.Operate);
        CloseWindowWithinUIMgr(true);
    }
}