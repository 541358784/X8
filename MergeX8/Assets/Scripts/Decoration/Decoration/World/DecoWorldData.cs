using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK.Storage;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using DragonU3DSDK.Asset;
using Framework;
using Decoration;
namespace Deco.World
{
    using StorageDecoration=DragonU3DSDK.Storage.Decoration.StorageDecoration;
    using StorageWorld=DragonU3DSDK.Storage.Decoration.StorageWorld;
    using StorageArea=DragonU3DSDK.Storage.Decoration.StorageArea;
    using StorageItem=DragonU3DSDK.Storage.Decoration.StorageItem;
    using StorageNode=DragonU3DSDK.Storage.Decoration.StorageNode;
    using StorageStage=DragonU3DSDK.Storage.Decoration.StorageStage;
    internal class DecoWorldData
    {
        private DecoWorld _world;

        internal TableWorlds _config = null;
        internal StorageWorld _storage = null;

        internal DecoWorldData(DecoWorld world, TableWorlds config, StorageWorld storage)
        {
            _config = config;
            _world = world;
            _storage = storage;

            if (config.id == 1 || config.id == 2)
            {
                _storage.State = (int)DecoWorld.Status.Unlock;
            }
        }
    }
    
}