using Deco.Node;
using DragonPlus.Config.Farm;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void Speed(DecoNode node, int time)
        {
            if(node == null)
                return;
            
            var type = FarmConfigManager.Instance.GetDecoNodeType(node.Id);
            switch (type)
            {
                case FarmType.Ground:
                {
                    SpeedSeed(node, time);
                    break;
                }
                case FarmType.Machine:
                {
                    SpeedMachine(node, time);
                    break;
                }
                case FarmType.Animal:
                {
                    SpeedAnimal(node, time);
                    break;
                }
                case FarmType.Tree:
                {
                    SpeedTree(node, time);
                    break;
                }
            }
            
            EventDispatcher.Instance.DispatchEvent(EventEnum.FARM_SPEED_UP, type, node);
        }
    }
}