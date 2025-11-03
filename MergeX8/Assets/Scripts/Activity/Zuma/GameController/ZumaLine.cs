using System;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class ZumaLine
{
    public ZumaRoad Road;
    public List<ZumaBall> Balls = new List<ZumaBall>();
    public ZumaLine LastLine;
    public ZumaLine NextLine;
    public float StaticMoveSpeed => LastLine==null ? Road.TotalMoveSpeed : 0;

    public float GetDynamicMoveSpeed()
    {
        return GetTriggerChainSpeed() + GetHitSpeed();
    }
    public float MoveSpeed => StaticMoveSpeed + GetDynamicMoveSpeed();
    private int TriggerChainFrame = -1;
    public void AddTriggerChainSpeed()
    {
        TriggerChainFrame = Time.frameCount;
    }

    public float GetTriggerChainSpeed()
    {
        if (TriggerChainFrame < 0)
            return 0;
        var cross= Time.frameCount - TriggerChainFrame;
        if (cross == 0)
            return 0.01f;
        return (Road.Game.LevelConfig.HitBackSpeed * (cross) * -Road.MoveSpeed);
    }
    private List<int> HitFrameList = new List<int>();
    private List<float> HitSpeedList = new List<float>();
    private int HitFrame => Road.Game.LevelConfig.HitSpeedDisappearFrameCount;
    public void AddHitSpeed(float hitSpeed)
    {
        HitFrameList.Add(Time.frameCount);
        HitSpeedList.Add(hitSpeed);
    }

    public float GetHitSpeed()
    {
        var hitSpeed = 0f;
        var curFrameCount = Time.frameCount;
        for (var i = 0; i < HitFrameList.Count; i++)
        {
            var baseHitSpeed = HitSpeedList[i];
            var cross= curFrameCount - HitFrameList[i];
            var left = HitFrame - cross;
            if (left <= 0)
            {
                HitFrameList.RemoveAt(i);
                HitSpeedList.RemoveAt(i);
                i--;
                continue;
            }
            hitSpeed += (baseHitSpeed * ((float)left / HitFrame));
        }
        return hitSpeed;
    }
    
    const int DeleteLeastCount = 3;
    public List<int> GetDeleteList(int index)
    {
        var ball = Balls[index];
        if (ball.State != ZumaBallState.InLine)
            return null;
        if (ball.Color == ZumaBallColor.Bomb)
        {
            var bombList = new List<int>();
            var startIndex = Math.Max(index - 2,0);
            var endIndex = Math.Min(index + 2, Balls.Count-1);
            for (var i = startIndex; i <= endIndex;i++)
            {
                bombList.Add(i);
            }
            return bombList;
        }
        var checkColor = ball.Color;
        var deleteList = new List<int>(){index};
        for (var i = index-1; i >= 0; i--)
        {
            var tempBall = Balls[i];
            if (tempBall.State != ZumaBallState.InLine)
                break;
            if (tempBall.Color != checkColor)
            {
                break;
            }
            deleteList.Insert(0,i);
        }

        for (var i = index + 1; i < Balls.Count; i++)
        {
            var tempBall = Balls[i];
            if (tempBall.State != ZumaBallState.InLine)
                break;
            if (tempBall.Color != checkColor)
            {
                break;
            }
            deleteList.Add(i);
        }
        
        if (deleteList.Count >= DeleteLeastCount)
            return deleteList;
        return null;
    }
    public void Reset()
    {
        foreach (var ball in Balls)
        {
            GameObject.DestroyImmediate(ball.gameObject);
        }
        Balls = new List<ZumaBall>();
    }
    
    public void InsertBall(ZumaBall insertBall,ZumaBall triggerBall)
    {
        for (var i = 0; i < Balls.Count; i++)
        {
            if (Balls[i] == triggerBall)
            {
                insertBall.Road = Road;
                insertBall.Line = this;
                insertBall.transform.SetParent(Road.DefaultBall.parent,true);
                insertBall.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                // var lastBallPointIndex = Road.FindPreviousPoint(triggerBall.PointIndex);
                // var lastBallPosition = Road.GetPoint(lastBallPointIndex).ToVector3();
                var triggerBallPosition = triggerBall.transform.localPosition;
                if (triggerBall.State != ZumaBallState.InLine)
                {
                    triggerBallPosition = Road.GetPoint(triggerBall.PointIndex).ToVector3();
                }
                var lastBallPosition = Road.FindPreviousPoint(triggerBall.PointIndex, triggerBallPosition,
                    out var lastBallPointIndex);
                // var nextBallPointIndex = Road.FindNext(triggerBall.PointIndex);
                // var nextBallPosition = Road.GetPoint(nextBallPointIndex).ToVector3();
                var nextBallPosition = Road.FindNext(triggerBall.PointIndex, triggerBallPosition,
                    out var nextBallPointIndex);
                var lastBallDistance = (UnityEngine.Vector2)insertBall.transform.localPosition - lastBallPosition;
                var lastBallDistanceAbs =
                    lastBallDistance.x * lastBallDistance.x + lastBallDistance.y * lastBallDistance.y;
                var nextBallDistance = (UnityEngine.Vector2)insertBall.transform.localPosition - nextBallPosition;
                var nextBallDistanceAbs =
                    nextBallDistance.x * nextBallDistance.x + nextBallDistance.y * nextBallDistance.y;
                if (lastBallDistanceAbs > nextBallDistanceAbs)
                {
                    Balls.Insert(i+1,insertBall);
                    insertBall.PointIndex = triggerBall.PointIndex;
                    insertBall.State = ZumaBallState.InsertNext;
                    // Debug.LogError("前方插入");
                }
                else
                {
                    Balls.Insert(i,insertBall);
                    insertBall.PointIndex = triggerBall.PointIndex;
                    insertBall.State = ZumaBallState.InsertLast;
                    // Debug.LogError("后方插入");
                }
                return;
            }
        }
    }

    public int GetBaseBallIndex()
    {
        var curLine = this;
        var index = 0;
        while (curLine.LastLine != null)
        {
            index += curLine.LastLine.Balls.Count;
            curLine = curLine.LastLine;
        }
        return index;
    }
}