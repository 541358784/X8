using DragonU3DSDK.Storage;
using DragonU3DSDK.Storage.Decoration;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupMixMasterListController:UIWindowController
{
    public static UIPopupMixMasterListController Open(StorageMixMaster storage)
    {
        var openWindow = UIManager.Instance.GetOpenedUIByPath<UIPopupMixMasterListController>(UINameConst.UIPopupMixMasterList);
        if (openWindow)
            openWindow.CloseWindowWithinUIMgr(true);
        return UIManager.Instance.OpenUI(UINameConst.UIPopupMixMasterList, storage) as UIPopupMixMasterListController;
    }
    public override void PrivateAwake()
    {
        FormulaView = transform.Find("Root/Scroll View1").gameObject.AddComponent<FormulaListView>();
        TaskView = transform.Find("Root/Scroll View2").gameObject.AddComponent<MixTaskView>();
        CloseBtn = GetItem<Button>("Root/ButtonClose");
        CloseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }

    private StorageMixMaster Storage;
    private Button FormulaTagBtn;
    private Transform FormulaTagSelect;
    private Transform FormulaTagUnSelect;
    private Button MixTaskTagBtn;
    private Transform MixTaskTagSelect;
    private Transform MixTaskTagUnSelect;
    private Transform ShowTag;
    private FormulaListView FormulaView;
    private MixTaskView TaskView;
    private Button CloseBtn;
    private MixMasterEntranceRedPoint FormulaRedPoint;
    private MixMasterEntranceRedPoint TaskRedPoint;
    
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StorageMixMaster;
        FormulaRedPoint = transform.Find("Root/LabelGroup/1/RedPoint").gameObject
            .AddComponent<MixMasterEntranceRedPoint>();
        FormulaRedPoint.gameObject.SetActive(true);
        FormulaRedPoint.IgnoreTask();
        FormulaRedPoint.Init(Storage);
        TaskRedPoint = transform.Find("Root/LabelGroup/2/RedPoint").gameObject
            .AddComponent<MixMasterEntranceRedPoint>();
        TaskRedPoint.gameObject.SetActive(true);
        TaskRedPoint.IgnoreFormula();
        TaskRedPoint.Init(Storage);
        FormulaTagBtn = transform.Find("Root/LabelGroup/1").GetComponent<Button>();
        FormulaTagSelect = transform.Find("Root/LabelGroup/1/Selected");
        FormulaTagUnSelect = transform.Find("Root/LabelGroup/1/Normal");
        FormulaTagBtn.onClick.AddListener(() =>
        {
            if (ShowTag != FormulaView.transform)
            {
                FormulaTagSelect.gameObject.SetActive(true);
                FormulaTagUnSelect.gameObject.SetActive(false);
                MixTaskTagSelect.gameObject.SetActive(false);
                MixTaskTagUnSelect.gameObject.SetActive(true);
                ShowTag.gameObject.SetActive(false);
                ShowTag = FormulaView.transform;
                ShowTag.gameObject.SetActive(true);
            }
        });
        MixTaskTagBtn = transform.Find("Root/LabelGroup/2").GetComponent<Button>();
        MixTaskTagSelect = transform.Find("Root/LabelGroup/2/Selected");
        MixTaskTagUnSelect = transform.Find("Root/LabelGroup/2/Normal");
        MixTaskTagBtn.onClick.AddListener(() =>
        {
            if (ShowTag != TaskView.transform)
            {
                FormulaTagSelect.gameObject.SetActive(false);
                FormulaTagUnSelect.gameObject.SetActive(true);
                MixTaskTagSelect.gameObject.SetActive(true);
                MixTaskTagUnSelect.gameObject.SetActive(false);
                ShowTag.gameObject.SetActive(false);
                ShowTag = TaskView.transform;
                ShowTag.gameObject.SetActive(true);
            }
        });
        FormulaView.Init(this,Storage);
        TaskView.Init(this,Storage);
        
        TaskView.gameObject.SetActive(false);
        FormulaView.gameObject.SetActive(true);
        ShowTag = FormulaView.transform;
        FormulaTagSelect.gameObject.SetActive(true);
        FormulaTagUnSelect.gameObject.SetActive(false);
        MixTaskTagSelect.gameObject.SetActive(false);
        MixTaskTagUnSelect.gameObject.SetActive(true);
    }
}