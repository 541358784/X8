using DigTrench;
using DragonPlus;
using DragonU3DSDK.Asset;
using Psychology;
using Psychology.Model;

namespace Gameplay.UI.MiniGame
{
    public class UIMiniGamePsychology : MiniGameViewBase
    {
        protected override void InitInfos()
        {
            var setting = Makeover.Utils.GetMiniGameSettings();
            var miniGameSetting = setting.Find(a => a.type == (int)_miniGameType);
            if(miniGameSetting == null)
                return;

            int index = 1;
            foreach (var id in miniGameSetting.childIds)
            {
                var config = PsychologyConfigManager.Instance._configs.Find(a => a.id == id);
                if(config == null)
                    continue;
                
                MiniGameInfo info = new MiniGameInfo();

                info._id = id;
                info._name = LocalizationManager.Instance.GetLocalizedString("ui_asmr_level_num") + index;
                info._sprite = ResourcesManager.Instance.GetSpriteVariant("BlueBlockAtlas", config.icon);
                info._isUnLock = PsychologyModel.Instance.IsUnLock(config);
                info._isFinish = PsychologyModel.Instance.IsFinish(config);
                info.unlockNum = config.unlockNodeNum;
                info.needRv = false;
                _infos.Add(info);
                index++;
            }
        }

        protected override void OnClickPlay(MiniGameInfo info)
        {    
            var config = PsychologyConfigManager.Instance._configs.Find(a => a.id == info._id);
            if(config == null)
                return;
            
            SceneFsm.mInstance.ChangeState(StatusType.EnterPsychology, config, false);
        }
    }
}