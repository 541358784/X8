using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using DG.Tweening;
using UnityEngine.Tilemaps;
using System;
using Deco.Graphic;
using Deco.World;
using Deco.Node;
using DragonU3DSDK;
using Decoration;
using Framework;
using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
namespace Deco.Item
{
    using TableItem=Decoration.TableItem;
    public class DecoItem : DecoGraphicHost<DecoItemGraphic>
    {
        public enum Status
        {
            Lock = 0, //锁定中
            Unlock, //已解锁，可购买
            Owned, //已拥有
        }

        internal DecoItemData _data;

        public delegate bool AttachUpdate();

        internal Node.DecoNode _node = null;
        internal bool _isDefault;

        public int Id => _data._config.id;
        public TableItem Config => _data._config;
        public StorageItem Storage => _data._storage;
        public Node.DecoNode Node => _node;
        public GameObject GameObject => Graphic.gameObject;
        public Transform TipTransform => Graphic._tipTransform;
        public Transform CameraTipTransform => Graphic._cameraTipTransform;

        public bool IsOwned
        {
            get => _data.isOwned;
        }
        // public bool IsRendering { get => _graphic.isRendering; }

        public DecoItem(TableItem config, StorageItem storage, Node.DecoNode node, bool isDefault)
        {
            _node = node;
            _data = new DecoItemData(this, config, storage);
            Graphic = new DecoItemGraphic(this);
            _isDefault = isDefault;

            World.DecoWorld.ItemLib[config.id] = this;
            var activityType = _data.GetActivityType();
            if (activityType != EActivityType.NONE)
            {
                if (DecoWorld.ActivityItemLib.ContainsKey(activityType))
                {
                    DecoWorld.ActivityItemLib[activityType].Add(this);
                }
                else
                {
                    DecoWorld.ActivityItemLib.Add(activityType, new List<DecoItem>() { this });
                }
            }
        }

        public override void UnloadGraphic()
        {
            Graphic.Unload();
        }

        public override void LoadGraphic(GameObject parentObj)
        {
            Graphic.Load(parentObj?.transform);
        }

        public void Show(bool playNormalAnim = false)
        {
            Graphic.Show(playNormalAnim);
        }

        public void Hide(bool playAnim = true)
        {
            CoroutineManager.Instance.StartCoroutine(Graphic.Hide(playAnim));
        }

        public void Buy()
        {
            _data.buy();

            if (_data._config.price > 0)
            {
                //DragonPlus.GameBIManager.SendDecoEvent_PurcharseItem(_data._config.id);
            }
        }
        public void Lock()
        {
            _data.Lock();
        }

        /// <summary>
        /// 显示选中状态
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowSelectStatus(bool isShow)
        {
            Graphic.ShowSelectStatus(isShow);
        }

        public void PlayAnimation(string animationName)
        {
            Graphic.PlayAnimation(animationName);
        }

        public void PlaySpineAnimation(string animName, Action action)
        {
            Graphic.PlaySpineAnimation(animName, action);
        }
        
        
        public DecoItemTouchResult TouchTest(Vector2 screenPos)
        {
            return Graphic.TouchTest(screenPos);
        }

        public void MarkGray(bool gray, bool anim)
        {
            Graphic.MarkGray(gray, anim);
        }

        public void DEBUG_SET_LOCKED()
        {
            if (_node.IsOwned)
            {
                _node.ChangeItem(_node._data._config.itemList[0]);
                _data.DEBUG_SET_LOCKED();
            }
        }
        
        public override void AsyncLoadGraphic(GameObject parentObj, bool isPreview, Action onFinished)
        {
            CoroutineManager.Instance.StartCoroutine(Graphic.AsyncLoad(parentObj?.transform, onFinished, isPreview));
        }
    }
}