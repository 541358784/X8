using Activity.BalloonRacing;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public partial class MainOrderManager
    {
        private void AddBalloonRacing(StorageTaskItem orderItem, MergeTaskTipsItem order)
        {
            if (!BalloonRacingModel.Instance.IsOpened())
                return;

            if (!BalloonRacingModel.Instance.IsJoinRacing())
                return;

            BalloonRacingModel.Instance.AddRandomTaskScore(orderItem.Slot, CommonUtils.GetTaskValue(orderItem),
                order.GetBalloonRacing().transform.position);
        }
    }
}