using DragonPlus;
using DragonPlus.Config.JungleAdventure;
using DragonU3DSDK.Asset;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Activity.JungleAdventure.Controller
{
    public class UIPopupJungleAdventureRewardController : UIWindowController
    {
        private Button _close;
        private Image _boxImage;

        private GameObject _item;
        public override void PrivateAwake()
        {
            _item = transform.Find("Root/ItemGroup/Item").gameObject;
            _item.gameObject.SetActive(false);
            
            _boxImage= transform.Find("Root/Box1").GetComponent<Image>();
            
            _close= transform.Find("Root/ButtonClose").GetComponent<Button>();
            _close.onClick.AddListener(()=>AnimCloseWindow());
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            int stage = (int)objs[0];
            
            var config = JungleAdventureConfigManager.Instance.GetConfigByStage(stage);

            if (config == null)
                return;
            
            foreach (Transform child in _item.transform.parent)
            {
                if(child.gameObject == _item)
                    continue;
                DestroyImmediate(child.gameObject);
            }
            
            for (var i = 0; i < config.RewardIds.Count; i++)
            {
                GameObject reward = Instantiate(_item, _item.transform.parent, false);
                reward.gameObject.SetActive(true);
                    
                reward.transform.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(config.RewardIds[i]);
                reward.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>().SetText(config.RewardNums[i].ToString());
            }
            
            _boxImage.sprite = ResourcesManager.Instance.GetSpriteVariant("JungleAdventureAtlas", config.FinalImage); 
        }
    }
}