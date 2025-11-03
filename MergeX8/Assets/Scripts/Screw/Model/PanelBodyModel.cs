using System.Collections.Generic;
using Screw;
using Screw.Configs;
using UnityEngine;

namespace Screw
{
    public class PanelBodyModel
    {
        public int LayerId { get; }

        public int PanelId { get; }

        public ColorType PanelColor { get; }

        public Vector2 Scale { get; }

        public Vector3 Position { get; }
        public Vector3 EulerAngles { get; }

        public Vector3 BodyScale { get; }
        
        public Vector3 BodyRotate { get; }

        public Vector3 ShadowPos { get; }

        public string BodyRes { get; }

        public Dictionary<int, HoleModel> HoleModels { get; }

        public bool IsActive { get; set; }

        public PanelBodyModel(int layerId, LevelPanel panel, Dictionary<int, HoleModel> holeModels)
        {
            IsActive = true;
            LayerId = layerId;
            PanelId = panel.instanceId;
            PanelColor = panel.colorType;
            Scale = new Vector2(panel.scale.x, panel.scale.y);
            // 注意，不使用关卡配置的Z轴，214出现层级错误
            Position = new Vector3(panel.position.x, panel.position.y, 0);
            EulerAngles = new Vector3(0, 0, panel.rotate.z);
            BodyScale = new Vector3(panel.bodyScale.x, panel.bodyScale.y, panel.bodyScale.x);
            BodyRotate = new Vector3(0, 0, panel.bodyRotate.z);
            BodyRes = panel.bodyImageName;
            ShadowPos = new Vector3(panel.shadowPosition.x, panel.shadowPosition.y, panel.shadowPosition.z);
            HoleModels = holeModels;
        }
    }
}