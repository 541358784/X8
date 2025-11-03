using Scripts.UI;

namespace MiniGame
{
    public partial class MiniGameModel
    {
        public void MiniGameHandle()
        {
            if (Framework.UI.UIManager.Instance.GetView<UIChapter>() == null)
                return;

            if (!GuideSubSystem.Instance.isFinished(1))
            {
                GuideSubSystem.Instance.ForceFinished(1);
            }

            if (!GuideSubSystem.Instance.isFinished(4))
            {
                EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, true, true);
            }
            
            if(GuideSubSystem.Instance.isFinished(203) && !GuideSubSystem.Instance.isFinished(204))
                EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, true, true);
            
            if(GuideSubSystem.Instance.isFinished(253) && !GuideSubSystem.Instance.isFinished(254))
                EventDispatcher.Instance.DispatchEvent(EventMiniGame.MINIGAME_SETSHOWSTATUS, true, true);
        }
    }
}