using System;
using System.Collections.Generic;
using Decoration;
using DragonPlus;
using DragonPlus.Config.TileMatch;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using TileMatch.Event;
using TileMatch.Game;
using TileMatch.Game.Block;
using UnityEngine;
using UnityEngine.UI;
using BiEventTileGarden = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
public enum FailTypeEnum
{
    None = -1,
    Normal = 0, //默认
    EnterBack, //主动点返回
    ReduceHp, //扣体力
    GridFull, //格子满 超级回退
    Special, //特殊失败 可回退
    SpecialFail, //特殊失败 不可退回 直接失败
    Time, //时间失败
}

public class FailData
{
    public List<Block> _blocks = new List<Block>();
    public Block _failBlock;
}

public partial class UIPopupTileMatchFailController : UIWindowController
{

    private LocalizeTextMeshProUGUI _playOnValue;

    private Button _playOnBtn;
    private Button _tryAgainBtn;
    private Button _leaveBtn;
    private Button _giveUpBtn;
    private Button _closeBtn;
    // private Button _undoBtn;

    // FailGroup
    private Transform _fail;
    private Transform _fail1;
    private Transform _infinite;
    private Transform _rugbyChallenge;
    private Transform _time;
    private Transform _grass;
    private Transform _lock;
    private Transform _bomb;
    private Transform _ice;
    private Transform _gridFull;
    private Transform _rankLeaderBoard;

    public Action _OnBtnLevel;
    public Action _OnBtnTryAgain;
    public Action _OnBtnGiveUp;
    public Action _OnBtnClose;
    public Action _OnBtnPlayOn;

    private LocalizeTextMeshProUGUI _title;

    private KapiTileGiftBagExtraView GiftBagExtraView;
    public void EnableClick(bool clickFlag)
    {
        if (clickFlag)
        {
            var raycaster = gameObject.GetComponent<GraphicRaycaster>();
            if (raycaster)
                raycaster.enabled = true;
        }
        else
        {
            var raycaster = gameObject.GetComponent<GraphicRaycaster>();
            if (!raycaster)
                raycaster = gameObject.AddComponent<GraphicRaycaster>();
            raycaster.enabled = false;
        }
    }

    private FailTypeEnum type;
    private BlockTypeEnum eBlockTypeEnum;

    public override void PrivateAwake()
    {
        _playOnValue = GetItem<LocalizeTextMeshProUGUI>("Root/PlayOnButton/NumText");

        _playOnBtn = GetItem<Button>("Root/PlayOnButton");
        var rebornIcon = GetItem<Image>("Root/PlayOnButton/Icon");
        rebornIcon.sprite = UserData.GetResourceIcon(UserData.ResourceId.KapiTileReborn, UserData.ResourceSubType.Big);
        _tryAgainBtn = GetItem<Button>("Root/TryAgainButton");
        _leaveBtn = GetItem<Button>("Root/LeaveButton");
        _giveUpBtn = GetItem<Button>("Root/GiveUpButton");
        // _undoBtn = GetItem<Button>("Root/UndoButton");
        GetItem<Button>("Root/UndoButton").gameObject.SetActive(false);
        _closeBtn = GetItem<Button>("Root/CommonPopupBG1/CloseButton");
        _title = GetItem<LocalizeTextMeshProUGUI>("Root/CommonPopupBG1/Title/Text");


        _fail = GetItem<Transform>("Root/FailGroup/Fail");
        _fail1 = GetItem<Transform>("Root/FailGroup/Fail1");
        _infinite = GetItem<Transform>("Root/FailGroup/Infinite");
        _rugbyChallenge = GetItem<Transform>("Root/FailGroup/RugbyChallenge");
        _time = GetItem<Transform>("Root/FailGroup/Time");
        _grass = GetItem<Transform>("Root/FailGroup/Grass");
        _ice = GetItem<Transform>("Root/FailGroup/Ice");
        _gridFull = GetItem<Transform>("Root/FailGroup/GridFull");
        _lock = GetItem<Transform>("Root/FailGroup/Lock");
        _bomb = GetItem<Transform>("Root/FailGroup/Bomb");
        _rankLeaderBoard = GetItem<Transform>("Root/FailGroup/RankLeaderBoard");
        TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_PauseGame);
        GiftBagExtraView = transform.Find("Gift").gameObject.AddComponent<KapiTileGiftBagExtraView>();
        GiftBagExtraView.Init();
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        type = (FailTypeEnum)objs[0];
        eBlockTypeEnum = BlockTypeEnum.Normal;

        if (type == FailTypeEnum.Special || type == FailTypeEnum.SpecialFail)
            eBlockTypeEnum = (BlockTypeEnum)objs[1];
        CommonUtils.SetShieldButTime(_giveUpBtn.gameObject, 0.1f);
        CommonUtils.SetShieldButTime(_leaveBtn.gameObject, 0.1f);
        Init(type, eBlockTypeEnum);
        
    }

    private void Init(FailTypeEnum type, BlockTypeEnum blockTypeEnum = BlockTypeEnum.Normal)
    {
        _failTipsController = null;
        InitUI();
        switch (type)
        {
            case FailTypeEnum.EnterBack:
                _title.SetTerm("UI_tittle_quit_1");
                var tip = _fail.gameObject;
                if (EnergyModel.Instance.IsEnergyUnlimited())
                {
                    tip = _infinite.gameObject;
                }

                _leaveBtn.gameObject.SetActive(true);
                _closeBtn.gameObject.SetActive(true);
                _leaveBtn.onClick.AddListener(OnClickLeaveBtn);
                _closeBtn.onClick.AddListener(OnBtnClose);
                _failTipsController = new FailTipsController(this);
                CheckExtraFailTip(_failTipsController);
                _failTipsController.PushTip(tip.AddComponent<FailTipsBase>());
                _failTipsController.InitShow();
                GiftBagExtraView.gameObject.SetActive(false);
                break;
            case FailTypeEnum.GridFull:
                GiftBagExtraView.gameObject.SetActive(true);
                _title.SetTerm("UI_tittle_quit_2");
                _giveUpBtn.gameObject.SetActive(true);
                _playOnBtn.onClick.AddListener(OnClickSuperBackOnButton);
                _giveUpBtn.onClick.AddListener(OnBtnGiveUp);
                _playOnBtn.gameObject.SetActive(true);
                _failTipsController = new FailTipsController(this);
                _failTipsController.PushTip(_gridFull.gameObject.AddComponent<FailTipsBase>());
                CheckExtraFailTip(_failTipsController);
                _failTipsController.InitShow();
                break;
            case FailTypeEnum.Special:
            {
                GiftBagExtraView.gameObject.SetActive(true);
                _title.SetTerm("UI_tittle_quit_2");
                _playOnBtn.gameObject.SetActive(true);
                _playOnBtn.onClick.AddListener(OnClickUnDoButton);
                _giveUpBtn.gameObject.SetActive(true);
                _giveUpBtn.onClick.AddListener(OnBtnGiveUp);
                Transform blockTypeNode = _fail;
                switch (blockTypeEnum)
                {
                    case BlockTypeEnum.Bomb:
                    {
                        _bomb.gameObject.SetActive(true);
                        blockTypeNode = _bomb;
                        break;
                    }
                    case BlockTypeEnum.Grass:
                    {
                        _grass.gameObject.SetActive(true);
                        blockTypeNode = _grass;
                        break;
                    }
                    case BlockTypeEnum.Lock:
                    {
                        _lock.gameObject.SetActive(true);
                        blockTypeNode = _lock;
                        break;
                    }
                    case BlockTypeEnum.Ice:
                    {
                        _ice.gameObject.SetActive(true);
                        blockTypeNode = _ice;
                        break;
                    }
                }

                _failTipsController = new FailTipsController(this);
                _failTipsController.PushTip(blockTypeNode.gameObject.AddComponent<FailTipsBase>());
                CheckExtraFailTip(_failTipsController);
                _failTipsController.InitShow();
                break;
            }
            case FailTypeEnum.ReduceHp:
                _title.SetTerm("UI_tittle_quit_2");
                if (EnergyModel.Instance.IsEnergyUnlimited())
                {
                    _infinite.gameObject.SetActive(true);
                }
                else
                {
                    _fail1.gameObject.SetActive(true);
                }

                _tryAgainBtn.gameObject.SetActive(true);
                _tryAgainBtn.onClick.AddListener(OnClickLeaveBtn);
                _closeBtn.gameObject.SetActive(true);
                _closeBtn.onClick.AddListener(OnClickLeaveBtn);
                GiftBagExtraView.gameObject.SetActive(false);
                break;
            case FailTypeEnum.Time:
            {
                GiftBagExtraView.gameObject.SetActive(true);
                _title.SetTerm("UI_tittle_quit_2");
                _time.gameObject.SetActive(true);
                _giveUpBtn.gameObject.SetActive(true);
                _giveUpBtn.onClick.AddListener(OnBtnGiveUp);
                _playOnBtn.onClick.AddListener(OnClickAddTime);
                _playOnBtn.gameObject.SetActive(true);
                break;
            }
            case FailTypeEnum.SpecialFail:
            {
                GiftBagExtraView.gameObject.SetActive(false);
                _title.SetTerm("UI_tittle_quit_2");
                _tryAgainBtn.gameObject.SetActive(false);
                _closeBtn.gameObject.SetActive(true);
                // _tryAgainBtn.onClick.AddListener(OnClickTryAgainButton);
                _closeBtn.onClick.AddListener(OnClickLeaveBtn);

                Transform blockTypeNode = _fail;
                switch (blockTypeEnum)
                {
                    case BlockTypeEnum.Ice:
                    {
                        _ice.gameObject.SetActive(true);
                        blockTypeNode = _ice;
                        break;
                    }
                    case BlockTypeEnum.Grass:
                    {
                        _grass.gameObject.SetActive(true);
                        blockTypeNode = _grass;
                        break;
                    }
                    case BlockTypeEnum.Lock:
                    {
                        _lock.gameObject.SetActive(true);
                        blockTypeNode = _lock;
                        break;
                    }
                    case BlockTypeEnum.Bomb:
                    {
                        _bomb.gameObject.SetActive(true);
                        blockTypeNode = _bomb;
                        break;
                    }
                }

                _failTipsController = new FailTipsController(this);
                _failTipsController.PushTip(blockTypeNode.gameObject.AddComponent<FailTipsBase>());
                CheckExtraFailTip(_failTipsController);
                _failTipsController.InitShow();
                break;
            }
        }
    }

    private void InitUI()
    {
        _fail.gameObject.SetActive(false);
        _infinite.gameObject.SetActive(false);
        _rugbyChallenge.gameObject.SetActive(false);
        _time.gameObject.SetActive(false);
        _grass.gameObject.SetActive(false);
        _lock.gameObject.SetActive(false);
        _bomb.gameObject.SetActive(false);
        _ice.gameObject.SetActive(false);
        _gridFull.gameObject.SetActive(false);
        _rankLeaderBoard.gameObject.SetActive(false);

        _playOnBtn.onClick.RemoveAllListeners();
        _tryAgainBtn.onClick.RemoveAllListeners();
        _leaveBtn.onClick.RemoveAllListeners();
        _giveUpBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.RemoveAllListeners();
        // _undoBtn.onClick.RemoveAllListeners();

        _playOnBtn.gameObject.SetActive(false);
        _tryAgainBtn.gameObject.SetActive(false);
        _leaveBtn.gameObject.SetActive(false);
        _giveUpBtn.gameObject.SetActive(false);
        // _undoBtn.gameObject.SetActive(false);
        _closeBtn.gameObject.SetActive(false);

        _OnBtnLevel = null;
        _OnBtnTryAgain = null;
        _OnBtnGiveUp = null;
        _OnBtnClose = null;
        _OnBtnPlayOn = null;

        _playOnValue.SetText(KapiTileModel.Instance.GetRebornCount().ToString());
        EventDispatcher.Instance.AddEvent<EventKapiTileRebornCountChange>(OnRebornCountChange);
        DestroyActions.Add(() => EventDispatcher.Instance.RemoveEvent<EventKapiTileRebornCountChange>(OnRebornCountChange));
    }

    public void OnRebornCountChange(EventKapiTileRebornCountChange evt)
    {
        _playOnValue.SetText(KapiTileModel.Instance.GetRebornCount().ToString());
    }

    private List<Action> DestroyActions = new List<Action>();
    private void OnDestroy()
    {
        foreach (var action in DestroyActions)
        {
            action.Invoke();
        }
    }

    private async void OnBtnGiveUp()
    {
        if (_failTipsController != null && await _failTipsController.ShowNext())
            return;
        CloseWindowWithinUIMgr(true);
        _OnBtnGiveUp?.Invoke();
    }

    private void OnBtnClose()
    {
        TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_RecoverGame);
        CloseWindowWithinUIMgr(true);
        _OnBtnClose?.Invoke();
    }

    private async void OnClickLeaveBtn()
    {
        if (_failTipsController != null && await _failTipsController.ShowNext())
            return;
        DecoManager.Instance.CurrentWorld.ShowByPosition();
        _OnBtnLevel?.Invoke();
        KapiTileModel.Instance.DealFail();
        CloseWindowWithinUIMgr(true);
        if (type == FailTypeEnum.EnterBack)
        {
            TileMatchGameManager.Instance.LevelFailedBi(FailTypeEnum.EnterBack);
        }

        TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_GoHome);
        if (!UIKapiTileMainController.Instance)
            DragonPlus.AudioManager.Instance.PlayMusic(1, true);
        UIKapiTileMainController.Show(false);
    }

    private void OnClickUnDoButton()
    {
        // if (UserData.Instance.CanAford(UserData.ResourceId.Prop_Back, 1))
        // {
        //     CloseWindowWithinUIMgr(true);
        //     TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_FailedContinuesGame,
        //         FailTypeEnum.Special);
        // }
        // else
        // {
        //     UIManager.Instance.OpenWindow(UINameConst.UIPopupBuyItem, UserData.ResourceId.Prop_Back);
        // }
        
        if (KapiTileModel.Instance.GetRebornCount() > 0)
        {
            KapiTileModel.Instance.AddRebornItem(-1,"use");
            CloseWindowWithinUIMgr(true);
            TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_FailedContinuesGame,
                FailTypeEnum.Special);
        }
        else
        {
            UIPopupKapiTileGiftBagController.Open();
        }
    }

    private void OnClickSuperBackOnButton()
    {
        // var playOnCoin = TileMatchConfigManager.Instance.GetInt("PlayOnGame");
        // if (UserData.Instance.CanAford(UserData.ResourceId.KapiTileReborn, playOnCoin))
        // {
        //     CloseWindowWithinUIMgr(true);
        //     UserData.Instance.ConsumeRes(UserData.ResourceId.Coin, playOnCoin,
        //         new GameBIManager.ItemChangeReasonArgs(BiEventTileGarden.Types.ItemChangeReason.UseLevel));
        //     TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_FailedContinuesGame,
        //         FailTypeEnum.GridFull);
        // }
        // else
        // {
        // }

        if (KapiTileModel.Instance.GetRebornCount() > 0)
        {
            KapiTileModel.Instance.AddRebornItem(-1,"use");
            CloseWindowWithinUIMgr(true);
            TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_FailedContinuesGame,
                FailTypeEnum.GridFull);
        }
        else
        {
            UIPopupKapiTileGiftBagController.Open();
        }
    }

    private void OnClickAddTime()
    {
        // CloseWindowWithinUIMgr(true);
        // TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_FailedContinuesGame, FailTypeEnum.Time);
        if (KapiTileModel.Instance.GetRebornCount() > 0)
        {
            KapiTileModel.Instance.AddRebornItem(-1,"use");
            CloseWindowWithinUIMgr(true);
            TileMatchEventManager.Instance.SendEventImmediately(GameEventConst.GameEvent_FailedContinuesGame,
                FailTypeEnum.Time);
        }
        else
        {
            UIPopupKapiTileGiftBagController.Open();
        }
    }

    // private void OnClickTryAgainButton()
    // {
    //     if (EnergyModel.Instance.IsEnergyEmpty())
    //     {
    //         UIManager.Instance.OpenWindow(UINameConst.UIPopupBuyHp, true);
    //         return;
    //     }
    //
    //     CloseWindowWithinUIMgr(true);
    //     TileMatchEventManager.Instance.SendEvent(GameEventConst.GameEvent_Replay);
    //     
    // }
}