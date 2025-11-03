using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.JumpGrid;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JumpGrid
{
    public class UIJumpGridMainController : UIWindowController
    {
        private Button _closeBtn;

        private List<UIJumpGridRewardItem> _rewardItems = new List<UIJumpGridRewardItem>();

        private LocalizeTextMeshProUGUI _timeText;
        private LocalizeTextMeshProUGUI _buttonText;
        private Button _button;

        private Slider _slider;
        private LocalizeTextMeshProUGUI _sliderText;

        private SkeletonGraphic _skeletonGraphic;

        private List<List<RewardData>> _rewardData = new List<List<RewardData>>();

        private Action _action;
        private bool _isAnim = false;

        private int[] _offsetY = new int[]
        {
            45,
            45,
            45,
            45,
            45,
            25,
            25,
            25,
            25,
            25,
            45
        };
        
        public override void PrivateAwake()
        {
            _closeBtn = GetItem<Button>("Root/ButtonClose");
            _closeBtn.onClick.AddListener(OnBtnClose);
            
            _button = GetItem<Button>("Root/Button");
            _buttonText = GetItem<LocalizeTextMeshProUGUI>("Root/Button/Text");
            _button.onClick.AddListener(OnBtnGo);
            
            _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/TimeGroup/TimeText");
            
            _slider = GetItem<Slider>("Root/TopGroup/Reward/Slider");
            _sliderText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/Reward/Score");

            for (int i = 1; i <= 3; i++)
            {
                _rewardData.Add(new List<RewardData>());
                var rewardItem = transform.Find($"Root/TopGroup/Reward/RewardIcon/{i}");
                for (int j = 1; j <= i; j++)
                {
                    RewardData data = new RewardData();

                    data.gameObject = rewardItem.Find($"Item{j}").gameObject;
                    data.image = rewardItem.Find($"Item{j}/Icon").GetComponent<Image>();
                    
                    _rewardData.Last().Add(data);
                }
            }
            
            for (int i = 1; i <= 11; i++)
            {
                var item = GetItem($"Root/Game/Level/{i}").AddComponent<UIJumpGridRewardItem>();
                _rewardItems.Add(item);
            }

            _skeletonGraphic = transform.Find("Root/Game/Dog").GetComponent<SkeletonGraphic>();

            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JumpGridPreview, transform as RectTransform);
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_button.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.JumpGridPlay, _button.transform as RectTransform, topLayer: topLayer);

            InvokeRepeating("UpdateTimeText", 1, 1);

            InitEntityPosition();
        }


        protected override void OnOpenWindow(params object[] objs)
        {
            if (objs != null && objs.Length > 0)
            {
                _action = (Action)objs[0];
            }
            
            UpdateUI();
            UpdateTimeText();
            
            var rewards = JumpGridConfigManager.Instance.GetRewards();
            for (int i = 0; i < _rewardItems.Count; i++)
            {
                _rewardItems[i].Init(rewards[i], i);
            }
            
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JumpGridPreview, null);
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.JumpGridPlay, null);

            if (_action != null)
            {
                ClaimReward();
            }
        }

        private async UniTask ClaimReward()
        {
            if (!JumpGridModel.Instance.IsCanClaim())
                return;
            
            var currentReward = JumpGridModel.Instance.GetCurrentReward();
            await EntityAnim(currentReward.Id);
            
            JumpGridModel.Instance.Claim(() => { UpdateUI(); }, true);
        }

        private void UpdateUI()
        {
            if (JumpGridModel.Instance.IsExchangeAll())
                JumpGridModel.Instance.StorageJumpGrid.IsShowEndView = true;

            var currentReward = JumpGridModel.Instance.GetCurrentReward();
            UpdateSlider(currentReward);
            UpdateBtnStatus();
            
            _rewardItems.ForEach(a=>a.UpdateStatus());
        }

        
        public void UpdateSlider(TableJumpGridReward cur)
        {
            foreach (var rewardData in _rewardData)
            {
                foreach (var data in rewardData)
                {
                    data.gameObject.SetActive(false);
                }
            }

            int need = JumpGridModel.Instance.GetLevelNeedStore(cur.Id);
            _slider.value = (float)(JumpGridModel.Instance.StorageJumpGrid.TotalScore - cur.Score + need) / (need);
            _sliderText.SetText((JumpGridModel.Instance.StorageJumpGrid.TotalScore - cur.Score + need) + "/" + need);
            for (int i = 0; i < cur.RewardId.Count; i++)
            {
                _rewardData[cur.RewardId.Count-1][i].gameObject.SetActive(true);
                _rewardData[cur.RewardId.Count-1][i].UpdateReward(cur.RewardId[i], 1);
            }
        }

        private void UpdateBtnStatus()
        {
            _button.gameObject.SetActive(!JumpGridModel.Instance.StorageJumpGrid.IsShowEndView);
            
            if (JumpGridModel.Instance.IsCanClaim())
                _buttonText.SetTerm("UI_button_claim");
            else
            {
                _buttonText.SetTerm("ui_goldmatch_button1");
            }
        }

        private void OnBtnGo()
        {
            if (JumpGridModel.Instance.IsCanClaim())
            {
                if(_isAnim)
                    return;
                
                ClaimReward();
                //JumpGridModel.Instance.Claim(() => { UpdateUI(); }, true);
            }
            else
            {
                AnimCloseWindow(() =>
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                    {
                        SceneFsm.mInstance.TransitionGame();
                        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.JumpGridPlay, null);
                    }
                });

            }
        }

        private void UpdateTimeText()
        {
            _timeText.SetText(JumpGridModel.Instance.GetActivityLeftTimeString());
        }

        private void OnBtnClose()
        {
            if(_isAnim)
                return;
            
            AnimCloseWindow();
            
            _action?.Invoke();
        }

        private void InitEntityPosition()
        {
            int index = JumpGridModel.Instance.StorageJumpGrid.AnimIndex;
            if(JumpGridModel.Instance.StorageJumpGrid.AnimIndex <= 0)
               return;

            index -= 1;
            if(index >= _rewardItems.Count)
                return;
            
            Vector3 localPosition = _rewardItems[index].transform.localPosition;
            localPosition.y -= _offsetY[index];
            _skeletonGraphic.transform.localPosition = localPosition;
        }

        private async UniTask EntityAnim(int index)
        {
            if(JumpGridModel.Instance.StorageJumpGrid.AnimIndex == index)
                return;

            _isAnim = true;
            InitEntityPosition();
            
            index = index-1;
            if(index >= _rewardItems.Count)
                return;

            PlaySkeletonAnimation("jump", false);
            Vector3 localPosition = _rewardItems[index].transform.localPosition;
            localPosition.y -= _offsetY[index];
           await UniTask.WaitForSeconds(0.6f);
           _skeletonGraphic.transform.DOLocalMove(localPosition, 0.5f).SetEase(Ease.Linear);
           await UniTask.WaitForSeconds(0.8f);
           PlaySkeletonAnimation(index+1 >= _rewardItems.Count ? "happy" : "idle", true);
           await UniTask.WaitForSeconds(0.3f);
           JumpGridModel.Instance.StorageJumpGrid.AnimIndex = index+1;
           _isAnim = false;
        }
        
        private float PlaySkeletonAnimation(string animName, bool isLoop)
        {
            if (_skeletonGraphic == null)
                return 0;

            TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
            if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
                return trackEntry.AnimationEnd;

            _skeletonGraphic.AnimationState?.SetAnimation(0, animName, isLoop);
            _skeletonGraphic.Update(0);

            return trackEntry.AnimationEnd;
        }
    }
}