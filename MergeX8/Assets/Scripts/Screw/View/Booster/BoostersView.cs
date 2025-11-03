using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DragonU3DSDK;
using Screw.Module;
using Screw.UIBinder;
using UnityEngine;
using UnityEngine.UI;

namespace Screw
{
    public class BoostersView : Entity
    {
        [UIBinder("Bottom")] private RectTransform _bottom;
        [UIBinder("Bottom")] private Animator _bottomAni;
        [UIBinder("ExtraSlotButton")] private Transform _extraSlotButton;
        [UIBinder("BreakBodyButton")] private Transform _breakBodyButton;
        [UIBinder("TwoTaskButton")] private Transform _twoTaskButton;
        [UIBinder("ButtonClose")] private Button _closeBtn;

        private static Dictionary<BoosterType, BoosterButton> _boostersMap;
        //迁移报错注释
        // private EventBus.Listener _listener;
        // private EventBus.Listener _listener1;

        public static Transform GetBoosterRoot(BoosterType boosterType)
        {
            if (_boostersMap == null)
            {
                return null;
            }
            if (_boostersMap.ContainsKey(boosterType))
            {
                return _boostersMap[boosterType].GetRoot();
            }

            return null;
        }
        
        public BoostersView(Transform boosterRoot, ScrewGameContext context)
        {
            Bind(boosterRoot, context);

            _boostersMap = new Dictionary<BoosterType, BoosterButton>();

            _boostersMap.Add(BoosterType.ExtraSlot, new BoosterButton(_extraSlotButton, context, BoosterType.ExtraSlot));
            _boostersMap.Add(BoosterType.BreakBody, new BoosterButton(_breakBodyButton, context, BoosterType.BreakBody));
            _boostersMap.Add(BoosterType.TwoTask, new BoosterButton(_twoTaskButton, context, BoosterType.TwoTask));
            
            
            ResBarModule.Instance.RegisterResBar(ResBarType.ExtraSlot, GetBoosterRoot(BoosterType.ExtraSlot));
            ResBarModule.Instance.RegisterResBar(ResBarType.BreakBody, GetBoosterRoot(BoosterType.BreakBody));
            ResBarModule.Instance.RegisterResBar(ResBarType.TwoTask, GetBoosterRoot(BoosterType.TwoTask));
            
            _closeBtn.onClick.AddListener(OnExitBreakBody);
            RegisterEvent();

            if (base.context is ScrewMiniGameContext)
            {
                _bottom.gameObject.SetActive(false);
            }
        }

        private void RegisterEvent()
        {
            //迁移报错注释
            // _listener = EventBus.Subscribe<EventLanguageChange>((evt) =>
            // {
            //     UpdateText();
            // });
            // _listener1 = EventBus.Subscribe<EventRefreshCurrency>((evt) =>
            // {
            //     RefreshUI();
            // });
        }

        private void UpdateText()
        {
            foreach (var kv in _boostersMap)
            {
                kv.Value.UpdateText();
            }
        }

        private void OnExitBreakBody()
        {
            context.gameState = ScrewGameState.InProgress;
            context.hookContext.OnLogicEvent(LogicEvent.ExitBreakPanel, null);
            context.hookContext.OnLogicEvent(LogicEvent.BlockCheckFail, null);
        }

        public BoosterButton GetButton(BoosterType boosterType)
        {
            return _boostersMap[boosterType];
        }

        public void OnEnterLevel()
        {
            RefreshUI();
            TryShowBanner();
        }

        public void RefreshUI()
        {
            foreach (var kv in _boostersMap)
            {
                kv.Value.UpdateLockState();
                kv.Value.UpdateBoosterCount();
            }
        }

        private void TryShowBanner()
        {
            return;
            //迁移报错注释
            // var showBanner = GameApp.Get<AdSys>().TryShowBanner(eAdBanner.Game);
            // if (!showBanner) return;
            // GameApp.Get<AdSys>().ShowingBanner = true;
            
            var offset = 110f;
            var originalHeight = Screen.height;
            DebugUtil.LogP($"originalHeight : [{originalHeight}]");
            var sdkBannerHeight = MaxSdkUtils.GetAdaptiveBannerHeight();//当前SDK没有直接可以使用的ADManager获取banner高度的聚合方法，目前直接取max的banner高度
            DebugUtil.LogP($"Banner height: [{sdkBannerHeight}]");
            var uiHeight = (root.gameObject.transform as RectTransform).rect.height;
            DebugUtil.LogP($"uiHeight: [{uiHeight}]");
            if (sdkBannerHeight > 0)
            {
                var density = MaxSdkUtils.GetScreenDensity();
                DebugUtil.LogP($"Banner density: [{density}]");
                var heightPx = sdkBannerHeight * density;
            
                // heightPx 对应的就是原始分辨率中的px, 所以要转化为当前适配canvas高度
                offset = heightPx * uiHeight / originalHeight; 
            
                DebugUtil.LogP($"Screen.height : [{Screen.height}]");
                DebugUtil.LogP($"ui offset [{offset}]");
            }
        
            //Root锚点下方位置整体上移offset
            // _bottom.anchoredPosition += new Vector2(0, offset);
            // _bottom.sizeDelta -= new Vector2(0, offset);
        }
        
        public void OnExitLevel()
        {
            //迁移报错注释
            // if (_listener != null)
            //     EventBus.UnSubscribe(_listener);
            // _listener = null;
            // if (_listener1 != null)
            //     EventBus.UnSubscribe(_listener1);
            // _listener1 = null;
            //
            // GameApp.Get<AdSys>().ShowingBanner = false;
            // GameApp.Get<AdSys>().HideBanner();
            _boostersMap.Clear();
            
            ResBarModule.Instance.UnRegisterResBar(ResBarType.ExtraSlot, GetBoosterRoot(BoosterType.ExtraSlot));
            ResBarModule.Instance.UnRegisterResBar(ResBarType.BreakBody, GetBoosterRoot(BoosterType.BreakBody));
            ResBarModule.Instance.UnRegisterResBar(ResBarType.TwoTask, GetBoosterRoot(BoosterType.TwoTask));
        }

        public void OnPurchaseBooster(BoosterType boosterType, int count)
        {
            if (_boostersMap.ContainsKey(boosterType))
                _boostersMap[boosterType].OnPurchaseBooster(boosterType, count);
        }

        public async UniTask PlayAni(string aniName)
        {
            await ScrewUtility.PlayAnimationAsync(_bottomAni, aniName, root.GetCancellationTokenOnDestroy());
        }
    }
}