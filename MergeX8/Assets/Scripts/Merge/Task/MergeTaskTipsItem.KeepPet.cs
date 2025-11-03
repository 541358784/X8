using Activity.TimeOrder;
using DragonU3DSDK.Storage;
using Merge.Order;
using UnityEngine;


public partial class MergeTaskTipsItem
{
    private GameObject _keepPetOrderBg;

    private void AwakeKeepPetOrder()
    {
        _keepPetOrderBg = transform.Find("BGKeepPet").gameObject;
    }

    private void RefreshRepeatingKeepPetOrder()
    {
        if(storageTaskItem.Type != (int)MainOrderType.KeepPet)
            return;
    }
    
    private void InitKeepPetOrder(StorageTaskItem storageItem)
    {
        _keepPetOrderBg.gameObject.SetActive(false);
        
        if(storageItem == null || storageItem.Type != (int)MainOrderType.KeepPet)
            return;
        
        _keepPetOrderBg.gameObject.SetActive(true);
    }
}