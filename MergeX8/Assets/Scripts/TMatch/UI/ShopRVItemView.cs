using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine.UI;


namespace TMatch
{
    public class ShopRVItemView : UIView
    {
        // [ComponentBinder("Icon")] private Image icon;
        // [ComponentBinder("TipsText")] private LocalizeTextMeshProUGUI tipsText;
        // [ComponentBinder("ADButton")] private Button buyButton;
        // [ComponentBinder("NumberText")] private LocalizeTextMeshProUGUI numberText;
        //
        // public ShopItemViewParam drivedParam;
        //
        // public override void OnViewOpen(UIViewParam param)
        // {
        //     base.OnViewOpen(param);
        //     drivedParam = param as ShopItemViewParam;
        //     Refresh(drivedParam);
        //     buyButton.onClick.AddListener(BuyOnClick);
        //     EventDispatcher.Instance.AddEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
        // }
        //
        // public override void OnViewDestroy()
        // {
        //     EventDispatcher.Instance.RemoveEventListener(EventEnum.IAPSuccess, OnIAPSuccessEvent);
        //     base.OnViewDestroy();
        // }
        //
        // public void Refresh(ShopItemViewParam param)
        // {
        //     drivedParam = param;
        //     icon.sprite = ResourcesManager.Instance.GetSpriteVariant(drivedParam.ItemData.shopCfg.Atlas, drivedParam.ItemData.shopCfg.Icon);
        //     tipsText.SetText($"{LocalizationManager.Instance.GetLocalizedString(drivedParam.ItemData.shopCfg.Desc)}\r\n{AdLogicManager.Instance.GetRewardRewardCnt(eAdReward.Shop)}");
        //     numberText.SetText(AdLogicManager.Instance.GetRewardLastCount(eAdReward.Shop).ToString());
        // }
        //
        // private void BuyOnClick()
        // {
        //     if (AdLogicManager.Instance.GetRewardLastCount(eAdReward.Shop) <= 0) return;
        //     
        //     if (!AdLogicManager.Instance.ShouldShowRV(eAdReward.Shop))
        //     {
        //         UIViewSystem.Instance.Open<NoticeController>(new NoticeUIData() { DescString = LocalizationManager.Instance.GetLocalizedString("UI_button_loading_ADS") });
        //         return;
        //     }
        //
        //     if (AdLogicManager.Instance.TryShowRewardedVideo(eAdReward.Shop, (b, s) =>
        //     {
        //         if (b)
        //         {
        //             List<ItemData> itemDatas = new List<ItemData>();
        //             ItemData itemData = new ItemData(){ id = drivedParam.ItemData.shopCfg.ItemId[0], cnt = AdLogicManager.Instance.GetRewardRewardCnt(eAdReward.Shop) };
        //             itemDatas.Add(itemData);
        //             CommonUtils.AddRewards(itemDatas, new DragonPlus.GameBIManager.ItemChangeReasonArgs(BiEventMatchFrenzy.Types.ItemChangeReason.Ads){ data1 = eAdReward.Shop.ToString() });
        //             
        //             EventDispatcher.Instance.DispatchEvent(new IAPSuccessEvent(drivedParam.ItemData.shopCfg, this));
        //         }
        //     }))
        //     {
        //         numberText.SetText(AdLogicManager.Instance.GetRewardLastCount(eAdReward.Shop).ToString());
        //     }
        // } 
        //
        // private void OnIAPSuccessEvent(BaseEvent evt)
        // {
        //     IAPSuccessEvent drivedEvt = evt as IAPSuccessEvent;
        //     if (drivedEvt.shop.Id == drivedParam.ItemData.shopCfg.Id && (drivedEvt.userData as UIView) == this)
        //     {
        //         int id = drivedEvt.shop.ItemId[0];
        //         int cnt = AdLogicManager.Instance.GetRewardRewardCnt(eAdReward.Shop);
        //         FlySystem.Instance.FlyItem(id, cnt, icon.transform.position, FlySystem.Instance.GetShopTargetTransform(id).position, null);
        //     }
        // }
    }
}