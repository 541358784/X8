using System;
using System.Collections.Generic;
using UnityEngine;
namespace DigTrench
{
    [Serializable]
    public class DigTrenchGameConfig
    {
        public List<Vector3Int> StartPosition;
        public List<Vector3Int> EndPosition;
        public List<Vector3Int> SideQuestPosition;
        public List<Vector3Int> TrapPosition;
        public List<ObstacleConfig> ObstaclePosition;
        public List<PropsConfig> PropsPosition;
        public CameraConfig CameraConfig;
        public List<ProgressPointConfig> ProgressPoints;
        public string ShowTargetWordsKey;
        public string StartWordsKey;
        public string EndWordsKey;
        public List<GuidePositionConfig> GuidePositionList;
        public int StartGuidePosition;
    }

    [Serializable]
    public class ObstacleConfig
    {
        public Vector3Int Position;
        public List<PropsGroup> PropsNeedConfig;
        public string EffectAsset;
        public int GuidePosition;
    }
    [Serializable]
    public class PropsGroup
    {
        public int PropsId;
        public int Count;

        public void Copy(PropsGroup source)
        {
            PropsId = source.PropsId;
            Count = source.Count;
        }
    }
    [Serializable]
    public class PropsConfig
    {
        public Vector3Int Position;
        public List<PropsGroup> PropsGetConfig;
    }
    [Serializable]
    public class CameraConfig
    {
        public JsonPosition Position;
        public float Size;
        public int CullingMask;
        public JsonPosition EndPosition;
        public float CameraMoveTime;
    }
    [Serializable]
    public class JsonPosition
    {
        public float x;
        public float y;
        public float z;
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class ProgressPointConfig
    {
        public Vector3Int Position;
        public string IconName;
    }

    [Serializable]
    public class GuidePositionConfig
    {
        public int GuidePosition;
        public Vector3Int Position;
    }
}