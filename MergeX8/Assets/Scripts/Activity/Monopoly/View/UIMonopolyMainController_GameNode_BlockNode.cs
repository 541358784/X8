using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public partial class UIMonopolyMainController
{
    public class BlockNode:MonoBehaviour
    {
        private int MultiValue = 1;
        public void SetScoreMultiValue(int multiValue)
        {
            if (BlockType != MonopolyBlockType.Score && BlockType != MonopolyBlockType.Reward)
                return;
            MultiValue = multiValue;
            UpdateViewState();
        }

        public void UpdateViewState()
        {
            if (BlockType != MonopolyBlockType.Score && BlockType != MonopolyBlockType.Reward)
                return;
            if (MultiValue > 1)
            {
                if (BlockType == MonopolyBlockType.Score)
                {
                    // ItemText.SetText(BaseScore + "X" + MultiValue);
                    ItemText.SetText((BaseScore*MultiValue).ToString());
                }
                else if (BlockType == MonopolyBlockType.Reward)
                {
                    // ItemText.SetText(BlockConfig.RewardNum[0]+ "X" + MultiValue);
                    ItemText.SetText((BlockConfig.RewardNum[0]*MultiValue).ToString());
                }
            }
            else
            {
                if (BlockType == MonopolyBlockType.Score)
                {
                    ItemText.SetText(BaseScore.ToString());
                }
                else if (BlockType == MonopolyBlockType.Reward)
                {
                    ItemText.SetText(BlockConfig.RewardNum[0].ToString());
                }
            }
            for (var i=0;i<BGBuyList.Count;i++)
            {
                BGBuyList[i].gameObject.SetActive(i == BuyTimes);
            }
        }

        public bool SetGroupFull(bool groupFull)
        {
            if (BlockType != MonopolyBlockType.Score)
                return false;
            if (GroupFull == groupFull)
                return false;
            GroupFull = groupFull;
            UpdateViewState();
            return true;
        }
        
        public void SetBuyTimes(int buyTimes)
        {
            if (BlockType != MonopolyBlockType.Score)
                return;
            if (BuyTimes == buyTimes)
                return;
            BuyTimes = buyTimes;
            UpgradeEffect.DOKill();
            UpgradeEffect.gameObject.SetActive(false);
            UpgradeEffect.gameObject.SetActive(true);
            DOVirtual.DelayedCall(2f, () => UpgradeEffect.gameObject.SetActive(false)).SetTarget(UpgradeEffect);
            UpdateViewState();
        }

        private int BuyTimes;
        private bool GroupFull;
        public MonopolyBlockConfig BlockConfig;
        private int BaseScore => BlockConfig.GetBlockScore(BuyTimes, GroupFull);
        MonopolyBlockType BlockType => (MonopolyBlockType) BlockConfig.BlockType;
        private Image ItemSprite;
        private LocalizeTextMeshProUGUI ItemText;

        private Button BuyBlockBtn;
        private List<Transform> BGBuyList = new List<Transform>();
        private Transform UpgradeEffect;
        public void Init(MonopolyBlockConfig config,int buyTimes,bool groupFull)
        {
            GroupFull = groupFull;
            BuyTimes = buyTimes;
            BlockConfig = config;
            if (BlockType != MonopolyBlockType.Score && BlockType != MonopolyBlockType.Reward)
                return;
            ItemSprite = transform.Find("RewardGroup/Icon").GetComponent<Image>();
            ItemText = transform.Find("RewardGroup/Text").GetComponent<LocalizeTextMeshProUGUI>();
            if (BlockType == MonopolyBlockType.Reward)
            {
                ItemSprite.sprite = UserData.GetResourceIcon(BlockConfig.RewardId[0],UserData.ResourceSubType.Big);
            }

            BuyBlockBtn = transform.GetComponent<Button>();
            BuyBlockBtn.onClick.RemoveAllListeners();
            BuyBlockBtn.onClick.AddListener(OnClickBuyBlockBtn);
            
            BGBuyList.Add(transform.Find("BGNormal"));
            BGBuyList.Add(transform.Find("BGBuy1"));
            BGBuyList.Add(transform.Find("BGBuy2"));
            BGBuyList.Add(transform.Find("BGBuy3"));
            UpgradeEffect = transform.Find("FX_Level");

            UpdateViewState();
        }

        public void OnClickBuyBlockBtn()
        {
            var mainUI = UIManager.Instance.GetOpenedUIByPath<UIMonopolyMainController>(UINameConst.UIMonopolyMain);
            if (mainUI)
            {
                mainUI.OnClickBuyBlockBtn(BlockConfig);
            }
        }
    }
}