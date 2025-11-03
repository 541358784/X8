using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIBiuBiuMainController : UIWindowController
{

    public static UIBiuBiuMainController Instance;
    public static UIBiuBiuMainController Open()
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIBiuBiuMain) as UIBiuBiuMainController;
        return Instance;
    }
    public BiuBiuMergeBoard mergeBoard;
    public void ReloadShowState()
    {
        _roundText.SetText(BiuBiuModel.Instance.Storage.Round.ToString());
        foreach (var pair in BubbleList)
        {
            BiuBiuUIConfig uiConfig = null;
            foreach (var configPair in BiuBiuModel.Instance.UIConfigDic)
            {
                if (configPair.Value.ShowIndex.Contains(pair.Key))
                {
                    uiConfig = configPair.Value;
                    break;
                }
            }
            pair.Value.Init(pair.Key,uiConfig);
        }
        UpdateRewardCountText();
    }

    public int LeftRewardCount;
    public void UpdateRewardCountText()
    {
        var leftRewardCount = 0;
        foreach (var fate in BiuBiuModel.Instance.Storage.Fate)
        {
            if (fate > 0)
            {
                leftRewardCount++;
            }
        }
        LeftRewardCount = leftRewardCount;
        CountText.SetTermFormats(LeftRewardCount.ToString());
    }

    public void HideReward(int position)
    {
        var bubble = BubbleList[position];
        bubble.HideReward();
    }

    public async Task PerformBiuBiu(int position,List<ResData> rewards,bool hasReward)
    {
        var biu = Instantiate(DefaultBiuBiu, DefaultBiuBiu.parent);
        biu.gameObject.SetActive(true);
        biu.transform.localScale = Vector3.one*3;
        var bubble = BubbleList[position];
        biu.transform.position = bubble.transform.position;
        var bubbleRect = biu.transform as RectTransform;
        var targetPosition = bubbleRect.anchoredPosition + new Vector2(58, 41);
        bubbleRect.anchoredPosition = targetPosition + new Vector2(200,200);
        var biuTime = 0.15f;
        bubbleRect.DOAnchorPos(targetPosition, biuTime).SetEase(Ease.Linear);
        bubbleRect.DOScale(Vector3.one, biuTime).SetEase(Ease.Linear);
        await XUtility.WaitSeconds(biuTime);
        if (!this)
            return;
        if (hasReward)
        {
            biu.gameObject.SetActive(false);
            LeftRewardCount--;
            CountText.SetTermFormats(LeftRewardCount.ToString());
        }
        else
        {
            var emptyText = Instantiate(DefaultEmptyText, DefaultEmptyText.parent);
            emptyText.gameObject.SetActive(true);
            emptyText.position = bubble.transform.position;
            XUtility.WaitSeconds(1f, () =>
            {
                if (!this)
                    return;
                DestroyImmediate(emptyText.gameObject);
            });
        }
        bubble.OpenBubble(rewards,hasReward);
        await XUtility.WaitSeconds(0.5f);
        if (!this)
            return;
        DestroyImmediate(biu.gameObject);
    }

    private Transform DefaultEmptyText;
    public Transform DefaultBiuBiu;
    public Dictionary<int, Bubble> BubbleList = new Dictionary<int, Bubble>();
    public class Bubble : MonoBehaviour
    {
        public Image RewardIcon;
        public Image BubbleIcon;
        public int Position;
        public bool ShowState => !BiuBiuModel.Instance.Storage.ShowState.Contains(Position);
        public BiuBiuUIConfig Config;
        public void Init(int position,BiuBiuUIConfig config)
        {
            Config = config;
            Position = position;
            RewardIcon = transform.Find("Item").GetComponent<Image>();
            BubbleIcon = transform.Find("Cover").GetComponent<Image>();
            gameObject.SetActive(ShowState);
            BubbleIcon.gameObject.SetActive(true);
            RewardIcon.gameObject.SetActive(false);
        }

        public void OpenBubble(List<ResData> rewards,bool hasReward)
        {
            var effectAsset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/BiuBiu/"+Config.EffectAsset);
            if (effectAsset)
            {
                var effect = Instantiate(effectAsset, transform);
                effect.transform.position = transform.position;
                XUtility.WaitSeconds(2f, () =>
                {
                    if (effect)
                        Destroy(effect);
                });
            }
            RewardIcon.sprite = UserData.GetResourceIcon(rewards[0].id, UserData.ResourceSubType.Big);
            RewardIcon.gameObject.SetActive(hasReward);
            BubbleIcon.gameObject.SetActive(false);
        }

        public void HideReward()
        {
            RewardIcon.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    
    
    
    
    
    
    
    
    
    
    private Button _playBtn;
    private Button _closeBtn;
    public Button _newItemBtn;
    private Image _newItemIcon;
    private Transform _newItemRedPoint;
    private LocalizeTextMeshProUGUI _newItemRedPointText;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _buyTimeText;
    private Button _helpBtn;
    private Button _buyBtn;
    private LocalizeTextMeshProUGUI _roundText;

    public LocalizeTextMeshProUGUI CountText;
    public override void PrivateAwake()
    {
        CountText = GetItem<LocalizeTextMeshProUGUI>("Root/Text (1)");
        _playBtn = GetItem<Button>("Root/Button");
        _playBtn.onClick.AddListener(OnPlayBtn);

        _closeBtn = GetItem<Button>("Root/ButtonClose");
        _closeBtn.onClick.AddListener(OnCloseBtn);

        _newItemBtn = GetItem<Button>("Root/NewItem");
        _newItemBtn.onClick.AddListener(OnNewItemBtn);
        _newItemIcon = GetItem<Image>("Root/NewItem/Icon");
        _newItemRedPoint = GetItem("Root/NewItem/RedPoint").transform;
        _newItemRedPointText = GetItem<LocalizeTextMeshProUGUI>("Root/NewItem/RedPoint/Label");
        _timeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        _helpBtn = GetItem<Button>("Root/ButtonHelp");
        _helpBtn.onClick.AddListener(OnBtnHelp);  
        _buyBtn = GetItem<Button>("Root/ButtonBuy");
        _buyBtn.onClick.AddListener(OnBtnBuy);
        _buyTimeText = GetItem<LocalizeTextMeshProUGUI>("Root/ButtonBuy/Text");
        _roundText = GetItem<LocalizeTextMeshProUGUI>("Root/RewardGroup/Tab/Num");
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_newItemBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BiuBiuNewItem, _newItemBtn.transform as RectTransform, topLayer: topLayer);
        List<Transform> topLayer2 = new List<Transform>();
        topLayer.Add(_playBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.BiuBiuEnterGame, _playBtn.transform as RectTransform, topLayer: topLayer2);

        DefaultBiuBiu = transform.Find("Root/RewardGroup/BiuBiu");
        DefaultBiuBiu.gameObject.SetActive(false);
        var position = 1;
        var bubbleNode = transform.Find("Root/RewardGroup/Reward/" + position);
        while (bubbleNode!= null)
        {
            var bubble = bubbleNode.gameObject.AddComponent<Bubble>();
            // bubble.Init(position);
            BubbleList.Add(position,bubble);
            position++;
            bubbleNode = transform.Find("Root/RewardGroup/Reward/" + position);
        }

        DefaultEmptyText = transform.Find("Root/RewardGroup/Null");
        DefaultEmptyText.gameObject.SetActive(false);
    }
    
    private void OnBtnBuy()
    {
        UIPopupBiuBiuPackageController.Open();
    }
    private void OnBtnHelp()
    {
        UIBiuBiuHelpController.Open();
    }
    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        mergeBoard.StopAllTweenAnim();
        MergeManager.Instance.Refresh((int)MergeBoardEnum.Main);
    }
    
    
    protected override void OnOpenWindow(params object[] objs)
    {
        if (_newItemBtn.GetComponent<ShieldButtonOnClick>() != null)
        {
            _newItemBtn.GetComponent<ShieldButtonOnClick>().isUse = false;   
        }

        base.OnOpenWindow(objs);
        MergeManager.Instance.Refresh(MergeBoardEnum.BiuBiu);
        mergeBoard = transform.Find("Root").GetComponentDefault<BiuBiuMergeBoard>("Board");
        mergeBoard.activeIndex = 1;
        RefreshView();
    
        InvokeRepeating("RefreshTime",0,1);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.BiuBiuNewItem, null);

        UpdateRewardCountText();
    }

    public void RefreshTime()
    {
        _timeText.SetText(BiuBiuModel.Instance.GetActivityLeftTimeString());
        _buyTimeText.SetText(BiuBiuModel.Instance.GetActivityLeftTimeString());
        if(BiuBiuModel.Instance.GetActivityLeftTime()<=0)
            AnimCloseWindow();
    }
    
    
    
    void RefreshPlayBtnView()
    {
        _playBtn.gameObject.SetActive( UnSetItems.Count == 0);
    }
    void RefreshNewItemBtnView()
    {
        _newItemBtn.gameObject.SetActive(UnSetItems.Count > 0);
        if ((UnSetItems.Count) > 0)
        {
            _newItemIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(GameConfigManager.Instance.GetItemConfig(UnSetItems[0]).image);
        }
        _newItemRedPoint.gameObject.SetActive((UnSetItems.Count) > 0);
        _newItemRedPointText.gameObject.SetActive(UnSetItems.Count > 1);
        _newItemRedPointText.SetText((UnSetItems.Count).ToString());
    }
    public void RefreshBtnView()
    {
        RefreshPlayBtnView();
        RefreshNewItemBtnView();
    }
    public void RefreshView()
    {
        RefreshBtnView();
        ReloadShowState();
    }
    public List<int> UnSetItems => BiuBiuModel.Instance.Storage.UnSetItems;
    private ulong LastClickNewItemBtnTime;
    void OnNewItemBtn()
    {
        LastClickNewItemBtnTime = APIManager.Instance.GetServerTime();
 
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BiuBiuNewItem);
        if ((UnSetItems.Count) == 0)
            return;
        int emptyIndex = MergeManager.Instance.FindEmptyGrid(mergeBoard.activeIndex,MergeBoardEnum.BiuBiu);
        if (emptyIndex == -1)
        {
            MergePromptManager.Instance.ShowTextPrompt(_newItemBtn.transform.position, 1.5f);
            return;
        }
        var newItemId = UnSetItems[0];
        UnSetItems.RemoveAt(0);
        RefreshBtnView();
        var storageMergeItem = MergeManager.Instance.GetEmptyItem();
        storageMergeItem.Id = newItemId;
        storageMergeItem.State = 1;
        MergeManager.Instance.SetNewBoardItem(emptyIndex, storageMergeItem.Id, 1, RefreshItemSource.rewards, MergeBoardEnum.BiuBiu,-1, false);
        AudioManager.Instance.PlaySound(13);
        
        // TableMergeItem mergeItem = MergeResourceManager.Instance.ResourcesTableMerge;
        FlyGameObjectManager.Instance.FlyObject(storageMergeItem.Id, _newItemBtn.transform.position,
            mergeBoard.IndexToPosition(emptyIndex), 5f,
            () =>
            {
                ShakeManager.Instance.ShakeLight();
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardEnum.BiuBiu,emptyIndex, -1,
                    RefreshItemSource.rewards, storageMergeItem.Id);
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
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.BiuBiuEnterGame);
        AnimCloseWindow(() =>
        {
            if (SceneFsm.mInstance.GetCurrSceneType() != StatusType.Game)
            {
                SceneFsm.mInstance.TransitionGame();   
            }
        });
    }

    public void LockBoard()
    {
        mergeBoard.LockBoard();
    }
    public void UnLockBoard()
    {
        mergeBoard.UnLockBoard();
    }
    
    void OnCloseBtn()
    {
        AnimCloseWindow();
    }
}