using System.Collections.Generic;
using System.Linq;
using Framework;
using Screw.UserData;
using UnityEngine;

namespace Screw.Module
{
    public class ResBarModule : Singleton<ResBarModule>
    {
        private Dictionary<ResBarType, List<Transform>> _resBars = new Dictionary<ResBarType, List<Transform>>();
        private Dictionary<ResBarType, Transform> _currentBars = new Dictionary<ResBarType, Transform>();

        public void RegisterResBar(ResBarType type, Transform transform)
        {
            if (!_resBars.ContainsKey(type))
                _resBars[type] = new List<Transform>();
            
            if(!_resBars[type].Contains(transform))
                _resBars[type].Add(transform);

            _currentBars[type] = transform;
        }

        public void UnRegisterResBar(ResBarType type, Transform transform)
        {
            var resBars = GetResBars(type);
            if(resBars == null)
                return;

            resBars.Remove(transform);
            
            if(!_currentBars.ContainsKey(type))
                return;
            
            if(_currentBars[type] != transform)
                return;

            for (int i = resBars.Count - 1; i >= 0; i--)
            {
                if (resBars[i] == null)
                {
                    resBars.RemoveAt(i);
                    continue;
                }

                _currentBars[type] = resBars[i];
                
                break;
            }
        }

        public Transform GetFlyTransform(int id)
        {
            ResType resType = UserData.UserData.GetResType(id);
            ResBarType type = ResBarType.None;
            
            switch (resType)
            {
                case ResType.Coin:
                {
                    type = ResBarType.Coin;
                    break;
                }
                case ResType.Energy:
                case ResType.EnergyInfinity:
                {
                    type = ResBarType.Energy;
                    break;
                }
                case ResType.BreakBody:
                {
                    type =  ResBarType.BreakBody;
                    break;
                }
                case ResType.ExtraSlot:
                {
                    type =  ResBarType.ExtraSlot;
                    break;
                }
                case ResType.TwoTask:
                {
                    type =  ResBarType.TwoTask;
                    break;
                }
                default:
                {
                    type = ResBarType.MainPlay;
                    break;
                }
            }

            Transform resTransform = GetResBar(type);
            if (resTransform == null)
                resTransform = GetResBar(ResBarType.MainPlay);

            return resTransform;
        }

        public Vector3 GetFlyDestPosition(int id)
        {
            var transform = GetFlyTransform(id);
            if(transform == null)
                return Vector3.zero;

            return transform.position;
        }
        
        public void Release()
        {
            _resBars.Clear();
            _currentBars.Clear();
        }
        
        private List<Transform> GetResBars(ResBarType type)
        {
            if (!_resBars.ContainsKey(type))
                return null;

            return _resBars[type];
        }

        private Transform GetResBar(ResBarType type)
        {
            if (!_currentBars.ContainsKey(type))
                return null;

            return _currentBars[type];
        }
    }
}