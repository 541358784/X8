using DragonU3DSDK.Asset;
using Framework;
using UnityEngine;

namespace TMatch
{


    public class TMatchEnvSystem : GlobalSystem<TMatchEnvSystem>, IInitable
    {
        public Vector3 SceneBoundMin = Vector3.zero;
        public Vector3 SceneBoundMax = Vector3.zero;
        public Vector3 SceneRandomPosMin = Vector3.zero;
        public Vector3 SceneRandomPosMax = Vector3.zero;

        private Vector3 cameraOldPosition;
        private Quaternion cameraOldQuaternion;
        private float cameraOldOrthographicSize;
        private float nearClipPlane;
        private float farClipPlane;

        public GameObject Root;
        public GameObject ItemRoot;

        public Vector3 CollectorTopPos;

        public Vector3[] CollectorPos;

        private GameObject sceneInstant;

        public void Init()
        {
            Root = new GameObject("TMatchEnvRoot");

            ItemRoot = new GameObject("Item");
            ItemRoot.transform.parent = Root.transform;

            //camera
            {
                cameraOldPosition = CameraManager.MainCamera.transform.position;
                cameraOldQuaternion = CameraManager.MainCamera.transform.rotation;
                cameraOldOrthographicSize = CameraManager.MainCamera.orthographicSize;
                nearClipPlane = CameraManager.MainCamera.nearClipPlane;
                farClipPlane = CameraManager.MainCamera.farClipPlane;

                CameraManager.MainCamera.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
                CameraManager.MainCamera.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                CameraManager.MainCamera.orthographicSize = 6.8f;
                CameraManager.MainCamera.nearClipPlane = 0.5f;
                CameraManager.MainCamera.farClipPlane = 20.0f;
            }
            // //camera
            // {
            //     CameraManager.MainCamera.transform.position = new Vector3(0.0f, 10.0f, 0f);
            //     CameraManager.MainCamera.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
            // }
            //3d scene
            {
                GameObject match3DScenePrefab =
                    ResourcesManager.Instance.LoadResource<GameObject>("TMatch/TMatch/Mess/Match3DScene/Match3DScene",
                        assetDeepPath: "Match3DScene");
                sceneInstant = GameObject.Instantiate(match3DScenePrefab, Root.transform);

                Vector3 left = UIRoot.Instance.mRootCanvas.transform.Find("EnvGroup/Left").position;
                Vector3 leftScreenPoint = CameraManager.UICamera.WorldToScreenPoint(left);
                Vector3 leftWorldPos = CameraManager.MainCamera.ScreenToWorldPoint(leftScreenPoint);
                sceneInstant.transform.Find("AirWalls/Left").position = new Vector3(leftWorldPos.x, 0.0f, 0.0f);
                sceneInstant.transform.Find("AirWalls/Right").position = new Vector3(-leftWorldPos.x, 0.0f, 0.0f);

                if (global::CommonUtils.IsLE_16_10())
                {
                    Vector3 up = UIRoot.Instance.mRootCanvas.transform.Find("EnvGroup/Up_Pad").position;
                    Vector3 upScreenPoint = CameraManager.UICamera.WorldToScreenPoint(up);
                    Vector3 upWorldPos = CameraManager.MainCamera.ScreenToWorldPoint(upScreenPoint);
                    sceneInstant.transform.Find("AirWalls/Top").position = new Vector3(0.0f, 0.0f, upWorldPos.z);
                    
                    Vector3 bottom = UIRoot.Instance.mRootCanvas.transform.Find("EnvGroup/Bottom_Pad").position;
                    Vector3 bottomScreenPoint = CameraManager.UICamera.WorldToScreenPoint(bottom);
                    Vector3 bottomWorldPos = CameraManager.MainCamera.ScreenToWorldPoint(bottomScreenPoint);
                    sceneInstant.transform.Find("AirWalls/Bottom").position = new Vector3(0.0f, 0.0f, bottomWorldPos.z);
                }
                else
                {
                    Vector3 up = UIRoot.Instance.mRootCanvas.transform.Find("EnvGroup/Up").position;
                    Vector3 upScreenPoint = CameraManager.UICamera.WorldToScreenPoint(up);
                    Vector3 upWorldPos = CameraManager.MainCamera.ScreenToWorldPoint(upScreenPoint);
                    sceneInstant.transform.Find("AirWalls/Top").position = new Vector3(0.0f, 0.0f, upWorldPos.z);   
                }
            }
            //camera
            {
                if (global::CommonUtils.IsLE_16_10())
                {
                    CameraManager.MainCamera.transform.position = new Vector3(0.0f, 10.0f, 0f);
                }
                else
                {
                    CameraManager.MainCamera.transform.position = new Vector3(0.0f, 10.0f, -4.5f);
                    CameraManager.MainCamera.transform.rotation = Quaternion.Euler(70.0f, 0.0f, 0.0f);   
                }
            }

            //Collect
            {
                CollectorPos = new Vector3[7];
                Vector3 CollectorCenterPos = sceneInstant.transform.Find($"AirWalls/Bottom/Collect/Pos").position;
                for (int i = 1; i <= CollectorPos.Length; i++)
                {
                    CollectorPos[i - 1] = CollectorCenterPos + new Vector3(0.825f, 0.0f, 0.0f) * (i - 4);
                }

                CollectorTopPos = Root.transform.Find($"Match3DScene(Clone)/AirWalls/Bottom/Collect/Top").position;
            }

            //bound
            {
                float padding = 0.25f;
                SceneBoundMin.y = 0.5f;
                BoxCollider ceilingCollider = sceneInstant.transform.Find("AirCeiling").GetComponent<BoxCollider>();
                SceneBoundMax.y = 8 - ceilingCollider.size.y * 0.5f;
                BoxCollider wallLeftCollider = sceneInstant.transform.Find("AirWalls/Left").GetComponent<BoxCollider>();
                SceneBoundMin.x = wallLeftCollider.transform.position.x + wallLeftCollider.size.x * 0.5f;
                BoxCollider wallRightCollider =
                    sceneInstant.transform.Find("AirWalls/Right").GetComponent<BoxCollider>();
                SceneBoundMax.x = wallRightCollider.transform.position.x - wallRightCollider.size.x * 0.5f;
                BoxCollider wallBottomCollider =
                    sceneInstant.transform.Find("AirWalls/Bottom/Collect").GetComponent<BoxCollider>();
                SceneBoundMin.z = wallBottomCollider.transform.position.z + wallBottomCollider.size.z * 0.5f;
                BoxCollider wallTopCollider = sceneInstant.transform.Find("AirWalls/Top").GetComponent<BoxCollider>();
                SceneBoundMax.z = wallTopCollider.transform.position.z - wallTopCollider.size.z * 0.5f;

                SceneRandomPosMin.x = SceneBoundMin.x + padding;
                SceneRandomPosMax.x = SceneBoundMax.x - padding;
                SceneRandomPosMin.y = SceneBoundMin.y + padding;
                SceneRandomPosMax.y = SceneBoundMax.y - padding;
                SceneRandomPosMin.z = SceneBoundMin.z + padding;
                SceneRandomPosMax.z = sceneInstant.transform.Find("AirWalls/Top/Cross").position.z - padding;
            }
            
            global::UIRoot.Instance.mMainLight.SetActive(true);
        }

        public void Release()
        {
            GameObject.Destroy(Root);

            CollectorPos = null;
            GameObject.Destroy(sceneInstant);

            CameraManager.MainCamera.transform.position = cameraOldPosition;
            CameraManager.MainCamera.transform.rotation = cameraOldQuaternion;
            CameraManager.MainCamera.orthographicSize = cameraOldOrthographicSize;
            CameraManager.MainCamera.nearClipPlane = nearClipPlane;
            CameraManager.MainCamera.farClipPlane = farClipPlane;
            
            global::UIRoot.Instance.mMainLight.SetActive(false);
        }
    }
}