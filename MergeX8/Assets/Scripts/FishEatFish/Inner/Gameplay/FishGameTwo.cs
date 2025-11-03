using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System.IO.Compression;
using DragonPlus;
using DragonPlus.Config.FishEatFishInner;
using DragonPlus.Config.FishEatFishInnerTwo;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using OnePath.View;
using Random = UnityEngine.Random;

/*初始化步骤：-
- 设定背景图的高度（height为自身图片的高度/10,比例缩放到1536）
- 设定相机的最大size，1536f除以默认PPU
- 设定相机的最小size，最大size/2
- 读取配置表，加载玩家属性，计算最大的高度
- 依次加载每一列的鱼
*/

/* 游戏开始步骤：
1. 相机从终点移动到初始点
2. 播放提示
3. 提示消失，等待玩家操作
*/
namespace FishEatFishSpace
{
    public class FishGameTwo : MonoBehaviour
    {
        [SerializeField] Camera gameCamera;
        [SerializeField] SpriteRenderer background;
        [SerializeField] Player player;
        [SerializeField] Transform enemyNode;

        const float FIX_HEIGHT = 1366f; //默认分辨率最大尺寸
        const float PPU = 100f; //默认PPU为100，即100个像素为1个Unity单位

        public int screenWidth;
        public int screenHeight;
        public float bgScale = 1f; //背景缩放系数
        public float cameraHeight = 1f; //相机高度
        public float cameraWidth = 1f; //相机宽度

        // float cameraPosX = 0f; //相机当前的x坐标
        // float playerHeight = 0.5f;  //标准大小的鱼的高度
        // float levelScale = 1f; //关卡整体缩放大小
        public bool enableMove = false;
        bool isFirstGame = true;
        // public Enemy SelectEnemy;
        bool isDrag = false;
        ArrowLine arrowLine;
        public int levelID;
        public List<FishEatFishInnerTwoEnemy> EnemyConfigList = new List<FishEatFishInnerTwoEnemy>();
        public List<Enemy> enemyDic = new List<Enemy>();
        Dictionary<string, GameObject> enemyPrefabDic = new Dictionary<string, GameObject>();
        
        public List<FishEatFishInnerTwoPlayerSize> fish_size_data;

        static FishGameTwo m_instance;

        public static FishGameTwo CreateGame(Transform parent_node)
        {
            if (m_instance == null)
            {
                m_instance =
                    Instantiate(
                            ResourcesManager.Instance.LoadResource<GameObject>("FishEatFish/FishDynamic/Prefabs/UI/FishGameTwo"),
                            parent_node)
                        .GetComponent<FishGameTwo>();
            }
            return m_instance;
        }

        public float EnemyMoveAreaTop = 3f;
        public float EnemyMoveAreaBottom = 3f;
        public float EnemyMoveAreaLeft = 1f;
        public float EnemyMoveAreaRight = 1f;
        public float MaxX => cameraWidth / 2 * gameCamera.orthographicSize/CameraSizeMulti - EnemyMoveAreaRight;
        public float MinX => -cameraWidth / 2 * gameCamera.orthographicSize/CameraSizeMulti + EnemyMoveAreaLeft;
        public float MaxY => cameraHeight / 2 * gameCamera.orthographicSize/CameraSizeMulti - EnemyMoveAreaTop;
        public float MinY => -cameraHeight / 2 * gameCamera.orthographicSize/CameraSizeMulti + EnemyMoveAreaBottom;
        
        void UpdateEnemyPosition(float deltaTime)
        {
            foreach (var enemy in enemyDic)
            {
                var moveDistance = enemy.Speed * deltaTime;
                var pos = enemy.transform.localPosition + moveDistance;
                if (pos.x > MaxX)
                {
                    if (enemy.Speed.x > 0)
                    {
                        pos.x -= 2 * (pos.x - MaxX);
                        var tempSpeed = enemy.Speed;
                        tempSpeed.x *= -1;
                        enemy.Speed = tempSpeed;
                    }
                }
                if (pos.y > MaxY)
                {
                    if (enemy.Speed.y > 0)
                    {
                        pos.y -= 2 * (pos.y - MaxY);
                        var tempSpeed = enemy.Speed;
                        tempSpeed.y *= -1;
                        enemy.Speed = tempSpeed;
                    }
                }
                if (pos.x < MinX)
                {
                    if (enemy.Speed.x < 0)
                    {
                        pos.x += 2 * (MinX - pos.x);
                        var tempSpeed = enemy.Speed;
                        tempSpeed.x *= -1;
                        enemy.Speed = tempSpeed;
                    }
                }
                if (pos.y < MinY)
                {
                    if (enemy.Speed.y < 0)
                    {
                        pos.y += 2 * (MinY - pos.y);
                        var tempSpeed = enemy.Speed;
                        tempSpeed.y *= -1;
                        enemy.Speed = tempSpeed;
                    }
                }
                enemy.transform.localPosition = pos;
            }
        }
        void Update()
        {
            UpdateEnemyPosition(Time.deltaTime);
            if (!enableMove)
            {
                return;
            }
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                Touch touch = new Touch();
                touch.fingerId = 0; // 用于标识触摸点的唯一 ID
                touch.position = Input.mousePosition;
                touch.phase = TouchPhase.Began;
                HandleTouchEvent(touch);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Touch touch = new Touch();
                touch.fingerId = 0; // 用于标识触摸点的唯一 ID
                touch.position = Input.mousePosition;
                touch.phase = TouchPhase.Ended;
                HandleTouchEvent(touch);
            }
            else if (Input.GetMouseButton(0))
            {
                Touch touch = new Touch();
                touch.fingerId = 0; // 用于标识触摸点的唯一 ID
                touch.position = Input.mousePosition;
                touch.phase = TouchPhase.Moved;
                HandleTouchEvent(touch);
            }
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                HandleTouchEvent(touch);
            }
#endif
        }

        private void OnDestroy()
        {
            gameCamera.transform.DOKill(false);
            player.transform.DOKill(false);
            transform.DOKill(false);
            var notice = UIManager.Instance.GetOpenedUIByPath<UIPopupNotice1Controller>(UINameConst.UINotice1);
            if (notice)
                notice.CloseWindowWithinUIMgr(true);
            var failPopup = UIManager.Instance.GetOpenedUIByPath<UIFishEatFishFailController>(UINameConst.UIFishEatFishFail);
            if (failPopup)
                failPopup.CloseWindowWithinUIMgr(true);
        }

        public void HandleTouchEvent(Touch touch)
        {
            if (UIManager.Instance.GetOpenedUIByPath<UIPopupNotice1Controller>(UINameConst.UINotice1))
            {
                // Debug.LogError("弹窗打开时忽略点击");
                return;
            }
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 touch_pos = new Vector3(touch.position.x, touch.position.y, 0);
                Vector3 world_pos = gameCamera.ScreenToWorldPoint(touch_pos);
                Ray ray = new Ray(world_pos, Vector3.forward);
                // Debug.DrawRay(world_pos,Vector3.forward*8f,Color.yellow,1f);
                if (isFirstGame)
                {
                    arrowLine.HideTip();
                    guideArrow.Release();
                    isFirstGame = false;
                }

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 8f, LayerMask.GetMask("Fish"), QueryTriggerInteraction.Collide))
                {
                    string tag = hit.collider.gameObject.tag;
                    if (tag == "Player") //点中鱼
                    {
                        isDrag = true;
                        // EffectManager.Instance.PlayEffect("Bang",player.transform,new Vector3(0,0,0));
                        arrowLine.StartDraw();
                    }
                    else if (tag == "Enemy")
                    {
                        enableMove = false;
                        {
                            isDrag = false;
                            arrowLine.Reset();
                        }
                        AttackEnemy(hit.collider.GetComponent<Enemy>());
                    }
                }
            }

            if (isDrag)
            {
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    arrowLine.Draw(touch.position);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isDrag = false;
                    arrowLine.Reset();
                    Ray ray = gameCamera.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, 8f, LayerMask.GetMask("Fish"),
                            QueryTriggerInteraction.Collide))
                    {
                        if (hit.collider.tag == "Enemy")
                        {
                            enableMove = false;
                            AttackEnemy(hit.collider.GetComponent<Enemy>());
                        }
                    }
                }
            }
            // if (touch.phase == TouchPhase.Began)
            // {
            //     Vector3 touch_pos = new Vector3(touch.position.x, touch.position.y, 0);
            //     Vector3 world_pos = gameCamera.ScreenToWorldPoint(touch_pos);
            //     Ray ray = new Ray(world_pos, Vector3.forward);
            //     // Debug.DrawRay(world_pos,Vector3.forward*8f,Color.yellow,1f);
            //     // if (isFirstGame)
            //     // {
            //     //     isFirstGame = false;
            //     // }
            //
            //     RaycastHit hit;
            //     if (Physics.Raycast(ray, out hit, 8f, LayerMask.GetMask("Fish"), QueryTriggerInteraction.Collide))
            //     {
            //         string tag = hit.collider.gameObject.tag;
            //         if (tag == "Enemy")
            //         {
            //             SelectEnemy = hit.collider.GetComponent<Enemy>();
            //         }
            //     }
            // }
            //
            // if (SelectEnemy != null)
            // {
            //     if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            //     {
            //     }
            //     else if (touch.phase == TouchPhase.Ended)
            //     {
            //         Ray ray = gameCamera.ScreenPointToRay(touch.position);
            //         RaycastHit hit;
            //         if (Physics.Raycast(ray, out hit, 8f, LayerMask.GetMask("Fish"),
            //                 QueryTriggerInteraction.Collide))
            //         {
            //             if (hit.collider.tag == "Enemy")
            //             {
            //                 if (SelectEnemy == hit.collider.GetComponent<Enemy>())
            //                 {
            //                     enableMove = false;
            //                     AttackEnemy(SelectEnemy);
            //                 }
            //             }
            //         }
            //         SelectEnemy = null;
            //     }
            // }
        }
        
        public float CameraSizeMulti => cameraHeight / 2f;
        public float TargetCameraSize => Math.Min(CameraMaxSize,player.Size * CameraSizeMulti);
        public float CameraMaxSize => 2*CameraSizeMulti;
        void AttackEnemy(Enemy enemy)
        {
            if (enemy.GetType() == typeof(Bubble))
            {
                player.GoEnemyTwo(enemy,
                    () =>
                    {
                        AudioManager.Instance.PlaySound("sfx_fig_bubble");
                        enemyDic.Remove(enemy);
                        enemy.gameObject.SetActive(false);
                        XUtility.WaitSeconds(0.2f,()=>
                        {
                            if (!this) return;
                            Destroy(enemy.gameObject);
                        });
                        EffectManager.Instance.PlayEffect("FX_SoftPortal", enemyNode,
                            enemy.transform.localPosition + new Vector3(0, 0, -2f));
                    }, () =>
                    {
                        // enemy.gameObject.SetActive(false);
                        EffectManager.Instance.PlayEffect("FX_SlotUnlock", player.transform, new Vector3(0, 0, 0),
                            false); //播放特效
                        player.Bubble((enemy as Bubble).Multi, () =>
                        {
                            if (gameCamera.orthographicSize < TargetCameraSize)
                            {
                                gameCamera.DOOrthoSize(TargetCameraSize, 1f).SetEase(Ease.Linear)
                                    .OnComplete(()=>
                                    {
                                        arrowLine.Init(Screen.height / FIX_HEIGHT / player.Size, PPU, player.transform, gameCamera);
                                        CheckGameEnd();
                                    });
                            }
                            else
                            {
                                CheckGameEnd();
                            }
                            //todo: 角色放大
                        });
                    });
                return;
            }

            player.GoEnemyTwo(enemy,
                () =>
                {
                    AudioManager.Instance.PlaySound("sfx_fig_eat");
                    enemyDic.Remove(enemy);
                    enemy.gameObject.SetActive(false);
                    XUtility.WaitSeconds(0.2f,()=>
                    {
                        if (!this) return;
                        Destroy(enemy.gameObject);
                    });
                    EffectManager.Instance.PlayEffect("FX_laugh", enemyNode,
                        enemy.transform.localPosition + new Vector3(0, 0, -2f));
                }, () =>
                {
                    if (player.HP >= enemy.HP)
                    {
                        // enemy.gameObject.SetActive(false); //隐藏敌人
                        EffectManager.Instance.PlayEffect("FX_SlotUnlock", player.transform, new Vector3(0, 0, 0),
                            false); //播放特效
                        //角色放大
                        FishChangeSize();
                        player.Win(enemy.HP, () =>
                        {
                            //镜头移动
                            if (gameCamera.orthographicSize < TargetCameraSize)
                            {
                                gameCamera.DOOrthoSize(TargetCameraSize, 1f).SetEase(Ease.Linear)
                                    .OnComplete(()=>
                                    {
                                        arrowLine.Init(Screen.height / FIX_HEIGHT / player.Size, PPU, player.transform, gameCamera);
                                        CheckGameEnd();
                                    });
                            }
                            else
                            {
                                CheckGameEnd();
                            }
                        });
                        
                    }
                    else
                    {
                        //失败
                        System.Action OnEnableMove = () =>
                        {
                            RestartGame();
                        };
                        player.Lose(() =>
                        {
                            AudioManager.Instance.PlaySound("sfx_fig_defeat");
                            UIFishEatFishFailController.Open(OnEnableMove);
                            // CommonUtils.OpenCommon1ConfirmWindow(new NoticeUIData
                            // {
                            //     DescString = LocalizationManager.Instance.GetLocalizedString("UI_POPUP_FAILED_Title"),
                            //     OKButtonText = LocalizationManager.Instance.GetLocalizedString("UI_button_retry"),
                            //     CancelButtonText = LocalizationManager.Instance.GetLocalizedString("UI_button_exit"),
                            //     sortingOrder = 200,
                            //     OKCallback = OnEnableMove,
                            //     CancelCallback = () => {
                            //         TMatch.UILoadingEnter.Open(() =>
                            //         {
                            //             GameBIManager.Instance.SendGameEvent(
                            //                 BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameFishLevelend,
                            //                 data1:BiLevelId.ToString(),data2:"1");
                            //             EventDispatcher.Instance.DispatchEventImmediately(EventEnum.FinishFishEatFishGame);
                            //         }); 
                            //     },
                            //     HasCancelButton = true,
                            //     NoTweenClose = true,
                            //     HasCloseButton = false
                            // });
                        });
                    }
                });
        }

        public async void CheckGameEnd()
        {
            if (CreateEnemy() != null)//创建出新的敌人则游戏继续
            {
                enableMove = true;
                return;
            }
            if (enemyDic.Count > 0)//有存量敌人则游戏继续
            {
                enableMove = true;
                return;
            }
            //无剩余敌人则游戏结束
            if (gameCamera.orthographicSize < CameraMaxSize)
            {
                gameCamera.DOOrthoSize(CameraMaxSize, 0.5f);
                await XUtility.WaitSeconds(0.5f);
                if (!this) return;
            }
            var moveTime = 2f;
            var boxLocalPosition =
                player.transform.parent.InverseTransformPoint(background.transform.Find("box").position);
            var playerMoveY = player.transform.localPosition.y - boxLocalPosition.y;

            {
                var distance = boxLocalPosition - player.transform.localPosition;
                distance.z = 0;
                player.ChangeDirection(distance);
            }
            boxLocalPosition.z = player.transform.localPosition.z;
            player.transform.DOLocalMove(boxLocalPosition, moveTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                EffectManager.Instance.PlayEffect("FX_SlotUnlock", player.transform, new Vector3(0, 0, 0),
                    false); //播放特效
                player.OpenBox();
                background.transform.Find("Box").GetComponent<Animator>().PlayAnimation("open");
                XUtility.WaitSeconds(1.5f, () =>
                {
                    if (!this) return;
                    UIManager.Instance.GetOpenedUIByPath<UIFishEatFishMainController>(UINameConst.UIFishEatFishMain).SetFinishLevel();
                });
            });
            var cameraMoveY = background.transform.localScale.y * background.sprite.bounds.size.y / 2 -
                              gameCamera.orthographicSize;
            
            gameCamera.transform.DOLocalMoveY(-cameraMoveY, (playerMoveY>cameraMoveY?cameraMoveY/playerMoveY:playerMoveY/cameraMoveY)  * moveTime).SetEase(Ease.Linear);
        }
        void RestartGame()
        {
            player.Reset();
            ResetParam();
            //todo： 删除不必要的加载
            Initial(levelID,BiLevelId);
        }

        void ResetParam()
        {
            enableMove = false;
            EnemyConfigList = new List<FishEatFishInnerTwoEnemy>();
            enemyDic = new List<Enemy>();
            enemyPrefabDic = new Dictionary<string, GameObject>();
            //删除enemyNode下的所有内容
            foreach (Transform child in enemyNode)
            {
                child.DOKill();
                Destroy(child.gameObject);
            }
        }

        public FishGuideArrow guideArrow;
        void GameStart()
        {
            gameObject.SetActive(true);
            
            arrowLine = GameObject
                .Instantiate(
                    ResourcesManager.Instance.LoadResource<GameObject>(
                        "FishEatFish/FishDynamic/Prefabs/Items/ArrowLine"), transform)
                .GetComponent<ArrowLine>();
            arrowLine.Init(Screen.height / FIX_HEIGHT / player.Size, PPU, player.transform, gameCamera);
            if (isFirstGame)
            {
                Enemy guideEnemy = null;
                foreach (var enemy in enemyDic)
                {
                    if (enemy.GetType() != typeof(Bubble) && enemy.HP <= player.HP)
                    {
                        guideEnemy = enemy;
                        break;
                    }
                }

                if (guideEnemy == null)
                {
                    foreach (var enemy in enemyDic)
                    {
                        if (enemy.GetType() == typeof(Bubble))
                        {
                            guideEnemy = enemy;
                            break;
                        }
                    }
                }

                if (guideEnemy != null)
                {
                    guideArrow = FishGuideArrow.CreateGuide(guideEnemy,transform);
                }
            }
            enableMove = true;
        }

        public int BiLevelId;
        public void Initial(int level_id,int bi_id) //传入关卡id，以1开始
        {
            BiLevelId = bi_id;
            ResetParam();
            levelID = level_id;
            //获取设备分辨率
            screenWidth = Screen.width;
            screenHeight = Screen.height;

            cameraHeight = FIX_HEIGHT / PPU;
            cameraWidth = cameraHeight * screenWidth / screenHeight;
            bgScale = cameraWidth / background.sprite.bounds.size.x;
            background.transform.localScale = new Vector3(bgScale * CameraMaxSize/CameraSizeMulti, bgScale * CameraMaxSize/CameraSizeMulti, 1f);
            // cameraPosX = cameraInitX;
            player.transform.localPosition = new Vector3(0f, 0f, 0f);
            player.OriginPos = player.transform.localPosition;
            // player.playerBox.enabled = false;
            gameCamera.orthographicSize = player.Size * CameraSizeMulti;
            background.transform.localPosition = new Vector3(0, 0, 4f);
            gameCamera.transform.localPosition = new Vector3(0, 0f, -4f);
            ImportFishSizeData();
            ImportLevelData(level_id);

            //初始化完成，等待start消息，这里直接开始
            GameStart();
        }

        enum EnemyType
        {
            None = 0,
            Normal = 1,
            Bubble = 2,
        }

        public int CurEnemyIndex;
        void ImportLevelData(int level_id)
        {
            var levelConfig = FishEatFishInnerTwoConfigManager.Instance.GetConfig<FishEatFishInnerTwoLevel>()
                .Find((a) => a.id == level_id);

            if (levelConfig == null)
            {
                throw new System.Exception("Incorrect Level ID!");
            }

            player.HP = levelConfig.hp;
            FishChangeSize();
            gameCamera.orthographicSize = player.Size * CameraSizeMulti;
            var enemy_data = levelConfig.enemyList;
            for (int i = 0; i < enemy_data.Length; i++)
            {
                var enemyConfig = FishEatFishInnerTwoConfigManager.Instance.GetConfig<FishEatFishInnerTwoEnemy>()
                    .Find(a => a.id == enemy_data[i]);
                string path = $"FishEatFish/FishDynamic/Prefabs/Enemy/{enemyConfig.path}";
                if (enemyConfig.type == (int) EnemyType.Bubble)
                {
                    path = "FishEatFish/FishDynamic/Prefabs/Items/bubble";
                }
                else if (enemyConfig.type == (int) EnemyType.None)
                {
                    path = "FishEatFish/FishDynamic/Prefabs/Items/empty";
                }
                if (!enemyPrefabDic.ContainsKey(enemyConfig.path))
                {
                    enemyPrefabDic.Add(enemyConfig.path, ResourcesManager.Instance.LoadResource<GameObject>(path));
                }
                EnemyConfigList.Add(enemyConfig);
            }
            CurEnemyIndex = 0;
            for (var i = 0; i < levelConfig.enemyCount; i++)
            {
                CreateEnemy(false);
            }
        }

        public enum EnemyPositionDirectionType
        {
            LeftBottom,
            RightBottom,
            LeftTop,
            RightTop,
        }
        public enum EnemyMoveDirection
        {
            XPositive,
            XNegative,
            YPositive,
            YNegative,
        }
        public EnemyPositionDirectionType GetPositionType(Vector3 pos)
        {
            if (pos.x < 0 && pos.y < 0)
            {
                return EnemyPositionDirectionType.LeftBottom;
            }
            else if (pos.x >= 0 && pos.y < 0)
            {
                return EnemyPositionDirectionType.RightBottom;
            }
            else if (pos.x < 0 && pos.y >= 0)
            {
                return EnemyPositionDirectionType.LeftTop;
            }
            return EnemyPositionDirectionType.RightTop;
        }

        public float TopBottomSpace = 3f;
        public Vector3 GetEnemyInitPosition(EnemyPositionDirectionType directionType,Enemy fish)
        {
            var height =
                GetRandomHeight(
                    -(cameraHeight / 2 * gameCamera.orthographicSize / CameraSizeMulti - TopBottomSpace), 
                    (cameraHeight / 2 * gameCamera.orthographicSize/CameraSizeMulti - TopBottomSpace), fish);
            // Debug.LogError("GetRandomHeight="+height);
            if (directionType == EnemyPositionDirectionType.LeftBottom)
            {
                return new Vector3(-cameraWidth / 2 * gameCamera.orthographicSize/CameraSizeMulti, height,0);
            }
            if (directionType == EnemyPositionDirectionType.RightBottom)
            {
                return new Vector3(cameraWidth / 2 * gameCamera.orthographicSize/CameraSizeMulti, height,0);
            }
            if (directionType == EnemyPositionDirectionType.LeftTop)
            {
                return new Vector3(-cameraWidth / 2 * gameCamera.orthographicSize/CameraSizeMulti, height,0);
            }
            if (directionType == EnemyPositionDirectionType.RightTop)
            {
                return new Vector3(cameraWidth / 2 * gameCamera.orthographicSize/CameraSizeMulti, height,0);
            }
            return Vector3.zero;
        }

        public float GetRandomHeight(float bottom,float top,Enemy fish)
        {
            var areaList = GetUnUseHeightArea(new Tuple<float, float>(bottom, top),fish);
            if (areaList.Count == 0)
                return 0;
            var totalWeight = 0f;
            foreach (var tempArea in areaList)
            {
                totalWeight += (tempArea.Item2 - tempArea.Item1);
            }
            var randomWeight = Random.Range(0, totalWeight);
            var curWeight = 0f;
            foreach (var tempArea in areaList)
            {
                curWeight += (tempArea.Item2 - tempArea.Item1);
                if (curWeight > randomWeight)
                {
                    var area = tempArea;
                    var height = Random.Range(area.Item1, area.Item2);
                    return height;
                }
            }
            return 0f;
        }

        public List<Tuple<float, float>> ClipArea(List<Tuple<float, float>> list,float minY,float maxY)
        {
            var newList = new List<Tuple<float, float>>();
            for (var j = 0; j < list.Count; j++)
            {
                var area = list[j];
                if (maxY < area.Item2)
                {
                    if (minY < area.Item1)
                    {
                        if (maxY > area.Item1)
                        {
                            newList.Add(new Tuple<float, float>(maxY, area.Item2));   
                        }
                        else
                        {
                            newList.Add(area);
                        }
                    }
                    else
                    {
                        newList.Add(new Tuple<float, float>(area.Item1,minY));  
                        newList.Add(new Tuple<float, float>(maxY, area.Item2));
                    }
                }
                else
                {
                    if (minY > area.Item1)
                    {
                        if (minY < area.Item2)
                        {
                            newList.Add(new Tuple<float, float>(area.Item1, minY)); 
                        }
                        else
                        {
                            newList.Add(area);
                        }
                    }
                }
            }
            return newList;
        }

        public List<Tuple<float, float>> GetUnUseHeightArea(Tuple<float,float> initArea,Enemy fish,float sizeScale = 0.8f)
        {
            var fishHalfBoxHeight = fish.GetBoxSize().y / 2 * sizeScale;
            var list = new List<Tuple<float, float>>();
            // Debug.LogError("裁剪前"+initArea.Item1+","+initArea.Item2);
            list.Add(initArea);
            {
                var minY = player.transform.localPosition.y - player.GetBoxSize().y/2 * sizeScale - fishHalfBoxHeight;
                var maxY = player.transform.localPosition.y + player.GetBoxSize().y/2 * sizeScale + fishHalfBoxHeight;
                // Debug.LogError("player.transform.localPosition.y="+player.transform.localPosition.y);
                // Debug.LogError("minY="+minY);
                // Debug.LogError("maxY="+maxY);
                list = ClipArea(list, minY, maxY);
            }
            for (var i = 0; i < enemyDic.Count; i++)
            {
                var enemy = enemyDic[i];
                var minY = enemy.transform.localPosition.y - enemy.GetBoxSize().y/2 * sizeScale - fishHalfBoxHeight;
                var maxY = enemy.transform.localPosition.y + enemy.GetBoxSize().y/2 * sizeScale + fishHalfBoxHeight;
                // Debug.LogError("enemy.transform.localPosition.y="+enemy.transform.localPosition.y);
                // Debug.LogError("minY="+minY);
                // Debug.LogError("maxY="+maxY);
                list = ClipArea(list, minY, maxY);
            }
            // foreach (var temp in list)
            // {
            //     Debug.LogError("裁剪后"+temp.Item1+","+temp.Item2);
            // }

            if (list.Count == 0)
            {
                // Debug.LogError("未找到空间，查找间距下降为"+sizeScale * 0.75f);
                return GetUnUseHeightArea(initArea, fish, sizeScale * 0.75f);
            }
            return list;
        }
        public Enemy CreateEnemy(bool byPlayerPos = true)//是否根据玩家位置生成
        {
            if (CurEnemyIndex >= EnemyConfigList.Count)
                return null;
            var enemyConfig = EnemyConfigList[CurEnemyIndex];
            CurEnemyIndex++;
            
            Enemy fish = GameObject.Instantiate(enemyPrefabDic[enemyConfig.path], enemyNode).GetComponent<Enemy>();
            if (enemyConfig.type == (int) EnemyType.Bubble)
            {
                (fish as Bubble).Multi = enemyConfig.multiply;
            }
            else
            {
                fish.HP = enemyConfig.hp;
            }

            fish.Size = enemyConfig.size;
            if (byPlayerPos)
            {
                var directionType = GetPositionType(player.transform.localPosition);
                var oppositeType = XUtility.GetRandomEnumValue<EnemyPositionDirectionType>();
                while (oppositeType == directionType)
                {
                    oppositeType = XUtility.GetRandomEnumValue<EnemyPositionDirectionType>();
                }
                var initPos = GetEnemyInitPosition(oppositeType,fish);
                fish.transform.localPosition = initPos;
                // Debug.LogError("NewEnemyLocalPosition="+fish.transform.localPosition);
                var speedValue = Random.Range(0.5f, 1.5f);
                
                if (initPos.x > 0)
                    speedValue *= -1;
                fish.Speed = new Vector3(speedValue, 0, 0);
            }
            else
            {
                var oppositeType = XUtility.GetRandomEnumValue<EnemyPositionDirectionType>();
                var initPos = GetEnemyInitPosition(oppositeType,fish);
                fish.transform.localPosition = initPos;
                // Debug.LogError("NewEnemyLocalPosition="+fish.transform.localPosition);
                var speedValue = Random.Range(0.5f, 1.5f);
                if (initPos.x > 0)
                    speedValue *= -1;
                fish.Speed = new Vector3(speedValue, 0, 0);
            }
            enemyDic.Add(fish);
            for (var i=0;i<enemyDic.Count;i++)
            {
                var enemy = enemyDic[i];
                var tempPos = enemy.transform.localPosition;
                tempPos.z = i * 0.5f;
                enemy.transform.localPosition = tempPos;
            }
            return fish;
        }

        void ImportFishSizeData()
        {
            fish_size_data = FishEatFishInnerTwoConfigManager.Instance.GetConfig<FishEatFishInnerTwoPlayerSize>();
        }

        void FishChangeSize()
        {
            foreach (var data in fish_size_data)
            {
                if (player.HP >= data.minScore && player.HP <= data.maxScore)
                {
                    player.Size = data.size;
                    break;
                }
            }
        }
    }
}