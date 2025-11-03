using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using DG.Tweening;
using System.IO.Compression;
using DragonPlus;
using DragonPlus.Config.FishEatFishInner;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using OnePath.View;

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
    public class FishGame : MonoBehaviour
    {
        [SerializeField] Camera gameCamera;
        [SerializeField] SpriteRenderer background;
        [SerializeField] Player player;
        [SerializeField] Transform enemyNode;
        [SerializeField] LosePanel losePanel;

        const float FIX_HEIGHT = 1366f; //默认分辨率最大尺寸
        const float PPU = 100f; //默认PPU为100，即100个像素为1个Unity单位

        const float MAX_INTER = 1.5f; //敌人上下两条鱼之间最大间隔

        int screenWidth;
        int screenHeight;
        int maxLineIdx = 0; //关卡最大列数
        float bgScale = 1f; //背景缩放系数
        float cameraHeight = 1f; //相机高度
        float cameraWidth = 1f; //相机宽度
        float cameraInitX = 0f; //相机初始x坐标
        float cameraEndX = 0f;

        float middlePos = 1.8f; //默认左右两边鱼离中间的距离

        // float cameraPosX = 0f; //相机当前的x坐标
        // float playerHeight = 0.5f;  //标准大小的鱼的高度
        // float levelScale = 1f; //关卡整体缩放大小
        float enemyLineX = 0f; //当前正在摆放的敌人位置x坐标
        bool enableMove = false;
        bool isDrag = false;
        bool isFirstGame = true;
        int levelID;
        float firstFishY = 0f;

        Dictionary<int, List<Enemy>> enemyDic = new Dictionary<int, List<Enemy>>();
        Dictionary<int, float> enemyHeightDic = new Dictionary<int, float>();
        Dictionary<string, GameObject> enemyPrefabDic = new Dictionary<string, GameObject>();

        Boss boss;
        ArrowLine arrowLine;
        List<FishEatFishInnerPlayerSize> fish_size_data;

        static FishGame m_instance;

        public static FishGame CreateGame(Transform parent_node = null)
        {
            if (m_instance == null)
            {
                if (parent_node == null)
                {
                    m_instance =
                        Instantiate(
                                ResourcesManager.Instance.LoadResource<GameObject>("FishEatFish/FishDynamic/Prefabs/UI/FishGame"))
                            .GetComponent<FishGame>();
                }
                else
                {
                    m_instance =
                        Instantiate(
                                ResourcesManager.Instance.LoadResource<GameObject>("FishEatFish/FishDynamic/Prefabs/UI/FishGame"),
                                parent_node)
                            .GetComponent<FishGame>();
                }
            }

            return m_instance;
        }

        void Update()
        {
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

                    arrowLine.Reset();
                    isDrag = false;
                }
            }
        }

        void AttackEnemy(Enemy enemy)
        {
            //获取敌人的坐标
            //冲向敌人，
            //播放特效
            //判断敌人和玩家谁的hp大，
            Vector3 enemy_pos = enemy.transform.position;
            isDrag = false;
            if (enemy_pos.x < player.transform.position.x)
            {
                return;
            }

            if (enemy.GetType() == typeof(Bubble))
            {
                player.GoEnemy(enemy_pos,
                    () =>
                    {
                        AudioManager.Instance.PlaySound("sfx_fig_bubble");
                        enemy.gameObject.SetActive(false);
                        EffectManager.Instance.PlayEffect("FX_SoftPortal", enemyNode,
                            enemy.transform.localPosition + new Vector3(0, 0, -2f));
                    }, () =>
                    {
                        // enemy.gameObject.SetActive(false);
                        EffectManager.Instance.PlayEffect("FX_SlotUnlock", player.transform, new Vector3(0, 0, 0),
                            false); //播放特效
                        player.Bubble((enemy as Bubble).Multi, () =>
                        {
                            //镜头移动
                            if (gameCamera.transform.localPosition.x < cameraEndX &&
                                enemy_pos.x > gameCamera.transform.position.x)
                            {
                                gameCamera.transform.DOMoveX(enemy_pos.x + middlePos, 1f).SetEase(Ease.InOutSine)
                                    .OnComplete(CheckGameEnd);
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

            player.GoEnemy(enemy_pos,
                () =>
                {
                    AudioManager.Instance.PlaySound("sfx_fig_eat");
                    enemy.gameObject.SetActive(false);
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
                            if (gameCamera.transform.localPosition.x < cameraEndX &&
                                enemy_pos.x > gameCamera.transform.position.x)
                            {
                                gameCamera.transform.DOMoveX(Mathf.Min(enemy_pos.x + middlePos, cameraEndX), 1f)
                                    .SetEase(Ease.InOutSine).OnComplete(CheckGameEnd);
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
                            // enableMove = true;
                            RestartGame();
                        };
                        // losePanel.OnEnableMove += OnEnableMove;
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

        public void CheckGameEnd()
        {
            if (boss.gameObject.activeSelf)
            {
                enableMove = true;
                return;
            }
            UIManager.Instance.GetOpenedUIByPath<UIFishEatFishMainController>(UINameConst.UIFishEatFishMain).SetFinishLevel();
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
            //重置变量
            enemyLineX = 0f;
            enableMove = false;
            maxLineIdx = 0;
            enemyDic = new Dictionary<int, List<Enemy>>();
            enemyHeightDic = new Dictionary<int, float>();
            enemyPrefabDic = new Dictionary<string, GameObject>();
            //删除enemyNode下的所有内容
            foreach (Transform child in enemyNode)
            {
                child.DOKill();
                Destroy(child.gameObject);
            }
        }


        void GameStart()
        {
            gameObject.SetActive(true);
            DOTween.Sequence().SetTarget(transform)
                .AppendInterval(0.5f)
                .Append(gameCamera.transform.DOLocalMoveX(cameraInitX, (cameraEndX - cameraInitX) / 5f)
                    .SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    arrowLine = GameObject
                        .Instantiate(
                            ResourcesManager.Instance.LoadResource<GameObject>(
                                "FishEatFish/FishDynamic/Prefabs/Items/ArrowLine"), transform)
                        .GetComponent<ArrowLine>();
                    arrowLine.Init(Screen.height / FIX_HEIGHT, PPU, player.transform, gameCamera);
                    if (isFirstGame)
                    {
                        arrowLine.ShowTip(new Vector3(cameraInitX + middlePos, firstFishY, 0));
                    }

                    enableMove = true;
                });
        }

        private int BiLevelId;
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
            middlePos = cameraWidth / 4f;
            Vector2 bg_size = background.sprite.bounds.size;
            bgScale = cameraHeight / bg_size.y;
            background.transform.localScale = new Vector3(bgScale, bgScale, 1f);
            gameCamera.orthographicSize = cameraHeight / 2f;
            cameraInitX = cameraWidth / 2f;
            // cameraPosX = cameraInitX;
            player.transform.localPosition = new Vector3(cameraInitX - middlePos, 0f, 0f);
            player.OriginPos = player.transform.localPosition;
            ImportFishSizeData();
            ImportLevelData(level_id);
            PlaceEnemies(); //摆放敌人
            // enemyNode.localPosition = new Vector3(cameraPosX + middlePos,0,1f);
            cameraEndX = enemyLineX - middlePos;
            background.size = new Vector2((cameraEndX + cameraWidth / 2f) / bgScale, bg_size.y); //todo：bg_size要改成对应
            gameCamera.transform.localPosition = new Vector3(cameraEndX, 0f, -4f);

            //初始化完成，等待start消息，这里直接开始
            GameStart();
        }

        void PlaceEnemies()
        {
            enemyLineX = cameraInitX + middlePos;
            bool get_Y = false;
            int min_hp = -1;
            for (int i = 0; i <= maxLineIdx; i++)
            {
                if (enemyDic.ContainsKey(i))
                {
                    List<Enemy> line_list = enemyDic[i];
                    float height = enemyHeightDic[i];
                    int count = line_list.Count;
                    //todo： 下面算法未考虑id为空的情况
                    if (count >= 1) //不会出现count=0的情况
                    {
                        float space = count == 1
                            ? 0f
                            : Mathf.Min((gameCamera.orthographicSize * 2f - height) / (count - 1), MAX_INTER);
                        height += space * (count - 1);
                        float start_pos = -height / 2f;
                        for (int j = 0; j < count; j++)
                        {
                            float fish_height = line_list[j].GetBoxSize().y;
                            line_list[j].transform.localPosition =
                                new Vector3(enemyLineX, start_pos + fish_height / 2, 0f);
                            start_pos += fish_height + space;
                        }

                        if (!get_Y)
                        {
                            for (int k = 0; k < count; k++)
                            {
                                // if (line_list[k].GetType() != typeof(Enemy))
                                // {
                                //     continue;
                                // }

                                if (min_hp < 0 || min_hp > line_list[k].HP)
                                {
                                    min_hp = line_list[k].HP;
                                    firstFishY = line_list[k].transform.position.y;
                                }
                            }

                            get_Y = true;
                        }
                    }
                }
                else
                {
                    //todo：隔一个空格摆放
                }

                enemyLineX += middlePos * 2f;
            }

            boss.transform.localPosition = new Vector3(enemyLineX, 0f, 0f);
        }

        enum EnemyType
        {
            None = 0,
            Normal = 1,
            Bubble = 2,
        }

        void ImportLevelData(int level_id)
        {
            var levelConfig = FishEatFishInnerConfigManager.Instance.GetConfig<FishEatFishInnerLevel>()
                .Find((a) => a.id == level_id);

            if (levelConfig == null)
            {
                throw new System.Exception("Incorrect Level ID!");
            }

            player.HP = levelConfig.hp;
            // player.Size = (float)level_data["player"]["size"];
            FishChangeSize();

            var enemy_data = levelConfig.enemyList;
            for (int i = 0; i < enemy_data.Length; i++)
            {
                var enemyConfig = FishEatFishInnerConfigManager.Instance.GetConfig<FishEatFishInnerEnemy>()
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

                int _line = enemyConfig.line;
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
                if (enemyDic.ContainsKey(_line))
                {
                    enemyDic[_line].Add(fish);
                    enemyHeightDic[_line] += fish.GetBoxSize().y;
                }
                else
                {
                    enemyDic.Add(_line, new List<Enemy>() {fish});
                    enemyHeightDic[_line] = fish.GetBoxSize().y;
                }

                maxLineIdx = Mathf.Max(maxLineIdx, _line);
            }

            var boss_id = levelConfig.bossId;
            var bossConfig = FishEatFishInnerConfigManager.Instance.GetConfig<FishEatFishInnerBoss>()
                .Find(a => a.id == boss_id);
            boss = GameObject
                .Instantiate(
                    ResourcesManager.Instance.LoadResource<GameObject>(
                        $"FishEatFish/FishDynamic/Prefabs/Enemy/{bossConfig.path}"), enemyNode)
                .GetComponent<Boss>();
            boss.HP = bossConfig.hp;
            boss.Size = bossConfig.size;
        }

        void ImportFishSizeData()
        {
            fish_size_data = FishEatFishInnerConfigManager.Instance.GetConfig<FishEatFishInnerPlayerSize>();
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