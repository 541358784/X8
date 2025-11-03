using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASMR;
using DragonPlus;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;
using Makeover;
using UnityEngine;
using UnityEngine.UI;

public partial class UIGameMainController : UIWindowController
{
    private Button _exitBut;
    private GameObject operateStatusGroup;
    private GameObject operateBtnGroup;
    private GameObject finishGroup;
    private TableMoLevel _curLevelConfig;
    private Scrollbar _progressBar;

    private int _index = 0;

    private GameObject _guideObj;
    private LocalizeTextMeshProUGUI _guideText;
    
    public override void PrivateAwake()
    {
        _exitBut = GetItem<Button>("Root/PauseButton");
        _exitBut.onClick.AddListener(OnExit);
        operateStatusGroup = transform.Find("Root/OperateGroup").gameObject;
        operateBtnGroup = transform.Find("Root/Point").gameObject;
        finishGroup = transform.Find("Root/Finish").gameObject;
        finishGroup.GetComponent<Button>().onClick.AddListener(() =>
        {
            MakeoverModel.Instance.FinishLevel(_curLevelConfig);
            GameBIManager.Instance.SendGameEvent(
                BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrLevelend,
                data1:_curLevelConfig.id.ToString(),data2:"0");
            SceneFsm.mInstance.ChangeState(StatusType.ExitMakeover); 
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventAsmrComplete);
            
            if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
            {
                AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b =>
                {
                });
            }
        });

        _progressBar = transform.Find("Root/Scrollbar").GetComponent<Scrollbar>();
        _progressBar.gameObject.SetActive(false);
        
        operateStatusGroup.SetActive(false);
        operateBtnGroup.SetActive(false);
        finishGroup.SetActive(false);

        _guideObj = transform.Find("Root/BottomText").gameObject;
        _guideText = _guideObj.FindChild("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _guideObj.SetActive(false);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        
        _curLevelConfig = (TableMoLevel)objs[0];
        
        InitStatus();
    }

    private void OnExit()
    {
        CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
        {
            HasCloseButton = false,
            HasCancelButton = true,
            DescString = LocalizationManager.Instance.GetLocalizedString("ui_asmr_quit_pop_desc"),
            OKCallback = () =>
            {
                TMatch.UILoadingEnter.Open(()=>
                {
                    AudioManager.Instance.StopAllSound();
                    GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventAsmrQuit);
                    GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameAsmrLevelend,
                        data1:_curLevelConfig.id.ToString(),data2:"1");
                    SceneFsm.mInstance.ChangeState(StatusType.ExitMakeover);
                
                    if (_curLevelConfig.id == 1)
                    {
                        string ftueBi = "GAME_EVENT_ASMROFF";
                        BiEventAdventureIslandMerge.Types.GameEventType ftueBiEvent;
                        if(GameBIManager.TryParseGameEventType(ftueBi, out ftueBiEvent))
                            GameBIManager.Instance.SendGameEvent(ftueBiEvent);
                    }
                });
               
            }
        });
    }

    private void OnBtnOperate()
    {
        operateBtnGroup.SetActive(false);
        //PlayBodySound();
        //PlayBodyTreatPart(_index++, BodyActionType.Green);
    }

    public void SetFinishLevel()
    {
        FinishStatus(MakeoverConfigManager.Instance.stepList.FindAll( a => a.levelId == _curLevelConfig.stepLevelId).Count-1);
        finishGroup.SetActive(true);
    }
    
    public void GoToNextStep(int stepIndexAdd,int complete)
    {
        _index = stepIndexAdd-1;
        MakeoverModel.Instance.AddBodyStep(_curLevelConfig, _index+1);
        FinishStatus(_index);
        StartCoroutine(E_AutoGotNextStep(0.8f));
    }

    public void SetToNextStep(int stepIndexAdd)
    {
        _index = stepIndexAdd-1;
        MakeoverModel.Instance.AddBodyStep(_curLevelConfig, _index+1);
        FinishStatus(_index);
    }
    
    public void SetAllStepFinish()
    {
        _index = operateStatusGameObjects.Count - 1;
        MakeoverModel.Instance.AddBodyStep(_curLevelConfig, _index+1);
        FinishStatus(_index);
    }
    
    private IEnumerator E_AutoGotNextStep(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        OnClickStep();
    }
    
    private void OnClickStep()
    {
        // _step_obj.SetActive(false);
        int stepIndex=ASMR.Model.Instance.StartNextStep();
        //_lcells[stepIndex].SetState(LevelCell.State.Actvie);
    }
    public void ShowProgressBar(bool show)
    {
        _progressBar.gameObject.SetActive(show);
    }

    public void OnProgressChanged(float value)
    {
        _progressBar.size = value;
    }

    public void ShowGuideText(string textKey)
    {
        if (textKey.IsEmptyString())
        {
            HideGuideText();
            return;
        }
        _guideObj.SetActive(true);
        _guideText.SetTerm(textKey);
    }

    public void HideGuideText()
    {
        _guideObj.SetActive(false);
    }
}