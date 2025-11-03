using System;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.JungleAdventure;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JungleAdventure.Controller
{
    public partial class UIJungleAdventureMainController
    {
        public class RewardTipsItem
        {
            private Image _image;
            private LocalizeTextMeshProUGUI _text;
            private GameObject _gameObject;
            public GameObject GameObject
            {
                get { return _gameObject; }
            }
            
            public RewardTipsItem(GameObject gameObject)
            {
                _gameObject = gameObject;
                
                _image = _gameObject.transform.Find("Item/Icon").GetComponent<Image>();
                _text = _gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            }

            public void UpdateReward(int id, int num)
            {
                _image.sprite = UserData.GetResourceIcon(id,UserData.ResourceSubType.Big);
                _text?.SetText(num.ToString());
            }
        }
        
        private LocalizeTextMeshProUGUI _timeText;
        private Slider _slider;
        private Image _rewardIcon;
        private Button _rewardButton;
        private Button _playButton;

        private GameObject _rewardTipsObject;
        private GameObject _rewardTipsItem;

        private LocalizeTextMeshProUGUI _scoreText;
        
        private Coroutine _coroutine;
        private List<RewardTipsItem> _rewardTipsItems = new List<RewardTipsItem>();
        private void AwakeUI()
        {
            _slider = transform.Find("Root/TopGroup/Reward/Slider").GetComponent<Slider>();
            _timeText = transform.Find("Root/TopGroup/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardIcon = transform.Find("Root/TopGroup/Reward/RewardIcon").GetComponent<Image>();
            _rewardButton = transform.Find("Root/TopGroup/Reward/RewardIcon").GetComponent<Button>();
            _rewardButton.onClick.AddListener(OpenRewardInfo);
            _playButton= transform.Find("Root/Button").GetComponent<Button>();
            _playButton.onClick.AddListener(OnPlay);
            _playButton.gameObject.SetActive(false);

            _scoreText = transform.Find("Root/TopGroup/Reward/Score").GetComponent<LocalizeTextMeshProUGUI>();
            
            _rewardTipsObject = transform.transform.Find("Root/TopGroup/Reward/Tips").gameObject;
            _rewardTipsObject.gameObject.SetActive(false);
            _rewardTipsItem = transform.Find("Root/TopGroup/Reward/Tips/Item").gameObject;
            _rewardTipsItem.gameObject.SetActive(false);
            
            InvokeRepeating("UpdateTime", 0, 1);
        }

        private void OpenRewardInfo()
        {
            _rewardTipsObject.gameObject.SetActive(false);
            
            if(JungleAdventureModel.Instance.IsFinish())
                return;
            
            int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);
            if(config == null)
                return;
            
            _rewardTipsObject.gameObject.SetActive(true);
            _rewardTipsItems.ForEach(a=>a.GameObject.SetActive(false));

            int num = config.RewardIds.Count - _rewardTipsItems.Count;
            for (int i = 0; i < num; i++)
            {
                GameObject reward = Instantiate(_rewardTipsItem, _rewardTipsItem.transform.parent, false);
                reward.gameObject.SetActive(true);
                    
                var rewardItem = new RewardTipsItem(reward.gameObject);
                _rewardTipsItems.Add(rewardItem);
            }

            for (int i = 0; i < config.RewardIds.Count; i++)
            {
                _rewardTipsItems[i].GameObject.SetActive(true);
                _rewardTipsItems[i].UpdateReward(config.RewardIds[i], config.RewardNums[i]);
            }
            
            if(_coroutine != null)
               StopCoroutine( _coroutine);

            _coroutine = StartCoroutine(WaitCloseTips());
        }

        private IEnumerator WaitCloseTips()
        {
            yield return new WaitForSeconds(3);
        
            _rewardTipsObject.gameObject.SetActive(false);
            _coroutine = null;
        }
        
        private void InitUI()
        {
            RefreshReward();
        }

        private void UpdateTime()
        {
            if (!JungleAdventureModel.Instance.IsPreheatEnd())
            {
                _timeText.SetText(JungleAdventureModel.Instance.GetPreheatEndTimeString());
            }
            else
            {
                _timeText.SetText(JungleAdventureModel.Instance.GetEndTimeString());
            }
        }

        private void OnPlay()
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.JungleAdventurePlay);
            AnimCloseWindow(() =>
            {
                if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                    return;
                SceneFsm.mInstance.TransitionGame();
            });
        }
        
        private void RefreshReward()
        {
            if (JungleAdventureModel.Instance.IsFinish())
            {
                _rewardIcon.gameObject.SetActive(false);
                return;
            }
            
            int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);
            
            _rewardIcon.sprite =  ResourcesManager.Instance.GetSpriteVariant("JungleAdventureAtlas", config.FinalImage); 
        }

        private void InitSlider()
        {
            if (JungleAdventureModel.Instance.IsFinish())
            {
                _slider.value = 1f;
                _scoreText.SetText(JungleAdventureModel.Instance.JungleAdventure.CurrentScore.ToString());
                return;
            }
            int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
            int currentScore = JungleAdventureModel.Instance.JungleAdventure.AnimScore;
            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);
            
            _scoreText.SetText(JungleAdventureModel.Instance.JungleAdventure.AnimScore.ToString() + "/" + config.Score);
            
            _slider.value = 1.0f*currentScore/config.Score;
        }

        private void RefreshSlider(float ratio)
        {
            _slider.value = ratio;
            int stage = JungleAdventureModel.Instance.JungleAdventure.Stage;
            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);

            int maxScore = Math.Min(config.Score, JungleAdventureModel.Instance.JungleAdventure.CurrentScore);
            int score = (int)(config.Score* ratio);

            score = Math.Min(maxScore, score);
            _scoreText.SetText(score + "/" + config.Score);
        }
    }
}