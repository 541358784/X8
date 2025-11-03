// using System.Collections.Generic;
// using Ditch.Model;
// using DragonPlus;
// using DragonPlus.Config.Ditch;
// using DragonU3DSDK.Asset;
// using Gameplay;
// using UnityEngine;
// using UnityEngine.UI;
//
// public partial class UIPopupTaskController
// {
//     private Transform _ditchContent;
//     private Transform _ditchItem;
//     
//     private class DitchItem
//     {
//         private Transform _root;
//         
//         private GameObject _normalObj;
//         private GameObject _unLockObj;
//         private GameObject _lockObj;
//         private GameObject _comingSoonObj;
//
//         private LocalizeTextMeshProUGUI _normalText;
//         private LocalizeTextMeshProUGUI _unLockText;
//         private LocalizeTextMeshProUGUI _lockText;
//
//         private Image _normalImage;
//         private Image _unLockImage;
//         private Image _lockImage;
//
//         private Button _button;
//
//         private TableDitchLevel _config;
//         
//         public void Init(Transform root, TableDitchLevel config, int index)
//         {
//             _root = root;
//             _config = config;
//             
//             _normalObj = _root.Find("Normal").gameObject;
//             _normalText = _root.Find("Normal/Text").GetComponent<LocalizeTextMeshProUGUI>();
//             _normalImage = _root.Find("Normal/Icon").GetComponent<Image>();
//                 
//             _lockObj = _root.Find("Lock").gameObject;
//             _lockText = _root.Find("Lock/Text").GetComponent<LocalizeTextMeshProUGUI>();
//             _lockImage = _root.Find("Lock/Icon").GetComponent<Image>();
//             
//             _unLockObj = _root.Find("NotHave").gameObject;
//             _unLockText = _root.Find("NotHave/Text").GetComponent<LocalizeTextMeshProUGUI>();
//             _unLockImage = _root.Find("NotHave/Icon").GetComponent<Image>();
//             
//             _comingSoonObj = _root.Find("ComingSoon").gameObject;
//
//             _normalText.SetTerm(config.LevelName);
//             _unLockText.SetTerm(config.LevelName);
//             
//             _lockText.SetText(string.Format(LocalizationManager.Instance.GetLocalizedString("ui_asmr_unlock_day"),config.UnlockNodeNum));
//             
//             _normalImage.sprite = ResourcesManager.Instance.GetSpriteVariant(AtlasName.CommonAtlas, config.LevelIcon);
//             _lockImage.sprite = _normalImage.sprite;
//             _unLockImage.sprite = _normalImage.sprite;
//             
//             _button = _root.Find("NotHave/Button").GetComponent<Button>();
//             _button.onClick.AddListener(() =>
//             {
//                 DitchModel.Instance.EnterLevel(config);
//             });
//
//             RefreshUI();
//         }
//         public void RefreshUI()
//         {
//             bool isUnlock = DitchModel.Instance.IsUnLockLevel(_config);
//             bool isFinish = DitchModel.Instance.IsFinishLevel(_config.Id);
//
//             _comingSoonObj.gameObject.SetActive(false);
//             _normalObj.gameObject.SetActive(false);
//             _unLockObj.gameObject.SetActive(false);
//             _lockObj.gameObject.SetActive(false);
//             
//             if (_config.IsComingSoon)
//             {
//                 _comingSoonObj.gameObject.SetActive(true);
//             }
//             else if (isFinish)
//             {
//                 _normalObj.gameObject.SetActive(true);
//             }
//             else if (isUnlock)
//             {
//                 _unLockObj.gameObject.SetActive(true);
//             }
//             else
//             {
//                 _lockObj.gameObject.SetActive(true);
//             }
//         }
//     }
//
//     
//     private List<DitchItem> _ditchItems = new List<DitchItem>();
//     private void Awake_Ditch()
//     {
//         _ditchContent = transform.Find("Root/Scroll View2/Viewport/Content");
//         _ditchItem = transform.Find("Root/Scroll View2/Viewport/Content/Item");
//         _ditchItem.gameObject.SetActive(false);
//     }
//
//     private void InitDitch()
//     {
//         for (int i = 0; i < DitchConfigManager.Instance.TableDitchLevelList.Count; i++)
//         {
//             var item = Instantiate(_ditchItem, _ditchContent.transform);
//             item.gameObject.SetActive(true);
//
//             DitchItem dayItem = new DitchItem();
//             dayItem.Init(item, DitchConfigManager.Instance.TableDitchLevelList[i], i);
//
//             _ditchItems.Add(dayItem);
//         }
//     }
//
//     public void RefreshDitchUI()
//     {
//         _ditchItems.ForEach(a=>a.RefreshUI());
//     }
// }