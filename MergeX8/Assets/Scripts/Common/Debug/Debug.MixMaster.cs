using System.Collections.Generic;
using System.ComponentModel;
using DragonPlus;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Gameplay;
using UnityEngine;

public partial class SROptions
{
    private const string MixMaster = "调制大师";
    [Category(MixMaster)]
    [DisplayName("重制")]
    public void ResetMixMaster()
    {
        HideDebugPanel();
        HideDebugPanel();
        var storage = StorageManager.Instance.GetStorage<StorageHome>().MixMaster;
        StorageManager.Instance.GetStorage<StorageHome>().MixMaster.Clear();
        var guideIdList = new List<int>() {4311,4312};
        CleanGuideList(guideIdList);
        if (MixMasterModel.Instance.IsInitFromServer())
            return;
        MixMasterModel.Instance.InitStorage();
    }

    private int _mixMasterMaterialId = 960;
    [Category(MixMaster)]
    [DisplayName("材料id")]
    public int MixMasterMaterialId
    {
        get
        {
            return _mixMasterMaterialId;
        }
        set
        {
            if (!MixMasterModel.Instance.IsInitFromServer())
                return;
            var config = MixMasterModel.Instance.MaterialConfig.Find(a => a.Id == value);
            if (config == null)
            {
                Debug.LogError("材料Id错误");
                return;
            }
            _mixMasterMaterialId = value;
        }
    }

    private int _mixMasterMaterialCount = 1;
    [Category(MixMaster)]
    [DisplayName("材料数量")]
    public int MixMasterMaterialCount
    {
        get
        {
            return _mixMasterMaterialCount;
        }
        set
        {
            _mixMasterMaterialCount = value;
        }
    }

    [Category(MixMaster)]
    [DisplayName("增加材料")]
    public void MixMasterAddMaterial()
    {
        if (!MixMasterModel.Instance.IsInitFromServer())
            return;
        if (MixMasterModel.Instance.Storage.GetPreheatTime() > 0)
            return;
        UserData.Instance.AddRes(_mixMasterMaterialId,_mixMasterMaterialCount,
            new GameBIManager.ItemChangeReasonArgs(reason:BiEventAdventureIslandMerge.Types.ItemChangeReason.Debug));
    }
}