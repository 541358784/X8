
using System.Collections.Generic;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupRecoveryController : UIWindowController
{
    private Transform _recoverRoot;
    private GameObject _recoverCell;
    
    private Transform _resRoot;
    private GameObject _resCell;

    private Button _closeButton;
    
    private Dictionary<int, int> _recoveryRes = new Dictionary<int, int>();
    private Dictionary<int, int> _recoveryItem = new Dictionary<int, int>();
    
    private List<RewardData> _rewardDatas = new List<RewardData>();
    public override void PrivateAwake()
    {
        _recoverRoot = transform.Find("Root/RecoveryGroup/Scroll View/Viewport/Content");
        _recoverCell = _recoverRoot.Find("Item").gameObject;
        _recoverCell.gameObject.SetActive(false);
        
        
        _resRoot = transform.Find("Root/RewardGroup/Content");
        _resCell = _resRoot.Find("Item").gameObject;
        _resCell.gameObject.SetActive(false);

        _closeButton = transform.Find("Root/Button").GetComponent<Button>();
        _closeButton.onClick.AddListener(RecoverRes);
    }

    protected override void OnOpenWindow(params object[] objs)
    {
        _recoveryItem = (Dictionary<int, int>)objs[0];
        _recoveryRes = (Dictionary<int, int>)objs[1];

        InitUI();
    }

    private void InitUI()
    {
        foreach (var kv in _recoveryItem)
        {
            var itemCell = Instantiate(_recoverCell, _recoverRoot);
            itemCell.gameObject.SetActive(true);
            
            itemCell.transform.Find("Icon").GetComponent<Image>().sprite = UserData.GetResourceIcon(kv.Key, UserData.ResourceSubType.Big);
            itemCell.transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>().SetText(kv.Value.ToString());
        }
        
        foreach (var kv in _recoveryRes)
        {
            UserData.Instance.AddRes(kv.Key, kv.Value, new GameBIManager.ItemChangeReasonArgs(){reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.Recycle}, false);

            RewardData rewardData = new RewardData();
            _rewardDatas.Add(rewardData);
            
            var itemCell = Instantiate(_resCell, _resRoot);
            itemCell.gameObject.SetActive(true);
            
            rewardData.gameObject = itemCell;
            rewardData.numText = itemCell.transform.Find("Num").GetComponent<LocalizeTextMeshProUGUI>();
            rewardData.image = itemCell.transform.Find("Icon").GetComponent<Image>();
            rewardData.UpdateReward(kv.Key, kv.Value);
        }
    }
    
    private void RecoverRes()
    {
        _closeButton.interactable = false;
        
        FlyGameObjectManager.Instance.FlyObject(_rewardDatas, CurrencyGroupManager.Instance.currencyController, () =>
        {
            CloseWindowWithinUIMgr(true);
        });
    }
}