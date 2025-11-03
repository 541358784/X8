using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public partial class UISnakeLadderMainController
{
    private DefenseEffect ShowEffect;
    private Transform DefaultDefenseEffect;
    private bool InitDefenseEffectFlag = false;
    public void InitDefenseEffect()
    {
        if (InitDefenseEffectFlag)
            return;
        InitDefenseEffectFlag = true;
        DefaultDefenseEffect = GetItem<Transform>("Root/UseShield");
        DefaultDefenseEffect.gameObject.SetActive(false);
        EventDispatcher.Instance.AddEvent<EventSnakeLadderCardCountChange>(ShowDefenseEffect);
    }

    public void ShowDefenseEffect(EventSnakeLadderCardCountChange evt)
    {
        var cardConfig = evt.CardState;
        if (cardConfig.CardType == SnakeLadderCardType.Defense && evt.ChangeCount < 0)
        {
            Action<Action> performAction = (callback) =>
            {
                PopupDefenseEffect(callback);
            };
            PushPerformAction(performAction);   
        }
    }
    public void ReleaseDefenseEffect()
    {
        EventDispatcher.Instance.RemoveEvent<EventSnakeLadderCardCountChange>(ShowDefenseEffect);
    }
    public void PopupDefenseEffect(Action callback)
    {
        if (ShowEffect)
        {
            ShowEffect.OnClickCloseBtn();
        }
        var sortingLayerId = canvas.sortingLayerID;
        var sortingOrder = canvas.sortingOrder;
        var newCard = Instantiate(DefaultDefenseEffect, DefaultDefenseEffect.parent);
        newCard.gameObject.SetActive(true);
        var cardCanvas = newCard.gameObject.AddComponent<Canvas>();
        cardCanvas.overrideSorting = true;
        cardCanvas.sortingLayerID = sortingLayerId;
        cardCanvas.sortingOrder = sortingOrder + 3;
        newCard.gameObject.AddComponent<GraphicRaycaster>();
        var DefenseEffect = newCard.gameObject.AddComponent<DefenseEffect>();
        DefenseEffect.PopupEffect(this,callback);
        ShowEffect = DefenseEffect;
        AudioManager.Instance.PlaySound(118);
    }

    public class DefenseEffect : MonoBehaviour
    {
        // private Button CloseBtn;
        private Animator Animator;
        private void Awake()
        {
            // CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
            // CloseBtn.onClick.AddListener(OnClickCloseBtn);
            Animator = transform.GetComponent<Animator>();
        }

        // public bool WaitClick = false;
        public void OnClickCloseBtn()
        {
            // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SnakeLadderMainGetCard);
            // CloseBtn.interactable = false;
            // WaitClick = false;
            Action SendEvent = () =>
            {
                Callback?.Invoke();
            };
            Animator.PlayAnimation("disappear", () =>
            {
                Destroy(gameObject);
                SendEvent();
            });
        }
        public UISnakeLadderMainController MainUI;
        private Action Callback;
        public async void PopupEffect(UISnakeLadderMainController mainUI,Action callback)
        {
            Callback = callback;
            MainUI = mainUI;
            // CloseBtn.interactable = false;
            // WaitClick = true;
            Animator.PlayAnimation("appear",()=>
            {
                // if (!this)
                //     return;
                // if (WaitClick)
                // {
                //     CloseBtn.interactable = true;
                //     if (UISnakeLadderMainController.IsAuto)
                //     {
                //         OnClickCloseBtn();
                //     }
                // }
            });
            XUtility.WaitSeconds(1f, OnClickCloseBtn);
        }
    }
}