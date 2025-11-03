using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Screw
{
    public enum LevelFailReason
    {
        None,
        Exit,
        CollectableArea,
        Timer,
        IceFailed,
        ConnectFailed,
        ShutterFailed,
        BombFailed,
        LockFailed,
        TieFailed
    }
    
    public enum ScrewGameState
    {
        None,
        InProgress,
        InUseBooster,
        Win,
        Fail
    }

    public enum GameFeatureType
    {
        None = 0,
        StarShape,
        TriangleShape,
        DiamondShape,
        BombBlocker,
        ConnectBlocker,
        IceBlocker,
        LockBlocker,
        ShutterBlocker,
        TieBlocker,
    }
    
    [Serializable]
    public enum ColorType : int
    {
        Green = 0,
        Cyan = 1,
        Yellow = 2,
        Blue = 3,
        Lilac = 4,
        Red = 5,
        Pink = 6,
        Purple = 7,
        Orange = 8,
        Grey = 9,
    };
    
    [Serializable]
    public enum ScrewShape : int
    {
        Phillips = 0,
        Star = 1,
        Diamond = 2,
        Triangle = 3,
    };
    
    [Serializable]
    public class Order
    {
        public ColorType colorType;

        public int slotCount;

        public List<ScrewShape> shapes = new List<ScrewShape>();
    }

    [Serializable]
    public enum ScrewBlocker : int
    {
        NONE = 0,
        ConnectBlocker = 1,
        IceBlocker = 2,
        ShutterBlocker = 3,
        BombBlocker = 4,
        LockBlocker = 5,
        TieBlocker = 6,
    };
    
    [Serializable]
    public class Vector3Float
    {
        public float x;
        public float y;
        public float z;

        public Vector3Float()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
        public Vector3Float(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3Float(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector3Float(Vector3 position)
        {
            x = position.x;
            y = position.y;
            z = position.z;
        }
    }
}