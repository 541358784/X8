using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Screw.GameLogic;
using Screw.Module;

namespace Screw
{
    public class BoostersHandler : ILogicEventHandler
    {
        private Dictionary<BoosterType, BoosterHandler> _boosterHandlers;
        private ScrewGameContext _context;

        public int GetExecuteOrder()
        {
            return ExecuteOrder.BoosterHandlerOrder;
        }

        public BoostersHandler(ScrewGameContext inContext)
        {
            _context = inContext;
            _boosterHandlers = new Dictionary<BoosterType, BoosterHandler>();
            _boosterHandlers.Add(BoosterType.ExtraSlot, new ExtraSlotBoosterHandler(inContext));
            _boosterHandlers.Add(BoosterType.BreakBody, new BreakBodyBoosterHandler(inContext));
            _boosterHandlers.Add(BoosterType.TwoTask, new TwoTaskBoosterHandler(inContext));
        }

        public async void OnBoosterClicked(BoosterType boosterType, bool isFree = false)
        {
            if (_boosterHandlers.ContainsKey(boosterType))
            {
                if (_boosterHandlers[boosterType].CanUse())
                {
                    if (!isFree)
                    {
                        _boosterHandlers[boosterType].ConsumeBooster();
                    }

                    ScrewUtility.Vibrate();
                    _context.Record.SetUseOther();
                    await _boosterHandlers[boosterType].Use(true);
                    
                    foreach (var kv in _boosterHandlers)
                    {
                        _context.boostersView.GetButton(kv.Key).UpdateLockState();
                        _context.boostersView.GetButton(kv.Key).UpdateBoosterCount();
                    }
                }
            }
        }

        public T GetHandler<T>(BoosterType boosterType) where T : BoosterHandler
        {
            return (T) _boosterHandlers[boosterType];
        }
        
        public BoosterHandler GetHandler(BoosterType boosterType)
        {
            return _boosterHandlers[boosterType];
        }

        public void OnLogicEvent(LogicEvent logicEvent, LogicEventParams eventParams)
        {
            switch (logicEvent)
            {
                case LogicEvent.PreMove:
                    foreach (var kv in _boosterHandlers)
                    {
                        if(kv.Key != BoosterType.ExtraSlot)
                            _boosterHandlers[kv.Key].OnPreMove(((MoveParams) eventParams).moveAction);
                    }
                    break;
                case LogicEvent.ExitBreakPanel:
                    _context.boostersView.PlayAni("Close").Forget();
                    foreach (var kv in _boosterHandlers)
                    {
                        _context.boostersView.GetButton(kv.Key).UpdateLockState();
                        _context.boostersView.GetButton(kv.Key).UpdateBoosterCount();
                    }
                    break;
                case LogicEvent.TwoTaskEnd:
                    _context.boostersView.GetButton(BoosterType.TwoTask).UpdateLockState();
                    _context.boostersView.GetButton(BoosterType.TwoTask).UpdateBoosterCount();
                    break;
                case LogicEvent.RefreshExtraSlot:
                    _context.boostersView.GetButton(BoosterType.ExtraSlot).UpdateLockState();
                    _context.boostersView.GetButton(BoosterType.ExtraSlot).UpdateBoosterCount();
                    break;
            }
        }
        
        public static void ShowPurchaseBoosterPopup(BoosterHandler boosterHandler)
        {
            if (ScrewGameLogic.Instance.context is ScrewGameKapiScrewContext)
            {
                UIPopupKapiScrewShopController.Open();
            }
            else
            {
                UIModule.Instance.ShowUI(typeof(UIBoosterPurchasePopup), boosterHandler);
            }
            //迁移报错注释
        }
    }
}