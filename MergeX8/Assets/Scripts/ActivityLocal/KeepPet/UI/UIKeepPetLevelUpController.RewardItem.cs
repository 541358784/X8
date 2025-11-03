using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIKeepPetLevelUpController
{
    public class LevelItem:MonoBehaviour
    {
        public StorageKeepPet Storage => Controller.Storage;
        public KeepPetLevelConfig CurLevel => Controller.Level;
        public UIKeepPetLevelUpController Controller;
        public KeepPetLevelConfig LevelConfig;
        public ResourceRewardItem ResourceGroup;
        public BuildingRewardItem BuildingGroup;
        private LocalizeTextMeshProUGUI LevelLockText;
        private LocalizeTextMeshProUGUI LevelUnlockText;
        private Transform LevelLock;
        private Transform LevelUnlock;
        
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventKeepPetCollectLevelReward>(OnCollectPetLevelReward);
        }

        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventKeepPetCollectLevelReward>(OnCollectPetLevelReward);
        }

        public void OnCollectPetLevelReward(EventKeepPetCollectLevelReward evt)
        {
            if (evt.Level != LevelConfig)
                return;
            
        }
        public void Init(UIKeepPetLevelUpController controller,KeepPetLevelConfig levelConfig)
        {
            Controller = controller;
            LevelConfig = levelConfig;
            ResourceGroup = transform.Find("ItemReward").gameObject.AddComponent<ResourceRewardItem>();
            ResourceGroup.Init(Controller, LevelConfig);
            BuildingGroup = transform.Find("BuildReward").gameObject.AddComponent<BuildingRewardItem>();
            BuildingGroup.Init(Controller, LevelConfig);
            LevelLockText = transform.Find("LV/Text").GetComponent<LocalizeTextMeshProUGUI>();
            LevelUnlockText = transform.Find("LV_Finish/Text").GetComponent<LocalizeTextMeshProUGUI>();
            LevelLockText.SetText(LevelConfig.Id.ToString());
            LevelUnlockText.SetText(LevelConfig.Id.ToString());
            LevelLock = transform.Find("LV");
            LevelUnlock = transform.Find("LV_Finish");
            var unlockState = Storage.Exp >= LevelConfig.Exp;
            LevelLock.gameObject.SetActive(!unlockState);
            LevelUnlock.gameObject.SetActive(unlockState);
        }
    }

    public class BaseRewardItem : MonoBehaviour
    {
        public StorageKeepPet Storage => Controller.Storage;
        public KeepPetLevelConfig CurLevel => Controller.Level;
        public UIKeepPetLevelUpController Controller;
        public KeepPetLevelConfig LevelConfig;
        public Transform Lock;
        public Transform Normal;
        public Button CollectBtn;
        public Transform Finish;
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventKeepPetCollectLevelReward>(OnCollectPetLevelReward);
        }

        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventKeepPetCollectLevelReward>(OnCollectPetLevelReward);
        }

        public void OnCollectPetLevelReward(EventKeepPetCollectLevelReward evt)
        {
            if (evt.Level != LevelConfig)
                return;
            var collectState = Storage.LevelRewardCollectState.TryGetValue(LevelConfig.Id, out var state) && state;
            CollectBtn.gameObject.SetActive(!collectState);
            Finish.gameObject.SetActive(collectState);
        }
        public void Init(UIKeepPetLevelUpController controller,KeepPetLevelConfig levelConfig)
        {
            Controller = controller;
            LevelConfig = levelConfig;
            Lock = transform.Find("Lock");
            Normal = transform.Find("Normal");
            CollectBtn = transform.Find("Normal/Button").GetComponent<Button>();
            CollectBtn.onClick.AddListener(()=>Controller.OnClickCollectLevelRewardBtn(LevelConfig));
            Finish = transform.Find("Normal/Finish");
            var unlockState = Storage.Exp >= LevelConfig.Exp;
            Lock.gameObject.SetActive(!unlockState);
            Normal.gameObject.SetActive(unlockState);
            var collectState = Storage.LevelRewardCollectState.TryGetValue(LevelConfig.Id, out var state) && state;
            CollectBtn.gameObject.SetActive(!collectState);
            Finish.gameObject.SetActive(collectState);
            InitLocal();
        }
        public virtual void InitLocal()
        {
            
        }
    }

    public class ResourceRewardItem : BaseRewardItem
    {
        private Transform NormalDefaultRewardItem;
        private List<CommonRewardItem> NormalItemList = new List<CommonRewardItem>();
        private Transform LockDefaultRewardItem;
        private List<CommonRewardItem> LockItemList = new List<CommonRewardItem>();
        public override void InitLocal()
        {
            NormalDefaultRewardItem = transform.Find("Normal/ItemGroup/Item");
            NormalDefaultRewardItem.gameObject.SetActive(false);
            LockDefaultRewardItem = transform.Find("Lock/ItemGroup/Item");
            LockDefaultRewardItem.gameObject.SetActive(false);
            foreach (var item in NormalItemList)
            {
                DestroyImmediate(item.gameObject);
            }
            NormalItemList.Clear();
            foreach (var item in LockItemList)
            {
                DestroyImmediate(item.gameObject);
            }
            LockItemList.Clear();
            if (LevelConfig.RewardId != null)
            {
                var rewards = CommonUtils.FormatReward(LevelConfig.RewardId, LevelConfig.RewardNum);
                foreach (var reward in rewards)
                {
                    {
                        var item = Instantiate(NormalDefaultRewardItem, NormalDefaultRewardItem.parent).gameObject.AddComponent<CommonRewardItem>();
                        item.gameObject.SetActive(true);
                        item.Init(reward);
                        NormalItemList.Add(item);
                    }
                    {
                        var item = Instantiate(LockDefaultRewardItem, LockDefaultRewardItem.parent).gameObject.AddComponent<CommonRewardItem>();
                        item.gameObject.SetActive(true);
                        item.Init(reward);
                        LockItemList.Add(item);
                    }
                }
            }
            gameObject.SetActive(LevelConfig.RewardId != null);
        }
    }
    public class BuildingRewardItem : BaseRewardItem
    {
        public Image NormalBuildingIcon;
        public Image LockBuildingIcon;
        public override void InitLocal()
        {
            NormalBuildingIcon = transform.Find("Normal/Reward/Icon").GetComponent<Image>();
            LockBuildingIcon = transform.Find("Lock/Reward/Icon").GetComponent<Image>();
            if (LevelConfig.RewardBuildingItem > 0)
            {
                var buildingItemConfig = KeepPetModel.Instance.BuildingItemConfig[LevelConfig.RewardBuildingItem];
                NormalBuildingIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(buildingItemConfig.IconAtlasName, buildingItemConfig.IconAssetName);
                LockBuildingIcon.sprite = ResourcesManager.Instance.GetSpriteVariant(buildingItemConfig.IconAtlasName, buildingItemConfig.IconAssetName);   
            }
            gameObject.SetActive(LevelConfig.RewardBuildingItem > 0);
        }
    }
}