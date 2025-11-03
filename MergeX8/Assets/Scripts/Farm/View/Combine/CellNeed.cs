using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Storage;
using Facebook.Unity;
using Farm.Model;
using Farm.Order;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class NeedItem :  IInitContent
    {
        public Transform _root;
        public Image _icon;
        public LocalizeTextMeshProUGUI _text;
        public GameObject _finish;

        private int _needId;
        private int _needNum;
        private OrderCell _content;
        public NeedItem(Transform root, OrderCell content)
        {
            _root = root;
            _content = content;
            
            _icon = _root.Find("Icon").GetComponent<Image>();
            _text = _root.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
            _finish = _root.Find("Finish").gameObject;
            
            _root.GetComponent<Button>().onClick.AddListener(() =>
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupFarmItemInformation, _needId);
            });
        }

        public void InitContent(object content)
        {
            ResData data = (ResData)content;
            
            _needId = data.id;
            _needNum = data.count;

            var config = FarmConfigManager.Instance.GetFarmProductConfig(_needId);
            if (config != null)
                _icon.sprite = FarmModel.Instance.GetFarmIcon(config.Image);
        }

        public void UpdateData(params object[] param)
        {
            if(!_content._isClickFinish)
            _text.SetText(FarmModel.Instance.GetProductItemNum(_needId) + "/"+ _needNum);
            _finish.gameObject.SetActive(FarmModel.Instance.HavEnoughProduct(_needId, _needNum));
        }

        public bool IsEnough()
        {
            if (FarmModel.Instance.Debug_CompleteAllOrder)
                return true;

            return FarmModel.Instance.HavEnoughProduct(_needId, _needNum);
        }
    }
    
    
    public class CellNeed : IInitContent 
    {
        private Transform _root;
        private GameObject _item;
        private StorageFarmOrderItem _storage;
        private List<NeedItem> _needItems = new List<NeedItem>();
        private OrderCell _content;
        
        public CellNeed(Transform root, OrderCell content)
        {
            _root = root;
            _content = content;
            
            _item = _root.Find("item").gameObject;
            _item.gameObject.SetActive(false);
        }
        public void InitContent(object content)
        {
            _storage = (StorageFarmOrderItem)content;
            
            foreach (var needItem in _needItems)
            {
                needItem._root.gameObject.SetActive(false);
            }

            int num = _storage.NeedItemIds.Count-_needItems.Count;
            for (int i = 0; i < num; i++)
            {
                GameObject item = GameObject.Instantiate(_item, _root.transform);
                item.gameObject.SetActive(false);

                var data = new NeedItem(item.transform, _content);
                _needItems.Add(data);
            }
            
            for (int i = 0; i < _storage.NeedItemIds.Count; i++)
            {
                _needItems[i]._root.gameObject.SetActive(true);
                _needItems[i].InitContent(new ResData(_storage.NeedItemIds[i], _storage.NeedItemNums[i]));
            }
        }

        public void UpdateData(params object[] param)
        {
            for (int i = 0; i < _storage.NeedItemIds.Count; i++)
                _needItems[i].UpdateData();
        }

        public bool IsEnough()
        {
            bool isEnough = true;
            foreach (var needItem in _needItems)
            {
                if(!needItem._root.gameObject.activeSelf)
                    continue;
                
                if(needItem.IsEnough())
                    continue;

                isEnough = false;
            }

            return isEnough;
        }
    }
}