using System;
using DragonU3DSDK.Storage;
using Dynamic;
using UnityEngine;

namespace ActivityLocal.LevelUpPackage.UI
{
    public class DynamicEntry_Home_LevelUp : DynamicEntryBase
    {
        public DynamicEntry_Home_LevelUp(){}
        public DynamicEntry_Home_LevelUp(object param) : base(param){}
        
        protected override string _entryPath => LevelUpPackageModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_LevelUpPackage);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();
        protected override bool CanCreateEntry()
        {
            if (_parma == null)
                return false;

            return ((StorageLevelUpPackageSinglePackage)_parma).IsActive();
        }

        protected override void InvokerCreateEntry()
        {
            ((Aux_LevelUpPackage)(MonoBehaviour)).BindPackage((StorageLevelUpPackageSinglePackage)_parma);
        }
    }
}