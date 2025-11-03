using System;
using System.Collections.Generic;

namespace DragonU3DSDK
{
    public class ManagerBehaviour : Manager<ManagerBehaviour>
    {
        private List<Action<bool>> applicationFocusDelegates = new List<Action<bool>>();
        private List<Action<bool>> applicationPauseDelegates = new List<Action<bool>>();

        public void AddApplicationFocusDelegate(Action<bool> action)
        {
            if (action != null)
            {
                applicationFocusDelegates.Add(action);
            }
        }
        
        public void AddApplicationPauseDelegate(Action<bool> action)
        {
            if (action != null)
            {
                applicationPauseDelegates.Add(action);
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (applicationFocusDelegates.Count > 0)
            {
                List<Action<bool>> tempList = new List<Action<bool>>(applicationFocusDelegates);
                for (int i = 0; i < tempList.Count; i++)
                {
                    tempList[i]?.Invoke(focus);
                }
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (applicationPauseDelegates.Count > 0)
            {
                List<Action<bool>> tempList = new List<Action<bool>>(applicationPauseDelegates);
                for (int i = 0; i < tempList.Count; i++)
                {
                    tempList[i]?.Invoke(pause);
                }
            }
        }
    }
}
