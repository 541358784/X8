using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Activity.RabbitRacing.Dynamic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.RabbitRacing.UI
{
    public class SpeedRacePlayerItem : MonoBehaviour
    {
        //最大移动距离
        // private float _balloonMaxMove;
        // private float _balloonStartPosY;

        private RectTransform _rectBalloon;

        private GameObject _objRank;
        private GameObject _objFinish;

        private Animator _finishAnimator;
        //private GameObject _objRankEffect;


        private List<GameObject> _rankList = new List<GameObject>();
        private List<GameObject> _finishBox = new List<GameObject>();


        private Text _textName;
        private LocalizeTextMeshProUGUI _textScore;

        private RectTransform _headIconRoot;
        private HeadIconNode HeadIcon;

        private Button _buttonTips;

        private StorageBalloonRacingPlayer _player;

        public int Seat;

        private SkeletonGraphic _skeletonGraphic;
        private SkeletonGraphic _finishSkeletonGraphic;

        private int _rank = -1;
        private Slider _slider;

        private void Awake()
        {
            _slider = transform.Find("Slider").GetComponent<Slider>();
            
            _rectBalloon = transform.Find("Slider/Player").GetComponent<RectTransform>();
            _objFinish = transform.Find("Finish").gameObject;
            _finishAnimator = transform.GetComponent<Animator>();
            _objRank = transform.Find("Ranking").gameObject;
            _buttonTips = transform.Find("Ranking/TipsBtn").GetComponent<Button>();
            SetTailObjState(false);
            for (int i = 1; i < 4; i++)
            {
                _rankList.Add(_objRank.transform.Find(i.ToString()).gameObject);
            }

            for (int i = 1; i < 4; i++)
            {
                _finishBox.Add(_objFinish.transform.Find("1/Image/Player/Root/Spine1/"+i).gameObject);
            }

            _finishSkeletonGraphic = _objFinish.transform.Find("1/Image/Player/Root/Spine1").GetComponent<SkeletonGraphic>();
            _objFinish.SetActive(false);
            _objRank.SetActive(false);

            _skeletonGraphic = transform.Find("Slider/Player/Root/Spine").GetComponent<SkeletonGraphic>();
            _textName = transform.Find("PlayerHead/Text").GetComponent<Text>();
            _textScore = transform.Find("Slider/Player/Root/TopBubble/NumText").GetComponent<LocalizeTextMeshProUGUI>();
            _headIconRoot = transform.Find("PlayerHead/HeadItem/Icon") as RectTransform;


            _buttonTips.onClick.AddListener(delegate
            {
                if (_rank >= RabbitRacingModel.Instance.CurRacing.RewardRank)
                {
                    return;
                }

                List<ResData> listReward = new List<ResData>();

                string content = "";
                switch (_rank)
                {
                    case 0:
                        listReward = CommonUtils.FormatReward(RabbitRacingModel.Instance.CurRacing.RewardType1,
                            RabbitRacingModel.Instance.CurRacing.RewardNumber1);
                        break;
                    case 1:
                        listReward = CommonUtils.FormatReward(RabbitRacingModel.Instance.CurRacing.RewardType2,
                            RabbitRacingModel.Instance.CurRacing.RewardNumber2);

                        break;
                    case 2:
                        listReward = CommonUtils.FormatReward(RabbitRacingModel.Instance.CurRacing.RewardType3,
                            RabbitRacingModel.Instance.CurRacing.RewardNumber3);
                        break;
                }

                var pos = _buttonTips.transform.position + new Vector3(0, 0.3f, 0);
                CommonRewardManager.Instance.ShowNormalBoxReward(pos, listReward);
            });


            RewardTipController.RegisterTips(_buttonTips.transform as RectTransform);

            EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE_RANK, RankUpdate);
        }

        private void OnDestroy()
        {
            RewardTipController.UnRegisterTips(_buttonTips.transform as RectTransform);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.RABBIT_RACING_SCORE_UPDATE_RANK, RankUpdate);
        }

        private void RankUpdate(BaseEvent e)
        {
            int rankIndex = RabbitRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.Seat == Seat);

            if (rankIndex == _rank)
                return;

            _rank = rankIndex;


            //_objRankEffect.SetActive(false);
            //_objRankEffect.SetActive(true);

            _objRank.SetActive(_rank < RabbitRacingModel.Instance.CurRacing.RewardRank);

            for (int i = 0; i < _rankList.Count; i++)
            {
                _rankList[i].gameObject.SetActive(i == _rank);
            }
        }


        public void Init(int rank, StorageBalloonRacingPlayer player)
        {
            _rank = rank;
            _player = player;
            Seat = player.Seat;
            // _topTransform = topTransform;
            // _bottomTransform = bottomTransform;
            //
            // _balloonMaxMove = Mathf.Abs(_topTransform.position.y - _bottomTransform.position.y);
            // _balloonStartPosY = _bottomTransform.position.y;

            if (_player.IsMe)
            {
                _textName.text = LocalizationManager.Instance.GetLocalizedString("UI_race_user");
                _finishSkeletonGraphic.Skeleton.SetSkin("2");
            }
            else
            {
                _textName.text = _player.PlayerName;
                _finishSkeletonGraphic.Skeleton.SetSkin("1");
            }

            int showScore = _player.CurScore;
            if (RabbitRacingModel.Instance.Storage.IsDone && !RabbitRacingModel.Instance.Storage.IsAward &&
                _player.LastShowScore >= RabbitRacingModel.Instance.CurRacing.Collect)
            {
                showScore = _player.LastShowScore;
                _rank = _player.LastShowRank;
            }

            bool isShowAnimation = false;
            if (showScore != _player.LastShowScore)
            {
                float percent = _player.LastShowScore * 1f / RabbitRacingModel.Instance.CurRacing.Collect > 1f
                    ? 1f
                    : _player.LastShowScore * 1f / RabbitRacingModel.Instance.CurRacing.Collect;
                _slider.value = percent;

                SetSkeletonAnim("idle", true);
                //_rectBalloon.position = new Vector2(_rectBalloon.position.x, _balloonStartPosY + _balloonMaxMove * percent);
                isShowAnimation = true;
                _textScore.SetText(_player.LastShowScore.ToString());
                XUtility.WaitSeconds(0.5f, () => { DoRefreshAnim(player); });
            }
            else
            {
                float percent = showScore * 1f / RabbitRacingModel.Instance.CurRacing.Collect > 1f
                    ? 1f
                    : showScore * 1f / RabbitRacingModel.Instance.CurRacing.Collect;
                //_rectBalloon.position = new Vector2(_rectBalloon.position.x, _balloonStartPosY +_balloonMaxMove * percent);
                _slider.value = percent;
                SetSkeletonAnim("idle", true);
                _textScore.SetText(showScore.ToString());
            }

            _objRank.SetActive(_rank < RabbitRacingModel.Instance.CurRacing.RewardRank);
            for (int i = 0; i < _rankList.Count; i++)
            {
                _rankList[i].gameObject.SetActive(i == _rank);
            }

            UpdateHeadIcon();
            if (player.IsDone && !isShowAnimation)
            {
                _skeletonGraphic.gameObject.SetActive(false);
                _objFinish.gameObject.SetActive(true);

                for (int i = 0; i < _finishBox.Count; i++)
                {
                    _finishBox[i].gameObject.SetActive(i == _rank);
                }

                if (!IsAnimationPlaying("Finish") && !IsAnimationPlaying("Finish_Normal"))
                {
                    _finishAnimator.Play("Finish_Normal");
                    _objRank.SetActive(false);
                }
            }
            else
            {
                _skeletonGraphic.gameObject.SetActive(true);
                _finishAnimator.Play("Normal");
            }
        }

        public AvatarViewState GetAvatarViewState(StorageBalloonRacingPlayer playerStorage)
        {
            if (playerStorage.IsMe)
                return HeadIconUtils.GetMyViewState();
            else
            {
                return new AvatarViewState(playerStorage.PlayerHead, -1, "robot", false);
            }
        }

        public void UpdateHeadIcon(BaseEvent e = null)
        {
            if (_headIconRoot)
            {
                if (HeadIcon)
                {
                    HeadIcon.SetAvatarViewState(GetAvatarViewState(_player));
                }
                else
                {
                    HeadIcon = HeadIconNode.BuildHeadIconNode(_headIconRoot, GetAvatarViewState(_player));
                    HeadIcon.ShowHeadIconFrame(false);
                }
            }
        }

        public bool IsAnimationPlaying(string animationName)
        {
            AnimatorStateInfo stateInfo = _finishAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(animationName);
        }

        public bool IsMove { get; set; } = false;

        public async Task DoRefreshAnim(StorageBalloonRacingPlayer targetData, float duration = 1f)
        {
            if (IsMove) return;
            IsMove = true;
            SetTailObjState(true);
            var curScore = _player.LastShowScore;
            var targetScore = targetData.CurScore;
            DOTween.To(() => curScore, x => curScore = x, targetScore, duration)
                .OnUpdate(delegate { _textScore.SetText(curScore.ToString()); }).OnComplete(delegate
                {
                    IsMove = false;
                    _textScore.SetText(targetScore.ToString());
                });

            var task = new TaskCompletionSource<bool>();
            float percent = targetScore * 1f / RabbitRacingModel.Instance.CurRacing.Collect > 1f
                ? 1f
                : targetScore * 1f / RabbitRacingModel.Instance.CurRacing.Collect;
            //float targetY = _balloonStartPosY + _balloonMaxMove * percent;

            if (targetScore > _player.LastShowScore && _player.LastShowScore < RabbitRacingModel.Instance.CurRacing.Collect)
            {
                AudioManager.Instance.PlaySound(1826);
            }

            SetSkeletonAnim("work", true);
            _slider.DOValue(percent, duration).OnComplete(delegate
            {
                SetTailObjState(false);
                _player = targetData;

                SetSkeletonAnim("idle", true);
                //if (RabbitRacingModel.Instance.Storage.IsDone)
                {
                    int rankIndex = RabbitRacingModel.Instance.Storage.PlayerList.FindIndex(p => p.Seat == Seat);


                    if (targetData.IsDone)
                    {
                        if (rankIndex < RabbitRacingModel.Instance.CurRacing.RewardRank)
                        {
                            _skeletonGraphic.gameObject.SetActive(false);
                            _objFinish.SetActive(true);
                            _objRank.SetActive(false);

                            for (int i = 0; i < _finishBox.Count; i++)
                            {
                                _finishBox[i].gameObject.SetActive(i == rankIndex);
                            }

                            if (!IsAnimationPlaying("Finish") && !IsAnimationPlaying("Idle"))
                            {
                                _finishAnimator.Play("Finish");
                            }
                        }
                    }
                    else
                    {
                        if (rankIndex < RabbitRacingModel.Instance.CurRacing.RewardRank)
                        {
                            //_objRankEffect.SetActive(false);
                            //_objRankEffect.SetActive(true);

                            _objRank.SetActive(true);

                            for (int i = 0; i < _rankList.Count; i++)
                            {
                                _rankList[i].gameObject.SetActive(i == rankIndex);
                            }
                        }
                        else
                        {
                            _objRank.SetActive(false);
                        }
                    }
                }
                //else
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.RABBIT_RACING_SCORE_UPDATE_RANK);
                }

                task.SetResult(true);
            });

            await task.Task;
        }

        private void SetTailObjState(bool isShow)
        {
            //_tailObj.SetActive(isShow);
        }

        private void SetSkeletonAnim(string anim, bool isLoop)
        {
            _skeletonGraphic.AnimationState?.SetAnimation(0, anim, isLoop);
            _skeletonGraphic.Update(0);
        }
    }
}