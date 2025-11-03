using UnityEngine;
using System.Collections;
using Framework;

public class TimeSheduler
{
    public delegate void Task();

    public static void Schedule(float delay, Task task)
    {
        CoroutineManager.Instance.StartCoroutine(DoTask(task, delay));
    }

    private static IEnumerator DoTask(Task task, float delay)
    {
        yield return new WaitForSeconds(delay);
        task();
    }
}