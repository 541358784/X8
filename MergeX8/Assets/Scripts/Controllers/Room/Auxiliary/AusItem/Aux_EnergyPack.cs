using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.ConfigHub.Ad;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;
using Pack = BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities.Pack;
using BiEventCooking=DragonU3DSDK.Network.API.Protocol.BiEventAdventureIslandMerge;

public class Aux_EnergyPack : Aux_ItemBase
{
    private LocalizeTextMeshProUGUI _timeText;
    
    protected override void Awake()
    {
        base.Awake();

        _timeText= transform.Find("FreeTag/Text").GetComponent<LocalizeTextMeshProUGUI>();
        InvokeRepeating("UpdateUI",0,1);

   
    }
    public override void UpdateUI()
    {
        _timeText.SetText(EnergyPackageModel.Instance.GetGetPackageLeftTimeString());
        gameObject.SetActive(EnergyPackageModel.Instance.GetPackageLeftTime()>0 && EnergyPackageModel.Instance.IsCanShow() );
    }
    
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        UIManager.Instance.OpenUI(UINameConst.UIPopupEnergyBuy);
    }

    private void OnDestroy()
    {
    }

}
