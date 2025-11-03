using UnityEngine;
using System.Collections;
using Framework;

public class TimeScheduler
{
    public delegate void Task();

    public static Coroutine Schedule(float delay, Task task)
    {
        return CoroutineManager.Instance.StartCoroutine(DoTask(task, delay));
    }

    public static void Stop(Coroutine coroutineTask)
    {
        CoroutineManager.Instance.StopCoroutine(coroutineTask);
    }

    private static IEnumerator DoTask(Task task, float delay)
    {
        yield return new WaitForSeconds(delay);
        task();
    }
}