using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK;
using UnityEngine;
namespace Decoration
{
    public class RuntimeAtlas
    {
        public class TextureInfo
        {
            public TextureInfo(int x, int y, int textureWidth, int textureHeight, float textureScale, int textureIndex, Vector2 pivot, bool needPhysicsShape)
            {
                this.x = x;
                this.y = y;
                this.textureWidth = textureWidth;
                this.textureHeight = textureHeight;
                this.textureScale = textureScale;
                this.textureIndex = textureIndex;
                this.pivot = pivot;
                this.needPhysicsShape = needPhysicsShape;
            }

            public int x;
            public int y;
            public int textureWidth;
            public int textureHeight;
            public float textureScale;
            public int textureIndex;
            public Vector2 pivot;
            public bool needPhysicsShape;
            public string spriteName;
        }
        
        public RuntimeAtlas(int totalSize, int maxSize, int maxCount)
        { 
            _totalSize = totalSize;
            _maxTextureSize = maxSize;
            _maxTextureCount = maxCount;
        }

        public List<Texture2D> TextureList = new List<Texture2D>();
        private Dictionary<int, TextureInfo> _textureInfoDic = new Dictionary<int, TextureInfo>();
        private Dictionary<string, Material> _sharedMaterialDic = new Dictionary<string, Material>();

        private int _pendingSize;
        private int _textureIndex;
        private int _totalSize;
        private int _maxTextureSize;
        private int _maxTextureCount;
        private const string DEFAULT_SHADER_NAME = "Sprites/Default";

        private List<SpriteRenderer> _pendingList = new List<SpriteRenderer>();

        private List<Sprite> _runtimeSpriteList = new List<Sprite>();


        public class RuntimeTextureSetting
        {
            public int maxSize = 2048;
            public int textureCount = 1;

            public void HalfSelf()
            {
                if (maxSize != 2048)
                {
                    maxSize /= 2;
                }

                if (textureCount > 1)
                {
                    textureCount -= 1;
                }
            }
        }


        private RuntimeTextureSetting _maxTextureSetting;

        public RuntimeTextureSetting TextureSetting
        {
            get
            {
                if (_maxTextureSetting != null) return _maxTextureSetting;

                _maxTextureSetting = new RuntimeTextureSetting();

                switch (QualityMgr.Level)
                {
                    case QualityMgr.QualityLevel.VeryLow:
                        setMaxSizeAndTextureCount(2048, 2);
                        break;
                    case QualityMgr.QualityLevel.Low:
                        setMaxSizeAndTextureCount(4096, 1);
                        break;
                    case QualityMgr.QualityLevel.Medium:
                        setMaxSizeAndTextureCount(4096, 2);
                        break;
                    case QualityMgr.QualityLevel.High:
                        setMaxSizeAndTextureCount(8192, 1);
                        break;
                    case QualityMgr.QualityLevel.VeryHigh:
                        setMaxSizeAndTextureCount(8192, 1);
                        break;
                    default:
                        DebugUtil.LogError("动态图集-设备等级未处理");
                        break;
                }
#if UNITY_ANDROID
            //保护崩溃设备,显存未及时释放
            if (DeviceCheck.GraphicMemeryLeak())
            {
                _maxTextureSetting.HalfSelf();
            }
#endif

                return _maxTextureSetting;
            }
        }

        private void setMaxSizeAndTextureCount(int maxSize, int textureCount)
        {
            _maxTextureSetting.maxSize = Mathf.Min(maxSize, _maxTextureSize);
            _maxTextureSetting.textureCount = Mathf.Min(textureCount, _maxTextureCount);
        }

        private List<Vector2[]> getPhysics(Sprite sprite, float scale)
        {
            if (!sprite) return null;

            var physicsShapeList = new List<Vector2[]>();
            var list = new List<Vector2>();
            var physicsShapeCount = sprite.GetPhysicsShapeCount();
            for (int i = 0; i < physicsShapeCount; i++)
            {
                list.Clear();
                sprite.GetPhysicsShape(i, list);

                //转换坐标
                var listArray = new Vector2[list.Count];
                for (int j = 0; j < listArray.Length; j++)
                {
                    listArray[j] = list[j] * sprite.rect.size / 2 / sprite.bounds.extents + sprite.rect.size / 2;
                    listArray[j] *= scale;
                }

                if (listArray.Length >= 3)
                {
                    physicsShapeList.Add(listArray);
                }
            }

            return physicsShapeList;
        }

        public void ReleaseRuntimeTexture()
        {
            _runtimeSpriteList.Clear();

            foreach (var atlas in TextureList)
            {
                GameObject.DestroyImmediate(atlas);
            }

            TextureList.Clear();

            Resources.UnloadUnusedAssets();
        }

        public void ClearTemp()
        {
            _textureInfoDic.Clear();
            _pendingList.Clear();
            _textureIndex = 0;

            Resources.UnloadUnusedAssets();
        }

        public bool AddTest(SpriteRenderer render)
        {
            var spriteId = render.sprite.GetInstanceID();

            if (_textureInfoDic.ContainsKey(spriteId))
            {
                _pendingList.Add(render);
                return false;
            }

            return true;
        }

        private int _dynamicTotal = 0;

        public void AddToAtlas(SpriteRenderer render, bool needPhysicsShape)
        {
            var spriteId = render.sprite.GetInstanceID();


            var spriteRectInfo = new TextureInfo(0, 0, 0, 0, 1, 0, render.sprite.pivot, needPhysicsShape);
            _textureInfoDic.Add(spriteId, spriteRectInfo);


            //计算贴图分组
            var size = render.sprite.texture.width * render.sprite.texture.height;
            var maxArea = Mathf.CeilToInt(_totalSize / TextureSetting.textureCount);
            _dynamicTotal += size;
            //按尺寸分组，并且限制总贴图数量
            var needNewGroup = _pendingSize + size >= maxArea;
            var reachMaxGroup = _textureIndex >= TextureSetting.textureCount - 1;
            if (needNewGroup && !reachMaxGroup)
            {
                GenerateLast();
            }
            else
            {
                _pendingList.Add(render);
            }

            _pendingSize += size;
        }

        public void GenerateLast()
        {
            generateRuntimeAtlas(_pendingList, _textureIndex);
            _pendingList.Clear();

            _textureIndex++;
            DebugUtil.Log($"{this.GetHashCode()}_pendingSize:{_pendingSize}");
            _pendingSize = 0;

            Debug.Log("_dynamicTotal:" + _dynamicTotal);
        }


        private TextureFormat getTextureFormat()
        {
            var format = TextureFormat.RGBA32;
            if (SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_5x5))
            {
                if (QualityMgr.Level < QualityMgr.QualityLevel.High || DeviceCheck.GraphicMemeryLeak())
                {
                    format = TextureFormat.ASTC_6x6;
                }
                else
                {
                    format = TextureFormat.ASTC_5x5;
                }
            }
            else if (SystemInfo.SupportsTextureFormat(TextureFormat.ETC2_RGBA8))
            {
                if (QualityMgr.Level < QualityMgr.QualityLevel.High || DeviceCheck.GraphicMemeryLeak())
                {
                    format = TextureFormat.ETC2_RGBA1;
                }
                else
                {
                    format = TextureFormat.ETC2_RGBA8;
                }
            }
            else if (SystemInfo.SupportsTextureFormat(TextureFormat.PVRTC_RGBA4))
            {
                if (QualityMgr.Level < QualityMgr.QualityLevel.High || DeviceCheck.GraphicMemeryLeak())
                {
                    format = TextureFormat.PVRTC_RGBA2;
                }
                else
                {
                    format = TextureFormat.PVRTC_RGBA4;
                }
            }
            else
            {
                format = TextureFormat.RGBA4444;
            }

            return format;
        }

        private void generateRuntimeAtlas(List<SpriteRenderer> pendingList, int groupIndex)
        {
            var format = getTextureFormat();

            var texList = new Texture2D[pendingList.Count];
            for (int i = 0; i < texList.Length; i++)
            {
                texList[i] = pendingList[i].sprite.texture;
            }

            var runtimeAtlas = new Texture2D(4, 4, format, false);

            DebugUtil.Log($"RuntimeAtlas--format[{format}] Size:" + TextureSetting.maxSize);
            var closeReadable = true;
#if UNITY_EDITOR
        closeReadable = false;
#endif
            var rects = runtimeAtlas.PackTextures(texList, 2, TextureSetting.maxSize, closeReadable);
            if (rects == null) return;

            runtimeAtlas.name = "RuntimeAtlas";
            TextureList.Add(runtimeAtlas);

            for (var index = 0; index < rects.Length; index++)
            {
                try
                {
                    var sprite = pendingList[index].sprite;
                    if (sprite == null) continue;
                    if (sprite.texture == null) continue;

                    var rect = rects[index];
                    rect.x *= runtimeAtlas.width;
                    rect.y *= runtimeAtlas.height;

                    var spriteId = sprite.GetInstanceID();
                    var spriteRect = sprite.rect;
                    var textureScaleX = rect.width * runtimeAtlas.width / sprite.texture.width;
                    var textureScaleY = rect.height * runtimeAtlas.height / sprite.texture.height;
                    var newTextureWidth = spriteRect.width * textureScaleX;
                    var newTextureHeight = spriteRect.height * textureScaleY;
                    var newX = rect.x + spriteRect.x * textureScaleX;
                    var newY = rect.y + spriteRect.y * textureScaleY;

                    _textureInfoDic[spriteId].textureScale = Mathf.Min(textureScaleX, textureScaleY);
                    _textureInfoDic[spriteId].textureWidth = (int)newTextureWidth;
                    _textureInfoDic[spriteId].textureHeight = (int)newTextureHeight;
                    _textureInfoDic[spriteId].x = (int)newX;
                    _textureInfoDic[spriteId].y = (int)newY;
                    _textureInfoDic[spriteId].textureIndex = groupIndex;
                    _textureInfoDic[spriteId].spriteName = $"RT_{sprite.name}";
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            if (_textureInfoDic.Count > 0)
            {
                DebugUtil.Log("RuntimeAtlas--textureScale:" + _textureInfoDic.Values.First().textureScale);
            }

            replaceSpriteWithRuntimeAtlas(pendingList);

            pendingList.Clear();
        }

        //替换一批sp成动态图集
        private void replaceSpriteWithRuntimeAtlas(List<SpriteRenderer> pendingSpRenderList)
        {
            if (pendingSpRenderList.Count <= 0) return;

            for (var index = 0; index < pendingSpRenderList.Count; index++)
            {
                var spRender = pendingSpRenderList[index];
                var spriteId = spRender.sprite.GetInstanceID();

                if (_textureInfoDic.TryGetValue(spriteId, out var rectInfo))
                {
                    var rect = new Rect(rectInfo.x, rectInfo.y, rectInfo.textureWidth, rectInfo.textureHeight);
                    var runtimeAtlas = TextureList[rectInfo.textureIndex];

                    if (rectInfo.x + rectInfo.textureWidth > runtimeAtlas.width)
                    {
                        DebugUtil.LogError("动态图集坐标修正");
                        rect.width = runtimeAtlas.width - rectInfo.x;
                    }

                    if (rectInfo.y + rectInfo.textureHeight > runtimeAtlas.height)
                    {
                        DebugUtil.LogError("动态图集坐标修正");
                        rect.height = runtimeAtlas.height - rectInfo.y;
                    }

                    Sprite sprite = null;
                    try
                    {
                        var pivot = rectInfo.pivot;
                        pivot.x = pivot.x / rectInfo.textureWidth * rectInfo.textureScale;
                        pivot.y = pivot.y / rectInfo.textureHeight * rectInfo.textureScale;
                        sprite = Sprite.Create(runtimeAtlas, rect, pivot, 100 * rectInfo.textureScale, 0, SpriteMeshType.FullRect);
                        sprite.name = rectInfo.spriteName;
                        if (rectInfo.needPhysicsShape)
                        {
                            var physicsShape = getPhysics(spRender.sprite, rectInfo.textureScale);
                            sprite.OverridePhysicsShape(physicsShape);
                        }
                    }
                    catch (Exception e)
                    {
                        DebugUtil.LogError("RuntimeAtlas--" + $"{spRender.transform.parent.name.Replace("(Clone)", "")}/{spRender.gameObject.name}");
                        DebugUtil.LogError("RuntimeAtlas--" + e.ToString());
                    }

                    if (sprite != null && sprite.texture != null)
                    {
                        var matterialKey = $"{rectInfo.textureIndex}_rectInfo.usePointFilter";

                        _sharedMaterialDic.TryGetValue(matterialKey, out var material);
                        if (!material)
                        {
                            // if (rectInfo.usePointFilter)
                            // {
                            //     material = new Material(Shader.Find("Sprites/CustomDefaultPoint"));
                            // }
                            // else
                            // {
                            //     material = new Material(Shader.Find("Sprites/CustomDefaultBilinear"));
                            // }
                            material = new Material(Shader.Find(DEFAULT_SHADER_NAME));

                            _sharedMaterialDic.Add(matterialKey, material);
                        }

                        Resources.UnloadAsset(spRender.sprite.texture);
                        spRender.sprite = sprite;
                        // spRender.material = _sharedMaterialList[rectInfo.textureIndex];
                        if (spRender.sharedMaterial.shader.name == DEFAULT_SHADER_NAME)
                        {
                            spRender.sharedMaterial = material;
                        }

                        _runtimeSpriteList.Add(sprite);
                    }
                    else
                    {
                        DebugUtil.LogError("RuntimeAtlas--创建图集失败，使用默认图");
                    }
                }
            }

            Resources.UnloadUnusedAssets();
        }
    }
}