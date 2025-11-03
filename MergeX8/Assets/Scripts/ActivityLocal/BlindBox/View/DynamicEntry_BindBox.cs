using System;
using DragonU3DSDK.Storage;
using Dynamic;
using UnityEngine;

namespace ActivityLocal.BlindBox.View
{
    public class DynamicEntry_Home_BindBox : DynamicEntryBase
    {
        protected override string _entryPath => BlindBoxModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_BlindBox);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return BlindBoxModel.Instance.ShowAuxItem();
        }
    }
    [CancelAutoRegister]
    public class DynamicEntry_Home_BindBoxTheme : DynamicEntryBase
    {
        public DynamicEntry_Home_BindBoxTheme(){}
        public DynamicEntry_Home_BindBoxTheme(object param):base(param){}
        protected override string _entryPath =>  _parma == null ? "" : BlindBoxModel.Instance.GetAuxItemAssetPath((int)_parma);
        protected override Type _dynamicType => typeof(Aux_BlindBoxTheme);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return _parma != null && BlindBoxModel.Instance.GetStorage((int)_parma).IsResReady();
        }
        protected override void InvokerCreateEntry()
        {
            ((Aux_BlindBoxTheme)(MonoBehaviour)).SetTheme((int)_parma);
        }
    }
    
    public class DynamicEntry_Game_BindBox : DynamicEntryBase
    {
        protected override string _entryPath => BlindBoxModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeBlindBox);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return BlindBoxModel.Instance.ShowTaskEntrance();
        }
    }
}