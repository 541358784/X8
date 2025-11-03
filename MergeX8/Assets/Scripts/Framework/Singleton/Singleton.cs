public class Singleton<T> where T : new()
{
    protected static T _instance;
    private static readonly object _lockObj = new object();

    public static T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            lock (_lockObj)
                _instance = new T();

            return _instance;
        }
    }
}