using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using UnityEngine;

namespace Activity.RabbitRacing.Dynamic
{
    public class Home_RabbitRacing : Aux_ItemBase
    {
        private LocalizeTextMeshProUGUI _timeText;

        private GameObject _redPoint;
        private GameObject _redPointTxt;

        private bool _checkEnd;

        private LocalizeTextMeshProUGUI _rankTxt;

        protected override void Awake()
        {
            base.Awake();
            if (!RabbitRacingModel.Instance.IsOpened())
            {
                return;
            }

            _timeText = transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rankTxt = transform.Find("Rank/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _redPointTxt = transform.Find("RedPoint/Label").gameObject;
            _redPointTxt.SetActive(false);
            _redPoint = transform.Find("RedPoint").gameObject;
            InvokeRepeating(nameof(UpdateUI), 0, 1);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_AWARD, ScoreUpdate);
            ScoreUpdate(null);
        }


        public override void UpdateUI()
        {
            if (RabbitRacingModel.Instance.IsOpened())
            {
                _checkEnd = true;
            }

            gameObject.SetActive(RabbitRacingModel.Instance.IsOpened());
            if (gameObject.activeSelf)
            {
                _timeText.SetText(RabbitRacingModel.Instance.GetActivityLeftTimeString());
                _redPoint.SetActive(RabbitRacingModel.Instance.ShowRedPoint());
            }

            // if (_checkEnd)
            // {
            //     RabbitRacingModel.Instance.TimeCheckRacingEnding();
            // }
        }

        private void ScoreUpdate(BaseEvent e)
        {
            if ((RabbitRacingModel.Instance.Storage.IsDone && RabbitRacingModel.Instance.Storage.IsAward) ||
                RabbitRacingModel.Instance.Storage.CurRacingIndex <= 0)
            {
                _rankTxt.SetText("");
            }
            else
            {
                int rankIndex = RabbitRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);
                _rankTxt.SetText((rankIndex + 1).ToString());
            }
        }

        protected override void OnButtonClick()
        {
            base.OnButtonClick();
            RabbitRacingModel.Instance.TryOpenMain();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_AWARD, ScoreUpdate);
            CancelInvoke("UpdateUI");
        }
    }
}