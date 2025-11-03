using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonPlus.Config.TMatch;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TMatch
{


    public class LevelConfigTool
    {
        [MenuItem("Tools/Match/检查固定配置是否合法")]
        public static void CheckConfigValid()
        {
            List<string> errors = GetConfigError();

            foreach (var p in errors)
            {
                Debug.LogError(p);
            }

            if (errors.Count > 0) Debug.LogError("Match关卡配置检查 未通过!!!");
            else Debug.Log("Match关卡配置检查 通过.");
        }

        [MenuItem("Tools/Match/检查动态配置是否合法")]
        public static async void CheckDynamicConfigValid()
        {
            foreach (var p in TMatchConfigManager.Instance.LayoutDesignList)
            {
                for (int v = 1; v <= 3; v++)
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        TMatchConfigManager.Instance.dynamicCfgs.Clear();
                        TMatchConfigManager.Instance.DeleteDynamicCfgCache(p.levelMin);
                        PlayerPrefs.SetInt(string.Format(TMatchConfigManager.TMatchCacheKeyDynamicN, p.levelMin), v);
                        PlayerPrefs.SetInt(TMatchConfigManager.TMatchCacheKeyPreDifficult, 3);
                        TMatchConfigManager.TMacthDynamicCfg dynamicCfg =
                            TMatchConfigManager.Instance.GetDynamicCfg(p.levelMin);
                        
                        TMatchConfigManager.AddDicLayout(dynamicCfg.NormalLayout);
                        if (!TMatchConfigManager.CheckLayoutValid(
                                TMatchConfigManager.GetDicLayoutById(dynamicCfg.NormalLayout.id),
                                dynamicCfg.NormalLayout.id))
                        {
                            TMatchConfigManager.RemoveDicLayout(dynamicCfg.NormalLayout);
                            return;   
                        }
                        TMatchConfigManager.RemoveDicLayout(dynamicCfg.NormalLayout);
                        TMatchConfigManager.AddDicLayout(dynamicCfg.easyerLayout);
                        if (!TMatchConfigManager.CheckLayoutValid(
                                TMatchConfigManager.GetDicLayoutById(dynamicCfg.easyerLayout.id),
                                dynamicCfg.easyerLayout.id))
                        {
                            TMatchConfigManager.RemoveDicLayout(dynamicCfg.easyerLayout);
                            return;   
                        }
                        TMatchConfigManager.RemoveDicLayout(dynamicCfg.easyerLayout);
                    }

                    Debug.Log($"{p.levelMin} - {p.levelMax} difficlut : {v} finish.");
                    // await Task.Yield();
                }
            }

            Debug.Log("检查动态配置是否合法 通过.");
        }

        public static List<string> GetConfigError()
        {
            List<string> errors = new List<string>();

            List<Level> LevelList = new List<Level>();
            List<Layout> LayoutList = new List<Layout>();
            List<Item> ItemList = new List<Item>();

            //0.加载配置
            {
                // TextAsset levelText =
                //     AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Export/Configs/TMatchConfig/level.json");
                // LevelList = JsonConvert.DeserializeObject<List<Level>>(levelText.text);
                // TextAsset layoutText =
                //     AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Export/Configs/TMatchConfig/layout.json");
                // LayoutList = JsonConvert.DeserializeObject<List<Layout>>(layoutText.text);
                // TextAsset itemText =
                //     AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Export/Configs/TMatchConfig/item.json");
                // ItemList = JsonConvert.DeserializeObject<List<Item>>(itemText.text);
                LevelList = TMatchConfigManager.Instance.LevelList;
                LayoutList = TMatchConfigManager.Instance.LayoutList;
                ItemList = TMatchConfigManager.Instance.ItemList;
            }

            //1.检查Level配置的groupID为100或者200
            {
                List<Level> levelList = LevelList;
                if (levelList.FindAll(x => x.levelUserGroup == 100 ||
                                           x.levelUserGroup == 200 ||
                                           x.levelUserGroup == 300 ||
                                           x.levelUserGroup == 400 ||
                                           x.levelUserGroup == 500).Count != levelList.Count)
                {
                    errors.Add("level的配置的LevelUserGroup既不是100也不是200也不是300也不是400也不是500！");
                }
            }

            //2.检查Level配置不同分层的配置数量是一样的
            {
                List<Level> levelList = LevelList;
                HashSet<int> groupIds = new HashSet<int>();
                foreach (var level in levelList) groupIds.Add(level.levelUserGroup);
                int checkCount = -1;
                foreach (var groupId in groupIds)
                {
                    int count = levelList.FindAll(x => x.levelUserGroup == groupId).Count;
                    if (checkCount == -1) checkCount = count;
                    if (checkCount != count)
                    {
                        errors.Add($"分层：{groupId}的level配置数量为：{count}, 和其余的不同!");
                    }
                }
            }

            //3.检查Level依赖的layoutID都是存在的
            {
                List<Level> levelList = LevelList;
                List<Layout> layouts = LayoutList;
                foreach (var level in levelList)
                {
                    if (layouts.Find(x => x.id == level.layoutId) == null)
                    {
                        errors.Add($"level：{level.id}使用的LayoutId：{level.layoutId} 不存在！");
                    }

                    if (layouts.Find(x => x.id == level.easierLayoutId) == null)
                    {
                        errors.Add($"level：{level.id}使用的EasierLayoutId：{level.easierLayoutId} 不存在！");
                    }
                }
            }

            //4.检查item的layer值是否合法
            {
                List<Item> items = ItemList;
                if (items.FindAll(x => x.layer == 1 || x.layer == 2 || x.layer == 3 || x.layer == 4).Count !=
                    items.Count)
                {
                    errors.Add($"Item里的layer值不是1、2、3、4中的一个！");
                }
            }

            //5.检查item里的prefab是否合法
            {
                List<Item> items = ItemList;
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.prefabName))
                    {
                        errors.Add($"item : {item.id}的PrefabName为空！");
                    }

                    GameObject prefab =
                        AssetDatabase.LoadMainAssetAtPath($"Assets/Export/TMatch/TMatch/Item/Prefabs/{item.prefabName}.prefab")
                            as GameObject;
                    if (prefab == null)
                    {
                        errors.Add($"找不到预制件：{item.prefabName}资源！");
                        continue;
                    }

                    GameObject gameObject = GameObject.Instantiate(prefab);
                    if (gameObject == null)
                    {
                        errors.Add($"预制件：{item.prefabName}无法实例化");
                        continue;
                    }

                    MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                    if (meshFilter == null || meshFilter.sharedMesh == null)
                    {
                        errors.Add($"预制件：{item.prefabName}的MeshFilter有问题！");
                    }

                    MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                    if (meshRenderer == null ||
                        meshRenderer.shadowCastingMode != ShadowCastingMode.On ||
                        !meshRenderer.receiveShadows ||
                        meshRenderer.sharedMaterial == null)
                    {
                        errors.Add($"预制件：{item.prefabName}的meshRenderer有问题！");
                    }

                    Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
                    if (rigidbody == null ||
                        Mathf.Abs(rigidbody.mass - 1.0f) > 0.001f ||
                        Mathf.Abs(rigidbody.drag - 1.0f) > 0.001f ||
                        Mathf.Abs(rigidbody.angularDrag - 0.5f) > 0.001f ||
                        !rigidbody.useGravity ||
                        rigidbody.isKinematic ||
                        rigidbody.interpolation != RigidbodyInterpolation.None ||
                        rigidbody.collisionDetectionMode != CollisionDetectionMode.Discrete ||
                        rigidbody.constraints != RigidbodyConstraints.None)
                    {
                        errors.Add($"预制件：{item.prefabName}的rigidbody有问题！");
                    }

                    MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
                    if (meshCollider == null ||
                        meshCollider.sharedMesh == null ||
                        meshCollider.sharedMaterial != null ||
                        !meshCollider.convex ||
                        meshCollider.isTrigger)
                    {
                        errors.Add($"预制件：{item.prefabName}的meshCollider有问题！");
                    }

                    GameObject.DestroyImmediate(gameObject);
                }
            }

            //6.检查item里的layRot是否合法
            {
                List<Item> items = ItemList;
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.layRot))
                    {
                        float x, y, z;
                        string[] temp = item.layRot.Split(',');
                        if (temp.Length != 3 ||
                            !float.TryParse(temp[0], out x) ||
                            !float.TryParse(temp[1], out y) ||
                            !float.TryParse(temp[2], out z))
                        {
                            errors.Add($"Item里的layRot非法！");
                        }
                    }
                }
            }

            //7.检查item里的ScalingRatio是否合法
            {
                List<Item> items = ItemList;
                foreach (var item in items)
                {
                    if (!string.IsNullOrEmpty(item.scalingRatio))
                    {
                        float x, y, z;
                        string[] temp = item.scalingRatio.Split(',');
                        if (temp.Length != 3 ||
                            !float.TryParse(temp[0], out x) ||
                            !float.TryParse(temp[1], out y) ||
                            !float.TryParse(temp[2], out z))
                        {
                            errors.Add($"Item里的ScalingRatio非法！");
                        }
                    }
                }
            }

            //8.检查layout是否合法
            {
                List<Layout> layouts = LayoutList;
                foreach (var layout in layouts)
                {
                    if (!TMatchConfigManager.CheckLayoutValid(layout, layout.id))
                    {
                        errors.Add($"layout : {layout.id} 非法");
                    }

                    foreach (var id in layout.targetItemId)
                    {
                        List<Item> items = ItemList;
                        var itemCfg = items.Find(x => x.id == id);
                        if (!string.IsNullOrEmpty(itemCfg.effectName))
                        {
                            errors.Add($"layout: {layout.id} TargetItemId使用了活动模型，模型ID为{id}");
                        }
                    }

                    if (layout.normalItemId != null)
                    {
                        foreach (var id in layout.normalItemId)
                        {
                            List<Item> items = ItemList;
                            var itemCfg = items.Find(x => x.id == id);
                            if (!string.IsNullOrEmpty(itemCfg.effectName))
                            {
                                errors.Add($"layout: {layout.id} NormalItemId使用了活动模型，模型ID为{id}");
                            }
                        }
                    }

                }
            }

            return errors;
        }
    }
}