using System;
using Dynamic;
using UnityEngine;

namespace ActivityLocal.KeepPet.UI
{
    public class DynamicEntry_Game_KeepPet: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/KeepPet/TaskList_KeepPet";
        protected override Type _dynamicType => typeof(MergeKeepPet);
        protected override Transform _parent=>MergeTaskTipsController.Instance.DynamicParent;
        protected override bool CanCreateEntry()
        {
            return KeepPetModel.Instance.ShowEntrance();
        }
    }
}