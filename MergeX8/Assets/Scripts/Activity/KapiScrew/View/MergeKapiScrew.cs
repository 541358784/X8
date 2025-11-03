
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class MergeKapiScrew : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private KapiScrewLifeRedPoint RedPoint;
    
    private StorageKapiScrew Storage;
    private void SetStorage(StorageKapiScrew storage)
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
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<KapiScrewLifeRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        SetStorage(KapiScrewModel.Instance.Storage);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KapiScrewEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupKapiScrewPreviewController.Open(Storage);
        }
        else
        {
            UIKapiScrewMainController.Open(Storage);
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
            // if (Storage.IsStart)
            // {
            //     UIKapiScrewMainController.Open(Storage);
            // }
            // else
            // {
            //     KapiScrewModel.CanShowStart();
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