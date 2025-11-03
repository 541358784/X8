using Framework;

namespace Screw
{
    public class ScrewGameModel : Singleton<ScrewGameModel>
    {
        
        private const int BaseScrewResId = 200000000;
        public bool IsScrewResId(int resourceId)
        {
            return resourceId > BaseScrewResId && resourceId < 300000000;
        }
        public int ChangeToScrewId(int resourceId)
        {
            return resourceId - BaseScrewResId;
        }
    }
}