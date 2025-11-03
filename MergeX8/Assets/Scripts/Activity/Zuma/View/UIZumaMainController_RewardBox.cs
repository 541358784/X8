using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.Zuma;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using MagneticScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class UIZumaMainController
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
            if (textObj != null)
                _text = textObj.GetComponent<LocalizeTextMeshProUGUI>();

            UpdateReward();
        }

        private void UpdateReward()
        {
            if (_resDatas == null)
                return;

            if (_isSetSprite)
            {
                _image.sprite = UserData.GetResourceIcon(_resDatas[0].id, UserData.ResourceSubType.Big);
                _text?.SetText(_resDatas[0].count.ToString());
            }
            else
            {
                _text?.SetText("");
            }
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

        public void Init(GameObject gameObject, int index, List<ResData> resDatas, Action<RewardGroup> clickTips, StorageZuma Storage)
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

            _isGet = Storage.LevelId > index;

            _noramlGroup = new RewardItem(gameObject.transform.Find("Normal").gameObject, resDatas, false);
            _finishGroup = new RewardItem(gameObject.transform.Find("Finish").gameObject, resDatas, false);

            _tipObject = _gameObject.transform.Find("Tips").gameObject;
            SetTipsActive(false);
            _rewardItem = _tipObject.transform.Find("Item").gameObject;
            _rewardItem.gameObject.SetActive(false);

            _tipButton = _gameObject.transform.GetComponent<Button>();
            _tipButton.onClick.AddListener(() => { clickTips?.Invoke(this); });

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

            if (_isGet == isGet)
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
    private LocalizeTextMeshProUGUI _levelText;
    private Slider _normalSlider;
    private ScrollRect _normalScrollRect;
    private GameObject _normalRewardItem;

    private RewardGroup _finallyReward = new RewardGroup();
    private List<RewardGroup> _normalRewardGroups = new List<RewardGroup>();

    private Coroutine _coroutine;

    private GameObject _rewardRoot;
    
    private LocalizeTextMeshProUGUI _winScoreText;

    // private int _currentScore;
    private Tween _valueTween;
    
    
    private void AwakeReward()
    {
        _rewardRoot =  transform.Find("Root/NumGroup").gameObject;
        
        _normalGroup = transform.Find("Root/Reward/Scroll View").gameObject;
        _normalSlider = transform.Find("Root/Reward/Scroll View/Viewport/Content/Slider").GetComponent<Slider>();
        _normalScrollRect = _normalGroup.GetComponent<ScrollRect>();

        _normalRewardItem = transform.Find("Root/Reward/Scroll View/Viewport/Content/IconGroup/1").gameObject;
        _normalRewardItem.gameObject.SetActive(false);

        _levelText = transform.Find("Root/Reward/Level/Text").GetComponent<LocalizeTextMeshProUGUI>();

        _winScoreText = transform.Find("Root/NumGroup/NumText").GetComponent<LocalizeTextMeshProUGUI>();
        
        _normalGroup.gameObject.SetActive(false);
        
        EventDispatcher.Instance.AddEvent<EventZumaNewLevel>(OnLevelChange);
        EventDispatcher.Instance.AddEvent<EventZumaScoreChange>(OnScoreChange);
    }


    private void OnRewardDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventZumaNewLevel>(OnLevelChange);
        EventDispatcher.Instance.RemoveEvent<EventZumaScoreChange>(OnScoreChange);
        _valueTween.Kill();
    }
    
    public void OnLevelChange(EventZumaNewLevel evt)
    {
        LevelConfig = Model.GetLevel(Storage.LevelId);
        ShowScore = Storage.LevelScore;
        UpdateRewardStatus();
        var level = LevelConfig;
        if (level.IsLoopLevel)
        {
            _winScoreText.SetText(ShowScore.ToString());
        }
        else
        {
            _winScoreText.SetText(ShowScore + "/" + level.WinScore);
        }
    }

    private List<int> WaitAddValueList = new List<int>();
    public void OnScoreChange(EventZumaScoreChange evt)
    {
        if (evt.NeedWait)
        {
            WaitAddValueList.Add(evt.ChangeValue);
        }
        else
        {
            ShowScore += evt.ChangeValue;
        }
        UpdateProgressAnim();
    }

    public void TriggerWaitAddValue()
    {
        if (WaitAddValueList.Count > 0)
        {
            var addValue = WaitAddValueList[0];
            WaitAddValueList.RemoveAt(0);
            var alReadyAddValue = 0;
            var addValueF = (float) addValue;
            DOTween.To(() => 0f, (v) =>
            {
                var curV = (int) v;
                if (curV != alReadyAddValue)
                {
                    var distance = curV - alReadyAddValue;
                    alReadyAddValue = curV;
                    ShowScore += distance;
                    UpdateProgressAnim();
                }
            }, addValueF, 0.5f).OnComplete(() =>
            {
                if (addValue != alReadyAddValue)
                {
                    var distance = addValue - alReadyAddValue;
                    alReadyAddValue = addValue;
                    ShowScore += distance;
                    UpdateProgressAnim();
                }
                Debug.LogError("win2");
            }).SetTarget(transform);
        }
    }
    
    private void InitReward()
    {
        LevelConfig = Model.GetLevel(Storage.LevelId);
        ShowScore = Storage.LevelScore;
        Key = transform.Find("Root/Key");
        KeyInitPosition = Key.transform.localPosition;
        if(_normalRewardGroups.Count > 0)
            return;
        
        _normalGroup.gameObject.SetActive(true);

        InitNormalReward();

        // _levelText.SetText(Storage.LevelId.ToString());
        var level = LevelConfig;
        if (level.IsLoopLevel)
        {
            _winScoreText.SetText(ShowScore.ToString());
        }
        else
        {
            _winScoreText.SetText(ShowScore+"/"+level.WinScore);
        }
        UpdateRewardStatus();
    }

    private void InitNormalReward()
    {
        _normalSlider.maxValue = ZumaConfigManager.Instance.ZumaLevelConfigList.Count-1;

        for (var i = 0; i < ZumaConfigManager.Instance.ZumaLevelConfigList.Count-2; i++)
        {
            var config = ZumaConfigManager.Instance.ZumaLevelConfigList[i];

            GameObject reward = Instantiate(_normalRewardItem, _normalRewardItem.transform.parent.transform, false);
            reward.gameObject.SetActive(true);

            List<ResData> resDatas = new List<ResData>();
            for (int j = 0; j < config.RewardId.Count; j++)
            {
                ResData resData = new ResData(config.RewardId[j], config.RewardNum[j]);
                resDatas.Add(resData);
            }

            RewardGroup group = new RewardGroup();
            group.Init(reward, config.Id, resDatas, ClickTips, Storage);

            _normalRewardGroups.Add(group);
        }

        var lastConfig = ZumaConfigManager.Instance.ZumaLevelConfigList[ZumaConfigManager.Instance.ZumaLevelConfigList.Count - 2];
        List<ResData> lastResDatas = new List<ResData>();
        for (int j = 0; j < lastConfig.RewardId.Count; j++)
        {
            ResData resData = new ResData(lastConfig.RewardId[j], lastConfig.RewardNum[j]);
            lastResDatas.Add(resData);
        }

        _finallyReward.Init(transform.Find("Root/Reward/FinallyReward").gameObject, lastConfig.Id, lastResDatas, ClickTips, Storage);

        _normalSlider.value = GetNormalProgress();

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)(_normalRewardItem.transform.parent.parent.transform));

        DOHorizontalNormalizedPos(0);
    }

    private void ClickTips(RewardGroup group)
    {
        _normalRewardGroups.ForEach(a => a.SetTipsActive(false));
        _finallyReward.SetTipsActive(false);
        group.SetTipsActive(true);

        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(WaitCloseTips(group));
    }

    private IEnumerator WaitCloseTips(RewardGroup group)
    {
        yield return new WaitForSeconds(3);

        group.SetTipsActive(false);
        _coroutine = null;
    }

    private ZumaLevelConfig LevelConfig;
    private int ShowScore;
    
    public float GetNormalProgress()
    {
        int index = GetNormalIndex();
        index -= 1;

        return 1.0f + index + GetProgress();
    }
    public int GetNormalIndex()
    {
        int index = ZumaConfigManager.Instance.ZumaLevelConfigList.FindIndex(a => a.Id == LevelConfig.Id);
        return index;
    }
    public float GetProgress()
    {
        return 1.0f*ShowScore/LevelConfig.WinScore;
    }
    private void UpdateProgressAnim()
    {
        _normalSlider.value = GetNormalProgress();
        bool isGetAll = ShowScore >= LevelConfig.WinScore;
        if (isGetAll)
        {
            DOHorizontalNormalizedPos(0.3f);
        }
        // if (isGetAll)
        // {
        //     int lastLevelId = ZumaConfigManager.Instance.ZumaLevelConfigList[ZumaConfigManager.Instance.ZumaLevelConfigList.Count - 2].Id;
        //     if (LevelConfig.Id == lastLevelId)
        //         _finallyReward.UpdateStatus(true, false);
        // }
        if (LevelConfig.IsLoopLevel)
        {
            _winScoreText.SetText(ShowScore.ToString());
        }
        else
        {
            _winScoreText.SetText(ShowScore+"/"+LevelConfig.WinScore);
        }
    }

    private void DOHorizontalNormalizedPos(float time)
    {
        int normalIndex = GetNormalIndex();
        float progress = 1.0f * normalIndex / (ZumaConfigManager.Instance.ZumaLevelConfigList.Count - 3);

        progress = Math.Clamp(progress, 0f, 1f);
        _normalScrollRect.DOHorizontalNormalizedPos(progress, time).SetEase(Ease.Linear);
    }

    private void UpdateRewardStatus()
    {
        _normalGroup.gameObject.SetActive(true);
        if (!LevelConfig.IsLoopLevel)
        {
            _levelText.SetText(LevelConfig.Id.ToString());
        }
        else
        {
            _levelText.SetText("\u221e");
        }
    }
    private Transform Key;
    private Vector3 KeyInitPosition;
    public async Task PerformFlyKey(int levelId)
    {
        Key.gameObject.SetActive(true);
        Key.localPosition = KeyInitPosition;
        Key.DOKill();
        AudioManager.Instance.PlaySoundById(196);
        await XUtility.WaitSeconds(0.5f);
        RewardGroup rewardBoxGroup = null;
        if (levelId == 10)
        {
            rewardBoxGroup = _finallyReward;
        }
        else
        {
            rewardBoxGroup = _normalRewardGroups.Find((a) => a.index == levelId);
        }
        Key.DOMove(rewardBoxGroup._gameObject.transform.position,1f);
        await XUtility.WaitSeconds(1f);
        if (levelId == 10)
            _finallyReward.UpdateStatus(true, false);
        else
        {
            _normalRewardGroups.ForEach(a => { a.UpdateStatus(LevelConfig.Id+1 > a.index, false); });
        }
        Key.gameObject.SetActive(false);
        
    }
}