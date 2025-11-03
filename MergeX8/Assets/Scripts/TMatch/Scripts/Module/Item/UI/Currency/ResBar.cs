using System;
using DragonPlus;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace TMatch
{


    public abstract class ResBar : MonoBehaviour
    {
        public static float NUMBER_INCREASE_DURATION = 1f;

        protected int _currentNumber;

        //protected ResHostUI _hostUI;
        protected ResourceId _resourceId;
        protected ItemType _itemType;
        protected abstract bool TypeTest(ResourceId resId);
        protected abstract int CurrentCount { get; }
        protected abstract TextMeshProUGUI CountText { get; }
        protected Image _icon;

        public Image Icon
        {
            get => _icon;
        }

        public virtual void HideAdd()
        {

        }

        protected virtual void OnEnable()
        {
            EventDispatcher.Instance.AddEventListener(EventEnum.CURRENCY_CHANGED, UpdateTopResource);

            UpdateNumber(false);
        }

        protected virtual void OnDisable()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.CURRENCY_CHANGED, UpdateTopResource);
        }

        private void UpdateTopResource(BaseEvent evt = null)
        {
            if (evt == null)
            {
                UpdateNumber(true);
            }
            else
            {
                var cce = evt as ResChangeEvent;
                if (TypeTest(cce.CurrencyId))
                {
                    //_hostUI = cce.FlyUI;
                    UpdateNumber(cce.AnimateChange, cce);
                }
            }
        }

        protected virtual void UpdateNumber(bool playAnimation = true, ResChangeEvent e = null)
        {
            var ignoreNumber = e?.IgnoreNumber ?? 0;
            if (playAnimation)
            {
                playNumberAnimation(CountText, () =>
                {
                    _currentNumber = CurrentCount - ignoreNumber;
                    //var currencyBarHost = ResUtil.GetResBarHost(_hostUI);
                    //currencyBarHost?.OnNumberAnimationEnd();
                }, e);
            }
            else
            {
                _currentNumber = CurrentCount - ignoreNumber;
                CountText.SetText(_currentNumber.ToString("N0"));
            }
        }

        private float playNumberAnimation(LocalizeTextMeshProUGUI textMeshProUGUI, Action callBack,
            ResChangeEvent e = null)
        {
            var ignoreNumber = e?.IgnoreNumber ?? 0;
            var cacheCurrentCount = CurrentCount - ignoreNumber;
            DOTween.To(() => _currentNumber,
                    value => textMeshProUGUI.SetText(value.ToString("N0")),
                    cacheCurrentCount,
                    NUMBER_INCREASE_DURATION)
                .OnComplete(() =>
                {
                    textMeshProUGUI.SetText(cacheCurrentCount.ToString("N0"));
                    callBack?.Invoke();
                });

            return NUMBER_INCREASE_DURATION;
        }

        private float playNumberAnimation(TextMeshProUGUI textMeshProUGUI, Action callBack, ResChangeEvent e = null)
        {
            var ignoreNumber = e?.IgnoreNumber ?? 0;
            var cacheCurrentCount = CurrentCount - ignoreNumber;
            DOTween.To(() => _currentNumber,
                    value => textMeshProUGUI.SetText(value.ToString("N0")),
                    cacheCurrentCount,
                    NUMBER_INCREASE_DURATION)
                .OnComplete(() =>
                {
                    textMeshProUGUI.SetText(cacheCurrentCount.ToString("N0"));
                    callBack?.Invoke();
                });

            return NUMBER_INCREASE_DURATION;
        }

        protected virtual void CurrencyFlyAniEnd(BaseEvent evt)
        {
            CurrencyFlyAniEnd eventData = evt as CurrencyFlyAniEnd;
            if (eventData.itemType == _itemType)
            {
                var animator = transform.GetComponent<Animator>();
                transform.GetComponent<Animator>()?.Play("shake", -1, 0);
                // UpdateNumber(true);
            }
        }

        protected virtual void onAddButtonClick()
        {

        }
    }
}
