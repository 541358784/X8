using System.Collections.Generic;
using Screw;
using Screw.Configs;
using UnityEngine;

namespace Screw
{
    public class ScrewModel
    {
        public int LayerId { get; }
        public int ScrewId { get; }

        public ScrewShape ScrewShape { get; }
        public Vector3 Position { get; }

        public ColorType ScrewColor { get; }

        // 关卡配置了，但暂时钉子是固定大小的，所以不使用改字段
        private float _radius;

        private Dictionary<ScrewBlocker, BaseBlockerModel> _screwBlockers;

        public Dictionary<ScrewBlocker, BaseBlockerModel> ScrewBlockers => _screwBlockers;

        public bool HasBlocker => ScrewBlockers != null && ScrewBlockers.Count > 0;

        public ScrewModel(int layerId, int screwId, Vector3Float position, ColorType screwColor, float radius, List<LevelScrewBlock> screwBlockers, ScrewShape screwShape, ScrewGameContext context)
        {
            LayerId = layerId;
            ScrewId = screwId;
            ScrewShape = screwShape;
            Position = new Vector3(position.x, position.y, position.z);
            ScrewColor = screwColor;
            _radius = radius;

            if (screwBlockers != null && screwBlockers.Count > 0)
            {
                _screwBlockers = new Dictionary<ScrewBlocker, BaseBlockerModel>();
                foreach (var screwBlocker in screwBlockers)
                {
                    switch (screwBlocker.blockType)
                    {
                        case ScrewBlocker.ConnectBlocker:
                            var connectBlocker = new ConnectBlockerModel();
                            connectBlocker.Init(this, screwBlocker, context);
                            _screwBlockers.Add(ScrewBlocker.ConnectBlocker, connectBlocker);
                            break;
                        case ScrewBlocker.IceBlocker:
                            var iceBlockerModel = new IceBlockerModel();
                            iceBlockerModel.Init(this, screwBlocker, context);
                            _screwBlockers.Add(ScrewBlocker.IceBlocker, iceBlockerModel);
                            break;
                        case ScrewBlocker.ShutterBlocker:
                            var shutterBlockerModel = new ShutterBlockerModel();
                            shutterBlockerModel.Init(this, screwBlocker, context);
                            _screwBlockers.Add(ScrewBlocker.ShutterBlocker, shutterBlockerModel);
                            break;
                        case ScrewBlocker.BombBlocker:
                            var bombBlockerModel = new BombBlockerModel();
                            bombBlockerModel.Init(this, screwBlocker, context);
                            _screwBlockers.Add(ScrewBlocker.BombBlocker, bombBlockerModel);
                            break;
                        case ScrewBlocker.LockBlocker:
                            var lockBlockerModel = new LockBlockerModel();
                            lockBlockerModel.Init(this, screwBlocker, context);
                            _screwBlockers.Add(ScrewBlocker.LockBlocker, lockBlockerModel);
                            break;
                        case ScrewBlocker.TieBlocker:
                            var tieBlockerModel = new TieBlockerModel();
                            tieBlockerModel.Init(this, screwBlocker, context);
                            _screwBlockers.Add(ScrewBlocker.TieBlocker, tieBlockerModel);
                            break;
                    }
                }
            }
        }
    }
}