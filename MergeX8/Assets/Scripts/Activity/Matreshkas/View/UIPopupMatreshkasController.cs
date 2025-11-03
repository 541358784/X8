using System.Collections;
using System.Collections.Generic;
using Activity.Matreshkas.Model;
using DragonPlus;
using DragonPlus.Config.Matreshkas;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

namespace Activity.Matreshkas.View
{
    public class UIPopupMatreshkasController : UIWindowController
    {
        private bool _isCanClose = true;
        private bool _isFinish = false;
        
        
        private Image _rewardIcon;
        private LocalizeTextMeshProUGUI _rewardText;
        
        public override void PrivateAwake()
        {
            var closeButton = transform.Find("Root/ButtonClose").GetComponent<Button>();
            closeButton.onClick.AddListener(CloseUI);
            
            var okButton = transform.Find("Root/Button").GetComponent<Button>();
            okButton.onClick.AddListener(CloseUI);
                
            _rewardText = transform.Find("Root/Reward/NumText").GetComponent<LocalizeTextMeshProUGUI>();
            _rewardIcon = transform.Find("Root/Reward/Icon").GetComponent<Image>();
        }

        protected override void OnOpenWindow(params object[] objs)
        {
            var config = MatreshkasConfigManager.Instance.MatreshkasSettingList[0];
            if (config != null)
            {
                _rewardIcon.sprite = UserData.GetResourceIcon(config.RewardIds[0]);
                _rewardText.SetText("x"+config.RewardNums[0]);
            }
            
            if (objs == null || objs.Length == 0)
                return;

            _isFinish = (bool)objs[0];
            
            if(!_isFinish)
                return;
            _isCanClose = false;
            StartCoroutine(WaitOpenReward());
        }
        
        private void CloseUI()
        {
            if(!_isCanClose)
                return;
            
            AnimCloseWindow(() =>
            {
                if(!_isFinish)
                    MergeGuideLogic.Instance.CheckMergeGuide();
                
                EventDispatcher.Instance.DispatchEventImmediately(MergeEvent.MERGE_REWARD_REFRESH);
            });
        }

        private IEnumerator WaitOpenReward()
        {
            yield return new WaitForSeconds(1);

            OpenReward();

        }
        private void OpenReward()
        {
            _isCanClose = true;
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventMatreshkasReward);

            var reasonArgs =new GameBIManager.ItemChangeReasonArgs(BiEventCooking.Types.ItemChangeReason.MatreshkasGet);
            
            List<ResData> resDatas = new List<ResData>();
            var config = MatreshkasConfigManager.Instance.MatreshkasSettingList[0];
            for(int i = 0;i < config.RewardIds.Count; i++)
            {
                ResData resData = new ResData(config.RewardIds[i], config.RewardNums[i]);
                resDatas.Add(resData);
                
                if (!UserData.Instance.IsResource(resData.id))
                {
                    var itemConfig = GameConfigManager.Instance.GetItemConfig(resData.id);
                    if (itemConfig != null)
                    {
                        GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
                        {
                            MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonMatreshkasGet,
                            itemAId = itemConfig.id,
                            isChange = true,
                        });
                    }
                }
            }
            
            CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController,true, reasonArgs, () =>
            {
                CloseUI();
            });
        }
    }
}