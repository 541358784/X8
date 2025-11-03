using DragonPlus;
using DragonPlus.Config.TMatch;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DragonU3DSDK.Asset;
using TMPro;
using DragonPlus.Config.TMatchShop;
using DragonU3DSDK.Storage;
using System;
using DragonU3DSDK.Network.API.Protocol;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UISettlementInterrupt")]
    public class UITMatchLevelInterruptController : UIPopup
    {
        public override Action EmptyCloseAction => CloseOnClick;

        private List<TMatchRewardItemData> loseItemDatas;
        private List<Transform> loseItems = new List<Transform>();

        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);

            TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchModel.Instance.GetMainLevel());
            CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);

            transform.Find($"Root/CloseButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/StayButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/QuitButton").GetComponent<Button>().onClick.AddListener(QuitOnClick);

            AddLoseItem();
        }

        private void CloseOnClick()
        {
            UIViewSystem.Instance.Close<UITMatchLevelInterruptController>();
        }

        private void QuitOnClick()
        {
            CloseOnClick();
            GameBIManager.Instance.LevelInfo.LeftTimeCount = (uint)TMatchSystem.LevelController.LevelData.LastTimes;
            GameBIManager.Instance.LevelInfo.LevelTime = GameBIManager.Instance.LevelInfo.TotalTime - (uint)TMatchSystem.LevelController.LevelData.LastTimes;
            GameBIManager.Instance.LevelInfo.LevelTime = (uint)TMatchSystem.LevelController.LevelData.LastTimes;
            GameBIManager.Instance.LevelInfo.LevelResult = "quit";
            GameBIManager.Instance.LevelInfo.FailReason = 3;
            GameBIManager.Instance.SendLevelInfoEvent();
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                .GameEventTmLevelEnd,data2:"0",data1: TMatchModel.Instance.GetMainLevel().ToString());
            UIViewSystem.Instance.Open<UITMatchFailController>();
            
            // if (RemoveAdModel.Instance.IsUnlock() && (!StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd.RemoveAd) && AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.TM_FINISH_FAIL_GAME))
            // {
            //     AdSubSystem.Instance.PlayInterstital(ADConstDefine.TM_FINISH_FAIL_GAME, b =>
            //     {
            //         RemoveAdModel.Instance.TryToAutoOpen();
            //     });
            // }
        }

        // 展示规则：
        //    必有：星星（无数量）。 
        //    动态1：体力图标，若玩家当前存在限时无限体力期间，则不显示。反之则显示1个体力损失； 
        //    动态2：玩家进入关卡前的连赢BUFF等级图标，分为:无，1级，2级，3级。对应主界面关卡上的帽子图标，没有数量显示
        //    动态3：活动收集物品、周挑战收集物品、公会收集物品 对应玩家本局已经临时收集到的类型和数量；
        private void AddLoseItem()
        {
            loseItemDatas = new List<TMatchRewardItemData>();
            loseItemDatas.Add(new TMatchRewardItemData(HospitalConst.TMatchAtlas, "ui_common_icon_stars", false));
            // 暂时固定添加一个体力显示，后续如果有了无限体力，则这里不展示体力
            if (!EnergyModel.Instance.GetEnterGameIsUnlimitedState()) // 条件判断当前是否无限体力
                loseItemDatas.Add(new TMatchRewardItemData(HospitalConst.TMatchAtlas, "ui_common_icon_live", true, 1));
            // 收集物品
            if (UITMatchMainCollectItemView.collectItems.Count > 0)
            {
                foreach (var dictItem in UITMatchMainCollectItemView.collectItems)
                {
                    if (dictItem.Value > 0)
                    {
                        var matchCfg = TMatchConfigManager.Instance.GetItem(dictItem.Key);
                        var cnt = dictItem.Value;
                        var cfg = TMatchShopConfigManager.Instance.GetItem(matchCfg.boosterId);
                        loseItemDatas.Add(new TMatchRewardItemData(HospitalConst.TMatchAtlas, cfg.pic_res, true, cnt));
                    }
                }
            }

            // 帽子
            if (TMatchGoldenHatterSystem.Instance.GoldenHatterMarkValue > 0)
            {
                string iconName = "";
                if (TMatchGoldenHatterSystem.Instance.GoldenHatterMarkValue == 1)
                {
                    iconName = "ui_common_level_pull_1";
                }
                else if (TMatchGoldenHatterSystem.Instance.GoldenHatterMarkValue == 2)
                {
                    iconName = "ui_common_level_pull_2";
                }
                else
                {
                    iconName = "ui_common_level_pull_3";
                }

                loseItemDatas.Add(new TMatchRewardItemData(HospitalConst.TMatchAtlas, iconName, false));
            }

            RefreshLoseItem();
        }

        private void RefreshLoseItem()
        {
            if (loseItems.Count > 0)
            {
                for (var i = 0; i < loseItems.Count; i++)
                {
                    GameObject.Destroy(loseItems[i]);
                }

                loseItems.Clear();
            }

            transform.Find($"Root/InsideGroup/Items1").gameObject.SetActive(loseItemDatas.Count > 4);
            if (loseItemDatas.Count > 4)
            {
                var root1 = transform.Find($"Root").GetComponent<RectTransform>();
                var size = root1.sizeDelta;
                size.y = 975;
                root1.sizeDelta = size;
            }

            var transItem = transform.Find($"Root/InsideGroup/Items/Items");
            transItem.gameObject.SetActive(false);
            for (var i = 0; i < loseItemDatas.Count; i++)
            {
                var parent = transItem.parent;
                if (i > 3)
                {
                    parent = transform.Find($"Root/InsideGroup/Items1");
                }

                var trans = GameObject.Instantiate(transItem, parent);
                trans.gameObject.SetActive(true);
                var icon = trans.Find("Icon").GetComponent<Image>();
                var data = loseItemDatas[i];
                icon.sprite = ResourcesManager.Instance.GetSpriteVariant(data.atlasName, data.iconName);
                var numText = trans.Find("NumberText").GetComponent<TextMeshProUGUI>();
                if (data.isShowNum)
                {
                    numText.text = data.num.ToString();
                }
                else
                {
                    numText.text = "";
                }

                loseItems.Add(trans);
            }

        }
    }
}