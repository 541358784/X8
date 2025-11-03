using System.Collections.Generic;
using ActivityLocal.DecoBuildReward;
using Deco.Area;
using Deco.Node;
using Deco.World;
using Farm.Model;
using Gameplay;
using Gameplay.UI.Capybara;
using UnityEngine;

namespace Decoration.Bubble
{
    public class NodeBubbleManager : Singleton<NodeBubbleManager>
    {
        private bool _isActive = true;
        private bool _isInit = false;
        public enum BubbleType
        {
            Normal,
            Lock,
            Play,
            Get,
        }

        private Dictionary<BubbleType, List<FollowTargetBase>> _bubbles = new Dictionary<BubbleType, List<FollowTargetBase>>();
        private Dictionary<BubbleType, Dictionary<DecoNode,DecoNode>> _bubbleActives = new Dictionary<BubbleType, Dictionary<DecoNode,DecoNode>>();
        
        public void Init()
        {
            if(_isInit)
                return;

            _isInit = true;
            EventDispatcher.Instance.AddEventListener(EventEnum.SHOW_BUILD_BUBBLE, OnShowBubble);
        }
        
        public void SetBubbleActive(bool isActive)
        {
            if(_isActive == isActive)
                return;
            
            _isActive = isActive;
            foreach (var bubble in _bubbles)
            {
                bubble.Value.ForEach(a=>
                {
                    if (!isActive)
                    {
                        a.gameObject?.SetActive(IsActive(bubble.Key, a._node));
                    }
                    else
                    {
                        if (bubble.Key == BubbleType.Normal)
                        {
                            var resready = a._node.Stage.Area.Graphic._resReady;
                            bool isShow = isActive && resready;
                        
                            a.gameObject?.SetActive(isShow);
                        }
                        else
                        {
                            a.gameObject?.SetActive(isActive);
                        }
                    }
                });
            }
        }

        public void OnLoadBubble(DecoNode node)
        {
            OnLoadBubble(BubbleType.Normal, node);
            OnLoadBubble(BubbleType.Lock, node);
            OnLoadBubble(BubbleType.Play, node);
            OnLoadBubble(BubbleType.Get, node);
        }
        
        public void UnLoadBubble(DecoNode node)
        {
            UnLoadBubble(BubbleType.Normal, node);
            UnLoadBubble(BubbleType.Lock, node);
            UnLoadBubble(BubbleType.Play, node);
            UnLoadBubble(BubbleType.Get, node);
        }

        public void SetBubbleActive(BubbleType type, DecoNode node)
        {
            FollowTargetBase targetBase = GetBubble(type, node);
            if (targetBase != null)
                targetBase.gameObject.SetActive(true);
            
            if (!_bubbleActives.ContainsKey(type))
            {
                _bubbleActives.Add(type, new Dictionary<DecoNode, DecoNode>());
            }
            
            if(!_bubbleActives[type].ContainsKey(node))
                _bubbleActives[type].Add(node, node);
        }

        private bool IsActive(BubbleType type, DecoNode node)
        {
            if (!_bubbleActives.ContainsKey(type))
                return false;

            if (_bubbleActives[type].ContainsKey(node))
                return true;
            
            return false;
        }
        
        public FollowTargetBase OnLoadBubble(BubbleType type, DecoNode node)
        {
            if (type == BubbleType.Play)
                return null;
            
            if (node.Config.costId == (int)UserData.ResourceId.Mermaid || node.Config.costId == (int)UserData.ResourceId.HappyGo || node.Config.costId == (int)UserData.ResourceId.Ship|| node.Config.costId == (int)UserData.ResourceId.Theme)
                return null;

            if (node.Id == 101099 || node.Id == 101098)
            {
                if (CapybaraManager.Instance.IsOpenCapybara())
                {
                    if (node.Id == 101099)
                        return null;
                }
                else
                {
                    if(node.Id == 101098)
                        return null;
                }
            }
            
            if (node._stage.Area.Config.hideAreaInDeco)
            {
                if (node.Config.costId == 10001)
                {
                    if (type != BubbleType.Get)
                        return null;
                }
                else
                {
                    return null;
                }
            }
            
            FollowTargetBase bubble = GetBubble(type, node);
            if (bubble != null)
            {
                if (!_isActive)
                    bubble.gameObject?.SetActive(IsActive(type, node));
                else
                    bubble.gameObject?.SetActive(_isActive);
                return bubble;
            }

            switch (type)
            {
                case BubbleType.Lock:
                {
                    if (!IsActive(type, node) && !CanCreateLockBubble(node))
                        return null;
                    
                    break;
                }
                case BubbleType.Normal:
                {  
                    if (!CanCreateNormalBubble(node))
                        return null;
                    
                    break;
                }
                case BubbleType.Get:
                {
                    if (node.Config.costId != 10001)
                        return null;

                    if (!DecoBuildRewardManager.Instance.CanGetReward(node.Config.id.ToString()))
                        return null;
                    
                    bool isOwned = false;
                    foreach (var kv in node.ItemDic)
                    {
                        if (kv.Value.IsOwned)
                        {
                            isOwned = true;
                            break;
                        }
                    }

                    if (!isOwned)
                        return null;
                    
                    break;
                }
            }

            return CreateBubble(type, node);
        }

        public bool UnLoadBubble(BubbleType type, DecoNode node, bool unLoadData = true)
        {
            if (_bubbleActives.ContainsKey(type))
            {
                if (_bubbleActives[type].ContainsKey(node))
                    _bubbleActives[type].Remove(node);
            }
            
            FollowTargetBase bubble = GetBubble(type, node);
            if (bubble == null)
                return false;
            
            OpUtils.UnloadObjFromBundleManager(GetBubblePath(type));
            DecoManager.Instance.PoolManager.RecycleGameObject(bubble.gameObject);
            if(unLoadData)
                UnLoadBubbleData(type, node);
            
            return true;
        }

        public void UnLoadBubble()
        {
            foreach (var bubble in _bubbles)
            {
                foreach (var followTargetBase in bubble.Value)
                {
                    UnLoadBubble(bubble.Key, followTargetBase._node, false);
                }
                
                bubble.Value.Clear();
            }
            
            _bubbles.Clear();
        }

        public FollowTargetBase CreateBubble(BubbleType type, DecoNode node)
        {
            string bubblePath = GetBubblePath(type);
            FollowTargetBase targetBase = null;
            switch (type)
            {
                case BubbleType.Normal:
                {
                    if (AssetCheckManager.Instance.GetAreaResNeedToDownload(node._stage.Area.Id).Count > 0)
                        return null;
                    
                    targetBase = CreateBubble<SuggestBubbleController>(bubblePath);
                    break;
                }
                case BubbleType.Lock:
                {
                    targetBase = CreateBubble<LockBubbleController>(bubblePath);
                    break;
                }
                case BubbleType.Play:
                {
                    targetBase = CreateBubble<PlayBubbleController>(bubblePath);
                    break;
                }
                case BubbleType.Get:
                {
                    targetBase = CreateBubble<GetBubbleController>(bubblePath);
                    break;
                }
            }

            if (targetBase == null)
                return null;

            targetBase.BindNode(node);
            targetBase.FollowTarget(node.Graphic.TipTransform);
#if UNITY_EDITOR
            targetBase.gameObject.name = "Node-" + node.Id;
#endif
            if (!_isActive)
            {
                targetBase.gameObject.SetActive(IsActive(type, node));
            }
            else
            {
                targetBase.gameObject.SetActive(_isActive);
            }
                
            List<FollowTargetBase> bubbles = GetBubblesByType(type);
            if (bubbles == null)
            {
                bubbles = new List<FollowTargetBase>();
                _bubbles.Add(type, bubbles);
            }
            
            bubbles.Add(targetBase);

            return targetBase;
        }

        private FollowTargetBase CreateBubble<T>(string path) where T : FollowTargetBase
        {
            if (path.IsEmptyString())
                return null;
            
            var tipObject = DecoManager.Instance.PoolManager.SpawnGameObject(path);
            if (tipObject == null)
                return null;
            
            CommonUtils.AddChild(UIRoot.Instance.mWorldUIRoot.transform, tipObject.transform, false);
            tipObject.transform.localScale = Vector3.one;
            return CommonUtils.GetOrCreateComponent<T>(tipObject);
        }
        
        public FollowTargetBase GetBubble(BubbleType type, DecoNode node)
        {
            var bubbles = GetBubblesByType(type);
            if (bubbles == null || bubbles.Count == 0)
                return null;

            return bubbles.Find(a => a._node == node);
        }
        
        private List<FollowTargetBase> GetBubblesByType(BubbleType type)
        {
            if (_bubbles.ContainsKey(type))
                return _bubbles[type];

            return null;
        }

        private bool UnLoadBubbleData(BubbleType type, DecoNode node)
        {
            List<FollowTargetBase> bubbles = GetBubblesByType(type);
            if (bubbles == null)
                return false;

            var findNode = bubbles.Find(a => a._node == node);
            if (findNode == null)
                return false;

            bubbles.Remove(findNode);

            return true;
        }
        
        private string GetBubblePath(BubbleType type)
        {
            switch (type)
            {
                case BubbleType.Normal:
                {
                    return SuggestBubbleController.BUBBLE_PREFAB_PATH;
                }
                case BubbleType.Lock:
                {
                    return LockBubbleController.BUBBLE_PREFAB_PATH;
                }
                case BubbleType.Play:
                {
                    return PlayBubbleController.BUBBLE_PREFAB_PATH;
                }
                case BubbleType.Get:
                {
                    return GetBubbleController.BUBBLE_PREFAB_PATH;
                }
            }

            return null;
        }

        private bool CanCreateNormalBubble(DecoNode node)
        {
            if (node == null)
                return false;
            
            if (FarmModel.Instance.IsFarmModel())
            {
                return FarmModel.Instance.CanUnLockNode(node);
            }
            
            if(!DecoManager.Instance.IsAreaUnLock(node))
                return false;
            
            return node.SuggestTest();
        }

        private bool CanCreateLockBubble(DecoNode node)
        {
            if (FarmModel.Instance.IsFarmModel())
                return false;
            
            if (node == null)
                return false;
            
            DecoArea lockArea = DecoManager.Instance.GetLockArea();
            if(lockArea != node._stage.Area)
                return false;
            
            if(!DecoManager.Instance.IsFirstNode(node))
                return false;

            return true;
        }
        
        private void OnShowBubble(BaseEvent baseEvent)
        {
            if (baseEvent == null || baseEvent.datas == null || baseEvent.datas.Length == 0)
                return;

            bool isShow = (bool)baseEvent.datas[0];
            if (isShow)
            {
                _bubbleActives.Clear();
                
                if(DecoWorld.SuggestNode != null)
                    DecoWorld.SuggestNode.ForEach(a=>OnLoadBubble(a));
            }

            if (baseEvent.datas.Length >= 2)
            {
                _isActive = !isShow;
            }
            
            SetBubbleActive(isShow);
        }
    }
}