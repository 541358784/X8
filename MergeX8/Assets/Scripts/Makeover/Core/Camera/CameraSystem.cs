using Framework;
using UnityEngine;

namespace Makeover
{
    public class CameraSystem : GlobalSystem<CameraSystem>, IInitable
    {
        private Vector3 cameraOldPosition;
        private Quaternion cameraOldQuaternion;
        private float cameraOldOrthographicSize;
        private float nearClipPlane;
        private float farClipPlane;
        
        public void Init()
        {
            cameraOldPosition = CameraManager.MainCamera.transform.position;
            cameraOldQuaternion = CameraManager.MainCamera.transform.rotation;
            cameraOldOrthographicSize = CameraManager.MainCamera.orthographicSize;
            nearClipPlane = CameraManager.MainCamera.nearClipPlane;
            farClipPlane = CameraManager.MainCamera.farClipPlane;
        
            CameraManager.MainCamera.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            CameraManager.MainCamera.transform.rotation = Quaternion.identity;
            CameraManager.MainCamera.nearClipPlane = -20000.0f;
            CameraManager.MainCamera.farClipPlane = 20000.0f;
            
            float designWidth = 729.0f; //768.0f * 0.95f(预制件上有缩放);
            CameraManager.MainCamera.orthographicSize = designWidth / 200.0f / CameraManager.MainCamera.aspect;
        }

        public void Release()
        {
            CameraManager.MainCamera.transform.position = cameraOldPosition;
            CameraManager.MainCamera.transform.rotation = cameraOldQuaternion;
            CameraManager.MainCamera.orthographicSize = cameraOldOrthographicSize;
            CameraManager.MainCamera.nearClipPlane = nearClipPlane;
            CameraManager.MainCamera.farClipPlane = farClipPlane;
        }
    }
}