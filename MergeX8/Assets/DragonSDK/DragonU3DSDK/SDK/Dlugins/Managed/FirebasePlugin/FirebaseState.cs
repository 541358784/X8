using System;

namespace Dlugin
{
    public class FirebaseState
    {

        public bool Initialized { set; get; }
        private static FirebaseState instance = null;
        private static readonly object syslock = new object();

        public static FirebaseState Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syslock)
                    {
                        if (instance == null)
                        {
                            instance = new FirebaseState();
                        }
                    }
                }
                return instance;
            }
        }

        private FirebaseState()
        {
            Initialized = false;
        }

    }
}