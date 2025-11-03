using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BattlePass
{
    public class UIBattlePassRewardController : UIWindowController
    {
        private LocalizeTextMeshProUGUI _numText;
        private int score = 0;
        private bool isRefresh;

        public override void PrivateAwake()
        {
            _numText = GetItem<LocalizeTextMeshProUGUI>("Root/Reward/Text");
            GetItem<Button>("Root/ButtonClose").onClick.AddListener(() => { ClickUIMask(); });
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            score = (int)objs[0];
            isRefresh = (bool)objs[1];

            score = BattlePassModel.Instance.MultipleScore(score);
            _numText.SetText(score.ToString());
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventBpScorePop, score.ToString());

            BattlePassModel.Instance.AddScore(score);
        }

        public override void ClickUIMask()
        {
            if (!canClickMask)
                return;

            canClickMask = false;

            CloseWindowWithinUIMgr(true);

            var bpTaskMain = UIManager.Instance.GetOpenedUIByPath<UIBattlePassMainController>(UINameConst.UIBattlePassMain);
            if (bpTaskMain != null)
            {
                //动画增加分数
                bpTaskMain.UpdateTaskUI();
            }
            else
            {
                if (isRefresh)
                    UIManager.Instance.OpenUI(UINameConst.UIPopupBattlePassRefresh, BpTaskOpenType.NewTask);
            }

            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BATTLE_PASS_STORE_REFRESH, score);
        }
    }
}