
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class MergeKapibala : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private KapibalaLifeRedPoint RedPoint;
    
    private StorageKapibala Storage;
    private void SetStorage(StorageKapibala storage)
    {
        Storage = storage;
        RedPoint.Init(Storage);
        RefreshView();
    }

    private void Awake()
    {
        _btn = transform.GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
        _countDownTime = transform.Find("Root/TimeGroup/TimeText").GetComponent<LocalizeTextMeshProUGUI>();
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<KapibalaLifeRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);

        SetStorage(KapibalaModel.Instance.Storage);
        
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KapibalaEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupKapibalaPreviewController.Open(Storage);
        }
        else
        {
            UIKapibalaMainController.Open(Storage);
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
            // if (Storage.IsStart)
            // {
            //     UIKapibalaMainController.Open(Storage);
            // }
            // else
            // {
            //     KapibalaModel.CanShowStart();
            // }
        }
    }

    public void RefreshView()
    {
        gameObject.SetActive(Storage != null && Storage.ShowTaskEntrance());
        if (!gameObject.activeSelf)
            return;
    }
    private void RefreshCountDown()
    {
        RefreshView();
        
        if (Storage.GetPreheatLeftTime() > 0)
        {
            _countDownTime.SetText(Storage.GetPreheatLeftTimeText());
        }
        else
        {
            _countDownTime.SetText(Storage.GetLeftTimeText());
        }
    }
    private void OnDestroy()
    {
    }
}