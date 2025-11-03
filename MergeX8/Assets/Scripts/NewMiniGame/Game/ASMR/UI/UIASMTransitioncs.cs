using Framework.UI;

namespace ASMR
{
    public class UIASMRTransition : UIView
    {
        public static void Open()
        {
            Framework.UI.UIManager.Instance.Open<UIASMRTransition>("NewMiniGame/UIMiniGame/Prefab/UIASMRTransition");
        }
    }
}