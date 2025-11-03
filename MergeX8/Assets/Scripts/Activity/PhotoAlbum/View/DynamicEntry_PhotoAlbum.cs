using System;
using Dynamic;
using UnityEngine;

namespace Activity.PhotoAlbum.View
{
    public class DynamicEntry_Home_PhotoAlbum : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/PhotoAlbum/Aux_PhotoAlbum";
        protected override Type _dynamicType => typeof(Aux_PhotoAlbum);
        protected override Transform _parent=>UIHomeMainController.GetAuxController(UIHomeMainController.AuxUIType.Left).GetTransform();
        protected override bool CanCreateEntry()
        {
            return PhotoAlbumModel.Instance.ShowAuxItem();
        }
    }
    
    
    public class DynamicEntry_Game_PhotoAlbum : DynamicEntryBase
    {
        protected override string _entryPath => "Prefabs/Activity/PhotoAlbum/TaskList_PhotoAlbum";
        protected override Type _dynamicType => typeof(MergePhotoAlbum);
        protected override Transform _parent => MergeTaskTipsController.Instance.DynamicParent;

        protected override bool CanCreateEntry()
        {
            return PhotoAlbumModel.Instance.ShowTaskEntrance();
        }
    }
}