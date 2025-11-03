using System;
using Activity.Matreshkas.Model;
using Dynamic;
using UnityEngine;

namespace Activity.Matreshkas.View
{
    public class DynamicEntry_Game_Matreshkas: DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/Matreshkas/TaskList_Matreshkas";
        protected override Type _dynamicType => typeof(MergeMatreshkasEntry);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return MatreshkasModel.Instance.IsOpened();
        }
    }
}