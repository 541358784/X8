using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using BiEventTileGarden = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;
namespace TileMatch.Game
{
    public partial class TileMatchGameManager
    {
        private GameBIManager.GameEventArgs _biArgs = new GameBIManager.GameEventArgs();
        private float _startTime = 0;
        private int _removeNum = 0;
        
        private void LevelStartBi()
        {
            _removeNum = 0;
            
            _startTime = Time.realtimeSinceStartup;
            _biArgs.level_id = (uint)_levelId;
            _biArgs.enter_time++;
            _biArgs.initial_thing_count = (uint)GetBlockNum();
            _biArgs.remove_thing_count = 0;
            _biArgs.left_thing_count = 0;
            _biArgs.total_time = (uint)GetTime();
            _biArgs.left_time_count = (uint)GetLeftTime();
            _biArgs.level_time = 0;
            _biArgs.fail_reason = 0;
            _biArgs.energy_infinite = EnergyModel.Instance.IsEnergyUnlimited();
            _biArgs.data1 = null;
            _biArgs.data2 = null;
            _biArgs.data3 = null;
            GameBIManager.Instance.SendGameMatchEvent(BiEventTileGarden.Types.GameEventType.GameEventLevelStart, _biArgs);
        }

        public void LevelFailedBi(FailTypeEnum type)
        {
            _biArgs.level_time = (uint)(Time.realtimeSinceStartup - _startTime);
            _biArgs.left_thing_count = _biArgs.initial_thing_count - _biArgs.remove_thing_count;
            
            _biArgs.data1 = null;
            _biArgs.data2 = null;
            _biArgs.data3 = null;
            
            switch (type)
            {
                case FailTypeEnum.EnterBack:
                    _biArgs.fail_reason = 1;
                    break;
                case FailTypeEnum.Time:
                    _biArgs.fail_reason = 3;
                    break;
                case FailTypeEnum.GridFull:
                    _biArgs.fail_reason = 2;
                    break;
                case FailTypeEnum.Special:
                    _biArgs.fail_reason = 4;
                    break;
                case FailTypeEnum.SpecialFail:
                    _biArgs.fail_reason = 5;
                    break;
            }
            GameBIManager.Instance.SendGameMatchEvent(BiEventTileGarden.Types.GameEventType.GameEventLevelFail, _biArgs);
        }

        private void LevelSuccessBi()
        {
            _biArgs.level_time = (uint)(Time.realtimeSinceStartup - _startTime);
            _biArgs.left_thing_count = _biArgs.initial_thing_count - _biArgs.remove_thing_count;
            _biArgs.left_time_count = (uint)GetLeftTime();
            
            _biArgs.data1 = null;
            _biArgs.data2 = null;
            _biArgs.data3 = null;
            
            GameBIManager.Instance.SendGameMatchEvent(BiEventTileGarden.Types.GameEventType.GameEventLevelComp, _biArgs);
        }

        private void UsePropBi(UserData.ResourceId resourceId)
        {
            switch (resourceId)
            {
                case UserData.ResourceId.Prop_Back:
                    _biArgs.use_prop_back++;
                    break;
                case UserData.ResourceId.Prop_SuperBack:
                    _biArgs.use_prop_superback++;
                    break;
                case UserData.ResourceId.Prop_Magic:
                    _biArgs.use_prop_magic++;
                    break;
                case UserData.ResourceId.Prop_Shuffle:
                    _biArgs.use_prop_shuffle++;
                    break;
                case UserData.ResourceId.Prop_Extend:
                    _biArgs.use_prop_extend++;
                    break;
            }
        }

        private void LevelResume()
        {
            _biArgs.space_respawn_count++;
        }
        
        private void LevelTimeResume()
        {
            _biArgs.time_respawn_count++;
        }
        
        private void BuyPropBi(UserData.ResourceId resourceId)
        {
            switch (resourceId)
            {
                case UserData.ResourceId.Prop_Back:
                    _biArgs.get_prop_back++;
                    break;
                case UserData.ResourceId.Prop_SuperBack:
                    _biArgs.get_prop_superback++;
                    break;
                case UserData.ResourceId.Prop_Magic:
                    _biArgs.get_prop_magic++;
                    break;
                case UserData.ResourceId.Prop_Shuffle:
                    _biArgs.get_prop_shuffle++;
                    break;
                case UserData.ResourceId.Prop_Extend:
                    _biArgs.get_prop_extend++;
                    break;
            }
        }

        private void RemoveBlockBi()
        {
            _biArgs.remove_thing_count += 3;

            _removeNum++;
            
            _biArgs.data1 = _collectBannerList.Count.ToString();
            _biArgs.data2 = _removeNum.ToString();
            GameBIManager.Instance.SendGameMatchEvent(BiEventTileGarden.Types.GameEventType.GameEventClickRemove, _biArgs);
        }
        
        private int GetBlockNum()
        {
            int num = 0;
            for (int i = 0; i < _allBlocks.Count; i++)
            {
                num += _allBlocks[i].GetBlockNum();
            }

            return num;
        }

        private void BuyPropSuccess(BaseEvent e)
        {
            UserData.ResourceId propId = (UserData.ResourceId)e.datas[0];
            int num = (int)e.datas[1];
            
            BuyPropBi(propId);
        }

        public void SelectDifficulty(int diff)
        {
            _biArgs.level_difficulty = (uint)diff;
        }
    }
}