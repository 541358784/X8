using UnityEngine;

namespace Framework
{
    public class CameraManager
    {
        private static Camera _mainCamera;

        public static Camera MainCamera
        {
            get
            {
                if (_mainCamera == null)
                {
                    _mainCamera = Camera.main;
                }

                return _mainCamera;
            }
        }

        public static Camera UICamera
        {
            get { return UIRoot.Instance.mUICamera; }
        }

        public static Camera CreateCamera(string name)
        {
            var go = GameObjectFactory.Create(true);
            go.name = name;
            return go.AddComponent<Camera>();
        }

        public static void DestroyCamera(Camera camera)
        {
            if (camera != null)
            {
                GameObjectFactory.Destroy(camera.gameObject);
            }
        }

        public static void ReRenderAll()
        {
            foreach (var c in Camera.allCameras)
            {
                var oldEnabled = c.enabled;
                c.enabled = true;
                c.Render();
                c.enabled = oldEnabled;
            }
        }
    }
}