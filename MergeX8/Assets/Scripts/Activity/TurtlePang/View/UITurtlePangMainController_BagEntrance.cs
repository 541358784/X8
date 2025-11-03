using System;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UITurtlePangMainController
{
    public Button BagBtn;
    public TurtlePangBagRedPoint BagRedPoint;

    public void InitBagEntrance()
    {
        BagBtn = GetItem<Button>("Root/ButtonWarehouse");
        BagBtn.onClick.AddListener(() =>
        {
            if (ExchangeGroup.gameObject.activeSelf)
                return;
            ShowBag().WrapErrors();
        });
        BagRedPoint = transform.Find("Root/ButtonWarehouse/RedPoint").gameObject.AddComponent<TurtlePangBagRedPoint>();
        BagRedPoint.gameObject.SetActive(true);
        BagRedPoint.Init(Storage);
    }
    public class TurtlePangBagRedPoint:MonoBehaviour
    {
        private StorageTurtlePang Storage;
        public void Init(StorageTurtlePang storage)
        {
            Storage = storage;
            UpdateView();
        }

        private TurtlePangModel Model => TurtlePangModel.Instance;
        public void UpdateView()
        {
            var show = false;
            foreach (var pair in Storage.Bag)
            {
                if (pair.Value >= Model.GlobalConfig.TurtleExchangeScore[0])
                {
                    show = true;
                    break;
                }
            }
            gameObject.SetActive(show);
        }
        public void OnBagItemChange(EventTurtlePangBagItemChange evt)
        {
            UpdateView();
        }
        private void Awake()
        {
            EventDispatcher.Instance.AddEvent<EventTurtlePangBagItemChange>(OnBagItemChange);
        }

        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEvent<EventTurtlePangBagItemChange>(OnBagItemChange);
        }
    }
}