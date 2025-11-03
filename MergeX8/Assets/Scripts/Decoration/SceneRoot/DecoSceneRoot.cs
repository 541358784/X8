using UnityEngine;
using System;
using Deco.World;
using Deco.Graphic;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace Decoration
{
    public class DecoSceneRoot : MonoBehaviour 
    {
        public static DecoSceneRoot Instance;
        public GameObject mRoot;
        public Camera mSceneCamera;
        public Physics2DRaycaster m2DRaycaster;
        
        private void Awake()
        {
            Instance=this;
            mSceneCamera.useOcclusionCulling = false;

            SetPhysics2DRaycasterEnable(false);
        }

        public void SetPhysics2DRaycasterEnable(bool enable)
        {
            m2DRaycaster.enabled = enable;
        }
    }
}