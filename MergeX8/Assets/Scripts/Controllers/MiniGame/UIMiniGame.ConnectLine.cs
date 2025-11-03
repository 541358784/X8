using ConnectLine;
using ConnectLine.Model;
using DragonPlus;
using DragonU3DSDK.Asset;

namespace Gameplay.UI.MiniGame
{
    public class UIMiniGameConnectLine : MiniGameViewBase
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
                var config = ConnectLineConfigManager.Instance._configs.Find(a => a.id == id);
                if(config == null)
                    continue;
                
                MiniGameInfo info = new MiniGameInfo();

                info._id = id;
                info._name = LocalizationManager.Instance.GetLocalizedString("ui_asmr_level_num") + index;
                info._sprite = ResourcesManager.Instance.GetSpriteVariant("ConnectLineAtlas", config.icon);
                info._isUnLock = ConnectLineModel.Instance.IsUnLock(config);
                info._isFinish = ConnectLineModel.Instance.IsFinish(config);
                info.unlockNum = config.unlockNodeNum;
                info.needRv = info._isFinish;
                _infos.Add(info);

                index++;
            }
        }

        protected override void OnClickPlay(MiniGameInfo info)
        {
            var config = ConnectLineConfigManager.Instance._configs.Find(a => a.id == info._id);
            if(config == null)
                return;
            SceneFsm.mInstance.ChangeState(StatusType.EnterConnectLine, config, false);
        }
    }
}