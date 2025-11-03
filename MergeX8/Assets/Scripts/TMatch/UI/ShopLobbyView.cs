using System;
using System.Threading.Tasks;
using UnityEngine;


namespace TMatch
{
    public class LobbyShopView : ShopBaseView
    {
        public override Animator Animator => null;

        public override string OpenAnimStateName => "";
        public override string CloseAnimStateName => "";

        public override string OpenAudio => "";
        public override string CloseAudio => "";

        public override Action EmptyCloseAction => null;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyNavigationActiveType, OnLobbyNavigationActiveTypeEvt);
        }

        public override Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyNavigationActiveType, OnLobbyNavigationActiveTypeEvt);

            return base.OnViewClose();
        }

        private void OnLobbyNavigationActiveTypeEvt(BaseEvent evt)
        {
            LobbyNavigationActiveTypeEvent derivedEvt = evt as LobbyNavigationActiveTypeEvent;
            if (derivedEvt.type != UILobbyNavigationBarView.UIType.Shop) return;
            Init();
        }
    }
}