using System.Collections.Generic;
using Activity.BalloonRacing;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine;
using UnityEngine.UI;

namespace Activity.BalloonRacing.UI
{
    public class UIBalloonRacingRewardController : UIWindowController
    {
        public static void Open(List<ResData> rewardDataList, int selfRank)
        {
            UIManager.Instance.OpenUI(UINameConst.UIBalloonRacingReward, rewardDataList, selfRank);
        }

        private Button CloseBtn;
        private Image HeadIcon;
        private LocalizeTextMeshProUGUI RankText;

        private List<ResData> _rewardDataList;
        private int _selfRank;

        public override void PrivateAwake()
        {
            CloseBtn = GetItem<Button>("Root/ButtonClose");
            CloseBtn.onClick.AddListener(OnClickCloseBtn);
            HeadIcon = GetItem<Image>("Root/Player/Head");
            RankText = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);
            _rewardDataList = (List<ResData>)objs[0];
            _selfRank = (int)objs[1];
            SetData();
            AudioManager.Instance.PlaySound(39);
        }

        private void SetData()
        {
            for (var i = 1; i <= 3; i++)
            {
                transform.Find("Root/Box/" + i).gameObject.SetActive(i == _selfRank);
            }

            // HeadIcon.sprite = CommonUtils.GetSelfHead();
            RankText.SetTermFormats(_selfRank.ToString());
        }

        private void OnClickCloseBtn()
        {
            CloseBtn.enabled = false;
            UserData.Instance.AddRes(_rewardDataList, new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.RaceGet));
            var mainPopup = UIManager.Instance.GetOpenedUIByPath<UISpeedRaceMainController>(UINameConst.UIBalloonRacingMain);
            if (mainPopup)
            {
                mainPopup.AnimCloseWindow();
            }

            AnimCloseWindow(() =>
            {
                UIBalloonRacingOpenBoxController.Open(_rewardDataList, _selfRank,
                    () =>
                    {
                        EventDispatcher.Instance.DispatchEvent(EventEnum.BALLOON_RACING_AWARD);
                        BalloonRacingModel.Instance.TryOpenMain();
                    });
            });
        }
    }
}