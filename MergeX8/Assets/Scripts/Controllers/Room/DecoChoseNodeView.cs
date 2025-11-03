using System;
using System.Collections;
using System.Collections.Generic;
using ConnectLine;
using Deco.Area;
using Deco.Node;
using Decoration;
using Decoration.Bubble;
using Decoration.DaysManager;
using Decoration.DynamicMap;
using Decoration.WorldFogManager;
using Ditch.Model;
using Dlugin;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonPlus.Config.Ditch;
using DragonPlus.Config.FishEatFish;
using DragonPlus.Config.Makeover;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Filthy.Game;
using Framework;
using Gameplay;
using Gameplay.UI.MiniGame;
using Makeover;
using OnePath;
using Psychology;
using UnityEngine;
using UnityEngine.UI;
using Utils = IFix.Core.Utils;


public class DecoChoseNodeView : MonoBehaviour
{
    private class ChoseNodeItem
    {
        public enum AnimType
        {
            On,
            Off,
        }

        public Image _icon;
        public Animator _animator;
        public GameObject _itemObj;
        public Button _btn;
        public Deco.Item.DecoItem _decoItem;
        public TableAreas _areaConfig;

        public ChoseNodeItem(GameObject itemObj)
        {
            _itemObj = itemObj;

            _icon = _itemObj.transform.Find("Icon").GetComponent<Image>();
            _animator = _itemObj.transform.GetComponent<Animator>();
            _btn = _itemObj.transform.GetComponent<Button>();
        }

        public void UpdateData(Deco.Item.DecoItem decoItem, bool loadSprite)
        {
            if (decoItem.Config.source != 0 && !decoItem.IsOwned)
                _itemObj.gameObject.SetActive(false);

            if (_decoItem == decoItem)
                return;

            _decoItem = decoItem;

            if (loadSprite)
                _icon.sprite = CommonUtils.LoadDecoItemIconSprite(decoItem._node._stage.Area.Id,
                    decoItem._data._config.buildingIcon);
        }

        public void UpdateData(TableAreas config)
        {
            _areaConfig = config;

            _icon.sprite = CommonUtils.LoadAreaIconSprite(config.icon);
        }

        public void PlayAnim(AnimType type)
        {
            string animName = type == AnimType.On ? "FurnitureItem_Choice" : "FurnitureItem_Normal";
            _animator.Play(animName);
        }
    }


    private Button _btnCancel;
    private Button _btnOK;
    private GameObject _cloneItem;
    private GameObject _itemGroup;
    private List<ChoseNodeItem> _nodeItems = new List<ChoseNodeItem>();

    Deco.Node.DecoNode _selectDecNode;
    DecoArea _selectDecArea;
    DecoArea _defaultSelectDecArea;
    Deco.Item.DecoItem _selectDecItem;
    List<Deco.Item.DecoItem> _items;
    private bool _fromFirstBuy;

    private bool _isPreview = false;

    private bool _isDefault = true;

    private void Awake()
    {
        _btnCancel = transform.Find("SelectFurnituresGroup/CloseSelectButton").GetComponent<Button>();
        _btnCancel.onClick.AddListener(OnClickCancel);

        _btnOK = transform.Find("SelectFurnituresGroup/SureSelectButton").GetComponent<Button>();
        _btnOK.onClick.AddListener(OnClickOk);
        _cloneItem = transform.Find("SelectFurnituresGroup/FurnituresScrollRect/FurnituresLayout/FurnitureItem")
            .gameObject;
        _cloneItem.gameObject.SetActive(false);

        _itemGroup = transform.Find("SelectFurnituresGroup/FurnituresScrollRect/FurnituresLayout")
            .gameObject;
        List<Transform> topLayer = new List<Transform>();
        topLayer.Add(_btnOK.transform);
        GuideSubSystem.Instance.RegisterTarget(GuideTargetType.ConfirmNode, _btnOK.transform as RectTransform,
            topLayer: topLayer);


        EventDispatcher.Instance.AddEventListener(EventEnum.NODE_PREVIEW_END, OnNodePreViewEnd);
    }

    public void SetData(DecoArea area, List<TableAreas> areas)
    {
        _defaultSelectDecArea = area;
        _selectDecArea = area;
        EndPreView();

        _nodeItems.ForEach(a =>
        {
            a._btn.onClick.RemoveAllListeners();
            a._itemObj.SetActive(false);
        });

        if (areas != null && areas.Count > _nodeItems.Count)
        {
            int count = areas.Count - _nodeItems.Count;
            for (int i = 0; i < count; i++)
            {
                var decoArea = DecoManager.Instance.FindArea(areas[i].id);
                if (!decoArea.HaveOwnedNode())
                    continue;

                var obj = GameObject.Instantiate(_cloneItem, _itemGroup.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                var nodeItem = new ChoseNodeItem(obj);
                _nodeItems.Add(nodeItem);
                CommonUtils.SetShieldButUnEnable(nodeItem._btn.gameObject);
                nodeItem._btn.onClick.AddListener(() =>
                {
                    _isDefault = false;
                    SelectItem(nodeItem._areaConfig);

                    _nodeItems.ForEach(a =>
                    {
                        a.PlayAnim(a._areaConfig == nodeItem._areaConfig
                            ? ChoseNodeItem.AnimType.On
                            : ChoseNodeItem.AnimType.Off);
                    });
                });
                var arrowPointer = nodeItem._itemObj.transform.Find("Arrow");
                arrowPointer.gameObject.SetActive(false);
                if (!arrowPointer.gameObject.TryGetComponent<KeepScaleAndPosition>(out var scaleKeeper))
                {
                    scaleKeeper = arrowPointer.gameObject.AddComponent<KeepScaleAndPosition>();
                    scaleKeeper.Awake();
                }
            }
        }

        if (areas == null)
            return;

        int activeNum = 0;
        int index = 0;
        for (int i = 0; i < areas.Count; i++)
        {
            var decoArea = DecoManager.Instance.FindArea(areas[i].id);
            if (!decoArea.HaveOwnedNode())
                continue;

            _nodeItems[index]._itemObj.SetActive(true);
            _nodeItems[index].UpdateData(areas[i]);

            _nodeItems[index]._itemObj.transform.localScale = Vector3.one;
            // var arrowPointer = _nodeItems[index]._itemObj.transform.Find("Arrow");
            // arrowPointer.gameObject.SetActive(false);
            // if (!arrowPointer.gameObject.TryGetComponent<KeepScaleAndPosition>(out var scaleKeeper))
            // {
            //     scaleKeeper = arrowPointer.gameObject.AddComponent<KeepScaleAndPosition>();
            // }

            var nodeItem = _nodeItems[index];
            nodeItem._btn.onClick.AddListener(() =>
            {
                _isDefault = false;
                SelectItem(nodeItem._areaConfig);

                _nodeItems.ForEach(a =>
                {
                    a.PlayAnim(a._areaConfig == nodeItem._areaConfig
                        ? ChoseNodeItem.AnimType.On
                        : ChoseNodeItem.AnimType.Off);
                });
            });

            if (_nodeItems[index]._itemObj.gameObject.activeSelf)
                activeNum++;

            _nodeItems[index].PlayAnim(areas[i] == _selectDecArea.Config
                ? ChoseNodeItem.AnimType.On
                : ChoseNodeItem.AnimType.Off);
            index++;
        }

        _itemGroup.transform.localScale = activeNum >= 4 ? new Vector3(0.9f, 0.9f, 1) : Vector3.one;
    }

    public void SetData(Deco.Node.DecoNode node, List<Deco.Item.DecoItem> items, bool fromFirstBuy)
    {
        _isDefault = true;
        _items = items;
        _selectDecNode = node;
        _fromFirstBuy = fromFirstBuy;

        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ConfirmNode, _selectDecNode.Config.id.ToString());
        GuideSubSystem.Instance.Trigger(GuideTriggerPosition.ConfirmNode, _selectDecNode.Config.costId.ToString());
        EndPreView();
        SelectItem();
        UpdateNodeItems();
    }

    public void PreView()
    {
        _isPreview = true;
        _itemGroup.gameObject.SetActive(false);
        _btnOK.gameObject.SetActive(false);
        _btnCancel.gameObject.SetActive(true);
    }

    public void EndPreView()
    {
        _isPreview = false;
        _itemGroup.gameObject.SetActive(true);
        _btnOK.gameObject.SetActive(true);
    }

    private void SelectItem()
    {
        if (_items == null || _items.Count == 0)
        {
            OnClickOk();
            return;
        }

        _btnCancel.gameObject.SetActive(_selectDecNode.IsFinish);

        Action selectAction = () =>
        {
            if (_selectDecNode._data._storage.CurrentItemId > 0 &&
                _selectDecNode._data._storage.CurrentItemId != _selectDecNode.Config.defaultItem)
            {
                SelectItem(_selectDecNode._data._storage.CurrentItemId);
            }
            else
                SelectItem(_items[0].Id);
        };

        if (_selectDecNode._data._config.nodeDepends > 0)
        {
            int autoChoseIndex = 0;
            DecoNode decoNode = DecoManager.Instance.FindNode(_selectDecNode._data._config.nodeDepends);
            if (decoNode != null)
            {
                if (decoNode._data._config.itemList != null)
                {
                    for (int i = 0; i < decoNode._data._config.itemList.Length; i++)
                    {
                        if (decoNode._data._storage.CurrentItemId != decoNode._data._config.itemList[i])
                            continue;

                        autoChoseIndex = i;
                        break;
                    }
                }
            }

            if (_selectDecNode.IsFinish)
            {
                selectAction();
            }
            else if (_selectDecNode.IsOwned)
            {
                autoChoseIndex = autoChoseIndex >= _items.Count ? 0 : autoChoseIndex;
                SelectItem(_items[autoChoseIndex].Id, false);
                OnClickOk();
            }
            else if (_selectDecNode._data._storage.CurrentItemId > 0)
            {
                selectAction();
            }
            else
            {
                autoChoseIndex = autoChoseIndex >= _items.Count ? 0 : autoChoseIndex;
                SelectItem(_items[autoChoseIndex].Id, false);
                OnClickOk();
            }

            return;
        }

        if (_fromFirstBuy)
        {
            if (_selectDecNode._data._config.itemList != null && _selectDecNode._data._config.itemList.Length == 1)
            {
                SelectItem(_items[0].Id, false);
                OnClickOk();
            }
            else
            {
                selectAction();
            }

            return;
        }

        selectAction();
    }

    public void SelectItem(int itemId, bool isSendEvent = true)
    {
        if (_selectDecItem != null && _selectDecItem.Id == itemId)
            return;

        _selectDecItem = _items.Find(a => a.Id == itemId);
        if (_selectDecItem == null)
            return;

        if (!isSendEvent)
            return;
        AudioManager.Instance.PlaySound(39);
        _selectDecNode.OnTap();
        EventDispatcher.Instance.DispatchEvent(EventEnum.SELECT_ONE_ITEM, _selectDecNode.Id, itemId, _fromFirstBuy);
    }

    public void SelectItem(TableAreas areaConfig)
    {
        if (_selectDecArea.Config != null && _selectDecArea.Config == areaConfig)
            return;

        if (_selectDecArea.Graphic != null)
            _selectDecArea.Graphic.SetActive(false);

        DynamicMapManager.Instance.ForceLoadCurrentChunk();

        _selectDecArea = DecoManager.Instance.FindArea(areaConfig.id);
        if (_selectDecArea == null)
            return;

        if (_selectDecArea.Graphic != null)
            _selectDecArea.Graphic.SetActive(true);
    }

    private void UpdateNodeItems()
    {
        _nodeItems.ForEach(a =>
        {
            a._btn.onClick.RemoveAllListeners();
            a._itemObj.SetActive(false);
        });

        if (_items != null && _items.Count > _nodeItems.Count)
        {
            int count = _items.Count - _nodeItems.Count;
            for (int i = 0; i < count; i++)
            {
                var obj = GameObject.Instantiate(_cloneItem, _itemGroup.transform);
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localScale = Vector3.one;
                var nodeItem = new ChoseNodeItem(obj);
                _nodeItems.Add(nodeItem);
                CommonUtils.SetShieldButUnEnable(nodeItem._btn.gameObject);
                nodeItem._btn.onClick.AddListener(() =>
                {
                    _isDefault = false;
                    SelectItem(nodeItem._decoItem.Id);
                    UpdateChoseAnim();
                });
                var arrowPointer = nodeItem._itemObj.transform.Find("Arrow");
                arrowPointer.gameObject.SetActive(false);
                if (!arrowPointer.gameObject.TryGetComponent<KeepScaleAndPosition>(out var scaleKeeper))
                {
                    scaleKeeper = arrowPointer.gameObject.AddComponent<KeepScaleAndPosition>();
                    scaleKeeper.Awake();
                }
            }
        }

        if (_items == null)
            return;

        int activeNum = 0;
        for (int i = 0; i < _items.Count; i++)
        {
            _nodeItems[i]._itemObj.SetActive(true);
            _nodeItems[i].UpdateData(_items[i], _items.Count > 1);

            var arrowPointer = _nodeItems[i]._itemObj.transform.Find("Arrow");
            arrowPointer.gameObject.SetActive(_selectDecNode.Id == 101002);
            // if (!arrowPointer.gameObject.TryGetComponent<KeepScaleAndPosition>(out var scaleKeeper))
            // {
            //     scaleKeeper = arrowPointer.gameObject.AddComponent<KeepScaleAndPosition>();
            // }

            if (_nodeItems[i]._itemObj.gameObject.activeSelf)
                activeNum++;
            var nodeItem = _nodeItems[i];
            nodeItem._btn.onClick.AddListener(() =>
            {
                _isDefault = false;
                SelectItem(nodeItem._decoItem.Id);
                UpdateChoseAnim();
            });

            _nodeItems[i]
                .PlayAnim(_items[i] == _selectDecItem ? ChoseNodeItem.AnimType.On : ChoseNodeItem.AnimType.Off);
        }

        _itemGroup.transform.localScale = activeNum >= 4 ? new Vector3(0.9f, 0.9f, 1) : Vector3.one;
    }

    private void UpdateChoseAnim()
    {
        _nodeItems.ForEach(a => { a.PlayAnim(a._decoItem == _selectDecItem ? ChoseNodeItem.AnimType.On : ChoseNodeItem.AnimType.Off); });
    }

    private void OnClickCancel()
    {
        if (_selectDecArea != null)
        {
            if (_defaultSelectDecArea.Graphic != null)
                _defaultSelectDecArea.Graphic.SetActive(true);

            if (_selectDecArea != _defaultSelectDecArea && _selectDecArea.Graphic != null)
                _selectDecArea.Graphic.SetActive(false);
        }

        RestLocalData();

        UIHomeMainController.ShowUI(true);
        MainDecorationController.mainController.AnimShowUIs(false, endCall: () =>
        {
            EndPreView();
            DecoManager.Instance.OnNodeEndPreView();
        });
        EventDispatcher.Instance.DispatchEvent(EventEnum.UNSELECT_NODE);
        EventDispatcher.Instance.DispatchEvent(EventEnum.SHOW_BUILD_BUBBLE, true);
    }

    public void OnClickOk()
    {
        if (_selectDecArea != null)
        {
            DecoExtendAreaManager.Instance.UpdateAreaShow(_selectDecArea.Config.showArea, _selectDecArea.Config.id);
            RestLocalData();

            UIHomeMainController.ShowUI(true);
            MainDecorationController.mainController.AnimShowUIs(false, endCall: () =>
            {
                EndPreView();
                DecoManager.Instance.OnNodeEndPreView();
            });
            EventDispatcher.Instance.DispatchEvent(EventEnum.UNSELECT_NODE);
            EventDispatcher.Instance.DispatchEvent(EventEnum.SHOW_BUILD_BUBBLE, true);
            return;
        }

        UIRoot.Instance.EnableEventSystem = false;
        UIHomeMainController.HideUI(true);
        AnimControlManager.Instance.AnimShow(AnimKey.Der_Select, false, false);
        bool isOwned = _selectDecNode.IsOwned;

        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ConfirmNode, _selectDecNode.Config.id.ToString());
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.ConfirmNode, _selectDecNode.Config.costId.ToString());

        List<ResData> dayResData = null;
        bool isNewDay = false;

        if (_fromFirstBuy)
        {
            InteractLogicManager.Instance.Interact(_selectDecNode.Config.interactLogic);

            AddDecorationReward(_selectDecNode.Config);
            DaysManager.Instance.OwnNode(_selectDecNode);
            dayResData = DaysManager.Instance.GetDayStepReward(_selectDecNode, true);
            isNewDay = DaysManager.Instance.NewDay();

//             if (Makeover.Utils.IsOpen && SceneFsm.mInstance.GetCurrSceneType() != StatusType.EnterFarm)
//             {
//                 if (DitchModel.Instance.Ios_Ditch_Plan_D())
//                 {
//                     int totalNodes = DaysManager.Instance.GetDayTotalNodes();
//                     foreach (var config in DitchConfigManager.Instance.TableDitchLevelList)
//                     {
//                         if (config.UnlockNodeNum == totalNodes + DaysManager.Instance.DayStep + 1)
//                         {
//                             if (dayResData == null)
//                                 dayResData = new List<ResData>();
//
//                             dayResData.Add(new ResData((int)UserData.ResourceId.DigTrench, 1));
//                             break;
//                         }
//                     }
//                 }
//                 else if (!FilthyGameLogic.Instance.IsOpenFilthy())
//                 {
//                     int totalNodes = DaysManager.Instance.GetDayTotalNodes();
//                     if (!Makeover.Utils.IsUseNewMiniGame())
//                     {
//                         bool isAddMiniGameIcon = true;
// #if UNITY_ANDROID || UNITY_EDITOR
//                         if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.69"))
//                         {
//                             isAddMiniGameIcon = false;
//                         }
// #endif
//                         if (isAddMiniGameIcon)
//                         {
//                             foreach (var level in ConnectLineConfigManager.Instance._configs)
//                             {
//                                 if (level.unlockNodeNum == totalNodes + DaysManager.Instance.DayStep + 1)
//                                 {
//                                     if (dayResData == null)
//                                         dayResData = new List<ResData>();
//
//                                     dayResData.Add(new ResData((int)UserData.ResourceId.ConnectLine, 1));
//                                     break;
//                                 }
//                             }
//
//                             foreach (var level in OnePathConfigManager.Instance._configs)
//                             {
//                                 if (level.unlockNodeNum == totalNodes + DaysManager.Instance.DayStep + 1)
//                                 {
//                                     if (dayResData == null)
//                                         dayResData = new List<ResData>();
//
//                                     dayResData.Add(new ResData((int)UserData.ResourceId.OnePath, 1));
//                                     break;
//                                 }
//                             }
//
//                             foreach (var level in FishEatFishConfigManager.Instance.FishEatFishLevelList)
//                             {
//                                 if (level.unlockNodeNum == totalNodes + DaysManager.Instance.DayStep + 1)
//                                 {
//                                     if (dayResData == null)
//                                         dayResData = new List<ResData>();
//
//                                     dayResData.Add(new ResData((int)UserData.ResourceId.FishEatFish, 1));
//                                     break;
//                                 }
//                             }
//
//                             foreach (var level in DigTrenchConfigManager.Instance.DigTrenchLevelList)
//                             {
//                                 if (level.unlockNodeNum == totalNodes + DaysManager.Instance.DayStep + 1)
//                                 {
//                                     if (dayResData == null)
//                                         dayResData = new List<ResData>();
//
//                                     dayResData.Add(new ResData((int)UserData.ResourceId.DigTrench, 1));
//                                     break;
//                                 }
//                             }
//                         }
//                     }
//                     else
//                     {
//                         var group = Makeover.Utils.GetMiniGroup();
//
//                         if (Makeover.Utils.IsOn(UIPopupMiniGameController.MiniGameType.DigTrench))
//                             group = MiniGroup.DigTrench;
// #if UNITY_ANDROID || UNITY_EDITOR
//                         if (StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey("1.0.69"))
//                         {
//                             group = MiniGroup.None;
//                         }
// #endif
//                         switch (group)
//                         {
//                             case MiniGroup.Puzzle:
//                             {
//                                 foreach (var level in PsychologyConfigManager.Instance._configs)
//                                 {
//                                     if (level.unlockNodeNum == totalNodes + DaysManager.Instance.DayStep + 1)
//                                     {
//                                         if (dayResData == null)
//                                             dayResData = new List<ResData>();
//
//                                         dayResData.Add(new ResData((int)UserData.ResourceId.BlueBlock, 1));
//                                         break;
//                                     }
//                                 }
//
//                                 break;
//                             }
//                             case MiniGroup.DigTrench:
//                             {
//                                 foreach (var level in DigTrenchConfigManager.Instance.DigTrenchLevelList)
//                                 {
//                                     if (level.unlockNodeNum == totalNodes + DaysManager.Instance.DayStep + 1)
//                                     {
//                                         if (dayResData == null)
//                                             dayResData = new List<ResData>();
//
//                                         dayResData.Add(new ResData((int)UserData.ResourceId.DigTrench, 1));
//                                         break;
//                                     }
//                                 }
//
//                                 break;
//                             }
//                         }
//                     }
//                 }
//             }
        }

        bool isRestParent = _selectDecNode.Config.costId == (int)UserData.ResourceId.HappyGo;
        PlayerManager.Instance.PlayAnimation(_selectDecNode, true, isRestParent: isRestParent);

        float waitTime = 0f;
        var npcConfig =
            DecorationConfigManager.Instance.NpcConfigList.Find(a => a.id == _selectDecNode.Config.npcConfigId);
        if (npcConfig != null)
        {
            var info = DecorationConfigManager.Instance.NpcInfoList.Find(a => a.id == npcConfig.chiefInfoId);
            if (info != null)
                waitTime = info.waitHeroTime;
        }

        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(waitTime, () =>
        {
            bool isNextAreaUnLock = true;
            if (_selectDecNode._stage.Area._data._config.nextAreaId > 0)
            {
                var nextArea =
                    _selectDecNode._stage.Area._world.AreaDict[_selectDecNode._stage.Area._data._config.nextAreaId];
                isNextAreaUnLock = nextArea.IsUnlock;
            }

            NodeDepends();

            var tempNode = _selectDecNode;
            applySelection(_selectDecNode, _selectDecItem, _fromFirstBuy, () =>
            {
                PlayerManager.Instance.SwitchPlayerStatusIdle(_selectDecNode);

                if (!isOwned || (_fromFirstBuy && isOwned))
                {
                    StartCoroutine(PlayGetRewardAnim(isNextAreaUnLock, dayResData, isNewDay));

                    DecoManager.Instance.CurrentWorld.UpdateSuggestNode();
                }
                else
                {
                    if (!StorySubSystem.Instance.Trigger(StoryTrigger.DecoItemSuccess, _selectDecItem.Id.ToString(),
                            isFinish =>
                            {
                                UIRoot.Instance.EnableEventSystem = true;
                                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY, tempNode.Id);
                                RestLocalData();
                                tempNode._previewItem = null;
                            }))
                    {
                        UIRoot.Instance.EnableEventSystem = true;
                        EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY, tempNode.Id);
                        RestLocalData();
                        tempNode._previewItem = null;
                    }
                    else
                    {
                        UIRoot.Instance.EnableEventSystem = true;
                    }
                }
            });
        }));
    }

    private void NodeDepends()
    {
        if (_selectDecNode == null || _selectDecItem == null)
            return;

        if (_selectDecNode._data._config.nodeDepends <= 0)
            return;

        if (_selectDecNode._data._storage.CurrentItemId <= 0)
            return;

        int choseIndex = 0;

        if (_selectDecNode._data._config.itemList != null)
        {
            for (int i = 0; i < _selectDecNode._data._config.itemList.Length; i++)
            {
                if (_selectDecItem.Id != _selectDecNode._data._config.itemList[i])
                    continue;

                choseIndex = i;
                break;
            }
        }

        DecoNode decoNode = DecoManager.Instance.FindNode(_selectDecNode._data._config.nodeDepends);
        if (decoNode == null)
            return;

        if (decoNode._data._config.itemList != null)
        {
            for (int i = 0; i < decoNode._data._config.itemList.Length; i++)
            {
                if (i != choseIndex)
                    continue;

                decoNode._data._storage.CurrentItemId = decoNode._data._config.itemList[i];
                break;
            }
        }
    }

    private IEnumerator PlayGetRewardAnim(bool isNextAreaUnLock, List<ResData> dayResData, bool isNewDay)
    {
        yield return new WaitForSeconds(0.1f);

        AudioManager.Instance.PlaySound(28);

        Action flyReward = () =>
        {
            if (isNewDay)
            {
                Action closeAction = () => { FlyRewardObject(isNextAreaUnLock, dayResData); };

                UIManager.Instance.OpenUI(UINameConst.UIPopupTaskNewDay, closeAction);
            }
            else
            {
                FlyRewardObject(isNextAreaUnLock, dayResData);
            }
        };

        if (StorySubSystem.Instance.Trigger(StoryTrigger.BuyNodeShowSuccess, _selectDecNode.Id.ToString(),
                isFinish =>
                {
                    if (isFinish)
                        flyReward();
                },
                isGuide => { }))
        {
            UIRoot.Instance.EnableEventSystem = true;
        }
        else
        {
            flyReward();
        }
    }

    private void FlyRewardObject(bool isNextAreaUnLock, List<ResData> dayResData)
    {
        PlayerManager.Instance.PlayAnimation(_selectDecNode, false,
            isRestParent: _selectDecNode.Config.costId == (int)UserData.ResourceId.HappyGo);

        UIRoot.Instance.EnableEventSystem = false;
        if (_selectDecNode._data._config.costId == (int)UserData.ResourceId.Mermaid)
        {
            UIRoot.Instance.EnableEventSystem = true;
            UIManager.Instance.OpenUI(UINameConst.MermaidSlider);
            RestLocalData();
            return;
        }

        AnimControlManager.Instance.AnimShow(AnimKey.Main_ResBar, true);
        AnimControlManager.Instance.AnimShow(AnimKey.Main_Bottom, true);

        FlyGameObjectManager.Instance.FlyObject(_selectDecNode.Config, dayResData, () =>
        {
            EventDispatcher.Instance.DispatchEvent(EventEnum.ASMR_REFRESH_REDPOINT);
            EventDispatcher.Instance.DispatchEvent(EventEnum.DIG_TRENCH_REFRESH_REDPOINT);
            EventDispatcher.Instance.DispatchEvent(EventEnum.FISH_EAT_FISH_REFRESH_REDPOINT);
            EventDispatcher.Instance.DispatchEvent(EventEnum.ONE_PATH_REFRESH_REDPOINT);
            EventDispatcher.Instance.DispatchEvent(EventEnum.CONNECT_LINE_REFRESH_REDPOINT);

            bool isUnlockNext = _selectDecNode.UnlockNextAfterFirstBuy();

            if (_selectDecNode._stage.Area._data._config.nextAreaId == 110)
            {
                if (ABTest.ABTestManager.Instance.IsLockMap())
                    isUnlockNext = false;
            }

            if (_selectDecNode._stage.Area._data._config.nextAreaId > 0 && !isNextAreaUnLock && isUnlockNext)
            {
                DynamicMapManager.Instance.ForceLoadCurrentChunk();

                var firstNode = DecoManager.Instance.GetFirstNode(_selectDecNode._stage.Area._data._config.nextAreaId);
                if (firstNode != null)
                {
                    NodeBubbleManager.Instance.SetBubbleActive(NodeBubbleManager.BubbleType.Lock, firstNode);
                    NodeBubbleManager.Instance.OnLoadBubble(firstNode);
                }

                DecoManager.Instance.TriggerAreaUnlock(_selectDecNode._stage.Area._data._config.id,
                    _selectDecNode._stage.Area._data._config.nextAreaId,
                    () =>
                    {
                        Action unlockArea = () =>
                        {
                            WorldFogManager.Instance.Hide(_selectDecNode._stage.Area._data._config.nextAreaId, true,
                                () =>
                                {
                                    CurrencyGroupManager.Instance.currencyController.CheckLevelUp(null);
                                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY,
                                        _selectDecNode.Id);
                                    RestLocalData();
                                    UIRoot.Instance.EnableEventSystem = true;
                                    // if (CoinLeaderBoardModel.CanShowAllNodeFinishUI())
                                    //     return;
                                    if (RecoverCoinModel.CanShowAllNodeFinishUI())
                                        return;
                                });
                        };

                        if (firstNode == null)
                        {
                            unlockArea();
                        }
                        else
                        {
                            FollowTargetBase targetBase =
                                NodeBubbleManager.Instance.GetBubble(NodeBubbleManager.BubbleType.Lock, firstNode);
                            if (targetBase == null)
                            {
                                unlockArea();
                            }
                            else
                            {
                                targetBase.gameObject.SetActive(true);
                                ((LockBubbleController)targetBase).PlayUnLockAnim(() =>
                                {
                                    unlockArea();
                                    NodeBubbleManager.Instance.UnLoadBubble(NodeBubbleManager.BubbleType.Lock,
                                        firstNode);
                                });
                            }
                        }
                    });
            }
            else
            {
                var tempNode = _selectDecNode;
                var isCanLevelUp = ExperenceModel.Instance.IsCanLevelUp();
                CurrencyGroupManager.Instance.currencyController.CheckLevelUp(() =>
                {
                    if (StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.FinishNode, _selectDecNode.Id.ToString(),
                            onStoryEnd: (b) =>
                            {
                                if (isCanLevelUp && b)
                                    CoroutineManager.Instance.StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.LevelUpPopUIViewLogic());
                                else
                                {
                                    EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY, tempNode.Id);
                                }
                            }))
                    {
                        RestLocalData();

                        if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_Room_Decorate))
                        {
                            AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_Room_Decorate, b => { });
                        }
                    }
                    else
                    {
                        if (tempNode._data._config.costId == (int)UserData.ResourceId.Coin)
                            EventDispatcher.Instance.DispatchEvent(EventEnum.FocusOnSuggestNode, tempNode);

                        CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(0.5f, () =>
                        {
                            UIRoot.Instance.EnableEventSystem = true;
                            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.HIDE_NODE_BUY,
                                _selectDecNode.Id);
                            RestLocalData();
                            // if (CoinLeaderBoardModel.CanShowAllNodeFinishUI())
                            //     return;
                            if (RecoverCoinModel.CanShowAllNodeFinishUI())
                                return;
                            if (AdSubSystem.Instance.CanPlayInterstitial(ADConstDefine.IN_Room_Decorate))
                            {
                                AdSubSystem.Instance.PlayInterstital(ADConstDefine.IN_Room_Decorate, b => { });
                            }

                            if (isCanLevelUp)
                                CoroutineManager.Instance.StartCoroutine(AutoPopupManager.AutoPopupManager.Instance.LevelUpPopUIViewLogic());
                        }));
                    }
                });
            }
        });
    }

    public static void AddDecorationReward(TableNodes _nodeCfg)
    {
        if (_nodeCfg == null)
            return;

        if (_nodeCfg.rewardId == null || _nodeCfg.rewardId.Length == 0)
            return;

        for (int i = 0; i < _nodeCfg.rewardId.Length; i++)
        {
            if (!UserData.Instance.IsResource(_nodeCfg.rewardId[i]))
            {
                var config = GameConfigManager.Instance.GetItemConfig(_nodeCfg.rewardId[i]);
                if (config != null)
                {
                    GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                    {
                        MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType
                            .MergeChangeReasonDecorateReward,
                        itemAId = config.id,
                        ItemALevel = config.level,
                        isChange = true,
                    });
                }
            }

            UserData.Instance.AddRes(_nodeCfg.rewardId[i], _nodeCfg.rewardNumber[i],
                new GameBIManager.ItemChangeReasonArgs()
                    { reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.DecorateReward });
        }
    }

    private void applySelection(Deco.Node.DecoNode node, Deco.Item.DecoItem selectItem, bool firstBuyOnNode,
        Action onFinished = null)
    {
        var _nodeCfg = node.Config;
        //BI
        GameBIManager.Instance.SendDecoEvent_ChangeItem((int)(_nodeCfg.id / 1000), _nodeCfg.id,
            selectItem == null ? _nodeCfg.id.ToString() : selectItem.Id.ToString(), _isDefault);

        Decoration.DecoManager.Instance.ChangeItemAsync(node, selectItem, firstBuyOnNode, onFinished);
    }

    private void RestLocalData(bool restDecoArea = true)
    {
        _selectDecArea = null;
        _defaultSelectDecArea = null;
        _selectDecNode = null;
        _selectDecItem = null;
        _items = null;
        _fromFirstBuy = false;
        MainDecorationController.mainController.Status = DecoUIStatus.Normal;
        DecoManager.Instance.CurrentWorld.SelectedNode = null;
    }

    private void OnNodePreViewEnd(BaseEvent e)
    {
        DecoManager.Instance.CurrentWorld.FocusDefaultCameraSize(null);
        OnClickCancel();
    }

    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEventListener(EventEnum.NODE_PREVIEW_END, OnNodePreViewEnd);
    }
}