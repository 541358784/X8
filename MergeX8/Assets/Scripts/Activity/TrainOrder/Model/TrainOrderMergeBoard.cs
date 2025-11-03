using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;


namespace Activity.TrainOrder
{

    public class TrainOrderMergeBoard : MergeBoard
    {
        public override void InitBoardId()
        {
            SetBoardID((int)MergeBoardEnum.TrainOrder);
        }
    }
}