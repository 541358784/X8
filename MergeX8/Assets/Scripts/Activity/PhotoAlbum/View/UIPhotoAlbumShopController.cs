using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decoration;
using DG.Tweening;
using DragonPlus;
using DragonPlus.UI;
using DragonU3DSDK.Storage;
using Mosframe;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPhotoAlbumShopController:UIWindowController
{
    private LocalizeTextMeshProUGUI TimeText;
    private StoragePhotoAlbum Storage;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI ScoreText;
    
    // private Transform DefaultLevelGroup;
    // private List<StoreItemLevel> LevelList = new List<StoreItemLevel>();
    
    // private StoreItemLevel LeaderBoardLevelItem;
    // private RectTransform Content;
    // private ScrollRect ScrollView;
    // private Button PreViewBtn;
    private Button ProgressViewBtn;
    private Button EnterMergeBtn;
    public override void PrivateAwake()
    {
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0f,01f);
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(OnClickCloseBtn);
        ScoreText = GetItem<LocalizeTextMeshProUGUI>("Root/NumGroup/Text");
        EventDispatcher.Instance.AddEvent<EventPhotoAlbumScoreChange>(OnScoreChange);
        // Content = transform.Find("Root/Scroll View/Viewport/Content").GetComponent<RectTransform>();
        // ScrollView = transform.Find("Root/Scroll View").GetComponent<ScrollRect>();
        
        // DefaultLevelGroup = transform.Find("Root/Grid/1");
        // DefaultLevelGroup.gameObject.SetActive(false);
        
        // LeaderBoardLevelItem = transform.Find("Root/Scroll View/Viewport/Content/RankGroup").gameObject.AddComponent<LeaderBoardLevel>();
        EventDispatcher.Instance.AddEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
        
        
        // PreViewBtn = GetItem<Button>("Root/PreviewButton");
        // PreViewBtn.onClick.AddListener(() =>
        // {
        //     // GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PhotoAlbumPreview);
        //     var mainUI = UIManager.Instance.GetOpenedUIByPath<UIPhotoAlbumMainController>(UINameConst.UIPhotoAlbumMain);
        //     if (mainUI)
        //         mainUI.AnimCloseWindow();
        //     AnimCloseWindow(() =>
        //     {
        //         var rewards = new List<int>();
        //         foreach (var pair in PhotoAlbumModel.Instance.StoreItemConfig)
        //         {
        //             var config = pair.Value;
        //             var type = (PhotoAlbumStoreItemType)config.Type;
        //             if (type == PhotoAlbumStoreItemType.BuildItem)
        //             {
        //                 rewards.Add(config.RewardId[0]);
        //             }
        //         }
        //         for (var i = 0; i < rewards.Count; i++)
        //         {
        //             if (rewards[i] == PhotoAlbumModel.Instance.GlobalConfig.CenterDecoItem)
        //             {
        //                 rewards.RemoveAt(i);
        //                 rewards.Insert(0,PhotoAlbumModel.Instance.GlobalConfig.CenterDecoItem);
        //                 break;
        //             }
        //         }
        //         Action callback = ()=>
        //         {
        //             UIPhotoAlbumMainController.Open(Storage);
        //             UIPhotoAlbumShopController.Open(Storage);
        //         };
        //         if (SceneFsm.mInstance.GetCurrSceneType() == StatusType.Game)
        //         {
        //             SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome,
        //                 DecoOperationType.Preview, rewards,callback);
        //         }
        //         else
        //         {
        //             EventDispatcher.Instance.DispatchEventImmediately(EventEnum.NODE_PREVIEW, rewards,callback);
        //         }
        //     });
        // });
        // PreViewBtn.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        transform.DOKill(false);
        EventDispatcher.Instance.RemoveEvent<EventPhotoAlbumScoreChange>(OnScoreChange);
        EventDispatcher.Instance.RemoveEvent<EventPhotoAlbumBuyStoreItem>(OnBuyStoreItem);
    }

    public void OnBuyStoreItem(EventPhotoAlbumBuyStoreItem evt)
    {
        CheckPerformUnLockStoreLevel();
    }

    public void OnScoreChange(EventPhotoAlbumScoreChange evt)
    {
        UpdateScoreText();
    }
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StoragePhotoAlbum;
        UpdateScoreText();
        // var storeLevelConfigList = PhotoAlbumModel.Instance.StoreLevelConfig;
        // for (var i = 0; i < storeLevelConfigList.Count; i++)
        // {
        //     var levelItem = Instantiate(DefaultLevelGroup, DefaultLevelGroup.parent).gameObject
        //         .AddComponent<StoreItemLevel>();
        //     levelItem.gameObject.SetActive(true);
        //     levelItem.InitStoreItemLevel(Storage,storeLevelConfigList[i]);
        //     LevelList.Add(levelItem);
        // }

        // LeaderBoardLevelItem = transform.Find("Root/Scroll View/Viewport/Content/RankGroup").gameObject
        //     .AddComponent<StoreItemLevel>();
        // LeaderBoardLevelItem.InitStoreItemLevel(Storage,storeLevelConfigList.Last());
        // LeaderBoardLevelItem.transform.SetAsLastSibling();
        // LevelList.Add(LeaderBoardLevelItem);
        // ScrollView.enabled = false;
        // LayoutRebuilder.ForceRebuildLayoutImmediate(DefaultLevelGroup.parent as RectTransform);
        // SetContentPosition();
        XUtility.WaitSeconds(0.3f, () =>
        {
            // ScrollView.enabled = true;
            CheckPerformUnLockStoreLevel();
        });
        InitPhotoGroup();
        ProgressViewBtn = transform.Find("Root/ButtonProgress").GetComponent<Button>();
        ProgressViewBtn.onClick.AddListener(() =>
        {
            UIPopupPhotoAlbumProgressController.Open(Storage);
        });
        EnterMergeBtn = transform.Find("Root/Button").GetComponent<Button>();
        EnterMergeBtn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.PhotoAlbumStart);
            if (Storage.IsEnd)
            {
                UIPopupPhotoAlbumExitController.Open((b) =>
                {
                    if (b)
                    {
                        AnimCloseWindow(() =>
                        {
                            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                            {
                                SceneFsm.mInstance.TransitionGame();
                            }
                        });
                    }
                });
            }
            else
            {
                AnimCloseWindow(() =>
                {
                    if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
                    {
                        SceneFsm.mInstance.TransitionGame();
                    }
                });
            }
        });
        
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }

        if (!Storage.IsStart)
        {
            Storage.IsStart = true;
            PlayStory("PhotoAlbum_Guide").AddCallBack(() =>
            {
                var auxItem = EnterMergeBtn;
                List<Transform> topLayer = new List<Transform>();
                topLayer.Add(auxItem.transform);
                GuideSubSystem.Instance.RegisterTarget(GuideTargetType.PhotoAlbumStart, auxItem.transform as RectTransform,
                    topLayer: topLayer);
                if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.PhotoAlbumStart, null))
                {
                    
                }
            }).WrapErrors();
        }
    }

    public void CheckPerformUnLockStoreLevel()
    {
        var curStoreLevelConfig = Storage.GetCurStoreLevel();
        if (!Storage.UnLockStoreLevel.Contains(curStoreLevelConfig.Id))
        {
            // ScrollView.enabled = false;
            // var targetPosY = GetContentAnchorPosY(curStoreLevelConfig.Id);
            // targetPosY = Math.Min(MaxContentY, targetPosY);
            // targetPosY = Math.Max(MinContentY, targetPosY);
            // Content.DOAnchorPosY(targetPosY, 0.5f).OnComplete(() =>
            // {
            if (Pages[CurPageId].Config.PageLevels.Contains(curStoreLevelConfig.Id))
            {
                Pages[CurPageId].PerformUnlock(curStoreLevelConfig).AddCallBack(() =>
                {
                    Storage.UnLockStoreLevel.Add(curStoreLevelConfig.Id);
                }).WrapErrors();
                return;
            }   
            // DOVirtual.DelayedCall(0.3f, () =>
            // {
            //     Storage.UnLockStoreLevel.Add(curStoreLevelConfig.Id);
            //     if (!this)
            //         return;
            //     LevelList[curStoreLevelConfig.Id - 1].PerformUnlock(() =>
            //     {
            //         // ScrollView.enabled = true;
            //     });
            // }).SetTarget(transform);
            // });   
        }
    }
    private float UnitBaseHeight = 80;
    private float UnitRowHeight = 265;
    private float RankGroupHeight = 415;
    // private float MaxContentY
    // {
    //     get
    //     {
    //         var height = GetContentAnchorPosY(999);
    //         height += RankGroupHeight*Content.localScale.y;
    //         var viewHeight = (Content.parent as RectTransform).rect.height;
    //         return height - viewHeight;
    //     }
    // }

    // private float MinContentY = 0;
    // public void SetContentPosition()
    // {
    //     var curStoreLevelConfig = Storage.GetCurStoreLevel();
    //     var targetPosY = GetContentAnchorPosY(curStoreLevelConfig.Id);
    //     if (!Storage.UnLockStoreLevel.Contains(curStoreLevelConfig.Id))
    //     {
    //         targetPosY = GetContentAnchorPosY(curStoreLevelConfig.Id-1);
    //     }
    //     targetPosY = Math.Min(MaxContentY, targetPosY);
    //     var tempPos = Content.anchoredPosition;
    //     tempPos.y = targetPosY;
    //     Content.anchoredPosition = tempPos;
    // }

    // public float GetContentAnchorPosY(int storeLevelId)
    // {
    //     var storeLevelConfigList = PhotoAlbumModel.Instance.StoreLevelConfig;
    //     var anchorPosY = 0f;
    //     for (var i = 0; i < storeLevelConfigList.Count; i++)
    //     {
    //         var storeLevelConfig = storeLevelConfigList[i];
    //         if (storeLevelConfig.Id < storeLevelId)
    //         {
    //             var rowCount = (storeLevelConfig.StoreItemList.Count-1) / 3 + 1;
    //             anchorPosY += UnitBaseHeight + rowCount * UnitRowHeight;
    //         }
    //         else
    //         {
    //             break;
    //         }
    //     }
    //     anchorPosY *= Content.localScale.y;
    //     return anchorPosY;
    // }

    public void UpdateScoreText()
    {
        ScoreText.SetText(Storage.Score.ToString());
    }

    public void OnClickCloseBtn()
    {
        if (Storage.IsEnd)
        {
            UIPopupPhotoAlbumExitController.Open((b) =>
            {
                if (b)
                    AnimCloseWindow();
            });
        }
        else
        {
            AnimCloseWindow();   
        }
    }

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetLeftPreEndTimeText());
        if (Storage.GetLeftPreEndTime() <= 0 && !Storage.IsEnd)
        {
            AnimCloseWindow();
            CancelInvoke("UpdateTime");
            if (UIPopupPhotoAlbumShopBuyController.Instance)
                UIPopupPhotoAlbumShopBuyController.Instance.AnimCloseWindow();
        }
    }

    public async void PerformCollectPhotoPiece(int pieceId,bool isFull)
    {
        var pieceConfig = PhotoAlbumModel.Instance.PhotoPieceConfig[pieceId];
        var photoConfig = PhotoAlbumModel.Instance.PhotoConfig[pieceConfig.PhotoId];
        var page = Pages.Find(a => a.Config == photoConfig);
        DisableChangePageBtn = true;
        await page.PhotoGroup.PerformCollectPiece(pieceConfig,isFull);
        if (isFull)
        {
            await PlayStory("PhotoAlbum_Photo"+photoConfig.Id);
            await PerformPhoto(photoConfig);
            if (CurPageId >= Pages.Count - 1)
            {
                await UIPopupPhotoAlbumProgressController.Open(Storage).PerformCollectAll();
                var keys = PhotoAlbumModel.Instance.PhotoConfig.Keys.ToList();
                var storyKey = "PhotoAlbum_All_";
                foreach (var key in keys)
                {
                    storyKey += key.ToString();
                }
                Debug.LogError("相册StoryKey="+storyKey);
                await PlayStory(storyKey);
            }
            else
            {
                Pages[CurPageId].HideLeft();
                CurPageId++;
                Pages[CurPageId].ShowRight();
                UpdatePageBtnState();
                await XUtility.WaitSeconds(0.3f);
                CheckPerformUnLockStoreLevel();   
            }
        }
        DisableChangePageBtn = false;
    }
    
    public Task PerformPhoto(PhotoAlbumPhotoConfig photoConfig)
    {
        var task = new TaskCompletionSource<bool>();
        UIPopupPhotoAlbumSpineController.Open(photoConfig,false,true).AddCallback(()=>task.SetResult(true));
        return task.Task;
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
        var window = UIDigTrenchNewStoryController.Open(story, () =>
        {
            task.SetResult(true);
        });
        window.gameObject.setLayer(5,true);
        return task.Task;
    }
    
    public static UIPhotoAlbumShopController Instance;
    public static UIPhotoAlbumShopController Open(StoragePhotoAlbum storagePhotoAlbum)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPhotoAlbumShop, storagePhotoAlbum) as
            UIPhotoAlbumShopController;
        return Instance;
    }
}