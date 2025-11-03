using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Gameplay;
using IAPChecker;
using Spine;
using Spine.Unity;
using TMatch;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UICrocodileMainController : TMatch.UIWindowController
{
    #region open ui

    /// <summary>
    /// 预制体路径
    /// </summary>
    private const string PREFAB_PATH = "Prefabs/Activity/TMatch/Crocodile/UICrocodileMain";

    /// <summary>
    /// 打开
    /// </summary>
    public static void Open(Action action=null)
    {
        var uidata = new UIData();
        uidata.OnCallback = action;
        TMatch.UIManager.Instance.OpenWindow<UICrocodileMainController>(PREFAB_PATH,uidata);
    }

    #endregion
    private Button _closeBtn;
    private Button _helpBtn;
    private Button _closeButton;
    private Button _playButton;

    private Transform _topContent;
    private Transform _closeTipsGroup;
    private Transform _headGroup;
    private Transform _platStart;
    private Transform _platGroup;
    private Transform _pointGroup;
    private Transform _myHead;
    private Transform _robotHeadGroup;
    private Transform _bubble;

    private LocalizeTextMeshProUGUI _levelText;
    private LocalizeTextMeshProUGUI _playerCountText;
    public LocalizeTextMeshProUGUI TimerText;
    private LocalizeTextMeshProUGUI _prizeText;
    private LocalizeTextMeshProUGUI _tipsText;

    private Button _finalReward;

    private Transform _levelVfx;
    private Transform _playerVfx;

    private StorageActivityWinStreak _storage;
    private List<Transform> _robotHeadList;
    private List<RectTransform> _pointList;
    private int _showRobotCount;
    private int _robotCount;
    private int _storageLevel;
 
    private bool _canJump;

    private SkeletonGraphic _skeletonGraphic;
    private Action closeCallBack = null;
    /// <summary>
    /// 玩家组预制体和平台 UI 的偏移量
    /// </summary>
    private static readonly Vector3 OFFSET_BETWEEN_HEADGROUP_AND_PLATFORM = Vector3.up * 0.3f + Vector3.left * 0.1f;

    /// <summary>
    /// 台子消失动画名
    /// </summary>
    private const string PLAT_DISAPPEAR_ANIM_NAME = "disappear";

    private List<Vector3> _spinStartPos = new List<Vector3>()
    {
        new Vector3(345,-505,0),
        new Vector3(394,-627,0),
        new Vector3(246,-553,0),
        new Vector3(316,-423,0),
        new Vector3(-318,-358,0),
        new Vector3(-245,-223,0),
        new Vector3(-215,-135,0),
        new Vector3(-215,-135,0),
    };
    private List<Vector3> _eatPos = new List<Vector3>()
    {
        new Vector3(345,-505,0),
        new Vector3(394,-627,0),
        new Vector3(246,-553,0),
        new Vector3(316,-423,0),
        new Vector3(-318,-358,0),
        new Vector3(-245,-223,0),
        new Vector3(-215,-135,0),
        new Vector3(-215,-135,0),
    };

    private List<int> _spinRotation = new List<int>()
    {
        0, 0, 0, 0, 180, 180, 180, 180
    };
    public override void PrivateAwake()
    {
        _closeBtn = GetItem<Button>("Root/TopGroup/Content/CloseBtn");
        _closeBtn.onClick.AddListener(OnCloseBtnClicked);  
        
        _helpBtn = GetItem<Button>("Root/TopGroup/Content/HelpBtn");
        _helpBtn.onClick.AddListener(OnHelpBtnClicked);
       
        _closeButton = GetItem<Button>("Root/ButtonClose");
        _closeButton.onClick.AddListener(OnCloseBtnClicked);

        _playButton = GetItem<Button>("Root/ButtonPlay");
        _playButton.onClick.AddListener(() =>
        {
            OnCloseBtnClicked();
            UIViewSystem.Instance.Open<UITMatchLevelPrepareView>();
        });

        _topContent = transform.Find("Root/TopGroup/Content");
        _closeTipsGroup = transform.Find("Root/Text");
        _headGroup = transform.Find("Root/PlatformGroup/NameGroup");
        _platStart = transform.Find("Root/PlatformGroup/PlatStart");
        _platGroup = transform.Find("Root/PlatformGroup");
        _pointGroup = transform.Find("Root/BG/Bg/PointGroup");
        _myHead = transform.Find("Root/PlatformGroup/NameGroup/NameItemOne");
        _robotHeadGroup = transform.Find("Root/PlatformGroup/NameGroup/RobotGroup");
        _bubble = transform.Find("Root/Bubble");

        _levelText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/Content/Levels/NumberText");
        _playerCountText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/Content/Players/NumberText");
        TimerText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/Content/TimeGroup/TimeText");
        _prizeText = GetItem<LocalizeTextMeshProUGUI>("Root/PlatformGroup/FinalReward/PrizeGroup/NumberText");
        _tipsText = GetItem<LocalizeTextMeshProUGUI>("Root/TopGroup/Content/TipText");
        _tipsText.SetTerm("");
        
        _finalReward = GetItem<Button>("Root/PlatformGroup/FinalReward");
        _levelVfx = transform.Find("Root/TopGroup/Content/Levels/VFX_Hint_Stars_001");
        _playerVfx = transform.Find("Root/TopGroup/Content/Players/VFX_Hint_Stars_001");

        _skeletonGraphic = transform.Find("Root/PlatformGroup/Spine").GetComponent<SkeletonGraphic>();;
        GetJumpTargets();
        
        InvokeRepeating("UpdateRepeating", 0, 1);
    }

    protected override void OnOpenWindow(TMatch.UIWindowData data)
    {
        base.OnOpenWindow(data);
        if (data != null)
        {
            UIData uidata = data as UIData;
            closeCallBack = uidata.OnCallback;
        }
     
        TMatch.EventDispatcher.Instance.AddEventListener(EventEnum.RefreshWinStreak, OnCloseBtnClicked);
        _storage = CrocodileActivityModel.Instance.Storage;
        InitUI();
    }

    // public override void OnViewOpen<T>(UIViewParam param)
    // {
    //     base.OnViewOpen<WinStreakMainPopup_Fsm_Normal>(param);
    //
    //   
    // }
    private float PlaySkeletonAnimation(string animName)
    {
        if (_skeletonGraphic == null)
            return 0;

        TrackEntry trackEntry = _skeletonGraphic.AnimationState.GetCurrent(0);
        if (trackEntry != null && trackEntry.Animation != null && trackEntry.Animation.Name == animName)
            return trackEntry.AnimationEnd;

        _skeletonGraphic.AnimationState?.SetAnimation(0, animName, true);
        _skeletonGraphic.Update(0);
        if (trackEntry == null)
            return 0;
        return trackEntry.AnimationEnd;
    }
    private void UpdateRepeating()
    {
        var leftTime = CrocodileActivityModel.Instance.GetChallengeLeftTime();
         if (leftTime > 0)
         {
             TimerText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
         }
         else
         {
             TimerText.SetTerm("UI_lava_finished");
             CrocodileActivityModel.Instance.CheckChallengeIsOutTime();
         }
    }
    private void OnCloseBtnClicked(TMatch.BaseEvent evt)
    {
        OnCloseBtnClicked();
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        TMatch.EventDispatcher.Instance.RemoveEventListener(EventEnum.RefreshWinStreak, OnCloseBtnClicked);
        closeCallBack?.Invoke();
    }

    // public override void OnViewDestroy()
    // {
    //     base.OnViewDestroy();
    //     if (!MyMain.myGame.IsInMatch())
    //     {
    //         LobbyTaskSystem.Instance.FinishCurrentTask();
    //     }
    //     EventDispatcher.Instance.RemoveEventListener(EventEnum.RefreshWinStreak, OnCloseBtnClicked);
    // }
    //
    // public override Task OnViewClose()
    // {
    //     if (MyMain.myGame.IsInMatch())
    //     {
    //         if (_storage.EnterLevelResult)
    //         {
    //             Main.Game.Fsm.ChangeState<StateMakeover>(new FsmParamMakeover()
    //             {
    //                 lastMatchLevelData = TMatchSystem.LevelController.LevelData
    //             });
    //         }
    //         else
    //         {
    //             UIViewSystem.Instance.Open<UITMatchFailController>();
    //         }
    //     }
    //
    //     return base.OnViewClose();
    // }

    /// <summary>
    /// 获取所有预设点位
    /// </summary>
    private void GetJumpTargets()
    {
        var pointCnt = _pointGroup.childCount;
        _pointList = new List<RectTransform>(pointCnt);
        for (var i = 0; i < pointCnt; i++)
        {
            _pointList.Add(_pointGroup.GetChild(i).GetComponent<RectTransform>());
        }
    }

    private void InitUI()
    {
        _storageLevel = _storage.Level;
        _canJump = _storageLevel != _storage.EnterLevel;
        _robotCount = _storage.RobotCount;
        _skeletonGraphic.transform.localPosition = _spinStartPos[_storageLevel];
        _skeletonGraphic.transform.localRotation = new Quaternion(0,_spinRotation[_storageLevel],0,0);
        DealWithNotch();
        DealWithUI();
        DealWithPlatforms();
        InitHeadsToAPlatform();
        PreJump();
        DealWithJump();
    }

    /// <summary>
    /// 标题 UI 适配刘海
    /// </summary>
    private void DealWithNotch()
    {
        var currRes = Screen.currentResolution;
        var resHeight = currRes.height;
        var safeArea = Screen.safeArea;
        var safeHeight = safeArea.height;
        if (resHeight > safeHeight && _topContent != null)
        {
            var delta = resHeight - safeHeight;
            var offset = delta / 1000f;
            var currPos = _topContent.position;
            _topContent.position = new Vector3(currPos.x, currPos.y - offset);
        }

        DebugUtil.Log($"[WinStreakMainPopup] currRes: {currRes}");
        DebugUtil.Log($"[WinStreakMainPopup] safeArea.height: {safeHeight}");
    }

    /// <summary>
    /// 初始化 UI，并添加监听
    /// </summary>
    private void DealWithUI()
    {
        _levelText.SetText(_storageLevel + "/" + CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT);
        _playerCountText.SetText((_robotCount + 1) + "/100");
        _prizeText.SetText(CrocodileActivityModel.Instance.GetBaseConfig().RewardCnt[0].ToString());
        _bubble.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetTerm("");
        _bubble.gameObject.SetActive(false);

        UICrocodileMatchController.ShowLetterOnHeadIcon(_myHead, isOther: false);

        _helpBtn.gameObject.SetActive(!_canJump);
        _closeBtn.gameObject.SetActive(!_canJump);
        _playButton.gameObject.SetActive(!_canJump && !CrocodileActivityModel.Instance.IsFinishCurrentTurnReward());

        // 不跳，仅做展示
        if (!_canJump)
        {

            if (_storageLevel == 0)
            {
                _platStart.GetComponent<Button>().onClick.AddListener(() => OnPlatClicked(0));
            }

            _finalReward.onClick.AddListener(OnFinalRewardClicked);
            if (_storageLevel == CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT)
            {
                _tipsText.SetTerm("UI_lava_win_desc");
            }
            else
            {
                _tipsText.SetTerm("");
                var context =
                    LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_lava_normal_desc2",
                        (CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT - _storageLevel).ToString());
                _tipsText.SetText(context);
            }
        }
        // 要跳
        else
        {
            _tipsText.SetTerm("");
            if (_storage.EnterLevelResult)
            {
                _tipsText.SetTerm(_storageLevel == 6 ? "UI_lava_win_desc" : "UI_lava_advance_desc");
            }
            else
            {
                _tipsText.SetTerm("UI_lava_fail_desc");
            }
        }
    }

    /// <summary>
    /// 初始化台子
    /// </summary>
    private void DealWithPlatforms()
    {
        for (var i = 1; i <= CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT; i++)
        {
            // 消失的台子：需要初始化为消失状态
            if (i < _storageLevel)
            {
                if (!CrocodileActivityModel.Instance.IsFinishCurrentTurnReward() &&
                    !NotYetReachTheLastPlatform())
                {
                    continue;
                }

                var currPlatform = GetPlat(i);
                ResetDisappearedPlatform(currPlatform);
            }
            // 保留的台子：需要添加点击事件
            else
            {
                if (_canJump)
                {
                    continue;
                }

                var index = i;
                GetPlat(i).GetComponent<Button>().onClick.AddListener(() => OnPlatClicked(index));
            }
        }
    }

    /// <summary>
    /// 对于已经消失的台子，直接播放到消失动画的最后一帧
    /// </summary>
    /// <param name="platform"></param>
    private void ResetDisappearedPlatform(Transform platform)
    {
        var platAnim = platform.GetComponent<Animator>();
        var runtimeController = platAnim.runtimeAnimatorController;
        var clips = runtimeController.animationClips;
        foreach (var clip in clips)
        {
            if (!clip.name.Equals(PLAT_DISAPPEAR_ANIM_NAME))
            {
                continue;
            }

            platAnim.Play(PLAT_DISAPPEAR_ANIM_NAME, 0, 1);
        }
    }

    /// <summary>
    /// 将玩家和 robot 头像均归位于某个台阶上
    /// </summary>
    private void InitHeadsToAPlatform()
    {
        GetInitRobotHeads();

        if (_storageLevel < 1 || _storageLevel > CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT)
        {
            return;
        }

        var platPos = GetPlatPos(_storageLevel);
        _headGroup.position = platPos + OFFSET_BETWEEN_HEADGROUP_AND_PLATFORM * (_storageLevel < 4 ? 1 : 1.2f);
    }

    /// <summary>
    /// 获取所有 robot 头像，并初始化视图
    /// </summary>
    private void GetInitRobotHeads()
    {
        var hideRobotCount = _storageLevel >= 4 ? _storageLevel + 1 : 0;

        var robotTotalCount = _robotHeadGroup.childCount;
        _showRobotCount = robotTotalCount - hideRobotCount;
        _robotHeadList = new List<Transform>(_showRobotCount);
        for (var i = 0; i < robotTotalCount; i++)
        {
            
            var currRobotHead = _robotHeadGroup.GetChild(i);
            if (i < hideRobotCount)
            {
                currRobotHead.gameObject.SetActive(false);
                continue;
            }

            UICrocodileMatchController.ShowLetterOnHeadIcon(currRobotHead, isOther: true);
            _robotHeadList.Add(currRobotHead);
        }
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void PreJump()
    {
        if (!_canJump)
        {
            return;
        }

        CrocodileActivityModel.Instance.PreJump();
    }

    /// <summary>
    /// 处理跳跃逻辑
    /// </summary>
    /// <param name="willJump">是否即将跳跃</param>
    private void DealWithJump()
    {
        if (NotYetReachTheLastPlatform() && _canJump)
        {
            PlayJumpAnim();
            // BiUtil.SendGameEvent(BiEventProjectMatch.Types.GameEventType.GameEventLavaIconPop, data1: storage.EnterLevel.ToString(), data2: "main");
        }
    }

    /// <summary>
    /// 播放跳跃动画
    /// </summary>
    private async void PlayJumpAnim()
    {
        await Task.Delay(500);

        var nextPlatPos = GetPlatPos(_storageLevel + 1);
        var targetLocalPos = _headGroup.InverseTransformPoint(nextPlatPos);

        var currPlat = GetPlat(_storageLevel);
        var failAnim = "jump_fail";
        var successAnim = "jump_succeed";

        // 自己成功，跳下一台阶
        var enterLevelResult = _storage.EnterLevelResult;
        if (enterLevelResult)
        {
            PlayHeadAnim(_myHead, currPlat, successAnim);

            Jump(_myHead, targetLocalPos, 0.05f, () =>
            {
                _levelText.SetText((_storageLevel + 1) + "/" + CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT);
                _levelVfx.gameObject.SetActive(true);
            });
        }

        // 成功的机器人
        var failRobotList = GetFailRobotList();
        var maxDelay = 0f;
        targetLocalPos -= _myHead.localPosition;
        for (var i = 0; i < _robotHeadList.Count; i++)
        {
            var head = _robotHeadList[i];
            var targetPos = targetLocalPos + head.localPosition;
            if (!head.gameObject.activeSelf || failRobotList.Contains(i))
            {
                continue;
            }

            var delay = 0.2f + Random.Range(0f, 0.5f);
            maxDelay = Math.Max(maxDelay, delay);
            PlayHeadAnim(head, currPlat, successAnim, delay);
            Jump(head, targetPos, delay + 0.05f);
        }

        await Task.Delay((int)(maxDelay * 1000) + 200);

        // 自己失败，往下跳的动画
        if (!enterLevelResult)
        {
            PlayHeadAnim(_myHead, currPlat, failAnim);
            
            var data2 = TMatchModel.Instance.GetMainLevel().ToString();
        
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTmLavaquestFailed,
                CrocodileActivityModel.Instance.Storage.ChallengeTimes.ToString(),data2);
        }

        // 失败的机器人
        foreach (var t in failRobotList)
        {
            var head = _robotHeadList[t];
            if (head == null)
            {
                Debug.LogError("找寻头像错误：" + failRobotList.ToString());
                continue;
            }

            PlayHeadAnim(head, currPlat, failAnim);
        }

        // 台子消失动画
        if (_storageLevel > 0)
        {
            var animator = GetPlat(_storageLevel).GetComponent<Animator>();
            animator.Play(PLAT_DISAPPEAR_ANIM_NAME);
            await Task.Delay(500);
            Vector3 poseat;
            if (_spinRotation[_storageLevel] > 0)
            {
                poseat = animator.transform.localPosition + new Vector3(-130,-30,0);
            }
            else
            {
                poseat = animator.transform.localPosition + new Vector3(130,-30,0);
            }
            _skeletonGraphic.transform.DOLocalMove(poseat,1f);
            await Task.Delay(100);
            PlaySkeletonAnimation("2");
            await Task.Delay(1000);
            foreach (var t in failRobotList)
            {
                var head = _robotHeadList[t];
                if (head == null)
                {
                    Debug.LogError("找寻头像错误：" + failRobotList.ToString());
                    continue;
                }
                head.gameObject.SetActive(false);
            }

            if (!enterLevelResult)
            {
                _myHead.gameObject.SetActive(false);
            }

            await Task.Delay(1000);
            PlaySkeletonAnimation("1");
            Vector3 pos;
            if (_spinRotation[_storageLevel] > 0)
            {
                 pos = animator.transform.localPosition + new Vector3(800,0,0);
            }
            else
            {
                pos = animator.transform.localPosition + new Vector3(-800,0,0);
            }
            _skeletonGraphic.transform.DOLocalMove(pos,1f);

        }

        OnJumpEnd();
    }

    /// <summary>
    /// 播放一个原地动画
    /// </summary>
    /// <param name="head">当前玩家头像</param>
    /// <param name="currPlat">当前跳台</param>
    /// <param name="animName">动画名</param>
    private async void PlayHeadAnim(Transform head, Transform currPlat, string animName, float delay = 0f)
    {
        if (head == null || currPlat == null || string.IsNullOrEmpty(animName))
        {
            return;
        }

        var isFail = animName.Contains("fail") || animName.Contains("Fail");
        if (isFail)
        {
            if (currPlat == _platStart)
            {
                var distance = 500;
                var duration = distance / 1000f;
                head.DOLocalMoveX(head.localPosition.x + distance, duration).OnComplete(() =>
                {
                    var anim = head.GetComponent<Animator>();
                    if (anim == null)
                    {
                        return;
                    }
                    
                    anim.Play(animName);
                });
                return;
            }

            head.SetParent(currPlat);

            if (!_storage.EnterLevelResult)
            {
                _myHead.SetAsLastSibling();
            }
        }

        var anim = head.GetComponent<Animator>();
        if (anim == null)
        {
            return;
        }

        await Task.Delay((int)(delay * 1000));

        anim.Play(animName);
    }

    /// <summary>
    /// 执行一个跳跃动画
    /// </summary>
    /// <param name="head"></param>
    /// <param name="targetPos"></param>
    /// <param name="status">1成功跳上台子，2跳岩浆</param>
    private async void Jump(Transform head, Vector3 targetPos, float delayTime = 0, Action completeCallBack = null)
    {
        if (delayTime > 0)
        {
            await Task.Delay((int)(delayTime * 1000));
        }

        head.DOLocalJump(targetPos, 100, 1, 0.33f).OnComplete(() => { completeCallBack?.Invoke(); });
    }

    /// <summary>
    /// 获取失败机器人列表，并排序
    /// </summary>
    /// <returns></returns>
    private List<int> GetFailRobotList()
    {
        var minFailCount = 2;
        var levelThreshold = 3;
        var lowLevelFailCount = 7;
        var highLevelFailCount = 5;
        var simulateFailedCount = Random.Range(minFailCount, _storageLevel <= levelThreshold ? lowLevelFailCount : highLevelFailCount);
        simulateFailedCount = simulateFailedCount > _showRobotCount - 2 ? _showRobotCount - 2 : simulateFailedCount;

        var robotList = CommonUtils.GenerateRandom(simulateFailedCount, 0, _showRobotCount);
        robotList.Sort();

        return robotList;
    }

    /// <summary>
    /// 是否还没有到达最后一级
    /// </summary>
    /// <returns></returns>
    private bool NotYetReachTheLastPlatform()
    {
        return _storageLevel < CrocodileActivityModel.Instance.TOTAL_PLAT_COUNT;
    }

    #region DealWithBubble

    /// <summary>
    /// 气泡箭头类型（左/右/上）
    /// </summary>
    private enum BubbleArrowType
    {
        Left,
        Right,
        Up,
    }

    /// <summary>
    /// 气泡数据
    /// </summary>
    private struct BubbleData
    {
        public string information;
        public Vector3 position;
        public BubbleArrowAndOffsets bubbleArrowAndOffsets;
    }

    /// <summary>
    /// 气泡箭头和偏移
    /// </summary>
    private struct BubbleArrowAndOffsets
    {
        public BubbleArrowType arrowType;
        public int offsetX;
        public int offsetY;

        public BubbleArrowAndOffsets(BubbleArrowType _arrowType,int _offsetX,int _offsetY)
        {
            arrowType = _arrowType;
            offsetX = _offsetX;
            offsetY = _offsetY;
        }
    }

    /// <summary>
    /// 创建新气泡
    /// </summary>
    /// <param name="information">气泡内容</param>
    /// <param name="position">气泡位置</param>
    /// <returns></returns>
    private BubbleData CreateBubble(string information, Vector3 position, BubbleArrowAndOffsets bubbleArrowAndOffsets)
    {
        return new BubbleData
        {
            information = information,
            position = position,
            bubbleArrowAndOffsets = bubbleArrowAndOffsets,
        };
    }

    /// <summary>
    /// 点击平台
    /// </summary>
    /// <param name="index"></param>
    private void OnPlatClicked(int index)
    {
        var target = index == 0 ? _platStart : GetPlat(index);
        var position = target.position;
        var currPlatInfo = LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_lava_explain3", (_robotCount + 1).ToString());
        var otherPlatInfo = LocalizationManager.Instance.GetLocalizedString("UI_lava_explain2");
        var textStr = _storageLevel == index ? currPlatInfo : otherPlatInfo;

        var bubbleDetailList = new List<BubbleArrowAndOffsets>()
        {
            new BubbleArrowAndOffsets(BubbleArrowType.Left,  0,  100 ),
            new BubbleArrowAndOffsets(BubbleArrowType.Left,   40,  100-10 ),
            new BubbleArrowAndOffsets(BubbleArrowType.Right, 30, 100 -20),
            new BubbleArrowAndOffsets(BubbleArrowType.Left, 0,100-30 ),
            new BubbleArrowAndOffsets(BubbleArrowType.Right, 0, 100 -30),
            new BubbleArrowAndOffsets(BubbleArrowType.Left,  0, 100-30),
            new BubbleArrowAndOffsets(BubbleArrowType.Right, 0,  100 -35),
            new BubbleArrowAndOffsets(BubbleArrowType.Left, 0, 100 -40)
        };
        

        var bubbleData = CreateBubble(textStr, position, bubbleDetailList[index]);
        ShowBubble(bubbleData);
    }

    /// <summary>
    /// 根据气泡数据，显示气泡
    /// </summary>
    /// <param name="bubbleData"></param>
    private void ShowBubble(BubbleData bubbleData)
    {
        if (_bubble.gameObject.activeSelf)
        {
            _bubble.gameObject.SetActive(false);
            return;
        }

        // 位置
        _bubble.position = bubbleData.position;

        // text
        var bubbleInfo = bubbleData.information;
        var bubbleText = _bubble.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        bubbleText.SetText(bubbleInfo);

        // // 显示对应的箭头
        // var leftArrow =_bubble.Find("Left").gameObject; 
        // var rightArrow =_bubble.Find("Right").gameObject; 
        // var upArrow =_bubble.Find("Up").gameObject; 
        //
        // leftArrow.SetActive(IsArrowType(bubbleData, BubbleArrowType.Left));
        // rightArrow.SetActive(IsArrowType(bubbleData, BubbleArrowType.Right));
        // upArrow.SetActive(IsArrowType(bubbleData, BubbleArrowType.Up));

        // 更正一下位置
        var offsetX = bubbleData.bubbleArrowAndOffsets.offsetX;
        var offsetY = bubbleData.bubbleArrowAndOffsets.offsetY;
        if (offsetX != 0f || offsetY != 0f)
        {
            var localPos = _bubble.localPosition;
            localPos.x += offsetX;
            localPos.y += offsetY;
            _bubble.localPosition = localPos;
        }

        _bubble.gameObject.SetActive(true);
    }

    /// <summary>
    /// 判断箭头指向是否与输入参数一致
    /// </summary>
    private bool IsArrowType(BubbleData bubbleData, BubbleArrowType arrowType)
    {
        return bubbleData.bubbleArrowAndOffsets.arrowType == arrowType;
    }

    /// <summary>
    /// 点击大奖
    /// </summary>
    private void OnFinalRewardClicked()
    {
        var textStr = LocalizationManager.Instance.GetLocalizedString("UI_lava_explain1");
        var position = _finalReward.transform.position;
        var bubbleDetails = new BubbleArrowAndOffsets { arrowType = BubbleArrowType.Up, offsetX = 0, offsetY = 0 };
        var bubbleData = CreateBubble(textStr, position, bubbleDetails);
        ShowBubble(bubbleData);
    }

    #endregion

    private void OnCloseBtnClicked()
    {
        CloseWindowWithinUIMgr();
    }

    private void OnHelpBtnClicked()
    {
        UICrocodileHelpController.Open();
    }

    /// <summary>
    /// 获取平台位置信息
    /// </summary>
    private Vector3 GetPlatPos(int level)
    {
        var pointIndex = level - 1;
        return pointIndex < 0 ? Vector3.zero : _pointList[pointIndex].position;
    }

    /// <summary>
    /// 获取平台 Transform
    /// </summary>
    private Transform GetPlat(int level)
    {
        return level == 0 ? _platStart : _platGroup.Find("Plat" + level);
    }

    /// <summary>
    /// 跳跃 Ending 时
    /// </summary>
    private async void OnJumpEnd()
    {
        // 剩余人数 DOTween
        var robotCount = _storage.RobotCount;
        var leftCount = _storage.EnterLevelResult ? robotCount + 1 : robotCount;
        var startNumber = _robotCount;
        DOTween.To(() => startNumber, x => startNumber = x, leftCount, 0.2f)
            .SetEase(Ease.OutQuint)
            .OnUpdate(() =>
            {
                _playerCountText.SetText(startNumber + "/100");
                if (startNumber == leftCount)
                {
                    _playerVfx.gameObject.SetActive(true);
                }
            });

        // 还没有连赢成功，点击界面以继续
        if (!_storage.ChallengeResult)
        {
            _closeTipsGroup.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(true);
            return;
        }

        // 连赢成功，延时一会儿弹出领奖界面
        await Task.Delay(500);
        PopupSuccess();
    }

    /// <summary>
    /// 弹出领奖界面
    /// </summary>
    private void PopupSuccess()
    {
        UICrocodileSuccessController.Open();
    }


}