using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus.Config.TMatchShop;
using DragonPlus.Config.TMatch;
using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public partial class UITMatchMainController
    {
        private static Transform magnetTopView, broomTopView, windmillTopView, FrozenTopView;

        public static Transform GetMagentTopView()
        {
            return magnetTopView;
        }

        public static Transform GetBroomTopView()
        {
            return broomTopView;
        }

        public static Transform GetWindmillTopView()
        {
            return windmillTopView;
        }

        public static Transform GetFozenTopView()
        {
            return FrozenTopView;
        }

        public Transform GuideTipsCenter => transform.Find("Root/GuideTipsCenter");
        public Transform GuideFrame => transform.Find("Root/GuideFrame");

        private void InitBoost()
        {
            magnetTopView = transform.Find("Root/PropGroup/Prop/Viewport/Content/Adsorb");
            broomTopView = transform.Find("Root/PropGroup/Prop/Viewport/Content/Broom");
            windmillTopView = transform.Find("Root/PropGroup/Prop/Viewport/Content/Fan");
            FrozenTopView = transform.Find("Root/PropGroup/Prop/Viewport/Content/Frozen");

            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock1 || TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                transform.Find("Root/PropGroup/Prop/Viewport/Content/Adsorb").GetComponent<Button>().onClick.AddListener(MagnetOnClick);
                RefreshBoost(TMatchShopConfigManager.Instance.InnerBoostMagnetItemId);

                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchBoost, transform.Find("Root/PropGroup/Prop/Viewport/Content/Adsorb"), 
                //     TMatchShopConfigManager.Instance.InnerBoostMagnetItemId.ToString());
            }
            else
            {
                LockBoost(TMatchShopConfigManager.Instance.InnerBoostMagnetItemId);
            }

            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock2 || TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                transform.Find("Root/PropGroup/Prop/Viewport/Content/Broom").GetComponent<Button>().onClick.AddListener(BroomOnClick);
                RefreshBoost(TMatchShopConfigManager.Instance.InnerBoostBroomItemId);

                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchBoost, transform.Find("Root/PropGroup/Prop/Viewport/Content/Broom"), 
                //     TMatchShopConfigManager.Instance.InnerBoostBroomItemId.ToString());
            }
            else
            {
                LockBoost(TMatchShopConfigManager.Instance.InnerBoostBroomItemId);
            }

            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock3 || TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                transform.Find("Root/PropGroup/Prop/Viewport/Content/Fan").GetComponent<Button>().onClick.AddListener(WindmillOnClick);
                RefreshBoost(TMatchShopConfigManager.Instance.InnerBoostWindmillItemId);

                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchBoost, transform.Find("Root/PropGroup/Prop/Viewport/Content/Fan"), 
                //     TMatchShopConfigManager.Instance.InnerBoostWindmillItemId.ToString());
            }
            else
            {
                LockBoost(TMatchShopConfigManager.Instance.InnerBoostWindmillItemId);
            }

            if (TMatchModel.Instance.GetMainLevel() >= TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock4 || TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            {
                transform.Find("Root/PropGroup/Prop/Viewport/Content/Frozen").GetComponent<Button>().onClick.AddListener(FrozenOnClick);
                RefreshBoost(TMatchShopConfigManager.Instance.InnerBoostFrozenItemId);

                // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MatchBoost, transform.Find("Root/PropGroup/Prop/Viewport/Content/Frozen"), 
                //     TMatchShopConfigManager.Instance.InnerBoostFrozenItemId.ToString());
            }
            else
            {
                LockBoost(TMatchShopConfigManager.Instance.InnerBoostFrozenItemId);
            }

            EventDispatcher.Instance.AddEventListener(EventEnum.GAME_ITEM_CHANGE, OnGameItemChangeEvent);
        }

        private void DestoryBoost()
        {
            magnetTopView = null;
            broomTopView = null;
            windmillTopView = null;
            FrozenTopView = null;

            EventDispatcher.Instance.RemoveEventListener(EventEnum.GAME_ITEM_CHANGE, OnGameItemChangeEvent);
        }

        private void RefreshBoost(int itemId)
        {
            // DragonPlus.Config.Game.Item item = TMatch.TMatchShopConfigManager.Instance.GetItem(itemId);
            var item = ItemModel.Instance.GetConfigById(itemId);
            string typeName = "";
            if (item.GetResouceId() == ResourceId.TMMagnet) typeName = "Adsorb";
            else if (item.GetResouceId() == ResourceId.TMBroom) typeName = "Broom";
            else if (item.GetResouceId() == ResourceId.TMWindmill) typeName = "Fan";
            else if (item.GetResouceId() == ResourceId.TMFrozen) typeName = "Frozen";
            else return;
            GameObject node = transform.Find($"Root/PropGroup/Prop/Viewport/Content/{typeName}").gameObject;
            // int cnt = CurrencyModel.Instance.GetRes(item.GetResouceId());
            int cnt = ItemModel.Instance.GetNum(item.id);
            if (cnt == 0)
            {
                node.transform.Find("Normal/NumberText").gameObject.SetActive(false);
                node.transform.Find("Normal/BuyIcon").gameObject.SetActive(true);
            }
            else
            {
                node.transform.Find("Normal/NumberText").gameObject.SetActive(true);
                node.transform.Find("Normal/BuyIcon").gameObject.SetActive(false);

                node.transform.Find("Normal/NumberText").GetComponent<TextMeshProUGUI>().SetText(cnt.ToString());
            }
        }

        private void LockBoost(int itemId)
        {
            // DragonPlus.Config.Game.Item item = TMatch.TMatchShopConfigManager.Instance.GetItem(itemId);
            var item = ItemModel.Instance.GetConfigById(itemId);
            string typeName = "";
            int unlockValue = 1;
            if (item.GetResouceId() == ResourceId.TMMagnet)
            {
                typeName = "Adsorb";
                unlockValue = TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock1;
            }
            else if (item.GetResouceId() == ResourceId.TMBroom)
            {
                typeName = "Broom";
                unlockValue = TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock2;
            }
            else if (item.GetResouceId() == ResourceId.TMWindmill)
            {
                typeName = "Fan";
                unlockValue = TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock3;
            }
            else if (item.GetResouceId() == ResourceId.TMFrozen)
            {
                typeName = "Frozen";
                unlockValue = TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock4;
            }
            else return;

            transform.Find($"Root/PropGroup/Prop/Viewport/Content/{typeName}/Normal").gameObject.SetActive(false);
            transform.Find($"Root/PropGroup/Prop/Viewport/Content/{typeName}/Lock").gameObject.SetActive(true);
            transform.Find($"Root/PropGroup/Prop/Viewport/Content/{typeName}/Lock/Text").GetComponent<TextMeshProUGUI>().SetText($"Lv.{unlockValue}");
        }

        private void OnGameItemChangeEvent(BaseEvent evt)
        {
            GameItemChangeEvent realEvt = evt as GameItemChangeEvent;
            RefreshBoost(realEvt.itemId);
        }

        public void GuideClickMagnet()
        {
            MagnetOnClick();
        }

        //磁铁
        private void MagnetOnClick()
        {
            // DragonPlus.Config.Game.Item item = TMatch.TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.InnerBoostMagnetItemId);
            var item = ItemModel.Instance.GetConfigById(TMatchShopConfigManager.Instance.InnerBoostMagnetItemId);
            if (!ItemModel.Instance.IsEnough(item.id))
                // if (!CurrencyModel.Instance.CanAford(item.GetResouceId(), 1))
            {
                if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
                {
                    UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage);
                }
                else
                {
                    UIViewSystem.Instance.Open<UITMatchBuyProps>(new UITMatchBuyPropsData() { inItem = item });   
                }
                return;
            }

            if (!TMatchBoostSystem.Instance.CanUseMagnetIgnoreInterval(item))
            {
                EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_LOW_SPACE_TIP);
                return;
            }

            EventDispatcher.Instance.DispatchEventImmediately(new GameItemUseEvent(TMatchShopConfigManager.Instance.InnerBoostMagnetItemId));
            // if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            // {
            //     (TMatchSystem.LevelController as TMatchKapibalaLevelController).CostProp(
            //         TMatchShopConfigManager.Instance.InnerBoostMagnetItemId, 1);
            // }
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchBoost);
        }

        public void GuideClickBroom()
        {
            BroomOnClick();
        }

        //扫把
        private void BroomOnClick()
        {
            // DragonPlus.Config.Game.Item item = TMatch.TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.InnerBoostBroomItemId);
            var item = ItemModel.Instance.GetConfigById(TMatchShopConfigManager.Instance.InnerBoostBroomItemId);
            if (!ItemModel.Instance.IsEnough(item.id))
                // if (!CurrencyModel.Instance.CanAford(item.GetResouceId(), 1))
            {
                if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
                {
                    UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage);
                }
                else
                {
                    UIViewSystem.Instance.Open<UITMatchBuyProps>(new UITMatchBuyPropsData() { inItem = item });   
                }
                return;
            }

            if (!TMatchBoostSystem.Instance.CanUseBroom(item)) return;

            EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(TMatchShopConfigManager.Instance.InnerBoostBroomItemId));
            // if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            // {
            //     (TMatchSystem.LevelController as TMatchKapibalaLevelController).CostProp(
            //         TMatchShopConfigManager.Instance.InnerBoostBroomItemId, 1);
            // }
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchBoost);
        }

        public void GuideClickWindmill()
        {
            WindmillOnClick();
        }

        //风车
        private void WindmillOnClick()
        {
            // DragonPlus.Config.Game.Item item = TMatch.TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.InnerBoostWindmillItemId);
            var item = ItemModel.Instance.GetConfigById(TMatchShopConfigManager.Instance.InnerBoostWindmillItemId);
            if (!ItemModel.Instance.IsEnough(item.id))
                // if (!CurrencyModel.Instance.CanAford(item.GetResouceId(), 1))
            {
                if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
                {
                    UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage);
                }
                else
                {
                    UIViewSystem.Instance.Open<UITMatchBuyProps>(new UITMatchBuyPropsData() { inItem = item });   
                }
                return;
            }

            if (!TMatchBoostSystem.Instance.CanUseWindmill(item)) return;

            EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(TMatchShopConfigManager.Instance.InnerBoostWindmillItemId));
            // if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            // {
            //     (TMatchSystem.LevelController as TMatchKapibalaLevelController).CostProp(
            //         TMatchShopConfigManager.Instance.InnerBoostWindmillItemId, 1);
            // }
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchBoost);
        }

        public void GuideClickFrozen()
        {
            FrozenOnClick();
        }

        //冰冻
        private void FrozenOnClick()
        {
            // DragonPlus.Config.Game.Item item = TMatch.TMatchShopConfigManager.Instance.GetItem(TMatchShopConfigManager.Instance.InnerBoostFrozenItemId);
            var item = ItemModel.Instance.GetConfigById(TMatchShopConfigManager.Instance.InnerBoostFrozenItemId);
            if (!ItemModel.Instance.IsEnough(item.id))
                // if (!CurrencyModel.Instance.CanAford(item.GetResouceId(), 1))
            {
                if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
                {
                    UIPopupKapibalaGiftBagController.Open(KapibalaModel.Instance.Storage);
                }
                else
                {
                    UIViewSystem.Instance.Open<UITMatchBuyProps>(new UITMatchBuyPropsData() { inItem = item });   
                }
                return;
            }

            if (!TMatchBoostSystem.Instance.CanUseFrozen(item)) return;

            EventDispatcher.Instance.DispatchEvent(new GameItemUseEvent(TMatchShopConfigManager.Instance.InnerBoostFrozenItemId));
            // if (TMatchSystem.LevelController.GameType == TMGameType.Kapibala)
            // {
            //     (TMatchSystem.LevelController as TMatchKapibalaLevelController).CostProp(
            //         TMatchShopConfigManager.Instance.InnerBoostFrozenItemId, 1);
            // }
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchBoost);
        }

        public void SetMagnetEffectState(bool active)
        {
            if (active)
            {
                transform.Find("Root/VFX_Adsorb").gameObject.SetActive(true);
                transform.Find("Root/VFX_Adsorb").GetComponent<Animator>().Play("adsorb");
            }
            else
            {
                transform.Find("Root/VFX_Adsorb").GetComponent<Animator>().Play("normal");
                transform.Find("Root/VFX_Adsorb").gameObject.SetActive(false);
            }
        }

        public void SetBroomEffectState(bool active)
        {
            if (active)
            {
                Vector3 upScreenPoint = CameraManager.MainCamera.WorldToScreenPoint(TMatchEnvSystem.Instance.CollectorPos[3]);
                Vector3 upWorldPos = CameraManager.UICamera.ScreenToWorldPoint(upScreenPoint);
                transform.Find("Root/VFX_Broom").gameObject.transform.position = upWorldPos;
                transform.Find("Root/VFX_Broom").gameObject.transform.localPosition += new Vector3(0.0f, -235.0f, 0.0f);
                transform.Find("Root/VFX_Broom").gameObject.SetActive(true);
                transform.Find("Root/VFX_Broom").GetComponent<Animator>().Play("broom");
            }
            else
            {
                transform.Find("Root/VFX_Broom").GetComponent<Animator>().Play("normal");
                transform.Find("Root/VFX_Broom").gameObject.SetActive(false);
            }
        }

        public void SetWindmillEffectState(bool active)
        {
            if (active)
            {
                transform.Find("Root/VFX_Fan").gameObject.SetActive(true);
                transform.Find("Root/VFX_Fan").GetComponent<Animator>().Play("fan");
            }
            else
            {
                transform.Find("Root/VFX_Fan").GetComponent<Animator>().Play("normal");
                transform.Find("Root/VFX_Fan").gameObject.SetActive(false);
            }
        }
    }
}