using System;
using System.Collections.Generic;
using DragonPlus;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;

public partial class UISnakeLadderMainController
{
    private RollerNode Roller;
    private bool InitRollerViewFlag = false;
    public void InitRollerView()
    {
        if (InitRollerViewFlag)
            return;
        InitRollerViewFlag = true;
        Roller = transform.Find("Root/Turntable").gameObject.AddComponent<RollerNode>();
        Roller.MainUI = this;
        Roller.Init();
    }

    public void SpinBtnGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SnakeLadderGuideSpin))
        {
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(Roller.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SnakeLadderGuideSpin, Roller.SpinBtn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SnakeLadderGuideSpin, null))
            {
                SnakeLadderModel.Instance.AddTurntable(1,"GuideSend");
                Storage.TurntableRandomPool.Clear();
                Storage.TurntableRandomPool.Add(1);
            }
        }
    }
    public class RollerNode:MonoBehaviour
    {
        public UISnakeLadderMainController MainUI;
        public Button SpinBtn;
        private SnakeLadderRollerView RollerView;
        private LocalizeTextMeshProUGUI SpinCountText;
        private Transform DoubleBack;
        private Transform BtnDoubleBack;
        
        public void Init()
        {
            SpinCountText.SetText(MainUI.Storage.TurntableCount.ToString());
            DoubleBack.gameObject.SetActive(MainUI.Storage.StepMultiValue > 1);
            BtnDoubleBack.gameObject.SetActive(MainUI.Storage.StepMultiValue > 1);
            RollerView.StepMultiValue = MainUI.Storage.StepMultiValue;
            RollerView.RefreshRewardState();
        }
        private void Awake()
        {
            DoubleBack = transform.Find("Double");
            BtnDoubleBack = transform.Find("Button/Double");
            SpinBtn = transform.Find("Button").GetComponent<Button>();
            SpinBtn.onClick.AddListener(OnClickSpinBtn);
            SpinCountText = transform.Find("Button/Num").GetComponent<LocalizeTextMeshProUGUI>();
            RollerView = new SnakeLadderRollerView(transform.Find("Reward"));
            RollerView.Init();
            EventDispatcher.Instance.AddEvent<EventSnakeLadderUIPlayTurntable>(PlayTurntable);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderTurntableCountChange>(ChangeSpinCountText);
            EventDispatcher.Instance.AddEvent<EventSnakeLadderStepMultiChange>(StepMultiChange);
        }

        public void ChangeSpinCountText(EventSnakeLadderTurntableCountChange evt)
        {
            Action<Action> performAction = (callback) =>
            {
                SpinCountText.SetText(evt.TotalValue.ToString());
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        public void PlayTurntable(EventSnakeLadderUIPlayTurntable evt)
        {
            var cardConfig = evt.CardConfig;
            var cardState = new SnakeLadderCardState(cardConfig);
            var indexList = new List<int>();
            if (cardState.CardType == SnakeLadderCardType.Step)
            {
                var resultList = SnakeLadderModel.Instance.GlobalConfig.TurntableResultList;
                for (var i = 0; i < resultList.Count; i++)
                {
                    if (resultList[i] == cardState.Step)
                    {
                        indexList.Add(i);
                    }
                }
            }
            else
            {
                var resultList = SnakeLadderModel.Instance.GlobalConfig.TurntableResultList;
                for (var i = 0; i < resultList.Count; i++)
                {
                    if (resultList[i] == 0)
                    {
                        indexList.Add(i);
                    }
                }
            }
            if (indexList.Count == 0)
                throw new Exception("转盘结果未找到");
            var index = indexList.RandomPickOne();
            Action<Action> performAction = (callback) =>
            {
                AudioManager.Instance.PlaySound(113);
                RollerView.PerformTurntable(index).AddCallBack(callback).WrapErrors();
            };
            MainUI.PushPerformAction(performAction);
        }

        public void StepMultiChange(EventSnakeLadderStepMultiChange evt)
        {
            Action<Action> performAction = (callback) =>
            {
                DoubleBack.gameObject.SetActive(evt.NewValue > 1);
                BtnDoubleBack.gameObject.SetActive(evt.NewValue > 1);
                RollerView.StepMultiValue = evt.NewValue;
                RollerView.RefreshRewardState();
                callback();
            };
            MainUI.PushPerformAction(performAction);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderUIPlayTurntable>(PlayTurntable);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderTurntableCountChange>(ChangeSpinCountText);
            EventDispatcher.Instance.RemoveEvent<EventSnakeLadderStepMultiChange>(StepMultiChange);
        }

        public void OnClickSpinBtn()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SnakeLadderGuideSpin);
            if (MainUI.IsPlaying())
                return;
            if (!SnakeLadderModel.Instance.ReduceTurntableCount(1))
            {
                if (MainUI.Storage.CompleteTimes > 0)
                {
                    UIPopupSnakeLadderNoTurntableController.Open(MainUI.Storage);   
                }
                return;
            }
            MainUI.Storage.SpinTurntable();
        }
    }
}