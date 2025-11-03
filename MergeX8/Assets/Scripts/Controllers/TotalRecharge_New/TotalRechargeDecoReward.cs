using System;
using System.Collections.Generic;
using ActivityLocal.DecoBuildReward;
using Decoration;
using Decoration.Bubble;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace TotalRecharge_New
{
    public class TotalRechargeDecoReward :  MonoBehaviour
    {
        private const int BuildId = 10001;
        public const string DecoBuildId = "10001";
        
        private List<GameObject> _items = new List<GameObject>();
        
        private void Awake()
        {
            for (int i = 1; i <= 2; i++)
            {
                _items.Add(transform.Find("Root/RewardIcon/"+i).gameObject);
            }
            
            transform.Find("Root/Button").GetComponent<Button>().onClick.AddListener((() =>
            {
                var resDatas = DecoBuildRewardManager.Instance.GetReward(DecoBuildId);
                if(resDatas == null)
                    return;
        
                var reasonArgs = new GameBIManager.ItemChangeReasonArgs(BiEventAdventureIslandMerge.Types.ItemChangeReason.TotalRechargeRewardGet);
                CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.GetCurrencyUseController(),
                    true, reasonArgs, animEndCall:
                    () =>
                    {
                        var decoItem = DecoManager.Instance.FindItem(BuildId);
                        if (decoItem != null)
                        {
                            NodeBubbleManager.Instance.UnLoadBubble(decoItem.Node);
                            NodeBubbleManager.Instance.OnLoadBubble(decoItem.Node); 
                        }
                    });
            }));
        }

        private void OnEnable()
        {
            _items.ForEach(a=>a.gameObject.SetActive(false));
            
            var resData = DecoBuildRewardManager.Instance.GetRewardInfo(DecoBuildId);
            if(resData == null)
                return;

            _items[0].gameObject.SetActive(resData.Count == 1);
            _items[1].gameObject.SetActive(resData.Count > 1);
        }
    }
}