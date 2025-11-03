using System;
using DG.Tweening;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    private Button BetBtn;
    private LocalizeTextMeshProUGUI BetText;
    private MonopolyModel Model => MonopolyModel.Instance;
    private int BetIndex=0;
    private int BetValue => Model.GlobalConfig.BetList[BetIndex];
    private Transform BetLockTips;
    private bool IsBetUnlock=>Storage.CompleteTimes >= 1;
    public void InitBetGroup()
    {
        BetLockTips = transform.Find("Root/Sieve/NumButton/Tip");
        BetLockTips.gameObject.SetActive(false);
        BetText = GetItem<LocalizeTextMeshProUGUI>("Root/Sieve/NumButton/Text");
        BetBtn = GetItem<Button>("Root/Sieve/NumButton");
        BetBtn.onClick.AddListener(() =>
        {
            if (isPlaying)
                return;
            if (!IsBetUnlock)
            {
                BetLockTips.DOKill(false);
                BetLockTips.gameObject.SetActive(false);
                BetLockTips.gameObject.SetActive(true);
                DOVirtual.DelayedCall(1f, () =>
                {
                    BetLockTips.gameObject.SetActive(false);
                }).SetTarget(BetLockTips);
                return;
            }
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MonopolyBet2);
            Action<Action> performAction = (callback) =>
            {
                BetIndex++;
                BetIndex %= Model.GlobalConfig.BetList.Count;
                BetText.SetText("x"+BetValue);
                EventDispatcher.Instance.SendEventImmediately<EventMonopolyUIBetChange>(new EventMonopolyUIBetChange(BetValue));
                callback();
            };
            PushPerformAction(performAction);
        });
        BetIndex = 0;
        BetText.SetText("x"+BetValue);
    }
}