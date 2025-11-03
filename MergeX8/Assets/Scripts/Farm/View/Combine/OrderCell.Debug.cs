using DragonPlus;
using Farm.Model;
using Farm.Order;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public partial class OrderCell
    {
        private GameObject _debugObj;
        private Text _debugText_Id;
        private Text _debugText_Info;

        private void InitDebug()
        {
            _debugObj = transform.Find("DebugInfo").gameObject;
            _debugObj.gameObject.SetActive(false);

            _debugText_Id = transform.Find("DebugInfo/id").GetComponent<Text>();
            _debugText_Info = transform.Find("DebugInfo/reward").GetComponent<Text>();
        }

        private void UpdateDebugInfo()
        {
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
                return;

            if (!FarmModel.Instance.Debug_OpenModule)
                return;

            _debugText_Id.text = string.Format("ID:{0}", _storage.Id + "  Slot:" + _storage.Slot);

            _debugText_Info.text = "";
            for (int i = 0; i < _storage.RewardIds.Count; i++)
            {
                _debugText_Info.text += string.Format("奖励{0}: I:{1}  N:{2}", i, _storage.RewardIds[i], _storage.RewardNums[i]);
                if (i < _storage.RewardIds.Count)
                    _debugText_Info.text += "\n";
            }

            for (int i = 0; i < _storage.NeedItemIds.Count; i++)
            {
                _debugText_Info.text += string.Format("物品{0}: I:{1}  N:{2}", i, _storage.NeedItemIds[i], _storage.NeedItemNums[i]);
                if (i < _storage.NeedItemIds.Count)
                    _debugText_Info.text += "\n";
            }
        }
        
        private void Event_DebugOrderOpen(BaseEvent e)
        {
            if (ConfigurationController.Instance.version == VersionStatus.RELEASE)
                return;

            _debugObj.gameObject.SetActive(FarmModel.Instance.Debug_OpenModule);
            UpdateDebugInfo();
        }
    }
}