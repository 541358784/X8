using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.ClimbTower;
using UnityEngine;
using UnityEngine.UI;

namespace ActivityLocal.ClimbTower.Model
{
    public class ClimbTowerShopItem : MonoBehaviour
    {
        private Button _clickButton;
        private LocalizeTextMeshProUGUI _text;
        private GameObject _gameTag;
        
        public void Awake()
        {
            _text = transform.Find("NumberText").GetComponent<LocalizeTextMeshProUGUI>();
            _gameTag = transform.Find("TagGroup").gameObject;
            
            _clickButton = transform.Find("Button").GetComponent<Button>();
            _clickButton.onClick.AddListener(() =>
            {
                if (ClimbTowerModel.Instance.IsCanPay())
                {
                    UIManager.Instance.OpenWindow(UINameConst.UIPopupClimbTowerPay);
                    return;
                }
                
                if (!ClimbTowerModel.Instance.ClimbTower.IsPayLevel && !ClimbTowerModel.Instance.ClimbTower.IsEnter)
                {
                    ClimbTowerModel.Instance.ClimbTower.IsEnter = true;
                    ClimbTowerModel.Instance.ClimbTower.FreeTimes--;
                }
                
                GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GetFreeReward);
                
                if(ClimbTowerModel.Instance.ClimbTower.IsPayLevel)
                    UIManager.Instance.OpenWindow(UINameConst.UIClimbTowerMainPay);
                else
                {
                    UIManager.Instance.OpenWindow(UINameConst.UIClimbTowerMain);
                }
            });
            
            InvokeRepeating("InvokeUpdate", 0, 1);
            
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(_clickButton.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.GetFreeReward, _clickButton.transform as RectTransform, topLayer:topLayer);
            
            bool isCanEnter = ClimbTowerModel.Instance.IsCanEnter();
            if(isCanEnter)
                GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GetFreeReward, "");
        }

        private void InvokeUpdate()
        {
            bool isCanEnter = ClimbTowerModel.Instance.IsCanEnter();
            _clickButton.interactable = isCanEnter;

            _gameTag.SetActive(isCanEnter);

            if (!ClimbTowerModel.Instance.IsOpen())
            {
                _text.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_unlock_tips", UnlockManager.GetUnlockParam(UnlockManager.MergeUnlockType.ClimbTower).ToString()));
                return;
            }
            
            if (isCanEnter)
            {
                _text.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_info_text37", "1"));
            }
            else
            {
                _text.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormat("UI_info_text37", "0"));
            }
        }
    }
}