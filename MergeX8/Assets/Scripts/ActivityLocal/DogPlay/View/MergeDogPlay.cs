using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class MergeDogPlay:MonoBehaviour
{
    public static MergeDogPlay Instance;

    public static MergeDogPlay CreateEntrance(Transform trans)
    {
        var entrance = trans.gameObject.AddComponent<MergeDogPlay>();
        entrance.Init();
        Instance = entrance;
        return Instance;
    }

    private StorageDogPlay Storage => DogPlayModel.Instance.Storage;

    private Image Slider;
    private LocalizeTextMeshProUGUI SldierText;
    private Button Btn;
    private Transform FinishEffect;
    private LocalizeTextMeshProUGUI TimeText;
    public void Init()
    {
        TimeText = transform.Find("TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        FinishEffect = transform.Find("Finish");
        Btn = transform.GetComponent<Button>();
        Btn.onClick.AddListener(() =>
        {
            GuideSubSystem.Instance.FinishCurrent(GuideTargetType.DogPlayCollect);
            DogPlayModel.Instance.OpenMainPopup();
        });
        Slider = transform.Find("Filled").GetComponent<Image>();
        SldierText = transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateTime",0,1);
        RefreshView();
    }

    public void RefreshView()
    {
        Slider.fillAmount = Storage.MaxCount > 0?((float)Storage.CurCount / Storage.MaxCount):0;
        SldierText.SetText(Storage.CurCount+"/"+Storage.MaxCount);
        FinishEffect?.gameObject.SetActive(Storage.CurCount >= Storage.MaxCount);
    }
    public void UpdateTime()
    {
        gameObject.SetActive(DogPlayModel.Instance.LastOpenState);
        TimeText.gameObject.SetActive(DogPlayExtraRewardModel.Instance.IsOpened() && DogPlayExtraRewardModel.Instance.GetExtraRewards(Storage).Count > 0);
        TimeText.SetText(DogPlayExtraRewardModel.Instance.GetActivityLeftTimeString());
    }

    public void OnCollectProp()
    {
        var time = 0.3f;
        Slider.DOKill(true);
        var progress = Storage.MaxCount > 0?((float)Storage.CurCount / Storage.MaxCount):0;
        Slider.DOFillAmount(progress, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            Slider.fillAmount = progress;
        });
        SldierText.SetText(Storage.CurCount+"/"+Storage.MaxCount);
        FinishEffect?.gameObject.SetActive(Storage.CurCount >= Storage.MaxCount);
        if (Storage.CurCount >= Storage.MaxCount)
        {
            DogPlayModel.Instance.CanShowCollectGuide();   
        }
    }

    public void PerformInitFly()
    {
        var i = 0;
        foreach (var pair in Storage.OrderActiveState)
        {
            var taskItem = MergeTaskTipsController.Instance.GetTaskItem(pair.Key);
            if (!taskItem)
                continue;
            var target = taskItem.DogPlayGroup;
            target.gameObject.gameObject.SetActive(true);
            FlyGameObjectManager.Instance.FlyObject(target.gameObject, transform.position,
                target.transform.position, true, 1f, 0.1f * i, () =>
                {
                    FlyGameObjectManager.Instance.PlayHintStarsEffect(target.transform.position);
                    ShakeManager.Instance.ShakeLight();
                    target.Refresh();
                });
            target.gameObject.gameObject.SetActive(false);
            i++;
        }
    }
}