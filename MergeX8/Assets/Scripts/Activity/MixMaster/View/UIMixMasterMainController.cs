using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMixMasterMainController:UIWindowController
{
    private MixMasterEntranceRedPoint RedPoint;
    private Button ShopBtn;
    private Button FormulaBtn;
    private Button MakeBtn;
    private Button CloseBtn;
    private LocalizeTextMeshProUGUI TimeText;
    private Dictionary<int, Bottle> BottleDic = new Dictionary<int, Bottle>();
    private Button MakeBottleBtn;
    public Animator BottleAnimator;
    public SkeletonGraphic BottleSpine;
    public Transform DropPointLiquid;
    public static UIMixMasterMainController Open(StorageMixMaster storage)
    {
        var openWindow =
            UIManager.Instance.GetOpenedUIByPath<UIMixMasterMainController>(UINameConst.UIMixMasterMain);
        if (openWindow)
        {
            openWindow.CloseWindowWithinUIMgr(true);
        }
        return UIManager.Instance.OpenUI(UINameConst.UIMixMasterMain, storage) as UIMixMasterMainController;
    }

    public bool Mixing;
    public override void PrivateAwake()
    {
        ShopBtn = GetItem<Button>("Root/ButtonShop");
        ShopBtn.onClick.AddListener(() =>
        {
            if (Mixing)
                return;
            UIPopupMixMasterShopController.Open();
        });
        RedPoint = transform.Find("Root/ButtonList/RedPoint").gameObject.AddComponent<MixMasterEntranceRedPoint>();
        RedPoint.gameObject.SetActive(true);
        FormulaBtn = GetItem<Button>("Root/ButtonList");
        FormulaBtn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.MixMasterFormula);
            if (Mixing)
                return;
            UIPopupMixMasterListController.Open(Storage);
        });
        MakeBottleBtn = GetItem<Button>("Root/MakeItem");
        MakeBottleBtn.onClick.AddListener(() =>
        {
            if (Mixing)
                return;
            UIPopupMixMasterListController.Open(Storage);
        });
        MakeBtn = GetItem<Button>("Root/Button");
        MakeBtn.onClick.AddListener(async () =>
        {
            if (Mixing)
                return;
            Mixing = true;
            MakeBottleBtn.interactable = false;
            MakeBtn.interactable = false;
            foreach (var pair in BottleDic)
            {
                pair.Value.SelectView.gameObject.SetActive(false);
                pair.Value.Replace.gameObject.SetActive(false);
            }
            foreach (var pair in BottleDic)
            {
                await pair.Value.PerformMix();
            }
            AudioManager.Instance.PlaySoundById(166);
            BottleAnimator.PlayAnimation("work");
            BottleSpine.PlaySkeletonAnimation("machine_work");
            await XUtility.WaitSeconds(1f);
            BottleAnimator.PlayAnimation("normal");
            MixMasterModel.Instance.Mix();
            foreach (var pair in BottleDic)
            {
                pair.Value.UpdateView();
            }
            Mixing = false;
            MakeBottleBtn.interactable = true;
        });
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            if (Mixing)
                return;
            AnimCloseWindow();
        });
        TimeText = GetItem<LocalizeTextMeshProUGUI>("Root/TimeGroup/TimeText");
        InvokeRepeating("UpdateTime",0,1);
        
        for (var i = 1; transform.Find("Root/Make/" + i); i++)
        {
            if (i == 3)
            {
                var bottle = transform.Find("Root/Make/" + i).gameObject.AddComponent<BottleSolid>();
                BottleDic.Add(i,bottle);   
            }
            else
            {
                var bottle = transform.Find("Root/Make/" + i).gameObject.AddComponent<BottleLiquid>();
                BottleDic.Add(i,bottle);   
            }
        }
        BottleAnimator = GetItem<Animator>("Root/MakeItem/Root");
        BottleAnimator.PlayAnimation("normal");
        BottleSpine = GetItem<SkeletonGraphic>("Root/MakeItem/Root/Spine");
        DropPointLiquid = transform.Find("Root/MakeItem/DropPointLiquid");
    }

    public void UpdateTime()
    {
        TimeText.SetText(Storage.GetLeftTimeText());
    }

    public StorageMixMaster Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMixMaster;
        RedPoint.Init(Storage);
        foreach (var bottlePair in BottleDic)
        {
            bottlePair.Value.Init(Storage,bottlePair.Key,this);
        }
        MakeBtn.interactable = Storage.Desktop.Count == MixMasterModel.Instance.GlobalConfig.MaterialNeedCount;
        var startDayId = (ulong)Storage.PreheatTime / XUtility.DayTime;
        var curDayId = APIManager.Instance.GetServerTime() / XUtility.DayTime;
        var curGiftBagPopupDayId = (int)(curDayId - startDayId);
        if (curGiftBagPopupDayId < 1)
        {
            if (Storage.GiftBagPopupDayId == -10086)
            {
                Storage.GiftBagPopupDayId = curGiftBagPopupDayId;
                UIPopupMixMasterShopController.Open();
            }
            else if (Storage.GiftBagPopupDayId != curGiftBagPopupDayId)
            {
                Storage.GiftBagPopupDayId = -10086;
            }
        }
        else
        {
            if (Storage.GiftBagPopupDayId != curGiftBagPopupDayId)
            {
                Storage.GiftBagPopupDayId = curGiftBagPopupDayId;
                UIPopupMixMasterShopController.Open();
            }
        }
        ShieldButtonOnClick[] shieldButtons = gameObject.GetComponentsInChildren<ShieldButtonOnClick>(true);
        foreach (var shieldBtn in shieldButtons)
        {
            shieldBtn.isUse = false;
        }
        {
            var canvas2 = transform.Find("Root/MakeItem/Root").gameObject.AddComponent<Canvas>();
            canvas2.overrideSorting = true;
            canvas2.sortingOrder = canvas.sortingOrder + 2;
        }
        CheckGuide();
    }

    public void CheckGuide()
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.MixMasterFormula))
        {
            MixMasterFormulaConfig enableFormula = null;
            var bagState = new Dictionary<int, int>();
            foreach (var bag in Storage.Bag)
            {
                bagState.TryAdd(bag.Key,0);
                bagState[bag.Key] += bag.Value;
            }
            foreach (var desktop in Storage.Desktop)
            {
                bagState.TryAdd(desktop.Value.Id,0);
                bagState[desktop.Value.Id] += desktop.Value.Count;
            }
            foreach (var pair in Storage.History)
            {
                var formula = MixMasterModel.Instance.FormulaConfig.Find(a => a.Id == pair.Key);
                if (MixMasterModel.Instance.CheckFormula(formula, bagState))
                {
                    enableFormula = formula;
                    break;
                }
            }
            if (enableFormula != null)
            {
                if (FormulaBtn.gameObject.activeInHierarchy)
                {
                    List<Transform> topLayer = new List<Transform>();
                    topLayer.Add(FormulaBtn.transform);
                    GuideSubSystem.Instance.RegisterTarget(GuideTargetType.MixMasterFormula, FormulaBtn.transform as RectTransform,
                        topLayer: topLayer);
                    GuideSubSystem.Instance.Trigger(GuideTriggerPosition.MixMasterFormula, null);
                }
            }
        }
    }
    public void UpdateDesktop()
    {
        MakeBtn.interactable = Storage.Desktop.Count == MixMasterModel.Instance.GlobalConfig.MaterialNeedCount;
        foreach (var pair in BottleDic)
        {
            pair.Value.UpdateView();
        }
    }
}