using System.Collections.Generic;

public class ClassObjectPool
{
    Stack<object> pool = new Stack<object>();
    System.Func<object> createMethod;
    int capacity;

    public ClassObjectPool(int capacity = 5, System.Func<object> createMethod = null)
    {
        this.capacity = capacity;
        this.createMethod = createMethod;
    }

    public void Push(object o)
    {
        if (pool.Count < capacity)
        {
            pool.Push(o);
        }
    }

    public object Pop()
    {
        if (pool.Count > 0)
        {
            return pool.Pop();
        }
        else if (createMethod != null)
        {
            return createMethod();
        }
        else
        {
            return null;
        }
    }

    public void Clear()
    {
        pool.Clear();
    }
}