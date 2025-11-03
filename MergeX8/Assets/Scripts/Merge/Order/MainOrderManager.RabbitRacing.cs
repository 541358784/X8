using Activity.RabbitRacing.Dynamic;
using DragonU3DSDK.Storage;

namespace Merge.Order
{
    public partial class MainOrderManager
    {
        private void AddRabbitRacing(StorageTaskItem orderItem, MergeTaskTipsItem order)
        {
            if (!RabbitRacingModel.Instance.IsOpened())
                return;

            if (!RabbitRacingModel.Instance.IsJoinRacing())
                return;

            RabbitRacingModel.Instance.AddRandomTaskScore(orderItem.Slot, CommonUtils.GetTaskValue(orderItem),
                order.GetRabbitRacing().transform.position);
        }
    }
}