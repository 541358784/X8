using DigTrench;
using DragonPlus;
using DragonPlus.Config.DigTrench;
using DragonU3DSDK.Asset;

namespace Gameplay.UI.MiniGame
{
    public class UIMiniGameDigTrench : MiniGameViewBase
    {
        protected override void InitInfos()
        {
            var setting = Makeover.Utils.GetMiniGameSettings();
            var miniGameSetting =setting.Find(a => a.type == (int)_miniGameType);
            if(miniGameSetting == null)
                return;

            int index = 1;
            foreach (var id in miniGameSetting.childIds)
            {
                var config = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == id);
                if(config == null)
                    continue;
                
                MiniGameInfo info = new MiniGameInfo();

                info._id = id;
                info._name = LocalizationManager.Instance.GetLocalizedString("ui_asmr_level_num") + index;
                info._sprite = ResourcesManager.Instance.GetSpriteVariant("DigTrenchAtlas", config.icon);
                info._isUnLock = DigTrenchEntryControllerModel.Instance.IsUnLock(config);
                info._isFinish = DigTrenchEntryControllerModel.Instance.IsFinish(config);
                info.unlockNum = config.unlockNodeNum;
                info.needRv = info._isFinish;
                _infos.Add(info);
                index++;
            }
        }

        protected override void OnClickPlay(MiniGameInfo info)
        {    
            var config = DigTrenchConfigManager.Instance.DigTrenchLevelList.Find(a => a.id == info._id);
            if(config == null)
                return;
            
            SceneFsm.mInstance.ChangeState(StatusType.DigTrench, config, false);
        }
    }
}