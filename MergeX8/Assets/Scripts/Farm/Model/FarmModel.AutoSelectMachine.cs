using Deco.Node;
using Decoration;
using DragonPlus.Config.Farm;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void AutoSelectMachine(int productId, bool isAutoTouch = true)
        {
            DecoNode selectNode = GetMachineNode(productId);
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
        
        private DecoNode GetMachineNode(int productId)
        {
            TableFarmMachine machineConfig = null;
            foreach (var config in FarmConfigManager.Instance.TableFarmMachineList)
            {
                foreach (var id in config.MachineOrder)
                {
                    var orderConfig = FarmConfigManager.Instance.TableFarmMachineOrderList.Find(a => a.Id == id);
                    if(orderConfig == null)
                        continue;
                    
                    if(orderConfig.ProductItem != productId)
                        continue;
                    
                    machineConfig = config;
                    break;
                }
                
                if(machineConfig != null)
                    break;
            }
            
            if(machineConfig == null)
                return null;

            return DecoManager.Instance.FindNode(machineConfig.DecoNode);
        }
    }
}