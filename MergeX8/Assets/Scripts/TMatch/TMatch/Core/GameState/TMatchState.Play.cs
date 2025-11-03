using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus.Config.TMatch;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using OutsideGuide;
using UnityEngine;

namespace TMatch
{


    public class TMatchPlayStateParam : TMatchStateParam
    {
        public FsmParamTMatch fsmParamTMatch;
    }

    public class TMatchPlayState : TMatchBaseState
    {
        TMatchStateType TMatchBaseState.Type => TMatchStateType.Play;

        private TMatchBaseItem hitItem;

        public async Task Enter(TMatchStateParam param)
        {
            TMatchPlayStateParam stateParam = param as TMatchPlayStateParam;

            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_WIN, OnWinEvt);
            EventDispatcher.Instance.AddEventListener(EventEnum.TMATCH_GAME_FAIL, OnFailEvt);

            EventDispatcher.Instance.DispatchEvent(EventEnum.TMATCH_GAME_START);

            // GuideSubSystem.Instance.Trigger(GuideTrigger.MatchGameStart, stateParam.fsmParamTMatch.level.ToString());
        }

        public void Update(float deltaTime)
        {
            if (!TMatchSystem.LevelController.LevelStateData.pause && !DecoGuideManager.Instance.IsRunning)
            {
                TMatchBaseItem lastHitItem = hitItem;
                if (lastHitItem != null)
                {
                    lastHitItem.Pick();
                }

                if (Input.GetMouseButton(0) || Input.touchCount > 0)
                {
                    Vector2 pos =
#if UNITY_EDITOR
                        new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else
                    (Input.touchCount > 0 ? Input.GetTouch(0).position : Vector2.zero);
#endif
                    Ray ray = CameraManager.MainCamera.ScreenPointToRay(pos);
                    int layerMask = 1 << LayerMask.NameToLayer("TMatchItem");
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 100, layerMask))
                    {
                        hitItem = TMatchItemSystem.Instance.FindItem(hit.rigidbody);
                        if (hitItem.OperState == TMatchBaseItem.OperStateType.Scene)
                        {
                            //引导
                            // if (GuideSubSystem.Instance.IsShowingGuide())
                            // {
                            //     if (GuideSubSystem.Instance.GetCurGuideId().Equals("GUIDE_101"))
                            //     {
                            //         if (hitItem.GameObject.transform != GuideSubSystem.Instance.GetTarget(GuideTargetType.MatchItemTripleOfOne, "1")) hitItem = null;
                            //     }
                            //     else if (GuideSubSystem.Instance.GetCurGuideId().Equals("GUIDE_102"))
                            //     {
                            //         if (hitItem.GameObject.transform != GuideSubSystem.Instance.GetTarget(GuideTargetType.MatchItemTripleOfTwo, "1")) hitItem = null;
                            //     }
                            //     else if (GuideSubSystem.Instance.GetCurGuideId().Equals("GUIDE_103"))
                            //     {
                            //         if (hitItem.GameObject.transform != GuideSubSystem.Instance.GetTarget(GuideTargetType.MatchItemTripleOfThree, "1")) hitItem = null;
                            //     }
                            //     else if (GuideSubSystem.Instance.GetCurGuideId().Equals("GUIDE_121"))
                            //     {
                            //         if (hitItem.GameObject.transform != GuideSubSystem.Instance.GetTarget(GuideTargetType.MatchItemGlodenHatter, TMatchGoldenHatterSystem.LightingItemId.ToString())) hitItem = null;
                            //     }
                            //     else if (GuideSubSystem.Instance.GetCurGuideId().Equals("GUIDE_122"))
                            //     {
                            //         if (hitItem.GameObject.transform != GuideSubSystem.Instance.GetTarget(GuideTargetType.MatchItemGlodenHatter, TMatchGoldenHatterSystem.ClockItemId.ToString())) hitItem = null;
                            //     }
                            //     else if (GuideSubSystem.Instance.GetCurGuideId().Equals("GUIDE_123"))
                            //     {
                            //         if (hitItem.GameObject.transform != GuideSubSystem.Instance.GetTarget(GuideTargetType.MatchItemWeeklyChallenge, WeeklyChallengeController.Instance.CurCollectItemId.ToString())) hitItem = null;
                            //     }
                            //     else
                            //     {
                            //         hitItem = null;
                            //     }
                            // }

                            hitItem?.Pick();
                        }
                        else
                        {
                            hitItem = null;
                        }
                    }
                    else
                    {
                        hitItem = null;
                    }

                    if (lastHitItem != null && lastHitItem != hitItem)
                    {
                        lastHitItem.UnPick();
                    }
                }
                else if (hitItem != null)
                {
                    hitItem.UnPick();
                    List<TMatchBaseItem> itmes = new List<TMatchBaseItem>();
                    itmes.Add(hitItem);
                    AudioSysManager.Instance.PlaySound(SfxNameConst.button_s);
                    TMatchCollectorSystem.Instance.Collect(itmes, true, false, 0.2f, Ease.OutQuad);

                    //引导
                    // int targetId = 1;
                    // if (targetId == hitItem.Id)
                    // {
                    //     List<TMatchBaseItem> targetItems = TMatchItemSystem.Instance.Items.FindAll(x => x.Id == targetId && x.OperState == TMatchBaseItem.OperStateType.Scene);
                    //     if(targetItems.Count == 2) GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchItemTripleOfOne);
                    //     else if(targetItems.Count == 1) GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchItemTripleOfTwo);
                    //     else if(targetItems.Count == 0) GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchItemTripleOfThree);
                    // }
                    // if(hitItem.Cfg.BoosterId == TMatchGoldenHatterSystem.LightingItemId || hitItem.Cfg.BoosterId == TMatchGoldenHatterSystem.ClockItemId) GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchItemGlodenHatter);
                    // if (hitItem.Cfg.BoosterId == WeeklyChallengeController.Instance.CurCollectItemId) GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MatchItemWeeklyChallenge);
                    hitItem = null;
                }
            }
        }

        public async Task Exit()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_WIN, OnWinEvt);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.TMATCH_GAME_FAIL, OnFailEvt);
        }

        private void OnWinEvt(BaseEvent evt)
        {
            EventDispatcher.Instance.DispatchEvent(new TMatchGameChangeStateEvent(TMatchStateType.Finish,
                new TMatchPlayStateParam()));
        }

        private void OnFailEvt(BaseEvent evt)
        {
            EventDispatcher.Instance.DispatchEvent(new TMatchGameChangeStateEvent(TMatchStateType.Finish,
                new TMatchPlayStateParam()));
        }
    }
}