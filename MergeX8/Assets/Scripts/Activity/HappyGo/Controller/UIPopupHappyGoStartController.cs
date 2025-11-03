
using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class UIPopupHappyGoStartController : UIWindowController
{
    private Button _closeBtn;
    private LocalizeTextMeshProUGUI _timeText;
    private Button _playBtn;
    private Button _enterRoomBtn;
    private const string coolTimeKey = "HappyGoStartPop";
    private Button _helpBtn;
    private Transform _preHeat;
    private Transform _start;
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/CloseButton");
        _playBtn = GetItem<Button>("Root/PlayButton");
        _preHeat = transform.Find("Root/PreHeat");
        _start = transform.Find("Root/Start");
        _enterRoomBtn = GetItem<Button>("Root/EnterRoom");
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _closeBtn.onClick.AddListener(OnClose);
        _playBtn.onClick.AddListener(OnPlayBtn);
        _enterRoomBtn.onClick.AddListener(OnEnterRoom);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.HappyGoButton, null);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_playBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.HappyGoPlay, _playBtn.transform as RectTransform, topLayer: topLayer);
        InvokeRepeating("Invoke_SetHappyGoCDTime", 0f, 1f);
    }


    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgPop);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.HappyGoPlay, null);
    }
    private void Invoke_SetHappyGoCDTime()
    {
        if (HappyGoModel.Instance.IsPreheating())
        {
            _playBtn.gameObject.SetActive(false);
            _timeText.SetText(HappyGoModel.Instance.GetActivityPreheatLeftTimeString());
            _preHeat.gameObject.SetActive(true);           
            _start.gameObject.SetActive(false);
        }
        else
        {
            _playBtn.gameObject.SetActive(true);
            _start.gameObject.SetActive(true);
            _timeText.SetText(HappyGoModel.Instance.GetActivityLeftTimeString());
            _preHeat.gameObject.SetActive(false);

        }
    }
    private void OnPlayBtn()
    {
        if (!HappyGoModel.Instance.IsStart())
        {
            HappyGoModel.Instance.InitHappyGo();
            AnimCloseWindow();
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.HappyGoPlay);
            StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.FinishNode, "8880001");
        }
        else
        {
            AnimCloseWindow();
            EnterGame();
        } 
    }
        
       

    private void OnEnterRoom()
    {
       
    }
    
    void EnterGame()
    {
        ShakeManager.Instance.ShakeSelection();
        AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        SceneFsm.mInstance.TransitionGame(MergeBoardEnum.HappyGo);
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgEnter);
    }
    private void OnClose()
    {
        CloseWindowWithinUIMgr(true);
    }
    
    public static bool CanShowUI()
    {
        if (!HappyGoModel.Instance.IsOpened())
            return false;
        
        if (HappyGoModel.Instance.IsStart())
            return false;
        
        if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.LossTime, coolTimeKey))
            return false;
        
        CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.LossTime, coolTimeKey, CommonUtils.GetTimeStamp(),2*24*60*60*1000);
        // GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventHgPop);
        UIManager.Instance.OpenUI(UINameConst.UIPopupHappyGoStart);
        return true;
    }

    private void OnDestroy()
    {
        CancelInvoke("Invoke_SetHappyGoCDTime");
    }
}
