using System.Collections;
using System.Collections.Generic;
using Activity.BalloonRacing;
using DragonPlus;
using UnityEngine;

namespace Activity.BalloonRacing.Dynamic
{
    public class Home_BalloonRacing : Aux_ItemBase
    {
        private LocalizeTextMeshProUGUI _timeText;

        private GameObject _redPoint;
        private GameObject _redPointTxt;

        private bool _checkEnd;

        private LocalizeTextMeshProUGUI _rankTxt;

        protected override void Awake()
        {
            base.Awake();
            if (!BalloonRacingModel.Instance.IsOpened())
            {
                return;
            }

            _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rankTxt = transform.Find("Rank/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _redPointTxt = transform.Find("RedPoint/Label").gameObject;
            _redPointTxt.SetActive(false);
            _redPoint = transform.Find("RedPoint").gameObject;
            InvokeRepeating(nameof(UpdateUI), 0, 1);
            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_AWARD, ScoreUpdate);
            ScoreUpdate(null);
        }


        public override void UpdateUI()
        {
            if (BalloonRacingModel.Instance.IsOpened())
            {
                _checkEnd = true;
            }

            gameObject.SetActive(BalloonRacingModel.Instance.IsOpened());
            if (gameObject.activeSelf)
            {
                _timeText.SetText(BalloonRacingModel.Instance.GetActivityLeftTimeString());
                _redPoint.SetActive(BalloonRacingModel.Instance.ShowRedPoint());
            }

            // if (_checkEnd)
            // {
            //     BalloonRacingModel.Instance.TimeCheckRacingEnding();
            // }
        }

        private void ScoreUpdate(BaseEvent e)
        {
            if ((BalloonRacingModel.Instance.Storage.IsDone && BalloonRacingModel.Instance.Storage.IsAward) ||
                BalloonRacingModel.Instance.Storage.CurRacingIndex <= 0)
            {
                _rankTxt.SetText("");
            }
            else
            {
                int rankIndex = BalloonRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);
                _rankTxt.SetText((rankIndex + 1).ToString());
            }
        }

        protected override void OnButtonClick()
        {
            base.OnButtonClick();
            BalloonRacingModel.Instance.TryOpenMain();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_AWARD, ScoreUpdate);
            CancelInvoke("UpdateUI");
        }
    }
}
