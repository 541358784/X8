using System;
using Dynamic;
using UnityEngine;

namespace Activity.JungleAdventure.Controller
{
    public class DynamicEntry_Home_JungleAdventure: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/JungleAdventure/Aux_JungleAdventure";
        protected override Type _dynamicType => typeof(Aux_JungleAdventure);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return JungleAdventureModel.Instance.IsOpened();
        }
    }
    
    public class DynamicEntry_Game_JungleAdventure: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/JungleAdventure/TaskList_JungleAdventure";
        protected override Type _dynamicType => typeof(MergeJungleAdventure);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return JungleAdventureModel.Instance.IsOpened() && JungleAdventureModel.Instance.IsPreheatEnd();
        }
    }
}