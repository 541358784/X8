using UnityEngine;

namespace Zuma
{
    [System.Serializable]
    public class Segment
    {
        public Transform p1;
        public Transform p2;
        public Transform cp1;
        public Transform cp2;


        public float Distance()
        {
            float distance = 0;
            if (cp1 == null && cp2 == null)
            {
                distance = Vector2.Distance(p1.position, p2.position);
            }
            else
            {
                if (cp1 && cp2)
                {
                    distance = Vector2.Distance(p1.position, cp1.position);
                    distance += Vector2.Distance(cp1.position, cp2.position);
                    distance += Vector2.Distance(p2.position, cp2.position);
                }
                else
                {
                    if (cp1)
                    {
                        distance = Vector2.Distance(p1.position, cp1.position);
                        distance += Vector2.Distance(p2.position, cp1.position);
                    }
                    else if (cp2)
                    {
                        distance = Vector2.Distance(p1.position, cp2.position);
                        distance += Vector2.Distance(p2.position, cp2.position);
                    }
                }
            }

            return distance;
        }
    }
}