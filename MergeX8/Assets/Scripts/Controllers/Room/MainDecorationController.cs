using System;
using System.Collections;
using System.Collections.Generic;
using Deco.Item;
using Deco.Node;
using Deco.World;
using Decoration;
using Decoration.Bubble;
using Decoration.DynamicMap;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using EpForceDirectedGraph.cs;
using Farm.Model;
using Framework;
using Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class MainDecorationController : SuperMainController
{
    private GameObject _goSelectUI;
    private DecoChoseNodeView _choseNodeView;
    private GameObject _buyNodeGroup;
    private GameObject _lockNodeGroup;
    private bool _fromLongPress;

    public static MainDecorationController mainController = null;

    private Button fullClose;

    private DecoBuyNodeView _decoBuyNodeView;
    public DecoBuyNodeView DecoBuyNodeView=>_decoBuyNodeView;
    private DecoLockNodeView _decoLockNodeView;
    private Decoration.LongPressArrowComponent _longPressArrowComponent;
    
    public DecoUIStatus Status { get; set; }
    
    public void Awake()
    {
        mainController = this;

        _buyNodeGroup=GetItem("RenovationTask");
        _decoBuyNodeView = _buyNodeGroup.AddComponent<DecoBuyNodeView>();
        
        _lockNodeGroup = GetItem("RenovationTaskLock");
        _decoLockNodeView = _lockNodeGroup.AddComponent<DecoLockNodeView>();
        
        _goSelectUI = GetItem("SelectUI");
        _choseNodeView = _goSelectUI.AddComponent<DecoChoseNodeView>();

        fullClose = GetItem<Button>("FullScreenClose");
        fullClose.onClick.AddListener(() =>
        {
            if (GuideSubSystem.Instance.IsShowingGuide())
                return;

            if (GuideSubSystem.Instance.ShieldDecoClose())
                return;

            Status = DecoUIStatus.Normal;
            OnHideNodeBuy();
            // GameBIManager.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFtue14);
        });
        fullClose.gameObject.SetActive(false);
        
        EventDispatcher.Instance.AddEventListener(EventEnum.SELECT_NODE, OnSelectNode);
        EventDispatcher.Instance.AddEventListener(EventEnum.NODE_PREVIEW, OnNodePreView);
        EventDispatcher.Instance.AddEventListener(EventEnum.HIDE_NODE_BUY, OnHideNodeBuy);
        EventDispatcher.Instance.AddEventListener(EventEnum.FocusOnSuggestNode, OnFocusOnSuggestNode);
        EventDispatcher.Instance.AddEventListener(EventEnum.SHOW_OR_HIDE_LONG_PRESS_ARROW, OnLongPressArrowEvent);
        EventDispatcher.Instance.AddEventListener(EventEnum.SHOW_NODE_BUY, OnShowNodeBuy);

        Status = DecoUIStatus.Normal;
    }

    protected void Start()
    {
        
        AnimControlManager.Instance.InitAnimControl(AnimKey.Der_BuyNode, _buyNodeGroup, false, 0.35f);
        AnimControlManager.Instance.InitAnimControl(AnimKey.Der_Lockode, _lockNodeGroup, false, 0.35f);
        AnimControlManager.Instance.InitAnimControl(AnimKey.Der_Select, _goSelectUI);
        
        AnimControlManager.Instance.AnimShow(AnimKey.Der_Select, false, true);
        AnimControlManager.Instance.AnimShow(AnimKey.Der_Lockode, false, true);

        AnimShowUIs(false, true);

        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, true);
    }

    public void AnimShowUIs(bool isShow, bool isImmediately = false, Action endCall = null)
    {
        AnimControlManager.Instance.AnimShow(AnimKey.Der_Select, isShow, isImmediately);
        AnimControlManager.Instance.AnimShow(AnimKey.Der_Lockode, isShow, isImmediately);
        AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, isShow, isImmediately, endCall);
    }

    private void OnHideNodeBuy(BaseEvent e=null)
    {
        DecoManager.Instance.OnNodeEndPreView();
        PlayerManager.Instance.UpdatePlayersState();
        fullClose.gameObject.SetActive(false);
        AnimShowUIs(false);
        int nodeId = -1;

        DecoNode node = DecoManager.Instance.CurrentWorld.SelectedNode;
        if (e != null && e.datas != null && e.datas.Length > 0)
        {
            nodeId = (int)e.datas[0];

            node = DecoManager.Instance.FindNode(nodeId);
        }
        if (node != null && node._data._config.costId == (int)UserData.ResourceId.RareDecoCoin)
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.EventCloseBuyItemPopup);
        }

        bool isMainUIShow = UIHomeMainController.mainController.IsMainUIShow();
        if(!isMainUIShow)
            UIRoot.Instance.EnableEventSystem = false;
        
        UIHomeMainController.ShowUI(true, false, () =>
        {
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, true, true);

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
                {
                    BackHomeControl.CheckMainGuide(false, false, nodeId);
                }));
                
                StartCoroutine(CommonUtils.DelayWork(0.6f, () =>
                {
                    if(!isMainUIShow)
                        UIRoot.Instance.EnableEventSystem = true;  
                }));
            }
            else
            {
                StartCoroutine(CommonUtils.DelayWork(0.1f, () =>
                {
                    UIRoot.Instance.EnableEventSystem = true;  
                }));
            }
        });
    }

    public void ResetUI()
    {
        AnimShowUIs(false);
    }

    private void OnDestroy()
    {       
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NODE_PREVIEW, OnNodePreView);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.HIDE_NODE_BUY, OnHideNodeBuy);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.SELECT_NODE, OnSelectNode);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.FocusOnSuggestNode, OnFocusOnSuggestNode);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.SHOW_OR_HIDE_LONG_PRESS_ARROW, OnLongPressArrowEvent);
        EventDispatcher.Instance.RemoveEventListener(EventEnum.SHOW_NODE_BUY, OnShowNodeBuy);
    }

    public override void Show()
    {
        if (_choseNodeView == null)
            return;
        
       _choseNodeView.gameObject.SetActive(true);
    }

    public override void Hide()
    {
        if (_choseNodeView == null)
            return;
        
        _choseNodeView.gameObject.SetActive(false);
    }

    public void UpdateKeyText(int subNum, float time)
    {
        int newValue = 0;
        newValue = UserData.Instance.GetRes(UserData.ResourceId.Coin);

        int oldValue = newValue - subNum;
        oldValue = Math.Max(oldValue, 0);
        newValue = Math.Max(newValue, 0);

        DOTween.To(() => oldValue, x => oldValue = x, newValue, time).OnUpdate(() =>
        {
            //keyNum.SetText(oldValue.ToString());
        });
    }

    private void OnShowNodeBuy(BaseEvent baseEvent)
    {
        if(DecoManager.Instance.CurrentWorld.SelectedNode == null)
            return;
        
        fullClose.gameObject.SetActive(true);
        
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Group, false);
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, false);


        if (DecoManager.Instance.CurrentWorld.SelectedNode._data._config.costId == (int)UserData.ResourceId.RareDecoCoin)
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.EventOpenBuyItemPopup);
        }
        
        if (DecoManager.Instance.CurrentWorld.SelectedNode._data._config.costId == (int)UserData.ResourceId.Mermaid)
        {
            AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, false);
            fullClose.gameObject.SetActive(false);
            Hide();
            UIManager.Instance.OpenUI(UINameConst.MermaidMapBuild, DecoManager.Instance.CurrentWorld.SelectedNode);
        }
        else
        {
            if (!DecoManager.Instance.IsAreaUnLock(DecoManager.Instance.CurrentWorld.SelectedNode))
            {
                AnimControlManager.Instance.AnimShow(AnimKey.Der_Lockode, true);
                Status = DecoUIStatus.Decoration;
                _decoLockNodeView.SetNodeData(DecoManager.Instance.CurrentWorld.SelectedNode, DecoManager.Instance.CurrentWorld.SelectedNode._stage.Area.Id);
            
                NodeBubbleManager.Instance.SetBubbleActive(NodeBubbleManager.BubbleType.Lock, DecoManager.Instance.CurrentWorld.SelectedNode);
            }
            else
            {
                AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, true);
                Status = DecoUIStatus.Decoration;
                Show();
                _decoBuyNodeView.SetNodeData(DecoManager.Instance.CurrentWorld.SelectedNode, DecoManager.Instance.CurrentWorld.SelectedNode._stage.Area.Id);
            }
        }

        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
    }
    
    private void OnSelectNode(BaseEvent baseEvent)
    {
        if (baseEvent == null || baseEvent.datas == null || baseEvent.datas.Length < 2)
            return;
        var itemId = (int)baseEvent.datas[0];
        var fromFirstBuy = (bool)baseEvent.datas[1];

        var world = DecoManager.Instance.CurrentWorld;
        if (world.SelectedNode == null)
            return;

        Status = DecoUIStatus.Decoration;
        
        fullClose.gameObject.SetActive(false);
        AudioManager.Instance.PlaySound(200);
        
        UIHomeMainController.HideUI(true);
        Show();
        
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
        
        UIRoot.Instance.EnableEventSystem = false;
        AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, false);
        AnimControlManager.Instance.AnimShow(AnimKey.Farm_Buy, false);
        AnimControlManager.Instance.AnimShow(AnimKey.Der_Select, true, false, () =>
        {
            UIRoot.Instance.EnableEventSystem = true;
        });
        
        //建筑选择UI
        var defaultItemId = world.SelectedNode.DefaultItem?.Id;
        var currentItemId = 0;
        if (world.SelectedNode.CurrentItem != null) 
            currentItemId = world.SelectedNode.CurrentItem.Config.id;
        
        List<Deco.Item.DecoItem> _items = new List<Deco.Item.DecoItem>();
        foreach (var item in world.SelectedNode.ItemDic.Values)
        {
            if (item.Id == defaultItemId) 
                continue;
            _items.Add(item);
        }

        if (world.SelectedNode.Stage.Area.Config.showArea > 0)
        {
            var areas = DecoExtendAreaManager.Instance.GetExtendAreas(world.SelectedNode.Stage.Area.Config.showArea);
            _choseNodeView.SetData(world.SelectedNode.Stage.Area, areas);
        }
        else
        {
            _choseNodeView.SetData(world.SelectedNode, _items, fromFirstBuy);
        }
    }

    private void OnNodePreView(BaseEvent e)
    {
        List<int> itemIds = (List<int>)e.datas[0];
        if(itemIds == null || itemIds.Count == 0)
            return;
        Action callback = null;
        if (e.datas.Length > 1)
        {
            callback = e.datas[1] as Action;
        }
        DecoItem item = null;
        foreach (var itemId in itemIds)
        {
            if (DecoWorld.ItemLib.ContainsKey(itemId))
            {
                item = DecoWorld.ItemLib[itemId];
                break;
            }
        }
        if(item == null)
            return;
        
        UIRoot.Instance.EnableEventSystem = false;
        DecoManager.Instance.CurrentWorld.LookAtSuggestNodeBySpeed(item.Node, true,
            30f/Decoration.DecorationConfigManager.Instance.GetGlobalConfigNumber(GlobalNumberConfigKey.deco_OriginCameraSize), 10,
            () =>
            {
                if(DecoExtendAreaManager.Instance.ReceiveExtendArea(item.Node))
                    DynamicMapManager.Instance.ForceLoadCurrentChunk();
                DecoManager.Instance.OnNodePreView(itemIds);
                UIRoot.Instance.EnableEventSystem = true;
                
                if (item.Node.Config.costId == (int)UserData.ResourceId.Mermaid)
                    UIManager.Instance.OpenUI(UINameConst.MermaidMapPreview);
                else if (item.Node.Config.costId == (int) UserData.ResourceId.Theme)
                    ThemeDecorationModel.OpenMapPreviewUI();
                else
                    UIManager.Instance.OpenUI(UINameConst.UIDecorationPreview,callback);
            });

        _choseNodeView.PreView();
        UIHomeMainController.HideUI(true);

        if (item.Node.Config.costId == (int)UserData.ResourceId.Mermaid || item.Node.Config.costId == (int)UserData.ResourceId.Theme)
        {
            Hide();
        }
        else
        {
            // Show();
            Hide();
        }
        
        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SHOW_BUILD_BUBBLE, false);
        
        AnimControlManager.Instance.AnimShow(AnimKey.Der_BuyNode, false);
        AnimControlManager.Instance.AnimShow(AnimKey.Der_Select, true, false, () =>
        {
        });
    }
    
    private void OnFocusOnSuggestNode(BaseEvent e)
    {
        if(FarmModel.Instance.IsFarmModel())
            return;
        
        DecoNode node = null;
        if (e != null && e.datas != null && e.datas.Length > 0)
            node = (DecoNode)e.datas[0];
        
        DecoManager.Instance.CurrentWorld.LookAtSuggestNodeCurrentCameraSize(true, node:node);
    }

    private void OnLongPressArrowEvent(BaseEvent baseEvent)
    {
        if (Status == DecoUIStatus.Decoration)
            return;
        
        if (baseEvent == null || baseEvent.datas == null || baseEvent.datas.Length != 2)
            return;
        var isShow = (bool)baseEvent.datas[0];
        var PointPos = (Vector2)baseEvent.datas[1];

        if (isShow)
        {
            if (_longPressArrowComponent == null)
            {
                var arrow_prefab = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Home/LongPressArrow");
                var arrow = GameObject.Instantiate<GameObject>(arrow_prefab);
                CommonUtils.AddChildTo(UIRoot.Instance.mRoot.transform, arrow.transform);
                arrow.transform.SetAsLastSibling();
                _longPressArrowComponent = CommonUtils.GetOrCreateComponent<LongPressArrowComponent>(arrow);
            }

            _longPressArrowComponent.Show(PointPos);
            //GuideSubSystem.Instance.FinishCurrent(GuideTargetType.NodeLongPress);

            DragonPlus.AudioManager.Instance.PlaySound("sfx_room_item_change");
        }
        else
        {
            if (_longPressArrowComponent != null)
                _longPressArrowComponent.Hide();
        }
    }
}