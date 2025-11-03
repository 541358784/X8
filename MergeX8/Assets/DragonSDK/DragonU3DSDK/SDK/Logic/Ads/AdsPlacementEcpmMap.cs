using System;
using System.Collections.Generic;

namespace Dlugin
{
    public class AdsPlacementEcpmMap
    {
        private static AdsPlacementEcpmMap instance = null;
        private static readonly object syslock = new object();

        public static AdsPlacementEcpmMap Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new AdsPlacementEcpmMap();
                        }
                    }
                }
                return instance;
            }
        }


        private Dictionary<string, float> m_PlacementEcpmMap = null;

        private AdsPlacementEcpmMap()
        {
            m_PlacementEcpmMap  = new Dictionary<string, float>();
        }


        public void Put(string placement,float ecpm)
        {
            if(!"0".Equals(placement))
                m_PlacementEcpmMap[placement] = ecpm;
        }

        public float GetEcpmFloor(string placement)
        {
            if (string.IsNullOrEmpty(placement))
            {
                return 0f;
            }

            if (!m_PlacementEcpmMap.ContainsKey(placement))
            {
                return 0f;
            }

            return m_PlacementEcpmMap[placement];
        }
    }
}