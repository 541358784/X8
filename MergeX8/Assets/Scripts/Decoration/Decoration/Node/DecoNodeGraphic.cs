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
using System.Threading.Tasks;
using Deco.Area;
using Deco.Graphic;
using Deco.World;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;
using Decoration;
using Decoration.Bubble;
using Farm.Model;
using Gameplay.UI.Capybara;

namespace Deco.Node
{
    public class DecoNodeGraphic : DecoGraphic
    {
        internal List<Transform> _broomTransformList = null;

        private DecoNode _node;
        private Transform _tipTransform = null;
        private Transform _cameraTipTransform = null;
        private Transform _guideTipTransform = null;

        public Transform TipTransform
        {
            get
            {
                var t = _node._currentItem != null && _node._currentItem.TipTransform != null
                    ? _node._currentItem.TipTransform
                    : _tipTransform;
                if (!t)
                {
                    t = gameObject ? gameObject.transform : null;
                }

                return t;
            }
        }

        public Transform CameraTipTransform
        {
            get
            {
                var t = _node._currentItem != null && _node._currentItem.CameraTipTransform != null
                    ? _node._currentItem.CameraTipTransform
                    : _cameraTipTransform;

                if (!t)
                {
                    t = gameObject ? gameObject.transform : null;
                }

                return t;
            }
        }

        public DecoNodeGraphic(DecoNode node)
        {
            _node = node;
        }

        public void Show(bool playNormalAnim)
        {
            _node._currentItem?.Show(playNormalAnim: playNormalAnim);

            if (_node.Id == 101099 || _node.Id == 101098)
            {
                if (CapybaraManager.Instance.IsOpenCapybara())
                {
                    if(_node.Id == 101099)
                        gameObject?.SetActive(false);
                    else
                        gameObject?.SetActive(true);
                }
                else
                {
                    if(_node.Id == 101098)
                        gameObject?.SetActive(false);
                    else
                        gameObject?.SetActive(true);
                }
            }
            else
            {
                gameObject?.SetActive(true);
            }
            
            _node.ViewDependenceTest();
            _node.HideNodeNeeded();
        }

        public void Hide()
        {
            gameObject?.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        internal IEnumerator ChangeItem(int itemId, bool playShowAnim = true, bool playHideAnim = true, Action changeEnd = null)
        {
            if (!gameObject)
            {
                changeEnd?.Invoke();
                yield break;
            }

            if (_node._itemDict.ContainsKey(itemId))
            {
                var newItem = _node._itemDict[itemId];
                newItem.Hide(false);
            }
            
            //处理老建筑
            var oldItem = _node._currentItem;
            if (oldItem != null)
            {
                oldItem.ShowSelectStatus(false);
                if(playHideAnim)
                    yield return CoroutineManager.Instance.StartCoroutine(oldItem.Graphic.Hide(playHideAnim));
            }

            //处理新建筑
            if (_node._itemDict.ContainsKey(itemId))
            {
                var newItem = _node._itemDict[itemId];
                newItem.ShowSelectStatus(false);
                newItem.Show(!playShowAnim);
                //播放新建筑的显示动画
                newItem.LoadGraphic(gameObject);
                
                if (FarmModel.Instance.IsFarmModel())
                {
                    _node.ChangeItemSilence(itemId);
                    FarmModel.Instance.Load(_node);
                }
                
                StoryMovieSubSystem.Instance.Trigger(StoryMovieTrigger.StartNode, _node.Id.ToString());
                
                if (playShowAnim)
                {
                    yield return CoroutineManager.Instance.StartCoroutine( newItem.Graphic.PlayShowAnimation());

                    bool waitCameraFocus = false;
                    DecoManager.Instance.CurrentWorld.FocusDefaultCameraSize(()=>
                    {
                        waitCameraFocus = true;
                    });

                    while (!waitCameraFocus)
                    {
                        yield return null;
                    }
                }
            }
        }
        
        
        internal IEnumerator ReplaceItem(int itemId, bool playShowAnim = true, bool playHideAnim = true, Action changeEnd = null)
        {
            if (!gameObject)
            {
                changeEnd?.Invoke();
                yield break;
            }

            if (_node._itemDict.ContainsKey(itemId))
            {
                var newItem = _node._itemDict[itemId];
                newItem.Hide(false);
            }
            
            //处理老建筑
            var oldItem = _node._currentItem;
            if (oldItem != null)
            {
                oldItem.ShowSelectStatus(false);
                if(playHideAnim)
                    yield return CoroutineManager.Instance.StartCoroutine(oldItem.Graphic.Hide(playHideAnim));
                else
                {
                    CoroutineManager.Instance.StartCoroutine(oldItem.Graphic.Hide(false));
                }
            }

            //处理新建筑
            if (_node._itemDict.ContainsKey(itemId))
            {
                var newItem = _node._itemDict[itemId];
                newItem.ShowSelectStatus(false);
                newItem.Show(!playShowAnim);
                //播放新建筑的显示动画
                newItem.LoadGraphic(gameObject);

                if (playShowAnim)
                {
                    yield return CoroutineManager.Instance.StartCoroutine( newItem.Graphic.PlayShowAnimation());
                }
            }
        }
        
        protected override string PREFAB_PATH { get; }

        protected override void OnLoad()
        {
            gameObject = _parentTransform.gameObject;
            _tipTransform = _parentTransform.Find("Tip");
            _cameraTipTransform = _parentTransform.Find("CameraTip");
            _guideTipTransform = _parentTransform.Find("GuideTip");
            
            if (_guideTipTransform)
            {
                //GuideSubSystem.Instance.RegisterTarget(GuideTargetType.NodeLongPress, _guideTipTransform as RectTransform, moveToTarget:null, _node.Config.id.ToString());
            }
        }

        // public void PlayUnLockAnim(Action action)
        // {
        //     if (_loackbubble == null)
        //     {
        //         action?.Invoke();
        //         return;
        //     }
        //     
        //     _loackbubble.PlayUnLockAnim(() =>
        //     {
        //         RecycleLockBubble();
        //         action?.Invoke();
        //     });
        // }
        //
        // public bool IsLockBubbleNode()
        // {
        //     return _loackbubble != null;
        // }
        
        protected override void OnUnload()
        {
            _broomTransformList?.Clear();
            _broomTransformList = null;
            _tipTransform = null;
            _cameraTipTransform = null;
            _guideTipTransform = null;
        }

        internal void playAction(DecoNodeAction nodeAction)
        {
            switch (nodeAction)
            {
                case DecoNodeAction.OpenDoor:
                    _node._currentItem?.PlayAnimation("OpenDoor");
                    break;
            }
        }

        internal void PlaySpineAnimation(string animName, Action action)
        {
            _node._currentItem?.PlaySpineAnimation(animName, action);
        }
    }
}

public enum DecoNodeAction
{
    OpenDoor = 1,
}