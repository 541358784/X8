using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine.UI;

namespace Merge.UnlockMergeLine
{
    public class UIPopupSyntheticChainUnlockController : UIWindowController
    {
        private Image _iconImage;
        private LocalizeTextMeshProUGUI _name;
        private LocalizeTextMeshProUGUI _tipName;
        private Button _closeButton;
        private TableUnlockMergeLine _config;
        private UnlockMergeLineManager.UnlockType _unlockType;
        
        public override void PrivateAwake()
        {
            _iconImage = GetItem<Image>("Root/Item/Icon");
            _name = GetItem<LocalizeTextMeshProUGUI>("Root/Item/Name");
            _tipName = GetItem<LocalizeTextMeshProUGUI>("Root/Text");
            _closeButton = GetItem<Button>("Root/Button");
            _closeButton.onClick.AddListener(OnClickCloseButton);
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            _config = (TableUnlockMergeLine)objs[0];
            _unlockType = (UnlockMergeLineManager.UnlockType)objs[1];

            var animObj = transform.Find(_config.animName);
            if(animObj != null)
                animObj.gameObject.SetActive(true);
            
            _tipName.SetTerm(_config.desKey);

            var config = GameConfigManager.Instance.GetItemConfig(_config.unlockMergeId);
            if(config == null)
                return;
            
            _iconImage.sprite = MergeConfigManager.Instance.mergeIcon.GetSprite(config.image);
            _name.SetTerm(config.name_key);
        }
        
        public void OnClickCloseButton()
        {
            if(!StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord.ContainsKey($"success{_config.id}"))
                StorageManager.Instance.GetStorage<StorageHome>().RcoveryRecord[$"success{_config.id}"] = true;
            
            AnimCloseWindow(() =>
            {
                if (_unlockType == UnlockMergeLineManager.UnlockType.unlockLine || _unlockType == UnlockMergeLineManager.UnlockType.level)
                {
                    bool isUnlock = UnlockMergeLineManager.Instance.UnLockMergeLine();
                    if (_unlockType == UnlockMergeLineManager.UnlockType.unlockLine)
                    {
                        if (!isUnlock)
                        {
                            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                            MergeGuideLogic.Instance.CheckMergeGuide();
                        }
                    }
                    else
                    {
                        if (!isUnlock)
                        {
                            EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                        }
                    }
                }
                else if (_unlockType == UnlockMergeLineManager.UnlockType.finishOrder)
                {
                    EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
                    MergeGuideLogic.Instance.CheckMergeGuide();
                }
            });
        }
    }
}