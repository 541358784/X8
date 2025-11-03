using System;
using System.Collections;
using System.Collections.Generic;
using Activity.RabbitRacing.Dynamic;
using DragonPlus;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.RabbitRacing.UI
{
    public class MergeRabbitRacing : MonoBehaviour
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

            _buttonSelf.onClick.AddListener(delegate { RabbitRacingModel.Instance.TryOpenMain(); });

            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_AWARD, ScoreUpdate);
        }


        private void OnEnable()
        {
            ScoreUpdate(null);
        }


        private void ScoreUpdate(BaseEvent e)
        {
            if ((RabbitRacingModel.Instance.Storage.IsDone && RabbitRacingModel.Instance.Storage.IsAward) ||
                RabbitRacingModel.Instance.Storage.CurRacingIndex <= 0)
            {
                _textRank.SetText("0");
            }
            else
            {
                int rankIndex = RabbitRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);
                _textRank.SetText((rankIndex + 1).ToString());
            }
        }

        public void UpdateView()
        {
            gameObject.SetActive(RabbitRacingModel.Instance.IsOpened());
            if (RabbitRacingModel.Instance.IsOpened() != _lastShowState)
            {
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.RABBIT_RACING_DONE);
            }

            _lastShowState = RabbitRacingModel.Instance.IsOpened();

            if (!RabbitRacingModel.Instance.IsOpened())
                return;

            if (_textTime != null)
                _textTime.SetText(RabbitRacingModel.Instance.GetActivityLeftTimeString());
        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_AWARD, ScoreUpdate);
        }
    }
}