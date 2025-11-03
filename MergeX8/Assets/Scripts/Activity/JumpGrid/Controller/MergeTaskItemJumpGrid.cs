using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using Gameplay;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JumpGrid
{
    public class MergeTaskItemJumpGrid : MonoBehaviour
    {
        private Slider _slider;
        private LocalizeTextMeshProUGUI _sliderText;
        private Image _sliderIcon;
        private List<GameObject> _sliderIcons;

        private LocalizeTextMeshProUGUI _timeText;

        private Button _button;
        //private SkeletonGraphic _skeletonGraphic;
        private int _currentLevel;
        private int _currentMaxValue;
        private int _currentValue;

        private void Awake()
        {
            _slider = transform.Find("Root/Slider").GetComponent<Slider>();
            _sliderText = transform.Find("Root/Slider/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _timeText = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _sliderIcon = transform.Find("Root/Slider/Reward").GetComponent<Image>();
            _sliderIcon.gameObject.SetActive(false);
            GetComponent<Button>().onClick.AddListener(OnBtnCoinCompetition);
            _button = transform.Find("Root/Button").GetComponent<Button>();
            _button.onClick.AddListener(OnBtnClick);
            //_skeletonGraphic = transform.Find("Person/PortraitSpine/PortraitSpine1/PortraitSpine22 (3)").GetComponent<SkeletonGraphic>();
            InvokeRepeating("UpdateTimeText", 1, 1);
            UpdateTimeText();
            EventDispatcher.Instance.AddEventListener(EventEnum.JUMP_GRID_ADD_COIN, UpdateUI);
            EventDispatcher.Instance.AddEventListener(EventEnum.JUMP_GRID_REFRESH, OnRefresh);
            _sliderIcons = new List<GameObject>();
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.JUMP_GRID_ADD_COIN, UpdateUI);
            EventDispatcher.Instance.RemoveEventListener(EventEnum.JUMP_GRID_REFRESH, OnRefresh);
        }

        private void OnRefresh(BaseEvent obj)
        {
            UpdateUIOnLevelUp();
        }

        private void OnBtnCoinCompetition()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.JumpGridTask);
            UIManager.Instance.OpenUI(UINameConst.UIJumpGridMain);
        }

        public void UpdateUI(BaseEvent obj)
        {
            int store = (int)obj.datas[0];
            PerformAddValue(store, 1.5f);
        }

        private void OnEnable()
        {
            if (JumpGridModel.Instance.GetIsInit())
                InitUI();
            StopAllCoroutines();
        }

        private void OnBtnClick()
        {
            Action action = () =>
            {
                InitUI();
            };

            UIManager.Instance.OpenUI(UINameConst.UIJumpGridMain, action);
            //
            // JumpGridModel.Instance.Claim(null);
            // InitUI();
        }

        private void UpdateTimeText()
        {
            RefreshJumpGrid();
            
            _timeText.SetText(JumpGridModel.Instance.GetActivityLeftTimeString());
            if (JumpGridModel.Instance.GetIsInit() && JumpGridModel.Instance.GetActivityLeftTime() <= 0)
            {
                if (UIManager.Instance.GetOpenedUIByPath(UINameConst.UIJumpGridMain) == null && !JumpGridModel.Instance.StorageJumpGrid.IsShowEndView)
                {
                    JumpGridModel.Instance.StorageJumpGrid.IsShowEndView = true;
                    UIManager.Instance.OpenUI(UINameConst.UIJumpGridMain);
                }
            }
        }

        public void RefreshJumpGrid()
        {
            gameObject.SetActive(JumpGridModel.Instance.ShowEntrance());
        }
        
        private void InitUI()
        {
            transform.DOKill(true);
            var currentReward = JumpGridModel.Instance.GetCurrentReward();
            _currentLevel = currentReward.Id;
            _currentMaxValue = JumpGridModel.Instance.GetLevelNeedStore(_currentLevel);
            _currentValue = JumpGridModel.Instance.GetLevelHaveStore(_currentLevel);
            _button.gameObject.SetActive(JumpGridModel.Instance.IsCanClaim());
            if (JumpGridModel.Instance.IsCanClaim())
            {
                PlaySkeletonAnimation("happy");
            }
            else
            {
                PlaySkeletonAnimation("normal");
            }

            UpdateSlider();
        }

        public void UpdateUIOnLevelUp()
        {
            _currentValue -= _currentMaxValue;
            var currentReward = JumpGridModel.Instance.GetCurrentReward();
            _currentLevel = currentReward.Id;
            _currentMaxValue = JumpGridModel.Instance.GetLevelNeedStore(_currentLevel);
            _button.gameObject.SetActive(JumpGridModel.Instance.IsCanClaim());
            maxState2 = _currentValue >= _currentMaxValue;
            if (JumpGridModel.Instance.IsCanClaim())
            {
                PlaySkeletonAnimation("happy");
            }
            else
            {
                PlaySkeletonAnimation("normal");
            }

            UpdateSlider();
        }

        public void UpdateSlider()
        {
            for (int i = _sliderIcons.Count - 1; i >= 0; i--)
            {
                GameObject.Destroy(_sliderIcons[i]);
            }

            _sliderIcons.Clear();
            var cur = JumpGridModel.Instance.GetCurrentReward();
            _slider.value = JumpGridModel.Instance.GetCurrentProgress();
            _sliderText.SetText(JumpGridModel.Instance.GetCurrentProgressStr());
            for (int i = 0; i < cur.RewardId.Count; i++)
            {
                if (cur.RewardShowIndex == cur.RewardId[i])
                {
                    var item = Instantiate(_sliderIcon, _sliderIcon.transform.parent);
                    item.gameObject.SetActive(true);
                    _sliderIcons.Add(item.gameObject);
                    if (UserData.Instance.IsResource(cur.RewardId[i]))
                    {
                        item.sprite = UserData.GetResourceIcon(cur.RewardId[i]);
                    }
                    else
                    {
                        var itemConfig = GameConfigManager.Instance.GetItemConfig(cur.RewardId[i]);
                        item.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
                    }
                }
            }
        }

        public void PerformAddValue(int subNum, float time, System.Action callBack = null)
        {
            transform.DOKill(true);
            int lastUpdateSubNum = 0;
            DOTween.To(() => 0f, curSubNumF =>
            {
                var curSubNum = (int)Mathf.Floor(curSubNumF);
                var addValue = curSubNum - lastUpdateSubNum;
                lastUpdateSubNum = curSubNum;
                _currentValue += addValue;
                OnChangeValue();
            }, subNum, time).OnComplete(() =>
            {
                var addValue = subNum - lastUpdateSubNum;
                lastUpdateSubNum = addValue;
                _currentValue += addValue;
                OnChangeValue();
                callBack?.Invoke();
            }).SetTarget(transform);
        }

        private bool maxState = false;
        private bool maxState2 = false;

        public void OnChangeValue()
        {
            _sliderText.SetText(_currentValue + "/" + _currentMaxValue);
            _slider.value = Mathf.Min(_currentValue / (float)_currentMaxValue, 1f);
            var curMaxState = _currentValue >= _currentMaxValue;
            if (maxState != curMaxState)
            {
                maxState = false;
                _button.gameObject.SetActive(curMaxState);
                PlaySkeletonAnimation("happy");
            }

            if (maxState2 != curMaxState)
            {
                maxState2 = curMaxState;
                if (curMaxState)
                {
                    if (MergeTaskTipsController.Instance != null && MergeTaskTipsController.Instance.contentRect != null)
                    {
                        MergeTaskTipsController.Instance.contentRect.DOAnchorPosX(-transform.localPosition.x + 220, 0);
                    }
                }
            }
        }

        private float PlaySkeletonAnimation(string animName)
        {
            return 0;
            // if (_skeletonGraphic == null)
            //     return 0;
            //
            // TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
            // if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
            //     return trackEntry.AnimationEnd;
            //
            // _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
            // _skeletonGraphic.Update(0);
            // if (trackEntry == null)
            //     return 0;
            // return trackEntry.AnimationEnd;
        }
    }
}