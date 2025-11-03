using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public partial class UITurtlePangMainController
{
    public Transform SelectPackageGroup;
    public SelectPackageBtn SelectPackageBtn1;
    public SelectPackageBtn SelectPackageBtn2;
    private LocalizeTextMeshProUGUI PackageNumText;
    public void InitSelectPackageGroup()
    {
        SelectPackageGroup = GetItem<Transform>("Root/StartButton");
        SelectPackageGroup.gameObject.SetActive(true);
        SelectPackageBtn1 = GetItem<Transform>("Root/StartButton/Button1").gameObject.AddComponent<SelectPackageBtn>();
        SelectPackageBtn1.Init(Model.GlobalConfig.PackageType1,Storage,OnSelectPackage);
        SelectPackageBtn2 = GetItem<Transform>("Root/StartButton/Button2").gameObject.AddComponent<SelectPackageBtn>();
        SelectPackageBtn2.Init(Model.GlobalConfig.PackageType2,Storage,OnSelectPackage);
        SelectPackageGroup.gameObject.SetActive(false);
        PackageNumText = GetItem<LocalizeTextMeshProUGUI>("Root/StartButton/NumGroup/Text");
        GetItem<LocalizeTextMeshProUGUI>("Root/StartButton/Button2/Text1").SetTermFormats(Model.GlobalConfig.PackageType2[1].ToString());
        UpdatePackageNumText();
    }
    public TaskCompletionSource<List<int>> SelectPackageTask;
    public async Task SelectPackage()
    {
        if (Storage.IsInGame)
            return;
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.TurtlePangPackage))
        {

            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(SelectPackageBtn1.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.TurtlePangPackage, SelectPackageBtn1.Btn.transform as RectTransform,
                topLayer: topLayer);
            if (GuideSubSystem.Instance.Trigger(GuideTriggerPosition.TurtlePangPackage, null))
            {
                Model.AddPackage(Model.GlobalConfig.PackageType1[0],"Guide");
                UpdatePackageNumText();
            }
        }
        IsPerforming = true;
        SelectPackageTask = new TaskCompletionSource<List<int>>();
        GameGroup.gameObject.SetActive(false);
        SelectPackageGroup.gameObject.SetActive(true);
        var result = await SelectPackageTask.Task;
        Storage.IsInGame = true;
        var baseCount = result[0];
        var extraCount = result[1];
        Storage.PackageCount -= baseCount;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventTurtlePangPackageUse,
            data1:baseCount.ToString(),data2:Storage.PackageCount.ToString());
        UpdatePackageNumText();
        Storage.BasePackageCount = baseCount;
        Storage.ExtraPackageCount = extraCount;
        SelectPackageGroup.gameObject.SetActive(false);
        GameGroup.gameObject.SetActive(true);
        await PerformBasePackage(Storage.BasePackageCount + Storage.ExtraPackageCount);
        IsPerforming = false;
        //刷新界面
    }

    public void UpdatePackageNumText()
    {
        PackageNumText.SetText(Storage.PackageCount.ToString());
    }
    public void OnSelectPackage(List<int> config)
    {
        SelectPackageTask.SetResult(config);
    }

    public class SelectPackageBtn : MonoBehaviour
    {
        public Button Btn;
        public LocalizeTextMeshProUGUI NumText;
        public List<int> Config;
        public StorageTurtlePang Storage;
        public Action<List<int>> Callback;
        public void Init(List<int> config,StorageTurtlePang storage,Action<List<int>> callback)
        {
            Config = config;
            Storage = storage;
            Callback = callback;
            NumText.SetText(Config[0].ToString());
            // Btn.interactable = Storage.PackageCount >= Config[0];
        }

        private void Awake()
        {
            Btn = transform.GetComponent<Button>();
            Btn.onClick.AddListener(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.TurtlePangPackage);
                if (Storage.PackageCount < Config[0])
                {
                    UIPopupTurtlePangNoItemController.Open(Storage);
                    return;
                }
                Callback.Invoke(Config);
            });
            NumText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        }
    }
}
