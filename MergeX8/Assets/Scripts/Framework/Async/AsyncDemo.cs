/*-------------------------------------------------------------------------------------------
// Copyright (C) 2021 北京，天龙互娱
//
// 模块名：AsyncDemo
// 创建日期：2021-9-9
// 创建者：jun.zhao
// 模块描述：异步Demo
//-------------------------------------------------------------------------------------------*/

using UnityEngine;
using Framework.Async;
using System.Threading;

public class AsyncDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.RunOnAsyncThread(() =>
        {
            while (true)
            {
                Thread.Sleep(1000);
                this.RunOnMainThread(() => { Debug.LogError("Test1 Async -> Sync !!!"); });
            }
        });

        Async.RunOnMainThread(() =>
        {
            while (true)
            {
                Thread.Sleep(1005);
                Async.RunOnAsyncThread(() => { Debug.LogError("Test2 Async -> Sync !!!"); });
            }
        });
    }
}