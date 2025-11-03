using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UISeaRacingMainController : UIWindowController
{
    private Transform GuideNode;
    private LocalizeTextMeshProUGUI GuideText;
    private Button StartBtn;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private const int HangPointCount = 10;
    private Transform DefalutPlayerNode;
    private List<PlayerNodeController> PlayerNodeControllerList = new List<PlayerNodeController>();
    public float UnitScore => (float) Storage.MaxScore / HangPointCount;
    private bool IsPad;
    private ScrollRect ScrollRect;
    private RectTransform Content;
    private LocalizeTextMeshProUGUI TileText;
    public override void PrivateAwake()
    {
        GuideNode = transform.Find("Guide");
        GuideNode.gameObject.SetActive(false);
        GuideText = GetItem<LocalizeTextMeshProUGUI>("Guide/Image/Bubble /Text");
        StartBtn = GetItem<Button>("Root/Button");
        StartBtn.onClick.AddListener(OnClickStartBtn);
        CloseBtn = GetItem<Button>("Root/Title/Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        DefalutPlayerNode = transform.Find("Root/Scroll View/Viewport/Content/Player");
        DefalutPlayerNode.gameObject.SetActive(false);
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/Title/Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime", 0f, 1.0f);
        for (var i = 1; i <= 3; i++)
        {
            AddRewardBtn(i);
        }

        // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SeaRacingMainUIProgress, transform.Find("Root/Scroll View/Viewport/Content/PointGuide") as RectTransform, moveToTarget: null);
        // GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SeaRacingMainUIReward, transform.Find("Root/Scroll View/Viewport/Content/RewardGuide") as RectTransform, moveToTarget: null);
        IsPad = transform.name.Contains("Pad");
        ScrollRect = transform.Find("Root/Scroll View").GetComponent<ScrollRect>();
        Content = transform.Find("Root/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        TileText = transform.Find("Root/Title/Root/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        
        EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,DealGuideFinish);
        EventDispatcher.Instance.AddEventListener(EventEnum.GuideFinish,DealPadGuideFinish);
    }

    public void AddRewardBtn(int rankLevel)
    {
        var btn = GetItem<Button>("Root/Scroll View/Viewport/Content/Reward" + rankLevel);
        btn.onClick.AddListener(() =>
        {
            if (Storage != null)
            {
                var rewards = Storage.GetRewardsByRank(rankLevel);
                SeaRacingTipController.ShowTip(btn.transform.position, rewards, btn.transform);
            }
        });
    }

    public void UpdateTime()
    {
        if (Storage == null)
            return;
        if (Storage.GetLeftTime() <= 0)
        {
            TimeText.SetText(Storage.GetLeftTimeText());
            CancelInvoke("UpdateTime");
            AnimCloseWindow();
        }
        else
        {
            if (gameObject.activeSelf)
            {
                PerformJumpPoint();
            }
            TimeText.SetText(Storage.GetLeftTimeText());
        }
    }

    public Vector2 GetPlayerAnchorPosition(SeaRacingPlayer player)
    {
        if (player.SortValue >= Storage.MaxScore)
        {
            return transform.Find("Root/Scroll View/Viewport/Content/Reward" + player.Rank)
                .GetComponent<RectTransform>().anchoredPosition;
        }
        else
        {
            var pointIndex = Mathf.FloorToInt(player.SortValue / UnitScore);
            var positionIndex = 0;
            for (var i = 1; i < player.Rank; i++)
            {
                var tempPointIndex = Mathf.FloorToInt(Storage.SortController().Players[i].SortValue / UnitScore);
                if (tempPointIndex == pointIndex)
                {
                    positionIndex++;
                }
            }

            var anchorPosition = transform.Find("Root/Scroll View/Viewport/Content/" + (pointIndex + 1))
                .GetComponent<RectTransform>().anchoredPosition;
            anchorPosition += GetPointOffset(positionIndex);
            return anchorPosition;
        }
    }

    public Vector2 GetCrossPlayerAnchorPosition(int positionIndex)
    {
        var anchorPosition = transform.Find("Root/Scroll View/Viewport/Content/" + (positionIndex + 1))
            .GetComponent<RectTransform>().anchoredPosition;
        return anchorPosition;
    }

    private const float UnitDistanceX = 30;

    public Vector2 GetPointOffset(int positionIndex)
    {
        if (positionIndex == 0)
            return Vector2.zero;
        positionIndex++;
        var distanceX = positionIndex / 2 * UnitDistanceX;
        var direction = positionIndex % 2 == 0 ? -1 : 1;
        return new Vector2(distanceX * direction, 0);
    }

    public void OnClickStartBtn()
    {
        CloseWindowWithinUIMgr(false,() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
                return;
            SceneFsm.mInstance.TransitionGame();
        });
    }

    public void OnClickCloseBtn()
    {
        if (_animator == null)
        {
            CloseWindowWithinUIMgr(false);
            return;
        }
        StartCoroutine(CommonUtils.PlayAnimation(_animator, UIAnimationConst.DisAppear, null, () =>
        {
            CloseWindowWithinUIMgr(false);
        }));
    }

    private StorageSeaRacingRound Storage;

    public void BindStorage(StorageSeaRacingRound storage)
    {
        if (storage != null)
        {
            if (Storage != storage)
            {
                if (Storage != null)
                {
                    foreach (var playerNode in PlayerNodeControllerList)
                    {
                        Destroy(playerNode.gameObject);
                    }

                    PlayerNodeControllerList.Clear();
                }

                Storage = storage;
                var sortController = Storage.SortController();
                for (var i = 1; i <= sortController.PlayerCount; i++)
                {
                    var newPlayerNodeObj = Instantiate(DefalutPlayerNode.gameObject, DefalutPlayerNode.parent);
                    newPlayerNodeObj.SetActive(true);
                    var playerNodeController = newPlayerNodeObj.AddComponent<PlayerNodeController>();
                    playerNodeController.BindMainController(this);
                    playerNodeController.BindPlayer(sortController.Players[i]);
                    PlayerNodeControllerList.Add(playerNodeController);
                }
                GuideText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("ui_searacing_npc_notice",storage.MaxScore.ToString()));
                TileText.SetTermFormats(storage.GetRoundString());
                UpdateSibling();
            }
            else
            {
                foreach (var playerNodeController in PlayerNodeControllerList)
                {
                    playerNodeController.RefreshPlayer();
                }
            }
        }
        else
        {
            Debug.LogError("主弹窗绑定round为null");
        }
    }

    public void UpdateSibling()
    {
        foreach (var playerNode in PlayerNodeControllerList)
        {
            playerNode.transform.SetSiblingIndex(15+5-playerNode.Player.Rank);
        }
    }
    private bool IsJumping = false;
    public async void PerformJumpPoint()
    {
        if (IsJumping)
            return;
        IsJumping = true;
        var taskList = new List<Task>();
        foreach (var playerNode in PlayerNodeControllerList)
        {
            if (playerNode.IsNeedUpdateState())
            {
                taskList.Add(playerNode.UpdateState());
            }
            
            playerNode.RefreshPlayer();
        }
        if (taskList.Count > 0)
        {
            UpdateSibling();
            CloseBtn.gameObject.SetActive(false);
            StartBtn.gameObject.SetActive(false);
            ShowGuide(false);
        }
        await Task.WhenAll(taskList);
        if (taskList.Count > 0)
        {
            CloseBtn.gameObject.SetActive(true);
            StartBtn.gameObject.SetActive(true);
            ShowGuide(true);   
        }
        if (Storage.Score >= Storage.MaxScore)
        {
            var windows = SeaRacingModel.OpenRoundRewardPopup(Storage);
            while (true)
            {
                if (windows != null && windows.isActiveAndEnabled)
                    await Task.Delay(300);
                else
                {
                    break;
                }
            }
            IsJumping = false;
        }
        else
        {
            IsJumping = false;
        }
    }

    private bool GuideShowState = false;
    public void ShowGuide(bool active, bool force = false)
    {
        if (GuideShowState == active)
            return;
        if (!active)
        {
            if (GuideNode.gameObject.activeSelf)
            {
                if (force)
                    GuideNode.gameObject.SetActive(false);
                else
                    GuideNode.GetComponent<Animator>().PlayAnimation("disappear");
            }
        }
        else
        {
            if (IsJumping)
                return;
            if (GuideNode.gameObject.activeSelf)
            {
                GuideNode.GetComponent<Animator>().PlayAnimation("appear");
            }
            else
            {
                GuideNode.gameObject.SetActive(true);
            }
        }

        GuideShowState = active;
    }

    public bool CheckGuide()
    {
        if (!GuideSubSystem.Instance.IsShowingGuide() && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SeaRacingMainUIDes) &&
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SeaRacingMainUIDes, null))
        {
            // if (IsPad)
            // {
            //     ScrollRect.enabled = false;
            //     var tempPos = Content.anchoredPosition;
            //     tempPos.y = 67.5f;
            //     Content.anchoredPosition = tempPos;
            //
            // }
            ShowGuide(false,true);
            CloseBtn.gameObject.SetActive(false);
            StartBtn.gameObject.SetActive(false);
            return true;
        }
        ShowGuide(false,true);
        XUtility.WaitSeconds(1f, () =>
        {
            if (!this)
                return;
            ShowGuide(true);
        });
        // if (IsPad)
        // {
        //     ScrollRect.enabled = false;
        //     XUtility.WaitSeconds(0.5f, () =>
        //     {
        //         if (!this)
        //             return;
        //         ScrollRect.enabled = true;
        //     });
        // }
        return false;
    }


    void DealGuideFinish(BaseEvent evt)
    {
        if(IsPad)
            return;
        
        if (evt.datas.Length < 1)
            return;
        var config = evt.datas[0] as TableGuide;
        if (config == null)
            return;
        if (config.triggerPosition == (int) GuideTriggerPosition.SeaRacingMainUIDes)
        {
            var btn = GetItem<Button>("Root/Scroll View/Viewport/Content/Reward1");
            var rewards = Storage.GetRewardsByRank(1);
            SeaRacingTipController.ShowTip(btn.transform.position, rewards, btn.transform,autoClose:false);
        }
        else if (config.triggerPosition == (int) GuideTriggerPosition.SeaRacingMainUIProgress)
        {
            ShowGuide(true);
            CloseBtn.gameObject.SetActive(true);
            StartBtn.gameObject.SetActive(true); 
        }
    }
    
    void DealPadGuideFinish(BaseEvent evt)
    {
        if(!IsPad)
            return;
        
        if (evt.datas.Length < 1)
            return;
        var config = evt.datas[0] as TableGuide;
        if (config == null)
            return;
        // if (config.triggerPosition == (int) GuideTriggerPosition.SeaRacingMainUIProgress)
        // {
        //     var tempPos = Content.anchoredPosition;
        //     tempPos.y = -67.5f;
        //     Content.anchoredPosition = tempPos;
        //     ScrollRect.enabled = true;
        // }
        
        if (config.triggerPosition == (int) GuideTriggerPosition.SeaRacingMainUIDes)
        {
            var btn = GetItem<Button>("Root/Scroll View/Viewport/Content/Reward1");
            var rewards = Storage.GetRewardsByRank(1);
            SeaRacingTipController.ShowTip(btn.transform.position, rewards, btn.transform,autoClose:false);
        }
        else if (config.triggerPosition == (int) GuideTriggerPosition.SeaRacingMainUIProgress)
        {
            ShowGuide(true);
            CloseBtn.gameObject.SetActive(true);
            StartBtn.gameObject.SetActive(true); 
        }
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,DealPadGuideFinish);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GuideFinish,DealGuideFinish);  
    }

    public static UISeaRacingMainController Open(StorageSeaRacingRound storage)
    {
        var popup = UIManager.Instance.OpenUI(UINameConst.UISeaRacingMain) as UISeaRacingMainController;
        popup.BindStorage(storage);
        if (!popup.CheckGuide())
            popup.PerformJumpPoint();
        return popup;
    }
}