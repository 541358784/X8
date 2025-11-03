using DragonU3DSDK.Audio;
using AudioManager = DragonPlus.AudioManager;

namespace DigTrench
{
    public partial class DigTrenchModel
    {
        private bool WomanLoopPainEffectEnable = true;
        private int WomanLoopPainEffectId = -1;
        public async void PlayWomanLoopPainEffect()
        {
            await XUtility.WaitSeconds(8f);
            while (this && WomanLoopPainEffectEnable)
            {
                WomanLoopPainEffectId = AudioManager.Instance.PlaySound("sfx_dig_ache");
                await XUtility.WaitSeconds(17.233f);
            }
        }

        public void StopWomanLoopPainEffect()
        {
            WomanLoopPainEffectEnable = false;
            AudioManager.Instance.StopSoundById(WomanLoopPainEffectId);
        }

        public void PlayDigEffect()
        {
            AudioManager.Instance.PlaySound("sfx_dig_digging");
        }
        public void PlaySaveFish()
        {
            AudioManager.Instance.PlaySound("sfx_dig_fish");
        }

        public void PlayGetProps()
        {
            AudioManager.Instance.PlaySound("sfx_dig_use_item");
        }

        public void BreakObstacle()
        {
            AudioManager.Instance.PlaySound("sfx_dig_get_item");
        }

        public void PlayPlantGrow()
        {
            AudioManager.Instance.PlaySound("sfx_dig_growth");
        }
        public void PlayWomanCelebrateEffect()
        {
            AudioManager.Instance.PlaySound("sfx_dig_celebrate");
        }
        public void PlayWomanWinEffect()
        {
            AudioManager.Instance.PlaySound("sfx_dig_yes");
        }
    }
}