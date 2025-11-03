using System.Collections;
using System.Collections.Generic;
using System.Resources;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DragonU3DSDK.Asset;
using Game;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;

public class UIPopupLevelRankingShowController : UIWindowController
{
    private static string constPlaceId = "LevelRankingShow";
    
    private Button _buttonClose;
    private LocalizeTextMeshProUGUI _resetTimeText;
    private Animator _animator;
    private RectTransform _content;

    private GameObject _itemOther;
    private GameObject _itemSelf;

    private List<UIDailyRankItem> _dailyRankItems = new List<UIDailyRankItem>();

    private List<Image> _rewardIcons = new List<Image>();
    public override void PrivateAwake()
    {
        _buttonClose = GetItem<Button>("Root/MainGroup/CloseButton");
        _buttonClose.onClick.AddListener(OnCloseClicked);

        _animator = transform.GetComponent<Animator>();
        
        _resetTimeText = GetItem<LocalizeTextMeshProUGUI>("Root/MainGroup/TimeGroup/TimeText");
        _content = GetItem<RectTransform>("Root/MainGroup/Scroll View/Viewport/Content");

        Button helpBtn = GetItem<Button>("Root/MainGroup/HelpButton");
        helpBtn.onClick.AddListener(() =>
        {
            
        });
        
        EventDispatcher.Instance.AddEventListener(EventEnum.DAILY_RANK_UPDATE, RefreshItems);
        
        _itemOther = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/LevelRanking/LevelRankingOtherItem");
        _itemSelf = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/LevelRanking/LevelRankingSelfItem");

        for (int i= 1; i <= 5; i++)
        {
            _rewardIcons.Add(transform.Find("Root/BGGroup/reward_" + i + "/Image").gameObject.GetComponent<Image>());
        }
        InvokeRepeating("RepeatingTime", 0, 1);
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.DAILY_RANK_UPDATE,RefreshItems);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);

        DailyRankModel.Instance._curDailyRank.IsShowStartView = true;
        
        CancelInvoke();
        InvokeRepeating("RepeatingTime", 0, 1);

        InitView();
        DailyRankModel.Instance.UpdateRobotInfo(false, null);
    }

    private void InitView()
    {
        var itemSelf = Instantiate(_itemSelf, _content);
        _dailyRankItems.Add(itemSelf.AddComponent<UIDailyRankItem>());
        _dailyRankItems[0].InitData(1, LocalizationManager.Instance.GetLocalizedString("ui_common_me"), DailyRankModel.Instance._curDailyRank.CurScore, true, canvas.sortingOrder, DailyRankModel.Instance._curDailyRank);

        int index = 1;
        foreach (var robot in DailyRankModel.Instance._curDailyRank.Robots)
        {
            var item = Instantiate(_itemOther, _content);
            _dailyRankItems.Add(item.AddComponent<UIDailyRankItem>());
            _dailyRankItems[index].InitData(++index, robot.RobotName, robot.CurScore, false, canvas.sortingOrder, DailyRankModel.Instance._curDailyRank);
            
        }

        index = 0;
        foreach (var resData in DailyRankModel.Instance.GetRankReward(1, null))
        {
            _rewardIcons[index++].sprite = UserData.GetResourceIcon(resData.id, UserData.ResourceSubType.Big);
        }
        

        RefreshItems(null);
    }

    private void OnCloseClicked()
    {
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(true);
        }));
    }
    
    
    public override void ClickUIMask()
    {
        if (!canClickMask)
            return;

        canClickMask = false;
        OnCloseClicked();
    }
    
    private void RefreshItems(BaseEvent baseEvent)
    {
        if(_dailyRankItems == null)
            return;
        
        for (int i = 0; i < _dailyRankItems.Count; i++)
        {
            int score = 0;
            if (_dailyRankItems[i]._isSelf)
                score = DailyRankModel.Instance._curDailyRank.CurScore;
            else
            {
                StorageDailyRankRobot robot = DailyRankModel.Instance._curDailyRank.Robots.Find(a => a.RobotName.Equals(_dailyRankItems[i]._userName));
                if (robot != null)
                    score = robot.CurScore;
            }
            
            _dailyRankItems[i].UpdateData(i + 1, score);
        }
        
        _dailyRankItems.Sort((x, y) => { return y._scoreValue - x._scoreValue; });
        if (_dailyRankItems[1]._isSelf && _dailyRankItems[1]._scoreValue == _dailyRankItems[0]._scoreValue)
        {
            UIDailyRankItem temp = _dailyRankItems[1];
            _dailyRankItems[1] = _dailyRankItems[0];
            _dailyRankItems[0] = temp;
        }
        
        for (int i = 0; i < _dailyRankItems.Count; i++)
        {
            int score = 0;
            if (_dailyRankItems[i]._isSelf)
                score = DailyRankModel.Instance._curDailyRank.CurScore;
            else
            {
                StorageDailyRankRobot robot = DailyRankModel.Instance._curDailyRank.Robots.Find(a => a.RobotName.Equals(_dailyRankItems[i]._userName));
                if (robot != null)
                    score = robot.CurScore;
            }
            
            _dailyRankItems[i].UpdateData(i + 1, score);
            _dailyRankItems[i].transform.SetSiblingIndex(i);
        }
    }

    private void RepeatingTime()
    {
        _resetTimeText.SetText(DailyRankModel.Instance.GetActiveTime());
    }
    
    public static bool CanShowUI()
    {
        if (!UnlockManager.IsOpen(UnlockManager.MergeUnlockType.DailyRank))
            return false;

        StorageDailyRank rankDaily = DailyRankModel.Instance.CheckActivityEnd();
        if (rankDaily != null && ResExist())
        {
            DailyRankModel.Instance.UpdateRobotInfo(true, rankDaily);

            var mainPopup = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupLevelRankingMain) as UIPopupLevelRankingMainController;
            if(!mainPopup)
                mainPopup = UIManager.Instance.OpenUI(UINameConst.UIPopupLevelRankingMain, rankDaily) as UIPopupLevelRankingMainController;
            if (mainPopup)
            {
                rankDaily.IsShowEndView = true;
                return true;
            }
        }

        if (DailyRankModel.Instance.CheckActivityStart())
        {
            UIManager.Instance.OpenUI(UINameConst.UIPopupLevelRankingStart);
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,
                CommonUtils.GetTimeStamp());
            return true;
        }

        if (DailyRankModel.Instance.IsOpenActivity() && !DailyRankModel.Instance.CheckActivityStart())
        {
            if (CoolingTimeManager.Instance.IsCooling(CoolingTimeManager.CDType.OtherDay, constPlaceId))
                return false;
            CoolingTimeManager.Instance.UpdateCoolTime(CoolingTimeManager.CDType.OtherDay, constPlaceId,
                CommonUtils.GetTimeStamp());
            UIManager.Instance.OpenUI(UINameConst.UIPopupLevelRankingShow);
            return true;
        }

        return false;
    }
    private static List<string> ResList = new List<string>()
    {
        "prefabs/activity/levelranking.ab",
        "spriteatlas/activityatlas/levelrankingatlas/hd.ab",
        "spriteatlas/activityatlas/levelrankingatlas/sd.ab",
    };
    public static bool ResExist()
    {
        return ActivityManager.Instance.CheckResExist(ResList);
    }
}