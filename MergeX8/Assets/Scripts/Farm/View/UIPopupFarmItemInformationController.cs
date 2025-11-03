using DragonPlus;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Asset;
using Farm.Model;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Farm.View
{
    public class UIPopupFarmItemInformationController : UIWindowController
    {
        private Image _icon;
        private LocalizeTextMeshProUGUI _titleText;
        private LocalizeTextMeshProUGUI _desText;
        private Transform _buildRoot;

        private Image _orgIcon;
        private Image _targetIcon;

        private Button _closeButton;
        private Button _getButton;

        private GameObject _arrowObj;

        private int _itemId;
        
        public override void PrivateAwake()
        {
            _icon = transform.Find("Root/Icon").GetComponent<Image>();
            _titleText = transform.Find("Root/TitleText").GetComponent<LocalizeTextMeshProUGUI>();
            _desText = transform.Find("Root/BG/Text").GetComponent<LocalizeTextMeshProUGUI>();

            _buildRoot = transform.Find("Root/Build");
            
            _orgIcon = transform.Find("Root/Item1/Icon").GetComponent<Image>();
            _targetIcon = transform.Find("Root/Item2/Icon").GetComponent<Image>();

            _arrowObj = transform.Find("Root/BG/Image (2)").gameObject;

            _closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            _closeButton.onClick.AddListener(() =>
            {
                AnimCloseWindow();
            });
            
            _getButton = transform.Find("Root/ButtonGroup/GetButton").GetComponent<Button>();
            _getButton.onClick.AddListener(() =>
            {
                AnimCloseWindow(() =>
                {
                    FarmModel.Instance.AutoSelect(_itemId);
                });
            });
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _itemId = (int)objs[0];

            var config = FarmConfigManager.Instance.GetFarmProductConfig(_itemId);
            _icon.sprite = FarmModel.Instance.GetFarmIcon(config.Image);
            _targetIcon.sprite = _icon.sprite;
            
            _titleText.SetTerm(config.NameKey);
            _desText.SetTerm(config.DesKey);

            var buildPrefab = ResourcesManager.Instance.LoadResource<GameObject>($"Farm/Prefabs/UI/{config.PrefabName}", addToCache: false);
            if (buildPrefab != null)
                Instantiate(buildPrefab, _buildRoot);

            _orgIcon.transform.parent.gameObject.SetActive(config.Original != null);
            _arrowObj.gameObject.SetActive(config.Original != null);
            
            if (config.Original != null)
            {
                _orgIcon.sprite = FarmModel.Instance.GetFarmIcon(config.Original);
            }
        }
    }
}