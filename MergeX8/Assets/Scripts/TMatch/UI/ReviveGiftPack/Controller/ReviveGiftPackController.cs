
using Framework;

namespace TMatch
{
    public class ReviveGiftPackController : GlobalSystem<ReviveGiftPackController>, IInitable
    {
        public ReviveGiftPackModel model;
        public bool isInPurchase;  // 记录是否在购买状态
        public void Init()
        {
        
        }

        public void Release()
        {
        
        }

        public void InitModel()
        {
            model = new ReviveGiftPackModel();
        }
    }
}
