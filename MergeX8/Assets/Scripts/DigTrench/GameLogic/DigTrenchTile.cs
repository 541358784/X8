using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DigTrench
{
    
    public class DigTrenchTile
    {
        public TileBase Tile;
        public Vector3Int Position;
        public bool Init = false;
        public TileType Type;
        public bool HasVideo = false;//节点上是否有动画
        public List<PropsGroup> PropsNeedList;
        public List<PropsGroup> PropsGetList;
        public string EffectAssetName;
        public int GuidePosition;
        public int ObstacleGuidePosition;

        public Dictionary<ConnectDirection, ConnectState> TileConnectState =
            new Dictionary<ConnectDirection, ConnectState>()
            {
                {ConnectDirection.XPositive, ConnectState.Wall},
                {ConnectDirection.XNegative, ConnectState.Wall},
                {ConnectDirection.YPositive, ConnectState.Wall},
                {ConnectDirection.YNegative, ConnectState.Wall},
            };

        public ConnectState XPositive
        {
            get => TileConnectState[ConnectDirection.XPositive];
            set => TileConnectState[ConnectDirection.XPositive] = value;
        }
        public ConnectState XNegative
        {
            get => TileConnectState[ConnectDirection.XNegative];
            set => TileConnectState[ConnectDirection.XNegative] = value;
        }
        public ConnectState YPositive
        {
            get => TileConnectState[ConnectDirection.YPositive];
            set => TileConnectState[ConnectDirection.YPositive] = value;
        }
        public ConnectState YNegative
        {
            get => TileConnectState[ConnectDirection.YNegative];
            set => TileConnectState[ConnectDirection.YNegative] = value;
        }
        public bool IsOpen = false;

        public string GetNodeName()
        {
            return Position.x + "," + Position.y;
        }
    }
}