using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using Spine.Unity;
using System;
using DG.Tweening;
using Framework;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Deco.Area;
using Deco.Graphic;
using Deco.World;
using Deco.Stage;
using Deco.Item;
using DragonU3DSDK;
using SRF;
using Decoration;
using Decoration.Bubble;
using Gameplay;


namespace Deco.Node
{
    using StorageDecoration=DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld=DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea=DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode=DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage=DragonU3DSDK.Storage.Decoration.StorageStage;
    public class DecoNode : DecoGraphicHost<DecoNodeGraphic>
    {
        public enum Status
        {
            Lock = 0, //锁定中
            Unlock, //已解锁，可购买
            Owned, //已拥有
            Finish, //已安装过
        }

        public enum Type
        {
            ChangeDirect = 0,
            SelectRequired,
            NewAreaUnlock,
        }

        internal DecoNodeData _data;

        internal Stage.DecoStage _stage = null;
        internal Dictionary<int, Item.DecoItem> _itemDict = new Dictionary<int, Item.DecoItem>();
        internal Item.DecoItem _currentItem = null;
        internal Item.DecoItem _previewItem = null;

        public bool IsPreview
        {
            get { return _previewItem != null; }
        }
        
        public int Id => _data._config.id;
        public TableNodes Config => _data._config;
        public Dictionary<int, Item.DecoItem> ItemDic => _itemDict;
        public Item.DecoItem CurrentItem => _currentItem;
        public Item.DecoItem DefaultItem => _itemDict.ContainsKey(_data._config.defaultItem) ? _itemDict[_data._config.defaultItem] : null;
        public Stage.DecoStage Stage => _stage;
        public GameObject GameObject => Graphic.gameObject;
        public Transform IconTipTransform => Graphic.TipTransform;
        public Transform CameraTipTransform => Graphic.CameraTipTransform;
        public bool Locked => _data._storage.Status == (int) Status.Lock;
        public bool UnLocked => _data._storage.Status == (int) Status.Unlock;
        public bool IsOwned => _data._storage.Status >= (int) Status.Owned;
        public bool IsFinish => _data._storage.Status >= (int) Status.Finish;

        protected Animator _animator;
        
        //public bool BubbleVisible => Graphic._bubbleButton && Graphic._bubbleButton.Visible;
        public int SelectItemIndex(int itemId)
        {
            if (Config.itemList == null || Config.itemList.Length == 0) return -1;
            for (int i = 0; i < Config.itemList.Length; i++)
            {
                if (Config.itemList[i] == itemId)
                {
                    return i;
                }
            }
            return -1;
        }

        public DecoNode(TableNodes config, StorageNode storage, Stage.DecoStage stage)
        {
            _stage = stage;
            _data = new DecoNodeData(this, config, storage);
            Graphic = new DecoNodeGraphic(this);

            World.DecoWorld.NodeLib[config.id] = this;

            initItems();
            _itemDict.TryGetValue(_data._storage.CurrentItemId, out _currentItem);

            fixCurrentItem();
        }

        private void initItems()
        {
            initDefaultItem();
            initItemList();
        }

        private void fixCurrentItem()
        {
            List<int> cleanIds = new List<int>();
            foreach (var item in _data._storage.ItemsData)
            {
                if(_itemDict.ContainsKey(item.Key))
                    continue;
                
                cleanIds.Add(item.Key);
            }
            cleanIds.ForEach(a=>_data._storage.ItemsData.Remove(a));

            //多次新手引导 强制修复
            if (_data._storage.Id == 101002)
            {
                if (_currentItem != null && IsOwned)
                {
                    if (_data._config != null && _data._config.itemList != null && _data._storage.CurrentItemId != _data._config.itemList[0])
                        _data._storage.CurrentItemId = _data._config.itemList[0];
                } 
            }
            
            if (!IsFinish)
            {
                if (!IsOwned)
                {
                    if(_currentItem != null)
                        return;
                    
                    _data._storage.CurrentItemId = _data._config.defaultItem;
                    _itemDict.TryGetValue(_data._storage.CurrentItemId, out _currentItem);
                }
                return;
            }
            
            if(_currentItem != null)
                return;
            
            //剧情挂点
            if(_data._storage.CurrentItemId == 0 &&  (_data._storage.ItemsData == null || _data._storage.ItemsData.Count == 0))
                return;

            //清扫
            if (_data._storage.CurrentItemId < 0)
            {
                if(_data._config.itemList == null || _data._config.itemList.Length == 0 || _data._config.itemList[0] == 0)
                    return;
            }

            if (_data._config.itemList == null || _data._config.itemList.Length == 0 || _data._config.itemList[0] == 0)
            {
                _data._storage.CurrentItemId = -1;
            }
            else if (_data._config.itemList != null && _data._config.itemList.Length >= 1 && _data._config.itemList[0] > 0)
            {
                _data._storage.CurrentItemId = _data._config.itemList[0];
            }
            else
            {
                _data._storage.CurrentItemId = _data._config.defaultItem;
            }
            
            _itemDict.TryGetValue(_data._storage.CurrentItemId, out _currentItem);
        }

        private void initAnimator()
        {
            if(!_data._config.touchAnim)
                return;

            if (Graphic.gameObject == null)
                return;

            if(_animator != null)
                return;
            
            _animator = Graphic.gameObject.AddComponent<Animator>();
            _animator.runtimeAnimatorController =   RuntimeAnimatorManager.Instance.GetCommonClick();
            _animator.enabled = false;
        }
        
        private void initDefaultItem()
        {
            var configManager = DecorationConfigManager.Instance;

            if (_data._config.defaultItem > 0)
            {
                int itemId = _data._config.defaultItem;
                var itemConfig = configManager.GetItemConfig(itemId);
                if (itemConfig != null)
                {
                    var itemStorage=AssetCheckManager.Instance.GetStorageItem(_data._storage, itemConfig.id);
                    var item = new Item.DecoItem(itemConfig, itemStorage, this, true);
                    _itemDict[itemId] = item;
                }
                else
                {
                    DebugUtil.LogError(string.Format("## ItemConfig [{0}] not exist in Node [{1}] ##", itemId, Id));
                }
            }
        }

        private void initItemList()
        {
            var configManager = DecorationConfigManager.Instance;
            if (_data._config.itemList != null)
            {
                foreach (var itemId in _data._config.itemList)
                {
                    var itemConfig = configManager.GetItemConfig(itemId);
                    if (itemConfig != null)
                    {
                        //var itemStorage = UserDataMoudule.GetStorageItem(_data._storage, itemConfig.id);
                        var itemStorage=AssetCheckManager.Instance.GetStorageItem(_data._storage, itemConfig.id);
                        var item = new Item.DecoItem(itemConfig, itemStorage, this, false);
                        _itemDict[itemId] = item;
                    }
                    else
                    {
                        if(itemId > 0)
                            DebugUtil.LogError(string.Format("## BuildingConfig [{0}] not exist in BuildingPoint [{1}] ##",
                                itemId, Id));
                    }
                }
            }
        }

        public override void LoadGraphic(GameObject parentObj)
        {
            Graphic.Load(parentObj.transform);
        }

        public override void UnloadGraphic()
        {
            Graphic.Unload();
        }

        public bool DependenceTest()
        {
            return _data.dependenceTest();
        }

        public void NoDependenceTest()
        {
            _data.noDependenceTest();
        }

        public void Show(bool fromLogin)
        {
            Graphic.Show(fromLogin);
        }

        public void MarkNodeFinish()
        {
            _data.markNodeFinish();
        }
        
        /// <returns>区域是否完成</returns>
        public bool UnlockNextAfterFirstBuy()
        {
            MarkNodeFinish();
            
            //解锁依赖此挂点的挂点
            _stage.NodeDependenceTest();

            //检查Stage是否完成
            var stageFinish = _stage.TryFinish();

            if (stageFinish)
            {
                //检测Area是否完成
                var areaFinish = _stage.Area._data.TryFinish();

                if (areaFinish)
                {
                    //检测世界完成
                    var worldFinish = _stage.Area.World.TryWorldFinish();
                }
            }

            return stageFinish;
        }

        private IEnumerator continueToStoryOrCameraFocus(bool stageFinish, Action delayFinished)
        {
            Action focusOnSuggestNode = () =>
            {
                if (!_stage.Area._data.IsFinish) //如果区域完成，有单独的镜头切换
                {
                    EventDispatcher.Instance.DispatchEvent(EventEnum.FocusOnSuggestNode);
                }
            };

            DecoManager.Instance.PauseAll();
            
            yield return new WaitForSeconds(1f);
            delayFinished?.Invoke();
            DecoManager.Instance.ResumeAll();
            UIRoot.Instance.EnableTouch(true);

            if (stageFinish && _stage.Area._data.TryFinish())
            {
                //触发剧情
                CommonUtils.OpenCommonConfirmWindow(new NoticeUIData
                {
                    HasCancelButton = false,
                    HasCloseButton = false,
                    DescString = "Area Unlock",
                    OKCallback = () =>
                    {
                        var areaFinish = DecoManager.Instance.TriggerAreaUnlock(_stage.Area._data._config.id, _stage.Area._data._config.nextAreaId, null);
                    }
                });
            }
            else
            {
                focusOnSuggestNode.Invoke();
            }
        }

        public void ChangeItemSilence(int itemId)
        {
            _data.changeItem(itemId);
            _itemDict.TryGetValue(itemId, out _currentItem);
        }

        public IEnumerator BuyFirstItem(int itemId, Action onFinished)
        {
            HideNodeNeeded();
            
            yield return CoroutineManager.Instance.StartCoroutine(ChangeItem(itemId));
            
            //激活依赖此挂点显示控制的其他挂点
            ViewDependenceTest();
            ViewDependenceOtherTest();
            
            onFinished?.Invoke();
            
            NodeBubbleManager.Instance.UnLoadBubble(this);
        }
        
        public IEnumerator ReplaceItem(int itemId, bool playShowAnim = true, bool playHideAnim = true, Action onFinished = null)
        {
            HideNodeNeeded();
            
            _previewItem?.ShowSelectStatus(false);
            if (_data._storage.CurrentItemId != itemId)
            {                
                _data.changeItem(itemId);
                yield return CoroutineManager.Instance.StartCoroutine(Graphic.ReplaceItem(itemId, playShowAnim, playHideAnim));
                ChangeItemSilence(itemId);
            }

            UpdateNodeDepends();
            
            onFinished?.Invoke();
        }
        
        public IEnumerator ChangeItem(int itemId, bool playShowAnim = true, bool playHideAnim = true, Action onFinished = null)
        {
            _previewItem?.ShowSelectStatus(false);
            if (_data._storage.CurrentItemId != itemId)
            {               
                _data.changeItem(itemId);
                yield return CoroutineManager.Instance.StartCoroutine(Graphic.ChangeItem(itemId, playShowAnim, playHideAnim, onFinished));
                ChangeItemSilence(itemId);
            }

            if (_data._storage.Status != (int)Status.Finish)
            {
                UnlockNextAfterFirstBuy();
            }

            onFinished?.Invoke();
            //DragonPlus.GameBIManager.SendDecoEvent_ChangeItem(Id, itemId);
        }
        
        public void UpdateSuggestNode()
        {
            if (SuggestTest())
            {
                World.DecoWorld.SuggestNode.Add(this);
            }
        }

        public void PreviewItem(int itemId, bool showSelectEffect = true)
        {
            if (_itemDict.ContainsKey(itemId))
            {
                PreviewItem(_itemDict[itemId], showSelectEffect);
                GameObject.SetActive(true);
            }
            else
            {
                _currentItem?.Hide();
                _previewItem?.Hide();
            }

            SetNodeDependsActive(false);
            HideNodeNeeded(true);
        }

        public void PreviewItem(Item.DecoItem item, bool showSelectEffect = true, bool playAnim = false)
        {
            if (_currentItem != null && _currentItem != item)
            {
                _currentItem.Hide(playAnim);
            }

            _previewItem = item;
            _previewItem.LoadGraphic(Graphic.gameObject);
            _previewItem.Graphic?.EnableAnimator(false, true);
            _previewItem.Show(true);
           
            if (showSelectEffect)
            {
                _previewItem?.ShowSelectStatus(true);
            }
        }

        public IEnumerator PreviewItem(int itemId, bool playShowAnim, bool playHideAnim)
        {
            if (_itemDict.ContainsKey(itemId))
            {
                _previewItem = _itemDict[itemId];
                _previewItem.Hide(false);
            }
            
            //处理老建筑
            var oldItem = _currentItem;
            if (oldItem != null)
            {
                oldItem.ShowSelectStatus(false);
                yield return CoroutineManager.Instance.StartCoroutine(oldItem.Graphic.Hide(playHideAnim));
            }

            //处理新建筑
            if (_itemDict.ContainsKey(itemId))
            {
                _previewItem.ShowSelectStatus(false);
                _previewItem.Show(true);
                //播放新建筑的显示动画
                _previewItem.LoadGraphic(GameObject);

                if (playShowAnim)
                {
                    yield return CoroutineManager.Instance.StartCoroutine( _previewItem.Graphic.PlayShowAnimation());
                }
            }
        }

        public void EndPreview(bool playAnim = true)
        {
            _previewItem?.ShowSelectStatus(false);
            if (_previewItem == _currentItem)
            {
                _previewItem = null;
            }
            else
            {
                _previewItem?.Hide(playAnim);
                _previewItem = null;

                _currentItem?.Show(true);
            }

            SetNodeDependsActive(true);
        }

        public void SetNodeDependsActive(bool active)
        {
            var nodeConfig = DecorationConfigManager.Instance.nodeConfigs.Find(a => a.nodeDepends == _data._config.id);
            if(nodeConfig == null)
                return;

            DecoNode decoNode = DecoManager.Instance.FindNode(nodeConfig.id);
            if(decoNode.GameObject == null)
                return;
            
            decoNode.GameObject.SetActive(active);
        }

        private void UpdateNodeDepends()
        {
            var nodeConfig = DecorationConfigManager.Instance.nodeConfigs.Find(a => a.nodeDepends == _data._config.id);
            if(nodeConfig == null)
                return;

            DecoNode decoNode = DecoManager.Instance.FindNode(nodeConfig.id);
            if(decoNode == null)
                return;

            if (!decoNode.IsOwned)
                return;
            
            if(decoNode.GameObject == null)
                return;

            int choseIndex = 0;
            if (_data._config.itemList != null)
            {
                for (int i = 0; i < _data._config.itemList.Length; i++)
                {
                    if(_data._storage.CurrentItemId!=_data._config.itemList[i])
                        continue;

                    choseIndex = i;
                    break;
                } 
            }
            
            if (decoNode._data._config.itemList != null)
            {
                if (choseIndex >= decoNode._data._config.itemList.Length)
                    choseIndex = 0;
                
                if(decoNode._data._storage.CurrentItemId == decoNode._data._config.itemList[choseIndex])
                    return;

                var itemId = decoNode._data._config.itemList[choseIndex];
                var decoItem = DecoManager.Instance.FindItem(itemId);

                bool isNormalArea = false;
                foreach (var area in DecorationConfigManager.Instance.AreaConfigList)
                {
                    if (area.id.ToString() == decoItem._data._config.BuildingInArea)
                    {
                        isNormalArea = true;
                        break;
                    }
                }

                if (!isNormalArea)
                {
                    if(!decoItem.IsOwned)
                        return;
                }
                
                var oldItem = decoNode._currentItem;
                if (oldItem != null)
                    oldItem.ShowSelectStatus(false);
                
                decoNode._data._storage.CurrentItemId = decoNode._data._config.itemList[choseIndex];
               
                if (decoNode._itemDict.ContainsKey( decoNode._data._storage.CurrentItemId))
                {
                    var newItem = decoNode._itemDict[ decoNode._data._storage.CurrentItemId];
                    newItem.ShowSelectStatus(false);
                    newItem.Show(true);
                    //播放新建筑的显示动画
                    newItem.LoadGraphic(decoNode.Graphic.gameObject);
                }
            }
        }
        
        // 建筑点上是否有建筑已建造(不包含初始建筑)
        public bool DirtyTest()
        {
            return _data.dirtyTest();
        }
        public bool CanPassTest(){
            //特殊装修币装修的物品可以跳过
            if(_data._config.costId == (int)UserData.ResourceId.RareDecoCoin 
               || _data._config.costId == (int)UserData.ResourceId.Seal    
               || _data._config.costId == (int)UserData.ResourceId.Dolphin
               || _data._config.costId == (int)UserData.ResourceId.Capybara)
            {
                return true;
            }
            return false;
        }
        //挂点泡泡测试
        public bool SuggestTest()
        {
            return _data.suggestTest();
        }

        public void Buy()
        {
            _data.own();
            //DragonPlus.GameBIManager.SendDecoEvent_BuyNode(_data._config.id);
        }

        public void OnTap()
        {
            if (!_stage.Area.IsUnlock)
                return;

            initAnimator();
            
            if (_animator == null)
                return;

            _animator.enabled = true;
            _animator.Play("click", -1, 0);
            //EventDispatcher.Instance.DispatchEventImmediately(EventEnum.SELECT_NODE,0,false);
        }

        public DecoItemTouchResult TouchTest(Vector2 screenPos)
        {
            var touchResult = new DecoItemTouchResult();

            if (_currentItem != null)
            {
                var temp = _currentItem.TouchTest(screenPos);
                if (temp.result && World.DecoWorld.FrontTest(temp.z, touchResult.z))
                {
                    touchResult = temp;
                }
            }

            return touchResult;
        }

        public void PlayAction(DecoNodeAction nodeAction)
        {
            Graphic.playAction(nodeAction);
        }

        public void PlaySpineAnimation(string animName, Action action)
        {
            Graphic.PlaySpineAnimation(animName, action);
        }
        //隐藏此挂点要求隐藏的挂点
        public void HideNodeNeeded(bool isPreView = false)
        {
            if (!isPreView && !IsOwned)
                return;

            if (Config.hideNodes == null)
                return;
            
            foreach (var hideNodeId in Config.hideNodes)
            {
                if (DecoWorld.NodeLib.TryGetValue(hideNodeId, out var hideNode))
                {
                    hideNode.Graphic.Hide();
                }
            }
        }
        
        public void RestoreNodeNeeded()
        {
            if (IsOwned)
                return;
            
            if (Config.hideNodes == null)
                return;
            
            foreach (var hideNodeId in Config.hideNodes)
            {
                if (DecoWorld.NodeLib.TryGetValue(hideNodeId, out var hideNode))
                {
                    hideNode.Graphic.Show();
                }
            }
        }
        
        public void ViewDependenceTest()
        {
            if(Config == null || Config.viewDependNodes == null)
                return;
            
            var dependFinishAll = true;
            foreach (var dependNodeId in Config.viewDependNodes)
            {
                var dependNode = DecoWorld.NodeLib[dependNodeId];
                if (!dependNode.IsFinish)
                {
                    dependFinishAll = false;
                    break;
                }
            }
                
            Graphic?.gameObject.SetActive(dependFinishAll);
        }

        public void ViewDependenceOtherTest()
        {
            foreach (var area in _stage.Area.World._areaList)
            {
                foreach (var kv in area._stageDict)
                {
                    var stage = kv.Value;
                    foreach (var kv2 in stage._nodeDict)
                    {
                        var node = kv2.Value;
                        if (node.Config.viewDependNodes == null)
                            continue;
                    
                        if(!node.Config.viewDependNodes.Contains(Config.id))
                            continue;
                        
                        var dependFinishAll = true;
                        foreach (var dependNodeId in node.Config.viewDependNodes)
                        {
                            if(dependNodeId != Config.id)
                                continue;
                            
                            var dependNode = DecoWorld.NodeLib[dependNodeId];
                            if (!dependNode.IsFinish)
                            {
                                dependFinishAll = false;
                                break;
                            }
                        }

                        node.GameObject.SetActive(dependFinishAll);
                    }
                }
            }
        }
        
        public override void AsyncLoadGraphic(GameObject parentObj, bool isPreview, Action onFinished)
        {
        }
    }
}