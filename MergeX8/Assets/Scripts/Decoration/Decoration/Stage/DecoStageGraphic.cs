using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Deco.Graphic;
using DragonPlus;
using DragonPlus.Config;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using EpForceDirectedGraph.cs;
using UnityEngine.Tilemaps;
using Framework;
using Spine.Unity;

namespace Deco.Stage
{
    /// <summary>
    /// Stage本身无具象，逻辑层
    /// </summary>
    public class DecoStageGraphic : DecoGraphic
    {
        private DecoStage _stage;

        public DecoStageGraphic(DecoStage stage)
        {
            _stage = stage;
        }

        protected override string PREFAB_PATH { get; }

        protected override void OnUnload()
        {
        }

        protected override void OnLoad()
        {
            foreach (var kv in _stage._nodeDict)
            {
                var node = kv.Value;
                var nodeId = kv.Key;

                var nodeTransform = _parentTransform.Find(nodeId.ToString());

                if (nodeTransform != null)
                {
                    node.LoadGraphic(nodeTransform.gameObject);
                }
                else
                {
                    DebugUtil.LogError(string.Format("## Node [{0}] not found in [{1}] area prefab ##", nodeId, _stage.Area.Id));
                }
            }
        }
    }
}