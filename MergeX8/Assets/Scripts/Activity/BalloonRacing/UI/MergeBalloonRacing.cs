using System;
using System.Collections;
using System.Collections.Generic;
using Activity.BalloonRacing;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BalloonRacing.UI
{
    public class MergeBallonRacing : MonoBehaviour
    {
        private Button _buttonSelf;
        private LocalizeTextMeshProUGUI _textTime;
        private LocalizeTextMeshProUGUI _textRank;

        public GameObject Target;

        private bool _lastShowState = false;

        private void Awake()
        {
            _buttonSelf = transform.GetComponent<Button>();
            _textTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _textRank = transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Target = transform.Find("Root/ItemIcon").gameObject;

            _buttonSelf.onClick.AddListener(delegate { BalloonRacingModel.Instance.TryOpenMain(); });

            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_AWARD, ScoreUpdate);
        }


        private void OnEnable()
        {
            ScoreUpdate(null);
        }


        private void ScoreUpdate(BaseEvent e)
        {
            if ((BalloonRacingModel.Instance.Storage.IsDone && BalloonRacingModel.Instance.Storage.IsAward) ||
                BalloonRacingModel.Instance.Storage.CurRacingIndex <= 0)
            {
                _textRank.SetText("0");
            }
            else
            {
                int rankIndex = BalloonRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);
                _textRank.SetText((rankIndex + 1).ToString());
            }
        }

        public void UpdateView()
        {
            gameObject.SetActive(BalloonRacingModel.Instance.IsOpened());
            if (BalloonRacingModel.Instance.IsOpened() != _lastShowState)
            {
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.BALLOON_RACING_DONE);
            }

            _lastShowState = BalloonRacingModel.Instance.IsOpened();

            if (!BalloonRacingModel.Instance.IsOpened())
                return;

            if (_textTime != null)
                _textTime.SetText(BalloonRacingModel.Instance.GetActivityLeftTimeString());
        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_AWARD, ScoreUpdate);
        }
    }
}