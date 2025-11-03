using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus.UI;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Mosframe;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public partial class UIFishCultureMainController
{
    private bool InitFioshNodeFlag = false;
    public FishNode FishNodeController;

    public void InitFishNode()
    {
        if (InitFioshNodeFlag)
            return;
        InitFioshNodeFlag = true;
        FishNodeController = transform.Find("Root/FishGroup").gameObject.AddComponent<FishNode>();
        FishNodeController.Init(Storage, this);
    }

    public class FishNode : MonoBehaviour
    {
        public StorageFishCulture Storage;
        public UIFishCultureMainController MainUI;
        public Transform RootNode;
        public RawImage CameraImage;

        public void Init(StorageFishCulture storage, UIFishCultureMainController mainUI)
        {
            Storage = storage;
            MainUI = mainUI;
            var rootAsset = ResourcesManager.Instance.LoadResource<GameObject>("Prefabs/Activity/FishCulture/FishRoot");
            RootNode = Instantiate(rootAsset).transform;
            Camera = RootNode.Find("Camera").GetComponent<Camera>();
            CameraImage = transform.Find("Image").GetComponent<RawImage>();
            var viewRect = (transform as RectTransform).rect;
            var maxSize = Math.Max(viewRect.width, viewRect.height);
            CameraImage.rectTransform.SetWidth(maxSize);
            CameraImage.rectTransform.SetHeight(maxSize);
            for (var i = 0; i < Storage.CollectList.Count; i++)
            {
                var assetPath = FishCultureModel.Instance.LevelConfig.Find(a => a.Id == Storage.CollectList[i])
                    .GetFishAssetPath();
                var asset = ResourcesManager.Instance.LoadResource<GameObject>(assetPath);
                Vector3 randomPosition = GetRandomPositionInCameraView();
                var fish = Instantiate(asset, RootNode).transform;
                fish.position = randomPosition;
                FishList.Add(fish);
            }
            EventDispatcher.Instance.AddEvent<EventFishCultureGetNewFish>(OnGetNewFish);
        }

        public void OnGetNewFish(EventFishCultureGetNewFish evt)
        {
            AddNewFish(evt.FishConfig);
        }

        private void OnDestroy()
        {
            Destroy(RootNode.gameObject);
            EventDispatcher.Instance.RemoveEvent<EventFishCultureGetNewFish>(OnGetNewFish);
        }

        public List<Transform> FishList = new List<Transform>();
        public Camera Camera;
        public float swimSpeed = 2f;
        public float rotationSpeed = 1f; // 基础转向速度
        public float avoidanceRotationSpeed = 2f; // 避让时的转向速度
        public float avoidanceRadius = 1.0f; // 触发避让的检测半径
        public float maxViewDistance = 10f; // 鱼的最大视野范围

        void Update()
        {
            foreach (Transform fish in FishList)
            {
                Vector3 avoidanceDirection = CalculateAvoidanceDirection(fish); // 计算避让方向
                Vector3 targetDirection = GetTargetDirection(fish); // 计算目标方向（摄像机视野内）

                // 综合方向：优先避让，其次朝向目标
                Vector3 finalDirection = (avoidanceDirection != Vector3.zero)
                    ? Vector3.Lerp(targetDirection, avoidanceDirection, 0.7f)
                    : targetDirection;

                RotateFish(fish, finalDirection); // 调整朝向
                MoveFish(fish); // 沿当前方向移动
                // KeepFishInView(fish);             // 确保在视野内
            }
        }

        // 计算避让方向（基于附近其他鱼的位置）
        Vector3 CalculateAvoidanceDirection(Transform fish)
        {
            Vector3 avoidanceSum = Vector3.zero;
            int avoidCount = 0;

            foreach (Transform otherFish in FishList)
            {
                if (otherFish != fish)
                {
                    Vector3 toOther = otherFish.position - fish.position;
                    float distance = toOther.magnitude;

                    if (distance < avoidanceRadius)
                    {
                        // 避让方向为远离其他鱼的方向
                        avoidanceSum -= toOther.normalized;
                        avoidCount++;
                    }
                }
            }

            if (avoidCount > 0)
            {
                return avoidanceSum.normalized;
            }

            return Vector3.zero;
        }

        // 获取目标方向（摄像机视野内的随机点）
        Vector3 GetTargetDirection(Transform fish)
        {
            Vector3 targetPosition = GetTargetPosition(fish);
            return (targetPosition - fish.position).normalized;
        }

        public Dictionary<Transform, Vector3> TargetPositionDic = new Dictionary<Transform, Vector3>();
        public Dictionary<Transform, ulong> TargetChangeTimeDic = new Dictionary<Transform, ulong>();

        public Vector3 GetTargetPosition(Transform fish)
        {
            var curTime = APIManager.Instance.GetServerTime();
            if (TargetPositionDic.ContainsKey(fish))
            {
                if (TargetChangeTimeDic[fish] < curTime)
                {
                    TargetChangeTimeDic[fish] = curTime + GetRandomIntervalTime();
                    TargetPositionDic[fish] = GetRandomPositionInCameraView();
                    swimSpeed = Random.Range(1f, 2f);
                }
                else
                {
                    Vector3 toOther = TargetPositionDic[fish] - fish.position;
                    float distance = toOther.magnitude;
                    if (distance < avoidanceRadius)
                    {
                        TargetChangeTimeDic[fish] = curTime + GetRandomIntervalTime();
                        TargetPositionDic[fish] = GetRandomPositionInCameraView();
                        swimSpeed = Random.Range(1f, 2f);
                    }
                }
            }
            else
            {
                TargetChangeTimeDic[fish] = curTime + GetRandomIntervalTime();
                TargetPositionDic[fish] = GetRandomPositionInCameraView();
                swimSpeed = Random.Range(1f, 2f);
            }

            return TargetPositionDic[fish];
        }

        public ulong GetRandomIntervalTime()
        {
            return (ulong)(Random.Range(5f, 8f) * XUtility.Second);
        }

        // 平滑调整鱼的朝向
        void RotateFish(Transform fish, Vector3 targetDirection)
        {
            if (targetDirection != Vector3.zero)
            {
                float currentRotationSpeed = (targetDirection != GetTargetDirection(fish))
                    ? avoidanceRotationSpeed
                    : rotationSpeed;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                fish.rotation = Quaternion.Slerp(
                    fish.rotation,
                    targetRotation,
                    currentRotationSpeed * Time.deltaTime
                );
            }
        }

        // 沿当前方向移动
        void MoveFish(Transform fish)
        {
            fish.position += fish.forward * swimSpeed * Time.deltaTime;
        }

        // 生成摄像机视野内的随机位置
        Vector3 GetRandomPositionInCameraView()
        {
            float zDepth = Random.Range(6f, 8f);
            Vector3 viewportPoint = new Vector3(Random.Range(-0.1f, 1.1f), Random.Range(0.3f, 0.7f), zDepth);
            var pos = Camera.ViewportToWorldPoint(viewportPoint);
            // Debug.LogError("相机坐标" + Camera.transform.position + "屏幕点" + viewportPoint + " 目标点" + pos);
            return pos;
        }
        Vector3 GetCenterPositionInCameraView()
        {
            float zDepth = 6f;
            Vector3 viewportPoint = new Vector3(0.5f,0.5f, zDepth);
            var pos = Camera.ViewportToWorldPoint(viewportPoint);
            // Debug.LogError("相机坐标" + Camera.transform.position + "屏幕点" + viewportPoint + " 目标点" + pos);
            return pos;
        }

        public void AddNewFish(FishCultureRewardConfig fishConfig)
        {
            var assetPath = fishConfig.GetFishAssetPath();
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(assetPath);
            Vector3 randomPosition = GetCenterPositionInCameraView();
            randomPosition.z -= 4f;
            var fish = Instantiate(asset, RootNode).transform;
            fish.position = randomPosition;
            fish.rotation = Quaternion.LookRotation(Vector3.left);
            randomPosition.z += 4f;
            fish.DOMove(randomPosition, 0.5f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                FishList.Add(fish);
            });
        }
    }
}