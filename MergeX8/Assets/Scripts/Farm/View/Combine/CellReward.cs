using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Farm.Model;
using Farm.Order;
using Gameplay;
using MagneticScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
  public class RewardItem :  IInitContent
    {
        public Transform _root;
        public Image _icon;
        public LocalizeTextMeshProUGUI _text;
        public LocalizeTextMeshProUGUI _activity_timeOrderText;

        public int _rewardId;
        private int _rewardNum;
        public RewardItem(Transform root)
        {
            _root = root;

            _icon = _root.Find("Image").GetComponent<Image>();
            _text = _root.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _activity_timeOrderText = _root.Find("TextTimeLimitOrder").GetComponent<LocalizeTextMeshProUGUI>();
        }

        public void InitContent(object content)
        {
            ResData data = (ResData)content;
            
            _rewardId = data.id;
            _rewardNum = data.count;

            if (_rewardId == (int)UserData.ResourceId.Farm_Exp)
                _icon.sprite = UserData.GetResourceIcon(_rewardId);
            else
            {
                if (UserData.Instance.IsFarmProp(_rewardId))
                {
                    var propConfig = FarmConfigManager.Instance.TableFarmPropList.Find(a => a.Id == _rewardId);
                    if(propConfig != null)
                        _icon.sprite = FarmModel.Instance.GetFarmIcon(propConfig.Image);
                }
                else
                {
                    TableFarmProduct config = FarmConfigManager.Instance.GetFarmProductConfig(_rewardId);
                    if(config != null)
                        _icon.sprite = FarmModel.Instance.GetFarmIcon(config.Image);
                    else
                    {
                        _icon.sprite = UserData.GetResourceIcon(_rewardId);
                    }
                }
            }
        }

        public void UpdateData(params object[] param)
        {
            _text.SetText(_rewardNum.ToString());
            _activity_timeOrderText.SetText(_rewardNum.ToString());
        }
    }


    public class CellReward : IInitContent
    {
        private Transform _root;
        private GameObject _item;
        private StorageFarmOrderItem _storage;
        private List<RewardItem> _rewardItems = new List<RewardItem>();

        public CellReward(Transform root)
        {
            _root = root;

            _item = _root.Find("item").gameObject;
            _item.gameObject.SetActive(false);
        }

        public void InitContent(object content)
        {
            _storage = (StorageFarmOrderItem)content;

            foreach (var needItem in _rewardItems)
            {
                needItem._root.gameObject.SetActive(false);
            }

            int num = _storage.RewardIds.Count - _rewardItems.Count;
            for (int i = 0; i < num; i++)
            {
                GameObject item = GameObject.Instantiate(_item, _root.transform);
                item.gameObject.SetActive(false);

                var data = new RewardItem(item.transform);
                _rewardItems.Add(data);
            }

            for (int i = 0; i < _storage.RewardIds.Count; i++)
            {
                _rewardItems[i]._root.gameObject.SetActive(true);
                _rewardItems[i].InitContent(new ResData(_storage.RewardIds[i], _storage.RewardNums[i]));
                
                _rewardItems[i]._text.gameObject.SetActive(_storage.Slot != (int)OrderSlot.Activity_TimeOrder);
                _rewardItems[i]._activity_timeOrderText.gameObject.SetActive(_storage.Slot == (int)OrderSlot.Activity_TimeOrder);
            }
        }

        public void UpdateData(params object[] param)
        {
            for (int i = 0; i < _storage.RewardIds.Count; i++)
                _rewardItems[i].UpdateData(_storage.RewardNums[i]);
        }

        public GameObject GetRewardIcon(int id)
        {
            var item = _rewardItems.Find(a => a._rewardId == id);
            if (item == null)
                return null;

            return item._icon.gameObject;
        }
    }
}