using System;
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.MiniGame
{
    public class MiniGameCell : MonoBehaviour
    {
        private MiniGameInfo _info;
        private Action<MiniGameInfo> _action;
        
        private Button _playButton;
        private Image _icon;
        private LocalizeTextMeshProUGUI _levelText;
        private LocalizeTextMeshProUGUI _lockText;
        private LocalizeTextMeshProUGUI _lockTipText;
        private GameObject _passObj;
        private GameObject _gameObject;
        private Button _rvButton;

        private GameObject _normal;
        private GameObject _lock;
        public GameObject _comingsoon;
        
        private void Awake()
        {
            _normal = transform.Find("Normal").gameObject;
            _lock = transform.Find("Lock").gameObject;
            _comingsoon = transform.Find("comingsoon").gameObject;
            
            _playButton = gameObject.transform.Find("Normal/PlayButton").GetComponent<Button>();
            _playButton.onClick.AddListener(() =>
            {
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.AsmrPlay, _info._id.ToString());
                OnClickPlay();
            });
            
            _levelText = gameObject.transform.Find("Normal/LevelText").GetComponent<LocalizeTextMeshProUGUI>();
            _lockText = gameObject.transform.Find("Lock/LevelText").GetComponent<LocalizeTextMeshProUGUI>();
            _lockTipText = gameObject.transform.Find("Lock/Text").GetComponent<LocalizeTextMeshProUGUI>();
            _rvButton = gameObject.transform.Find("Normal/RvButton").GetComponent<Button>();

            _icon = gameObject.transform.Find("Normal/LevelIcon").GetComponent<Image>();
            _passObj = gameObject.transform.Find("Normal/Pass").gameObject;
        }

        public void Init(MiniGameInfo info, Action<MiniGameInfo> action)
        {
            _info = info;
            _action = action;
            
            _normal.gameObject.SetActive(_info._isUnLock);
            _passObj.gameObject.SetActive(info._isFinish);
            _playButton.gameObject.SetActive(!info.needRv);
            _lock.gameObject.SetActive(!_info._isUnLock);
            _comingsoon.gameObject.SetActive(_info._isComingSoon);

            _icon.sprite = info._sprite;
            _levelText.SetText(info._name);
            _lockText.SetText(info._name);
            _lockTipText.SetTerm("ui_asmr_unlock_day");
            _lockTipText.SetTermFormats(info.unlockNum.ToString());
            if (_info.needRv)
            {
                UIAdRewardButton.Create(ADConstDefine.RV_REPLAY_MINIGAME, UIAdRewardButton.ButtonStyle.Disable, _rvButton.gameObject,
                    (s) =>
                    {
                        if (s)
                        {
                            var uipopView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupGameTabulation);
                            if (uipopView != null)
                            {
                                uipopView.CloseWindowWithinUIMgr(true);
                            }

                            OnClickPlay();
                        }
                    }, false, null, () =>
                    {
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigame2NdtryRv);
                        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.AsmrPlay, _info._id.ToString());
                    });   
            }
            else
            {
                _rvButton.gameObject.SetActive(false);
            }
        }

        private void OnClickPlay()
        {
            var uipopView = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIPopupMiniGame);
            if (uipopView != null)
            {
                uipopView.AnimCloseWindow();
            }
            
            UILoadingTransitionController.Show(() =>
            {
                _action?.Invoke(_info);
            });
        }
    }
}