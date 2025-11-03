using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DG.Tweening;
using DragonPlus.Config.Makeover;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniGame
{
    public partial class AsmrLevel
    {
        private Dictionary<string, Vector3> _toolInitPos;
        private Dictionary<string, Quaternion> _toolInitRot;
        public Dictionary<Renderer, int> ToolsLayerDic;


        private void InitSpineAndColliders(GameObject root)
        {
            var spines = root.GetComponentsInChildren<SkeletonAnimation>();

            var normalAnimName = "Normal";
            foreach (var skeletonAnimation in spines)
            {
                skeletonAnimation.loop = false;
                skeletonAnimation.autoUpdate = false;

                if (skeletonAnimation.skeleton.Data.FindAnimation(normalAnimName) != null)
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, normalAnimName, false);
                    skeletonAnimation.Update(0);
                }
            }

            var colliders = gameObject.GetComponentsInChildren<Collider2D>();
            foreach (var collider2D in colliders)
            {
                if (collider2D.gameObject.name == "collision")
                {
                    collider2D.enabled = false;
                }
            }
        }

        private void InitToolsAndTarget(GameObject root)
        {
            //隐藏所有target,tool

            for (var i = 0; i < Config.groupIds.Length; i++)
            {
                var groupId = Config.groupIds[i];
                var groupConfig = MakeoverConfigManager.Instance.groupList.Find(c => c.id == groupId);
                for (var j = 0; j < groupConfig.stepIds.Length; j++)
                {
                    var stepId = groupConfig.stepIds[j];
                    var stepConfig = MakeoverConfigManager.Instance.stepNewList.Find(c => c.id == stepId);
                    if (!string.IsNullOrEmpty(stepConfig.target))
                    {
                        var path = stepConfig.target;
                        var target = root.transform.Find(path);
                        if (target != null) target.gameObject.SetActive(false);
                    }

                    if (!string.IsNullOrEmpty(stepConfig.toolPath))
                    {
                        var path = stepConfig.toolPath;
                        var tool = root.transform.Find(path);
                        if (tool != null)
                        {
                            tool.gameObject.SetActive(false);
                            if (!_toolInitPos.ContainsKey(tool.gameObject.name))
                            {
                                _toolInitPos.Add(tool.gameObject.name, tool.transform.position);
                            }

                            if (!_toolInitRot.ContainsKey(tool.gameObject.name))
                            {
                                _toolInitRot.Add(tool.gameObject.name, tool.transform.rotation);
                            }
                        }
                    }
                }
            }

            //缓存所有layer order信息
            ToolsLayerDic = new Dictionary<Renderer, int>();
            var renders = root.transform.GetComponentsInChildren<Renderer>(true);
            if (renders != null)
                foreach (var r2 in renders)
                    ToolsLayerDic.Add(r2, r2.sortingOrder);
        }

        public void ResetToolsPos(List<string> ignoreList)
        {
            for (var i = 0; i < Config.groupIds.Length; i++)
            {
                var groupId = Config.groupIds[i];
                var groupConfig = MakeoverConfigManager.Instance.groupList.Find(c => c.id == groupId);
                for (var j = 0; j < groupConfig.stepIds.Length; j++)
                {
                    var stepId = groupConfig.stepIds[j];
                    var stepConfig = MakeoverConfigManager.Instance.stepNewList.Find(c => c.id == stepId);

                    if (!string.IsNullOrEmpty(stepConfig.toolPath))
                    {
                        if (ignoreList != null && ignoreList.Contains(stepConfig.toolPath)) continue;

                        var path = stepConfig.toolPath; //$"{stepConfig.ToolAttachPath}/{stepConfig.ToolPath}";
                        var tool = transform.Find(path);
                        if (tool != null)
                        {
                            if (_toolInitPos.TryGetValue(tool.gameObject.name, out var pos))
                            {
                                tool.transform.position = pos;
                            }

                            if (_toolInitRot.TryGetValue(tool.gameObject.name, out var rot))
                            {
                                tool.transform.rotation = rot;
                            }
                        }
                    }
                }
            }
        }
    }
}