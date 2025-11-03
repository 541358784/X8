using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spine.Unity
{
    public class SpineAnimationManager : MonoBehaviour
    {
        private Dictionary<string,SpineAnimationEvent> _spineNodes = new Dictionary<string, SpineAnimationEvent>();
        // Start is called before the first frame update
        void Awake()
        {

            SpineAnimationEvent[] spineAnimationEvents = transform.GetComponentsInChildren<SpineAnimationEvent>();
            foreach (var animationevent in spineAnimationEvents)
            {
                if (!_spineNodes.ContainsKey(animationevent.gameObject.name))
                {
                    _spineNodes.Add(animationevent.gameObject.name,animationevent);
                }
            }
            
        }

      

        public void PlaySpineAnimation(string animation)
        {
            if (string.IsNullOrEmpty(animation))
            {
                Debug.LogWarning("SpineAnimationEventParam is null !");
                return;
            }

            string[] param = animation.Split('#');
            if (param.Length < 2)
            {
                Debug.LogWarning($"SpineAnimationEventParam format error {animation} !");
                return;
            }

            string nodename = param[0];

            if (_spineNodes.ContainsKey(nodename))
            {
                _spineNodes[nodename].PlaySpineAnimation(param[1]);
            }
        } 
        
        public void PlaySpineAnimationLoop(string animation)
        {
            if (string.IsNullOrEmpty(animation))
            {
                Debug.LogWarning("SpineAnimationEventParam is null !");
                return;
            }

            string[] param = animation.Split('#');
            if (param.Length < 2)
            {
                Debug.LogWarning($"SpineAnimationEventParam format error {animation} !");
                return;
            }

            string nodename = param[0];

            if (_spineNodes.ContainsKey(nodename))
            {
                _spineNodes[nodename].PlaySpineAnimationLoop(param[1]);
            }
        } 
    }

    
}

