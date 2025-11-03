using UnityEngine;

namespace Framework
{
    public class ParticleSystemUtils
    {
        public static void Play(ParticleSystem ps, bool withChildren)
        {
            ps?.Play(withChildren);
        }

        public static void Play(ParticleSystem[] pss, bool withChildren)
        {
            if (pss != null)
            {
                for (int i = 0; i < pss.Length; i++)
                {
                    pss[i].Play(withChildren);
                }
            }
        }

        public static void Stop(ParticleSystem ps, bool withChildren)
        {
            ps?.Stop(withChildren);
        }

        public static void Stop(ParticleSystem[] pss, bool withChildren)
        {
            if (pss != null)
            {
                for (int i = 0; i < pss.Length; i++)
                {
                    pss[i].Stop(withChildren);
                }
            }
        }
    }
}