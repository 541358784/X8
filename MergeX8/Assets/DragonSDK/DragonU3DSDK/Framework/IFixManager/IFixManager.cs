#if ENABLE_INJECTFIX
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;
using IFix.Core;

namespace DragonU3DSDK.Asset
{
    public static class IFixManager
    {
        public const string ASSET_PATH = "Patches";
        public const string PATCH_LIST = "patchlist";
        static Dictionary<string, Stream> streams = new Dictionary<string, Stream>();

        public static void Init()
        {
            Clear();

            TextAsset textAsset = LoadAsset(PATCH_LIST);
            if (textAsset == null)
            {
                return;
            }

            string[] patchList = textAsset.text.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in patchList)
            {
                Stream stream = new MemoryStream(LoadAsset(name).bytes);
                PatchManager.Load(stream);
                streams.Add(name, stream);
            }
        }

        static TextAsset LoadAsset(string name)
        {
            return ResourcesManager.Instance.LoadResource<TextAsset>(ASSET_PATH + "/" + name);
        }

        public static void Clear()
        {
            foreach (string key in streams.Keys)
            {
                PatchManager.Unload(Assembly.Load(key.Replace(".patch", "")));
                streams[key].Dispose();
            }
            streams.Clear();
        }
    }
}

#endif