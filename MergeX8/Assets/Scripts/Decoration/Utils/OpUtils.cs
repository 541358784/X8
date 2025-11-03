using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DaVikingCode.RectanglePacking;
using DragonU3DSDK;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
namespace Decoration
{
    public class OpUtils
    {
        private const string DEFAULT_SHADER_NAME = "Sprites/Default";
        private static Material _spriteDefaultMat;

        public static void ReplaceDefaultShader(SpriteRenderer spRender)
        {
            if (_spriteDefaultMat == null)
            {
                _spriteDefaultMat = new Material(Shader.Find(DEFAULT_SHADER_NAME));
#if UNITY_EDITOR
                _spriteDefaultMat.name = "ReplacedMaterial";
#endif
            }
        
            if (spRender.sharedMaterial.shader.name == DEFAULT_SHADER_NAME)
            {
                spRender.sharedMaterial = _spriteDefaultMat;
            }
        }

        
        public static void UnloadSpriteAtlas(string atlasName)
        {
            if (string.IsNullOrEmpty(atlasName)) return;
            try
            {
                ResourcesManager.Instance.UnloadSpriteAtlasImmediateVariant(atlasName);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(atlasName);
                DebugUtil.LogError(e.ToString());
            }
        }

        public static void UnloadObjFromBundleManager(string path, bool free = true)
        {
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                ResourcesManager.Instance.ReleaseRes(path.ToLower(), free);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(path);
                DebugUtil.LogError(e.ToString());
            }
        }

        public static void ReleaseRes(string path, object obj)
        {
            try
            {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

                ResourcesManager.Instance.ReleaseRes(path.ToLower(), true);
                if (obj is GameObject)
                {
                    GameObject.Destroy(obj as GameObject);
                }
                else if (obj is Component)
                {
                    GameObject.Destroy(obj as Component);
                }
                else if (obj is AssetBundle)
                {
                    GameObject.Destroy(obj as AssetBundle);
                }
                else
                {
                    Resources.UnloadAsset(obj as Object);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError(path);
                DebugUtil.LogError(e.ToString());
            }
        }
    }
}