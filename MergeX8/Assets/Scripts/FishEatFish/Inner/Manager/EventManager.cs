using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace FishEatFishSpace
{
//-----------------------------------------------------------
//Enum defining all possible game events
//More events should be added to the list
    public enum GameEvent
    {
        //OnGameStart
    };

//-----------------------------------------------------------
//Singleton EventManager to send events to listeners
//Works with IListener implementations
    public class EventManager : Singleton<EventManager>
    {

        #region variables

        //Internal reference to Notifications Manager instance (singleton design pattern)
        // private static EventManager instance = null;

        // Declare a delegate type for events
        public delegate void OnEvent(GameEvent Event_Type, Component Sender, object Param = null);

        //Array of listener objects (all objects registered to listen for events)
        private Dictionary<GameEvent, List<OnEvent>> Listeners = new Dictionary<GameEvent, List<OnEvent>>();

        #endregion

        //-----------------------------------------------------------

        #region methods

        //-----------------------------------------------------------
        /// <summary>
        /// Function to add specified listener-object to array of listeners
        /// </summary>
        /// <param name="Event_Type">Event to Listen for</param>
        /// <param name="Listener">Object to listen for event</param>
        public void AddListener(GameEvent Event_Type, OnEvent Listener)
        {
            //List of listeners for this event
            List<OnEvent> ListenList = null;

            //New item to be added. Check for existing event type key. If one exists, add to list
            if (Listeners.TryGetValue(Event_Type, out ListenList))
            {
                //List exists, so add new item
                ListenList.Add(Listener);
                return;
            }

            //Otherwise create new list as dictionary key
            ListenList = new List<OnEvent>();
            ListenList.Add(Listener);
            Listeners.Add(Event_Type, ListenList); //Add to internal listeners list
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Function to post event to listeners
        /// </summary>
        /// <param name="Event_Type">Event to invoke</param>
        /// <param name="Sender">Object invoking event</param>
        /// <param name="Param">Optional argument</param>
        public void PostNotification(GameEvent Event_Type, Component Sender, object Param = null)
        {
            //Notify all listeners of an event

            //List of listeners for this event only
            List<OnEvent> ListenList = null;

            //If no event entry exists, then exit because there are no listeners to notify
            if (!Listeners.TryGetValue(Event_Type, out ListenList))
                return;

            //Entry exists. Now notify appropriate listeners
            for (int i = 0; i < ListenList.Count; i++)
            {
                if (!ListenList[i].Equals(null)) //If object is not null, then send message via interfaces
                    ListenList[i](Event_Type, Sender, Param);
            }
        }

        //-----------------------------------------------------------
        //Remove event type entry from dictionary, including all listeners
        public void RemoveEvent(GameEvent Event_Type)
        {
            //Remove entry from dictionary
            Listeners.Remove(Event_Type);
        }

        public void RemoveEvent(GameEvent Event_Type, OnEvent onEvent)
        {
            //List of listeners for this event
            List<OnEvent> ListenList = null;

            if (Listeners.TryGetValue(Event_Type, out ListenList))
            {
                if (ListenList.Contains(onEvent))
                {
                    ListenList.Remove(onEvent);
                }
            }
        }

        //-----------------------------------------------------------
        //Remove all redundant entries from the Dictionary
        public void RemoveRedundancies()
        {
            //Create new dictionary
            Dictionary<GameEvent, List<OnEvent>> TmpListeners = new Dictionary<GameEvent, List<OnEvent>>();

            //Cycle through all dictionary entries
            foreach (KeyValuePair<GameEvent, List<OnEvent>> Item in Listeners)
            {
                //Cycle through all listener objects in list, remove null objects
                for (int i = Item.Value.Count - 1; i >= 0; i--)
                {
                    //If null, then remove item
                    if (Item.Value[i].Equals(null))
                        Item.Value.RemoveAt(i);
                }

                //If items remain in list for this notification, then add this to tmp dictionary
                if (Item.Value.Count > 0)
                    TmpListeners.Add(Item.Key, Item.Value);
            }

            //Replace listeners object with new, optimized dictionary
            Listeners = TmpListeners;
        }

        //-----------------------------------------------------------
        //Called on scene change. Clean up dictionary
        void OnLoadCallBack(Scene scene, LoadSceneMode sceneMode)
        {
            RemoveRedundancies();
        }

        void OnAwake()
        {
            SceneManager.sceneLoaded += OnLoadCallBack;
        }


        //-----------------------------------------------------------

        #endregion
    }
}
