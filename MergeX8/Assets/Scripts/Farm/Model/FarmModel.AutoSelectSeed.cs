using System.Collections.Generic;
using Deco.Node;
using Decoration;
using DragonPlus.Config.Farm;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void AutoSelectGround(int productId, bool isAutoTouch = true)
        {
            DecoNode selectNode = GetGroundNode(productId);
            if(selectNode == null && !isAutoTouch)
                selectNode = GetGroundNode(productId, false);

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

        private DecoNode GetGroundNode(int productId, bool isFree = true)
        {
            TableFarmSeed seedConfig = null;
            foreach (var config in FarmConfigManager.Instance.TableFarmSeedList)
            {
                if(config.ProductItem != productId)
                    continue;

                seedConfig = config;
                break;
            }
            
            if(seedConfig == null)
                return null;

            List<DecoNode> decoNodes = new List<DecoNode>();
            foreach (var tableFarmGround in FarmConfigManager.Instance.TableFarmGroundList)
            {
                if(tableFarmGround.Type != seedConfig.Type)
                    continue;
                
                decoNodes.Add(DecoManager.Instance.FindNode(tableFarmGround.DecoNode));
            }

            DecoNode selectNode = null;
            foreach (var node in decoNodes)
            {
                if (isFree)
                {
                    if(!node.IsOwned)
                        continue;
                
                    var status = FarmModel.Instance.GetGroundProductStatus(node);
                    if(isFree && status != FarmProductStatus.Free)
                        continue;
                }

                selectNode = node;
                break;
            }

            return selectNode;
        }
    }
}