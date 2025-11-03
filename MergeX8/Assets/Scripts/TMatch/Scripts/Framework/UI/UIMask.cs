
using DragonU3DSDK;

namespace TMatch
{


    public class UIMask : UIWindow
    {
        public override bool IsUsedInTaskChokedEvent => false;
        public override UIWindowLayer WindowLayer => UIWindowLayer.Guide;
        private static int count = 0;

        public static void Enable(bool enable)
        {
            count += enable ? 1 : -1;

            if (count < 0)
            {
                DebugUtil.LogError($"UIMask count < 0:{count}");
                count = 0;
            }

            if (enable && count == 1)
                UIManager.Instance.OpenWindow<UIMask>("TMatch/Prefabs/UIMask");
            else if (!enable && count == 0)
                UIManager.Instance.CloseWindow<UIMask>();

            DebugUtil.Log($"UIMask.Enable:{enable}, {count}");
        }

        public override void PrivateAwake()
        {
            EffectUIAnimation = false;
            isPlayDefaultOpenAudio = false;
            isPlayDefaultCloseAudio = false;
        }
    }
}
