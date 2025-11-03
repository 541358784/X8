using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace SSAtlas
{
    public class SpriteInfo
    {
        public SpriteInfo(string path, float z)
        {
            this.path = path;
            this.z = z;
        }

        public string path;
        public float z;
    }

    public class AnimationInfo
    {
        public AnimationInfo(string content, float z)
        {
            this.content = content;
            this.z = z;
        }

        public string content;
        public float z;
    }

    public class DecoSpriteAtlasPacker
    {
        static public void ClearDecoAtals()
        {
            for (int i = 0; i < 7; i++)
            {
                var assetPath =
                    $"Assets/Export/SpriteAtlas/DecorationAtlas/init_{i}/Hd/init_{i}.spriteatlas";
                var spriteAtals = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
                if (spriteAtals == null)
                {
                    Debug.LogError("需要手动创建新图集");
                }
                spriteAtals.Remove(spriteAtals.GetPackables());
                
                SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { spriteAtals }, BuildTarget.Android);
            }
        }
        
        static public void BakeAtlas()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("需要在运行模式下执行");
                return;
            }

            var worldMap = GameObject.Find("WorldMap");
            if (worldMap == null)
            {
                Debug.LogError("未找到worldMap");
                return;
            }

            //获取动画信息
            var animatorInitPath = new string[]
            {
                "Assets/Export/Animations/101/",
                "Assets/Export/Animations/102/",
                "Assets/Export/Animations/103/",
                "Assets/Export/Animations/104/",
                "Assets/Export/Animations/105/",
            };

            var animationContentList = new List<AnimationInfo>();
            var animators = worldMap.GetComponentsInChildren<Animator>(true);
            foreach (var animator in animators)
            {
                if (animator.runtimeAnimatorController != null &&
                    animator.runtimeAnimatorController.animationClips != null &&
                    animator.runtimeAnimatorController.animationClips.Length > 0)
                {
                    foreach (var clip in animator.runtimeAnimatorController.animationClips)
                    {
                        var path = AssetDatabase.GetAssetPath(clip);
                        foreach (var initPath in animatorInitPath)
                        {
                            if (path.Contains(initPath))
                            {
                                var file = System.IO.File.OpenText(path);
                                var fileContent = file.ReadToEnd();
                                animationContentList.Add(new AnimationInfo(fileContent, animator.transform.position.z));

                                file.Dispose();
                            }
                        }
                    }
                }
            }

            //获取初始目录内所有资源
            var initPaths = new string[]
            {
                "Assets/Export/Textures/Decoration/Buildings/World/",
                "Assets/Export/Textures/Decoration/Buildings/100/",
                "Assets/Export/Textures/Decoration/Buildings/101/",
                "Assets/Export/Textures/Decoration/Buildings/101Common/",
                "Assets/Export/Textures/Decoration/Buildings/102/",
                "Assets/Export/Textures/Decoration/Buildings/103/",
                "Assets/Export/Textures/Decoration/Buildings/104/",
                "Assets/Export/Textures/Decoration/Buildings/105/",
                
                "Assets/Export/Textures/Decoration/Buildings/101Default/",
                "Assets/Export/Textures/Decoration/Buildings/102Default/",
                "Assets/Export/Textures/Decoration/Buildings/103Default/",
                "Assets/Export/Textures/Decoration/Buildings/104Default/",
                "Assets/Export/Textures/Decoration/Buildings/105Default/",
            };

            var spriteListInFolder = new List<Object>();
            foreach (var path in initPaths)
            {
                if (System.IO.Directory.Exists(path))
                {
                    var direction = new System.IO.DirectoryInfo(path);
                    var files = direction.GetFiles("*", System.IO.SearchOption.AllDirectories);

                    for (int i = 0; i < files.Length; i++)
                    {
                        if (!files[i].Name.ToLower().EndsWith(".png") &&
                        !files[i].Name.ToLower().EndsWith(".jpg") &&
                        !files[i].Name.ToLower().EndsWith(".jpeg")
                        )
                        {
                            continue;
                        }
                        var index = files[i].FullName.IndexOf("Assets/Export", System.StringComparison.OrdinalIgnoreCase);
                        var assetPath = files[i].FullName.Substring(index);
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                        if (sprite == null)
                        {
                            Debug.LogError($"assetPath:{assetPath}");
                            continue;
                        }
                        spriteListInFolder.Add(sprite);
                    }
                }
            }

            Debug.Log("初始目录资源数量:" + spriteListInFolder.Count);

            var initSpriteList = new List<SpriteInfo>();

            //遍历初始目录内资源
            var spriteListInWorld = new List<SpriteRenderer>(worldMap.GetComponentsInChildren<SpriteRenderer>(true));
            spriteListInWorld.Sort((a, b) =>
            {
                var detal = a.transform.position.z - b.transform.position.z;
                if (detal == 0) return 0;

                return detal < 0 ? 1 : -1;
            });

            var unReferencedResList = new List<SpriteInfo>();
            foreach (var obj in spriteListInFolder)
            {
                var objPath = AssetDatabase.GetAssetPath(obj);
                //判断场景引用
                var spRenderInWorld = spriteListInWorld.Find(c => c.sprite == obj);
                if (spRenderInWorld != null)
                {
                    if (initSpriteList.Find(c => c.path == objPath) == null)
                    {
                        initSpriteList.Add(new SpriteInfo(objPath, spRenderInWorld.transform.position.z));
                    }
                }
                else //判断动画引用
                {
                    var guid = AssetDatabase.AssetPathToGUID(objPath);
                    var animInfo = animationContentList.Find(c => c.content.Contains(guid));
                    if (animInfo != null)
                    {
                        if (initSpriteList.Find(c => c.path == objPath) == null)
                        {
                            initSpriteList.Add(new SpriteInfo(objPath, animInfo.z));
                        }
                    }
                    else
                    {
                        if (objPath != "Assets/Export/Textures/Decoration/Buildings/World/shadow.png")
                        {
                            Debug.LogWarning("资源未被引用:" + objPath);
                        }

                        unReferencedResList.Add(new SpriteInfo(objPath, float.MaxValue));
                    }
                }
            }

            initSpriteList.Sort((a, b) =>
            {
                var delta = a.z - b.z;
                if (delta == 0)
                {
                    return 0;
                }

                return delta < 0 ? 1 : -1;
            });

            initSpriteList.AddRange(unReferencedResList);

            var leftCount = initSpriteList.Count;
            var atlasIndex = 0;
            var beginIndex = 0;
            var targetCount = leftCount;
            while (targetCount > 0)
            {
                if (saveSpriteAtlas(atlasIndex, beginIndex, targetCount, initSpriteList))
                {
                    atlasIndex++;
                    beginIndex = beginIndex + targetCount;
                    leftCount = initSpriteList.Count - beginIndex;
                    targetCount = leftCount;
                }
                else
                {
                    targetCount -= targetCount / 2;

                    var step = 0;

                    while (saveSpriteAtlas(atlasIndex, beginIndex, targetCount, initSpriteList))
                    {
                        step = 30;
                        targetCount += step;
                    }
                    targetCount -= step;

                    while (saveSpriteAtlas(atlasIndex, beginIndex, targetCount, initSpriteList))
                    {
                        step = 20;
                        targetCount += step;
                    }
                    targetCount -= step;

                    while (saveSpriteAtlas(atlasIndex, beginIndex, targetCount, initSpriteList))
                    {
                        step = 10;
                        targetCount += step;
                    }
                    targetCount -= step;

                    while (saveSpriteAtlas(atlasIndex, beginIndex, targetCount, initSpriteList))
                    {
                        step = 5;
                        targetCount += step;
                    }
                    targetCount -= step;

                    while (saveSpriteAtlas(atlasIndex, beginIndex, targetCount, initSpriteList))
                    {
                        step = 3;
                        targetCount += step;
                    }
                    targetCount -= step;

                    while (saveSpriteAtlas(atlasIndex, beginIndex, targetCount, initSpriteList))
                    {
                        step = 1;
                        targetCount += step;
                    }
                    targetCount -= step;
                }
            }
        }

        static private bool saveSpriteAtlas(int atlasIndex, int beginIndex, int targetCount,
            List<SpriteInfo> initSpList)
        {
            var assetPath =
                $"Assets/Export/SpriteAtlas/DecorationAtlas/init_{atlasIndex}/Hd/init_{atlasIndex}.spriteatlas";
            var spriteAtals = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
            if (spriteAtals == null)
            {
                Debug.LogError("需要手动创建新图集");
                return false;
            }
            spriteAtals.Remove(spriteAtals.GetPackables());

            var spriteNameList = new List<string>();
            for (int i = 0; i < targetCount; i++)
            {
                var index = beginIndex + i;
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(initSpList[index].path);
                spriteNameList.Add(sprite.name);
                spriteAtals.Add(new Object[] { sprite });
            }

            SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { spriteAtals }, BuildTarget.Android);
            // var allSprite = new Sprite[spriteAtals.spriteCount];
            // spriteAtals.GetSprites(allSprite);
            for (int i = 1; i < spriteNameList.Count; i++)
            {
                var lastSprite = spriteAtals.GetSprite(spriteNameList[i - 1]);
                var currentSprite = spriteAtals.GetSprite(spriteNameList[i]);
                if (lastSprite.texture != currentSprite.texture)
                    return false;
            }

            return true;
        }
    }
}