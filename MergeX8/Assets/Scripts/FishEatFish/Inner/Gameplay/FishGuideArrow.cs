using DragonU3DSDK.Asset;
using Mosframe;
using UnityEngine;

namespace FishEatFishSpace
{
    public class FishGuideArrow:MonoBehaviour
    {
        private Enemy TargetFish;
        public void Init(Enemy targetFish)
        {
            TargetFish = targetFish;
            Update();
        }

        void Update()
        {
            if (!TargetFish)
            {
                Release();
                return;
            }
            var targetPosition = TargetFish.transform.position;
            targetPosition.z = -1;
            transform.position = targetPosition;
        }

        public void Release()
        {
            Destroy(this.gameObject);
        }
        private static GameObject guideHandResources;
        public static FishGuideArrow CreateGuide(Enemy targetFish,Transform _guideHandLayer)
        {
            if (guideHandResources == null)
                guideHandResources = ResourcesManager.Instance.LoadResource<GameObject>("FishEatFish/FishDynamic/Prefabs/UI/GuideArrow",addToCache:false);
            var handObj = GameObject.Instantiate(guideHandResources, _guideHandLayer).AddComponent<FishGuideArrow>();
            handObj.Init(targetFish);
            return handObj;
        }
    }
}