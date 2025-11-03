using UnityEngine;


namespace TMatch
{
    public class UILobbyMainViewActivity : UIView
    {
        [ComponentBinder("ButtonCoinCollection")]
        private Transform coinCollection;

        [ComponentBinder("ButtonDiamondsCollection")]
        private Transform diamondCollection;

        [ComponentBinder("ButtonStarChallenge")]
        private Transform starChallenge;

        [ComponentBinder("ButtonGuildCollection")]
        private Transform collectGuild;

        [ComponentBinder("ButtonPiggyBank")] private Transform piggyBank;

        protected override bool IsChildView => true;

        public override void OnViewOpen(UIViewParam param)
        {
            base.OnViewOpen(param);

            // AddChildView<CollectCoinGateView>(coinCollection.gameObject);
            // AddChildView<CollectDiamondGateView>(diamondCollection.gameObject);
            AddChildView<StarChallengeGateView>(starChallenge.gameObject);
            // AddChildView<CollectGuildGateView>(collectGuild.gameObject);
            // AddChildView<PiggyBankGateView>(piggyBank.gameObject);
        }
    }
}