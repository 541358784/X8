using System;
using Dynamic;
using UnityEngine;

namespace ActivityLocal.BuyDiamondTicket.View
{
    public class DynamicEntry_Home_BuyDiamondTicket : DynamicEntryBase
    {
        protected override string _entryPath => BuyDiamondTicketModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_BuyDiamondTicket);
        protected override Transform _parent => UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Right).GetTransform();

        protected override bool CanCreateEntry()
        {
            return BuyDiamondTicketModel.Instance.ShowAuxItem();
        }
    }
    
    public class DynamicEntry_Game_BuyDiamondTicket : DynamicEntryBase
    {
        protected override string _entryPath => BuyDiamondTicketModel.Instance.GetTaskItemAssetPath();
        protected override Type _dynamicType => typeof(MergeBuyDiamondTicket);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return BuyDiamondTicketModel.Instance.ShowTaskEntrance();
        }
    }
}