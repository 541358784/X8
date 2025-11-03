using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupSummerWatermelonBreadMainController:UIWindowController
{
    public SummerWatermelonBreadMergeBoard _mergeBoard;
    private Slider _progressSlider;
    private int _progressLength;
    private Dictionary<int,Transform> _iconGroupList;
    private Button _playBtn;
    private Button _closeBtn;
    public Button _newItemBtn;
    private Image _newItemIcon;
    private Transform _newItemRedPoint;
    private LocalizeTextMeshProUGUI _newItemRedPointText;
    private StorageSummerWatermelonBread StorageSummerWatermelonBread => SummerWatermelonBreadModel.Instance.StorageSummerWatermelonBread;
    public StorageList<int> UnSetItems => SummerWatermelonBreadModel.Instance.UnSetItems;
    public StorageList<int> UnSetRewards => SummerWatermelonBreadModel.Instance.UnSetRewards;

    public override void PrivateAwake()
    {
        _progressSlider = GetItem<Slider>("Root/Scroll View/Viewport/Content/Slider");
        _iconGroupList = new Dictionary<int, Transform>();
        var iconIdx = 1;
        while (transform.Find("Root/Scroll View/Viewport/Content/IconGroup/" + iconIdx + "/Icon") != null)
        {
            _iconGroupList.Add(iconIdx,GetItem("Root/Scroll View/Viewport/Content/IconGroup/" + iconIdx + "/Icon").transform);
            iconIdx++;
        }
        _progressLength = iconIdx - 1;
        
        _playBtn = GetItem<Button>("Root/Button");
        _playBtn.onClick.AddListener(OnPlayBtn);

        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnCloseBtn);

        _newItemBtn = GetItem<Button>("Root/NewItem");
        _newItemBtn.onClick.AddListener(OnNewItemBtn);
        _newItemIcon = GetItem<Image>("Root/NewItem/Icon");
        _newItemRedPoint = GetItem("Root/NewItem/RedPoint").transform;
        _newItemRedPointText = GetItem<LocalizeTextMeshProUGUI>("Root/NewItem/RedPoint/Label");
    }
    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        _mergeBoard.StopAllTweenAnim();
        MergeManager.Instance.Refresh((int)MergeBoardEnum.Main);
    }

    private bool _isGuide;
    protected override void OnOpenWindow(params object[] objs)
    {
        if (_newItemBtn.GetComponent<ShieldButtonOnClick>() != null)
        {
            _newItemBtn.GetComponent<ShieldButtonOnClick>().isUse = false;   
        }

        base.OnOpenWindow(objs);
        _isGuide = !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SummerWatermelonBreadDes);//有引导时强制显示play按钮，隐藏临时背包按钮
        MergeManager.Instance.Refresh(MergeBoardEnum.SummerWatermelonBread);
        _mergeBoard = transform.Find("Root").GetComponentDefault<SummerWatermelonBreadMergeBoard>("Board");
        _mergeBoard.activeIndex = 20;
        // mergeBoard.SetBoardID((int)MergeBoardEnum.SummerWatermelonBread);
        RefreshView();
        if (_isGuide)
        {
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SummerWatermelonBreadEnterGame, _playBtn.transform as RectTransform, topLayer: new List<Transform>()
            {
                _playBtn.transform
            });
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SummerWatermelonBreadDes, null);
        }
        else if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SummerWatermelonBreadProgressInfo))
        {
            var progressTransform = transform.Find("Root/Scroll View");
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SummerWatermelonBreadProgressInfo, progressTransform as RectTransform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.SummerWatermelonBreadUnSetItem, _newItemBtn.transform as RectTransform, topLayer: new List<Transform>()
            {
                _newItemBtn.transform
            });
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.SummerWatermelonBreadProgressInfo, null);
        }
    }

    void RefreshNewItemBtnView()
    {
        _newItemBtn.gameObject.SetActive(!_isGuide && (UnSetRewards.Count+UnSetItems.Count) > 0);
        if ((UnSetRewards.Count+UnSetItems.Count) > 0)
        {
            _newItemIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(GameConfigManager.Instance.GetItemConfig((UnSetRewards.Count > 0 ? UnSetRewards:UnSetItems)[0]).image);
        }
        _newItemRedPoint.gameObject.SetActive((UnSetRewards.Count+UnSetItems.Count) > 0);
        _newItemRedPointText.gameObject.SetActive((UnSetRewards.Count+UnSetItems.Count) > 1);
        _newItemRedPointText.SetText((UnSetRewards.Count+UnSetItems.Count).ToString());
    }

    void RefreshPlayBtnView()
    {
        _playBtn.gameObject.SetActive(_isGuide || (UnSetRewards.Count + UnSetItems.Count) == 0);
    }
    void RefreshProgressView()
    {
        _progressSlider.value = Math.Max(0,StorageSummerWatermelonBread.MaxUnlockLevel - 1);
        foreach (var pair in _iconGroupList)
        {
            pair.Value.gameObject.SetActive(pair.Key <= StorageSummerWatermelonBread.MaxUnlockLevel);
        }
    }

    public Task GrowProgressView()
    {
        var curSliderValue = Math.Max(0, StorageSummerWatermelonBread.MaxUnlockLevel - 1);
        if ((int)_progressSlider.value != curSliderValue)
        {
            _progressSlider.transform.DOKill(true);
            var task = new TaskCompletionSource<bool>();
            DOTween.To(() => _progressSlider.value, v =>
            {
                _progressSlider.value = v;
            }, curSliderValue, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                RefreshProgressView();
                ShowProgressUnlockEffect(StorageSummerWatermelonBread.MaxUnlockLevel);
                task.SetResult(true);
            }).SetTarget(_progressSlider.transform);
            return task.Task;
        }
        else
        {
            RefreshProgressView();
            return Task.CompletedTask;
        }
    }
    private static GameObject _progressEffectObj;
    public static GameObject ProgressEffectObj
    {
        get
        {
            if (_progressEffectObj == null)
            {
                var prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/SummerWatermelonBread/VFX_Hint_Stars_002");
                _progressEffectObj = prefab;
            }
            return _progressEffectObj;
        }
    }
    public async void ShowProgressUnlockEffect(int level)
    {
        var icon = _iconGroupList[level];
        var effect = GameObject.Instantiate(ProgressEffectObj,icon);
        effect.transform.localPosition = Vector3.zero;
        effect.SetActive(false);
        effect.SetActive(true);
        await XUtility.WaitSeconds(2f);
        if (effect)
        {
            Destroy(effect);
        }
        //播放解锁动画
    }

    public void RefreshBtnView()
    {
        RefreshPlayBtnView();
        RefreshNewItemBtnView();
    }
    public void RefreshView()
    {
        RefreshBtnView();
        RefreshProgressView();
    }
    private ulong LastClickNewItemBtnTime;
    void OnNewItemBtn()
    {
        LastClickNewItemBtnTime = APIManager.Instance.GetServerTime();
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SummerWatermelonBreadUnSetItem);
        if ((UnSetRewards.Count + UnSetItems.Count) == 0)
            return;
        int emptyIndex = MergeManager.Instance.FindEmptyGrid(_mergeBoard.activeIndex,MergeBoardEnum.SummerWatermelonBread);
        if (emptyIndex == -1)
        {
            MergePromptManager.Instance.ShowTextPrompt(_newItemBtn.transform.position, 1.5f);
            return;
        }
        var newItemId = (UnSetRewards.Count > 0 ? UnSetRewards:UnSetItems)[0];
        (UnSetRewards.Count > 0 ?UnSetRewards:UnSetItems).RemoveAt(0);
        SummerWatermelonBreadModel.Instance.UnSetItemsCount--;
        RefreshBtnView();
        var storageMergeItem = MergeManager.Instance.GetEmptyItem();
        storageMergeItem.Id = newItemId;
        storageMergeItem.State = 1;
        MergeManager.Instance.SetNewBoardItem(emptyIndex, storageMergeItem.Id, 1, RefreshItemSource.rewards, MergeBoardEnum.SummerWatermelonBread,-1, false);
        AudioManager.Instance.PlaySound(13);
        
        // TableMergeItem mergeItem = MergeResourceManager.Instance.ResourcesTableMerge;
        FlyGameObjectManager.Instance.FlyObject(storageMergeItem.Id, _newItemBtn.transform.position,
            _mergeBoard.IndexToPosition(emptyIndex), 5f,
            () =>
            {
                ShakeManager.Instance.ShakeLight();
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardEnum.SummerWatermelonBread,emptyIndex, -1,
                    RefreshItemSource.rewards, storageMergeItem.Id);
                MergeGuideLogic.Instance.CheckMergeGuide();
                // if (mergeItem != null)
                //     MergeResourceManager.Instance.GetMergeResource(mergeItem, true);
            });
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GetReward, storageMergeItem.Id.ToString());
        // MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Reward);
    }
    void OnPlayBtn()
    {
        if ((APIManager.Instance.GetServerTime() - LastClickNewItemBtnTime) < 0.5f * XUtility.Second)
        {
            return;
        }
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.SummerWatermelonBreadEnterGame);
        AnimCloseWindow(() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
            {
                if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.SummerWatermelonBreadGameEntrance))
                {
                    GuideSubSystem.Instance.InGuideChain = true;
                    SummerWatermelonBreadModel.InGuideChain = true;   
                }
                SceneFsm.mInstance.TransitionGame();   
            }
        });
    }

    public void LockBoard()
    {
        _mergeBoard.LockBoard();
    }
    public void UnLockBoard()
    {
        _mergeBoard.UnLockBoard();
    }
    
    void OnCloseBtn()
    {
        AnimCloseWindow();
    }
}