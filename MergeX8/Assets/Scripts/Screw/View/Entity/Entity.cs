using System;
using System.Reflection;
using Screw.UI;
using Screw.UIBinder;
using UnityEngine;

namespace Screw
{
    public class Entity
    {
        protected Transform root;
        
        protected ScrewGameContext context;
        public void Bind(Transform inRoot, ScrewGameContext inScrewGameContext)
        {
            root = inRoot;
            context = inScrewGameContext;
            
            ComponentBinderUI.BindingComponent(this, inRoot);
        }

        public virtual void AfterBind()
        {
            
        }

        public Transform GetRoot()
        {
            return root;
        }

        #region Update

        protected bool updateEnabled = false;

        /// <summary>
        /// 0:0.9f ,1:0.5f, 2:every frame
        /// </summary>
        /// <param name="updateInterval"></param>
        public void EnableUpdate(int updateInterval = 0)
        {
            if (updateEnabled)
            {
                DisableUpdate();
            } 
            if (updateInterval == 0)
                UpdateScheduler.Instance.HookSecondUpdate(OnUpdate);
            else if (updateInterval == 1)
                UpdateScheduler.Instance.HookHalfSecondUpdate(OnUpdate);
            else
                UpdateScheduler.Instance.HookUpdate(OnUpdate);
            updateEnabled = true;
        }
        
        public void DisableUpdate()
        {
            if (updateEnabled)
            {
                UpdateScheduler.Instance.UnhookUpdate(OnUpdate);
                updateEnabled = false;
            }
        }

        /// <summary>
        /// 窗口更新
        /// </summary>
        public virtual void OnUpdate()
        {
            
        }
        
        #endregion

        public virtual void Destroy()
        {
            DisableUpdate();
            if(root != null)
                GameObject.DestroyImmediate(root.gameObject);
        }
        
        public T LoadEntity<T>(Transform root, ScrewGameContext screwGameContext) where T : Entity
        {
            var viewAssetAttribute = typeof(T).GetCustomAttribute<ViewAssetAttribute>();
            var gameObject = AssetModule.Instance.LoadAsset<GameObject>(viewAssetAttribute.AssetName, root);

            var holder = Activator.CreateInstance<T>();
            holder.Bind(gameObject.transform, screwGameContext);
            holder.AfterBind();
            return holder;
        }
    }
}