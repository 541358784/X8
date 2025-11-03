using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
// using LobbyMain.Decoration.UI;
using UnityEngine;
using UnityEngine.UI;


namespace TMatch
{
    public class UILobbyMainView : UIView
    {
        [ComponentBinder("UIMainGroup")] private Transform mainGroup;
        [ComponentBinder("LevelGroup")] private Transform levelGroup;
        [ComponentBinder("ButtonGradeBox")] private Transform _levelChestButton;
        [ComponentBinder("ButtonStars")] private Transform starChestButton;
        [ComponentBinder("ButtonSignIn")] private Transform signButton;

        [ComponentBinder("WeekChallengeGroup")]
        private Transform weekChallengeGroup;

        [ComponentBinder("ButtonMailBox")] private Transform mailBoxButton;
        [ComponentBinder("ButtonADS")] private Transform adsButton;
        [ComponentBinder("ButtonDiscount")] private Transform adsCoinButton;
        [ComponentBinder("btnAsmr")] private Transform asmrButton;
        [ComponentBinder("Entrance_TMBP")] public Transform bpEntranceButton;
        [ComponentBinder("ButtonCrocodile")] public Transform crocodileButton;
        [ComponentBinder("ButtonGiftBagLink")] public Transform giftBagLinkButton;
        
        public UILobbyMainViewLevelButton LevelButtonView;

        protected override bool IsChildView => true;

        public override void OnViewOpen(UIViewParam data)
        {
            base.OnViewOpen(data);

            AddChildView<UILobbyMainViewActivity>(mainGroup.gameObject);
            LevelButtonView = AddChildView<UILobbyMainViewLevelButton>(levelGroup.gameObject);
            AddChildView<UILobbyMainViewLevelChest>(_levelChestButton.gameObject);
            AddChildView<UILobbyMainViewStarChest>(starChestButton.gameObject);
            // AddChildView<UILobbyMainViewSignButton>(signButton.gameObject);
            AddChildView<WeeklyChallengeGateView>(weekChallengeGroup.gameObject);
            // AddChildView<UILobbyMainViewMailBoxButton>(mailBoxButton.gameObject);
            AddChildView<UILobbyMainViewAdsButton>(adsButton.gameObject);
            AddChildView<UILobbyMainViewAdsButton>(adsCoinButton.gameObject);

            // transform.Find("UIDecorationMain").gameObject.SetActive(true);
            // transform.Find("UIDecorationMain").gameObject.AddComponent<Main_DecorationController>();
            // transform.Find("UIDecorationMain").gameObject.GetComponent<Main_DecorationController>().Hide();

            EventDispatcher.Instance.AddEventListener(EventEnum.LobbyMainShowState, Show);
            
            if (TMBPModel.Instance.IsDataValid)
            {
                bpEntranceButton.GetOrCreateComponent<TM_BPEntrance>();
            }
            // else
            // {
            //     TMBPModel.CanShowUnCollectRewardsUI();
            // }
            bpEntranceButton.gameObject.SetActive(TMBPModel.Instance.IsDataValid);
            
            crocodileButton.GetOrCreateComponent<CrocodileGateView>();
            crocodileButton.gameObject.SetActive(true);

            giftBagLinkButton.GetOrCreateComponent<GiftBagLinkEntranceView>();
            giftBagLinkButton.gameObject.SetActive(true);

            transform.Find("UIMainGroup/LeftGroup/TMWinPrizeEntranceContainer").gameObject.AddComponent<LobbyEntrance_TMWinPrizeCreator>().Init();
            transform.Find("LevelGroup/LevelButton/TMWinPrizeLabelContainer").gameObject
                .AddComponent<TMWinPrizeLabelCreator>().gameObject.SetActive(true);
        }

        public override Task OnViewClose()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.LobbyMainShowState, Show);
            return base.OnViewClose();
        }

        public void Show(BaseEvent evt)
        {
            LobbyMainShowStateEvent realEvt = evt as LobbyMainShowStateEvent;
            if (realEvt.enable)
            {
                DOTween.Kill(transform.Find("LevelGroup").GetComponent<RectTransform>());
                transform.Find("LevelGroup").GetComponent<RectTransform>().DOAnchorPosY(302.0f, 0.3f);
                DOTween.Kill(transform.Find("UIMainGroup/LeftGroup").GetComponent<RectTransform>());
                transform.Find("UIMainGroup/LeftGroup").GetComponent<RectTransform>().DOAnchorPosX(80.0f, 0.3f);
                DOTween.Kill(transform.Find("UIMainGroup/RightGroup").GetComponent<RectTransform>());
                transform.Find("UIMainGroup/RightGroup").GetComponent<RectTransform>().DOAnchorPosX(-80.0f, 0.3f);
            }
            else
            {
                AudioSysManager.Instance.PlaySound(SfxNameConst.panel_out);
                DOTween.Kill(transform.Find("LevelGroup").GetComponent<RectTransform>());
                transform.Find("LevelGroup").GetComponent<RectTransform>().DOAnchorPosY(-160.0f, 0.3f);
                DOTween.Kill(transform.Find("UIMainGroup/LeftGroup").GetComponent<RectTransform>());
                transform.Find("UIMainGroup/LeftGroup").GetComponent<RectTransform>().DOAnchorPosX(-100, 0.3f);
                DOTween.Kill(transform.Find("UIMainGroup/RightGroup").GetComponent<RectTransform>());
                transform.Find("UIMainGroup/RightGroup").GetComponent<RectTransform>().DOAnchorPosX(100, 0.3f);
            }
        }
    }
}