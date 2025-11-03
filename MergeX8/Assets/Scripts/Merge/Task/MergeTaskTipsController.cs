using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ABTest;
using Activity.BalloonRacing.Dynamic;
using Activity.CrazeOrder.Model;
using Activity.GardenTreasure.View;
using Activity.JumpGrid;
using Activity.JumpGrid.Controller;
using Activity.JungleAdventure.Controller;
using Activity.LimitTimeOrder;
using Activity.LuckyGoldenEgg;
using Activity.LuckyGoldenEgg.Controller;
using Activity.Matreshkas.View;
using Activity.SaveTheWhales;
using Activity.TimeOrder;
using Activity.TotalRecharge;
using Activity.TreasureHuntModel;
using Activity.Turntable.Model;
using ActivityLocal.DecoBuildReward;
using ActivityLocal.TipReward.Module;
using Deco.World;
using Decoration;
using DG.Tweening;
using Difference;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Dynamic;
using Farm.Model;
using Framework;
using Gameplay;
using Gameplay.UI.EnergyTorrent;
using Gameplay.UI.Store.Vip.Model;
using Manager;
using Merge.Order;
using TotalRecharge_New;
using UnityEngine;
using UnityEngine.UI;
using Type = System.Type;

public partial class MergeTaskTipsController : MonoBehaviour
{
    private Transform content;
    private GameObject _taskItem;
    private GameObject easterItem;
    
    private List<MergeTaskTipsItem> m_itemList = new List<MergeTaskTipsItem>();
    public RectTransform contentRect = null;
    public static MergeTaskTipsController Instance;
    private ScrollRect _scrollView;
    private Queue<MergeTaskTipsItem> freeItemPools = new Queue<MergeTaskTipsItem>();

    private List<MergeTaskTipsItem> completeList = new List<MergeTaskTipsItem>();
    private List<MergeTaskTipsItem> completeSubList = new List<MergeTaskTipsItem>();
    private List<MergeTaskTipsItem> noCompleteList = new List<MergeTaskTipsItem>();
    private List<MergeTaskTipsItem> firstList = new List<MergeTaskTipsItem>();
    private List<MergeTaskTipsItem> firstCompleteList = new List<MergeTaskTipsItem>();
    
    List<MergeTaskTipsItem> normalTask = new List<MergeTaskTipsItem>();
    List<MergeTaskTipsItem> specialTask = new List<MergeTaskTipsItem>();
    
    private int rectWidth = 768;
    private int cellHalfWidth = 150;

    private Dictionary<int, int> completeTaskIds = new Dictionary<int, int>();

    private GameObject _taskComingSoon;
    private LocalizeTextMeshProUGUI _taskComingText;

    private bool isInit = false;
    private int[] _mergeIds = new int[2]{-1,-1};


    public Transform _mergeRecoverCoinStar => _mergeRecoverCoin.GetStarNode();
    public MergeTaskUnlockTipController _mergeTaskUnlock;

    private float _itemsScale = 1;

    public bool _isMoveing = false;

    private int _siblingIndex = 0;

    private Button _energyTorrentBtn;
    private GameObject _m1;
    private GameObject _m2;
    private GameObject _m4;
    private GameObject _m8;
    private LocalizeTextMeshProUGUI _energyTorrentTimeText;
    private LocalizeTextMeshProUGUI _noCdTimeText;
    private void Awake()
    {
        Instance = this;
        content = this.transform.Find("Viewport/Content");
        contentRect = this.content.GetComponent<RectTransform>();
        _siblingIndex = 30;

        _scrollView = transform.GetComponent<ScrollRect>();
        
        _taskItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergeTaskItem");
        easterItem = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergeEaster");

        var mergeTaskUnlockAsset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/MergeTaskUnlock");
        _mergeTaskUnlock = Instantiate(mergeTaskUnlockAsset, content).AddComponent<MergeTaskUnlockTipController>();
        _mergeTaskUnlock.gameObject.SetActive(false);
        
        _taskComingSoon = content.Find("MergeHeroine").gameObject;
        _taskComingText = _taskComingSoon.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        _taskComingSoon.gameObject.SetActive(false);
        
        // _sizeDelta = ((RectTransform)rewardItem.transform).sizeDelta;
        _energyTorrentBtn = transform.Find("EnergyTorrentBtn").GetComponent<Button>();
        _m1 = transform.Find("EnergyTorrentBtn/1").gameObject;
        _m2 = transform.Find("EnergyTorrentBtn/2").gameObject;
        _m4 = transform.Find("EnergyTorrentBtn/4").gameObject;
        _m8 = transform.Find("EnergyTorrentBtn/8").gameObject;
        _energyTorrentTimeText = transform.Find("EnergyTorrentBtn/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _energyTorrentBtn.onClick.AddListener(OnBtnEnergyTorrent);
        
        List<Transform> topLayerTorrent = new List<Transform>();
        topLayerTorrent.Add(_energyTorrentBtn.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.EnergyTorrent1, _energyTorrentBtn.transform as RectTransform, topLayer:topLayerTorrent);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.EnergyTorrent4, _energyTorrentBtn.transform as RectTransform, topLayer:topLayerTorrent);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.EnergyTorrent5, _energyTorrentBtn.transform as RectTransform, topLayer:topLayerTorrent);

        _noCdTimeText = transform.Find("NoCD/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        _noCdTimeText.transform.parent.gameObject.SetActive(false);
        InitFixEntry();
        SetTaskGuideArrowActive(false);
    }


    void Start()
    {
        InitTimeOrder();
        InitLimitTimeOrder();
        InitMergeTask();
        isInit = true;
        InvokeRepeating("RefreshOrderCountDown", 0, 1f);
        UpdateDailyTaskStatus();
        
        InitStatus();
        
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardRefresh);
        EventDispatcher.Instance.AddEventListener(MergeEvent.DEBUG_TASK_REFRESH, DebugTaskRefresh);
        EventDispatcher.Instance.AddEventListener(MergeEvent.LEVELUP_TASK_REFRESH, TaskRefresh);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_REWARD_REFRESH, RefreshRewards);
        EventDispatcher.Instance.AddEventListener(EventEnum.GET_DECORATION_REWARD, RefreshDecoRewardButton);
        EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_TASK_COMPLETE, BattlePassTaskComplete);
        EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_TASK_COMPLETE, BattlePass2TaskComplete);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND, RefreshEnergyTorrendState);
        EventDispatcher.Instance.AddEventListener(EventEnum.ORDER_REMOVE_REFRESH, OrderRemoveRefresh);

        RegisterSibling();
        UpdateTaskItemsSibling();
        
        StartCoroutine(CommonUtils.DelayWork(0.05f, () =>
        {
            UpdateSibling();
        }));
    }

    private void UpdateStatus()
    {
        int m = EnergyTorrentModel.Instance.GetMultiply();
        if (m == 1)
        {
            _m1.gameObject.SetActive(true);
            _m2.gameObject.SetActive(false);
            _m4.gameObject.SetActive(false);
            _m8.gameObject.SetActive(false);
        }else if (m == 2)
        {
            _m1.gameObject.SetActive(false);
            _m2.gameObject.SetActive(true);
            _m4.gameObject.SetActive(false);
            _m8.gameObject.SetActive(false);
        }
        else if (m == 4)
        {
            _m1.gameObject.SetActive(false);
            _m2.gameObject.SetActive(false);
            _m4.gameObject.SetActive(true);
            _m8.gameObject.SetActive(false);
        }
        else
        {
            _m1.gameObject.SetActive(false);
            _m2.gameObject.SetActive(false);
            _m4.gameObject.SetActive(false);
            _m8.gameObject.SetActive(true);
        }
    }
    
    private void OnEnable()
    {
        InitBattlePass();
        _energyTorrentBtn.gameObject.SetActive(EnergyTorrentModel.Instance.IsUnlock());
        UpdateStatus();
        if (!isInit)
            return;

        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardRefresh);
        EventDispatcher.Instance.AddEventListener(MergeEvent.DEBUG_TASK_REFRESH, DebugTaskRefresh);
        EventDispatcher.Instance.AddEventListener(MergeEvent.LEVELUP_TASK_REFRESH, TaskRefresh);    
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_REWARD_REFRESH, RefreshRewards);  
        EventDispatcher.Instance.AddEventListener(EventEnum.GET_DECORATION_REWARD, RefreshDecoRewardButton);  
        EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_TASK_COMPLETE, BattlePassTaskComplete);
        EventDispatcher.Instance.AddEventListener(EventEnum.BATTLE_PASS_2_TASK_COMPLETE, BattlePass2TaskComplete);
        EventDispatcher.Instance.AddEventListener(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND, RefreshEnergyTorrendState);
        EventDispatcher.Instance.AddEventListener(EventEnum.ORDER_REMOVE_REFRESH, OrderRemoveRefresh);

        RefreshTaskGuideArrow();
        UpdateDailyTaskStatus();
        InitStatus();
        UpdateTaskItemsSibling();
        
        InvokeRepeating("RefreshOrderCountDown", 0, 1f);

        MainOrderManager.Instance.CurMergeTask = null;
        completeList.Clear();
        completeSubList.Clear();
        noCompleteList.Clear();
        completeTaskIds.Clear();
        firstCompleteList.Clear();
        

        if (m_itemList != null && m_itemList.Count > 0)
        {
            foreach (var kv in m_itemList)
            {
                kv.Complete();
                kv.transform.gameObject.SetActive(false);

                EnqueueItem(kv);
            }
        }

        m_itemList.Clear();
        InitMergeTask();
        _MergeMermaid.Init();
        UpdateSibling();
    }

    private void OnDisable()
    {
        CancelInvoke("RefreshOrderCountDown");
        
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardRefresh);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.DEBUG_TASK_REFRESH, DebugTaskRefresh);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.LEVELUP_TASK_REFRESH, TaskRefresh);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_REWARD_REFRESH, RefreshRewards);   
        EventDispatcher.Instance.RemoveEventListener(EventEnum.GET_DECORATION_REWARD, RefreshDecoRewardButton);  
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_TASK_COMPLETE, BattlePassTaskComplete);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.BATTLE_PASS_2_TASK_COMPLETE, BattlePass2TaskComplete);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BORAD_REFRESH_ENERGY_TORREND, RefreshEnergyTorrendState);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.ORDER_REMOVE_REFRESH, OrderRemoveRefresh);

    }

    private void InitMergeTask()
    {
        InitTaskUnlockTip();
        InitComingSoon();

        for (int i = 0; i < MainOrderManager.Instance.CurTaskList.Count; i++)
        {
            var orderItem = MainOrderManager.Instance.CurTaskList[i];
            if (orderItem.Type == (int)MainOrderType.Time)
            {
                if(!TimeOrderModel.Instance.IsOpened())
                    continue;
            }
            
            if (orderItem.Type == (int)MainOrderType.Limit)
            {
                if(!LimitTimeOrderModel.Instance.IsOpened())
                    continue;
            }
            
            if (orderItem.Type == (int)MainOrderType.Craze)
            {
                if(!CrazeOrderModel.Instance.IsOpened())
                    continue;
            }
            InitTaskItem(orderItem).CanTouchComplete = true;
        }

        MergeBoardRefresh(null);

        RemoveAllCompleteTaskId();
        foreach (var kv in completeList)
        {
            AddCompleteTaskId(kv.TableTaskId());
        }

        if (m_itemList != null && m_itemList.Count > 0)
            m_itemList[0].CheckTaskGuide();

        GuideTipsLogic();

        m_itemList.ForEach(a =>
        {
            if(MainOrderManager.Instance.IsSpecialTask(a.TableTaskId()) && !a.IsComplteTask)
                a.transform.SetAsLastSibling();
            if(MainOrderManager.Instance.IsDolphinTask(a.TableTaskId()) && !a.IsComplteTask)
                a.transform.SetAsLastSibling();
            if(MainOrderManager.Instance.IsCapybaraTask(a.TableTaskId()) && !a.IsComplteTask)
                a.transform.SetAsLastSibling();
        });
    }

    private void InitComingSoon()
    {
        if (MainOrderManager.Instance.CurTaskList.Count == 0)
        {
            // && TaskModuleManager.Instance.IsCompleteAllTask() 换key
            _taskComingSoon.transform.SetAsLastSibling();
            _taskComingSoon.gameObject.SetActive(true);
        }
        else
            _taskComingSoon.gameObject.SetActive(false);

        RefreshTaskGuideArrow();
    }

    public void InitTaskUnlockTip()
    {
        _mergeTaskUnlock.Refresh();
    }


    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.MERGE_BOARD_REFRESH, MergeBoardRefresh);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.DEBUG_TASK_REFRESH, DebugTaskRefresh);
        EventDispatcher.Instance.RemoveEventListener(MergeEvent.LEVELUP_TASK_REFRESH, TaskRefresh);
    }


    public void RefreshTask(List<StorageTaskItem> newTasks, Action endCall = null)
    {
        if (newTasks == null || newTasks.Count == 0)
            return;
        
        for (int i = 0; i < newTasks.Count; i++)
        {
            int index = i;
            RefreshTask(newTasks[i], () =>
            {
                if (index == newTasks.Count - 1)
                {
                    endCall.Invoke();
                }
            });
        }
    }
    
    private void RefreshTask(StorageTaskItem taskConfig, Action endCall = null)
    {
        if (taskConfig == null)
            return;

        MergeTaskTipsItem tipsItem = InitTaskItem(taskConfig);
        MergeBoardRefresh(null);
        if (tipsItem.IsComplteTask)
            tipsItem.PlayCompleteEf(false);
        StartCoroutine(CommonUtils.PlayAnimation(tipsItem.animator,"Appear","", () =>
        {
            if (tipsItem.IsComplteTask)
                tipsItem.PlayCompleteEf(true);

            tipsItem.CheckTaskGuide();
            tipsItem.CanTouchComplete = true;

            StartCoroutine(CommonUtils.DelayWork(0.2f, () =>
            {
                MergeGuideLogic.Instance.CheckMergeGuide();
            }));

            endCall?.Invoke();
        }));
    }

    public void DeleteTask(MergeTaskTipsItem curTask, List<StorageTaskItem> newTasks)
    {
        if (curTask == null)
            return;

        for (int i = 0; i < m_itemList.Count; i++)
        {
            m_itemList[i].SiblingIndex = i;
        }

        curTask.CanTouchComplete = false;
        curTask.PlayCompleteEf(false);
        StartCoroutine(CommonUtils.PlayAnimation(curTask.animator, "DisAppear", "", () =>
        {
            RemoveCompleteTaskId(curTask.TableTaskId());
            curTask.Complete();
            curTask.transform.gameObject.SetActive(false);

            EnqueueItem(curTask);

            if (m_itemList.Contains(curTask))
                m_itemList.Remove(curTask);
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.TASK_COMPLETE_REFRESH);
            MergeMainController.Instance?.UpdateTaskRedPoint();

            if (newTasks != null && newTasks.Count > 0)
            {
                for (int i = 0; i < newTasks.Count; i++)
                {
                    int index = i;
                    RefreshTask(newTasks[i], () =>
                    {
                        if (index == newTasks.Count - 1)
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                            MergeMainController.Instance?.UpdateTaskRedPoint();
                        }
                    });
                }
            }
        }));
    }
    private void RefreshEnergyTorrendState(BaseEvent obj)
    {
        UpdateStatus();
    }

    private void OrderRemoveRefresh(BaseEvent e)
    {
        int removeOrderId = (int)e.datas[0];
        for (var i = 0; i < m_itemList.Count; i++)
        {
            if(m_itemList[i].StorageTaskItem.OrgId != removeOrderId)
                continue;

            var delOrder = m_itemList[i];
            m_itemList.RemoveAt(i);
            i--;
            DeleteTask(delOrder, null);
        }
    }
    public void CompleteTask(MergeTaskTipsItem curTask, List<StorageTaskItem> newTasks, List<ResData> resDatas)
    {
        if (curTask == null)
            return;

        if (curTask.StorageTaskItem.Type == (int)MainOrderType.Limit || curTask.StorageTaskItem.Type == (int)MainOrderType.Craze)
        {
            StorageTaskItem orderTask = null;
            if (newTasks != null && newTasks.Count > 0)
            {
                List<StorageTaskItem> otherTask = new List<StorageTaskItem>();
                foreach (var storageTaskItem in newTasks)
                {
                    if (storageTaskItem.Type == curTask.StorageTaskItem.Type)
                    {
                        orderTask = storageTaskItem;
                    }
                    else
                    {
                        otherTask.Add(storageTaskItem);
                    }
                }

                if (orderTask != null)
                {
                    if (otherTask.Count > 0)
                    {
                        RefreshTask(otherTask, () =>
                        {
                            EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                            MergeMainController.Instance?.UpdateTaskRedPoint();
                        });
                    }
                    
                    curTask.Init(orderTask);
                    curTask.CanTouchComplete = true;
                
                    UpdateDailyTaskStatus();
            
                    RemoveCompleteTaskId(curTask.TableTaskId());

                    EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.TASK_COMPLETE_REFRESH);
                    MergeMainController.Instance?.UpdateTaskRedPoint();
                    EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                    MergeMainController.Instance?.UpdateTaskRedPoint();

                    return;
                }
            }
        }

        for (int i = 0; i < m_itemList.Count; i++)
        {
            m_itemList[i].SiblingIndex = i;
        }

        curTask.PlayCompleteEf(false);
        StartCoroutine(CommonUtils.PlayAnimation(curTask.animator, "DisAppear", "", () =>
        {
            UIRoot.Instance.EnableEventSystem = true;
            
            UpdateDailyTaskStatus();
            
            RemoveCompleteTaskId(curTask.TableTaskId());
            curTask.Complete();
            curTask.transform.gameObject.SetActive(false);

            EnqueueItem(curTask);

            if (m_itemList.Contains(curTask))
                m_itemList.Remove(curTask);
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.TASK_COMPLETE_REFRESH);
            MergeMainController.Instance?.UpdateTaskRedPoint();

            if (newTasks != null && newTasks.Count > 0)
            {
                RefreshTask(newTasks, () =>
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main);
                    MergeMainController.Instance?.UpdateTaskRedPoint();
                    GuideTipsLogic();

                    InitComingSoon();
                });
            }
            else
            {
                InitComingSoon();
            }

            if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_TaskComplete))
            {
                AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_TaskComplete, b => { });
            }

            MergeGuideLogic.Instance.CheckTriggerProduct();
            MergeGuideLogic.Instance.CheckUnLockModule();

            TipRewardModule.Instance.CanShow(curTask.StorageTaskItem, resDatas);
        }));
    }

    private MergeTaskTipsItem GetFreeItem()
    {
        if (freeItemPools== null || freeItemPools.Count == 0)
            return null;

        return freeItemPools.Dequeue();
    }

    private void EnqueueItem( MergeTaskTipsItem tipsItem)
    {
        if (tipsItem == null)
            return;

        if (tipsItem.StorageTaskItem == null)
            return;

        if(freeItemPools.Contains(tipsItem))
            return;
        
         freeItemPools.Enqueue(tipsItem);
    }

    private MergeTaskTipsItem InitTaskItem(StorageTaskItem storageItem)
    {
        if (storageItem == null)
            return null;


        GameObject taskItem = _taskItem;
        
        MergeTaskTipsItem tipsItem = GetFreeItem();
        
        if (tipsItem == null)
        {
            GameObject item = Instantiate(taskItem, content);
            item.gameObject.SetActive(true);

            tipsItem = item.gameObject.AddComponent<MergeTaskTipsItem>();
            m_itemList.Add(tipsItem);
        }
        else
        {
            tipsItem.gameObject.SetActive(true);
            m_itemList.Add(tipsItem);
        }

        tipsItem.Init(storageItem);
        tipsItem.SiblingIndex = m_itemList.Count - 1;

        UpdateTaskItemsSibling();
        
        return tipsItem;
    }

    private void TaskRefresh(BaseEvent e)
    {
        if(!gameObject.activeSelf)
            return;
        
        InitMergeTask();
    }
    private void DebugTaskRefresh(BaseEvent e)
    {
        MergeBoardRefresh(null);
    }
    private void MergeBoardRefresh(BaseEvent e)
    {
        bool isProduct = false;
        _mergeIds[0] = -1;
        _mergeIds[1] = -1;
        RefreshItemSource itemSource = RefreshItemSource.none;
        if (e != null)
        {
            if (e.datas != null && e.datas.Length > 3)
            {
            
                _mergeIds[0] = (int) e.datas[4];
                itemSource = (RefreshItemSource)e.datas[3];
                isProduct = IsProduct(itemSource);

                if ((RefreshItemSource)e.datas[3] == RefreshItemSource.mergeOk)
                {
                    if(e.datas.Length >= 6)
                        _mergeIds[1] = (int) e.datas[5];
                }
            }
        }

        bool isRefresh = false;
        if (e == null || itemSource == RefreshItemSource.remove)
        {
            foreach (var item in m_itemList)
            {
                bool refreshStatus = item.RefreshView(isInit:e==null);
                if (!refreshStatus)
                    refreshStatus = item.IsComplteSubTask;
                    
                if(!refreshStatus)
                    continue;
                isRefresh = true;
            }
        }
        else
        {
            foreach (var item in m_itemList)
            {
                bool refreshStatus = false;
                for (int i = 0; i < _mergeIds.Length; i++)
                {
                    if(_mergeIds[i] < 0)
                        continue;
                
                    refreshStatus = item.RefreshView(_mergeIds[i]);
                    if (!refreshStatus && item.ContainsItemId(_mergeIds[i]))
                        refreshStatus = item.IsComplteSubTask;
                    
                    if(!refreshStatus)
                        continue;
                
                    isRefresh = true;
                }
            }
        }
        if(!isRefresh)
            return;

        RemoveAllCompleteTaskId();
        foreach (var kv in completeList)
        {
            AddCompleteTaskId(kv.TableTaskId());
        }
        foreach (var kv in firstCompleteList)
        {
            AddCompleteTaskId(kv.TableTaskId());
        }
        GuideTipsLogic();
        UpdateTaskItemsSibling();
        
        MergeMainController.Instance.MergeBoard.RefreshTask(new BaseEvent(EventEnum.TASK_REFRESH,MergeBoardEnum.Main));
        
        if (!isProduct)
            return;

        bool isMove = false;
        foreach (var kv in completeList)
        {
            if (IsHaveCompleteTaskId(kv.TableTaskId()))
                continue;
        
            isMove = true;
            break;
        }
        
        foreach (var kv in firstCompleteList)
        {
            if (IsHaveCompleteTaskId(kv.TableTaskId()))
                continue;
        
            isMove = true;
            break;
        }
        
        RemoveAllCompleteTaskId();
        foreach (var kv in completeList)
        {
            AddCompleteTaskId(kv.TableTaskId());
        }
        foreach (var kv in firstCompleteList)
        {
            AddCompleteTaskId(kv.TableTaskId());
        }
        
        if (!isMove)
            return;
        StartCoroutine(DailyMove(0));
    }

    private void UpdateTaskItemsSibling()
    {
        completeList.Clear();
        completeSubList.Clear();
        noCompleteList.Clear();
        firstList.Clear();
        firstCompleteList.Clear();
        
        for (int i = 0; i < m_itemList.Count; i++)
        {
            if (m_itemList[i].StorageTaskItem.Type == (int)MainOrderType.Time)
            {
                firstList.Add(m_itemList[i]);
                if (m_itemList[i].IsComplteTask)
                    firstCompleteList.Add(m_itemList[i]);
                continue;
            }
            if (m_itemList[i].StorageTaskItem.Type == (int)MainOrderType.Limit)
            {
                firstList.Add(m_itemList[i]);
                if (m_itemList[i].IsComplteTask)
                    firstCompleteList.Add(m_itemList[i]);
                continue;
            }
            if (m_itemList[i].StorageTaskItem.Type == (int)MainOrderType.Craze)
            {
                firstList.Add( m_itemList[i]);
                if (m_itemList[i].IsComplteTask)
                    firstCompleteList.Add(m_itemList[i]);
                continue;
            }
            if (m_itemList[i].StorageTaskItem.Type == (int)MainOrderType.KeepPet)
            {
                firstList.Add(m_itemList[i]);
                if (m_itemList[i].IsComplteTask)
                    firstCompleteList.Add(m_itemList[i]);
                continue;
            }
            if (m_itemList[i].StorageTaskItem.Type == (int)MainOrderType.Team)
            {
                firstList.Add(m_itemList[i]);
                if (m_itemList[i].IsComplteTask)
                    firstCompleteList.Add(m_itemList[i]);
                continue;
            }
            
            if (m_itemList[i].IsComplteTask)
            {
                completeList.Add(m_itemList[i]);
                continue;
            }

            if (m_itemList[i].IsComplteSubTask && !m_itemList[i].IsRareDecoCoin())
            {
                completeSubList.Add(m_itemList[i]);
                continue;
            }
            
            noCompleteList.Add(m_itemList[i]);
        }

        firstList.Sort((a, b) => { return b.StorageTaskItem.Type - a.StorageTaskItem.Type;});
        
        normalTask.Clear();
        specialTask.Clear();
        MergeTaskTipsItem sealTak = null;
        MergeTaskTipsItem dolphin = null;
        MergeTaskTipsItem capybara = null;
        for (int i = 0; i < noCompleteList.Count; i++)
        {
            if (MainOrderManager.Instance.IsSealTask(noCompleteList[i].TableTaskId()))
            {
                sealTak = noCompleteList[i];
                continue;
            }
            
            if (MainOrderManager.Instance.IsDolphinTask(noCompleteList[i].TableTaskId()))
            {
                dolphin = noCompleteList[i];
                continue;
            }
            if (MainOrderManager.Instance.IsCapybaraTask(noCompleteList[i].TableTaskId()))
            {
                capybara = noCompleteList[i];
                continue;
            }
            
            if(noCompleteList[i].StorageTaskItem.RewardTypes == null)
                continue;

            bool isSpecial = false;
            foreach (var rewardType in noCompleteList[i].StorageTaskItem.RewardTypes)
            {
                if (rewardType == (int)UserData.ResourceId.RareDecoCoin)
                {
                    specialTask.Add(noCompleteList[i]);
                    isSpecial = true;
                    break;
                }
            }
            
            if(isSpecial)
                continue;
            
            normalTask.Add(noCompleteList[i]);
        }
        
        noCompleteList.Clear();
        if(normalTask.Count > 0)
            noCompleteList.AddRange(normalTask);
        if(specialTask.Count > 0)
            noCompleteList.AddRange(specialTask);
        if(sealTak != null)
            noCompleteList.Add(sealTak);
        if (dolphin != null)
            noCompleteList.Add(dolphin);
        if (capybara != null)
            noCompleteList.Add(capybara);
        
        m_itemList.Clear();
        m_itemList.AddRange(firstList);
        m_itemList.AddRange(completeList);
        m_itemList.AddRange(completeSubList);
        m_itemList.AddRange(noCompleteList);

        UpdateSibling();
    }
    
    private IEnumerator DailyMove(int index)
    {
        yield return new WaitForEndOfFrame();
        
        MoveToIndex(index);
    }
    
    public void MoveToIndex(int index, Action moveEndCall = null)
    {
        if (m_itemList == null || m_itemList.Count <= index)
        {
            moveEndCall();
            return;
        }
        
        RectTransform viewport = _scrollView.viewport;
        RectTransform content = _scrollView.content;
        RectTransform item = (RectTransform)(m_itemList[index].transform);

        Vector3[] itemCorners = new Vector3[4];
        item.GetWorldCorners(itemCorners);

        Vector3[] viewportCorners = new Vector3[4];
        viewport.GetWorldCorners(viewportCorners);


        Vector3 itemMinInContent = content.InverseTransformPoint(itemCorners[0]);
        Vector3 itemMaxInContent = content.InverseTransformPoint(itemCorners[2]);
        Vector3 viewportMinInContent = content.InverseTransformPoint(viewportCorners[0]);
        Vector3 viewportMaxInContent = content.InverseTransformPoint(viewportCorners[2]);

        float itemMinX = itemMinInContent.x;
        float itemMaxX = itemMaxInContent.x;

        float viewportMinX = viewportMinInContent.x;
        float viewportMaxX = viewportMaxInContent.x;

        _isMoveing = true;
        if (itemMinX >= viewportMinX && itemMaxX <= viewportMaxX)
        {
            _isMoveing = false;
            moveEndCall?.Invoke();
            return;
        }

        float contentWidth = content.rect.width;
        float viewportWidth = viewport.rect.width;
        float targetPosX = content.anchoredPosition.x;

        if (itemMinX < viewportMinX)
        {
            float distanceToMove = viewportMinX - itemMinX+cellHalfWidth;
            targetPosX += distanceToMove;
        }
        else if (itemMaxX > viewportMaxX)
        {
            float distanceToMove = itemMaxX - viewportMaxX+cellHalfWidth;
            targetPosX -= distanceToMove;
        }



        targetPosX = Mathf.Clamp(targetPosX, viewportWidth - contentWidth, 0f);

        contentRect.DOKill();
        content.DOAnchorPosX(targetPosX, 0.2f).OnComplete(() =>
        {
            _isMoveing = false;
            moveEndCall?.Invoke();
        });
    }
    
    private void AddCompleteTaskId(int id)
    {
        if (completeTaskIds.ContainsKey(id))
            return;

        completeTaskIds.Add(id, id);
    }

    private void RemoveCompleteTaskId(int id)
    {
        if (!completeTaskIds.ContainsKey(id))
            return;

        completeTaskIds.Remove(id);
    }

    private void RemoveAllCompleteTaskId()
    {
        completeTaskIds.Clear();
    }

    private bool IsHaveCompleteTaskId(int id)
    {
        if (completeTaskIds.ContainsKey(id))
            return true;

        return false;
    }

    private bool IsProduct(RefreshItemSource source)
    {
        if (source == RefreshItemSource.remove)
            return false;

        if (source == RefreshItemSource.notDeal)
            return false;

        return true;
    }

    public void RefreshDebugModule()
    {
        if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
            return;

        m_itemList.ForEach(a => a.RefreshDebugModule());
    }

    public GameObject GetFirstCompleteTaskGuide()
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return null;

        return m_itemList[0].guideObj;
    }

    public bool FirstTaskComplete()
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return false;

        return m_itemList[0].IsComplteTask;
    }

    public string FirstTaskCompleteId()
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return "";

        return m_itemList[0].TableTaskId().ToString();
    }

    public MergeTaskTipsItem GetTaskItemByIndex(int index = 0)
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return null;

        if (index < 0 || index >= m_itemList.Count)
            return null;
        
        return m_itemList[index];
    }

    public MergeTaskTipsItem GetTaskItemBySlot(SlotDefinition slot)
    {
        for (var i = 0; i < m_itemList.Count; i++)
        {
            var item = m_itemList[i];
            if (item.StorageTaskItem.Slot == (int) slot)
            {
                return item;
            }
        }
        return null;
    }
    
    public MergeTaskTipsItem GetTaskItemByType(MainOrderType type)
    {
        for (var i = 0; i < m_itemList.Count; i++)
        {
            var item = m_itemList[i];
            if (item.StorageTaskItem.Type == (int) type)
            {
                return item;
            }
        }
        return null;
    }
    
    public int GetFirstTaskNeedItemId()
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return -1;

        return m_itemList[0].GetFirstNeedItemId();
    }

    public bool IsHaveTask(int id)
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return false;

        return m_itemList.Find(a => a.TableTaskId() == id);
    }

    public MergeTaskTipsItem GetTaskItem(int id)
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return null;

        return m_itemList.Find(a => a.TableTaskId() == id);
    }
    
    private void GuideTipsLogic()
    {
        if (m_itemList == null || m_itemList.Count == 0)
            return;

        if (MainOrderManager.Instance.CompleteTaskNum < 5 || MainOrderManager.Instance.CompleteTaskNum >= 10)
            return;

        m_itemList.ForEach(a => a.SetArrowActive(false));
        if (!m_itemList[0].IsComplteTask || MainOrderManager.Instance.CurMergeTask == m_itemList[0])
            return;

        m_itemList[0].SetArrowActive(true);
    }

    private void RefreshOrderCountDown()
    {
        RefreshDynamicEntry();
        RefreshRecoverCoinStatus();
        RefreshCoinLeaderBoardStatus();
        RefreshButterflyWorkShopStatus();
        RefreshSummerWatermelonStatus();
        RefreshTreasureHuntStatus();
        RefreshEaster();
        RefreshBattlePass();
        RefreshMermaid();
        RefreshTotalRecharge();
        RefreshTotalRechargeNew();
        RefreshNoCdProduct();
        RefreshCommonResourceLeaderBoard();
        InvokeRepeating_BattlePassTask();
        RefreshEnergyTorrent();
        RefreshTimeOrder();
        RefreshLimitTimeOrder();
        
        _mergeHappyGo.transform.gameObject.SetActive(HappyGoModel.Instance.IsCanPlay());
    }

    private void RefreshEnergyTorrent()
    {
        var leftTime = EnergyTorrentModel.Instance.GetLeftTime();
        if (leftTime <= 0)
        {
            if (EnergyTorrentModel.Instance.StorageEnergyTorrent.MaxStartTime > 0)
            {
                EnergyTorrentModel.Instance.StorageEnergyTorrent.MaxStartTime = 0;
                EnergyTorrentModel.Instance.ReSeStateX8();
            }
            else
            {
                if (VipStoreModel.Instance.VipLevel() < 5 && ((EnergyTorrentModel.Instance.StorageEnergyTorrent.Multiply == 8&&!EnergyTorrentModel.Instance.IsUnlock8Multiply())|| 
                                                              ((EnergyTorrentModel.Instance.StorageEnergyTorrent.Multiply == 4&&!EnergyTorrentModel.Instance.IsUnlock4Multiply()))))
                {
                    EnergyTorrentModel.Instance.SetCloseStateX8();
                }
            }
        }
        _energyTorrentTimeText.gameObject.SetActive(leftTime>0);
        _energyTorrentTimeText.SetText(CommonUtils.FormatLongToTimeStr(leftTime));
    }

    public void RefreshRecoverCoinStatus()
    {
        _mergeRecoverCoin.RefreshView();
    }   
    public void RefreshCoinLeaderBoardStatus()
    {
        _mergeCoinLeaderBoard.gameObject.SetActive(CoinLeaderBoardModel.Instance.IsStart());
    }
    
    
    public void RefreshCommonResourceLeaderBoard()
    {
        foreach (var pair in MergeTaskEntrance_CommonResourceLeaderBoard.CreatorDic)
        {
            if (pair.Value.TaskEntrance)
            {
                pair.Value.TaskEntrance.RefreshView();
            }
        }
    }

    public void RefreshButterflyWorkShopStatus()
    {
        _MergeButterflyWorkShop.gameObject.SetActive(ButterflyWorkShopModel.Instance.IsStart);   
    }   
    public void RefreshSummerWatermelonStatus()
    {
        _mergeSummerWatermelon.gameObject.SetActive(SummerWatermelonModel.Instance.IsStart);   
    }
    public void RefreshTreasureHuntStatus()
    {
        _MergeTreasureHunt.gameObject.SetActive(TreasureHuntModel.Instance.IsStart());   
    }
        
    private void RefreshEaster()
    {
        if (EasterModel.Instance.IsOpened() && EasterModel.Instance.StorageEaster.IsShowStartView && !EasterModel.Instance.StorageEaster.IsManualActivity)
        {
            _mergeEaster.gameObject.SetActive(true);
        }
        else
        {
            _mergeEaster.gameObject.SetActive(false);
        }
    }   
    private void RefreshBattlePass()
    {
        if (Activity.BattlePass.BattlePassModel.Instance.IsOpened() && Activity.BattlePass.BattlePassModel.Instance.storageBattlePass.IsShowStart)
        {
            _MergeBattlePass?.gameObject.SetActive(true);
        }
        else
        {
            _MergeBattlePass?.gameObject.SetActive(false);
        }
        
        if (Activity.BattlePass_2.BattlePassModel.Instance.IsOpened() && Activity.BattlePass_2.BattlePassModel.Instance.storageBattlePass.IsShowStart)
        {
            _MergeBattlePass2?.gameObject.SetActive(true);
        }
        else
        {
            _MergeBattlePass2?.gameObject.SetActive(false);
        }
    }
    
    private void RefreshMermaid()
    {
        if (MermaidModel.Instance.IsStart())
        {
            _MergeMermaid.gameObject.SetActive(true);
        }
        else
        {
            _MergeMermaid.gameObject.SetActive(false);
        }
        
        if (!MermaidModel.Instance.IsOpened())
        {
            _MergeMermaid.gameObject.SetActive(false);
        }
    }  
    private void RefreshTotalRecharge()
    {

        _MergeTotalRecharge.gameObject.SetActive((TotalRechargeModel.Instance.IsOpen() &&
                                                  TotalRechargeModel.Instance.IsHaveCanClaim()));
    }
    private void RefreshTotalRechargeNew()
    {

        _MergeTotalRecharge_New.gameObject.SetActive((TotalRechargeModel_New.Instance.IsOpen() &&
                                                  TotalRechargeModel_New.Instance.IsHaveCanClaim()));
        
        _totalRechargeNewDecoReward.gameObject.SetActive(DecoBuildRewardManager.Instance.CanGetReward(TotalRechargeDecoReward.DecoBuildId));
    }

  
    public void RefreshNoCdProduct()
    {
        if (MergeManager.Instance.IsInUnlimitedProduct(MergeBoardEnum.Main))
        {
            _noCdTimeText.transform.parent.gameObject.SetActive(true);
            _noCdTimeText.SetText(MergeManager.Instance.GetUnlimitedProductTimeStr(MergeBoardEnum.Main));
        }
        else
        {
            _noCdTimeText.transform.parent.gameObject.SetActive(false);
        }
    }
    private void UpdateDailyTaskStatus()
    {
        _mergeDailyTaskItem.UpdateDailyTaskStatus();
        _mergeFarmBtn.UpdateDailyTaskStatus();
    }

    private void SetTaskGuideArrowActive(bool isActive)
    {
        _mergeDailyTaskItem?._guideArrow?.gameObject.SetActive(isActive);
    }

    public void RefreshTaskGuideArrow()
    {
        return;
        SetTaskGuideArrowActive(false);

        int level = ExperenceModel.Instance.GetLevel();
        if (level >= 2 && level < 3)
        {
            SetTaskGuideArrowActive(DecoManager.Instance.CanBuyOrGet());
        }
    }

    private void RefreshRewards(BaseEvent e)
    {
        UpdateSibling();
    }

    private void RefreshDecoRewardButton(BaseEvent e)
    {
        UpdateDailyTaskStatus();
    }
    
    private void OnBtnEnergyTorrent()
    {
        if(EnergyTorrentModel.Instance.IsUnlock4Multiply())
        {
            if (EnergyTorrentModel.Instance.IsUnlock8Multiply() && EnergyTorrentModel.Instance.GetMultiply() ==4)
            {
                if (VipStoreModel.Instance.VipLevel() >= 5)
                {
                    EnergyTorrentModel.Instance.SetOpenStateX8();
                    return;
                }
                else if(EnergyTorrentModel.Instance.GetLeftTime() > 0)
                {
                    UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentMainX8);
                    return;
                }
            }
     
            EnergyTorrentModel.Instance.SetMultiply();
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.EnergyTorrent4,null);
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.EnergyTorrent5,null);
          
            string content = EnergyTorrentModel.Instance.IsOpen() ?
                string.Format(LocalizationManager.Instance.GetLocalizedString("ui_energy_frenzy_open_tips"),EnergyTorrentModel.Instance.GetMultiply())
                : "ui_energy_frenzy_close_tips";
            var obj = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Merge/UIEnergyTorrentTips");
            var instantiate=Instantiate(obj, UIRoot.Instance.mRoot.transform);
   
            UIEnergyTorrentTipsController controller = instantiate.AddComponent<UIEnergyTorrentTipsController>();
            controller.PlayAnim(content, () =>
            {
                CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(1, () =>
                {
                    Destroy(controller.gameObject);
                }));
            });
            
        }
        else
        {
            if (EnergyTorrentModel.Instance.StorageEnergyTorrent.IsShowStart)
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentMain);
            }
            else
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyTorrentStart);
            }
        
        }
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.EnergyTorrent1,null);

    }
    
}
