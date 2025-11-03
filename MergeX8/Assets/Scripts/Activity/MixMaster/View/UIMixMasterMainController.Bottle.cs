using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using Gameplay;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMixMasterMainController
{
    public class Bottle : MonoBehaviour
    {
        public UIMixMasterMainController Controller;
        private int BottleIndex;
        private StorageMixMaster Storage;
        private bool IsFull => Storage.Desktop.ContainsKey(BottleIndex);
        public StorageResData Content => IsFull ? Storage.Desktop[BottleIndex] : null;

        public MixMasterMaterialConfig MaterialConfig => Content == null
            ? null
            : MixMasterModel.Instance.MaterialConfig.Find(a => a.Id == Content.Id);
        private StorageResData LastContent;
        public SelectView SelectView;
        private Button Add;
        public Button Replace;
        public Canvas Canvas;

        public void Init(StorageMixMaster storage,int bottleIndex,UIMixMasterMainController controller)
        {
            Controller = controller;
            Storage = storage;
            BottleIndex = bottleIndex;
            LastContent = Content;
            Canvas = transform.gameObject.GetComponent<Canvas>();
            if (!Canvas)
                Canvas = transform.gameObject.AddComponent<Canvas>();
            Canvas.overrideSorting = true;
            Canvas.sortingOrder = Controller.canvas.sortingOrder+4;
            transform.gameObject.AddComponent<GraphicRaycaster>();
            SelectView = transform.Find("Open").gameObject.AddComponent<SelectView>();
            SelectView.Init(Storage,BottleIndex,this);
            Add = transform.Find("Add").GetComponent<Button>();
            Add.onClick.AddListener(() =>
            {
                if (Controller.Mixing)
                    return;
                SelectView.gameObject.SetActive(!SelectView.gameObject.activeSelf);
                if (SelectView.gameObject.activeSelf)
                {
                    SelectView.Open();
                    foreach (var pair in Controller.BottleDic)
                    {
                        if (pair.Value != this)
                        {
                            pair.Value.SelectView.gameObject.SetActive(false);
                        }
                    }
                }
            });
            Replace = transform.Find("Replace").GetComponent<Button>();
            Replace.onClick.AddListener(() =>
            {
                if (Controller.Mixing)
                    return;
                SelectView.gameObject.SetActive(!SelectView.gameObject.activeSelf);
                if (SelectView.gameObject.activeSelf)
                {
                    SelectView.Open();
                    foreach (var pair in Controller.BottleDic)
                    {
                        if (pair.Value != this)
                        {
                            pair.Value.SelectView.gameObject.SetActive(false);
                        }
                    }
                }
            });
            Add.gameObject.SetActive(!IsFull);
            Replace.gameObject.SetActive(IsFull);
            InitPrivate();
        }
        
        public void UpdateView()
        {
            if (Content != LastContent)
            {
                SelectView.gameObject.SetActive(false);
                LastContent = Content;
                Add.gameObject.SetActive(!IsFull);
                Replace.gameObject.SetActive(IsFull);
                UpdateViewPrivate();
                Controller.UpdateDesktop();
            }
        }
        public virtual void InitPrivate()
        {
            
        }
        public virtual void UpdateViewPrivate()
        {
            
        }
        public virtual Task PerformMix()
        {
            // SelectView.gameObject.SetActive(false);
            return Task.CompletedTask;
        }
    }

    public class BottleLiquid : Bottle
    {
        private Image IconBG;
        private Image Icon;
        private Sprite BGSprite => Content == null ? null : ResourcesManager.Instance.GetSpriteVariant("MixMasterAtlas", 
            (MaterialConfig.Count == 1 ?"UIMixMasterMain16":"UIMixMasterMain15"));
        public SkeletonGraphic Spine;
        public Transform MoveNode;
        private Vector3 InitLocalPosition;
        private MaterialAnimationState AnimationState;

        public override void InitPrivate()
        {
            base.InitPrivate();
            IconBG = transform.Find("IconBG").GetComponent<Image>();
            Icon = transform.Find("IconBG/Icon").GetComponent<Image>();
            Spine = transform.Find("Material/Spine").GetComponent<SkeletonGraphic>();
            MoveNode = transform;
            InitLocalPosition = MoveNode.localPosition;
            if (Content == null)
            {
                IconBG.gameObject.SetActive(false);
                AnimationState = null;
            }
            else
            {
                IconBG.gameObject.SetActive(true);
                IconBG.sprite = BGSprite;
                Icon.sprite = UserData.GetResourceIcon(MaterialConfig.Id, UserData.ResourceSubType.Big);
                AnimationState = MaterialAnimationState.GetMaterialAnimationState(MaterialConfig.Id);
                var skeletonAsset = ResourcesManager.Instance.LoadResource<SkeletonDataAsset>(AnimationState.SpineAssetPath);
                if (skeletonAsset == null)
                    Debug.LogError("加载"+AnimationState.SpineAssetPath+"失败");
                Spine.gameObject.SetActive(true);
                if (Spine.skeletonDataAsset != skeletonAsset)
                {
                    Spine.skeletonDataAsset = skeletonAsset;
                    Spine.initialSkinName = AnimationState.Skin;
                    Spine.Initialize(true);
                }
                (Spine.transform as RectTransform).anchoredPosition = AnimationState.IdleAnchorPosition;
                Spine.Skeleton.SetSkin(AnimationState.Skin);
                Spine.PlaySkeletonAnimation(AnimationState.IdleAnimation);
            }
        }

        public override void UpdateViewPrivate()
        {
            base.UpdateViewPrivate();
            if (Content == null)
            {
                IconBG.gameObject.SetActive(false);
                if (AnimationState != null)
                {
                    if (AnimationState.EmptyShow)
                    {
                        Spine.PlaySkeletonAnimation(AnimationState.EmptyAnimation);
                    }
                    else
                    {
                        Spine.gameObject.SetActive(false);
                    }
                }
                AnimationState = null;
            }
            else
            {
                IconBG.gameObject.SetActive(true);
                IconBG.sprite = BGSprite;
                Icon.sprite = UserData.GetResourceIcon(MaterialConfig.Id, UserData.ResourceSubType.Big);
                AnimationState = MaterialAnimationState.GetMaterialAnimationState(MaterialConfig.Id);
                var skeletonAsset = ResourcesManager.Instance.LoadResource<SkeletonDataAsset>(AnimationState.SpineAssetPath);
                if (skeletonAsset == null)
                    Debug.LogError("加载"+AnimationState.SpineAssetPath+"失败");
                Spine.gameObject.SetActive(true);
                if (Spine.skeletonDataAsset != skeletonAsset)
                {
                    Spine.skeletonDataAsset = skeletonAsset;
                    Spine.initialSkinName = AnimationState.Skin;
                    Spine.Initialize(true);
                }
                (Spine.transform as RectTransform).anchoredPosition = AnimationState.IdleAnchorPosition;
                Spine.Skeleton.SetSkin(AnimationState.Skin);
                if (AnimationState.InShow)
                {
                    Spine.PlaySkeletonAnimation(AnimationState.InAnimation);
                }
                else
                {
                    Spine.PlaySkeletonAnimation(AnimationState.IdleAnimation);   
                }
            }
        }

        public override async Task PerformMix()
        {
            MoveNode.DOMove(Controller.DropPointLiquid.position, 0.8f).SetEase(Ease.Linear);
            (Spine.transform as RectTransform).DOAnchorPos(AnimationState.MixAnchorPosition, 0.8f);

            var animationWaitTime = AnimationState.OutAnimationWaitTime;
            var prepareTime = 1.5f;
            var waitTime = prepareTime - animationWaitTime;
            XUtility.WaitSeconds(waitTime, () =>
            {
                if (!this)
                    return;
                var skeletonAsset = ResourcesManager.Instance.LoadResource<SkeletonDataAsset>(AnimationState.SpineAssetPath);
                if (skeletonAsset == null)
                    Debug.LogError("加载"+AnimationState.SpineAssetPath+"失败");
                Spine.gameObject.SetActive(true);
                if (Spine.skeletonDataAsset != skeletonAsset)
                {
                    Spine.skeletonDataAsset = skeletonAsset;
                    Spine.initialSkinName = AnimationState.Skin;
                    Spine.Initialize(true);
                }
                Spine.Skeleton.SetSkin(AnimationState.Skin);
                Spine.PlaySkeletonAnimation(AnimationState.OutAnimation);
            });
            Controller.BottleAnimator.PlayAnimation("open");
            await XUtility.WaitSeconds(0.8f);
            Canvas.sortingOrder = Controller.canvas.sortingOrder+1;
            await XUtility.WaitSeconds(2.4f);
            Canvas.sortingOrder = Controller.canvas.sortingOrder+4;
            Controller.BottleAnimator.PlayAnimation("closed");
            MoveNode.DOLocalMove(InitLocalPosition, 0.5f).SetEase(Ease.Linear);
            await XUtility.WaitSeconds(0.3f);
        }
    }

    public class BottleSolid : Bottle
    {
        public SkeletonGraphic Spine;
        public Transform MoveNode;
        private Vector3 InitLocalPosition;
        private MaterialAnimationState AnimationState;
        public override void InitPrivate()
        {
            base.InitPrivate();
            Spine = transform.Find("Material/Spine").GetComponent<SkeletonGraphic>();
            MoveNode = transform;
            InitLocalPosition = MoveNode.localPosition;
            if (Content == null)
            {
                AnimationState = null;
            }
            else
            {
                AnimationState = MaterialAnimationState.GetMaterialAnimationState(MaterialConfig.Id);
                var skeletonAsset = ResourcesManager.Instance.LoadResource<SkeletonDataAsset>(AnimationState.SpineAssetPath);
                if (skeletonAsset == null)
                    Debug.LogError("加载"+AnimationState.SpineAssetPath+"失败");
                Spine.gameObject.SetActive(true);
                if (Spine.skeletonDataAsset != skeletonAsset)
                {
                    Spine.skeletonDataAsset = skeletonAsset;
                    Spine.initialSkinName = AnimationState.Skin;
                    Spine.Initialize(true);
                }
                (Spine.transform as RectTransform).anchoredPosition = AnimationState.IdleAnchorPosition;
                Spine.Skeleton.SetSkin(AnimationState.Skin);
                Spine.PlaySkeletonAnimation(AnimationState.IdleAnimation);
            }
        }

        public override void UpdateViewPrivate()
        {
            base.UpdateViewPrivate();
            if (Content == null)
            {
                if (AnimationState != null)
                {
                    if (AnimationState.EmptyShow)
                    {
                        Spine.PlaySkeletonAnimation(AnimationState.EmptyAnimation);
                    }
                    else
                    {
                        Spine.gameObject.SetActive(false);
                    }
                }
                AnimationState = null;
            }
            else
            {
                AnimationState = MaterialAnimationState.GetMaterialAnimationState(MaterialConfig.Id);
                var skeletonAsset = ResourcesManager.Instance.LoadResource<SkeletonDataAsset>(AnimationState.SpineAssetPath);
                if (skeletonAsset == null)
                    Debug.LogError("加载"+AnimationState.SpineAssetPath+"失败");
                Spine.gameObject.SetActive(true);
                if (Spine.skeletonDataAsset != skeletonAsset)
                {
                    Spine.skeletonDataAsset = skeletonAsset;
                    Spine.initialSkinName = AnimationState.Skin;
                    Spine.Initialize(true);
                }
                (Spine.transform as RectTransform).anchoredPosition = AnimationState.IdleAnchorPosition;
                Spine.Skeleton.SetSkin(AnimationState.Skin);
                if (AnimationState.InShow)
                {
                    Spine.PlaySkeletonAnimation(AnimationState.InAnimation);
                }
                else
                {
                    Spine.PlaySkeletonAnimation(AnimationState.IdleAnimation);   
                }
            }
        }

        public override async Task PerformMix()
        {
            MoveNode.DOMove(Controller.DropPointLiquid.position, 0.8f).SetEase(Ease.Linear);
            (Spine.transform as RectTransform).DOAnchorPos(AnimationState.MixAnchorPosition, 0.8f);

            var animationWaitTime = AnimationState.OutAnimationWaitTime;
            var prepareTime = 1.5f;
            var waitTime = prepareTime - animationWaitTime;
            XUtility.WaitSeconds(waitTime, () =>
            {
                if (!this)
                    return;
                var skeletonAsset = ResourcesManager.Instance.LoadResource<SkeletonDataAsset>(AnimationState.SpineAssetPath);
                if (skeletonAsset == null)
                    Debug.LogError("加载"+AnimationState.SpineAssetPath+"失败");
                Spine.gameObject.SetActive(true);
                Spine.skeletonDataAsset = skeletonAsset;
                Spine.initialSkinName = AnimationState.Skin;
                Spine.Skeleton.SetSkin(AnimationState.Skin);
                Spine.PlaySkeletonAnimation(AnimationState.OutAnimation);
            });
            Controller.BottleAnimator.PlayAnimation("open");
            await XUtility.WaitSeconds(0.8f);
            Canvas.sortingOrder = Controller.canvas.sortingOrder+1;
            await XUtility.WaitSeconds(2.4f);
            Canvas.sortingOrder = Controller.canvas.sortingOrder+4;
            Controller.BottleAnimator.PlayAnimation("closed");
            MoveNode.DOLocalMove(InitLocalPosition, 0.5f).SetEase(Ease.Linear);
            (Spine.transform as RectTransform).DOAnchorPos(AnimationState.IdleAnchorPosition, 0.5f);
            await XUtility.WaitSeconds(0.3f);
        }
    }

    public class SelectView:MonoBehaviour
    {
        private int BottleIndex;
        private Bottle Bottle;
        private StorageMixMaster Storage;
        private Button PutBtn;
        private Transform DefaultMaterialItem;
        private List<SelectItem> ItemList = new List<SelectItem>();
        private SelectItem CurSelectItem;
        private SelectItem NullSelectItem;
        public Dictionary<int, int> BagState = new Dictionary<int, int>();

        public void Init(StorageMixMaster storage,int bottleIndex,Bottle bottle)
        {
            Bottle = bottle;
            BottleIndex = bottleIndex;
            Storage = storage;
            DefaultMaterialItem = transform.Find("Grid/1");
            DefaultMaterialItem.gameObject.SetActive(false);
            PutBtn = transform.Find("Button").GetComponent<Button>();
            PutBtn.onClick.AddListener(() =>
            {
                if (CurSelectItem && CurSelectItem.Config != null)
                    MixMasterModel.Instance.PutMaterial(BottleIndex,CurSelectItem.Config.Id);
                else
                    MixMasterModel.Instance.ReturnMaterial(BottleIndex);
                gameObject.SetActive(false);
                Bottle.UpdateView();
            });
            BagState.Clear();
            foreach (var bag in Storage.Bag)
            {
                BagState.TryAdd(bag.Key,0);
                BagState[bag.Key] += bag.Value;
            }
            if (Storage.Desktop.TryGetValue(BottleIndex,out var deskState))
            {
                BagState.TryAdd(deskState.Id,0);
                BagState[deskState.Id] += deskState.Count;
            }

            var materialList = new List<MixMasterMaterialConfig>();
            for (var i = 0; i < MixMasterModel.Instance.MaterialConfig.Count; i++)
            {
                var materialConfig = MixMasterModel.Instance.MaterialConfig[i];
                if (materialConfig.PoolIndex.Contains(BottleIndex))
                {
                    materialList.Add(materialConfig);
                }
            }
            NullSelectItem = transform.Find("Grid/Null").gameObject.AddComponent<SelectItem>();
            NullSelectItem.Init(Storage,this,null);
            NullSelectItem.gameObject.SetActive(false);
            for (var i = 0; i < materialList.Count; i++)
            {
                var material = materialList[i];
                var selectItem = Instantiate(DefaultMaterialItem, DefaultMaterialItem.parent).gameObject
                    .AddComponent<SelectItem>();
                selectItem.gameObject.SetActive(true);
                selectItem.Init(Storage,this,material);
                ItemList.Add(selectItem);
            }
            if (Storage.Desktop.TryGetValue(BottleIndex, out var curMaterial))
            {
                CurSelectItem = ItemList.Find(a => a.Config.Id == curMaterial.Id);
                if (CurSelectItem)
                    CurSelectItem.Select();
            }
        }

        public void Open()
        {
            BagState.Clear();
            foreach (var bag in Storage.Bag)
            {
                BagState.TryAdd(bag.Key,0);
                BagState[bag.Key] += bag.Value;
            }
            if (Storage.Desktop.TryGetValue(BottleIndex,out var deskState))
            {
                BagState.TryAdd(deskState.Id,0);
                BagState[deskState.Id] += deskState.Count;
            }
            foreach (var item in ItemList)
            {
                item.Reset();
            }
            CurSelectItem = null;
            if (Storage.Desktop.TryGetValue(BottleIndex, out var curMaterial))
            {
                CurSelectItem = ItemList.Find(a => a.Config.Id == curMaterial.Id);
                if (CurSelectItem)
                    CurSelectItem.Select();
            }
        }
        public void Select(SelectItem selectItem)
        {
            if (CurSelectItem)
                CurSelectItem.UnSelect();
            CurSelectItem = selectItem;
            if (CurSelectItem)
                CurSelectItem.Select();
        }
    }

    public class SelectItem : MonoBehaviour
    {
        private SelectView Controller;
        private StorageMixMaster Storage;
        public MixMasterMaterialConfig Config;
        private Button SelectBtn;
        public Dictionary<int, int> BagState=>Controller.BagState;
        private SelectItemView NormalState;
        private SelectItemView SelectState;
        private SelectItemView NotEnoughState;
        private int CurCount => BagState.TryGetValue(Config.Id, out var count) ? count : 0;
        private bool IsEnough => Config == null||(CurCount >= Config.Count);
        public void Init(StorageMixMaster storage,SelectView controller,MixMasterMaterialConfig config)
        {
            Config = config;
            Storage = storage;
            Controller = controller;
            SelectBtn = transform.gameObject.GetComponent<Button>();
            SelectBtn.onClick.AddListener(() =>
            {
                if (!IsEnough)
                    return;
                Controller.Select(this);
            });
            NormalState = transform.Find("Normal").gameObject.AddComponent<SelectItemView>();
            SelectState = transform.Find("Selected").gameObject.AddComponent<SelectItemView>();
            NotEnoughState = transform.Find("No").gameObject.AddComponent<SelectItemView>();
            if (Config != null)
            {
                SelectState.Init(Config.Count,CurCount,Config.Id);
                NormalState.Init(Config.Count,CurCount,Config.Id);
                NotEnoughState.Init(Config.Count,CurCount,Config.Id);
            }
            SelectState.gameObject.SetActive(false);
            NormalState.gameObject.SetActive(IsEnough);
            NotEnoughState.gameObject.SetActive(!IsEnough);
        }

        public void Reset()
        {
            if (Config != null)
            {
                SelectState.Init(Config.Count,CurCount,Config.Id);
                NormalState.Init(Config.Count,CurCount,Config.Id);
                NotEnoughState.Init(Config.Count,CurCount,Config.Id);
            }
            SelectState.gameObject.SetActive(false);
            NormalState.gameObject.SetActive(IsEnough);
            NotEnoughState.gameObject.SetActive(!IsEnough);
        }
        public void UnSelect()
        {
            SelectState.gameObject.SetActive(false);
            NormalState.gameObject.SetActive(IsEnough);
            NotEnoughState.gameObject.SetActive(!IsEnough);
        }
        public void Select()
        {
            SelectState.gameObject.SetActive(true);
            NormalState.gameObject.SetActive(false);
            NotEnoughState.gameObject.SetActive(false);
        }
    }

    public class SelectItemView : MonoBehaviour
    {
        private LocalizeTextMeshProUGUI Text;
        private Image Icon;
        public void Init(int needCount,int curCount,int materialId)
        {
            Text = transform.Find("Item/Text").GetComponent<LocalizeTextMeshProUGUI>();
            Icon = transform.Find("Item/Icon").GetComponent<Image>();
            Text.SetText(curCount+"/"+needCount);
            Icon.sprite = UserData.GetResourceIcon(materialId, UserData.ResourceSubType.Big);
        }
    }

    public class MaterialAnimationState
    {
        public string SpineAssetPath;//spine资源路径
        public string Skin;//spine皮肤
        public string EmptyAnimation;//空杯动画
        public string IdleAnimation;//材料待机动画
        public string OutAnimation;//材料倒出动画
        public float OutAnimationWaitTime;//倒出动画倒出的时间节点
        public string InAnimation;//材料进入动画
        public bool EmptyShow;//空杯状态是否显示动画
        public bool InShow;//是否有材料进入动画
        public Vector3 IdleAnchorPosition;
        public Vector3 MixAnchorPosition;
        public UserData.ResourceId Material;
        public void PlayInSound()
        {
            var materialId = (int)Material;
            if (materialId == 966 ||
                materialId == 967)
            {
                XUtility.WaitSeconds(0.5f, () => AudioManager.Instance.PlaySoundById(165));
            }
        }
        public void PlayOutSound()
        {
            var materialId = (int)Material;
            if (materialId == 966 ||
                materialId == 967)
            {
                XUtility.WaitSeconds(1.2f, () => AudioManager.Instance.PlaySoundById(165));
            }
        }

        public static Dictionary<int, MaterialAnimationState> StaticStateDic = new Dictionary<int, MaterialAnimationState>();
        public static MaterialAnimationState GetMaterialAnimationState(int resourceId)
        {
            if (StaticStateDic.ContainsKey(resourceId))
                return StaticStateDic[resourceId];
            StaticStateDic.Add(resourceId,new MaterialAnimationState((UserData.ResourceId)resourceId));
            return StaticStateDic[resourceId];
        }
        public MaterialAnimationState(UserData.ResourceId resourceId)
        {
            Material = resourceId;
            var materialId = (int)Material;
            if (materialId == 960)//冰块
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "ice_idle";
                EmptyAnimation = "bowl";
                OutAnimation = "ice_work";
                EmptyShow = true;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 961)//冰激凌球
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "icecream_idle";
                EmptyAnimation = "bowl";
                OutAnimation = "icecream_work";
                EmptyShow = true;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 962)//草莓
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "strawberry_idle";
                EmptyAnimation = "bowl";
                OutAnimation = "strawberry_work";
                EmptyShow = true;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 963)//芒果
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "mango_idle";
                EmptyAnimation = "bowl";
                OutAnimation = "mango_work";
                EmptyShow = true;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 964)//红豆
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "ormosia_idle";
                EmptyAnimation = "bowl";
                OutAnimation = "ormosia_work";
                EmptyShow = true;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 965)//巧克力
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "chocolate_idle";
                EmptyAnimation = "bowl";
                OutAnimation = "chocolate_work";
                EmptyShow = true;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 966)//抹茶
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "matcha_idle";
                OutAnimation = "matcha_work";
                EmptyShow = false;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 967)//牛奶
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/juice_make_SkeletonData";
                Skin = "milk";
                IdleAnimation = "cup2";
                EmptyAnimation = "cup1";
                OutAnimation = "liquid_out";
                InAnimation = "liquid_in";
                EmptyShow = false;
                IdleAnchorPosition = new Vector3(0, -57, 0);
                MixAnchorPosition = new Vector3(-48, -57, 0);
                OutAnimationWaitTime = 1f;
            }
            else if (materialId == 968)//坚果碎
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "peanut_idle";
                OutAnimation = "peanut_work";
                EmptyShow = false;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
            else if (materialId == 969)//水果干
            {
                SpineAssetPath = "Prefabs/Activity/MixMaster/drink_maker_SkeletonData";
                Skin = "default";
                IdleAnimation = "fruit_idle";
                OutAnimation = "fruit_work";
                EmptyShow = false;
                IdleAnchorPosition = new Vector3(0, -77, 0);
                MixAnchorPosition = new Vector3(0, -77, 0);
                OutAnimationWaitTime = 0.5f;
            }
        }
    }
}