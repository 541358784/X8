using System;
using Dynamic;
using UnityEngine;

namespace ActivityLocal.CardCollection.UI
{
    public class DynamicEntry_Home_CardCollection : DynamicEntryBase
    {
        protected override string _entryPath => CardCollectionModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_CardCollection);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return CardCollectionModel.Instance.ShowAuxItem();
        }
    }
    
    [CancelAutoRegister]
    public class DynamicEntry_Home_CardCollectionTheme : DynamicEntryBase
    {
        public DynamicEntry_Home_CardCollectionTheme(){}
        public DynamicEntry_Home_CardCollectionTheme(object param):base(param){}
        protected override string _entryPath =>  _parma == null ? "" : ((CardCollectionCardThemeState)_parma).GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_CardCollectionTheme);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return _parma != null && ((CardCollectionCardThemeState)_parma).IsResReady();
        }
        protected override void InvokerCreateEntry()
        {
            ((Aux_CardCollectionTheme)(MonoBehaviour)).SetTheme((CardCollectionCardThemeState)_parma);
        }
    }
    
    public class DynamicEntry_Game_CardCollection : DynamicEntryBase
    {
        public DynamicEntry_Game_CardCollection(){}
        public DynamicEntry_Game_CardCollection(object param):base(param){}
        protected override string _entryPath =>  _parma == null ? "" : ((CardCollectionCardThemeState)_parma).GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeCardCollection);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return _parma != null && ((CardCollectionCardThemeState)_parma).IsResReady();
        }
        protected override void InvokerCreateEntry()
        {
            ((MergeCardCollection)(MonoBehaviour)).SetTheme((CardCollectionCardThemeState)_parma);
        }
    }
    
    public class DynamicEntry_Game_CardPackage : DynamicEntryBase
    {
        protected override string _entryPath => CardCollectionModel.Instance.GetCardPackageTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeCardPackage);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return true;
        }
    }
}