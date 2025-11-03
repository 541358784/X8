using Stimulate.Event;

namespace Stimulate.Model.Merge
{
    public class StimulateMergeBoard : MergeBoard
    {
        public override void InitBoardId()
        {
            SetBoardID((int) MergeBoardEnum.Stimulate);
        }
        
        public override void OnNewItem(int index, int oldIndex, int id, int oldId, RefreshItemSource source)
        {
            base.OnNewItem(index, oldIndex, id, oldId, source);
            
            EventDispatcher.Instance.DispatchEvent(ConstEvent.Event_Refresh_Merge, id);
        }
    }
}