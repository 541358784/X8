using Deco.Node;
using Decoration;
using DragonPlus.Config.Farm;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void AutoSelectTree(int productId, bool isAutoTouch = true)
        {
            DecoNode selectNode = GetTreeNode(productId);
            if(selectNode == null)
                return;
            
            UIRoot.Instance.EnableEventSystem = false;
            DecoManager.Instance.CurrentWorld.LookAtSuggestNodeBySpeed(selectNode, true, DecoManager.Instance.CurrentWorld.PinchMap.CurrentCameraScale, 15, () =>
            {
                UIRoot.Instance.EnableEventSystem = true;
                
                if(isAutoTouch)
                    EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_TOUCH_NODE, selectNode);
            });
        }
        
        private DecoNode GetTreeNode(int productId)
        {
            TableFarmTree treeConfig = null;
            foreach (var config in FarmConfigManager.Instance.TableFarmTreeList)
            {
                if(config.ProductItem != productId)
                    continue;

                treeConfig = config;
                break;
            }
            
            if(treeConfig == null)
                return null;

            return DecoManager.Instance.FindNode(treeConfig.DecoNode);
        }
    }
}