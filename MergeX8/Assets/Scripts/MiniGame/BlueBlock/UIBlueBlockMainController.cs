using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Mosframe;
using Psychology;
using Psychology.Model;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIBlueBlockMainController:UIWindowController
{
    private BlueBlockGame CurGame;
    private Transform GameLayer;
    private TablePsychology LevelConfig;
    private int CurGameIndex;
    private Transform LeftPoint;
    private Transform CenterPoint;
    private Transform RightPoint;
    private bool _isFinish;
    private Button CloseBtn;
    private Transform FinishOneGroup;
    private Transform FinishGroup;
    private Button FinishBtn;
    private Slider Slider;
    private LocalizeTextMeshProUGUI LevelTitleText;
    public override void PrivateAwake()
    {
        GameLayer = transform.Find("Root/Content");
        LeftPoint = transform.Find("Root/Content/PointLeft");
        CenterPoint = transform.Find("Root/Content/PointMiddle");
        RightPoint = transform.Find("Root/Content/PointRight");
        CloseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
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
                            data1:LevelConfig.id.ToString(),"1");
                        
                        SceneFsm.mInstance.ChangeState(StatusType.ExitPsychology);
                    });
                },
                HasCloseButton = true,
                HasCancelButton = true,
                IsHighSortingOrder = true,
            });
        });
        FinishGroup = transform.Find("Root/Finish");
        FinishBtn = transform.Find("Root/Finish/Button").GetComponent<Button>();
        FinishBtn.onClick.AddListener(() =>
        {
            // SceneFsm.mInstance.ChangeState(StatusType.ExitPsychology);
            // if (_isFinish)
            // {
            //     if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
            //     {
            //         AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b => { });
            //     }
            // }
            
            PsychologyModel.Instance.FinishLevel(LevelConfig.id);
        
            if (LevelConfig.id == 1 && !_isFinish)
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
                if (_isFinish || LevelConfig.id != 2)
                {
                    if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_AsmrContinue))
                    {
                        AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_AsmrContinue, b => { });
                    }
                }
            }
            
            
            
        });
        FinishOneGroup = transform.Find("Root/FinishOne");
        FinishOneGroup.gameObject.SetActive(false);
        FinishGroup.gameObject.SetActive(false);
        Slider = transform.Find("Root/Slider").GetComponent<Slider>();
        LevelTitleText = transform.Find("Root/Slider/LevelText").GetComponent<LocalizeTextMeshProUGUI>();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        LevelConfig = objs[0] as TablePsychology;
        _isFinish = PsychologyModel.Instance.IsFinish(LevelConfig.id);
        LevelTitleText.SetTermFormats(LevelConfig.id.ToString());
        CurGameIndex = 0;
        Slider.maxValue = LevelConfig.blueBlockLevelId.Length;
        Slider.value = CurGameIndex;
        var level = LevelConfig.blueBlockLevelId[CurGameIndex];
        var levelAsset = ResourcesManager.Instance.LoadResource<GameObject>("MiniGame/BlueBlock/Level" + level+"/Level"+level,addToCache:false);
        var levelObject = Instantiate(levelAsset, GameLayer);
        levelObject.setLayer(5,true);
        levelObject.transform.position = CenterPoint.position;
        {
            var sortingLayerId = canvas.sortingLayerID;
            var sortingOrder = canvas.sortingOrder;
            var sortingGroup = levelObject.AddComponent<SortingGroup>();
            sortingGroup.enabled = true;
            sortingGroup.sortingLayerID = sortingLayerId;
            sortingGroup.sortingOrder = sortingOrder + 1;
        }
        CurGame = levelObject.AddComponent<BlueBlockGame>();
        CurGame.InitGame(level,OnSingleGameWin);

        if (levelObject.transform.Find("GuideStart") && levelObject.transform.Find("GuideEnd"))
        {
            var guideFinish = StorageManager.Instance.GetStorage<StorageHome>().Guide.GuideFinished;
            var cacheGuideFinished = GuideSubSystem.Instance.CacheGuideFinished;
            var guideIdList = new List<int>() {4301};
            for (var i = 0; i < guideIdList.Count; i++)
            {
                var guideId = guideIdList[i];
                if (guideFinish.ContainsKey(guideId))
                {
                    guideFinish.Remove(guideId);
                }
                if (cacheGuideFinished.ContainsKey(guideId))
                {
                    cacheGuideFinished.Remove(guideId);
                }
            }
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BlueBlock,"");
            var guideStartPos = levelObject.transform.Find("GuideStart").position;
            var guideEndPos = levelObject.transform.Find("GuideEnd").position;
            ShowHandGuideTo(guideStartPos, guideEndPos);
        }
    }
    private GameObject guideHandResources;
    private List<Transform> guideHands = new List<Transform>();
    public void ShowHandGuideTo(Vector3 startPos,Vector3 endPos)
    {
        if (guideHandResources == null)
            guideHandResources = ResourcesManager.Instance.LoadResource<GameObject>("MiniGame/BlueBlock/UI/GuideArrow",addToCache:false);
        var handObj = GameObject.Instantiate(guideHandResources, transform).transform;
        handObj.gameObject.setLayer(5,true);
        handObj.position = startPos;
        handObj.DOMove(endPos, 2f).SetLoops(-1, LoopType.Restart);
        guideHands.Add(handObj);
    }
    public void RemoveAllHandGuide()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.None);
        foreach (var hand in guideHands)
        {
            hand.DOKill();
            GameObject.Destroy(hand.gameObject);
        }
        guideHands.Clear();
    }
    public async void OnSingleGameWin(BlueBlockGame game)
    {
        AudioManager.Instance.PlaySound("sfx_snake_shield");
        if (game != CurGame)
        {
            Debug.LogError("完成的游戏非当前进行的游戏 当前进行:"+CurGame.CurLevelConfig.id+" 完成:"+game.CurLevelConfig.id);
            return;
        }
        CurGameIndex++;
        Slider.value = CurGameIndex;
        if (CurGameIndex >= LevelConfig.blueBlockLevelId.Length)
        {
            //完成所有小关卡，大关卡胜利
            DestroyImmediate(CurGame);
            OnGameWin();
        }
        else
        {
            var lastGameObj = CurGame.gameObject;
            DestroyImmediate(CurGame);
            //加载下一个小关卡
            FinishOneGroup.gameObject.SetActive(true);
            await XUtility.WaitSeconds(2f);
            FinishOneGroup.gameObject.SetActive(false);
            var level = LevelConfig.blueBlockLevelId[CurGameIndex];
            var levelAsset = ResourcesManager.Instance.LoadResource<GameObject>("MiniGame/BlueBlock/Level" + level+"/Level"+level,addToCache:false);
            var levelObject = Instantiate(levelAsset, GameLayer);
            levelObject.setLayer(5,true);
            levelObject.transform.position = LeftPoint.position;
            {
                var sortingLayerId = canvas.sortingLayerID;
                var sortingOrder = canvas.sortingOrder;
                var sortingGroup = levelObject.AddComponent<SortingGroup>();
                sortingGroup.enabled = true;
                sortingGroup.sortingLayerID = sortingLayerId;
                sortingGroup.sortingOrder = sortingOrder + 1;
            }
            CurGame = levelObject.AddComponent<BlueBlockGame>();
            CurGame.InitGame(level,OnSingleGameWin);
            var moveTime = 0.3f;
            lastGameObj.transform.DOMove(RightPoint.position, moveTime).OnComplete(() =>
            {
                DestroyImmediate(lastGameObj);
            });
            CurGame.transform.DOMove(CenterPoint.position, moveTime);
        }
    }

    public void OnGameWin()
    {
        PsychologyModel.Instance.FinishLevel(LevelConfig.id);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameRoomsortLevelend,data1:LevelConfig.id.ToString());
        FinishGroup.gameObject.SetActive(true);
        CloseBtn.gameObject.SetActive(false);
    }

    public static UIBlueBlockMainController Open(TablePsychology levelConfig)
    {
        return UIManager.Instance.OpenUI(UINameConst.UIBlueBlockMain, levelConfig) as UIBlueBlockMainController;
    }
}