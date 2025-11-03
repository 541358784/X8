using DragonPlus.Config.Farm;

namespace Farm.Model
{
    public partial class FarmModel
    {
        public void AutoSelect(int productId)
        {
            var type = FarmConfigManager.Instance.GetItemType(productId);

            switch (type)
            {
                case FarmType.Animal:
                {
                    AutoSelectAnim(productId, false);
                    break;
                }
                case FarmType.Tree:
                {
                    AutoSelectTree(productId, false);
                    break;
                }
                case FarmType.Ground:
                {
                    AutoSelectGround(productId, false);
                    break;
                }
                case FarmType.Machine:
                {
                    AutoSelectMachine(productId, false);
                    break;
                }
            }
        }
    }
}