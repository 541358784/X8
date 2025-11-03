using Framework.UI;

namespace ASMR
{
    public class UIASMRFinish : UIView
    {
        public static void Open()
        {
            Framework.UI.UIManager.Instance.Open<UIASMRFinish>("NewMiniGame/UIMiniGame/Prefab/UIASMRFinish");
        }
    }
}