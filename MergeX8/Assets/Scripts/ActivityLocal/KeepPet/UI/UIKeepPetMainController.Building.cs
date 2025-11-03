using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public partial class UIKeepPetMainController
{
    private Dictionary<int, KeepPetBuildingHangPointConfig> HangPointConfig =>
        KeepPetModel.Instance.BuildingHangPointConfig;

    private Dictionary<int, KeepPetBuildingItemConfig> BuildingItemConfig => KeepPetModel.Instance.BuildingItemConfig;
    private Dictionary<int, Transform> HangPointDic = new Dictionary<int, Transform>();
    private Button EnterBuildingBtn;
    private bool IsInBuilding;
    private Dictionary<GameObject, bool> HideObjectState = new Dictionary<GameObject, bool>();
    private Transform BuildingButtonLayer;
    private Transform DefaultBuildingBtn;
    private SelectView SelectBuildingItemView;
    private Dictionary<int, HangPointBtn> HangPointBtnDic = new Dictionary<int, HangPointBtn>();
    public void InitBuilding()
    {
        foreach (var activeState in Storage.BuildingActiveState)
        {
            var hangPointConfig = HangPointConfig[activeState.Key];
            var buildingItemConfig = BuildingItemConfig[activeState.Value];
            var asset = ResourcesManager.Instance.LoadResource<GameObject>(buildingItemConfig.AssetPath);
            var hangPoint = transform.Find(hangPointConfig.HangPointPath);
            var buildItem = Instantiate(asset, hangPoint);
            HangPointDic.Add(activeState.Key,buildItem.transform);
            if (buildItem.name.Contains("DogSkin"))
            {
                SetDogSkin(buildItem.transform.GetChild(0).name);
            }
        }

        IsInBuilding = false;
        EnterBuildingBtn = transform.Find("Root/BGButton").GetComponent<Button>();
        EnterBuildingBtn.onClick.AddListener(() =>
        {
            if (IsInBuilding)
            {
                IsInBuilding = false;
                var keyList = HideObjectState.Keys.ToList();
                foreach (var obj in keyList)
                {
                    obj.gameObject.SetActive(HideObjectState[obj]);
                }
                BuildingButtonLayer.gameObject.SetActive(false);
                SelectBuildingItemView.gameObject.SetActive(false);
            }
            else
            {
                IsInBuilding = true;
                var keyList = HideObjectState.Keys.ToList();
                foreach (var obj in keyList)
                {
                    HideObjectState[obj] = obj.activeSelf;
                    obj.gameObject.SetActive(false);
                }
                BuildingButtonLayer.gameObject.SetActive(true);
                foreach (var pair in HangPointBtnDic)
                {
                    var hangPoint = HangPointConfig[pair.Key];
                    var itemCount = 0;
                    foreach (var itemId in hangPoint.SelectItemList)
                    {
                        if (Storage.BuildingCollectState.TryGetValue(itemId, out var state) && state)
                        {
                            itemCount++;
                        }
                    }
                    pair.Value.gameObject.SetActive(itemCount > 0);
                    if (KeepPetModel.Instance.CurState.Enum == KeepPetStateEnum.Searching &&
                        hangPoint.Id == 109)
                    {
                        pair.Value.gameObject.SetActive(false);
                    }
                }
            }
        });
        HideObjectState.Add(transform.Find("Root/ButtonRight").gameObject,false);
        HideObjectState.Add(transform.Find("Root/ButtonLeft").gameObject,false);
        HideObjectState.Add(transform.Find("Root/ButtonBottom").gameObject,false);
        HideObjectState.Add(transform.Find("Root/Patrol").gameObject,false);
        HideObjectState.Add(transform.Find("Root/Slider").gameObject,false);
        HideObjectState.Add(transform.Find("Root/ButtonHelp").gameObject,false);
        HideObjectState.Add(transform.Find("Root/ButtonClose").gameObject,false);
        BuildingButtonLayer = transform.Find("Root/BuildingButtonLayer");
        BuildingButtonLayer.gameObject.SetActive(IsInBuilding);
        DefaultBuildingBtn = transform.Find("Root/BuildingButtonLayer/BuildingButton");
        DefaultBuildingBtn.gameObject.SetActive(false);
        foreach (var pair in HangPointConfig)
        {
            var hangPoint = transform.Find(pair.Value.HangPointPath);
            var btnPoint = hangPoint.Find("ButtonPoint");
            var hangPointBtn = Instantiate(DefaultBuildingBtn, DefaultBuildingBtn.parent).gameObject.AddComponent<HangPointBtn>();
            hangPointBtn.transform.position = btnPoint.position;
            hangPointBtn.Init(pair.Value,this);
            HangPointBtnDic.Add(pair.Key,hangPointBtn);
        }

        SelectBuildingItemView = transform.Find("Root/SelectUI/SelectFurnituresGroup").gameObject.AddComponent<SelectView>();
        SelectBuildingItemView.transform.parent.gameObject.SetActive(true);
        SelectBuildingItemView.Init(this);
        SelectBuildingItemView.gameObject.SetActive(false);
    }

    public void SetHideActive(GameObject obj,bool state)
    {
        if (!HideObjectState.ContainsKey(obj))
        {
            obj.gameObject.SetActive(state);
            return;
        }
        if (!IsInBuilding)
        {
            obj.gameObject.SetActive(state);
            return;
        }
        HideObjectState[obj] = state;
    }

    private List<Animator> BuildingAnimatorList = new List<Animator>();
    public void OnChangeBuilding(EventKeepPetBuildingActiveChange evt)
    {
        if (HangPointDic.TryGetValue(evt.HangPoint.Id, out var curItem))
        {
            DestroyImmediate(curItem.gameObject);
            HangPointDic.Remove(evt.HangPoint.Id);
        }
        var asset = ResourcesManager.Instance.LoadResource<GameObject>(evt.ItemNew.AssetPath);
        var hangPoint = transform.Find(evt.HangPoint.HangPointPath);
        var buildItem = Instantiate(asset, hangPoint);
        HangPointDic.Add(evt.HangPoint.Id,buildItem.transform);
        if (buildItem.name.Contains("DogSkin"))
        {
            Storage.SkinName = evt.ItemNew.SkinName;
            SetDogSkin(evt.ItemNew.SkinName);
            EventDispatcher.Instance.SendEventImmediately(new EventKeepPetChangeSkin());
        }
        if (buildItem.TryGetComponent<Animator>(out var animator))
        {
            animator.gameObject.SetActive(false);
            BuildingAnimatorList.Add(animator);
        }
    }

    public void PlayAllBuildingAppearAnimation()
    {
        foreach (var animator in BuildingAnimatorList)
        {
            animator.gameObject.SetActive(true);
            animator.PlayAnimation("Appear");
        }
        BuildingAnimatorList.Clear();
    }

    public class HangPointBtn : MonoBehaviour
    {
        public KeepPetBuildingHangPointConfig Config;
        public UIKeepPetMainController Controller;
        private Button Btn;
        public void Init(KeepPetBuildingHangPointConfig config,UIKeepPetMainController controller)
        {
            Config = config;
            Controller = controller;
            Btn = transform.GetComponent<Button>();
            Btn.onClick.AddListener(() =>
            {
                Controller.SelectBuildingItemView.ShowView(Config);
            });
        }
    }

    public class SelectView : MonoBehaviour
    {
        public class SelectItem : MonoBehaviour
        {
            public KeepPetBuildingItemConfig Config;
            private Image Icon;
            private Transform Arrow;
            private Transform SelectState;
            private Button Btn;
            private SelectView SelectViewController;
            public void Init(KeepPetBuildingItemConfig config,SelectView controller)
            {
                Config = config;
                SelectViewController = controller;
                Icon = transform.Find("Icon").GetComponent<Image>();
                Icon.sprite =  ResourcesManager.Instance.GetSpriteVariant(config.IconAtlasName, config.IconSmallAssetName);
                Arrow = transform.Find("Arrow");
                Arrow.gameObject.SetActive(false);
                SelectState = transform.Find("Check");
                SelectState.gameObject.SetActive(false);
                Btn = transform.GetComponent<Button>();
                Btn.onClick.AddListener(() =>
                {
                    SelectViewController.OnSelectItem(this);
                });
            }

            public void SetSelectState(bool select)
            {
                SelectState.gameObject.SetActive(select);
            }
        }
        public KeepPetBuildingHangPointConfig Config;
        private Button CloseBtn;
        private Button SureBtn;
        private Transform DefaultItem;
        private List<SelectItem> ItemList = new List<SelectItem>();
        private UIKeepPetMainController Controller;
        public void Init(UIKeepPetMainController controller)
        {
            Controller = controller;
            SureBtn = transform.Find("SureSelectButton").GetComponent<Button>();
            SureBtn.onClick.AddListener(OnClickSureBtn);
            CloseBtn = transform.Find("CloseSelectButton").GetComponent<Button>();
            CloseBtn.onClick.AddListener(OnClickCloseBtn);
            DefaultItem = transform.Find("FurnituresScrollRect/FurnituresLayout/FurnitureItem");
            DefaultItem.gameObject.SetActive(false);
        }

        public void ShowView(KeepPetBuildingHangPointConfig config)
        {
            gameObject.SetActive(true);
            foreach (var item in ItemList)
            {
                DestroyImmediate(item.gameObject);
            }
            ItemList.Clear();
            CurSelectItem = null;
            Config = config;
            var isUsingItem = Controller.Storage.BuildingActiveState.TryGetValue(Config.Id, out var usingItem);
            foreach (var item in Config.SelectItemList)
            {
                if (!Controller.Storage.BuildingCollectState.TryGetValue(item,out var state) || !state)
                    continue;
                var itemConfig = KeepPetModel.Instance.BuildingItemConfig[item];
                var itemNode = Instantiate(DefaultItem, DefaultItem.parent).gameObject.AddComponent<SelectItem>();
                itemNode.gameObject.SetActive(true);
                itemNode.Init(itemConfig,this);
                itemNode.SetSelectState(isUsingItem && usingItem == item);
                if (isUsingItem && usingItem == item)
                {
                    CurSelectItem = itemNode;
                }
                ItemList.Add(itemNode);
            }
        }

        private SelectItem CurSelectItem;
        public void OnSelectItem(SelectItem item)
        {
            CurSelectItem = item;
            foreach (var tempItem in ItemList)
            {
                tempItem.SetSelectState(CurSelectItem == tempItem);
            }
        }

        public void OnClickCloseBtn()
        {
            gameObject.SetActive(false);
        }
        public void OnClickSureBtn()
        {
            if (CurSelectItem == null)
                return;
            KeepPetModel.Instance.ChangeActiveBuildingItem(CurSelectItem.Config.Id);
            Controller.PlayAllBuildingAppearAnimation();
            gameObject.SetActive(false);
        }
    }
}