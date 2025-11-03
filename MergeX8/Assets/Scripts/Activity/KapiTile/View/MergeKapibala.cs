
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class MergeKapiTile : MonoBehaviour
{
    private LocalizeTextMeshProUGUI _countDownTime;
    private Button _btn;
    // private Transform _rewardGroup;
    private KapiTileLifeRedPoint RedPoint;
    
    private StorageKapiTile Storage;
    private void SetStorage(StorageKapiTile storage)
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
        RedPoint = transform.Find("Root/RedPoint").gameObject.AddComponent<KapiTileLifeRedPoint>();
        RedPoint.gameObject.SetActive(true);
        InvokeRepeating("RefreshCountDown", 0, 1f);
        
        SetStorage(KapiTileModel.Instance.Storage);
    }
    public void OnClick()
    {
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.KapiTileEntrance);
        if (Storage.GetPreheatLeftTime() > 0)
        {
            UIPopupKapiTilePreviewController.Open(Storage);
        }
        else
        {
            UIKapiTileMainController.Open(Storage);
            SceneFsm.mInstance.ChangeState(StatusType.Transition, StatusType.BackHome);
            // if (Storage.IsStart)
            // {
            //     UIKapiTileMainController.Open(Storage);
            // }
            // else
            // {
            //     KapiTileModel.CanShowStart();
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