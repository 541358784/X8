using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using DaVikingCode.RectanglePacking;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Quality;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Serialization;

using Framework;
namespace Decoration
{


    public class RuntimeAtlasManager : GlobalSystem<RuntimeAtlasManager>
    {
        private const int ONE_SINGLE_ATLAS_ID = 9;

        public Dictionary<int, RuntimeAtlas> _bilinearFilterAtlasDic = new Dictionary<int, RuntimeAtlas>();
        

        public void ReleaseRuntimeTexture()
        {
            foreach (var atlas in _bilinearFilterAtlasDic.Values)
            {
                atlas?.ReleaseRuntimeTexture();
            }
        }

        public void ClearTemp(bool useOneAtlas, int worldId)
        {
            if (useOneAtlas) worldId = ONE_SINGLE_ATLAS_ID;

            if (_bilinearFilterAtlasDic.TryGetValue(worldId, out var bilinearAtlas))
            {
                bilinearAtlas.ClearTemp();
            }
        }

        public void CloseTexturesReadWrite(GameObject obj)
        {
            return;

            if (SystemInfo.deviceModel.ToLower().Contains("xiaomi")) return;

            //关闭新Item的读写，释放内存
            var renders = obj.GetComponentsInChildren<SpriteRenderer>(true);
            if (renders != null && renders.Length > 0)
            {
                for (int i = 0; i < renders.Length; i++)
                {
                    if (renders[i].sprite && renders[i].sprite.texture && renders[i].sprite.texture.isReadable) renders[i].sprite.texture.Apply(false, true);
                }
            }
        }

        public void Replace(bool useOneAtlas, int worldId, GameObject obj, bool needPhysicsShape)
        {
            if (!obj) return;

            if (useOneAtlas) worldId = ONE_SINGLE_ATLAS_ID;

            if (!_bilinearFilterAtlasDic.TryGetValue(worldId, out var bilinearAtlas))
            {
                bilinearAtlas = new RuntimeAtlas(68054533, 8192, int.MaxValue);
                _bilinearFilterAtlasDic.Add(worldId, bilinearAtlas);
            }


            var spRenders = new List<SpriteRenderer>();
            obj.GetComponentsInChildren<SpriteRenderer>(true, spRenders);
            if (spRenders == null || spRenders.Count <= 0) return;

            for (var index = 0; index < spRenders.Count; index++)
            {
                var render = spRenders[index];
                if (!render) continue;
                if (!render.sprite) continue;
                if (!render.sprite.texture) continue;
                if (render.sprite.name == "shadow") continue;
                
                var atlas = bilinearAtlas;
                
                if (atlas.AddTest(render))
                {
                    atlas.AddToAtlas(render, needPhysicsShape);
                }
            }
        }

        public void GenerateLast(bool useOneAtlas, int worldId)
        {
            if (useOneAtlas) worldId = ONE_SINGLE_ATLAS_ID;

            if (_bilinearFilterAtlasDic.TryGetValue(worldId, out var bilinearAtlas))
            {
                bilinearAtlas.GenerateLast();
            }
            DragonU3DSDK.DebugUtil.Log($"Arthur---->GenerateLast");
        }
    }
}