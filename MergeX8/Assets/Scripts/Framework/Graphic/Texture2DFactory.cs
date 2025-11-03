using System.Collections.Generic;
using DragonU3DSDK;
using UnityEngine;

namespace Framework
{
    public class Texture2DFactory : GlobalSystem<Texture2DFactory>
    {
        private const int cachedTime = 5;
        private const int maxCached = 6;

        class TextureInfo
        {
            public float cacheTime;
            public Texture2D tex;
        }

        private Dictionary<string, Queue<TextureInfo>> _texturePool = new Dictionary<string, Queue<TextureInfo>>();
        private float _currentTime = 0;


        public Texture2D CreateTexture(string name, int width = -1, int height = -1)
        {
//            if (width > 0 && height > 0)
//            {
//                var key = _GenKey(width, height);
//                if (_texturePool.TryGetValue(key, out var textures))
//                {
//                    if (textures != null && textures.Count > 0)
//                    {
//                        var texInfo = textures.Dequeue();
//                        return texInfo.tex;
//                    }
//                }
//            }

            var tex = new Texture2D(2, 2);
            tex.name = name;
            return tex;
        }


        public void DestroyTexture(Texture2D texture2D)
        {
//            if (texture2D != null)
//            {
//                var key = _GenKey(texture2D.width, texture2D.height);
//                Queue<TextureInfo> textureInfos = null;
//                if (!_texturePool.TryGetValue(key, out textureInfos) || textureInfos == null)
//                {
//                    textureInfos = new Queue<TextureInfo>();
//                    _texturePool.Add(key, textureInfos);
//                }
//
//                if (textureInfos.Count < maxCached)
//                {
//                    textureInfos.Enqueue(new TextureInfo() {cacheTime = Time.time, tex = texture2D}); 
//                }
//            }
        }

        private string _GenKey(int width, int height)
        {
            return $"{width}_{height}";
        }

        public void Update(float deltaTime)
        {
            _currentTime += deltaTime;
            if (_currentTime >= 1f)
            {
                _currentTime = 0;
                _ClearOldCache();
            }
        }

        private void _ClearOldCache()
        {
            var e = _texturePool.GetEnumerator();
            var currentTime = Time.time;
            while (e.MoveNext())
            {
                var textureInfos = e.Current.Value;
                if (textureInfos != null && textureInfos.Count > 0)
                {
                    var textureInfo = textureInfos.Peek();
                    while (currentTime - textureInfo.cacheTime >= cachedTime)
                    {
                        textureInfos.Dequeue();
                        if (textureInfos.Count > 0)
                        {
                            textureInfo = textureInfos.Peek();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}