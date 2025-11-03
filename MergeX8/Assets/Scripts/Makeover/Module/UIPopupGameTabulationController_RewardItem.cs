using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPopupGameTabulationController
{
    public class RewardItem
    {
        public class RewardItemBase
        {
            public GameObject _gameObject;
            public TableMiniGameGroup _groupConfig;
            public Transform DefaultItemObj;
            public List<CommonRewardItem> RewardList = new List<CommonRewardItem>();
            public Button PlayBtn;
            // private LocalizeTextMeshProUGUI LevelText;
            public virtual void Init(GameObject gameObject, TableMiniGameGroup groupConfig,int startIndex,int endIndex)
            {
                _gameObject = gameObject;
                _groupConfig = groupConfig;
                DefaultItemObj = _gameObject.transform.Find("RewardGroup/Reward1");
                DefaultItemObj.gameObject.SetActive(false);
                var rewards = CommonUtils.FormatReward(_groupConfig.rewardId, _groupConfig.rewardNum);
                for (var i = 0; i < rewards.Count; i++)
                {
                    var reward = rewards[i];
                    var item = Instantiate(DefaultItemObj.gameObject, DefaultItemObj.parent).AddComponent<CommonRewardItem>();
                    item.gameObject.SetActive(true);
                    item.Init(reward);
                    RewardList.Add(item);
                }
                PlayBtn = _gameObject.transform.Find("PlayButton").GetComponent<Button>();
                PlayBtn.onClick.AddListener(OnClickPlayBtn);
                // LevelText = _gameObject.transform.Find("LevelText").GetComponent<LocalizeTextMeshProUGUI>();
                // LevelText.SetText(LocalizationManager.Instance.GetLocalizedStringWithFormats(
                //     "key",(startIndex+1).ToString(),(endIndex+1).ToString()));
            }

            public virtual void OnClickPlayBtn() { }
        }

        public class RewardItemNormal:RewardItemBase
        {
            private LocalizeTextMeshProUGUI Text;
            private Transform Pass;
            public override void Init(GameObject gameObject, TableMiniGameGroup groupConfig,int startIndex,int endIndex)
            {
                base.Init(gameObject, groupConfig,startIndex,endIndex);
                var canCollect = CanCollect(_groupConfig);
                var isCollected = IsCollected(_groupConfig);
                Text = _gameObject.transform.Find("Text").GetComponent<LocalizeTextMeshProUGUI>();
                Text.gameObject.SetActive(!isCollected);
                Pass = _gameObject.transform.Find("Pass");
                Pass.gameObject.SetActive(isCollected);
                PlayBtn.gameObject.SetActive(canCollect);
            }

            public override void OnClickPlayBtn()
            {
                base.OnClickPlayBtn();
                if (CanCollect(_groupConfig))
                {
                    CollectGroup(_groupConfig);
                    Pass.gameObject.SetActive(true);
                    PlayBtn.gameObject.SetActive(false);
                    Text.gameObject.SetActive(false);
                }
            }
        }

        public class RewardItemLock : RewardItemBase
        {
        }
        private GameObject _gameObject;
        private TableMiniGameGroup _groupConfig;
        private RewardItemLock LockNode =new RewardItemLock();
        private RewardItemNormal NormalNode = new RewardItemNormal();
        public void Init(GameObject gameObject, TableMiniGameGroup groupConfig,int startIndex,int endIndex)
        {
            _gameObject = gameObject;
            _groupConfig = groupConfig;
            LockNode.Init(_gameObject.transform.Find("Lock").gameObject,_groupConfig,startIndex,endIndex);
            NormalNode.Init(_gameObject.transform.Find("Normal").gameObject,_groupConfig,startIndex,endIndex);
            LockNode._gameObject.SetActive(!IsFinishAllLevel(_groupConfig));
            NormalNode._gameObject.SetActive(IsFinishAllLevel(_groupConfig));
        }
    }
}