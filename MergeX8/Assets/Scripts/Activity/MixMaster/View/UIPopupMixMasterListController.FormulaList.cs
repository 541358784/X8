using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupMixMasterListController
{
    public class FormulaListView : MonoBehaviour
    {
        private UIPopupMixMasterListController Controller;
        private StorageMixMaster Storage;
        private Transform DefaultItem;
        private List<FormulaItem> FormulaItemList = new List<FormulaItem>();
        public void Init(UIPopupMixMasterListController controller, StorageMixMaster storage)
        {
            Controller = controller;
            Storage = storage;
            DefaultItem = transform.Find("Viewport/Content/1");
            DefaultItem.gameObject.SetActive(false);
            var formulaList = MixMasterModel.Instance.FormulaConfig;
            for (var i = 0; i < formulaList.Count; i++)
            {
                var formula = formulaList[i];
                var formulaItem = Instantiate(DefaultItem, DefaultItem.parent).gameObject.AddComponent<FormulaItem>();
                formulaItem.gameObject.SetActive(true);
                formulaItem.Init(Controller,Storage,formula);
                FormulaItemList.Add(formulaItem);
            }
        }
    }

    public class FormulaItem:MonoBehaviour
    {
        private UIPopupMixMasterListController Controller;
        private StorageMixMaster Storage;
        private MixMasterFormulaConfig FormulaConfig;
        private FormulaItemView UnlockGroup;
        private FormulaItemView LockGroup;

        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventMixMasterUnlockFormula>(OnUnlockFormula);
        }

        public void OnUnlockFormula(EventMixMasterUnlockFormula evt)
        {
            if (evt.Formula == FormulaConfig)
            {
                var isHide = !Storage.History.ContainsKey(FormulaConfig.Id);
                UnlockGroup.gameObject.SetActive(!isHide);
                LockGroup.gameObject.SetActive(isHide);
            }
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventMixMasterUnlockFormula>(OnUnlockFormula);
        }

        public void Init(UIPopupMixMasterListController controller, StorageMixMaster storage,MixMasterFormulaConfig formulaConfig)
        {
            Controller = controller;
            Storage = storage;
            FormulaConfig = formulaConfig;
            UnlockGroup = transform.Find("Unlock").gameObject.AddComponent<FormulaItemView>();
            UnlockGroup.Init(Controller,Storage,FormulaConfig,false);
            LockGroup = transform.Find("Lock").gameObject.AddComponent<FormulaItemView>();
            LockGroup.Init(Controller,Storage,FormulaConfig,true);
            var isHide = !Storage.History.ContainsKey(FormulaConfig.Id);
            UnlockGroup.gameObject.SetActive(!isHide);
            LockGroup.gameObject.SetActive(isHide);
        }
    }

    public class FormulaItemView : MonoBehaviour
    {
        private UIPopupMixMasterListController Controller;
        private StorageMixMaster Storage;
        private MixMasterFormulaConfig FormulaConfig;
        private bool LockView;
        private CommonRewardItem RewardItem;
        private Button UseBtn;
        private LocalizeTextMeshProUGUI Text1;
        private LocalizeTextMeshProUGUI Text2;
        private Image FormulaIcon;
        private Transform Label;
        private LocalizeTextMeshProUGUI LabelText;

        public void Init(UIPopupMixMasterListController controller, StorageMixMaster storage,
            MixMasterFormulaConfig formulaConfig,bool isLock)
        {
            Controller = controller;
            Storage = storage;
            FormulaConfig = formulaConfig;
            LockView = isLock;
            if (!LockView)
            {
                UseBtn = transform.Find("Button").GetComponent<Button>();
                UseBtn.onClick.AddListener(() =>
                {
                    for (var i = 0; i < FormulaConfig.MaterialId.Count; i++)
                    {
                        MixMasterModel.Instance.PutMaterial(i+1,FormulaConfig.MaterialId[i]);
                    }

                    var mainPopup =
                        UIManager.Instance.GetOpenedUIByPath<UIMixMasterMainController>(UINameConst
                            .UIMixMasterMain);
                    if (mainPopup)
                    {
                        mainPopup.UpdateDesktop();
                        Controller.AnimCloseWindow();
                    }
                });
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
                // UseBtn.interactable = MixMasterModel.Instance.CheckFormula(FormulaConfig,bagState);
                UseBtn.gameObject.SetActive(MixMasterModel.Instance.CheckFormula(FormulaConfig,bagState));

                for (var i = 0; i < FormulaConfig.MaterialId.Count; i++)
                {
                    var materialGroup = transform.Find("Make/" + (i + 1));
                    var materialId = FormulaConfig.MaterialId[i];
                    var materialConfig = MixMasterModel.Instance.MaterialConfig.Find(a => a.Id == materialId);
                    if (!bagState.TryGetValue(materialId, out var hasCount))
                        hasCount = 0;
                    var needCount = materialConfig.Count;
                    materialGroup.Find("Image").GetComponent<Image>().sprite = UserData.GetResourceIcon(materialId,UserData.ResourceSubType.Big);
                    var text = materialGroup.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                    var notText = materialGroup.Find("NotText").GetComponent<LocalizeTextMeshProUGUI>();
                    text.SetText(hasCount+"/"+needCount);
                    notText.SetText(hasCount+"/"+needCount);
                    text.gameObject.SetActive(hasCount >= needCount);
                    notText.gameObject.SetActive(hasCount < needCount);
                }
            }

            
            RewardItem = transform.Find("RewardItem").gameObject.AddComponent<CommonRewardItem>();
            var repeatRewards = CommonUtils.FormatReward(FormulaConfig.RepeatRewardId, FormulaConfig.RepeatRewardNum);
            var firstRewards = CommonUtils.FormatReward(FormulaConfig.FirstRewardId, FormulaConfig.FirstRewardNum);
            var isFirst = !Storage.History.TryGetValue(FormulaConfig.Id, out var count) || count == 0;
            var showReward = (isFirst ? firstRewards : repeatRewards)[0];
            RewardItem.Init(showReward);

            Text1 = transform.Find("TitleText").GetComponent<LocalizeTextMeshProUGUI>();
            Text1.SetTerm(FormulaConfig.NameKey);
            Text2 = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            Text2.SetTerm(FormulaConfig.TipKey);
            FormulaIcon = transform.Find("Item/Icon").GetComponent<Image>();
            FormulaIcon.sprite = FormulaConfig.GetFormulaIcon();

            if (!LockView)
            {
                var times = Storage.History.TryGetValue(FormulaConfig.Id,out var tempTimes)?tempTimes:0;
                Label = transform.Find("Label");
                Label.gameObject.SetActive(times > 0);
                LabelText = transform.Find("Label/Text").GetComponent<LocalizeTextMeshProUGUI>();
                LabelText.SetTermFormats(times.ToString());   
            }
        }
    }
}