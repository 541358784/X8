using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Psychology;
using Psychology.Model;
using UnityEngine;
using UnityEngine.UI;

public class UIPsychologyMainController : UIWindowController
{
    public class ButtonGroup
    {
        public GameObject _gameObject;
        public Button _button;
        public LocalizeTextMeshProUGUI _text;
        public GameObject _arrow;
        public GameObject _select;
        public GameObject _selectArrow;
    }
    
    public class LevelGroup
    {
        private Image _gameImage;
        private List<ButtonGroup> _buttonGroups = new List<ButtonGroup>();
        private GameObject _gameObject;
        private Action<int> _action;
        
        public GameObject gameObject
        {
            get { return _gameObject; }
        }
        
        public void Init(GameObject gameObject, Action<int> action)
        {
            _gameObject = gameObject;
            _action = action;
            
            _gameImage = _gameObject.transform.Find("GameImage").GetComponent<Image>();
            
            for (int i = 1; i <= 2; i++)
            {
                int index = i - 1;
                ButtonGroup data = new ButtonGroup();
                data._gameObject = _gameObject.transform.Find("ButtonGroup/Button" + i).gameObject;
                data._arrow = data._gameObject.transform.Find("Arrow").gameObject;
                data._select = data._gameObject.transform.Find("Select").gameObject;
                data._select.gameObject.SetActive(false);
                
                data._selectArrow = data._gameObject.transform.Find("Select/Image (1)").gameObject;
                data._selectArrow.gameObject.SetActive(false);
                
                data._button = data._gameObject.transform.GetComponent<Button>();
                data._button.onClick.AddListener(() =>
                {
                    action?.Invoke(index);
                });
                data._text = data._gameObject.transform.Find("Root/Text").GetComponent<LocalizeTextMeshProUGUI>();
            
                _buttonGroups.Add(data);
            }
        }

        public void UpdateUI(TablePsychologyLevel config)
        {
            _gameImage.sprite = ResourcesManager.Instance.GetSpriteVariant("PsychologyAtlas", config.imageName);
            
            for (var i = 0; i < config.buttonNameKeys.Length; i++)
            {
                string key = config.buttonNameKeys[i];
            
                if(i >= _buttonGroups.Count)
                    continue;
            
                _buttonGroups[i]._text.SetTerm(key);
            }
        }

        public void Select(int index, bool isEnd)
        {
            for (var i = 0; i < _buttonGroups.Count; i++)
            {
                _buttonGroups[i]._select.gameObject.SetActive(i==index);
                _buttonGroups[i]._arrow.gameObject.SetActive(false);

                if (isEnd)
                {
                    _buttonGroups[i]._selectArrow.gameObject.SetActive(i==index);
                }
            }
        }
    }
    
    private Button _backButton;
    private LocalizeTextMeshProUGUI _levelText;
    private Button _finishBtn;
    private GameObject _timeGroup;
    private GameObject _analysisGroup;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _analysisText;
    private Button _immediatelyShow;

    private LevelGroup _currentLevelGroup = new LevelGroup();
    private LevelGroup _nextLevelGroup = new LevelGroup();
    
    private int _levelId;
    private bool _isFinish = false;

    private int _lookTime = 5;
    private bool _isChose = false;
    private Tweener _tweener = null;
    private bool _isShowEnd = false;

    private int _currentLevelIndex = 0;


    private Vector3 _startPosition = new Vector3(1000f, 0f, 0f);
    private Vector3 _endPosition = new Vector3(-1000f, 0f, 0f);

    private List<int> _selectStatus = new List<int>();
    
    
    private Tweener _currentTweener = null;
    private Tweener _nextTweener = null;
    
    public override void PrivateAwake()
    {
        _currentLevelGroup.Init(transform.Find("Root/Level").gameObject, OnSelectClick);
        _nextLevelGroup.Init(transform.Find("Root/LevelNext").gameObject, OnSelectClick);
        
        _levelText = transform.Find("Root/LevelText").GetComponent<LocalizeTextMeshProUGUI>();
        _timeText = transform.Find("Root/Time/Text").GetComponent<LocalizeTextMeshProUGUI>();
        _analysisText = transform.Find("Root/Analysis/Scroll View/Viewport/Content/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _levelId =  PsychologyModel.Instance._config.id;
        
        _backButton = GetItem<Button>("Root/ButtonClose");
        _backButton.onClick.AddListener(OnButtonBack);
        
        _immediatelyShow = GetItem<Button>("Root/Analysis/Button");
        _immediatelyShow.onClick.AddListener(ImmediatelyShow);
        
        _timeGroup = transform.Find("Root/Time").gameObject;
        _timeGroup.gameObject.SetActive(false);
        
        _analysisGroup = transform.Find("Root/Analysis").gameObject;
        _analysisGroup.gameObject.SetActive(false);
        
        _finishBtn = GetItem<Button>("Root/Finish");
        _finishBtn.gameObject.SetActive(false);
        
        _isFinish = PsychologyModel.Instance.IsFinish(_levelId);
        _finishBtn.onClick.AddListener(() =>
        {
            PsychologyModel.Instance.FinishLevel(_levelId);
        
            if (_levelId == 1 && !_isFinish)
            {
                UILoadingTransitionController.Show(null);
                XUtility.WaitSeconds(0.3f, () =>
                {
                    SceneFsm.mInstance.ChangeState(StatusType.ExitPsychology);
                    XUtility.WaitFrames(1, () =>
                    {
                        SceneFsm.mInstance.ChangeState(StatusType.EnterPsychology,PsychologyConfigManager.Instance._configs[1], true);
                    });
                });
            }
            else
            {
                SceneFsm.mInstance.ChangeState(StatusType.ExitPsychology);
                if (_isFinish || _levelId != 2)
                {
                    if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
                    {
                        AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b => { });
                    }
                }
            }
        });

        InitView();
    }

    private IEnumerator Start()
    {
        _analysisGroup.gameObject.SetActive(false);
        // _timeGroup.gameObject.SetActive(true);
        //
        // _timeText.SetText(_lookTime.ToString());
        // while (_lookTime > 0)
        // {
        //     yield return new WaitForSeconds(1);
        //
        //     _lookTime--;
        //     _timeText.SetText(_lookTime.ToString());
        // }
        //
        // yield return new WaitForSeconds(1);
        _timeGroup.gameObject.SetActive(false);
        UpdateLevel();
        
        yield break;
    }

    private void UpdateLevel()
    {
        _levelText.SetText((_currentLevelIndex+1) + "/" + PsychologyModel.Instance._config.levels.Length);
    }
    private void OnButtonBack()
    {
        CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
        {
            DescString =
                LocalizationManager.Instance.GetLocalizedString("ui_minigame_11"),
            OKCallback = () =>
            {
                TMatch.UILoadingEnter.Open(() =>
                {
                    GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameConnectLineLevelend,
                        data1:_levelId.ToString(),"1");
                        
                    SceneFsm.mInstance.ChangeState(StatusType.ExitPsychology);
                });
            },
            HasCloseButton = true,
            HasCancelButton = true,
            IsHighSortingOrder = true,
        });
    }

    private void ImmediatelyShow()
    {
        if(_isShowEnd)
            return;
        
        _tweener.Kill();
        _analysisText.m_TmpText.maxVisibleCharacters = _analysisText.m_TmpText.textInfo.characterCount;
        
        StopCoroutine(WaitFinish());
        StartCoroutine(WaitFinish());
    }

    private void InitView()
    {
        _currentLevelGroup.gameObject.transform.localPosition = Vector3.zero;
        _nextLevelGroup.gameObject.transform.localPosition = _startPosition;

        int currentLevel = PsychologyModel.Instance._config.levels[_currentLevelIndex];
        var config = PsychologyConfigManager.Instance._configLevels.Find(a => a.id == currentLevel);
        _currentLevelGroup.UpdateUI(config);
    }

    private void OnSelectClick(int index)
    {
        if(_isChose)
            return;
        
        _isChose = true;

        _selectStatus.Add(index);
        
        bool isEnd = _currentLevelIndex >= PsychologyModel.Instance._config.levels.Length - 1;
        
        _currentLevelGroup.Select(index, isEnd);
        
        if (isEnd)
        {
            GameBIManager.Instance.SendGameEvent( BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigamePsychologyChoose,data1:PsychologyModel.Instance._config.id.ToString(), (_currentLevelIndex+1).ToString());

            ShowText();
        }
        else
        {
            _currentLevelIndex++;
            int currentLevel = PsychologyModel.Instance._config.levels[_currentLevelIndex];
            var config = PsychologyConfigManager.Instance._configLevels.Find(a => a.id == currentLevel);
            _nextLevelGroup.UpdateUI(config);
            _nextLevelGroup.Select(-1, false);
            
            _currentTweener = _currentLevelGroup.gameObject.transform.DOLocalMove(_endPosition, 0.5f).SetEase(Ease.Linear);
            _nextTweener = _nextLevelGroup.gameObject.transform.DOLocalMove(Vector3.zero,0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                var tempGroup = _currentLevelGroup;
                _currentLevelGroup = _nextLevelGroup;
                _nextLevelGroup = tempGroup;
                
                _currentLevelGroup.gameObject.transform.localPosition = Vector3.zero;
                _nextLevelGroup.gameObject.transform.localPosition = _startPosition;
                _isChose = false;
            });
            
            
            GameBIManager.Instance.SendGameEvent( BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigamePsychologyChoose,data1:PsychologyModel.Instance._config.id.ToString(), _currentLevelIndex.ToString());
        }
        
        UpdateLevel();
    }

    private void ShowText()
    {
        _isShowEnd = false;

        _analysisText.SetTerm(GetExplainKey());
        _analysisGroup.gameObject.SetActive(true);
        _analysisText.m_TmpText.maxVisibleCharacters = 0;
        _analysisText.m_TmpText.ForceMeshUpdate();
        
        float showTime = _analysisText.m_TmpText.textInfo.characterCount / 5f;
        _tweener = DOTween.To(() => _analysisText.m_TmpText.maxVisibleCharacters, x => _analysisText.m_TmpText.maxVisibleCharacters = x, _analysisText.m_TmpText.textInfo.characterCount, showTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            _isShowEnd = true;

            StartCoroutine(WaitFinish());
        });
    }

    private string GetExplainKey()
    {
        string firstKey = null;
        foreach (var explainId in PsychologyModel.Instance._config.explainIds)
        {
            var config = PsychologyConfigManager.Instance._configExplains.Find(a => a.id == explainId);
            if(config == null)
                continue;

            List<int> ids = new List<int>(config.explainIndex);

            if (firstKey == null)
                firstKey = config.explainKey;
            
            if (_selectStatus.SequenceEqual(ids))
                return config.explainKey;
        }

        return firstKey;
    }
    private IEnumerator WaitFinish()
    {
        yield return new WaitForSeconds(2);
        
        _finishBtn.gameObject.SetActive(true);
    }
}