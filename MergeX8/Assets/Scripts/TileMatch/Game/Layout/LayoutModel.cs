namespace TileMatch.Game.Layout
{
    public class LayoutModel : IDestroy
    {
        public Layout _layout;
        public LayoutData.Layout  _layoutData;
        public int _index;
        
        public LayoutModel(Layout layout, LayoutData.Layout layoutData, int index)
        {
            _layout = layout;
            _layoutData = layoutData;
            _index = index;
        }
        
        public void Destroy()
        {
        }
    }
}