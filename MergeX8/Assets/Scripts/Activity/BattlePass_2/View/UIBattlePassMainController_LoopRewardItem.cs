using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BattlePass_2
{
    public partial class UIBattlePassMainController
    {
        private LoopRewardItem LoopRewardBox;

        public void InitLoopRewardBox()
        {
            LoopRewardBox = transform.Find("Root/MiddleGroup/Scroll View/Viewport/Content/Box").gameObject
                .AddComponent<LoopRewardItem>();
            LoopRewardBox.gameObject.SetActive(true);

            Vector3 position = LoopRewardBox.transform.localPosition;
            position.y = -380 - BattlePassModel.Instance.BattlePassRewardConfig.Count * scrollDis;

            LoopRewardBox.transform.localPosition = position;
        }

        public class LoopRewardItem : MonoBehaviour
        {
            private Button CollectBtn;
            private Transform Lock;

            private void Awake()
            {
                CollectBtn = transform.Find("Button").GetComponent<Button>();
                CollectBtn.onClick.AddListener(OnClickCollectBtn);
                Lock = transform.Find("Lock");
                UpdateUI();
                EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, UpdateUIEvent);
                EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_STORE_REFRESH, UpdateUIEvent);
            }

            private void OnDestroy()
            {
                EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_PURCHASE, UpdateUIEvent);
                EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_STORE_REFRESH, UpdateUIEvent);
            }

            public void UpdateUIEvent(BaseEvent evt)
            {
                UpdateUI();
            }

            public void OnClickCollectBtn()
            {
                BattlePassModel.Instance.CollectLoopReward();
                UpdateUI();
            }

            public void UpdateUI()
            {
                CollectBtn.gameObject.SetActive(BattlePassModel.Instance.storageBattlePass.LoopRewardIsOpened() &&
                                                BattlePassModel.Instance.CurScoreRatio >= BattlePassModel.Instance.GetNextLoopRewardScore());
                Lock.gameObject.SetActive(!BattlePassModel.Instance.storageBattlePass.LoopRewardIsOpened() ||
                                          BattlePassModel.Instance.CurScoreRatio < BattlePassModel.Instance.storageBattlePass.Reward.Last().UnlockScore);
            }
        }
    }
}