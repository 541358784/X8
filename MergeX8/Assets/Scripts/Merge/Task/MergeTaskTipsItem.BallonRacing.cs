using Activity.BalloonRacing;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public partial class MergeTaskTipsItem
{
    private GameObject _balloonRacingObj;

    public Transform BalloonRacingTrans
    {
        get { return _balloonRacingObj.transform;}
    }

    private LocalizeTextMeshProUGUI _balloonRacingTxt;

    private int _balloonNum;
        
    private void AwakeBalloonRacing()
    {
        _balloonRacingObj = transform.Find("StartActivity/BalloonRacing/Icon").gameObject;
        _balloonRacingTxt = transform.Find("StartActivity/BalloonRacing/Text").GetComponent<LocalizeTextMeshProUGUI>();
        SetBalloonRacingState();
        EventDispatcher.Instance.AddEventListener(EventEnum.BALLOON_RACING_JOIN, UpdateBalloonRacing);
    }

    private void RefreshRepeatingBalloonRacing()
    {
        SetBalloonRacingState();
    }

    public Transform GetBalloonRacing()
    {
        return BalloonRacingTrans;
    }

    private void UpdateBalloonRacing(BaseEvent e)
    {
        InitBalloonRacing(storageTaskItem);
    }

    private void InitBalloonRacing(StorageTaskItem storageOrderItem)
    {
        if (BalloonRacingModel.Instance.IsShowReward())
        {
            _balloonNum = BalloonRacingModel.Instance.GetRandomTaskScore(storageOrderItem);
            if (_balloonNum > 0)
            {
                _balloonRacingTxt.SetText(_balloonNum.ToString());
            }
        }
    }

    private void SetBalloonRacingState()
    {
        bool isActive = BalloonRacingModel.Instance.IsShowReward() && _balloonNum > 0;
        if (_balloonRacingObj.transform.parent.gameObject.activeInHierarchy != isActive)
        {
            _balloonRacingObj.transform.parent.gameObject.SetActive(isActive);
        }
    }
}