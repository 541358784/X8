using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Ditch.Model;
using DragonPlus.Config.Ditch;
using DragonU3DSDK.Asset;
using Mosframe;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Debug = UnityEngine.Debug;
using Task = System.Threading.Tasks.Task;

namespace DigTrenchNew
{
    public partial class DigTrenchNewGameController:MonoBehaviour
    {
        // public DigTrenchNewPixelState[][] PixelState;

        // public Dictionary<Tuple<int, int>, DigTrenchNewPixelState> PixelStateDic =
        //     new Dictionary<Tuple<int, int>, DigTrenchNewPixelState>();
        public bool ShowStateTexture = false;
        public DigTrenchNewPixelState[][] PixelState;
        // public Dictionary<Tuple<int,int>, int> GameParamDic = new Dictionary<Tuple<int, int>, int>();

        public List<int> CheckDirectionXList = new List<int>()
        {
            0,
            0,
            -1,
            1,
        };
        public List<int> CheckDirectionYList = new List<int>()
        {
            1,
            -1,
            0,
            0,
        };
        public int Width;
        public int Height;
        public SpriteRenderer PixelStateSprite;
        public SpriteRenderer ShowSprite;
        public SpriteRenderer BackgroundSprite;
        public Camera GameCamera;
        public float CameraMaxY;
        public float CameraMinY;
        public bool IsWin;
        public bool IsGame;
        public int GameParam;
        public int GamePixelX;
        public int GamePixelY;
        public Texture2D StateTexture;
        public Texture2D TempSandTexture;

        public void SetCameraPosition(float progress)
        {
            if (CameraMaxY <= CameraMinY)
            {
                GameCamera.transform.SetLocalPositionY(0);
                return;   
            }
            GameCamera.transform.SetLocalPositionY(CameraMinY + (CameraMaxY - CameraMinY)*progress);
        }
        public UIDigTrenchNewMainController Window;

        public void Init(UIDigTrenchNewMainController controller)
        {
            Window = controller;
            ShowStateTexture = false;
            LoadPixelState();
            PlayWomanLoopPainEffect();
        }

        public void SetPixelState(int x,int y,DigTrenchNewPixelState state)
        {
            PixelState[x][y] = state;
            // var key = new Tuple<int, int>(x, y);
            // if (!PixelStateDic.ContainsKey(key))
            // {
            //     Debug.LogError("未初始化直接赋值像素状态");
            //     PixelStateDic.Add(key,state);
            // }
            // PixelStateDic[key] = state;
        }
        public DigTrenchNewPixelState GetPixelState(int x, int y)
        {
            if (x >= Width || x < 0 || y >= Height || y < 0)
                return DigTrenchNewPixelState.None;
            if (PixelState[x][y] != DigTrenchNewPixelState.Default)
                return PixelState[x][y];
            // var key = new Tuple<int, int>(x, y);
            // if (PixelStateDic.TryGetValue(key, out var state))
            //     return state;
            var mapTexture = PixelStateSprite.sprite.texture;
            Color pixelColor = mapTexture.GetPixel(x, y);
            if (pixelColor == new Color(0f, 0f, 0f, 1f))
            {
                // PixelStateDic.Add(key,DigTrenchNewPixelState.None);
                PixelState[x][y] = DigTrenchNewPixelState.None;
            }
            else if (pixelColor == new Color(1f, 1f, 1f, 1f))
            {
                // PixelStateDic.Add(key,DigTrenchNewPixelState.Cover);
                PixelState[x][y] = DigTrenchNewPixelState.Cover;
            }
            else if (pixelColor == new Color(0.0f, 0.0f, 1f, 1f))
            {
                // PixelStateDic.Add(key,DigTrenchNewPixelState.Water);
                PixelState[x][y] = DigTrenchNewPixelState.Water;
            }
            else if (pixelColor == new Color(0.0f, 1f, 0.0f, 1f))
            {
                // PixelStateDic.Add(key,DigTrenchNewPixelState.Win);
                PixelState[x][y] = DigTrenchNewPixelState.Win;
            }
            else if (pixelColor.r == 1f && pixelColor.g == 0f && pixelColor.b == 0f)
            {
                // var alpha = (int)Mathf.Round(pixelColor.a * 255f);
                // var key = new Tuple<int, int>(x, y);
                // GameParamDic.Add(key,255-alpha);
                // PixelStateDic.Add(key,DigTrenchNewPixelState.Game);
                PixelState[x][y] = DigTrenchNewPixelState.Game;
            }

            if (ShowStateTexture)
            {
                var state = PixelState[x][y];
                Color color = Color.magenta;
                if (state == DigTrenchNewPixelState.Cover)
                    color = Color.white;
                else if(state == DigTrenchNewPixelState.Water)
                    color = Color.blue;
                else if(state == DigTrenchNewPixelState.Win)
                    color = Color.green;
                else if(state == DigTrenchNewPixelState.None)
                    color = Color.black;
                else if(state == DigTrenchNewPixelState.Game)
                    color = Color.red;
                StateTexture.SetPixel(x,y,color);   
            }
            return PixelState[x][y];
        }
        public void LoadPixelState()
        {
            var mapTexture = PixelStateSprite.sprite.texture;
            Width = mapTexture.width;
            Height = mapTexture.height;
            var cameraLongSize = BackgroundSprite.sprite.texture.width / GameCamera.aspect;
            GameCamera.orthographicSize = cameraLongSize / 200;
            var freeSize = BackgroundSprite.sprite.texture.height - cameraLongSize;
            CameraMaxY = freeSize / 200;
            CameraMinY = -(freeSize / 200);
            PixelState = new DigTrenchNewPixelState[Width][];
            for (var i = 0; i < Width; i++)
            {
                PixelState[i] = new DigTrenchNewPixelState[Height];
            }

            // var otherColors = new List<Color>();
            // var threshold = 0.01f;
            // for (int x = 0; x < Width; x++)
            // {
            //     for (int y = 0; y < Height; y++)
            //     {
            //         Color pixelColor = mapTexture.GetPixel(x, y);
            //         
            //         if (pixelColor.Near(new Color(1f, 1f, 1f, 1f),threshold))
            //         {
            //             PixelState[x][y] = DigTrenchNewPixelState.Cover;
            //         }
            //         else if (pixelColor.Near(new Color(0.0f, 0.0f, 1f, 1f),threshold))
            //         {
            //             PixelState[x][y] = DigTrenchNewPixelState.Water;
            //         }
            //         else if (pixelColor.Near(new Color(0.0f, 1f, 0.0f, 1f),threshold))
            //         {
            //             PixelState[x][y] = DigTrenchNewPixelState.Win;
            //         }
            //         else if (pixelColor.Near(new Color(0.0f, 0.0f, 0.0f, 1f),threshold))
            //         {
            //             PixelState[x][y] = DigTrenchNewPixelState.None;
            //         }
            //         else if (pixelColor.Near(new Color(1.0f, 0.0f, 0.0f, 1f),threshold))
            //         {
            //             var alpha = (int)Mathf.Round(pixelColor.a * 255f);
            //             GameParamDic.Add(new Tuple<int, int>(x,y),255-alpha);
            //             PixelState[x][y] = DigTrenchNewPixelState.Game;
            //         }
            //         else
            //         {
            //             PixelState[x][y] = DigTrenchNewPixelState.Error;
            //             otherColors.Add(pixelColor);
            //         }
            //     }
            // }

            // if (otherColors.Count > 0)
            // {
            //     Debug.LogError("有杂色");
            //     foreach (var color in otherColors)
            //     {
            //         Debug.LogError(color.ToString());
            //     }
            // }
            if (ShowStateTexture)
            {
                StateTexture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
                // for (var x = 0; x < Width; x++)
                // {
                //     for (var y = 0; y < Height; y++)
                //     {
                //         var state = PixelState[x][y];
                //         Color color = Color.magenta;
                //         if (state == DigTrenchNewPixelState.Cover)
                //             color = Color.white;
                //         else if(state == DigTrenchNewPixelState.Water)
                //             color = Color.blue;
                //         else if(state == DigTrenchNewPixelState.Win)
                //             color = Color.green;
                //         else if(state == DigTrenchNewPixelState.None)
                //             color = Color.black;
                //         else if(state == DigTrenchNewPixelState.Game)
                //             color = Color.red;
                //         StateTexture.SetPixel(x,y,color);
                //     }
                // }
                StateTexture.Apply();
                gameObject.GetOrCreateComponent<SpriteRenderer>().sprite = Sprite.Create(StateTexture,
                    new Rect(0, 0, StateTexture.width, StateTexture.height), new Vector2(0.5f, 0.5f));
            }
            
            var source = ShowSprite.sprite.texture;
            TempSandTexture = new Texture2D(source.width, source.height);

            // 复制像素数据
            TempSandTexture.SetPixels(source.GetPixels());

            // 应用更改
            TempSandTexture.Apply();
            ShowSprite.sprite = Sprite.Create(TempSandTexture,
                new Rect(0, 0, TempSandTexture.width, TempSandTexture.height), new Vector2(0.5f, 0.5f));
        }
        private void Update()
        {
            if (IsWin)
                return;
            if (IsGame)
                return;
            HandleTouch();
            if (IsWin)
            {
                StopWomanLoopPainEffect();
                PlayWomanCelebrateEffect();
                Debug.LogError("赢了");
                var spineStart = transform.Find("Spine_Start")?.GetComponent<SkeletonAnimation>();
                var spineEnd = transform.Find("Spine_End")?.GetComponent<SkeletonAnimation>();
                if (spineStart)
                {
                    spineStart.AnimationState?.SetAnimation(0, "sad_happy", false);
                    spineStart.AnimationState?.AddAnimation(0, "happy", true,0);
                    spineStart.Update(0);
                }
                if (spineEnd)
                {
                    spineEnd.AnimationState?.SetAnimation(0, "sad_happy", false);
                    spineEnd.AnimationState?.AddAnimation(0, "happy", true,0);
                    spineEnd.Update(0);
                }

                if (Window.Config.Id == 1)
                {
                    var bag = transform.Find("-6,-6/03/01");
                    bag.DOLocalMove(new Vector3(2.53f, -4.31f, 0), 1.5f);
                    bag.DOScale(new Vector3(4, 4, 1), 1.5f);
                }

                if (spineStart || spineEnd)
                {
                    XUtility.WaitSeconds(3f, () =>
                    {
                        Window.OnWin();
                    });
                }
                else
                {
                    Window.OnWin();   
                }
            }
            else if (IsGame)
            {
                Window.DealGameLogic(GameParam, (b) =>
                {
                    IsGame = false;
                    if (b)
                    {
                        GameParam++;
                        transform.Find("obs"+GameParam).gameObject.SetActive(false);
                        BreakObstacle();
                        PaintGameArea(GamePixelX,GamePixelY);
                        ShowSprite.sprite.texture.Apply();
                    }
                });
            }
        }

        public void HandleTouch()
        {
            if (Input.GetMouseButtonDown(0)) // 鼠标左键摁下
            {
                // if (!EventSystem.current.IsPointerOverGameObject())
                {
                    OnTouchBegan(Input.mousePosition);   
                }
            }
            if (Input.GetMouseButton(0)) // 鼠标左键拖拽
            {
                OnTouchMoved(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0)) // 鼠标左键抬起
            {
                OnTouchEnded(Input.mousePosition);
            }

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            OnTouchBegan(touch.position);   
                        }
                        break;
                    case TouchPhase.Moved:
                        OnTouchMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                        OnTouchEnded(touch.position);
                        break;
                    case TouchPhase.Canceled:
                        OnTouchEnded(touch.position);
                        break;
                }
            }
        }
        private bool IsStartDrag;
        public void OnTouchBegan(Vector3 screenPosition)
        {
            RemoveAllHandGuide();
            IsStartDrag = true;
            Vector3 worldPosition = GameCamera.ScreenToWorldPoint(screenPosition);
            DigAtPosition(worldPosition);
        }
        public void OnTouchMoved(Vector3 screenPosition)
        {
            if (!IsStartDrag)
                return;
            Vector3 worldPosition = GameCamera.ScreenToWorldPoint(screenPosition);
            DigAtPosition(worldPosition);
        }
        public void OnTouchEnded(Vector3 screenPosition)
        {
            IsStartDrag = false;
            Vector3 worldPosition = GameCamera.ScreenToWorldPoint(screenPosition);
            DigAtPosition(worldPosition);
        }
        public void DigAtPosition(Vector3 worldPosition)
        {
            bool showDigEffect = false;
            PixelStateSprite.GetPixelAtWorldPosition(worldPosition,out var touchPixelPosX,out var touchPixelPosY);
            GetCheckArea(touchPixelPosX,touchPixelPosY,out var checkAreaX,out var checkAreaY);
            var waterXPixels = new Queue<int>();
            var waterYPixels = new Queue<int>();
            for (var i = 0; i < checkAreaX.Count; i++)
            {
                var state = GetPixelState(checkAreaX[i], checkAreaY[i]);
                if (state == DigTrenchNewPixelState.Water)
                {
                    ShowSprite.sprite.texture.SetPixel(checkAreaX[i],checkAreaY[i],Color.clear);//覆盖层对应像素设置为透明
                    waterXPixels.Enqueue(checkAreaX[i]);
                    waterYPixels.Enqueue(checkAreaY[i]);
                }
            }

            while (waterXPixels.Count > 0)
            {
                var waterXPixel = waterXPixels.Dequeue();
                var waterYPixel = waterYPixels.Dequeue();
                for (var i = 0; i < CheckDirectionXList.Count; i++)
                {
                    var nextPixelX = waterXPixel + CheckDirectionXList[i];
                    var nextPixelY = waterYPixel + CheckDirectionYList[i];
                    var distance2 = (nextPixelX - touchPixelPosX) * (nextPixelX - touchPixelPosX) +
                                    (nextPixelY - touchPixelPosY) * (nextPixelY - touchPixelPosY);
                    if (distance2 > radius2)
                    {
                        continue;
                    }
                    var state = GetPixelState(nextPixelX, nextPixelY);
                    if (state == DigTrenchNewPixelState.Cover)
                    {
                        // PixelState[nextPixel.x][nextPixel.y] = DigTrenchNewPixelState.Water;
                        SetPixelState(nextPixelX,nextPixelY,DigTrenchNewPixelState.Water);
                        ShowSprite.sprite.texture.SetPixel(nextPixelX,nextPixelY,Color.clear);//覆盖层对应像素设置为透明
                        waterXPixels.Enqueue(nextPixelX);
                        waterYPixels.Enqueue(nextPixelY);
                        showDigEffect = true;
                    }
                    else if (state == DigTrenchNewPixelState.Win && !IsWin)
                    {
                        IsWin = true;
                        PaintWinArea(nextPixelX,nextPixelY);
                    }
                    else if (state == DigTrenchNewPixelState.Game && !IsGame)
                    {
                        IsGame = true;
                        GamePixelX = nextPixelX;
                        GamePixelY = nextPixelY;
                        // GameParam = GameParamDic[new Tuple<int, int>(nextPixelX, nextPixelY)];
                    }
                }
            }
            ShowSprite.sprite.texture.Apply();
            if (ShowStateTexture)
            {
                StateTexture.Apply();
            }

            if (showDigEffect)
            {
                PlayDigEffect(worldPosition);
                PlayDigEffect();
            }
        }

        public void PaintGameArea(int winPointX,int winPointY)
        {
            // PixelState[gamePoint.x][gamePoint.y] = DigTrenchNewPixelState.Water;
            SetPixelState(winPointX,winPointY,DigTrenchNewPixelState.Water);
            ShowSprite.sprite.texture.SetPixel(winPointX,winPointY,Color.clear);//覆盖层对应像素设置为透明
            var winXPixels = new Queue<int>();
            var winYPixels = new Queue<int>();
            winXPixels.Enqueue(winPointX);
            winYPixels.Enqueue(winPointY);
            while (winXPixels.Count > 0)
            {
                var winXPixel = winXPixels.Dequeue();
                var winYPixel = winYPixels.Dequeue();

                for (var i = 0; i < CheckDirectionXList.Count; i++)
                {
                    var nextPixelX = winXPixel + CheckDirectionXList[i];
                    var nextPixelY = winYPixel + CheckDirectionYList[i];
                    var state = GetPixelState(nextPixelX, nextPixelY);
                    if (state == DigTrenchNewPixelState.Game)
                    {
                        // PixelState[nextPixel.x][nextPixel.y] = DigTrenchNewPixelState.Water;
                        SetPixelState(nextPixelX,nextPixelY,DigTrenchNewPixelState.Cover);
                        // ShowSprite.sprite.texture.SetPixel(nextPixelX,nextPixelY,Color.clear);//覆盖层对应像素设置为透明
                        winXPixels.Enqueue(nextPixelX);
                        winYPixels.Enqueue(nextPixelY);
                    }
                }
            }
        }
        public void PaintWinArea(int winPointX,int winPointY)
        {
            // PixelState[winPoint.x][winPoint.y] = DigTrenchNewPixelState.Water;
            SetPixelState(winPointX,winPointY,DigTrenchNewPixelState.Water);
            ShowSprite.sprite.texture.SetPixel(winPointX,winPointY,Color.clear);//覆盖层对应像素设置为透明
            var winXPixels = new Queue<int>();
            var winYPixels = new Queue<int>();
            winXPixels.Enqueue(winPointX);
            winYPixels.Enqueue(winPointY);
            while (winXPixels.Count > 0)
            {
                var winXPixel = winXPixels.Dequeue();
                var winYPixel = winYPixels.Dequeue();

                for (var i = 0; i < CheckDirectionXList.Count; i++)
                {
                    var nextPixelX = winXPixel + CheckDirectionXList[i];
                    var nextPixelY = winYPixel + CheckDirectionYList[i];
                    var state = GetPixelState(nextPixelX, nextPixelY);
                    if (state == DigTrenchNewPixelState.Win)
                    {
                        // PixelState[nextPixel.x][nextPixel.y] = DigTrenchNewPixelState.Water;
                        SetPixelState(nextPixelX,nextPixelY,DigTrenchNewPixelState.Water);
                        ShowSprite.sprite.texture.SetPixel(nextPixelX,nextPixelY,Color.clear);//覆盖层对应像素设置为透明
                        winXPixels.Enqueue(nextPixelX);
                        winYPixels.Enqueue(nextPixelY);
                    }
                }
            }
        }
        public static int radius = 30;
        public static int radius2 = radius * radius;
        public void GetCheckArea(int touchPixelPosX,int touchPixelPosY,out List<int> pixelXList,out List<int> pixelYList)//点击范围,圆形
        {
            var minX = touchPixelPosX - radius;
            minX = Math.Max(0, minX);
            var maxX = touchPixelPosX + radius;
            maxX = Math.Min(Width-1, maxX);
            var minY = touchPixelPosY - radius;
            minY = Math.Max(0, minY);
            var maxY = touchPixelPosY + radius;
            maxY = Math.Min(Height-1, maxY);
            pixelXList = new List<int>();
            pixelYList = new List<int>();
            for (var x = minX; x < maxX; x++)
            {
                for (var y = minY; y < maxY; y++)
                {
                    var distance2 = (x - touchPixelPosX) * (x - touchPixelPosX) +
                                    (y - touchPixelPosY) * (y - touchPixelPosY);
                    if (distance2 <= radius2)
                    {
                        pixelXList.Add(x);
                        pixelYList.Add(y);
                    }
                }
            }
        }

        public Transform GuideStart;
        public Transform GuideEnd;
        public GameObject guideHandResources;
        public List<Transform> guideHands = new List<Transform>();

        public void PlyGuide()
        {
            ShowHandGuideTo(GuideStart.position, GuideEnd.position);
        }
        public void ShowHandGuideTo(Vector3 startPos,Vector3 endPos)
        {
            if (guideHandResources == null)
                guideHandResources = ResourcesManager.Instance.LoadResource<GameObject>("DigTrenchNew/Prefabs/GuideArrow",addToCache:false);
            var handObj = GameObject.Instantiate(guideHandResources, transform).transform;
            handObj.gameObject.setLayer(27,true);
            handObj.position = startPos;
            handObj.DOMove(endPos, 2f).SetLoops(-1, LoopType.Restart);
            guideHands.Add(handObj);
        }
        
        public void RemoveAllHandGuide()
        {
            if (guideHands.Count == 0)
                return;
            foreach (var hand in guideHands)
            {
                hand.DOKill();
                GameObject.Destroy(hand.gameObject);
            }
            guideHands.Clear();
        }
        
        
        public void PlayDigEffect(Vector3 position)
        {
            // if (DigEffect)
            // {
            //     DigEffect.position = position;
            //     DigEffect.DOKill(false);
            //     DOVirtual.DelayedCall(1f, () =>
            //     {
            //         Destroy(DigEffect.gameObject);
            //     });
            //     return;
            // }
            var asset = ResourcesManager.Instance.LoadResource<GameObject>("DigTrenchNew/Prefabs/"+Window.Config.DigEffectAsset);
            var DigEffect = GameObject.Instantiate(asset,transform).transform;
            DigEffect.gameObject.setLayer(27,true);
            DigEffect.position = position;
            DOVirtual.DelayedCall(1f, () =>
            {
                Destroy(DigEffect.gameObject);
            });
        }
    }
    // #if UNITY_EDITOR
    // [CustomEditor(typeof(DigTrenchNewGameController))]
    // public class DigTrenchNewGameControllerEditor : Editor
    // {
    //     public override void OnInspectorGUI()
    //     {
    //         // 绘制默认的 Inspector UI
    //         DrawDefaultInspector();
    //
    //         // 获取目标对象
    //         DigTrenchNewGameController myComponent = (DigTrenchNewGameController)target;
    //
    //         // 添加按钮到 Inspector
    //         if (GUILayout.Button("LoadPath"))
    //         {
    //             // 调用目标对象的方法
    //             myComponent.LoadPixelState();
    //         }
    //     }
    // }
    // #endif
}