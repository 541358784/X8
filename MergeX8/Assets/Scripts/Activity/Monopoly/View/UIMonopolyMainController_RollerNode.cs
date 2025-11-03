using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    public RollerNode Roller;
    private bool InitRollerViewFlag = false;
    public void InitRollerView()
    {
        if (InitRollerViewFlag)
            return;
        InitRollerViewFlag = true;
        Roller = transform.Find("Root/Sieve").gameObject.AddComponent<RollerNode>();
        Roller.MainUI = this;
        Roller.Init();
    }

    public void SpinBtnGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MonopolyThrowDice))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(Roller.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MonopolyThrowDice, Roller.SpinBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MonopolyThrowDice, null))
            {
                // MonopolyModel.Instance.AddDice(1,"GuideSend");
                Storage.DiceRandomPool.Clear();
                Storage.DiceRandomPool.Add(MonopolyModel.Instance.GlobalConfig.GuideStep);
            }
        }
    }
    public class RollerNode:MonoBehaviour
    {
        public UIMonopolyMainController MainUI;
        public Button SpinBtn;
        private LocalizeTextMeshProUGUI SpinCountText;
        private Transform DoubleIcon;
        private Transform Icon;
        
        public void Init()
        {
            SpinCountText.SetText(MainUI.Storage.DiceCount.ToString());
            DoubleIcon.gameObject.SetActive(MainUI.Storage.StepMultiValue() > 1);
            Icon.gameObject.SetActive(MainUI.Storage.StepMultiValue() == 1);
        }
        private void Awake()
        {
            DoubleIcon = transform.Find("DoubleIcon");
            Icon = transform.Find("Icon");
            SpinBtn = transform.GetComponent<Button>();
            SpinBtn.onClick.AddListener(OnClickSpinBtn);
            SpinCountText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            
            EventDispatcher.Instance.AddEvent<EventMonopolyUIThrowMultipleDice>(PlayDice);
            EventDispatcher.Instance.AddEvent<EventMonopolyDiceCountChange>(ChangeSpinCountText);
            EventDispatcher.Instance.AddEvent<EventMonopolyStepMultiChange>(StepMultiChange);
        }

        public void ChangeSpinCountText(EventMonopolyDiceCountChange evt)
        {
            Action<Action> performAction = (callback) =>
            {
                SpinCountText.SetText(evt.TotalValue.ToString());
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PlayDice(EventMonopolyUIThrowMultipleDice evt)
        {
            var diceConfig = evt.DiceConfigList;
            
            Action<Action> performAction = (callback) =>
            {
                if (diceConfig.Count == 1)
                {
                    AudioManager.Instance.PlaySound(113);
                    var config = diceConfig[0];
                    var diceNode = transform.parent.Find("Dice_Ani_1");
                    diceNode.gameObject.SetActive(true);
                    var animator = diceNode.Find("Dice").GetComponent<Animator>();
                    animator.PlayAnimation("Play_"+config.Step, () =>
                    {
                        diceNode.gameObject.SetActive(false);
                        callback();
                    });
                }
                else if (diceConfig.Count == 2)
                {
                    AudioManager.Instance.PlaySound(113);
                    var diceNode = transform.parent.Find("Dice_Ani_2");
                    diceNode.gameObject.SetActive(true);
                    {
                        var config = diceConfig[0];
                        var animator = diceNode.Find("Dice").GetComponent<Animator>();
                        animator.PlayAnimation("Play_"+config.Step, () =>
                        {
                            diceNode.gameObject.SetActive(false);
                            callback();
                        });
                    }
                    {
                        var config = diceConfig[1];
                        var animator = diceNode.Find("Dice_1").GetComponent<Animator>();
                        animator.PlayAnimation("Play_"+config.Step);
                    }
                }
                else
                {
                    callback();
                }
            };
            MainUI.PushPerformAction(performAction);
        }

        public void StepMultiChange(EventMonopolyStepMultiChange evt)
        {
            Action<Action> performAction = (callback) =>
            {
                DoubleIcon.gameObject.SetActive(evt.NewValue > 1);
                Icon.gameObject.SetActive(evt.NewValue == 1);
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventMonopolyUIThrowMultipleDice>(PlayDice);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyDiceCountChange>(ChangeSpinCountText);
            EventDispatcher.Instance.RemoveEvent<EventMonopolyStepMultiChange>(StepMultiChange);
        }
        public void OnClickSpinBtn()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyThrowDice);
            if (MainUI.IsPlaying())
                return;
            
            if (!MonopolyModel.Instance.ReduceDiceCount(MainUI.BetValue))
            {
                UIPopupMonopolyNoDiceController.Open(MainUI.Storage);
                return;
            }
            var diceCount = 1;
            if (MainUI.Storage.StepMultiValue() > 1)
            {
                diceCount *= MainUI.Storage.StepMultiValue();
                MainUI.Storage.ReduceMultiStepCard();   
            }
            MainUI.Storage.AddRewardBoxProgress(MainUI.BetValue);
            var biStr = "";
            MainUI.Storage.ThrowDice(diceCount,biStr,out biStr,MainUI.BetValue);
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMonopolyDiceChange,
                "-1",MainUI.Storage.DiceCount.ToString(),biStr);
            if (MainUI.Storage.TryCollectRewardBox())
            {
                
            }
        }
    }
}