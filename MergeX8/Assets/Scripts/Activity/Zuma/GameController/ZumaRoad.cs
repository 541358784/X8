using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ConnectLine.Logic;
using DG.Tweening;
using DragonU3DSDK.Storage;
using SomeWhere;
using UnityEngine;
using UnityEngine.Rendering;
using Zuma;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ZumaRoad:MonoBehaviour
{
    public List<Vector2Wrapper> Points => PathMap._point;
    public List<ZumaLine> Lines = new List<ZumaLine>();
    public float MoveSpeed => Game.LevelConfig.MoveSpeed;
    public float TotalMoveSpeed => Game.IsWin?0:Game.IsGuide?0:IsPerforming?RoundDistance:MoveSpeed;
    public bool Direction => MoveSpeed > 0;
    public float Radius => Game.BallRaduis;
    public ZumaPathMap PathMap;
    public Queue<int> UnShowBallQueue = new Queue<int>();
    public Transform DefaultBall;
    public ZumaGameController Game;
    public StorageZuma Storage => Game.Storage;
    public bool IsPerforming = false;
    private void Awake()
    {
        if (DefaultBall == null)
        {
            DefaultBall = transform.Find("BallGroup/ball");
        }
        DefaultBall.gameObject.SetActive(false);
    }
    public bool IsPlaying()
    {
        foreach (var line in Lines)
        {
            if (line.GetDynamicMoveSpeed() != 0)
                return true;
            foreach (var ball in line.Balls)
            {
                if (ball.State != ZumaBallState.InLine)
                    return true;
            }
        }
        return false;
    }
    private void Update()
    {
        // if (Game.IsWin)
        //     return;
        if (Lines.Count == 0)
            return;
        for (var i = 0; i < Lines.Count; i++)
        {
            var line = Lines[i];
            ZumaBall lastBall = null;
            var moveSpeed = line.MoveSpeed * Time.deltaTime;
            var deleteList = new List<List<int>>();
            for (var j = 0; j < line.Balls.Count; j++)
            {
                var curBall = line.Balls[j];
                Vector2 targetPos;
                int targetIndex;
                if (lastBall == null)
                {
                    var curBallPosition = curBall.transform.localPosition;
                    if (curBall.State != ZumaBallState.InLine)
                    {
                        curBallPosition = GetPoint(curBall.PointIndex).ToVector3();
                    }
                    targetPos = FindDistancePointNext(curBall.PointIndex, curBallPosition,
                        out targetIndex, moveSpeed);
                }
                else
                {
                    var lastBallPosition = lastBall.transform.localPosition;
                    if (lastBall.State != ZumaBallState.InLine)
                    {
                        lastBallPosition = GetPoint(lastBall.PointIndex).ToVector3();
                    }
                    targetPos = FindNext(lastBall.PointIndex, lastBallPosition, out targetIndex);
                }
                // var targetIndex = lastBall == null ? curBall.PointIndex + moveSpeed : FindNext(lastBall.PointIndex);
                if (curBall.State == ZumaBallState.InLine)
                {
                    if (lastBall != null && lastBall.State == ZumaBallState.InsertLast)
                    {
                        var pointDistance = targetIndex - curBall.PointIndex;
                        curBall.PointIndex += pointDistance/(lastBall.InsertFrameCount+1);
                        curBall.transform.localPosition = GetPoint(curBall.PointIndex).ToVector3();
                    }
                    else
                    {
                        curBall.transform.localPosition = targetPos;
                        curBall.PointIndex = targetIndex;
                    }
                }
                else if (curBall.State == ZumaBallState.InsertLast)
                {
                    Vector2 targetPosition = targetPos;
                    curBall.InsertFrameCount--;
                    if (lastBall == null)
                    {
                        if (j < line.Balls.Count - 1)
                        {
                            var nextBall = line.Balls[j + 1];
                            var nextBallPosition = nextBall.transform.localPosition;
                            if (nextBall.State != ZumaBallState.InLine)
                            {
                                nextBallPosition = GetPoint(nextBall.PointIndex).ToVector3();
                            }
                            var nextPointTargetPos = FindDistancePointNext(nextBall.PointIndex,
                                nextBallPosition, out var tempIndex, moveSpeed);
                            
                            targetPosition = FindPreviousPoint(tempIndex,nextPointTargetPos,out targetIndex);
                        }
                        var pointDistance = targetIndex - curBall.PointIndex;
                        curBall.PointIndex += pointDistance/(curBall.InsertFrameCount+1);
                    }
                    else
                    {
                        curBall.PointIndex = targetIndex;
                    }
                    if (curBall.InsertFrameCount == 0)
                    {
                        curBall.State = ZumaBallState.InLine;
                        curBall.transform.localPosition = targetPosition;
                        var tempDeleteList = line.GetDeleteList(j);
                        if (tempDeleteList != null)
                        {
                            // Debug.LogError("插入消除");
                            deleteList.Add(tempDeleteList);
                        }
                    }
                    else
                    {
                        var distance = targetPosition - (Vector2)curBall.transform.localPosition;
                        var curInsertPosition =
                            (Vector2)curBall.transform.localPosition + distance / (curBall.InsertFrameCount+1);
                        curBall.transform.localPosition = curInsertPosition;
                    }
                }
                else if (curBall.State == ZumaBallState.InsertNext)
                {
                    curBall.InsertFrameCount--;
                    var targetPosition = targetPos;
                    if (lastBall == null)
                    {
                        curBall.PointIndex = targetIndex;
                    }
                    else
                    {
                        var pointDistance = targetIndex - curBall.PointIndex;
                        curBall.PointIndex += pointDistance/(curBall.InsertFrameCount+1);
                    }
                    if (curBall.InsertFrameCount == 0)
                    {
                        curBall.State = ZumaBallState.InLine;
                        curBall.transform.localPosition = targetPosition;
                        var tempDeleteList = line.GetDeleteList(j);
                        if (tempDeleteList != null)
                        {
                            // Debug.LogError("插入消除");
                            deleteList.Add(tempDeleteList);
                        }
                    }
                    else
                    {
                        var distance = targetPosition - (Vector2)curBall.transform.localPosition;
                        var curInsertPosition =
                            (Vector2)curBall.transform.localPosition + distance / (curBall.InsertFrameCount+1);
                        curBall.transform.localPosition = curInsertPosition;
                    }
                }
                //只在线段中显示
                curBall.gameObject.SetActive(curBall.PointIndex >= 0 && curBall.PointIndex < Points.Count);
                lastBall = curBall;
                if (j == line.Balls.Count - 1)
                {
                    if (i < Lines.Count-1)
                    {
                        var nextLine = Lines[i+1];
                        var nextLineTail = nextLine.Balls.First();
                        var nextLineTailCurPosition = nextLineTail.transform.localPosition;
                        if (nextLineTail.State != ZumaBallState.InLine)
                        {
                            nextLineTailCurPosition = GetPoint(nextLineTail.PointIndex).ToVector3();
                        }
                        var nextLineTailPosition = FindDistancePointNext(nextLineTail.PointIndex, nextLineTailCurPosition,
                            out var nextLineTailIndex, nextLine.MoveSpeed * Time.deltaTime);
                        var lastBallPosition = lastBall.transform.localPosition;
                        if (lastBall.State != ZumaBallState.InLine)
                        {
                            lastBallPosition = GetPoint(lastBall.PointIndex).ToVector3();
                        }
                        var lastLineHeadNextPosition = FindNext(lastBall.PointIndex,lastBallPosition,out var lastLineHeadNextIndex);
                        if (Direction
                                ? nextLineTailIndex < lastLineHeadNextIndex
                                : nextLineTailIndex > lastLineHeadNextIndex) //接触下一条线，合并下一条线
                        {
                            foreach (var ball in nextLine.Balls)
                            {
                                ball.Line = line;
                            }
                            line.Balls.AddRange(nextLine.Balls);
                            if (nextLine.MoveSpeed != 0)
                            {
                                line.AddHitSpeed(nextLine.MoveSpeed);
                            }
                            line.NextLine = nextLine.NextLine;
                            if (line.NextLine != null)
                                line.NextLine.LastLine = line;
                            Lines.RemoveAt(i+1);
                            // Debug.LogError("合并");
                            if (line.Balls[j].Color == line.Balls[j + 1].Color)
                            {
                                var tempDeleteList = line.GetDeleteList(j);
                                if (tempDeleteList != null)
                                {
                                    // Debug.LogError("合并消除");
                                    deleteList.Add(tempDeleteList);
                                }   
                            }
                        }
                    }   
                }
            }
            if (deleteList.Count > 0)
            {
                for (var k1 = deleteList.Count - 1; k1 >= 0; k1--)
                {
                    var list = deleteList[k1];
                    for (var k2 = list.Count - 1; k2 >= 0; k2--)
                    {
                        var index = list[k2];
                        var ball = line.Balls[index];
                        line.Balls.RemoveAt(index);
                        ball.DeleteSelf();
                    }
                    var brokeIndex = list[0]-1;
                    if (brokeIndex >= 0 && brokeIndex < line.Balls.Count - 1)//断裂处生成新线
                    {
                        var newLine = new ZumaLine();
                        newLine.Road = this;
                        newLine.NextLine = line.NextLine;
                        line.NextLine = newLine;
                        newLine.LastLine = line;
                        if (newLine.NextLine != null)
                            newLine.NextLine.LastLine = newLine;
                        Lines.Insert(i+1,newLine);
                        for (var k3 = line.Balls.Count - 1; k3 > brokeIndex; k3--)
                        {
                            var ball = line.Balls[k3];
                            line.Balls.RemoveAt(k3);
                            ball.Line = newLine;
                            newLine.Balls.Insert(0,ball);
                        }
                        // Debug.LogError("断裂生成新线");
                        if (line.Balls.Last().Color == newLine.Balls.First().Color)//断裂处如果相同颜色则回撞
                        {
                            var color = line.Balls.Last().Color;
                            ZumaBall tempLastLastBall = GetBallAtLineIndex(line,line.Balls.Count-2);
                            ZumaBall tempNextNextBall = GetBallAtLineIndex(newLine,1);
                            if ((tempLastLastBall != null && tempLastLastBall.Color == color) ||
                                (tempNextNextBall != null && tempNextNextBall.Color == color))
                                newLine.AddTriggerChainSpeed();
                            // Debug.LogError("新线回撞");
                        }
                    }
                    else if (brokeIndex < 0)
                    {
                        if (line.LastLine == null)
                        {
                            // Debug.LogError("临时补线,防止浮根");
                            var newLine = new ZumaLine();
                            newLine.Road = this;
                            Lines.Insert(0,newLine);
                            var curPointIndex = 0;
                            var ball = Instantiate(DefaultBall, DefaultBall.parent).gameObject.AddComponent<ZumaBall>();
                            ball.Game = Game;
                            ball.SetSize();
                            ball.gameObject.SetActive(true);
                            ball.transform.localPosition = GetPoint(curPointIndex).ToVector3();
                            ball.PointIndex = curPointIndex;
                            ball.State = ZumaBallState.InLine;
                            ball.Road = this;
                            ball.Line = newLine;
                            ball.Color = PopColorFromQueue();
                            newLine.Balls.Add(ball);
                            line.LastLine = newLine;
                            newLine.NextLine = line;
                            i++;
                        }

                        if (line.Balls.Count > 0)
                        {
                            if (line.LastLine.Balls.Last().Color == line.Balls.First().Color)
                            {
                                var color = line.Balls.First().Color;
                                ZumaBall tempLastLastBall = GetBallAtLineIndex(line.LastLine,line.LastLine.Balls.Count-2);
                                ZumaBall tempNextNextBall = GetBallAtLineIndex(line,1);
                                if ((tempLastLastBall != null && tempLastLastBall.Color == color) ||
                                    (tempNextNextBall != null && tempNextNextBall.Color == color))
                                    line.AddTriggerChainSpeed();
                            }   
                        }
                    }
                    else if (brokeIndex >= line.Balls.Count - 1)
                    {
                        if (line.Balls.Count > 0)
                        {
                            if (line.NextLine != null)
                            {
                                if (line.NextLine.Balls.First().Color == line.Balls.Last().Color)
                                {
                                    var color = line.Balls.Last().Color;
                                    ZumaBall tempLastLastBall = GetBallAtLineIndex(line,line.Balls.Count-2);
                                    ZumaBall tempNextNextBall = GetBallAtLineIndex(line.NextLine,1);
                                    if ((tempLastLastBall != null && tempLastLastBall.Color == color) ||
                                        (tempNextNextBall != null && tempNextNextBall.Color == color))
                                        line.NextLine.AddTriggerChainSpeed();
                                }
                            }   
                        }
                    }
                    if (line.Balls.Count == 0)
                    {
                        ZumaBall tempLastBall = null;
                        ZumaBall tempNextBall = null;
                        if (line.LastLine != null)
                        {
                            line.LastLine.NextLine = line.NextLine;
                            tempLastBall = line.LastLine.Balls.Last();
                        }
                        if (line.NextLine != null)
                        {
                            line.NextLine.LastLine = line.LastLine;
                            tempNextBall = line.NextLine.Balls.First();
                        }
                        if (tempLastBall != null && tempNextBall != null)
                        {
                            if (tempLastBall.Color == tempNextBall.Color)
                            {
                                var color = tempLastBall.Color;
                                ZumaBall tempLastLastBall = GetBallAtLineIndex(line.LastLine,line.LastLine.Balls.Count-2);
                                ZumaBall tempNextNextBall = GetBallAtLineIndex(line.NextLine,1);
                                if ((tempLastLastBall != null && tempLastLastBall.Color == color) ||
                                    (tempNextNextBall != null && tempNextNextBall.Color == color))
                                    line.NextLine.AddTriggerChainSpeed();
                            }
                        }
                        Lines.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        if (Lines.Count == 0)
        {
            // Debug.LogError("清场");
            var newLine = new ZumaLine();
            newLine.Road = this;
            Lines.Add(newLine);
            var curPointIndex = 0;
            var ball = Instantiate(DefaultBall, DefaultBall.parent).gameObject.AddComponent<ZumaBall>();
            ball.Game = Game;
            ball.SetSize();
            ball.gameObject.SetActive(true);
            ball.transform.localPosition = GetPoint(curPointIndex).ToVector3();
            ball.PointIndex = curPointIndex;
            ball.State = ZumaBallState.InLine;
            ball.Road = this;
            ball.Line = newLine;
            ball.Color = PopColorFromQueue();
            newLine.Balls.Add(ball);
        }
        //补球
        var firstLine = Lines.First();
        var firstBall = firstLine.Balls.First();
        var firstBallPosition = firstBall.transform.localPosition;
        if (firstBall.State != ZumaBallState.InLine)
        {
            firstBallPosition = GetPoint(firstBall.PointIndex).ToVector3();
        }
        var previousPosition =
            FindPreviousPoint(firstBall.PointIndex, firstBallPosition, out var previousPoint);
        while (previousPoint > 0)
        {
            // Debug.LogError("补球");
            var ball = Instantiate(DefaultBall, DefaultBall.parent).gameObject.AddComponent<ZumaBall>();
            ball.Game = Game;
            ball.SetSize();
            ball.gameObject.SetActive(true);
            ball.transform.localPosition = previousPosition;
            ball.PointIndex = previousPoint;
            ball.State = ZumaBallState.InLine;
            ball.Road = this;
            ball.Line = firstLine;
            ball.Color = PopColorFromQueue();
            firstLine.Balls.Insert(0,ball);
            firstBall = ball;
            firstBallPosition = firstBall.transform.localPosition;
            if (firstBall.State != ZumaBallState.InLine)
            {
                firstBallPosition = GetPoint(firstBall.PointIndex).ToVector3();
            }
            previousPosition = FindPreviousPoint(firstBall.PointIndex, firstBallPosition, out previousPoint);
        }
        
        //删球
        var lastLine = Lines.Last();
        var tailBall = lastLine.Balls.Last();
        var spacePointCount = 10;
        while (tailBall.PointIndex >= Points.Count+spacePointCount)
        {
            lastLine.Balls.RemoveAt(lastLine.Balls.Count-1);
            tailBall.OutOfGame();
            if (lastLine.Balls.Count == 0)
            {
                if (lastLine.LastLine != null)
                {
                    lastLine.LastLine.NextLine = null;
                }
                Lines.RemoveAt(Lines.Count-1);
                lastLine = Lines.Last();
            }
            tailBall = lastLine.Balls.Last();
        }
        
        
        
        if (!Game.LevelConfig.IsLoopLevel && Storage.LevelScore >= Game.LevelConfig.WinScore)
        {
            GameWin();
            return;
        }

        // var tailBall = Lines.First().Balls.First();
        // var headBall = Lines.Last().Balls.Last();
        // var headNextPosition = FindNext(headBall.PointIndex, headBall.transform.localPosition, out var headNextIndex);
        // if ((Direction?
        //         (headNextIndex - tailBall.PointIndex):
        //         (tailBall.PointIndex - headNextIndex)) > Points.Count)
        // {
        //     GameFailed();
        // }
    }

    public ZumaBall GetBallAtLineIndex(ZumaLine line,int index)
    {
        if (index < 0)
        {
            if (line.LastLine != null)
            {
                return GetBallAtLineIndex(line.LastLine, line.LastLine.Balls.Count + index);
            }
            else
            {
                return null;
            }
        }
        if (index >= line.Balls.Count)
        {
            if (line.NextLine != null)
            {
                return GetBallAtLineIndex(line.NextLine, index - line.Balls.Count);
            }
            else
            {
                return null;
            }
        }
        return line.Balls[index];
    }


    public Vector2 FindDistancePointPrevious(int index, Vector2 rootPoint, out int newIndex,float distance)
    {
        if (distance < 0)
        {
            return FindDistancePointNext(index, rootPoint, out newIndex, -distance);
        }
        distance %= RoundDistance;
        var direction = MoveSpeed > 0;
        var step = direction ? -1 : 1;
        var curIndex = index;
        var lastPoint = GetPoint(curIndex).ToVector2();
        var tempDistance = (lastPoint - rootPoint).GetDistance();
        if (distance <= tempDistance)
        {
            newIndex = curIndex;
            return ((Vector3)rootPoint).CalculatePointsOnLineSegment(rootPoint, lastPoint, distance);
        }
        distance -= (lastPoint - rootPoint).GetDistance();
        while (distance > 0)
        {
            curIndex += step;
            var curPoint = GetPoint(curIndex).ToVector2();
            var moveDistance = (curPoint - lastPoint).GetDistance();
            if (distance <= moveDistance)
            {
                var point = ((Vector3)lastPoint).CalculatePointsOnLineSegment(lastPoint, curPoint, distance);
                newIndex = curIndex;
                return point;
            }
            else
            {
                distance -= moveDistance;
                lastPoint = curPoint;
            }
        }
        throw new Exception("祖玛错误1");
    }
    public Vector2 FindDistancePointNext(int index, Vector2 rootPoint, out int newIndex,float distance)
    {
        var step = 1;
        if (distance < 0)
        {
            step = -1;
            distance = -distance;
        }
        distance %= RoundDistance;
        var curIndex = index;
        var lastPoint = GetPoint(curIndex).ToVector2();
        var tempDistance = (lastPoint - rootPoint).GetDistance();
        if (distance <= tempDistance)
        {
            newIndex = curIndex;
            return ((Vector3)rootPoint).CalculatePointsOnLineSegment(rootPoint, lastPoint, distance);
        }
        distance -= (lastPoint - rootPoint).GetDistance();
        while (distance > 0)
        {
            curIndex += step;
            var curPoint = GetPoint(curIndex).ToVector2();
            var moveDistance = (curPoint - lastPoint).GetDistance();
            if (moveDistance > 10f)
                moveDistance = 0;
            if (distance <= moveDistance)
            {
                var point = ((Vector3)lastPoint).CalculatePointsOnLineSegment(lastPoint, curPoint, distance);
                newIndex = curIndex;
                if (step < 0)
                {
                    newIndex = curIndex - step;
                }
                return point;
            }
            else
            {
                distance -= moveDistance;
                lastPoint = curPoint;
            }
        }
        throw new Exception("祖玛错误2");
    }
    public Vector2 FindPreviousPoint(int index,Vector2 rootPoint,out int newIndex)
    {
        if (rootPoint == default)
            rootPoint = GetPoint(index).ToVector2();
        var radiusDistance = 4 * Radius * Radius;
        var direction = MoveSpeed > 0;
        var step = direction ? -1 : 1;
        var curIndex = index;
        var targetIndex = -1;
        while (targetIndex < 0)
        {
            var curPoint = GetPoint(curIndex);
            var distance = rootPoint - curPoint.ToVector2();
            var absDistance = distance.x * distance.x + distance.y * distance.y;
            if (absDistance >= radiusDistance)
            {
                targetIndex = curIndex;
                break;
            }
            curIndex += step;
            // curIndex = UnitPointIndex(curIndex);
        }
        newIndex = targetIndex - step;
        var pointA = GetPoint(targetIndex).ToVector3();
        var pointB = GetPoint(targetIndex - step).ToVector3();
        if ((pointA - pointB).magnitude > 10f)
        {
            var leftDistance = Mathf.Sqrt(radiusDistance) - (rootPoint - (Vector2)pointB).magnitude;
            return FindDistancePointNext(targetIndex, pointA, out newIndex, -leftDistance);
        }
        var point = ((Vector3)rootPoint).CalculatePointsOnLineSegment(pointA, pointB, 2*Radius);
        return point;
    }

    public Vector2 FindNext(int index,Vector2 rootPoint,out int newIndex)
    {
        if (rootPoint == default)
            rootPoint = GetPoint(index).ToVector2();
        var radiusDistance = 4 * Radius * Radius;
        var direction = MoveSpeed > 0;
        var step = direction ? 1 : -1;
        var curIndex = index;
        var targetIndex = -1;
        while (targetIndex < 0)
        {
            var curPoint = GetPoint(curIndex);
            var distance = rootPoint - curPoint.ToVector2();
            var absDistance = distance.x * distance.x + distance.y * distance.y;
            if (absDistance >= radiusDistance)
            {
                targetIndex = curIndex;
                break;
            }
            curIndex += step;
            // curIndex = UnitPointIndex(curIndex);
        }
        newIndex = targetIndex;
        var pointA = GetPoint(targetIndex).ToVector3();
        var pointB = GetPoint(targetIndex - step).ToVector3();
        if ((pointA - pointB).magnitude > 10f)
        {
            var leftDistance = Mathf.Sqrt(radiusDistance) - (rootPoint - (Vector2)pointB).magnitude;
            return FindDistancePointNext(targetIndex, pointA, out newIndex, leftDistance);
        }
        var point = ((Vector3)rootPoint).CalculatePointsOnLineSegment(pointA, pointB, 2*Radius);
        return point;
    }

    public Vector2Wrapper GetPoint(int index)
    {
        var realIndex = UnitPointIndex(index);
        return Points[realIndex];
    }

    public int UnitPointIndex(int curIndex)
    {
        var value = curIndex % Points.Count;
        if (value < 0)
            value += Points.Count;
        return value;
    }

    public void GameWin()
    {
        // Debug.LogError("获胜");
        Game.OnCheckWin();
    }

    public float RoundDistance;
    public void Reset()
    {
        IsPerforming = true;
        DOVirtual.DelayedCall(0.7f, () =>
        {
            IsPerforming = false;
            if (Game.LevelConfig.Id == 1 && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaHit) && Storage.BallCount > 0)
            {
                Game.StartLevel1Guide();
            }
            if (Game.LevelConfig.Id >= 2 && !GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaChangeBall) && Storage.BallCount > 1)
            {
                Game.StartLevel2Guide();
            }
        }).SetTarget(transform);
        foreach (var line in Lines)
        {
            line.Reset();
        }
        Lines = new List<ZumaLine>();
        var newLine = new ZumaLine();
        newLine.Road = this;
        Lines.Add(newLine);
        UnShowBallQueue.Clear();
        var curPointIndex = 0;
        FillQueue(-1);
        var ball = Instantiate(DefaultBall, DefaultBall.parent).gameObject.AddComponent<ZumaBall>();
        ball.Game = Game;
        ball.SetSize();
        ball.gameObject.SetActive(true);
        ball.transform.localPosition = GetPoint(curPointIndex).ToVector3();
        ball.PointIndex = curPointIndex;
        ball.State = ZumaBallState.InLine;
        ball.Road = this;
        ball.Line = newLine;
        ball.Color = PopColorFromQueue();
        newLine.Balls.Add(ball);

        RoundDistance = 0;
        for (var i = 0; i < Points.Count-1; i++)
        {
            var a = Points[i].ToVector2();
            var b = Points[i + 1].ToVector2();
            var distance = (b - a).GetDistance();
            RoundDistance += distance;
        }
    }

    public void FillQueue(int lastColor)
    {
        var colorList = Storage.GetLeftColors();
        var level = ZumaModel.Instance.GetLevel(Storage.LevelId);
        var tempColorWeight = level.ColorWeight.DeepCopy();
        for (var i = 0; i < colorList.Count; i++)
        {
            if (colorList[i] == lastColor)
            {
                colorList.RemoveAt(i);
                tempColorWeight.RemoveAt(i);
                break;
            }
        }
        var colorIndex = tempColorWeight.RandomIndexByWeight();
        var color = colorList[colorIndex];
        var groupCount = level.GroupWeight.RandomIndexByWeight()+1;
        for (var i = 0; i < groupCount; i++)
        {
            UnShowBallQueue.Enqueue(color);
        }
    }

    public ZumaBallColor PopColorFromQueue()
    {
        var color = UnShowBallQueue.Dequeue();
        if (UnShowBallQueue.Count == 0)
        {
            FillQueue(color);
        }
        var showColor = Storage.GetColor(color);
        return showColor;
    }
}