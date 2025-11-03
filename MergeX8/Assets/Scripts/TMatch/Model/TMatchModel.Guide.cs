using System.Collections.Generic;
using DG.Tweening;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Network.API.Protocol;
using OutsideGuide;
using UnityEngine;

namespace TMatch
{
    public partial class TMatchModel
    {
        // public void StartEntryGuide()
        // {
        //     DecoGuideManager.Instance.UnRegisterUIGuide(1011201);
        //
        //     //var uiMain = UIMain.Get();
        //
        //     DecoGuideManager.Instance.RegisterUIGuide(1011201, new List<GuideStepData>
        //     {
        //         new GuideStepData
        //         {
        //             target = UIHomeMainController.mainController.butTmatch.transform,
        //             eventAction = TMatch.UINotice.Open
        //         }
        //     });
        //
        //     DecoGuideManager.Instance.RegisterUIGuide(1011202, new List<GuideStepData>()
        //     {
        //         new GuideStepData
        //         {
        //             eventTarget = ()=>TMatch.UIManager.Instance.GetOpenedWindow<UINotice>().widgets.buttons[1].transform,
        //             targetButtonAction = () =>
        //             {
        //                 GuideButton.Create(TMatch.UIManager.Instance.GetOpenedWindow<UINotice>().widgets.buttons[1].gameObject);
        //             },
        //         }
        //     });
        //
        //     DecoGuideManager.Instance.StartGuide(10112);
        // }

        /// <summary>
        /// 关卡按钮引导
        /// </summary>
        public bool TryGuideLevel()
        {
            if (DecoGuideManager.Instance.GetGuideState(10113))
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UILobbyView>()
                .GetComponentInChildren<UILobbyMainViewLevelButton>();

            DecoGuideManager.Instance.RegisterUIGuide(1011301, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = target.levelButton.transform,
                    eventAction = target.GuideClickLevel
                }
            });

            DecoGuideManager.Instance.StartGuide(10113);
            return true;
        }

        /// <summary>
        /// 关卡完成教学
        /// </summary>
        /// <returns></returns>
        public bool TryGuideFinishLevel()
        {
            if (DecoGuideManager.Instance.GetGuideState(10115) || GetMainLevel() <= 1)
                return false;

            DecoGuideManager.Instance.StartGuide(10115);
            DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                .GameEventTmLevle1Pass);
            return true;
        }

        /// <summary>
        /// 磁铁使用引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideMagnet()
        {
            if (DecoGuideManager.Instance.GetGuideState(10116) ||
                GetMainLevel() < TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock1)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();

            DecoGuideManager.Instance.RegisterUIGuide(1011601, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = UITMatchMainController.GetMagentTopView(),
                    eventAction = target.GuideClickMagnet
                }
            });

            DecoGuideManager.Instance.StartGuide(10116);
            return true;
        }

        /// <summary>
        /// 遮挡引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideCover()
        {
            if (DecoGuideManager.Instance.GetGuideState(10117) || GetMainLevel() < 4)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();

            DecoGuideManager.Instance.RegisterUIGuide(1011701, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = target.GuideTips2,
                }
            });

            DecoGuideManager.Instance.RegisterUIGuide(1011702, new List<GuideStepData>
            {
                new GuideStepData
                {
                    targetButtonAction = () => { target.GuideFrame.gameObject.SetActive(true); },
                    target = target.GuideFrame,
                    clickAnyWhereAction = () => { target.GuideFrame.gameObject.SetActive(false); },
                }
            });

            DecoGuideManager.Instance.StartGuide(10117);
            return true;
        }

        /// <summary>
        /// 扫把使用引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideBroom()
        {
            if (DecoGuideManager.Instance.GetGuideState(10118) ||
                GetMainLevel() < TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock2)
                return false;

            var id = 0;
            var collectItems = new List<TMatchBaseItem>();
            foreach (var item in TMatchItemSystem.Instance.Items)
            {
                if (collectItems.Count == 3)
                    break;
                if (item.Id == id)
                    continue;
                collectItems.Add(item);
                id = item.Id;
            }

            TMatchCollectorSystem.Instance.Collect(collectItems, false, false, 0, Ease.Linear);
            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();

            DecoGuideManager.Instance.RegisterUIGuide(1011801, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = UITMatchMainController.GetBroomTopView(),
                    eventAction = target.GuideClickBroom
                }
            });

            DecoGuideManager.Instance.StartGuide(10118);
            return true;
        }

        /// <summary>
        /// 风车使用引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideWindmill()
        {
            if (DecoGuideManager.Instance.GetGuideState(10119) ||
                GetMainLevel() < TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock3)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();

            DecoGuideManager.Instance.RegisterUIGuide(1011901, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = UITMatchMainController.GetWindmillTopView(),
                    eventAction = target.GuideClickWindmill
                }
            });

            DecoGuideManager.Instance.StartGuide(10119);
            return true;
        }

        /// <summary>
        /// 冰冻使用引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideFrozen()
        {
            if (DecoGuideManager.Instance.GetGuideState(10120) ||
                GetMainLevel() < TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterUnlock4)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();

            DecoGuideManager.Instance.RegisterUIGuide(1012001, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = UITMatchMainController.GetFozenTopView(),
                    eventAction = target.GuideClickFrozen
                }
            });

            DecoGuideManager.Instance.StartGuide(10120);
            return true;
        }

        /// <summary>
        /// 闪电引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideLighting()
        {
            if (DecoGuideManager.Instance.GetGuideState(10121) ||
                GetMainLevel() < TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterLightningUnlock)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchLevelPrepareView>();

            DecoGuideManager.Instance.RegisterUIGuide(1012101, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = UITMatchLevelPrepareView.GetLightingTopView(),
                    eventAction = target.GuideClickLighting
                }
            });

            DecoGuideManager.Instance.StartGuide(10121);
            return true;
        }

        /// <summary>
        /// 时钟引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideClock()
        {
            if (DecoGuideManager.Instance.GetGuideState(10123) ||
                GetMainLevel() < TMatchConfigManager.Instance.GlobalList[0].MatchLevelBoosterClockUnlock)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchLevelPrepareView>();

            DecoGuideManager.Instance.RegisterUIGuide(1012301, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = UITMatchLevelPrepareView.GetClockTopView(),
                    eventAction = target.GuideClickClock
                }
            });

            DecoGuideManager.Instance.StartGuide(10123);
            return true;
        }

        /// <summary>
        /// 玩法引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideGame()
        {
            if (DecoGuideManager.Instance.GetGuideState(10114) || GetMainLevel() != 1)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();

            DecoGuideManager.Instance.RegisterUIGuide(1011401, new List<GuideStepData>
            {
                new GuideStepData
                {
                    targetButtonAction = () =>
                    {
                        var guide = UIManager.Instance.GetOpenedWindow<UITMatchMainController>().GuideTipsCenter;
                        guide.position = TMatchItemSystem.Instance.Items[0].ToUIPosition();
                    },
                    target = target.GuideTipsCenter,
                    eventAction = () =>
                    {
                        DragonPlus.GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType
                            .GameEventTmLevle1Click);
                        TMatchCollectorSystem.Instance.Collect(
                            new List<TMatchBaseItem> {TMatchItemSystem.Instance.Items[0]}, true, false, 0.2f,
                            Ease.OutQuad);
                    }
                }
            });

            DecoGuideManager.Instance.RegisterUIGuide(1011402, new List<GuideStepData>
            {
                new GuideStepData
                {
                    targetButtonAction = () =>
                    {
                        var guide = UIManager.Instance.GetOpenedWindow<UITMatchMainController>().GuideTipsCenter;
                        guide.position = TMatchItemSystem.Instance.Items[1].ToUIPosition();
                    },
                    target = target.GuideTipsCenter,
                    eventAction = () =>
                    {
                        TMatchCollectorSystem.Instance.Collect(
                            new List<TMatchBaseItem> {TMatchItemSystem.Instance.Items[1]}, true, false, 0.2f,
                            Ease.OutQuad);
                    }
                }
            });

            DecoGuideManager.Instance.RegisterUIGuide(1011403, new List<GuideStepData>
            {
                new GuideStepData
                {
                    targetButtonAction = () =>
                    {
                        var guide = UIManager.Instance.GetOpenedWindow<UITMatchMainController>().GuideTipsCenter;
                        guide.position = TMatchItemSystem.Instance.Items[2].ToUIPosition();
                    },
                    target = target.GuideTipsCenter,
                    eventAction = () =>
                    {
                        TMatchCollectorSystem.Instance.Collect(
                            new List<TMatchBaseItem> {TMatchItemSystem.Instance.Items[2]}, true, false, 0.2f,
                            Ease.OutQuad);
                    }
                }
            });

            DecoGuideManager.Instance.RegisterUIGuide(1011404, new List<GuideStepData>
            {
                new GuideStepData
                {
                    target = target.GuideTips,
                }
            });

            DecoGuideManager.Instance.StartGuide(10114);
            return true;
        }

        private TMatchBaseItem _item;

        /// <summary>
        /// 收集闪电引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideCollectLighting()
        {
            if (DecoGuideManager.Instance.GetGuideState(10124) || GetMainLevel() != 23)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();
            _item = TMatchItemSystem.Instance.Create(126, 4);
            _item.GameObject.transform.position = new Vector3(0, 8, 0);

            DecoGuideManager.Instance.RegisterUIGuide(1012401, new List<GuideStepData>
            {
                new GuideStepData
                {
                    targetButtonAction = () =>
                    {
                        UIManager.Instance.GetOpenedWindow<UITMatchMainController>().SetGuideTMItem(_item);
                    },
                    target = target.GuideTipsCenter,
                    eventAction = () =>
                    {
                        TMatchCollectorSystem.Instance.Collect(new List<TMatchBaseItem> {_item}, true, false, 0.2f,
                            Ease.OutQuad);
                    }
                }
            });

            DecoGuideManager.Instance.StartGuide(10124);
            return true;
        }

        /// <summary>
        /// 收集时钟引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideCollectClock()
        {
            if (DecoGuideManager.Instance.GetGuideState(10125) || GetMainLevel() != 24)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();
            _item = TMatchItemSystem.Instance.Create(236, 4);
            _item.GameObject.transform.position = new Vector3(0, 8, 0);

            DecoGuideManager.Instance.RegisterUIGuide(1012501, new List<GuideStepData>
            {
                new GuideStepData
                {
                    targetButtonAction = () =>
                    {
                        UIManager.Instance.GetOpenedWindow<UITMatchMainController>().SetGuideTMItem(_item);
                    },
                    target = target.GuideTipsCenter,
                    eventAction = () =>
                    {
                        TMatchCollectorSystem.Instance.Collect(new List<TMatchBaseItem> {_item}, true, false, 0.2f,
                            Ease.OutQuad);
                    }
                }
            });

            DecoGuideManager.Instance.StartGuide(10125);
            return true;
        }

        /// <summary>
        /// 周收集引导
        /// </summary>
        /// <returns></returns>
        public bool TryGuideWeekCollect()
        {
            if (DecoGuideManager.Instance.GetGuideState(10122) || GetMainLevel() != 11)
                return false;

            var target = UIManager.Instance.GetOpenedWindow<UITMatchMainController>();
            var id = WeeklyChallengeController.Instance.model.GetCurWeeklyChallengeCfg().collectItemId;
            id = TMatchConfigManager.Instance.GetItemByBoosterId(id).id;
            _item = TMatchItemSystem.Instance.Items.Find(item => item.Id == id);
            if (_item == null)
                return false;
            _item.GameObject.transform.position = new Vector3(0, 8, 0);

            DecoGuideManager.Instance.RegisterUIGuide(1012201, new List<GuideStepData>
            {
                new GuideStepData
                {
                    targetButtonAction = () =>
                    {
                        UIManager.Instance.GetOpenedWindow<UITMatchMainController>().SetGuideTMItem(_item);
                    },
                    target = target.GuideTipsCenter,
                    eventAction = () =>
                    {
                        TMatchCollectorSystem.Instance.Collect(new List<TMatchBaseItem> {_item}, true, false, 0.2f,
                            Ease.OutQuad);
                    }
                }
            });

            DecoGuideManager.Instance.StartGuide(10122);
            return true;
        }
    }
}