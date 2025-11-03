using System.Collections.Generic;
using ASMR;
using Spine.Unity;
using UnityEngine;

namespace fsm_new
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

            for (var i = 0; i < Config.GroupIds.Count; i++)
            {
                var groupId = Config.GroupIds[i];
                var groupConfig = ASMRModel.Instance.AsmrGroupConfigs.Find(c => c.Id == groupId);
                for (var j = 0; j < groupConfig.StepIds.Count; j++)
                {
                    var stepId = groupConfig.StepIds[j];
                    var stepConfig = ASMRModel.Instance.AsmrStepConfigs.Find(c => c.Id == stepId);
                    if (!string.IsNullOrEmpty(stepConfig.Target))
                    {
                        var path = stepConfig.Target;
                        var target = root.transform.Find(path);
                        if (target != null) target.gameObject.SetActive(false);
                    }

                    if (string.IsNullOrEmpty(stepConfig.ToolPath)) continue;
                    {
                        var path = stepConfig.ToolPath;
                        var tool = root.transform.Find(path);
                        if (tool == null) continue;

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

            //缓存所有layer order信息
            ToolsLayerDic = new Dictionary<Renderer, int>();
            var renders = root.transform.GetComponentsInChildren<Renderer>(true);
            if (renders != null)
                foreach (var r2 in renders)
                    ToolsLayerDic.Add(r2, r2.sortingOrder);
        }

        public void ResetToolsPos(List<string> ignoreList)
        {
            for (var i = 0; i < Config.GroupIds.Count; i++)
            {
                var groupId = Config.GroupIds[i];
                var groupConfig = ASMRModel.Instance.AsmrGroupConfigs.Find(c => c.Id == groupId);
                for (var j = 0; j < groupConfig.StepIds.Count; j++)
                {
                    var stepId = groupConfig.StepIds[j];
                    var stepConfig = ASMRModel.Instance.AsmrStepConfigs.Find(c => c.Id == stepId);

                    if (string.IsNullOrEmpty(stepConfig.ToolPath)) continue;
                    if (ignoreList != null && ignoreList.Contains(stepConfig.ToolPath)) continue;

                    var path = stepConfig.ToolPath; //$"{stepConfig.ToolAttachPath}/{stepConfig.ToolPath}";
                    var tool = transform.Find(path);
                    if (tool == null) continue;
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