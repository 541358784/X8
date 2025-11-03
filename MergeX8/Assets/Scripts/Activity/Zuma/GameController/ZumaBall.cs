using System;
using DG.Tweening;
using DragonPlus;
using UnityEngine;

public enum ZumaBallState
{
    None=0,
    InLine = 1,
    InsertNext=2,
    InsertLast=2,
    Flying=3,
}

public enum ZumaBallColor
{
    Bomb = -2,
    Grey=-1,
    Yellow=0,
    Red=1,
    Purple=2,
    Green=3,
    Blue=4,
}
public class ZumaBall:MonoBehaviour
{
    private CircleCollider2D _collider;
    public CircleCollider2D Collider
    {
        get
        {
            if (_collider == null)
                _collider = transform.GetComponent<CircleCollider2D>();
            return _collider;
        }
    }

    public ZumaBallColor _color = ZumaBallColor.Grey;
    public Transform BallModel;
    private const float BallModelRadius = 50f;
    public ZumaBallColor Color
    {
        get
        {
            return _color;
        }
        set
        {
            if (_color == value)
                return;
            _color = value;
            if (BallModel != null)
            {
                Game.Pool.RecycleGameObject(BallModel.gameObject);
                BallModel = null;
            }

            var basePath = "Prefabs/Activity/Zuma/";
            if (_color == ZumaBallColor.Red)
            {
                basePath += "BallRed";
            }
            else if (_color == ZumaBallColor.Yellow)
            {
                basePath += "BallYellow";
            }
            else if (_color == ZumaBallColor.Purple)
            {
                basePath += "BallPurple";
            }
            else if (_color == ZumaBallColor.Green)
            {
                basePath += "BallGreen";
            }
            else if (_color == ZumaBallColor.Blue)
            {
                basePath += "BallBlue";
            }
            else if (_color == ZumaBallColor.Bomb)
            {
                XLayer.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
                basePath += "BallBomb";
            }

            var scale = Game.BallRaduis/BallModelRadius;
            BallModel = Game.Pool.SpawnGameObject(basePath).transform;
            BallModel.localScale = new Vector3(scale, scale, scale);
            BallModel.SetParent(XLayer,false);
        }
    }

    public ZumaGameController Game;
    public ZumaRoad Road;
    public ZumaLine Line;
    public ZumaBallState State;
    public int InsertFrameCount;
    public int InitInsertFrameCount => Game.LevelConfig.BallInsertFrameCount;
    public float FlySpeed => Game.LevelConfig.BallFlySpeed;
    public int PointIndex;
    public Vector2 FlyDirection;
    private Vector2 LastPosition;
    private float LastRotationX;
    private Transform ZLayer;
    private Transform XLayer;
    private void Awake()
    {
        LastPosition = transform.localPosition;
        ZLayer = transform.Find("ZLayer");
        XLayer = transform.Find("ZLayer/YLayer");
    }

    public void SetSize()
    {
        Collider.radius = Game.BallRaduis;
    }

    // public ZumaGameController Controller;
    
    public void StartFly(Vector3 pointPosition)
    {
        Collider.enabled = true;
        State = ZumaBallState.Flying;
        var curPosition = transform.position;
        curPosition.z = 0;
        var pointerDistance = pointPosition - curPosition;
        FlyDirection = pointerDistance.GetUnit();
        Game.FlyingBallList.Add(this);
        AudioManager.Instance.PlaySoundById(191);
    }

    private void Update()
    {
        // if (Game.IsWin)
        //     return;
        Vector2 curPosition = transform.localPosition;
        if (State == ZumaBallState.Flying)
        {
            var moveDistance = FlySpeed * Time.deltaTime * FlyDirection;
            transform.localPosition += (Vector3)moveDistance;
            Physics2D.Simulate(Time.deltaTime);
            foreach (var road in Game.RoadList)
            {
                foreach (var line in road.Lines)
                {
                    foreach (var ball in line.Balls)
                    {
                        if (Collider.IsCirclesOverlappingCircle(ball.Collider))
                        {
                            OnTrigger(ball.Collider);
                            return;
                        }   
                    }
                }
            }
            foreach (var wall in Game.WallList)
            {
                if (Collider.IsCircleOverlapping(wall))
                {
                    OnTrigger(wall);
                    return;
                }
            }
            if (Color!= ZumaBallColor.Bomb)
            {
                curPosition = transform.localPosition;
                Vector3 displacement = curPosition - LastPosition;
                // 计算移动距离
                float distance = displacement.magnitude;
                if (distance > 0)
                {
                    // 计算旋转的弧度并转换为角度
                    float angle = (distance / Game.BallRaduis) * Mathf.Rad2Deg;
                    LastRotationX -= angle;
                    // 调整rotation.z以匹配运动方向
                    // float zRotationAngle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
                    // ZLayer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, zRotationAngle+90));
                    XLayer.transform.localRotation = Quaternion.Euler(new Vector3(LastRotationX,0,0));
                }   
            }
        }
        else if (State == ZumaBallState.None)
        {
            if (Color != ZumaBallColor.Bomb)
            {
                LastRotationX += Time.deltaTime * -360f;
                XLayer.transform.localRotation = Quaternion.Euler(new Vector3(LastRotationX,0,0));
            }
        }
        else if (State == ZumaBallState.InLine ||
                 State == ZumaBallState.InsertLast ||
                 State == ZumaBallState.InsertNext)
        {
            Vector3 displacement = curPosition - LastPosition;
            // 计算移动距离
            float distance = displacement.magnitude;
            if (distance > 0)
            {
                // 计算旋转的弧度并转换为角度
                float angle = (distance / Road.Radius) * Mathf.Rad2Deg;
                LastRotationX -= angle;
                // 调整rotation.z以匹配运动方向
                float zRotationAngle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
                ZLayer.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, zRotationAngle+90));
                XLayer.transform.localRotation = Quaternion.Euler(new Vector3(LastRotationX,0,0));
            }
        }
        LastPosition = curPosition;
    }

    public void BreakSelf()//撞墙
    {
        Game.FlyingBallList.Remove(this);
        if (BallModel != null)
        {
            Game.Pool.RecycleGameObject(BallModel.gameObject);
            BallModel = null;
        }
        DestroyImmediate(gameObject);
    }
    public void OutOfGame()//撞墙
    {
        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.ZumaTip))
            Game.StartTipGuide();
        if (BallModel != null)
        {
            Game.Pool.RecycleGameObject(BallModel.gameObject);
            BallModel = null;
        }
        DestroyImmediate(gameObject);
    }

    public void DeleteSelf()//消除
    {
        Game.IsGuide = false;
        var mainUIView = UIZumaMainController.Instance;
        if (Color != ZumaBallColor.Bomb)
        {
            AudioManager.Instance.PlaySoundById(192);
            Game.Storage.AddScore(ZumaModel.Instance.GlobalConfig.EachBallScore, "BreakBall", true);
            if (mainUIView != null)
            {
                mainUIView.PerformAddScore(gameObject.transform.position);
            }
        }
        else
        {
            AudioManager.Instance.PlaySoundById(194);
        }
        if (BallModel != null)
        {
            Game.Pool.RecycleGameObject(BallModel.gameObject);
            BallModel = null;
        }
        
        if (mainUIView != null)
        {
            var basePath = "Prefabs/Activity/Zuma/";
            if (_color == ZumaBallColor.Red)
            {
                basePath += "FX_explode_red";
            }
            else if (_color == ZumaBallColor.Yellow)
            {
                basePath += "FX_explode_yellow";
            }
            else if (_color == ZumaBallColor.Purple)
            {
                basePath += "FX_explode_purple";
            }
            else if (_color == ZumaBallColor.Green)
            {
                basePath += "FX_explode_green";
            }
            else if (_color == ZumaBallColor.Blue)
            {
                basePath += "FX_explode_blue";
            }
            else if (_color == ZumaBallColor.Bomb)
            {
                basePath += "FX_explode_bomb";
            }

            var effect = Game.Pool.SpawnGameObject(basePath);
            effect.transform.SetParent(mainUIView.EffectLayer,false);
            effect.transform.position = gameObject.transform.position;
            effect.SetActive(false);
            effect.SetActive(true);
            DOVirtual.DelayedCall(1f, () =>
            {
                Game.Pool.RecycleGameObject(effect);
            }).SetTarget(effect.transform);
        }
        DestroyImmediate(gameObject);
    }
    public void OnTrigger(Collider2D other)
    {
        var otherBall = other.gameObject.GetComponent<ZumaBall>();
        if (otherBall == null)
        {
            BreakSelf();
            return;
        }
        if (otherBall.Road == null)
            return;
        Game.FlyingBallList.Remove(this);
        InsertFrameCount = InitInsertFrameCount;
        otherBall.Line.InsertBall(this, otherBall);
    }
}