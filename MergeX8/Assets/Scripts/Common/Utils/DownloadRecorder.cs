using System.Collections.Generic;
using Framework;

namespace Gameplay
{
    public class DownloadRecorder : GlobalSystem<DownloadRecorder>
    {
        private Dictionary<string, int> startDownloadTime = new Dictionary<string, int>(16);


        public void Start(string key)
        {
            if (startDownloadTime.ContainsKey(key))
            {
                startDownloadTime[key] = (int) UnityEngine.Time.time;
            }
            else
            {
                startDownloadTime.Add(key, (int) UnityEngine.Time.time);
            }
        }

        public int End(string key)
        {
            var time = GetDownloadTime(key);
            startDownloadTime.Remove(key);
            return time;
        }

        public int GetDownloadTime(string key)
        {
            if (startDownloadTime.ContainsKey(key))
            {
                return (int) UnityEngine.Time.time - startDownloadTime[key];
            }

            return 0;
        }
    }
}