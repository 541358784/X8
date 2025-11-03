using System.Collections.Generic;

public class SimpleEnumerable<T>
{
    List<T> list;
    T[] array;
    int size;

    public int Count
    {
        get
        {
            if (list != null)
            {
                return list.Count;
            }
            else if (array != null)
            {
                return array.Length;
            }
            else
            {
                return size;
            }
        }
    }

    public T this[int index]
    {
        get
        {
            if (list != null)
            {
                return list[index];
            }
            else if (array != null)
            {
                return array[index];
            }
            else if (index < size)
            {
                return default;
            }
            else
            {
                throw new System.ArgumentOutOfRangeException();
            }
        }
    }

    public void SetData(List<T> data)
    {
        list = data;
        array = null;
        size = 0;
    }

    public void SetData(T[] data)
    {
        list = null;
        array = data;
        size = 0;
    }

    public void SetData(int data)
    {
        list = null;
        array = null;
        size = data;
    }
}