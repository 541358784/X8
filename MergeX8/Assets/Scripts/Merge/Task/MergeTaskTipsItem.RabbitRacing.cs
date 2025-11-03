using Activity.RabbitRacing.Dynamic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public partial class MergeTaskTipsItem
{
    private GameObject _rabbitRacingObj;

    public Transform RabbitRacingTrans
    {
        get { return _balloonRacingObj.transform;}
    }

    private LocalizeTextMeshProUGUI _rabbitRacingTxt;

    private int _rabbitNum;
        
    private void AwakeRabbitRacing()
    {
        _rabbitRacingObj = transform.Find("StartActivity/RabbitRacing/Icon").gameObject;
        _rabbitRacingTxt = transform.Find("StartActivity/RabbitRacing/Text").GetComponent<LocalizeTextMeshProUGUI>();
        SetRabbitRacingState();
        EventDispatcher.Instance.AddEventListener(EventEnum.RABBIT_RACING_JOIN, UpdateRabbitRacing);
    }

    private void RefreshRepeatingRabbitRacing()
    {
        SetRabbitRacingState();
    }

    public Transform GetRabbitRacing()
    {
        return RabbitRacingTrans;
    }

    private void UpdateRabbitRacing(BaseEvent e)
    {
        InitRabbitRacing(storageTaskItem);
    }

    private void InitRabbitRacing(StorageTaskItem storageOrderItem)
    {
        if (RabbitRacingModel.Instance.IsShowReward())
        {
            _rabbitNum = RabbitRacingModel.Instance.GetRandomTaskScore(storageOrderItem);
            if (_rabbitNum > 0)
            {
                _rabbitRacingTxt.SetText(_rabbitNum.ToString());
            }
        }
    }

    private void SetRabbitRacingState()
    {
        bool isActive = RabbitRacingModel.Instance.IsShowReward() && _rabbitNum > 0;
        if (_rabbitRacingObj.transform.parent.gameObject.activeInHierarchy != isActive)
        {
            _rabbitRacingObj.transform.parent.gameObject.SetActive(isActive);
        }
    }
}