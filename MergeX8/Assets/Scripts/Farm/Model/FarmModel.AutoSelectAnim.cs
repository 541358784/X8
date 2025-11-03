using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DragonPlus.Config.Farm;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void AutoSelectAnim(int productId, bool isAutoTouch = true)
        {
            DecoNode selectNode = GetAnimalNode(productId);
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
        
        private DecoNode GetAnimalNode(int productId)
        {
            TableFarmAnimal animalConfig = null;
            foreach (var config in FarmConfigManager.Instance.TableFarmAnimalList)
            {
                if(config.ProductItem != productId)
                    continue;

                animalConfig = config;
                break;
            }
            
            if(animalConfig == null)
                return null;

            return DecoManager.Instance.FindNode(animalConfig.DecoNode);
        }
    }
}