using System;
using ConnectLine;
using ConnectLine.Logic;
using ConnectLine.Model;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UIConnectLineMainController : UIWindowController
{
    private Button _backButton;
    private Button _resetButton;
    private GameObject _levelRoot;
    private GameObject _levelPrefab;
    private Button _finishBtn;
    private GameObject _levelFailed;
    
    private int _levelId;
    private int _configId;

    private ConnectLineLogic _connectLineLogic;
    private bool _isFinish = false;
    
    public override void PrivateAwake()
    {
        _backButton = GetItem<Button>("ButtonBack");
        _backButton.onClick.AddListener(OnButtonBack);
        
        _resetButton = GetItem<Button>("ResetButton");
        _resetButton.onClick.AddListener(OnButtonReset);
        _levelRoot = GetItem("Level");

        _levelId = ConnectLineModel.Instance._config.levelId;
        _configId = ConnectLineModel.Instance._config.id;
        string path = $"ConnectLine/Level{_levelId}/Prefab/Level{_levelId}";
        GameObject obj = null;
        if (CommonUtils.IsLE_16_10())
        {
            var padPatch = path + "_Pad";
            obj = ResourcesManager.Instance.LoadResource<GameObject>(padPatch, addToCache:false);
        }
        if(obj == null) 
            obj = ResourcesManager.Instance.LoadResource<GameObject>(path, addToCache:false);
        
        _levelPrefab = Instantiate(obj);
        CommonUtils.AddChild(_levelRoot.transform, _levelPrefab.transform);
        ((RectTransform)_levelPrefab.transform).offsetMin = Vector2.zero;
        ((RectTransform)_levelPrefab.transform).offsetMax = Vector2.zero;
        _connectLineLogic = _levelPrefab.AddComponent<ConnectLineLogic>();
        _connectLineLogic.InitGuide(GetItem("Guide"), _levelId);
        
        ResourcesManager.Instance.ReleaseRes(path);
        
        _finishBtn = GetItem<Button>("Finish");
        _finishBtn.gameObject.SetActive(false);
        
        _finishBtn.onClick.AddListener(() =>
        {
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameConnectLineLevelend,
                data1:_levelId.ToString(),"0");
                        
            ConnectLineModel.Instance.FinishLevel(_configId);
        
            if (_levelId == 1 && !_isFinish)
            {
                UILoadingTransitionController.Show(null);
                XUtility.WaitSeconds(0.3f, () =>
                {
                    SceneFsm.mInstance.ChangeState(StatusType.ExitConnectLine);
                    XUtility.WaitFrames(1, () =>
                    {
                        SceneFsm.mInstance.ChangeState(StatusType.EnterConnectLine,ConnectLineConfigManager.Instance._configs[1], true);
                    });
                });
            }
            else
            {
                SceneFsm.mInstance.ChangeState(StatusType.ExitConnectLine);
                if (_isFinish || _levelId != 2)
                {
                    if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
                    {
                        AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b => { });
                    }
                }
            }
        });

        _levelFailed = GetItem("Fail");
        _levelFailed.gameObject.SetActive(false);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.CONNECTLINE_SUCCESS, ConnectLineSuccess);
        EventDispatcher.Instance.AddEventListener(EventEnum.CONNECTLINE_FAILED, ConnectLineFailed);

        _isFinish = ConnectLineModel.Instance.IsFinish(_levelId);
    }

    private void Start()
    { 
        _connectLineLogic.InitOrder(canvas.sortingOrder);
        
        _finishBtn.gameObject.GetComponent<Canvas>().sortingOrder = canvas.sortingOrder+10;
        _levelFailed.gameObject.GetComponent<Canvas>().sortingOrder = canvas.sortingOrder+10;;

        _backButton.gameObject.GetComponent<Canvas>().sortingOrder = canvas.sortingOrder+2;
        _resetButton.gameObject.GetComponent<Canvas>().sortingOrder = canvas.sortingOrder+2;
    
        _connectLineLogic.CanInput = true;
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
                        
                    SceneFsm.mInstance.ChangeState(StatusType.ExitOnePath);
                });
            },
            HasCloseButton = true,
            HasCancelButton = true,
            IsHighSortingOrder = true,
        });
    }

    private void OnButtonReset()
    {
        PathLineManager.Instance.CleanPath();
        _connectLineLogic.Reset();
        
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameConnectLineLevelend,
            data1:_levelId.ToString(),"3");
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.CONNECTLINE_SUCCESS, ConnectLineSuccess);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.CONNECTLINE_FAILED, ConnectLineFailed);
    }

    private void ConnectLineSuccess(BaseEvent e)
    {
        UIRoot.Instance.EnableEventSystem = false;
        _connectLineLogic.CanInput = false;
        
        StartCoroutine(CommonUtils.DelayWork(1.5f, () =>
        {
            UIRoot.Instance.EnableEventSystem = true;
            _finishBtn.gameObject.SetActive(true);
        }));
    }
    
    private void ConnectLineFailed(BaseEvent e)
    {
        AudioManager.Instance.PlaySound(111);
        _connectLineLogic.CanInput = false;
        _levelFailed.gameObject.SetActive(true);
        StartCoroutine(CommonUtils.DelayWork(1.5f, () =>
        {
            PathLineManager.Instance.CleanPath();
            _connectLineLogic.Reset();
            
            _levelFailed.gameObject.SetActive(false);
            _connectLineLogic.CanInput = true;
        }));
    }
}