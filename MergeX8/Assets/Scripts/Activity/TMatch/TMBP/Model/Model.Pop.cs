//-----------------------------------
//creator     :   xiaozejun
//time        :   2023年10月24日 星期二
//describe    :   
//-----------------------------------

using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using OutsideGuide;

namespace TMatch
{
    /// <summary>
    /// 弹出相关
    /// </summary>
    public partial class TMBPModel
    {
        /// <summary>
        /// 弹出购买界面
        /// </summary>
        /// <returns></returns>
        public bool PopBpBuy()
        {
            if (CanOpenBuyView())
            {
                TM_BPBuyView.Open(new BPBuyOpenData { TaskPop = true });
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否可以打开购买界面
        /// </summary>
        /// <returns></returns>
        public bool CanOpenBuyView()
        {
            if (!IsOpened(false)) return false;
            
            if (HaveBuy()) return false;

            if (DecoGuideManager.Instance.IsRunning)
                return false;

            if (UIManager.Instance.GetOpenedWindow<UILobbyView>())
                return false;

            if (Utils.IsSameDay((long)APIManager.Instance.GetServerTime() / 1000, (long)Data.LastPopBuyTime / 1000))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 弹出bp界面
        /// </summary>
        /// <returns></returns>
        public bool PopBpView()
        {
            if (CanOpenBpView())
            {
                TM_BPMainView.Open();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否可以打开Bp界面
        /// </summary>
        /// <returns></returns>
        public bool CanOpenBpView()
        {
            if (!IsOpened(false)) return false;
            
            if (DecoGuideManager.Instance.IsRunning)
                return false;

            int curLv = GetCurLevel();
            return curLv > Data.LevelViewed && gameWinExp == 0;
        }
    }
}