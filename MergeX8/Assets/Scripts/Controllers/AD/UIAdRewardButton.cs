using System;
using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;
using UnityEngine.UI;

namespace DragonPlus
{
    [RequireComponent(typeof(Button))]
    public class UIAdRewardButton : MonoBehaviour
    {
        public enum ButtonStyle
        {
            Hide,
            Disable
        }

        public delegate bool CustomCheck();

        public static UIAdRewardButton Create(string pos, ButtonStyle style, GameObject go, Action<bool> cb,
            bool isOutReward = false, CustomCheck check = null, Action onBtnClick = null)
        {
            var ad = go.GetComponent<UIAdRewardButton>();
            if (ad == null)
                ad = go.AddComponent<UIAdRewardButton>();
            ad.Init(pos, style, cb, isOutReward, check, onBtnClick);
            return ad;
        }

        private string Pos;
        private ButtonStyle Style;
        private Action<bool> Cb;
        private CustomCheck customCheck;
        private Button Bt;
        private bool IsOutReward;

        private void Init(string pos, ButtonStyle style, Action<bool> cb, bool isOutReward, CustomCheck check,
            Action onBtnClick = null)
        {
            Pos = pos;
            Style = style;
            Cb = cb;
            customCheck = check;
            IsOutReward = isOutReward;
            Bt = gameObject.GetComponent<Button>();
            if (Bt == null)
                throw new Exception($"AdReward button not exist in:{gameObject.name}");

            Bt.onClick.RemoveAllListeners();
            Bt.onClick.AddListener(() =>
            {
                if (onBtnClick != null)
                    onBtnClick();
                OnClick();
            });

            AdRewardedVideoPlacementMonitor.Bind(gameObject, Pos.ToString(), () =>
                Style == ButtonStyle.Hide || Check());

            OnCheck();

            TickManager.Instance.AddSecond(OnUpdate);
        }

        private void OnDestroy()
        {
            TickManager.Instance.RemoveSecond(OnUpdate);
        }

        private void OnUpdate(float delta)
        {
            OnCheck();
        }

        public class RVButtonShouldShowState
        {
            public int FrameCount;
            public bool State;
            public RVButtonShouldShowState(int frameCount,bool state)
            {
                FrameCount = frameCount;
                State = state;
            }
        }

        public static Dictionary<string, RVButtonShouldShowState> RvButtonShouldShowStateDic =
            new Dictionary<string, RVButtonShouldShowState>();
        private bool Check()
        {
            return (customCheck == null || customCheck()) && LocalCheck();
        }

        public bool LocalCheck()
        {
            if (RvButtonShouldShowStateDic.TryGetValue(Pos,out var showState))
            {
                if (showState.FrameCount != Time.frameCount)
                {
                    showState.FrameCount = Time.frameCount;
                    showState.State = AdLogicManager.Instance.ShouldShowRV(Pos);
                }
            }
            else
            {
                var state = AdLogicManager.Instance.ShouldShowRV(Pos);
                showState = new RVButtonShouldShowState(Time.frameCount, state);
                RvButtonShouldShowStateDic.Add(Pos,showState);
            }
            return showState.State;
        }

        private void OnCheck()
        {
            if (this == null)
                return;
            var available = Check();

            switch (Style)
            {
                case ButtonStyle.Hide:
                    gameObject.SetActive(available);
                    break;
                case ButtonStyle.Disable:
                    Bt.interactable = available;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnClick()
        {
            if (!AdLogicManager.Instance.ShouldShowRV(Pos))
            {
                Cb?.Invoke(false);
                return;
            }

            AdSubSystem.Instance.PlayRV(Pos, Cb, IsOutReward);
            RvButtonShouldShowStateDic.Remove(Pos);
            OnCheck();
        }
    }
}