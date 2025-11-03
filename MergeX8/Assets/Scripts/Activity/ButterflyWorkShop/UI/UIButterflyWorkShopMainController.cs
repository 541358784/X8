using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ButterFlyWorkShop;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.ButterflyWorkShop;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIButterflyWorkShopMainController:UIWindowController
{
    [FormerlySerializedAs("_mergeBoard")] public ButterflyWorkShopMergeBoard mergeBoard;
    private List<Transform> _iconFinishList;
    private List<Animator> _aniList;
    private Button _playBtn;
    private Button _closeBtn;
    public Button _newItemBtn;
    private Image _newItemIcon;
    private Transform _newItemRedPoint;
    private LocalizeTextMeshProUGUI _newItemRedPointText;
    private StorageButterflyWorkShop StorageButterflyWorkShop => ButterflyWorkShopModel.Instance.StorageButterflyWorkShop;
    public StorageList<int> UnSetItems => ButterflyWorkShopModel.Instance.UnSetItems;
    private List<Transform> _lockItems;
    private LocalizeTextMeshProUGUI _timeText;
    private LocalizeTextMeshProUGUI _buyTimeText;
    private Button _helpBtn;
    private Button _buyBtn;
    public static UIButterflyWorkShopMainController Instance;
    private Transform _otherEff;
    private Image _otherIcon;
    private LocalizeTextMeshProUGUI _otherText;
    private LocalizeTextMeshProUGUI _roundText;
  
    public override void PrivateAwake()
    {
        Instance = this;
        _iconFinishList =  new List<Transform>();
        _aniList =  new List<Animator>();
        _lockItems = new List<Transform>();

        var config = ButterflyWorkShopModel.Instance.ButterflyWorkShopConfig;
        for (int j = 0; j < config.RewardId.Count; j++)
        {
            _iconFinishList.Add(transform.Find("Root/RewardGroup/Reward/Content/"+(1)+"/"+(j+1)+"/Finish")); 
            _aniList.Add(transform.Find("Root/RewardGroup/Reward/Content/"+(1)+"/"+(j+1)).GetComponent<Animator>()); 
            _lockItems.Add(transform.Find("Root/RewardGroup/Reward/Content/"+(1)+"/"+(j+1)+"/Lock")); 
            InitItem(GetItem("Root/RewardGroup/Reward/Content/"+(1)+"/"+(j+1)+"/Item").transform,config.RewardId[j],config.RewardNum[j]);
            transform.Find("Root/RewardGroup/Reward/Content/1/" + (j + 1) + "/Butterfly/Image").GetComponent<Image>()
                .sprite = GetButterFLySprite(j + 1);
        }
      
        
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

        _otherIcon = GetItem<Image>("Root/OtherReward/Item/Icon");
        _otherEff = GetItem<Transform>("Root/OtherReward/Item/vfx_BG_01");
        _otherText = GetItem<LocalizeTextMeshProUGUI>("Root/OtherReward/Text");
        _roundText = GetItem<LocalizeTextMeshProUGUI>("Root/RewardGroup/Tab/Num");
    
     
        EventDispatcher.Instance.AddEventListener(EventEnum.BUTTERFLY_WORKSHOP_PURCHASE,OnPurchase);
        
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_newItemBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ButterFlyWorkShopNewItem, _newItemBtn.transform as RectTransform, topLayer: topLayer);
        List<Transform> topLayer2 = new List<Transform>();
        topLayer.Add(_playBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ButterFlyWorkShopEnterGame, _playBtn.transform as RectTransform, topLayer: topLayer2);

    }

    public Sprite GetButterFLySprite(int index)
    {
        var config= ButterflyWorkShopModel.Instance.ButterflyWorkShopRewardConfigList.Find(a => a.Reward.Contains(index));
        if (config != null)
           return ResourcesManager.Instance.GetSpriteVariant("ButterflyWorkShopAtlas", "Butterfly" + config.Id);
        
        return ResourcesManager.Instance.GetSpriteVariant("ButterflyWorkShopAtlas", "Butterfly1" );
    }
    private void OnPurchase(BaseEvent obj)
    {
        RefreshNewItemBtnView();
        RefreshPlayBtnView();
    }

    private void OnBtnBuy()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupButterflyWorkShop);
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventButterflyPackage,"1");

    }

    private void OnBtnHelp()
    {
        UIManager.Instance.OpenUI(UINameConst.UIButterflyWorkShopHelp);
    }

    protected override void OnCloseWindow(bool destroy = false)
    {
        base.OnCloseWindow(destroy);
        mergeBoard.StopAllTweenAnim();
        MergeManager.Instance.Refresh((int)MergeBoardEnum.Main);
        ButterflyWorkShopModel.Instance.IsInUseItem = false;
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        if (_newItemBtn.GetComponent<ShieldButtonOnClick>() != null)
        {
            _newItemBtn.GetComponent<ShieldButtonOnClick>().isUse = false;   
        }

        base.OnOpenWindow(objs);
        MergeManager.Instance.Refresh(MergeBoardEnum.ButterflyWorkShop);
        mergeBoard = transform.Find("Root").GetComponentDefault<ButterflyWorkShopMergeBoard>("Board");
        mergeBoard.activeIndex = 1;
        RefreshView();
    
        InvokeRepeating("RefreshTime",0,1);
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ButterFlyWorkShopNewItem, null);   
        ButterflyWorkShopModel.Instance.CheckFinish();
    }

    public void RefreshTime()
    {
        _timeText.SetText(ButterflyWorkShopModel.Instance.GetActivityLeftTimeString());
        _buyTimeText.SetText(ButterflyWorkShopModel.Instance.GetActivityLeftTimeString());
        if(ButterflyWorkShopModel.Instance.GetActivityLeftTime()<=0)
           AnimCloseWindow();
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

    public void FlyOtherReward(int itemID)
    {
        PlayOtherEff();
        FlyGameObjectManager.Instance.FlyObject(itemID, _otherIcon.transform.position, _newItemBtn.transform.position, 1.2f, 2.0f, 1f,
            () =>
            {
                RefreshBtnView();
            });
   
    }

    public void PlayOtherEff()
    {
        _otherEff.gameObject.SetActive(true);
        CommonUtils.DelayedCall(1.5f, () =>
        {
            _otherEff.gameObject.SetActive(false);
        });
    }
    void RefreshPlayBtnView()
    {
        _playBtn.gameObject.SetActive( UnSetItems.Count == 0);
    }
    void RefreshRewardView()
    {
        for (int i = 0; i < _iconFinishList.Count; i++)
        {
            _iconFinishList[i].gameObject.SetActive(ButterflyWorkShopModel.Instance.IsClaimed(i+1));
            _lockItems[i].gameObject.SetActive(!ButterflyWorkShopModel.Instance.IsClaimed(i+1));
            if (!ButterflyWorkShopModel.Instance.IsClaimed(i + 1))
                _aniList[i].Play("normal");
        }
        _roundText.SetText((ButterflyWorkShopModel.Instance.StorageButterflyWorkShop.Level+1).ToString());
        var config=ButterflyWorkShopModel.Instance.GetStageRewardConfig();
        _otherIcon.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(GameConfigManager.Instance.GetItemConfig(config.RewardId[0]).image);
        _otherText.SetText(ButterflyWorkShopModel.Instance.StorageButterflyWorkShop.StageStore+"/"+config.Score);
    }

    public Task GrowProgressView(int mergeId, int index, int targetIndex)
    {
        var task = new TaskCompletionSource<bool>();
        var src = mergeBoard.GetGridPosition(index);
        Vector3 targetPos = _iconFinishList[targetIndex - 1].position;
     
    ButterflyWorkShopModel.FlyButterFly(targetIndex, src, targetPos, 1.2f, false, 
            () =>
            {
                AudioManager.Instance.PlaySound(158);
                _otherText.transform.DOScale(1.3f, 0.3f).onComplete += () =>
                {
                    _otherText.transform.localScale=Vector3.one;
                };
                if (ButterflyWorkShopModel.Instance.IsClaimed(targetIndex))
                {
                    ButterflyWorkShopModel.FlyButterFly(targetIndex, targetPos, new Vector3(5, 0, 0), 1.2f, false, 
                        () =>
                        {
                            task.SetResult(true);
                        });
                }
                else
                {
                    StartCoroutine( CommonUtils.PlayAnimation(_aniList[targetIndex - 1], "disappear", "", () =>
                    {
                        task.SetResult(true);
                    })); 
                }
                
                


            });
    
        return task.Task;
    }  
    public void RefreshProgress()
    {
        RefreshRewardView();
        
    }
    private void InitItem(Transform item,int itemID,int ItemCount)
    {
        if (UserData.Instance.IsResource(itemID))
        {
            item.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(itemID,UserData.ResourceSubType.Reward);
        }
        else
        {
            var itemConfig = GameConfigManager.Instance.GetItemConfig(itemID);
            if (itemConfig != null)
            {
                item.Find("Icon").GetComponent<Image>().sprite =
                    MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
            }
            else
            {
                Debug.LogError("Get MergeItemConfig---null " + itemID);
            }
        }

        item.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(ItemCount.ToString());
        item.gameObject.SetActive(true);
        bool isRes = UserData.Instance.IsResource(itemID);
        var infoBtn = item.transform.Find("TipsBtn");
        if (infoBtn != null)
        {
            infoBtn.gameObject.SetActive(!isRes);
            if (!isRes)
            {
                var itemButton = CommonUtils.GetComponent<Button>(infoBtn.gameObject);
                itemButton.onClick.AddListener(() =>
                {
                    MergeInfoView.Instance.OpenMergeInfo(itemID, null,_isShowProbability:true);
                    UIPopupMergeInformationController controller =
                        UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMergeInformation) as UIPopupMergeInformationController;
                    if (controller != null)
                        controller.SetResourcesActive(false);
                });
            }
        }
    }

    public void RefreshBtnView()
    {
        RefreshPlayBtnView();
        RefreshNewItemBtnView();
    }
    public void RefreshView()
    {
        RefreshBtnView();
        RefreshRewardView();
    }

    private ulong LastClickNewItemBtnTime;
    void OnNewItemBtn()
    {
        LastClickNewItemBtnTime = APIManager.Instance.GetServerTime();
 
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ButterFlyWorkShopNewItem);
        if ((UnSetItems.Count) == 0)
            return;
        int emptyIndex = MergeManager.Instance.FindEmptyGrid(mergeBoard.activeIndex,MergeBoardEnum.ButterflyWorkShop);
        if (emptyIndex == -1)
        {
            MergePromptManager.Instance.ShowTextPrompt(_newItemBtn.transform.position, 1.5f);
            return;
        }
        var newItemId = UnSetItems[0];
        UnSetItems.RemoveAt(0);
        ButterflyWorkShopModel.Instance.UnSetItemsCount--;
        RefreshBtnView();
        var storageMergeItem = MergeManager.Instance.GetEmptyItem();
        storageMergeItem.Id = newItemId;
        storageMergeItem.State = 1;
        MergeManager.Instance.SetNewBoardItem(emptyIndex, storageMergeItem.Id, 1, RefreshItemSource.rewards, MergeBoardEnum.ButterflyWorkShop,-1, false);
        AudioManager.Instance.PlaySound(13);
        
        // TableMergeItem mergeItem = MergeResourceManager.Instance.ResourcesTableMerge;
        FlyGameObjectManager.Instance.FlyObject(storageMergeItem.Id, _newItemBtn.transform.position,
            mergeBoard.IndexToPosition(emptyIndex), 5f,
            () =>
            {
                ShakeManager.Instance.ShakeLight();
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardEnum.ButterflyWorkShop,emptyIndex, -1,
                    RefreshItemSource.rewards, storageMergeItem.Id);
                ButterflyWorkShopGuideLogic.Instance.GuideLogic();
                // if (mergeItem != null)
                //     MergeResourceManager.Instance.GetMergeResource(mergeItem, true);
            });
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GetReward, storageMergeItem.Id.ToString());
        // MergeResourceManager.Instance.CancelMergeResource(MergeResourceManager.MergeSourcesType.Reward);
    }
    void OnPlayBtn()
    {
        if (ButterflyWorkShopModel.Instance.IsInUseItem)
            return;
        if ((APIManager.Instance.GetServerTime() - LastClickNewItemBtnTime) < 0.5f * XUtility.Second)
        {
            return;
        }
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ButterFlyWorkShopEnterGame);
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
        if (ButterflyWorkShopModel.Instance.IsInUseItem)
            return;
        AnimCloseWindow();
    }
    
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BUTTERFLY_WORKSHOP_PURCHASE,OnPurchase);

    }

    public class TabItem
    {
        public Transform Current;
        public Transform Finish;
        public Transform Normal;

        public TabItem(Transform _current,Transform _finish ,Transform _normal)
        {
            Current = _current;
            Finish = _finish;
            Normal = _normal;
        }
    }
}