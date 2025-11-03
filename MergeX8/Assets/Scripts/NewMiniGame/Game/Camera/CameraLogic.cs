using Decoration;
using Framework;
using UnityEngine;

namespace MiniGame.Game.Camera
{
    public class CameraSystem : Singleton<CameraSystem>
    {
        private Vector3 cameraOldPosition;
        private Quaternion cameraOldQuaternion;
        private float cameraOldOrthographicSize;
        private float nearClipPlane;
        private float farClipPlane;
        private bool isActiveSelf;
        
        public void Init()
        {
            isActiveSelf = DecoSceneRoot.Instance.mSceneCamera.gameObject.activeSelf;
            cameraOldPosition = DecoSceneRoot.Instance.mSceneCamera.transform.position;
            cameraOldQuaternion = DecoSceneRoot.Instance.mSceneCamera.transform.rotation;
            cameraOldOrthographicSize = DecoSceneRoot.Instance.mSceneCamera.orthographicSize;
            nearClipPlane = DecoSceneRoot.Instance.mSceneCamera.nearClipPlane;
            farClipPlane = DecoSceneRoot.Instance.mSceneCamera.farClipPlane;
        
            DecoSceneRoot.Instance.mSceneCamera.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            DecoSceneRoot.Instance.mSceneCamera.transform.rotation = Quaternion.identity;
            DecoSceneRoot.Instance.mSceneCamera.nearClipPlane = -20000.0f;
            DecoSceneRoot.Instance.mSceneCamera.farClipPlane = 20000.0f;
            
            DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(true);
        }

        public void Release()
        {
            DecoSceneRoot.Instance.mSceneCamera.transform.position = cameraOldPosition;
            DecoSceneRoot.Instance.mSceneCamera.transform.rotation = cameraOldQuaternion;
            DecoSceneRoot.Instance.mSceneCamera.orthographicSize = cameraOldOrthographicSize;
            DecoSceneRoot.Instance.mSceneCamera.nearClipPlane = nearClipPlane;
            DecoSceneRoot.Instance.mSceneCamera.farClipPlane = farClipPlane;
            
            DecoSceneRoot.Instance.mSceneCamera.gameObject.SetActive(isActiveSelf);
        }
    }
}