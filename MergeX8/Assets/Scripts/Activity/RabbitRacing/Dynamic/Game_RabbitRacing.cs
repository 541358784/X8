using System.Collections;
using System.Collections.Generic;
using Activity.RabbitRacing.Dynamic;
using Cysharp.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Storage;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.RabbitRacing.Dynamic
{
    public class Game_RabbitRacing : MonoBehaviour
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

        private SkeletonGraphic _skeletonGraphic;
        private Animator _rankAnimator;

        private void Awake()
        {
            _buttonSelf = transform.GetComponent<Button>();
            _textTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _textRank = transform.Find("Root/RankGroup/Mask/RankText").GetComponent<LocalizeTextMeshProUGUI>();
            _newRankTxt = transform.Find("Root/RankGroup/Mask/RankNewText").GetComponent<LocalizeTextMeshProUGUI>();
            _textScore = transform.Find("Root/Num/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Target = transform.Find("Root/ItemIcon").gameObject;

            _redPoint = transform.Find("Root/RedPoint").gameObject;

            _skeletonGraphic = transform.Find("Root/Icon/SkeletonGraphic (rabbit_race)").GetComponent<SkeletonGraphic>();
            _rankAnimator = transform.Find("Root/RankGroup").GetComponent<Animator>();
            _buttonSelf.onClick.AddListener(delegate { RabbitRacingModel.Instance.TryOpenMain(); });

            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_AWARD, ScoreUpdate);
            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_STAGE_COLLECTION_FLY_SUCCESS, AnimationUpdate);

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
            int rankIndex = RabbitRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.IsMe == true);
            int recordScore = RabbitRacingModel.Instance.Storage.PlayerList.Find(p => p.IsMe == true).CurScore;
            if (_recordIndex != -1)
            {
                if (_recordIndex < rankIndex)
                {
                    _rankAnimator.Play("down", 0, 0);
                }
                else if (_recordScore != recordScore)
                {
                    PlaySpineAnim("work", false, "idle", true);
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
            _textScore.SetText(RabbitRacingModel.Instance.GetMyScore().ToString());
            if ((RabbitRacingModel.Instance.Storage.IsDone && RabbitRacingModel.Instance.Storage.IsAward) ||
                RabbitRacingModel.Instance.Storage.CurRacingIndex <= 0)
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
            _redPoint.SetActive(RabbitRacingModel.Instance.ShowRedPoint());
        }


        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_JOIN, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_AWARD, ScoreUpdate);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_STAGE_COLLECTION_FLY_SUCCESS, AnimationUpdate);
            CancelInvoke("UpdateView");
        }

        private async UniTask PlaySpineAnim(string animName, bool isLoop, string endAnimName = null, bool isLoopEnd = false)
        {
            if(_skeletonGraphic.AnimationState.GetCurrent(0).Animation.Name == animName)
                return;
            
            _skeletonGraphic.AnimationState.SetAnimation(0, animName, isLoop);
            _skeletonGraphic.Update(0);
            TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
            await UniTask.WaitForSeconds(trackEntry.AnimationEnd);
            
            if(endAnimName.IsEmptyString())
                return;
            
            _skeletonGraphic.AnimationState.SetAnimation(0, endAnimName, isLoopEnd);
            _skeletonGraphic.Update(0);
        }
    }
}
