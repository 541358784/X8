using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

// public class MergeDogHopeRewardItem
// {
//    public Image _rewardImage;
//    public LocalizeTextMeshProUGUI _rewardCountText;
//    public GameObject _rewardObj;
// }

public class MergeDogHope : MonoBehaviour
{
    private Image Slider;
    private Image _needImage;
    private LocalizeTextMeshProUGUI _needText;
    private LocalizeTextMeshProUGUI _countDownTime;
    private DogHopeReward _dogHope;
    private Button _butDog;
    private SkeletonGraphic _skeletonGraphic;

    private Image rewardItem;

    // private List<MergeDogHopeRewardItem> _rewardItems = new List<MergeDogHopeRewardItem>();
    private LocalizeTextMeshProUGUI _rankText;

    private void Awake()
    {
        Slider = transform.Find("Slider").GetComponent<Image>();

        _butDog = transform.GetComponent<Button>();
        _butDog.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.DogHopeLeaderBoardMergeEntrance);
            UIDogMainController controller =
                (UIDogMainController) UIManager.Instance.OpenUI(UINameConst.UIDogMain);
            controller.SetStartButtonStatus(false);
        });

        _needImage = transform.Find("Icon").GetComponent<Image>();
        _needText = transform.Find("Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _countDownTime = transform.Find("TimeGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _skeletonGraphic = transform.Find("Person/PortraitSpine/PortraitSpine").GetComponent<SkeletonGraphic>();

        rewardItem = transform.Find("RewardIcon").GetComponent<Image>();

        InvokeRepeating("RefreshCountDown", 0, 1f);
        _rankText = transform.Find("Root/RankText").GetComponent<LocalizeTextMeshProUGUI>();
        _rankText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InitUI();
        StopAllCoroutines();
        PlaySkeletonAnimation("normal");
    }

    public void RefreshState()
    {
        gameObject.SetActive(DogHopeModel.Instance.IsOpenActivity());
        if (gameObject.activeSelf)
        {
            UpdateRankText();
        }
    }

    public void UpdateRankText()
    {
        var enterLeaderBoard = DogHopeModel.Instance.CurStorageDogHopeWeek.LeaderBoardStorage.IsInitFromServer();
        _rankText.gameObject.SetActive(true);
        if (enterLeaderBoard)
        {
            _rankText.SetText("No." + DogHopeModel.Instance.CurStorageDogHopeWeek.LeaderBoardStorage.SortController()
                .MyRank);
        }
        else
        {
            _rankText.SetText(DogHopeModel.Instance.CurStorageDogHopeWeek.TotalScore + "/" +
                              DogHopeLeaderBoardModel.Instance.LeastEnterBoardScore);
        }
    }

    private void InitUI()
    {
        UpdateRankText();
        _dogHope = DogHopeModel.Instance.GetCurIndexData();
        if (_dogHope != null)
        {
            Slider.gameObject.SetActive(true);
            rewardItem.gameObject.SetActive(true);
            _needText.gameObject.SetActive(true);
            _needImage.gameObject.SetActive(true);
            rewardItem.sprite = UserData.GetResourceIcon(_dogHope.RewardId[0]);
            // for (int i = 0; i < _dogHope.RewardId.Count; i++)
            // {
            //    if(i >= _rewardItems.Count)
            //       continue;
            //    
            //    _rewardItems[i]._rewardObj.SetActive(true);
            //    _rewardItems[i]._rewardImage.sprite = UserData.GetResourceIcon(_dogHope.RewardId[i]);
            //    _rewardItems[i]._rewardCountText.SetText(_dogHope.RewardNum[i].ToString());
            // }

            _needImage.sprite = UserData.GetResourceIcon(DogHopeModel._dogCookiesId);

            int stateScore = DogHopeModel.Instance.GetIndexStageScore();
            int curScore = Math.Max(0, DogHopeModel.Instance.GetCurStageScore());

            _needText.SetText(curScore + "/" + stateScore);
            Slider.fillAmount = (float) curScore / stateScore;
        }
        else
        {
            Slider.gameObject.SetActive(true);
            rewardItem.gameObject.SetActive(false);
            _needText.gameObject.SetActive(true);
            _needImage.gameObject.SetActive(true);
            _needImage.sprite = UserData.GetResourceIcon(DogHopeModel._dogCookiesId);
            _needText.SetText(DogHopeModel.Instance.CurStorageDogHopeWeek.TotalScore.ToString());
            Slider.fillAmount = 1;
        }
    }

    private void RefreshCountDown()
    {
        RefreshState();
        
        _countDownTime.SetText(DogHopeModel.Instance.GetActivityLeftTimeString());
    }

    public void UpdateText(int newScore, int subNum, int oldIndex, int curIndex, float time,
        System.Action callBack = null)
    {
        PlayDogAnim();
        if (oldIndex == curIndex && DogHopeModel.Instance.GetCurIndexData(oldIndex) == null)
        {
            InitUI();
        }
        else
        {
            int oldValue = newScore - subNum;
            int newValue = 0;
            int stageScore = 0;

            if (curIndex > oldIndex)
            {
                var oldData = DogHopeModel.Instance.GetCurIndexData(oldIndex);
                stageScore = DogHopeModel.Instance.GetIndexStageScore(oldIndex);

                oldValue = stageScore - (subNum - (newScore - oldData.Score));
                newValue = stageScore;
            }
            else
            {
                stageScore = DogHopeModel.Instance.GetIndexStageScore(curIndex);
                var preData = DogHopeModel.Instance.GetPreIndexData(curIndex);
                int preScore = preData == null ? 0 : preData.Score;

                oldValue = newScore - preScore - subNum;
                newValue = newScore - preScore;
            }

            _needText.SetText(oldValue.ToString() + "/" + stageScore);
            Slider.fillAmount = (float) oldValue / stageScore;
            oldValue = Math.Max(oldValue, 0);
            newValue = Math.Max(newValue, 0);

            var tempDogHopeData = _dogHope;
            DOTween.To(() => oldValue, x => oldValue = x, newValue, time).OnUpdate(() =>
            {
                _needText.SetText(oldValue.ToString() + "/" + stageScore);
                Slider.fillAmount = (float) oldValue / stageScore;
            }).OnComplete(() =>
            {
                if (curIndex > oldIndex || DogHopeModel.Instance.CanManualActivity())
                {
                    for (int i = 0; i < tempDogHopeData.RewardId.Count; i++)
                    {
                        // if(i >= _rewardItems.Count)
                        //    continue;

                        int index = i;
                        float delayTime = 0.3f;
                        if (tempDogHopeData.RewardNum[i] >= 5)
                            delayTime = 0.1f;
                        FlyGameObjectManager.Instance.FlyCurrency(tempDogHopeData.RewardId[i],
                            tempDogHopeData.RewardNum[i], rewardItem.transform.position, 1, false, delayTime, action:
                            () =>
                            {
                                if (index == tempDogHopeData.RewardId.Count - 1)
                                {
                                    DogHopeModel.Instance.CheckActivitySuccess();

                                    InitUI();

                                    if (DogHopeModel.Instance.CanManualActivity())
                                    {
                                        DogHopeModel.Instance.EndActivity(true);
                                    }

                                    callBack?.Invoke();
                                }
                            });
                    }
                }
                else
                {
                    callBack?.Invoke();
                }
            });
        }
    }

    private void PlayDogAnim()
    {
        StopAllCoroutines();
        float time = PlaySkeletonAnimation("happy");
        StartCoroutine(CommonUtils.DelayWork(time, () => { PlaySkeletonAnimation("normal"); }));
    }

    private float PlaySkeletonAnimation(string animName)
    {
        if (_skeletonGraphic == null)
            return 0;

        TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
        if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
            return trackEntry.AnimationEnd;

        _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
        _skeletonGraphic.Update(0);

        return trackEntry.AnimationEnd;
    }
}