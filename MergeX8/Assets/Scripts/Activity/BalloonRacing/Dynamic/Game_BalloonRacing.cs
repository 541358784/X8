using System.Collections;
using System.Collections.Generic;
using Activity.BalloonRacing;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.BalloonRacing.Dynamic
{
    public class Game_BalloonRacing : MonoBehaviour
    {
        private Button _buttonSelf;
        private LocalizeTextMeshProUGUI _textTime;
        private LocalizeTextMeshProUGUI _textRank;
        private LocalizeTextMeshProUGUI _newRankTxt;
        private LocalizeTextMeshProUGUI _textScore;

        public GameObject Target;

        private bool _lastShowState = false;
        private GameObject _redPoint;

        private int _recordIndex = -1;
        private int _recordScore = 0;

        private Animator _animator;
        private Animator _rankAnimator;

        private void Awake()
        {
            _buttonSelf = transform.GetComponent<Button>();
            _textTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _textRank = transform.Find("Root/Anim/Icon (2)/RankGroup/Mask/RankText").GetComponent<LocalizeTextMeshProUGUI>();
            _newRankTxt = transform.Find("Root/Anim/Icon (2)/RankGroup/Mask/RankNewText").GetComponent<LocalizeTextMeshProUGUI>();
            _textScore = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Target = transform.Find("Root/ItemIcon").gameObject;

            _redPoint = transform.Find("Root/RedPoint").gameObject;

            _animator = transform.Find("Root/Anim").GetComponent<Animator>();
            _rankAnimator = transform.Find("Root/Anim/Icon (2)/RankGroup").GetComponent<Animator>();
            _buttonSelf.onClick.AddListener(delegate { BalloonRacingModel.Instance.TryOpenMain(); });

            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_AWARD, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.STAGE_COLLECTION_FLY_SUCCESS, AnimationUpdate);

            InvokeRepeating("UpdateView", 0, 1);
        }


        void Start()
        {
            ScoreUpdate(null);
        }

        private void OnEnable()
        {
            ScoreUpdate(null);
        }

        private void AnimationUpdate(BaseEvent e)
        {
            int rankIndex = BalloonRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);
            int recordScore = BalloonRacingModel.Instance.Storage.PlayerList.Find(p => p.IsMe == true).CurScore;
            if (_recordIndex != -1)
            {
                //AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);
                if (_recordIndex < rankIndex)
                {
                    _animator.Play("Flashing");
                    _rankAnimator.Play("down", 0, 0);
                }
                else if (_recordScore != recordScore)
                {
                    _animator.Play("Overtake");
                }

                if (_recordIndex > rankIndex)
                {
                    _rankAnimator.Play("up", 0, 0);
                }
            }

            _textRank.SetText((_recordIndex + 1).ToString());
            _newRankTxt.SetText((rankIndex + 1).ToString());
            _recordScore = recordScore;
            _recordIndex = rankIndex;
        }


        private void ScoreUpdate(BaseEvent e)
        {
            _textScore.SetText(BalloonRacingModel.Instance.GetMyScore().ToString());
            if ((BalloonRacingModel.Instance.Storage.IsDone && BalloonRacingModel.Instance.Storage.IsAward) ||
                BalloonRacingModel.Instance.Storage.CurRacingIndex <= 0)
            {
                _textRank.SetText("");
                _newRankTxt.SetText("");
                _recordIndex = -1;
                _recordScore = 0;
            }
            else
            {
                bool isSelfUpdate = false;

                if (e != null && e.datas != null && e.datas.Length > 0)
                {
                    List<StorageBalloonRacingPlayer> listChange = (List<StorageBalloonRacingPlayer>)e.datas[0];
                    for (int i = 0; i < listChange.Count; i++)
                    {
                        if (listChange[i].IsMe)
                        {
                            isSelfUpdate = true;
                        }
                    }
                }

                if (isSelfUpdate) return;
                AnimationUpdate(null);
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
            _redPoint.SetActive(BalloonRacingModel.Instance.ShowRedPoint());
        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.BALLOON_RACING_AWARD, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.STAGE_COLLECTION_FLY_SUCCESS, AnimationUpdate);
            CancelInvoke("UpdateView");
        }
    }
}
