using System.Collections.Generic;
using UnityEngine;
#if ! RENDER_SERVER
using DragonU3DSDK;

#endif

namespace Framework
{
    public class Skeleton
    {
        private Dictionary<string, Transform> _bones;

        public Skeleton(Transform[] transforms)
        {
            if (transforms != null)
            {
                _bones = new Dictionary<string, Transform>(transforms.Length);
                foreach (var t in transforms)
                {
                    if (_bones.ContainsKey(t.name))
                    {
#if RENDER_SERVER
                        DragonU3DSDK.DebugUtil.LogError("Skeleton Add Bone, a bone has already been collected with same name : {0}", t.name);
#else
                        DebugUtil.LogError("Skeleton Add Bone, a bone has already been collected with same name : {0}",
                            t.name);
#endif

                        continue;
                    }

                    _bones.Add(t.name, t);
                }
            }
        }

        public Transform GetBone(string name)
        {
            Transform bone = null;
            if (_bones != null && _bones.TryGetValue(name, out bone))
            {
            }

            return bone;
        }
    }
}