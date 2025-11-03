using System;
using Dynamic;
using UnityEngine;

namespace ThemeDecorationLeaderBoard
{
    public class DynamicEntry_Home_ThemeDecoration : DynamicEntryBase
    {
        protected override string _entryPath => ThemeDecorationModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_ThemeDecoration);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return ThemeDecorationModel.Instance.ShowEntrance();
        }
    }
    
    public class DynamicEntry_Home_ThemeDecorationLeaderBoard : DynamicEntryBase
    {
        protected override string _entryPath => ThemeDecorationLeaderBoardModel.Instance.GetAuxItemAssetPath();
        protected override Type _dynamicType => typeof(Aux_ThemeDecorationLeaderBoard);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return ThemeDecorationLeaderBoardModel.Instance.ShowEntrance();
        }
    }
    
    
    public class DynamicEntry_Game_ThemeDecorationLeaderBoard : DynamicEntryBase
    {
        protected override string _entryPath => ThemeDecorationLeaderBoardModel.Instance.GetMergeItemAssetPath();
        protected override Type _dynamicType => typeof(MergeThemeDecorationLeaderBoard);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return ThemeDecorationLeaderBoardModel.Instance.ShowMergeEntrance();
        }
    }
    
    public class DynamicEntry_Game_ThemeDecoration : DynamicEntryBase
    {
        protected override string _entryPath => ThemeDecorationModel.Instance.GetMergeItemAssetPath();
        protected override Type _dynamicType => typeof(MergeThemeDecoration);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return ThemeDecorationModel.Instance.ShowMergeEntrance();
        }
    }
    
}