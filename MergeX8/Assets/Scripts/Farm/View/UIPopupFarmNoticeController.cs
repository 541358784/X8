using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config.Farm;
using Farm.Model;
using Farm.Order;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.View
{
    public class UIPopupFarmNoticeController : UIWindowController
    {
        private Button _closeButton;
        private Button _buyButton;

        private GameObject item;

        private LocalizeTextMeshProUGUI _text;

        private List<GameObject> _cells = new List<GameObject>();

        private int _costId;
        
        public override void PrivateAwake()
        {
            item = transform.Find("Root/Content/Item").gameObject;
            item.gameObject.SetActive(false);

            _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _closeButton.onClick.AddListener(() => AnimCloseWindow());

            _buyButton = transform.Find("Root/Button").GetComponent<Button>();
            _buyButton.onClick.AddListener(() =>
            {
                FarmModel.Instance.AutoSelectGround(_costId);
                AnimCloseWindow();
            });

            _text = transform.Find("Root/BG/Text").GetComponent<LocalizeTextMeshProUGUI>();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            base.OnOpenWindow(objs);

            foreach (GameObject cell in _cells)
            {
                DestroyImmediate(cell);
            }
            _cells.Clear();
            
            
            List<ResData> resDatas = (List<ResData>)objs[0];
            for (var i = 0; i < resDatas.Count; i++)
            {
                int id = resDatas[i].id;
                int num = resDatas[i].count;

                _costId = id;
                
                var obj = Instantiate(item, item.transform.parent);
                obj.gameObject.SetActive(true);

                int itemNum = FarmModel.Instance.GetProductItemNum(id);

                var productConfig = FarmConfigManager.Instance.GetFarmProductConfig(id);
                if (productConfig != null)
                    obj.transform.Find("Icon").GetComponent<Image>().sprite = FarmModel.Instance.GetFarmIcon(productConfig.Image);
                obj.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(itemNum + "/" + num);
                
                _cells.Add(obj);
            }

            int productId = (int)objs[1];
            var config = FarmConfigManager.Instance.GetFarmProductConfig(productId);
            if(config != null)
                _text.SetTerm(config.NameKey);
        }

        public static void OpenNotice(int id, int num, int productId)
        {
            List<ResData> resDatas = new List<ResData>();
            resDatas.Add(new ResData(id, num));
            
            UIManager.Instance.OpenUI(UINameConst.UIPopupFarmNotice, resDatas, productId);
        }
    }
}