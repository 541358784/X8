using System;
using System.Collections.Generic;
using System.Linq;
using Activity.SaveTheWhales;
using ActivityLocal.TipReward.Module;
using DragonPlus;
using DragonPlus.Config.TipReward;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using GamePool;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace ActivityLocal.TipReward.UI
{
    public class UIPopupTipRewardMainController : UIWindowController
    {
        private StorageTaskItem _order;
        private List<ResData> _resDatas;

        private RewardData _normalReward = new RewardData();
        private List<RewardData> _rvRewards = new List<RewardData>();

        private Transform _item;

        private Transform _spineRoot;
        private GameObject _portraitSpineObj;
        
        public override void PrivateAwake()
        {
            TipRewardModule.Instance.ResetCoolTime();
            
            transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(OnClickClose);
            _item = transform.Find("Root/Reward/RewardGroup/Item");
            _item.gameObject.SetActive(false);
            _spineRoot = transform.Find("Root/BG/Person/PortraitSpine");

#if UNITY_EDITOR
            transform.Find("Root/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                AnimCloseWindow(beforeCloseFunc:() =>
                {
                    foreach (var resData in _resDatas)
                    {
                        if (resData.id == (int)UserData.ResourceId.Coin || resData.id == (int)UserData.ResourceId.RareDecoCoin)
                            resData.count *= 2;
                        
                        if(resData.id == (int)UserData.ResourceId.JungleAdventure)
                            JungleAdventureModel.Instance.AddScore(resData.count);
                        
                        if (!UserData.Instance.IsResource((int)resData.id))
                        {
                            TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig((int)resData.id);
                            if (mergeItemConfig != null)
                            {
                                GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                                {
                                    MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonRTipRewardGet,
                                    itemAId = resData.id,
                                    isChange = true,
                                });
                            }
                        }
                    }
                        
                    CommonRewardManager.Instance.PopCommonReward(_resDatas, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, bi:new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.RTipRewardGet), windowType:UIWindowType.PopupTip
                        ,animEndCall: () =>
                        {
                            SaveTheWhalesModel.Instance.CheckFinish();
                        });
                });
            });
#else
            
            UIAdRewardButton.Create(ADConstDefine.R_TIPS_REWARD, UIAdRewardButton.ButtonStyle.Disable, transform.Find("Root/Button").gameObject,
                (s) =>
                {
                    if (s)
                    {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventRTipRewardPop);
                        AnimCloseWindow(beforeCloseFunc:() =>
                                {
                                    foreach (var resData in _resDatas)
                                    {
                                        if (resData.id == (int)UserData.ResourceId.Coin || resData.id == (int)UserData.ResourceId.RareDecoCoin)
                                            resData.count *= 2;

                                        if(resData.id == (int)UserData.ResourceId.JungleAdventure)
                                            JungleAdventureModel.Instance.AddScore(resData.count);

                                    if (!UserData.Instance.IsResource((int)resData.id))
                                    {
                                        TableMergeItem mergeItemConfig = GameConfigManager.Instance.GetItemConfig((int)resData.id);
                                        if (mergeItemConfig != null)
                                        {
                                            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                                            {
                                                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonRTipRewardGet,
                                                itemAId = resData.id,
                                                isChange = true,
                                            });
                                        }
                                    }
                                    }
                                            
                                    CommonRewardManager.Instance.PopCommonReward(_resDatas, CurrencyGroupManager.Instance.GetCurrencyUseController(), true,bi:new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.RTipRewardGet), windowType:UIWindowType.PopupTip
                                        ,animEndCall: () =>
                                    {
                                        SaveTheWhalesModel.Instance.CheckFinish();
                                    });
                                });
                    }
                }, false, null, () =>
                {
                });   
#endif
        }

        private void OnClickClose()
        {
            Action PopReward = () =>
            {
                var resData = _resDatas.Find(a => a.id == (int)UserData.ResourceId.Coin);
                if(resData == null)
                    resData = _resDatas.Find(a => a.id == (int)UserData.ResourceId.RareDecoCoin);
                    
                if(resData == null)
                    return;
                
                AnimCloseWindow(beforeCloseFunc:() =>
                {
                    CommonRewardManager.Instance.PopCommonReward(new List<ResData>(){resData}, CurrencyGroupManager.Instance.GetCurrencyUseController(), true, bi:new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.ITipRewardGet), windowType:UIWindowType.PopupTip);
                });
            };
#if UNITY_EDITOR
            PopReward();
#else
            var common = AdConfigHandle.Instance.GetCommon();
            if (common.TipReward == 2)
            {
                PopReward();
            }
            else if (common.TipReward == 1)
            {
                AdSubSystem.Instance.PlayInterstital(ADConstDefine.I_TIPS_REWARD, b =>
                {
                    if (b)
                    {
                        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventITipRewardPop);
                        PopReward();
                    }
                    else
                    {
                        AnimCloseWindow();
                    }
                });   
            }
#endif
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            _order = (StorageTaskItem)objs[0];
            _resDatas = (List<ResData>)objs[1];

            InitRewards();
            
            _portraitSpineObj = GamePool.ObjectPoolManager.Instance.Spawn(string.Format(ObjectPoolName.PortraitSpine, OrderConfigManager.Instance.GetSpineName(_order.HeadIndex)));
            _portraitSpineObj.transform.SetParent(_spineRoot);
            _portraitSpineObj.transform.localScale = Vector3.one;
            _portraitSpineObj.transform.localRotation = Quaternion.identity;
            ((RectTransform)_portraitSpineObj.transform).anchoredPosition = Vector3.zero;
            
            var skeletonGraphic = _portraitSpineObj.transform.GetComponentInChildren<SkeletonGraphic>();
            skeletonGraphic.AnimationState?.SetAnimation(0, OrderConfigManager.Instance.GetSpineName(_order.HeadIndex, 2), true);
            skeletonGraphic.Update(0);
        }

        private void InitRewards()
        {
            foreach (var resData in _resDatas)
            {
                if (resData.id == (int)UserData.ResourceId.Coin)
                    resData.count = Mathf.CeilToInt(1.0f * TipRewardConfigManager.Instance.TipRewardSetting.CoinFactor / 100 * resData.count);
                else
                    resData.count = Mathf.CeilToInt(1.0f * TipRewardConfigManager.Instance.TipRewardSetting.ActivityFactor / 100 * resData.count);
            }
            
            _normalReward.gameObject = transform.Find("Root/Reward/Item").gameObject;
            _normalReward.image = _normalReward.gameObject.transform.Find("Icon").GetComponent<Image>();
            _normalReward.numText = _normalReward.gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();

            var coinData = _resDatas.Find(a => a.id == (int)UserData.ResourceId.Coin);
            if(coinData == null)
                coinData = _resDatas.Find(a => a.id == (int)UserData.ResourceId.RareDecoCoin);
            
            _normalReward.UpdateReward(coinData);
            
            foreach (var resData in _resDatas)
            {
                var item = Instantiate(_item, _item.parent);
                item.gameObject.SetActive(true);
                item.transform.localScale = Vector3.one;
                
                
                _rvRewards.Add(new RewardData());
                _rvRewards.Last().gameObject = item.gameObject;
                _rvRewards.Last().image = item.gameObject.transform.Find("Icon").GetComponent<Image>();
                _rvRewards.Last().numText = item.gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();


                if (resData.id == (int)UserData.ResourceId.Coin || resData.id == (int)UserData.ResourceId.RareDecoCoin)
                {
                    _rvRewards.Last().UpdateReward(coinData);
                }
                else
                {
                    _rvRewards.Last().UpdateReward(resData);
                }
            }
        }

        private void OnDestroy()
        {
            GamePool.ObjectPoolManager.Instance.DeSpawn(string.Format(ObjectPoolName.PortraitSpine, OrderConfigManager.Instance.GetSpineName(_order.HeadIndex)), _portraitSpineObj);

        }
    }
}