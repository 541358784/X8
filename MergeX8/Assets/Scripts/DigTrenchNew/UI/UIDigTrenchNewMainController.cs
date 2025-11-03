using System;
using System.Threading.Tasks;
using DigTrenchNew;
using Ditch.Model;
using DragonPlus;
using DragonPlus.Config.Ditch;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIDigTrenchNewMainController:UIWindowController
{
    public static UIDigTrenchNewMainController Instance;
    public static UIDigTrenchNewMainController Open(TableDitchLevel config)
    {
        UIManager.Instance.SetCanvasGroupAlpha(false, false);
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIDigTrenchNewMain,config) as UIDigTrenchNewMainController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        RemberCullingMask = UIRoot.Instance.mUICamera.cullingMask;
        UIRoot.Instance.mUICamera.cullingMask = 1<<28;
    }

    private void OnDestroy()
    {
        UIRoot.Instance.mUICamera.cullingMask = RemberCullingMask;
        Destroy(GameController.gameObject);
        UIManager.Instance.SetCanvasGroupAlpha(true, false);
    }

    public int RemberCullingMask;

    public TableDitchLevel Config;
    private DigTrenchNewGameController GameController;
    private Button CloseBtn;
    private Slider CameraSlider;
    private Button FinishBtn;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Config = objs[0] as TableDitchLevel;
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameWaterLevelstart,
            data1:Config.Id.ToString(),data2:"New");
        var levelAsset = ResourcesManager.Instance.LoadResource<GameObject>("DigTrenchNew/Prefabs/Level" + Config.DitchLevel);
        GameController = Instantiate(levelAsset).GetComponent<DigTrenchNewGameController>();
        GameController.Init(this);
        
        CloseBtn = transform.Find("Root/PauseButton").GetComponent<Button>();
        CloseBtn.onClick.AddListener(() =>
        {
            UIPopupDigTrenchNewGameTipsController.Open(Config);
        });

        CameraSlider = transform.Find("Root/Slider").GetComponent<Slider>();
        CameraSlider.value = 0;
        GameController.SetCameraPosition(1f);
        CameraSlider.onValueChanged.AddListener((v) =>
        {
            GameController.SetCameraPosition(1f-v);
        });
        
        CameraSlider.gameObject.SetActive(CommonUtils.IsLE_16_10()?Config.HasSliderPad:Config.HasSlider);
        
        InitProgressGroup();

        FinishBtn = transform.Find("Root/Finish").GetComponent<Button>();
        FinishBtn.onClick.AddListener(() =>
        {
            CloseWindowWithinUIMgr(true);
            var script = UIManager.Instance.GetOpenedUIByPath<UIPopupTaskController>(UINameConst.UIPopupTask);
            // if(script != null)
            //     script.RefreshDitchUI();
            
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.StoryEnd, "10100100000");
        });
        FinishBtn.gameObject.SetActive(false);
        var storyKey = "level" + Config.Id + "_" + "start";
        PlayStory(storyKey).AddCallBack(() =>
        {
            GameController.PlyGuide();
        }).WrapErrors();
    }

    public void OnWin()
    {
        GameBIManager.Instance.SendGameEvent(
            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameWaterLevelend,
            data1:Config.Id.ToString(),data2:"0");
        DitchModel.Instance.FinishLevel(Config.Id);
        ProgressGroupController.AddLevel();
        CloseBtn.gameObject.SetActive(false);
        XUtility.WaitSeconds(0.5f, async () =>
        {
            if (!this)
                return;
            var storyKey = "level" + Config.Id + "_" + "end";
            await PlayStory(storyKey);
            FinishBtn.gameObject.SetActive(true);
            GameController.PlayWomanWinEffect();
            transform.Find("Root/Finish/Bag").gameObject.SetActive(Config.Id == 1);
        });
    }

    public void OnMergeWin()
    {
        ProgressGroupController.AddLevel();
    }
    
    public async void DealGameLogic(int gameParam,Action<bool> callback)
    {
        var mergeGameId = Config.MergeId[gameParam];
        var storyKey1 = "level" + Config.Id + "_" + "merge"+gameParam+"start";
        await PlayStory(storyKey1);
        DitchModel.Instance.EnterMergeGame(mergeGameId, async (b) =>
        {
            callback(b);
            if (b)
            {
                OnMergeWin();
                var storyKey2 = "level" + Config.Id + "_" + "merge"+gameParam+"end";
                PlayStory(storyKey2).WrapErrors();   
            }
        });
    }

    public Task PlayStory(string storyKey)
    {
        var story = GlobalConfigManager.Instance.GetTableStory(23, storyKey);
        if (story == null)
            return Task.CompletedTask;
        if (StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog.Contains(story.id))
            return Task.CompletedTask;
        StorageManager.Instance.GetStorage<StorageHome>().DialogData.FinishedDialog.Add(story.id);
        var task = new TaskCompletionSource<bool>();
        UIDigTrenchNewStoryController.Open(story, () =>
        {
            task.SetResult(true);
        });
        return task.Task;
    }
}