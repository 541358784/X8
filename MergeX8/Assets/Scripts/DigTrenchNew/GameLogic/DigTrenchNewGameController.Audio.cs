using DragonU3DSDK.Audio;
using DragonU3DSDK.Network.API;
using AudioManager = DragonPlus.AudioManager;

namespace DigTrenchNew
{
    public partial class DigTrenchNewGameController
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

        private ulong NextPlayDigEffectTime;
        public void PlayDigEffect()
        {
            var curTime = APIManager.Instance.GetServerTime();
            if (curTime > NextPlayDigEffectTime)
            {
                AudioManager.Instance.PlaySound("sfx_dig_digging");
                NextPlayDigEffectTime = curTime + XUtility.Second/1;
            }
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