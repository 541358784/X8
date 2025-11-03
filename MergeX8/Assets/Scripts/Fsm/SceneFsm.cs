using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ConnectLine.Fsm;
using Filthy.Fsm;
using Merge.Order;
using OnePath.Fsm;
using Psychology.Fsm;
using Screw;
using Stimulate.Fsm;
using TileMatch.fsm;

public enum StatusType
{
    Non,
    Loading,
    Login, //登录
    Game,
    HappyGoGame,
    Home,
    BackHome,
    Transition,
    TransitionHappyGo,
    LoadRoom,
    BackLogin,
    // Decoration,
    Makeover,
    ExitMakeover,
    TripleMatch,
    TripleMatchEntry,
    DigTrench,
    ExitDigTrench,
    FishEatFish,
    ExitFishEatFish,
    EnterOnePath,
    ExitOnePath,
    EnterConnectLine,
    ExitConnectLine,
    EnterStimulate,
    ExitStimulate,
    EnterPsychology,
    ExitPsychology,
    
    ScrewGame,
    ScrewHome,
    
    EnterFilthy,
    ExitFilthy,
    
    EnterFarm,
    ExitFarm,
    
    TileMatch,
}

public class SceneFsm : MonoBehaviour
{
    public static SceneFsm mInstance = null;

    public bool ClientInited = false;
    public bool ClientLogin = false;

    private Dictionary<StatusType, IFsmState> mStateDictionary = new Dictionary<StatusType, IFsmState>();

    private IFsmState currentState = null;
    private IFsmState prevState = null;

    void Awake()
    {
        mInstance = this;

        mStateDictionary.Add(StatusType.Loading, new SceneFsmLoading());
        mStateDictionary.Add(StatusType.Login, new SceneFsmLogin());
        mStateDictionary.Add(StatusType.Home, new SceneFsmHome());
        mStateDictionary.Add(StatusType.Game, new SceneFsmGame());
        mStateDictionary.Add(StatusType.HappyGoGame, new SceneFsmHappyGoGame());
        mStateDictionary.Add(StatusType.Transition, new SceneFsmTransition());
        mStateDictionary.Add(StatusType.TransitionHappyGo, new SceneFsmTransitionHappyGo());
        mStateDictionary.Add(StatusType.BackHome, new SceneFsmBackHome());
        mStateDictionary.Add(StatusType.LoadRoom, new SceneFsmLoadRoom());
        mStateDictionary.Add(StatusType.BackLogin, new SceneFsmBackLogin());
        mStateDictionary.Add(StatusType.Makeover, new SceneFsmMakeover());
        mStateDictionary.Add(StatusType.ExitMakeover, new SceneFsmExitMakeover());
        mStateDictionary.Add(StatusType.TripleMatch, new TMatch.StateTMatch());
        mStateDictionary.Add(StatusType.TripleMatchEntry, new TMatch.StateTMatchEntry());
        mStateDictionary.Add(StatusType.DigTrench, new SceneFsmDigTrench());
        mStateDictionary.Add(StatusType.ExitDigTrench, new SceneFsmExitDigTrench());
        mStateDictionary.Add(StatusType.FishEatFish, new SceneFsmFishEatFish());
        mStateDictionary.Add(StatusType.ExitFishEatFish, new SceneFsmExitFishEatFish());
        mStateDictionary.Add(StatusType.EnterOnePath, new SceneFsmEnterOnePath());
        mStateDictionary.Add(StatusType.ExitOnePath, new SceneFsmExitOnePath());
        mStateDictionary.Add(StatusType.EnterConnectLine, new SceneFsmEnterConnectLine());
        mStateDictionary.Add(StatusType.ExitConnectLine, new SceneFsmExitConnectLine());
        mStateDictionary.Add(StatusType.EnterStimulate, new SceneFsmEnterStimulate());
        mStateDictionary.Add(StatusType.ExitStimulate, new SceneFsmExitStimulate());
        mStateDictionary.Add(StatusType.EnterPsychology, new SceneFsmEnterPsychology());
        mStateDictionary.Add(StatusType.ExitPsychology, new SceneFsmExitPsychology());
        
        
        mStateDictionary.Add(StatusType.ScrewGame, new SceneFsmScrewGame());
        mStateDictionary.Add(StatusType.ScrewHome, new SceneFsmScrewHome());
        
        mStateDictionary.Add(StatusType.EnterFilthy, new SceneFsmEnterFilthy());
        mStateDictionary.Add(StatusType.ExitFilthy, new SceneFsmExitFilthy());
        
        mStateDictionary.Add(StatusType.EnterFarm, new SceneFsmEnterFarm());
        
        mStateDictionary.Add(StatusType.TileMatch, new SceneFsmTileMatch());
        
        
    }
    // public void StartTM()
    // {
    //     ChangeState(StatusType.TripleMatch, );
    // }
    public void StartTMEntry()
    {
        ChangeState(StatusType.TripleMatchEntry);
    }
    public void ExitTM()
    {
        ChangeState(StatusType.BackHome);
    }
    public void StartGame()
    {
        ChangeState(StatusType.Loading, StatusType.Login);
    }

    public void BackToLogin()
    {
        SceneFsm.mInstance.ClientInited = false;
        ChangeState(StatusType.BackLogin, StatusType.Login);
    }

    public void EnterScrewGame(int levelIndex)
    {
        ChangeState(StatusType.ScrewGame, levelIndex);
    }
    
    public void EnterScrewHome()
    {
        ChangeState(StatusType.ScrewHome);
    }
    
        
    public void BackHome(params object[] objs)
    {
        List<object> parm = new List<object>();
        parm.Add(StatusType.BackHome);

        if (objs != null && objs.Length > 0)
        {
            foreach (var kv in objs)
            {
                parm.Add(kv);
            }
        }

        ChangeState(StatusType.Transition, parm.ToArray());
        //SceneFsm.mInstance.ChangeState(StatusType.BackHome, objs);
    }

    public void EnterGame()
    {
        if (currentState == null || currentState.Type == StatusType.Login || currentState.Type == StatusType.BackLogin)
        {
            //DecoManager.Instance.CurrentWorld.Id;
            int roomId = -1;
            ChangeState(StatusType.LoadRoom, StatusType.Home, roomId);
        }
    }

    public void TransitionGame(MergeBoardEnum boardType=MergeBoardEnum.Main, MainOrderType type = MainOrderType.None)
    {
        MergeManager.Instance.Refresh(boardType);
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PlayButton);
        if (boardType == MergeBoardEnum.HappyGo)
        {
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.HappyGoGame,boardType, type);

        }
        else
        {
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.Game,boardType, type);
        }
        //GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteFirstLevelButtonTap);
    }

    public void ChangeState(StatusType type, params object[] objs)
    {
        IFsmState newState = mStateDictionary[type];
        if (currentState == newState)
            return;

        StartCoroutine(ChangeState(newState, objs));
    }

    public void ChangeStateForce(StatusType type, params object[] objs)
    {
        IFsmState newState = mStateDictionary[type];

        StartCoroutine(ChangeState(newState, objs));
    }
    
    IEnumerator ChangeState(IFsmState newStatus, params object[] objs)
    {
        yield return new WaitForEndOfFrame();

        prevState = currentState;

        if (currentState != null)
        {
            var oldState = currentState;
            currentState.Exit();
            EventDispatcher.Instance.SendEventImmediately<EventExitFsmState>(new EventExitFsmState(oldState));
        }

        currentState = newStatus;
        currentState.Enter(objs);
        EventDispatcher.Instance.SendEventImmediately<EventEnterFsmState>(new EventEnterFsmState(newStatus));
    }

    public void Update()
    {
        if (currentState == null)
            return;
        currentState.Update(Time.deltaTime);
    }
    public void LateUpdate()
    {
        if (currentState == null)
            return;
        currentState.LateUpdate(Time.deltaTime);
    }

    public StatusType GetCurrSceneType()
    {
        if (currentState == null)
            return StatusType.Non;

        return currentState.Type;
    }

    public IFsmState GetCurrentState()
    {
        return currentState;
    }
}