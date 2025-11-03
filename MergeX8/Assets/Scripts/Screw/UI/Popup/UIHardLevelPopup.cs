using Screw;
using Screw.Module;
using Screw.UI;

namespace Screw
{
    [Window(UIWindowLayer.Normal, "Screw/Prefabs/PopUp/UIPopupHardLevel")]
    public class UIHardLevelPopup : UIWindowController
    {
        private ScrewGameContext _context;

        public override void PrivateAwake()
        {
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _context = (ScrewGameContext) objs[0];
            
            ScrewUtility.WaitSeconds(2, () =>
            {
                AnimCloseWindow(() =>
                {
                    _context.hookContext.OnLogicEvent(LogicEvent.EnterLevel, null);
                });
            }).Forget();
        }
    }
}