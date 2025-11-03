using DragonPlus.Config.TMatch;
using Framework;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.TMatchShop;
using Gameplay;
using Newtonsoft.Json;


namespace TMatch
{
    [AssetAddress("TMatch/Prefabs/UISettlementSuccess")]
    public class UITMatchWinController : UIPopup
    {
        public override string CloseAnimStateName => "";
        public override Action EmptyCloseAction => null;
        private List<TMatchRewardItemData> rewardList;

        private void RefreshStarShow()
        {
            var starNum = StarCurrencyModel.Instance.GetStarCnt(TMatchSystem.LevelController.LevelData.layoutCfg.levelTimes, TMatchSystem.LevelController.LevelData.LastTimes);

            transform.Find($"Root/StarsGroup/Stars2/Icon").gameObject.SetActive(starNum > 1);
            transform.Find($"Root/StarsGroup/Stars3/Icon").gameObject.SetActive(starNum > 2);
            transform.Find($"Root/Icon2Effect").gameObject.SetActive(starNum > 1);
            transform.Find($"Root/Icon3Effect").gameObject.SetActive(starNum > 2);
        }

        // 物品显示：
        // 动态显示：限时活动收集物品、周挑战收集物品、公会收集物品 对应玩家本局已经临时收集到的类型和数量；收集的活动元素类型和数量
        private void RefreshGetRewardShow()
        {
            var transItem = transform.Find($"Root/InsideGroup/Items/Items");
            transItem.gameObject.SetActive(false);
            GetRewardList();

            // 没有奖励列表时，倒计时的显示居中
            if (rewardList.Count == 0)
            {
                var timeGroup = transform.Find($"Root/InsideGroup/Time");
                timeGroup.localPosition = new Vector3(0, 0, 0);
                return;
            }

            transform.Find($"Root/InsideGroup/Items1").gameObject.SetActive(rewardList.Count > 4);
            if (rewardList.Count > 4)
            {
                var rectRoot = transform.Find($"Root").GetComponent<RectTransform>();
                var size = rectRoot.sizeDelta;
                size.y = 1171;
                rectRoot.sizeDelta = size;
            }

            for (var i = 0; i < rewardList.Count; i++)
            {
                var parent = transItem.parent;
                if (i > 3)
                {
                    parent = transform.Find($"Root/InsideGroup/Items1");
                }

                var trans = GameObject.Instantiate(transItem, parent);
                trans.gameObject.SetActive(true);
                var icon = trans.Find("Icon").GetComponent<Image>();
                var data = rewardList[i];
                if (data.sprite)
                {
                    icon.sprite = data.sprite;
                }
                else
                {
                    icon.sprite = ResourcesManager.Instance.GetSpriteVariant(data.atlasName, data.iconName);   
                }
                var numText = trans.Find("NumberText").GetComponent<TextMeshProUGUI>();
                if (data.isShowNum)
                {
                    numText.text = data.num.ToString();
                }
                else
                {
                    numText.text = "";
                }
            }
        }

        private void GetRewardList()
        {
            rewardList = new List<TMatchRewardItemData>();

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
                        rewardList.Add(new TMatchRewardItemData(HospitalConst.TMatchAtlas, cfg.pic_res, true, cnt));
                    }
                }
            }
            int bpExp = TMBPModel.Instance.GetLastGameWinExp();
            if (bpExp > 0)
            {
                rewardList.Add(new TMatchRewardItemData(HospitalConst.TMatchAtlas, "TM_Common_Key", true,bpExp));
            }

            var winPrize = TMWinPrizeModel.Instance.GetLastWinReward();
            if (winPrize != null && winPrize.Count > 0)
            {
                foreach (var resData in winPrize)
                {
                    rewardList.Add(new TMatchRewardItemData(null, null, true,resData.count,UserData.GetResourceIcon(resData.id)));
                }
            }
        }

        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);

            var starNum = StarCurrencyModel.Instance.GetStarCnt(TMatchSystem.LevelController.LevelData.layoutCfg.levelTimes, TMatchSystem.LevelController.LevelData.LastTimes);
            AudioSysManager.Instance.PlaySound(starNum == 1 ? SfxNameConst.Yx_Success1 : starNum == 2 ? SfxNameConst.Yx_Success2 : SfxNameConst.Yx_Success3);
            // DragonPlus.GameBIManager.SendGameCompleteBoardShowEvent(TMatchSystem.LevelController.LevelData.level);

            TMatchDifficulty difficulty = TMatchConfigManager.Instance.GetDifficulty(TMatchSystem.LevelController.LevelData.level);
            CommonUtils.TMatchRefreshImageByDifficulty(transform, difficulty);

            transform.Find($"Root/PlayButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            transform.Find($"Root/CloseButton").GetComponent<Button>().onClick.AddListener(CloseOnClick);
            // 时间显示
            var leftTime = (int)TMatchSystem.LevelController.LevelData.LastTimes;
            transform.Find($"Root/InsideGroup/Time/TimeText").GetComponent<TextMeshProUGUI>()
                .SetText(DragonU3DSDK.Utils.GetTimeString("%mm:%ss", leftTime));

            // 星星数量显示
            RefreshStarShow();
            RefreshGetRewardShow();

            //做本地存储，那么即使杀进程再次回到大厅也可以将资源加上
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            string jsonData = JsonConvert.SerializeObject(TMatchSystem.LevelController.LevelData, setting);
            PlayerPrefs.SetString("TMatchLevelResultData", jsonData);
            PlayerPrefs.SetInt("TMatchResultExecuteStar", 0);
            PlayerPrefs.SetInt("TMatchResultExecuteWeeklyChallenge", 0);
            PlayerPrefs.SetInt("TMatchResultExecuteLast", 0);
        }

        private void CloseOnClick()
        {
            if (RemoveAdModel.Instance.IsUnlock() && (!StorageManager.Instance.GetStorage<StorageTMatch>().RemoveAd.RemoveAd) && AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.TM_FINISH_GAME))
            {
                AdSubSystem.Instance.PlayInterstital(ADConstDefine.TM_FINISH_GAME, b =>
                {
                    RemoveAdModel.Instance.TryToAutoOpen();
                });
            }
            UIViewSystem.Instance.Close<UITMatchWinController>();

            SceneFsm.mInstance.ChangeState(StatusType.TripleMatchEntry, new FsmParamTMatchEntry(StatusType.TripleMatch)
            {
                lastMatchLevelData = TMatchSystem.LevelController.LevelData
            });
        }
    }
}