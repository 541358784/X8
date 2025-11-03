using System;
using System.Collections.Generic;
using DragonU3DSDK;
using Framework;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework
{
    public class GameplayInfoManager : GlobalSystem<GameplayInfoManager>
    {
        public class GameplayInfo
        {
            public enum GameInfoType
            {
                Unit,
                MatNode,
            }

            public int id;
            public GameInfoType type;
        }

        private Dictionary<int, GameplayInfo> _infos = new Dictionary<int, GameplayInfo>(32);

        public void Register(int gameObjectId, GameplayInfo info)
        {
            _infos.Add(gameObjectId, info);
        }


        public void UnRegister(int gameObjectId)
        {
            if (_infos.ContainsKey(gameObjectId))
            {
                _infos.Remove(gameObjectId);
            }
        }


        public GameplayInfo QueryByRaycast(Ray ray, int layerMask, float maxDistance = 100, int queryDeep = 6)
        {
            try
            {
                GameplayInfo gameplayInfo;
                RaycastHit hitInfo;
                UnityEngine.Physics.Raycast(ray, out hitInfo, maxDistance, layerMask);
                var trans = hitInfo.transform;
                while (trans != null && --queryDeep >= 0)
                {
                    var id = trans.gameObject.GetInstanceID();
                    if (_infos.TryGetValue(id, out gameplayInfo))
                    {
                        return gameplayInfo;
                    }

                    trans = trans.parent;
                }

                return null;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            return null;
        }

        public GameplayInfo QueryByTransform(Transform trans, int queryDeep = 10)
        {
            try
            {
                GameplayInfo gameplayInfo;
                while (trans != null && --queryDeep >= 0)
                {
                    var id = trans.gameObject.GetInstanceID();
                    if (_infos.TryGetValue(id, out gameplayInfo))
                    {
                        return gameplayInfo;
                    }

                    trans = trans.parent;
                }

                return null;
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e);
            }

            return null;
        }
    }
}