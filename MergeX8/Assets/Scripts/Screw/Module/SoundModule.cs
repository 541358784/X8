
using DragonPlus;

namespace Screw
{
    public class SoundModule
    {
        public static void PlaySfx(string name)
        {
            AudioManager.Instance.PlaySoundByPath($"Screw/Audios/{name}", false);
        }

        public static void PlayButtonClick()
        {
            AudioManager.Instance.PlaySound(SfxNameConst.button_s);
        }
    }
}