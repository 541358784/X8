using System;
using System.Collections;
using System.Collections.Generic;
using Activity.GardenTreasure.Model;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.GardenTreasure;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIGardenTreasureMainController
{
    public class RewardItem
    {
        private Image _image;
        private LocalizeTextMeshProUGUI _text;
        private GameObject _gameObject;
        private List<ResData> _resDatas;
        private bool _isSetSprite;
        public GameObject GameObject
        {
            get { return _gameObject; }
        }

        public void SetTipButtonActive(bool isActive)
        {
            //_tipButton.gameObject.SetActive(isActive);
        }
        
        public RewardItem(GameObject gameObject, List<ResData> resDatas, bool isSetSprite)
        {
            InitRewardItem(gameObject, resDatas, isSetSprite);
        }

        public RewardItem(GameObject gameObject, ResData resData, bool isSetSprite)
        {
            List<ResData> resDatas = new List<ResData>();
            resDatas.Add(resData);
            
            InitRewardItem(gameObject, resDatas, isSetSprite);
        }

        private void InitRewardItem(GameObject gameObject, List<ResData> resDatas, bool isSetSprite)
        {
            _gameObject = gameObject;
            _resDatas = resDatas;
            _isSetSprite = isSetSprite;
            
            _image = _gameObject.transform.Find("Item/Icon").GetComponent<Image>();
            
            var textObj = _gameObject.transform.Find("Text");
            if(textObj != null)
                _text = textObj.GetComponent<LocalizeTextMeshProUGUI>();

            UpdateReward(); 
        }

        private void UpdateReward()
        {
            if(_resDatas == null)
                return;

            if (_isSetSprite)
            {
                _image.sprite = UserData.GetResourceIcon(_resDatas[0].id,UserData.ResourceSubType.Big);
                _text?.SetText(_resDatas[0].count.ToString());
            }
            else
            {
                _text?.SetText("");
            }
            // if (_resDatas.Count == 1)
            // {
            //     _image.sprite = UserData.GetResourceIcon(_resDatas[0].id,UserData.ResourceSubType.Big);
            //     _text?.SetText(_resDatas[0].count.ToString());
            // }
            // else
            // {
            //     _image.sprite =  ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, "MysteryGift_Gift1");
            // }
           
        }

        public void UpdateReward(List<ResData> resDatas)
        {
            _resDatas = resDatas;

            UpdateReward();
        }
    }
    
    public class RewardGroup
    {
        private RewardItem _noramlGroup;
        private RewardItem _finishGroup;
        public GameObject _gameObject;
        private int _index;
        private List<ResData> _resDatas;
        private bool _isGet;
        private Button _tipButton;
        private GameObject _tipObject;
        private List<RewardItem> _rewardItems = new List<RewardItem>();
        private GameObject _rewardItem = null;
        
        public int index
        {
            get { return _index; }
        }
        public void Init(GameObject gameObject, int index, List<ResData> resDatas, Action<RewardGroup> clickTips)
        {
            _gameObject = gameObject;
            _resDatas = resDatas;
            _index = index;
            _isGet = false;

            foreach (var rewardItem in _rewardItems)
            {
                DestroyImmediate(rewardItem.GameObject);
            }
            _rewardItems.Clear();
            
            if (!GardenTreasureModel.Instance.IsRandomLevel())
                _isGet = GardenTreasureModel.Instance.GardenTreasure.NormalLevelId > index;

            _noramlGroup = new RewardItem(gameObject.transform.Find("Normal").gameObject, resDatas, false);
            _finishGroup = new RewardItem(gameObject.transform.Find("Finish").gameObject, resDatas, false);

            _tipObject = _gameObject.transform.Find("Tips").gameObject;
            SetTipsActive(false);
            _rewardItem = _tipObject.transform.Find("Item").gameObject;
            _rewardItem.gameObject.SetActive(false);
            
            _tipButton = _gameObject.transform.GetComponent<Button>();
            _tipButton.onClick.AddListener(() =>
            {
                clickTips?.Invoke(this);
            });

            foreach (var resData in _resDatas)
            {
                GameObject reward = Instantiate(_rewardItem, _rewardItem.transform.parent, false);
                reward.gameObject.SetActive(true);
                    
                var rewardItem = new RewardItem(reward.gameObject, resData, true);
                _rewardItems.Add(rewardItem);
            }
            UpdateStatus(_isGet, true);
        }

        public void UpdateReward(List<ResData> resDatas)
        {
            _resDatas = resDatas;
            
            foreach (var rewardItem in _rewardItems)
            {
                DestroyImmediate(rewardItem.GameObject);
            }
            _rewardItems.Clear();
            
            foreach (var resData in _resDatas)
            {
                GameObject reward = Instantiate(_rewardItem, _rewardItem.transform.parent, false);
                reward.gameObject.SetActive(true);
                    
                var rewardItem = new RewardItem(reward.gameObject, resData, true);
                _rewardItems.Add(rewardItem);
            }
            
            _noramlGroup.UpdateReward(resDatas);
            _finishGroup.UpdateReward(resDatas);
        }

        public void UpdateStatus(bool isGet, bool isInit)
        {
            if (isInit)
            {
                UpdateStatus(isGet);
                return;
            }
            
            if(_isGet == isGet)
                return;
            
            UpdateStatus(isGet);
        }

        private void UpdateStatus(bool isGet)
        {
            _noramlGroup.GameObject.SetActive(!isGet);
            _finishGroup.GameObject.SetActive(isGet);
        }

        public void SetTipsActive(bool isActive)
        {
            _tipObject.gameObject.SetActive(isActive);
        }
    }
    
    private GameObject _normalGroup;
    private GameObject _randomGroup;
    private LocalizeTextMeshProUGUI _levelText;
    private Slider _normalSlider;
    private Slider _randomSlider;
    private ScrollRect _normalScrollRect;
    private GameObject _normalRewardItem;

    private RewardGroup _finallyReward = new RewardGroup();
    private List<RewardGroup> _normalRewardGroups = new List<RewardGroup>();

    private Coroutine _coroutine;
    
    private void AwakeReward()
    {
        _normalGroup = transform.Find("Root/LevelGroup/Scroll View").gameObject;
        _normalSlider = transform.Find("Root/LevelGroup/Scroll View/Viewport/Content/Slider").GetComponent<Slider>();
        _normalScrollRect = _normalGroup.GetComponent<ScrollRect>();
        
        _normalRewardItem = transform.Find("Root/LevelGroup/Scroll View/Viewport/Content/IconGroup/1").gameObject;
        _normalRewardItem.gameObject.SetActive(false);
        
        _randomGroup = transform.Find("Root/LevelGroup/RandomSlider").gameObject;
        _randomSlider = _randomGroup.gameObject.GetComponent<Slider>();
        _randomSlider.maxValue = 1;
        
        _levelText = transform.Find("Root/LevelGroup/Level/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _normalGroup.gameObject.SetActive(false);
        _randomGroup.gameObject.SetActive(false);
    }

    private void InitReward()
    {
        _normalGroup.gameObject.SetActive(!GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel);
        _randomGroup.gameObject.SetActive(GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel);
        
        InitRandomReward();
        InitNormalReward();
        
        _levelText.SetText(GardenTreasureModel.Instance.GardenTreasure.ShowLevelId.ToString());
    }

    private void InitRandomReward()
    {
        if(!GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel)
            return;
        
        var config = GardenTreasureConfigManager.Instance._randomLevelConfig;
        List<ResData> resDatas = new List<ResData>();
        for (int j = 0; j < config.RewardId.Count; j++)
        {
            ResData resData = new ResData(config.RewardId[j], config.RewardNum[j]);
            resDatas.Add(resData);
        }
        _finallyReward.Init(transform.Find("Root/LevelGroup/FinallyReward").gameObject, config.Id, resDatas, ClickTips);
        _randomSlider.value = GardenTreasureModel.Instance.GetRandomProgress();
    }

    private void InitNormalReward()
    {
        if(GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel)
            return;

        _normalSlider.maxValue = GardenTreasureConfigManager.Instance._normalLevelConfigs.Count;
        
        for (var i = 0; i < GardenTreasureConfigManager.Instance._normalLevelConfigs.Count; i++)
        {
            var config = GardenTreasureConfigManager.Instance._normalLevelConfigs[i];
            if(i == GardenTreasureConfigManager.Instance._normalLevelConfigs.Count-1)
                break;
            
            GameObject reward = Instantiate(_normalRewardItem, _normalRewardItem.transform.parent.transform, false);
            reward.gameObject.SetActive(true);

            List<ResData> resDatas = new List<ResData>();
            for (int j = 0; j < config.RewardId.Count; j++)
            {
                ResData resData = new ResData(config.RewardId[j], config.RewardNum[j]);
                resDatas.Add(resData);
            }
            
            RewardGroup group = new RewardGroup();
            group.Init(reward, config.Id, resDatas, ClickTips);
            
            _normalRewardGroups.Add(group);
        }
        
        var lastConfig = GardenTreasureConfigManager.Instance._normalLevelConfigs[GardenTreasureConfigManager.Instance._normalLevelConfigs.Count-1];
        List<ResData> lastResDatas = new List<ResData>();
        for (int j = 0; j < lastConfig.RewardId.Count; j++)
        {
            ResData resData = new ResData(lastConfig.RewardId[j], lastConfig.RewardNum[j]);
            lastResDatas.Add(resData);
        }
        _finallyReward.Init(transform.Find("Root/LevelGroup/FinallyReward").gameObject, lastConfig.Id, lastResDatas, ClickTips);

        _normalSlider.value = GardenTreasureModel.Instance.GetNormalProgress();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(_normalRewardItem.transform.parent.parent.transform));

        DOHorizontalNormalizedPos(0);
    }

    private void ClickTips(RewardGroup group)
    {
        _normalRewardGroups.ForEach(a=>a.SetTipsActive(false));
        _finallyReward.SetTipsActive(false);
        group.SetTipsActive(true);
        
        if(_coroutine != null)
            StopCoroutine(_coroutine);
        
        _coroutine = StartCoroutine(WaitCloseTips(group));
    }

    private IEnumerator WaitCloseTips(RewardGroup group)
    {
        yield return new WaitForSeconds(3);
        
        group.SetTipsActive(false);
        _coroutine = null;
    }
    
    private void UpdateProgressAnim(bool isGetAll)
    {
        float moveTime = 0.5f;
        if (GardenTreasureModel.Instance.IsRandomLevel())
        {
            _randomSlider.DOKill();
            _randomSlider.DOValue(GardenTreasureModel.Instance.GetRandomProgress(), moveTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                if(isGetAll)
                    _finallyReward.UpdateStatus(true, false);
            });
        }
        else
        {
            _normalSlider.DOKill();
            _normalSlider.DOValue(GardenTreasureModel.Instance.GetNormalProgress(), moveTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (isGetAll)
                {
                    _normalRewardGroups.ForEach(a =>
                    {
                        a.UpdateStatus(GardenTreasureModel.Instance.GardenTreasure.NormalLevelId > a.index, false);
                    });

                    DOHorizontalNormalizedPos(moveTime);
                }
            });
            
            if (isGetAll)
            {
                int lastLevelId = GardenTreasureConfigManager.Instance._normalLevelConfigs[GardenTreasureConfigManager.Instance._normalLevelConfigs.Count-1].Id;
                if(GardenTreasureModel.Instance.GardenTreasure.NormalLevelId ==lastLevelId)
                    _finallyReward.UpdateStatus(true, false);
            }
        }
    }

    private void DOHorizontalNormalizedPos(float time)
    {
        int normalIndex = GardenTreasureModel.Instance.GetNormalIndex();
        float progress = 1.0f*normalIndex/(GardenTreasureConfigManager.Instance._normalLevelConfigs.Count-1);

        progress = Math.Clamp(progress, 0f, 1f);
        _normalScrollRect.DOHorizontalNormalizedPos(progress, time).SetEase(Ease.Linear);
    }
    
    private void UpdateRewardStatus()
    {
        _normalGroup.gameObject.SetActive(!GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel);
        _randomGroup.gameObject.SetActive(GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel);

        _levelText.SetText(GardenTreasureModel.Instance.GardenTreasure.ShowLevelId.ToString());

        if (GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel)
        {
            var config = GardenTreasureModel.Instance.GetCurrentLevelConfig();
            List<ResData> resDatas = new List<ResData>();
            for (int j = 0; j < config.RewardId.Count; j++)
            {
                ResData resData = new ResData(config.RewardId[j], config.RewardNum[j]);
                resDatas.Add(resData);
            }
            
            _finallyReward.UpdateReward(resDatas);
            _finallyReward.UpdateStatus(false, true);
            _randomSlider.value = 0;
        }
    }
}