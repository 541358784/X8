using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using DragonU3DSDK.Asset;
using Framework;
using Deco.Item;
using Deco.Node;
using Deco.Area;
using Decoration;
namespace Deco.World
{
    public partial class DecoWorld
    {
        private const float CAMERA_SCALE_AREA_FAR =1.0f;
        private const float CAMERA_SCALE_NODE = 0.6f * 1.2f;
        private const float CAMERA_SCALE_AREA_NEAR = 1f;
        private const float CAMERA_ACTIVITY_PREVIEW = 1f;

        private void lookAtNode(Node.DecoNode node, bool animate, float focusTime = 1f)
        {
            var lookAtPosition = node.IconTipTransform != null
                ? node.IconTipTransform.position
                : node.GameObject.transform.position;
            Graphic.focus(lookAtPosition, 1f, animate,focusTime);
        }

        private void lookAtNode(Node.DecoNode node, bool animate, float scale, float focusTime = 1f, Action onFinished = null)
        {
            var lookAtPosition = node.IconTipTransform != null
                ? node.IconTipTransform.position
                : node.GameObject.transform.position;
            Graphic.focus(lookAtPosition, scale, animate,focusTime,onFinished);
        }

        private const float _animDestanceLimit = 50;
        private void lookAtNodeBySpeed(Node.DecoNode node, bool animate, float scale, int speed = 50, Action onFinished = null)
        {
            if(node == null)
                return;
            
            var lookAtPosition = node.IconTipTransform != null
                ? node.IconTipTransform.position
                : node.GameObject.transform.position;

            float des = Vector2.Distance(DecoSceneRoot.Instance.mSceneCamera.transform.position, lookAtPosition);
            
            float focusTime =  des/ speed;
            focusTime = Mathf.Min(focusTime, 2);
            Graphic.focus(lookAtPosition, scale, des <= _animDestanceLimit ? animate : false, focusTime,onFinished);
        }
        
        public void FocusNode(int nodeId, Action onFinished = null)
        {
            var node = DecoManager.Instance.FindNode(nodeId);
            if (node != null)
            {
                FocusNode(node, onFinished);
            }
            else
            {
                DragonU3DSDK.DebugUtil.LogError("Focus[" + nodeId + "] error, not found nodeId");
                onFinished?.Invoke();
            }
        }

        public void FocusNode(Node.DecoNode node, Action onFinished)
        {
            if (node == null)
            {
                DragonU3DSDK.DebugUtil.LogError("node == null ,in focusNode function");
                return;
            }
            
            var t = node.IconTipTransform != null
                ? node.IconTipTransform
                : node.GameObject.transform;
            
            //var t = node.CameraTipTransform ?? node.IconTipTransform ?? node.GameObject.transform;

            float configFocus = 0;
            if (node.Config == null)
            {
                DragonU3DSDK.DebugUtil.LogError("node.Config == null ,in focusNode function");
            }
            else
            {
                configFocus = _isPad ? node.Config.cameraFocusPad : node.Config.cameraFocus;
            }
            var scale = configFocus == 0 ? _defaultCameraScaleForNode : configFocus;


            focus(t.position, scale, true, 1f, onFinished);
        }

        private void focusNodeList(List<DecoNode> nodeList, float cameraScale, Action onFinished)
        {
            var focusPos = Vector3.zero;

            foreach (var node in nodeList)
            {
                var t = node.CameraTipTransform ?? node.IconTipTransform ?? node.GameObject.transform;
                focusPos += t.position;
            }

            focusPos /= nodeList.Count;
            
            focus(focusPos, cameraScale, true, 1f, onFinished);
        }

        private void focusActivityArea(int areaId, bool animate, Action onFinished, Transform focustTransform)
        {
            
            // var area = MyMain.myGame.DecorationMgr.FindArea(areaId);
            // if (focustTransform == null)
            // {
            //     focustTransform = area.Graphic._festivalPreviewTransform ?? area.Graphic._loadingTip;
            // }
            // focus(focustTransform.position, area.Config.previewScale, animate, 0.5f, onFinished);
            var area = DecoManager.Instance.FindArea(areaId);
            if (focustTransform == null)
            {
                focustTransform = area.Graphic._festivalPreviewTransform ?? area.Graphic._loadingTip;
            }
            focus(focustTransform.position, area.Config.previewScale, animate, 0.5f, onFinished);
        }

        public void PreviewModeArea(int areaId, bool animate)
        {
            // var area = MyMain.myGame.DecorationMgr.FindArea(areaId);
            // var focustTransform = area.Graphic._cameraPreviewTransform ?? area.Graphic._loadingTip;

            // focus(focustTransform.position, area.Config.previewScale, animate, 0.5f, null);
            var area = DecoManager.Instance.FindArea(areaId);
            var focustTransform = area.Graphic._cameraPreviewTransform ?? area.Graphic._loadingTip;

            focus(focustTransform.position, area.Config.previewScale, animate, 0.5f, null);
        }

        private void focusAreaOut(int areaId, float scale, bool animate, float focusTime, Action onFinished)
        {
            // var area = MyMain.myGame.DecorationMgr.FindArea(areaId);
            // var focustTransform = area.Graphic._cameraOutTransform ?? area.Graphic._loadingTip;
            // focus(focustTransform.position, scale, animate, focusTime, onFinished);
            var area = DecoManager.Instance.FindArea(areaId);
            if(area==null){
                onFinished?.Invoke();
                return;
            }
            var focustTransform = area.Graphic._cameraOutTransform ?? area.Graphic._loadingTip;
            focus(focustTransform.position, scale, animate, focusTime, onFinished);
        }

        private void focusAreaIn(int areaId, float scale, bool animate, float focusTime, Action onFinished)
        {
            // var area = MyMain.myGame.DecorationMgr.FindArea(areaId);
            // var focustTransform = area.Graphic._cameraInTransform ?? area.Graphic._loadingTip;
            // focus(focustTransform.position, scale, animate, focusTime, onFinished);
            var area =DecoManager.Instance.FindArea(areaId);
            if(area==null){
                onFinished?.Invoke();
                return;
            }
            var focustTransform = area.Graphic._cameraInTransform ?? area.Graphic._loadingTip;
            focus(focustTransform.position, scale, animate, focusTime, onFinished);
        }

        private void focus(Vector3 position, float scale, bool animate, float focusTime, Action onFinished)
        {
            Graphic.focus(position, scale, animate, focusTime, onFinished);
        }
        
        public void FocusDefaultCameraSize(Action OnFinish)
        {
            Graphic.FocusDefaultCameraSize(OnFinish);
        }

        public void FocusArea(int areaId, Action onFinish = null)
        {
            var area = DecoManager.Instance.FindArea(areaId);
            if(area == null)
                return;
            
            var targetPos = area.Graphic._cameraPreviewTransform.position;
            focus(targetPos, area._data._config.previewScale, true, 1f, onFinish);
        }
    }
}