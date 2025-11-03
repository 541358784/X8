using System.Collections.Generic;
using Screw.Configs;
using UnityEngine;

namespace Screw
{
    public class ShieldModel
    {
        public int LayerId { get; }

        public int ShieldId { get; }

        public Vector3 Scale { get; }
        public Vector3 Position { get; }

        public Vector3 Rotate { get; }
        public Vector3 BodyScale { get; }
        public Vector3 BodyRotate { get; }
        public string BodyImageName { get; }

        public List<int> CoverPanelIds { get; }

        public bool IsActive { get; set; }
        public ShieldModel(int layerId, LevelShield shield)
        {
            IsActive = true;
            
            LayerId = layerId;

            ShieldId = shield.instanceId;
            Scale = new Vector2(shield.scale.x, shield.scale.y);
            Rotate = new Vector3(0, 0, shield.rotate.z);
            Position = new Vector3(shield.position.x, shield.position.y, 0);
            
            BodyScale = new Vector3(shield.bodyScale.x, shield.bodyScale.y, shield.bodyScale.x);
            BodyRotate = new Vector3(0, 0, shield.bodyRotate.z);
            BodyImageName = shield.bodyImageName;

            CoverPanelIds = new List<int>(shield.coverPanelIds);
        }
    }
}