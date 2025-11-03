//
// using System;
// using DragonPlus;
// using DragonU3DSDK.Network.API.Protocol;
// using Gameplay;
// using UnityEngine.UI;
//
// namespace Activity.BattlePass_2
// {
//     public class UIPopupBattlePassBuyController : UIWindowController
//     {
//         private Button _buttonClose;
//         private Button _buttonBuy;
//         private Text _priceText;
//         private Image _item1Image;
//         private LocalizeTextMeshProUGUI _item1Text;
//         private TableShop _shop;
//
//         public override void PrivateAwake()
//         {
//             _buttonClose = GetItem<Button>("Root/ButtonClose");
//             _buttonClose.onClick.AddListener(OnClose);
//             _buttonBuy = GetItem<Button>("Root/Button");
//             _priceText = GetItem<Text>("Root/Button/Text");
//             _buttonBuy.onClick.AddListener(OnBtnBuy);
//             _item1Image = GetItem<Image>("Root/ItemGroup/Item1");
//             _item1Text = GetItem<LocalizeTextMeshProUGUI>("Root/ItemGroup/Item1/Text");
//             InitUI();
//             EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, OnCloseUI);
//
//         }
//
//         protected override void OnOpenWindow(params object[] objs)
//         {
//             base.OnOpenWindow(objs);
//             int openSrc = 1;
//             if (objs != null && objs.Length > 0)
//                 openSrc = (int)objs[0];
//             GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpBuyBuy, BattlePassModel.Instance.storageBattlePass.PurchasePopCount.ToString());
//             GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpBuyPop, openSrc.ToString());
//             BattlePassModel.Instance.storageBattlePass.PurchasePopCount++;
//         }
//
//
//         private void OnClose()
//         {
//             AnimCloseWindow();
//         }
//
//         private void OnCloseUI(BaseEvent e)
//         {
//             OnClose();
//         }
//
//         public void InitUI()
//         {
//             var config = BattlePassModel.Instance.BattlePassActiveConfig;
//             _shop = GlobalConfigManager.Instance.GetTableShopByID(config.shopItemId);
//             _item1Image.sprite = UserData.GetResourceIcon(config.buyReward[0]);
//             _item1Text.SetText(config.buyReward[1].ToString());
//             _priceText.text = StoreModel.Instance.GetPrice(_shop.id);
//         }
//
//         private void OnBtnBuy()
//         {
//             StoreModel.Instance.Purchase(_shop.id, "battlePass");
//         }
//
//         private void OnDestroy()
//         {
//             EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, OnCloseUI);
//
//         }
//
//         public static bool CanShowUIWithOpenWindow()
//         {
//             if (!CanShowUIWithOutOpenWindow())
//                 return false;
//             UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2Buy);
//             return true;
//         }
//
//         public static bool CanShowUIWithOutOpenWindow()
//         {
//             if (!BattlePassModel.Instance.IsOpened())
//                 return false;
//             if (BattlePassModel.Instance.IsPurchase())
//                 return false;
//             return true;
//         }
//
//         public static bool CanShowUI()
//         {
//             if (!CanShowUIWithOutOpenWindow())
//                 return false;
//             UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePass2Buy);
//             return true;
//         }
//     }
// }
