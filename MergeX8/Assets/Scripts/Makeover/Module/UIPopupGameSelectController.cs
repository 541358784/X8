using DragonPlus;
using Gameplay;
using Makeover;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupGameSelectController : UIWindowController
{
    private Button _closeBtn;
    private Button _skipBtn;
    private Button _playBtn;
    private Transform _rewardItem;
    
    private TableMoLevel _curLevelConfig;
    
    
    public override void PrivateAwake()
    {
        _closeBtn = transform.Find("Root/CloseButton").GetComponent<Button>();
        _skipBtn = transform.Find("Root/Content/ButtonGroup/BtnSkip").GetComponent<Button>();
        _playBtn = transform.Find("Root/Content/ButtonGroup/BtnPlay").GetComponent<Button>();
        _rewardItem = transform.Find("Root/Content/Number/Reward1");
        _rewardItem.gameObject.SetActive(false);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        _curLevelConfig = (TableMoLevel)objs[0];
        for (int i = 0; i < _curLevelConfig.rewardID.Length; i++)
        {
           var tran=  Instantiate(_rewardItem, _rewardItem.parent);
           if (UserData.Instance.IsResource(_curLevelConfig.rewardID[i]))
           {
               tran.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(_curLevelConfig.rewardID[i],UserData.ResourceSubType.Big);
           }
           else
           {
               var itemConfig = GameConfigManager.Instance.GetItemConfig(_curLevelConfig.rewardID[i]);
               tran.Find("Icon").GetComponent<Image>().sprite= MergeConfigManager.Instance.mergeIcon.GetSprite(itemConfig.image);
           }
           tran.Find("Text_1").GetComponent<LocalizeTextMeshProUGUI>().SetText(_curLevelConfig.rewardCnt[i].ToString());
        }
        
        _closeBtn.onClick.AddListener(OnBtnClose);
        _skipBtn.onClick.AddListener(OnBtnClose);
        _playBtn.onClick.AddListener(OnBtnPlay);
    }

    private void OnBtnPlay()
    {
        AnimCloseWindow(() =>
        {
            UIManager.Instance.OpenUI(UINameConst.UIGameMain);
        });
    }

    private void OnBtnClose()
    {
        AnimCloseWindow();
    }
}