using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Threading.Tasks;
using DataInspector;
using Decoration;
using DG.Tweening;
using DigTrench.Tool;
using DigTrench.UI;
using DragonPlus;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
using Framework.Motion;
using Mosframe;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Ease = DG.Tweening.Ease;
using File = System.IO.File;

namespace DigTrench
{
    [Serializable]
    public partial class DigTrenchModel:MonoBehaviour
    {
        private const int CullingMask = 27;
        public Dictionary<int, Dictionary<int, DigTrenchTile>> TileDic;
        public DigTrenchModel()
        {
            
        }
        private Transform _transform;
        private DigTrenchGameConfig _gameConfig;
        private Tilemap _tileMap;
        private List<PropsGroup> _propsBag;
        private Transform _animationLayer;
        private Transform _staticLayer;
        private Transform _propsLayer;
        private Transform _effectLayer;
        private Transform _sideCoverLayer;
        private Transform _guideHandLayer;
        private Transform _obstacleBreakEffectLayer;
        private Vector3 _minCameraPosition;
        private Vector3 _maxCameraPosition;
        private Vector3 _screenMinPosition;
        private Vector3 _screenMaxPosition;
        private float _defaultScreenSize = 5;
        private int _levelId;
        private bool _isFirstTimePlay;
        private int _videoCount;
        private Camera _camera;
        private UIDigTrenchMainController _mainUI;
        private float _unitTileDistance;
        public List<Vector3> _unitOffsetList;
        // public Dictionary<ConnectDirection, Vector3> _unitSideDistanceList;
        public async void Init(Transform transform,bool isFirstTimePlay)
        {
            _isFirstTimePlay = isFirstTimePlay;
            if (_isFirstTimePlay)
                _videoCount = 0;
            // RuntimeAnimatorManager.Instance.InitRuntimeAnimators();
            _mainUI = UIManager.Instance.GetOpenedUIByPath(UINameConst.UIDigTrenchMain) as UIDigTrenchMainController;
            _transform = transform;
            _effectLayer = new GameObject("EffectLayer").transform;
            _effectLayer.SetParent(_transform,false);
            _sideCoverLayer = new GameObject("SideCoverLayer").transform;
            _sideCoverLayer.SetParent(_transform,false);
            _guideHandLayer = new GameObject("GuideHandLayer").transform;
            _guideHandLayer.SetParent(_transform,false);
            _obstacleBreakEffectLayer = new GameObject("ObstacleBreakEffectLayer").transform;
            _obstacleBreakEffectLayer.SetParent(_transform,false);
            _minCameraPosition = _transform.Find("MinPosition").position;
            _maxCameraPosition = _transform.Find("MaxPosition").position;
            var uiRootTransform = UIRoot.Instance.mRoot.transform as RectTransform;
            var uiCamera = UIRoot.Instance.mUICamera;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRootTransform, new Vector2(0, 0), uiCamera, out _screenMinPosition);
            RectTransformUtility.ScreenPointToWorldPointInRectangle(uiRootTransform, new Vector2(Screen.width, Screen.height), uiCamera, out _screenMaxPosition);
            
            var gameConfigMB = _transform.GetComponent<DigTrenchGameConfigMB>();
            var fileName = gameConfigMB.gameObject.name.Replace("(Clone)","") + "Config";
            var configStr = ResourcesManager.Instance.LoadResource<TextAsset>("DigTrench/Configs/"+fileName,addToCache:false);
            var config = JsonConvert.DeserializeObject<DigTrenchGameConfig>(configStr.text);
            gameConfigMB.Config = config;
            
            _levelId = gameConfigMB.gameObject.name.Replace("Level", "").Replace("(Clone)","").ToInt();
            _gameConfig = gameConfigMB.Config;
            _mainUI.BindLevelConfig(_gameConfig,_levelId,isFirstTimePlay);
            _camera = transform.Find("Camera").GetComponent<Camera>();
            _gameConfig.CameraConfig.CullingMask = CullingMask;
            _camera.cullingMask = 1<<_gameConfig.CameraConfig.CullingMask;
            _transform.gameObject.setLayer(_gameConfig.CameraConfig.CullingMask,true);
            if (CommonUtils.IsLE_16_10())
            {
                _camera.orthographicSize = _gameConfig.CameraConfig.Size * 0.7f;
            }
            else
            {
                _camera.orthographicSize = _gameConfig.CameraConfig.Size;
            }
            _camera.transform.localPosition = _gameConfig.CameraConfig.Position.ToVector3();
            if (!_camera.GetComponent<Physics2DRaycaster>())
            {
                _camera.gameObject.AddComponent<Physics2DRaycaster>();
            }
            // if (!_camera.GetComponent<EventSystem>())
            // {
            //     var eventSystem = _camera.gameObject.AddComponent<EventSystem>();
            //     eventSystem.AddComponent<StandaloneInputModule>();
            // }
            TileDic = new Dictionary<int, Dictionary<int, DigTrenchTile>>();
            _propsBag = new List<PropsGroup>();
            _animationLayer = _transform.Find("Animation");
            _staticLayer = _transform.Find("Static");
            _propsLayer = _transform.Find("Props");
            var pointHandler = _transform.Find("PointEventLayer").gameObject.AddComponent<PointerEventCustomHandler>();
            var dragHandler = _transform.Find("PointEventLayer").gameObject.AddComponent<DragDropEventCustomHandler>();
            pointHandler.BindingPointerDown(OnPointerDown);
            dragHandler.BindingDragAction(OnDrag);
            pointHandler.BindingPointerUp(OnPointerUp);
            _tileMap = _transform.Find("Sand/01").GetComponent<Tilemap>();
            _unitTileDistance = GetDistance(
                _tileMap.GetCellCenterWorld(new Vector3Int(0, 0, 0)),
                _tileMap.GetCellCenterWorld(new Vector3Int(1, 0, 0)));
            _unitOffsetList = new List<Vector3>()
            {
                _tileMap.GetCellCenterWorld(new Vector3Int(1, 1, 0)) - _tileMap.GetCellCenterWorld(new Vector3Int(0, 0, 0)),
                _tileMap.GetCellCenterWorld(new Vector3Int(1, 1, 0)) - _tileMap.GetCellCenterWorld(new Vector3Int(2, 0, 0)),
                _tileMap.GetCellCenterWorld(new Vector3Int(1, 1, 0)) - _tileMap.GetCellCenterWorld(new Vector3Int(2, 2, 0)),
                _tileMap.GetCellCenterWorld(new Vector3Int(1, 1, 0)) - _tileMap.GetCellCenterWorld(new Vector3Int(0, 2, 0)),
            };
            for (var i=0;i<_unitOffsetList.Count;i++)
            {
                _unitOffsetList[i] /= 2;
            }
            BoundsInt bounds = _tileMap.cellBounds;
            foreach (var position in bounds.allPositionsWithin)
            {
                TileBase tile = _tileMap.GetTile(position);
                if (tile != null)
                {
                    var newTile = GetOrCreateTile(position);
                    newTile.Type = TileType.Tile;
                    newTile.Tile = tile;
                }
            }
            
            var sideQuestPosition = _gameConfig.SideQuestPosition;
            foreach (var sideQuestPos in sideQuestPosition)
            {
                var position = sideQuestPos;
                var newTile = GetOrCreateTile(position);
                newTile.HasVideo = true;
                var nodeName = position.x + "," + position.y;
                if (_animationLayer)
                {
                    var animationNode = _animationLayer.Find(nodeName);
                    if (animationNode)
                        animationNode.gameObject.SetActive(false);   
                }
            }
            var startPosition = _gameConfig.StartPosition;
            foreach (var startPos in startPosition)
            {
                var position = startPos;
                var newTile = GetOrCreateTile(position);
                newTile.Type = TileType.StartPoint;
            }
            var endPosition = _gameConfig.EndPosition;
            foreach (var endPos in endPosition)
            {
                var position = endPos;
                var newTile = GetOrCreateTile(position);
                newTile.Type = TileType.EndPoint;
                newTile.HasVideo = true;
                var nodeName = position.x + "," + position.y;
                if (_animationLayer)
                {
                    var animationNode = _animationLayer.Find(nodeName);
                    if (animationNode)
                        animationNode.gameObject.SetActive(false);   
                }
            }
            var trapPosition = _gameConfig.TrapPosition;
            foreach (var trapPos in trapPosition)
            {
                var position = trapPos;
                var newTile = GetOrCreateTile(position);
                newTile.Type = TileType.Trap;
            }
            var obstaclePosition = _gameConfig.ObstaclePosition;
            foreach (var cfg in obstaclePosition)
            {
                var position = cfg.Position;
                var newTile = GetOrCreateTile(position);
                newTile.PropsNeedList = new List<PropsGroup>();
                foreach (var propsGroup in cfg.PropsNeedConfig)
                {
                    newTile.PropsNeedList.Add(propsGroup);
                }
                newTile.EffectAssetName = cfg.EffectAsset;
                newTile.ObstacleGuidePosition = cfg.GuidePosition;
            }
            var propsPosition = _gameConfig.PropsPosition;
            foreach (var cfg in propsPosition)
            {
                var position = cfg.Position;
                var newTile = GetOrCreateTile(position);
                newTile.PropsGetList = new List<PropsGroup>();
                foreach (var propsGroup in cfg.PropsGetConfig)
                {
                    newTile.PropsGetList.Add(propsGroup);
                }
            }
            foreach (var cfg in propsPosition)
            {
                CreatePropsBubble(cfg);
            }
            foreach (var startPos in startPosition)
            {
                var beginTile = TileDic[startPos.x][startPos.y];
                BuildTileConnectState(beginTile);
                OpenTile(beginTile);
            }
            foreach (var startPos in startPosition)//初始化地块边遮盖
            {
                CreateAllSideCoverOnDigTile(startPos);
            }

            foreach (var cfg in _gameConfig.GuidePositionList)
            {
                var tile = GetTile(cfg.Position);
                tile.GuidePosition = cfg.GuidePosition;
            }

            PlayWomanLoopPainEffect();
            await ShowTargetStatus();
            if (!this)
                return;
            // _mainUI.SetStartLevel();
            if (_isFirstTimePlay)
            {
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameWater1);
            }
            if (_gameConfig.StartGuidePosition > 0)
            {
                PlayGuide(_gameConfig.StartGuidePosition);
            }
            ShowHandGuideAt(startPosition[0]);
            EventDispatcher.Instance.AddEventListener(EventEnum.OnDigTile,RemoveAllHandGuide);
        }
        public float GetDistance(Vector3 a, Vector3 b)
        {
            var distanceVector = a - b;
            return Mathf.Sqrt(distanceVector.x * distanceVector.x + distanceVector.y * distanceVector.y);
        }
        private void OnDestroy()
        {
            EventDispatcher.Instance.RemoveEventListener(EventEnum.OnDigTile,RemoveAllHandGuide);
        }

        private GameObject _propsBubbleResources;
        public void CreatePropsBubble(PropsConfig cfg)
        {
            var tile = GetTile(cfg.Position);
            var propsNode = _propsLayer.Find(tile.GetNodeName());
            if (_propsBubbleResources == null)
                _propsBubbleResources = ResourcesManager.Instance.LoadResource<GameObject>("DigTrench/Prefabs/Bubble",addToCache:false);
            var bubble = GameObject.Instantiate(_propsBubbleResources, propsNode).transform;
            bubble.name = "Bubble";
            bubble.gameObject.setLayer(_gameConfig.CameraConfig.CullingMask,true);
            bubble.localPosition = Vector3.zero;
            bubble.GetComponent<Animator>().PlayAnim("loop");
        }
        public void ShowHandGuideAt(Vector3Int guidePosition)
        {
            var tile = GetTile(guidePosition);
            foreach (var pair in tile.TileConnectState)
            {
                if (pair.Value == ConnectState.Tile)
                {
                    ShowHandGuideTo(guidePosition, GetConnectPosition(guidePosition, pair.Key));
                }
            }
        }
        private GameObject guideHandResources;
        private List<Transform> guideHands = new List<Transform>();
        public void ShowHandGuideTo(Vector3Int startPosition,Vector3Int endPosition)
        {
            var startPos = _tileMap.GetCellCenterWorld(startPosition);
            var endPos = _tileMap.GetCellCenterWorld(endPosition);
            if (guideHandResources == null)
                guideHandResources = ResourcesManager.Instance.LoadResource<GameObject>("DigTrench/Prefabs/GuideArrow",addToCache:false);
            var handObj = GameObject.Instantiate(guideHandResources, _guideHandLayer).transform;
            handObj.gameObject.setLayer(_gameConfig.CameraConfig.CullingMask,true);
            handObj.position = startPos;
            handObj.DOMove(endPos, 2f).SetLoops(-1, LoopType.Restart);
            guideHands.Add(handObj);
        }

        public void RemoveAllHandGuide(BaseEvent evt)
        {
            foreach (var hand in guideHands)
            {
                hand.DOKill();
                GameObject.Destroy(hand.gameObject);
            }
            guideHands.Clear();
        }

        public void EnableClickEvent(bool enable)
        {
            _transform.Find("PointEventLayer").GetComponent<Collider2D>().enabled = enable;
        }
        public async Task ShowTargetStatus()
        {
            EnableClickEvent(false);
            // var totalTargetPosition = _tileMap.GetCellCenterWorld(_gameConfig.EndPosition[0]);
            // totalTargetPosition.z = _camera.transform.position.z;
            _camera.transform.localPosition = _gameConfig.CameraConfig.EndPosition.ToVector3();
            var totalTargetPosition = _camera.transform.position;
            var targetPosition = SetCameraPositionInScreen(totalTargetPosition);
            _camera.transform.position = targetPosition;
            var startSize = _camera.orthographicSize;
            var endSize = startSize * 0.7f;
            // await XUtility.WaitSeconds(1f);
            // if (!this)
            //     return;
            // var task1 = new TaskCompletionSource<bool>();
            // DOTween.To(() => startSize, (v) =>
            // {
            //     if (this)
            //     {
            //         _camera.orthographicSize = v;
            //         _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
            //     }
            // }, endSize, 1f).OnComplete(() =>
            // {
            //     if (this)
            //     {
            //         _camera.orthographicSize = endSize;
            //         _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
            //     }
            //     task1.SetResult(true);
            // });
            // await task1.Task;
            _camera.orthographicSize = endSize;
            _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
            if (!this)
                return;
            _mainUI.ShowTargetText();
            await XUtility.WaitSeconds(2f);
            if (!this)
                return;
            var task2 = new TaskCompletionSource<bool>();
            DOTween.To(() => endSize, (v) =>
            {
                if (this)
                {
                    _camera.orthographicSize = v;
                    _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
                }
            }, startSize, 1f).OnComplete(() =>
            {
                if (this)
                {
                    _camera.orthographicSize = startSize;
                    _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
                }

                task2.SetResult(true);
            });
            await task2.Task;
            if (!this)
                return;
            await XUtility.WaitSeconds(0.3f);
            if (!this)
                return;
            if (_gameConfig.CameraConfig.CameraMoveTime > 0f)
            {
                var waitTask = new TaskCompletionSource<bool>();
                _camera.transform.DOLocalMove(_gameConfig.CameraConfig.Position.ToVector3(), _gameConfig.CameraConfig.CameraMoveTime).OnComplete(() =>
                {
                    waitTask.SetResult(true);
                }).OnUpdate(() =>
                {
                    if (!this)
                        return;
                    _camera.transform.position = SetCameraPositionInScreen(_camera.transform.position);
                }).SetEase(Ease.Linear);
                await waitTask.Task;
                if (!this)
                    return;
            }
            else
            {
                _camera.transform.localPosition = _gameConfig.CameraConfig.Position.ToVector3();
                _camera.transform.position = SetCameraPositionInScreen(_camera.transform.position);
            }
            EnableClickEvent(true);
        }

        public Vector3 SetCameraPositionInScreen(Vector3 position)//限制camera坐标
        {
            var halfScreenSize = (_screenMaxPosition - _screenMinPosition) / 2 * (_camera.orthographicSize/_defaultScreenSize);

            var xMin = _minCameraPosition.x + halfScreenSize.x;
            var xMax = _maxCameraPosition.x - halfScreenSize.x;
            var yMin = _minCameraPosition.y + halfScreenSize.y;
            var yMax = _maxCameraPosition.y - halfScreenSize.y;

            if (position.x < xMin) position.x = xMin;
            if (position.x > xMax) position.x = xMax;
            if (position.y < yMin) position.y = yMin;
            if (position.y > yMax) position.y = yMax;

            return position;
        }

        public DigTrenchTile GetOrCreateTile(Vector3Int pos)
        {
            return GetOrCreateTile(pos.x, pos.y);
        }
        public DigTrenchTile GetOrCreateTile(int x, int y)
        {
            if (!TileDic.ContainsKey(x))
                TileDic.Add(x,new Dictionary<int, DigTrenchTile>());
            if (!TileDic[x].ContainsKey(y))
            {
                var newTile = new DigTrenchTile()
                {
                    Position = new Vector3Int(x,y,0),
                };
                TileDic[x].Add(y,newTile);   
            }
            return TileDic[x][y];
        }
        public DigTrenchTile GetTile(Vector3Int pos)
        {
            return GetTile(pos.x, pos.y);
        }
        public DigTrenchTile GetTile(int x, int y)
        {
            if (!TileDic.ContainsKey(x))
                return null;
            if (!TileDic[x].ContainsKey(y))
                return null;
            return TileDic[x][y];
        }

        public void OpenTile(DigTrenchTile tile,bool effect = false)
        {
            if (tile.IsOpen)
                return;
            tile.IsOpen = true;
            if (tile.Tile != null)
            {
                if (!effect)
                {
                    _tileMap.SetTile(tile.Position,null);   
                }
                else
                {
                    var a = _tileMap.GetTileFlags(tile.Position);
                    _tileMap.SetTileFlags(tile.Position,TileFlags.None);
                    DOTween.To(() => 1f, value =>
                    {
                        var tempColor = _tileMap.GetColor(tile.Position);
                        tempColor.a = value;
                        _tileMap.SetColor(tile.Position,tempColor);
                    }, 0f, 0.3f);
                }
            }
        }

        private Dictionary<string,GameObject> _obstacleBreakEffectAssets = new Dictionary<string, GameObject>();
        public void ShowObstacleBreakEffect(DigTrenchTile tile)
        {
            if (tile.EffectAssetName.IsEmptyString())
            {
                Debug.LogError("未配置障碍物消失特效");
                return;
            }
            if (!_obstacleBreakEffectAssets.TryGetValue(tile.EffectAssetName, out var effectAsset))
            {
                effectAsset = ResourcesManager.Instance.LoadResource<GameObject>("DigTrench/Prefabs/" + tile.EffectAssetName,addToCache:false);
                _obstacleBreakEffectAssets.Add(tile.EffectAssetName,effectAsset);
            }
            var propsNode = _propsLayer.Find(tile.GetNodeName());
            var effect = GameObject.Instantiate(effectAsset, _obstacleBreakEffectLayer).transform;
            if (_levelId == 1 || _levelId == 2)
            {
                effect.localScale = new Vector3(0.4f, 0.4f, 1f);
            }
            effect.gameObject.setLayer(_gameConfig.CameraConfig.CullingMask,true);
            effect.gameObject.SetActive(true);
            effect.position = propsNode.position;
            XUtility.WaitSeconds(2f, () =>
            {
                if (effect)
                    GameObject.Destroy(effect.gameObject);
            });
        }

        private GameObject tileBreakEffectResources;
        
        public async void ShowTileBreakEffect(Vector3 position)
        {
            if (tileBreakEffectResources == null)
            {
                tileBreakEffectResources = ResourcesManager.Instance.LoadResource<GameObject>("DigTrench/Prefabs/VFX_Soil_1",addToCache:false);
            }
            var tileBreakEffect = tileBreakEffectResources.Instantiate();
            if (_levelId == 99)
            {
                {
                    var main = tileBreakEffect.transform.Find("Root/Stone").GetComponent<ParticleSystem>().main;
                    main.startColor = new ParticleSystem.MinMaxGradient("81BDFB".HexToColor());
                }
                {
                    var main = tileBreakEffect.transform.Find("Root/Smoke").GetComponent<ParticleSystem>().main;
                    main.startColor = new ParticleSystem.MinMaxGradient("81BDFB".HexToColor());
                }
            }
            tileBreakEffect.setLayer(_gameConfig.CameraConfig.CullingMask,true);
            tileBreakEffect.transform.SetParent(_effectLayer,false);
            tileBreakEffect.transform.position = position;
            await XUtility.WaitSeconds(2f);
            if (tileBreakEffect)
                GameObject.Destroy(tileBreakEffect);
        }

        public void ShowTileBreakLineEffect(DigTrenchTile tile,ConnectDirection direction)
        {
            var centerPosition = _tileMap.GetCellCenterWorld(tile.Position);
            var endTile = GetConnectPosition(tile.Position, direction);
            var endPosition = (_tileMap.GetCellCenterWorld(tile.Position) + _tileMap.GetCellCenterWorld(endTile)) / 2;
            var startPosition = centerPosition - (endPosition - centerPosition);
            var effectCount = 10;
            var effectTile = 0.5f;
            for (var i = 0; i < effectCount; i++)
            {
                var effectPosition = startPosition + (endPosition - startPosition) / (effectCount - 1) * i;
                var waitTime = effectTile /(effectCount - 1) * i;
                XUtility.WaitSeconds(waitTime, () =>
                {
                    if (!this)
                        return;
                    ShowTileBreakEffect(effectPosition);
                });
            }
        }
        private GameObject sideCoverResources;

        string GetPositionName(Vector3Int pos)
        {
            return pos.x + "," + pos.y;
        }
        string GetSideCoverName(Vector3Int startPos,Vector3Int endPos)
        {
            return GetPositionName(startPos) + "_" + GetPositionName(endPos);
        }

        Vector3 GetSideCoverLocalScale(ConnectDirection connectType)
        {
            if (connectType == ConnectDirection.XNegative)
            {
                return new Vector3(1, 1, 1);
            }
            else if (connectType == ConnectDirection.XPositive)
            {
                return new Vector3(-1, -1, 1);
            }
            else if (connectType == ConnectDirection.YNegative)
            {
                return new Vector3(-1, 1, 1);
            }
            else if (connectType == ConnectDirection.YPositive)
            {
                return new Vector3(1, -1, 1);
            }
            Debug.LogError("GetSideCoverLocalScale 错误的方向类型"+connectType);
            return new Vector3(0, 0, 1);
        }

        private int _dustSortGroup = -100;
        public Transform CreateSideCover(Vector3Int startPos,Vector3Int endPos)
        {
            var name = GetSideCoverName(startPos, endPos);
            foreach (Transform oldSideCoverTransform in _sideCoverLayer)
            {
                if (oldSideCoverTransform.name == name)
                    return oldSideCoverTransform;
            }
            if (sideCoverResources == null)
                sideCoverResources = ResourcesManager.Instance.LoadResource<GameObject>("DigTrench/Prefabs/DustSideCover",addToCache:false);
            var sideCover = sideCoverResources.Instantiate();
            if (_levelId == 99)
            {
                {
                    var main = sideCover.transform.Find("Image").GetComponent<SpriteRenderer>();
                    main.color = "81BDFB".HexToColor();
                    main.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                }
            }
            else
            {
                {
                    var main = sideCover.transform.Find("Image").GetComponent<SpriteRenderer>();
                    // main.color = "81BDFB".HexToColor();
                    main.transform.localScale = new Vector3(0.4f, 0.4f, 1f);
                }
            }
            var sortGroup = sideCover.AddComponent<SortingGroup>();
            if (_levelId == 99)
            {
                sortGroup.sortingLayerName = "Roadside";
            }
            sortGroup.sortingOrder = _dustSortGroup++;
            sideCover.setLayer(_gameConfig.CameraConfig.CullingMask,true);
            sideCover.transform.SetParent(_sideCoverLayer,false);
            sideCover.name = name;
            var position = (_tileMap.GetCellCenterWorld(startPos) + _tileMap.GetCellCenterWorld(endPos)) / 2;
            sideCover.transform.position = position;
            var connectType = GetConnectType(startPos, endPos);
            sideCover.transform.localScale = GetSideCoverLocalScale(connectType);
            return sideCover.transform;
        }

        public bool RemoveSideCover(Vector3Int startPos,Vector3Int endPos)
        {
            var name = GetSideCoverName(startPos, endPos);
            foreach (Transform oldSideCoverTransform in _sideCoverLayer)
            {
                if (oldSideCoverTransform.name == name)
                {
                    GameObject.Destroy(oldSideCoverTransform.gameObject);
                    return true;
                }
            }
            return false;
        }

        public void RemoveAllSideCoverOnDigTile(Vector3Int endPos) //删除所有朝向该地块的边遮盖物
        {
            RemoveSideCover(new Vector3Int(endPos.x+1, endPos.y, endPos.z), endPos);
            RemoveSideCover(new Vector3Int(endPos.x-1, endPos.y, endPos.z), endPos);
            RemoveSideCover(new Vector3Int(endPos.x, endPos.y+1, endPos.z), endPos);
            RemoveSideCover(new Vector3Int(endPos.x, endPos.y-1, endPos.z), endPos);
        }
        public bool CanCreateSideCover(Vector3Int startPos,Vector3Int endPos)
        {
            var startTile = GetTile(startPos);
            var endTile = GetTile(endPos);
            if (startTile != null && endTile != null &&
                (startTile.Type == TileType.Tile || startTile.Type == TileType.StartPoint|| startTile.Type == TileType.Trap) && 
                (endTile.Type == TileType.Tile || endTile.Type == TileType.EndPoint || endTile.Type == TileType.Trap) &&
                startTile.IsOpen && !endTile.IsOpen)
                return true;
            return false;
        }
        public void CreateAllSideCoverOnDigTile(Vector3Int startPos) //生成所有从该地块开始的边遮盖物
        {
            {
                var endPos = new Vector3Int(startPos.x + 1, startPos.y, startPos.z);
                if (CanCreateSideCover(startPos, endPos))
                    CreateSideCover(startPos, endPos);
            }
            {
                var endPos = new Vector3Int(startPos.x - 1, startPos.y, startPos.z);
                if (CanCreateSideCover(startPos, endPos))
                    CreateSideCover(startPos, endPos);
            }
            {
                var endPos = new Vector3Int(startPos.x , startPos.y+1, startPos.z);
                if (CanCreateSideCover(startPos, endPos))
                    CreateSideCover(startPos, endPos);
            }
            {
                var endPos = new Vector3Int(startPos.x , startPos.y-1, startPos.z);
                if (CanCreateSideCover(startPos, endPos))
                    CreateSideCover(startPos, endPos);
            }
        }
        
        public DigResult DigTile(DigTrenchTile tile,ConnectDirection direction)
        {
            var breakObstacle = false;
            var getProps = false;
            if (tile.PropsNeedList != null)//地块需要道具时查找背包中是否有足够的道具
            {
                var canAffordAll = true;
                List<Action> backtraceAction = new List<Action>();//先减,当最后发现道具不够时再加回来
                foreach (var needPropsGroup in tile.PropsNeedList)
                {
                    var canAffordSingle = false;
                    foreach (var hasPropsGroup in _propsBag)
                    {
                        if (needPropsGroup.PropsId == hasPropsGroup.PropsId)
                        {
                            if (hasPropsGroup.Count >= needPropsGroup.Count)
                            {
                                hasPropsGroup.Count -= needPropsGroup.Count;
                                backtraceAction.Add(() =>
                                {
                                    hasPropsGroup.Count += needPropsGroup.Count;
                                });
                                canAffordSingle = true;
                            }
                            break;
                        }
                    }
                    if (!canAffordSingle)
                    {
                        canAffordAll = false;
                        break;
                    }
                }
                if (!canAffordAll)
                {
                    foreach (var backtrace in backtraceAction)
                    {
                        backtrace.Invoke();
                    }

                    if (tile.ObstacleGuidePosition > 0)
                    {
                        PlayGuide(tile.ObstacleGuidePosition);
                    }
                    return DigResult.Failed;
                }
                else
                {
                    var propsNode = _propsLayer.Find(tile.GetNodeName());
                    propsNode.gameObject.SetActive(false);
                    ShowObstacleBreakEffect(tile);
                    var worldPosition = propsNode.position;
                    foreach (var needPropsGroup in tile.PropsNeedList)
                    {
                        _mainUI.UseProps(needPropsGroup,worldPosition);
                    }

                    breakObstacle = true;
                    BreakObstacle();
                }
            }
            _isDigStart = true;
            var result = DigResult.Success;
            XUtility.WaitSeconds(0.1f, () =>
            {
                if (!this)
                    return;
                ShowTileBreakLineEffect(tile, direction);
            });
            OpenTile(tile,true);
            RemoveAllSideCoverOnDigTile(tile.Position);
            CreateAllSideCoverOnDigTile(tile.Position);
            PlayDigEffect();
            EventDispatcher.Instance.DispatchEventImmediately(EventEnum.OnDigTile, tile);
            if (tile.PropsGetList != null)//地块获得道具
            {
                getProps = true;
                PlayGetProps();
                var propsNode = _propsLayer.Find(tile.GetNodeName());
                var bubble = propsNode.Find("Bubble");
                if (bubble)
                {
                    propsNode.GetComponent<SpriteRenderer>().enabled = false;
                    bubble.GetComponent<Animator>().Play("disappear", (animator) =>
                    {
                        if (propsNode)
                            propsNode.gameObject.SetActive(false);
                    });
                }
                else
                {
                    propsNode.gameObject.SetActive(false);   
                }
                var propsSprite = propsNode.GetComponent<SpriteRenderer>().sprite;
                var worldPosition = propsNode.position;
                var screenPosition = _camera.WorldToScreenPoint(worldPosition);
                foreach (var getPropsGroup in tile.PropsGetList)
                {
                    _mainUI.GetProps(getPropsGroup,propsSprite,screenPosition);
                    var getFlag = false;
                    foreach (var hasPropsGroup in _propsBag)
                    {
                        if (hasPropsGroup.PropsId == getPropsGroup.PropsId)
                        {
                            hasPropsGroup.Count += getPropsGroup.PropsId;
                            getFlag = true;
                            break;
                        }
                    }
                    if (!getFlag)
                    {
                        var newPropsGroup = new PropsGroup();
                        newPropsGroup.Copy(getPropsGroup);
                        _propsBag.Add(newPropsGroup);
                    }
                }
            }
            if (tile.HasVideo)//终点或支线任务节点需要播放动画
            {
                // if (tile.Type == TileType.EndPoint)
                // {
                //     PlayWomanWinEffect();
                // }
                if (!getProps && !breakObstacle && tile.Type != TileType.EndPoint)
                {
                    PlaySaveFish();
                }
                PlayPlantGrow();
                if (_isFirstTimePlay)
                {
                    _videoCount++;
                    if (_videoCount == 1)
                    {
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameWater2);
                    }
                    else if (_videoCount == 2)
                    {
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameWater3);
                    }
                    else if (_videoCount == 3)
                    {
                        GameBIManager.Instance.SendGameEvent(
                            BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameWater4);
                    }
                }
                var nodeName = tile.GetNodeName();
                if (_animationLayer)
                {
                    var animationNode = _animationLayer.Find(nodeName);
                    if (animationNode)
                    {
                        animationNode.gameObject.SetActive(true);
                        var animationPlayer = new AnimationPlayer(animationNode);
                        if (_transform.GetComponent<MBCoroutine>() == null)
                        {
                            _transform.gameObject.AddComponent<MBCoroutine>();
                        }
                        _transform.GetComponent<MBCoroutine>().StartCoroutine(animationPlayer.PlayShowAnimation());   
                    }   
                }

                var staticNode = _staticLayer.Find(nodeName);
                if (staticNode)
                {
                    var staticSpinePlayer = new StaticSpinPlayer(staticNode);
                    staticSpinePlayer.PlayGetWaterSpineState();
                    result = DigResult.VideoSuccess;
                }
            }

            if (tile.GuidePosition > 0)
            {
                PlayGuide(tile.GuidePosition);
            }
            return result;
        }

        public void HandleDigFailed(DigResult result,DigTrenchTile tile)
        {
            EndDig(DigEndType.Wall);
        }
        public void BuildTileConnectState(DigTrenchTile tile)
        {
            if (tile.Init)
                return;
            tile.Init = true;

            var xPositiveTile = GetTile(tile.Position.x + 1, tile.Position.y);
            if (xPositiveTile == null)
                tile.XPositive = ConnectState.Wall;
            else
            {
                if (xPositiveTile.Type == TileType.Trap)
                    tile.XPositive = ConnectState.Trap;
                else if (xPositiveTile.Type == TileType.EndPoint)
                    tile.XPositive = ConnectState.Target;
                else
                    tile.XPositive = ConnectState.Tile;
                BuildTileConnectState(xPositiveTile);   
            }

            var xNegativeTile = GetTile(tile.Position.x - 1, tile.Position.y);
            if (xNegativeTile == null)
                tile.XNegative = ConnectState.Wall;
            else
            {
                if (xNegativeTile.Type == TileType.Trap)
                    tile.XNegative = ConnectState.Trap;
                else if (xNegativeTile.Type == TileType.EndPoint)
                    tile.XNegative = ConnectState.Target;
                else
                    tile.XNegative = ConnectState.Tile;
                BuildTileConnectState(xNegativeTile);   
            }

            var yPositiveTile = GetTile(tile.Position.x , tile.Position.y + 1);
            if (yPositiveTile == null)
                tile.YPositive = ConnectState.Wall;
            else
            {
                if (yPositiveTile.Type == TileType.Trap)
                    tile.YPositive = ConnectState.Trap;
                else if (yPositiveTile.Type == TileType.EndPoint)
                    tile.YPositive = ConnectState.Target;
                else
                    tile.YPositive = ConnectState.Tile;
                BuildTileConnectState(yPositiveTile);
            }

            var yNegativeTile = GetTile(tile.Position.x , tile.Position.y - 1);
            if (yNegativeTile == null)
                tile.YNegative = ConnectState.Wall;
            else
            {
                if (yNegativeTile.Type == TileType.Trap)
                    tile.YNegative = ConnectState.Trap;
                else if (yNegativeTile.Type == TileType.EndPoint)
                    tile.YNegative = ConnectState.Target;
                else
                    tile.YNegative = ConnectState.Tile;
                BuildTileConnectState(yNegativeTile);
            }
        }

        ConnectDirection GetConnectType(Vector3Int a, Vector3Int b)
        {
            if (a.x == b.x && a.y+1 == b.y)
                return ConnectDirection.YPositive;
            if (a.x == b.x && a.y-1 == b.y)
                return ConnectDirection.YNegative;
            if (a.x+1 == b.x && a.y == b.y)
                return ConnectDirection.XPositive;
            if (a.x-1 == b.x && a.y == b.y)
                return ConnectDirection.XNegative;
            if (a.x == b.x && a.y == b.y)
                return ConnectDirection.SamePosition;
            return ConnectDirection.DisConnect;
        }

        Vector3Int GetConnectPosition(Vector3Int a, ConnectDirection connectType)
        {
            if (connectType == ConnectDirection.XNegative)
                a.x -= 1;
            else if (connectType == ConnectDirection.XPositive)
                a.x += 1;
            else if (connectType == ConnectDirection.YNegative)
                a.y -= 1;
            else if (connectType == ConnectDirection.YPositive)
                a.y += 1;
            return a;
        }

        public List<Vector3Int> GetRoundPosition(Vector3Int center)
        {
            var list = new List<Vector3Int>();
            list.Add(new Vector3Int(center.x+1,center.y,center.z));
            list.Add(new Vector3Int(center.x+1,center.y+1,center.z));
            list.Add(new Vector3Int(center.x,center.y+1,center.z));
            list.Add(new Vector3Int(center.x-1,center.y+1,center.z));
            list.Add(new Vector3Int(center.x-1,center.y,center.z));
            list.Add(new Vector3Int(center.x-1,center.y-1,center.z));
            list.Add(new Vector3Int(center.x,center.y-1,center.z));
            list.Add(new Vector3Int(center.x+1,center.y-1,center.z));
            return list;
        }
        Vector3Int ToCellPosition(PointerEventData pointerData)
        {
            var worldPosition = pointerData.pressEventCamera.ScreenToWorldPoint(new Vector3(pointerData.position.x,pointerData.position.y,-pointerData.pressEventCamera.transform.position.z));
            var cellPosition = _tileMap.WorldToCell(worldPosition);
            // return cellPosition;
            var centerCell = GetTile(cellPosition);
            
            if (centerCell != null && 
                ((centerCell.IsOpen && GetConnectUnOpenTile(centerCell)!=null) || 
                 (!centerCell.IsOpen && GetConnectOpenTile(centerCell) != null)))
            {
                return cellPosition;
            }
            var cellPositionList = GetRoundPosition(cellPosition);
            var roundCellList = new List<DigTrenchTile>();
            foreach (var roundCellPosition in cellPositionList)
            {
                var roundTile = GetTile(roundCellPosition);
                if (roundTile != null && 
                    ((roundTile.IsOpen && GetConnectUnOpenTile(roundTile)!=null) || 
                     (!roundTile.IsOpen && GetConnectOpenTile(roundTile) != null)))
                {
                    roundCellList.Add(roundTile);
                }
            }
            var nearestCellPos = cellPosition;
            var nearestDistance = float.MaxValue;
            // var spaceDistance = 1.5f * _unitTileDistance;
            foreach (var roundCell in roundCellList)
            {
                var roundTileWorldPos = _tileMap.GetCellCenterWorld(roundCell.Position);
                var curDistance = GetDistance(roundTileWorldPos, worldPosition);
                if (curDistance < nearestDistance)
                {
                    var pointList = new List<Vector2>();
                    foreach (var offset in _unitOffsetList)
                    {
                        pointList.Add(roundTileWorldPos + offset * 2f);
                    }
                    if (XUtility.IsPointInPolygon(worldPosition,pointList))
                    {
                        nearestDistance = curDistance;
                        nearestCellPos = roundCell.Position;
                    }   
                }
            }

            // Debug.LogError("触碰偏移 前" + cellPosition + "后" + nearestCellPos);
            return nearestCellPos;
        }
        private Vector3Int _lastCellPos;
        private Vector2 _dragLastScreenPosition;
        private bool _isInDrag = false;//是否玩家在操作中
        private TouchType _touchType = TouchType.None;//此次操作是否挖是在挖格子，false为拖地图
        private bool _isDigStart = false;//此次操作是否开始挖开了新格子
        private bool _isDigEnd = false;//此次挖格子是否结束
        
        public void StartDig(Vector3Int startCellPos)
        {
            _isDigStart = false;
            _isDigEnd = false;
            _lastCellPos = startCellPos;
        }

        public void StartDragBackground(PointerEventData data)
        {
            _dragLastScreenPosition = data.position;
        }
        public void EndDragBackground(PointerEventData data)
        {
        }

        public void EndDig(DigEndType endType)
        {
            if (_isDigEnd)
                return;
            // _isDigStart = false;
            _isDigEnd = true;
            if (endType == DigEndType.PointerUp)
            {
                Debug.LogError("抬手结束");
            }
            else if (endType == DigEndType.Wall)
            {
                Debug.LogError("撞墙结束");
            }
            else if (endType == DigEndType.Trap)
            {
                Debug.LogError("踩陷阱结束");
            }
            else if (endType == DigEndType.Target)
            {
                Debug.LogError("到达终点结束");
                if (_isFirstTimePlay)
                {
                    GameBIManager.Instance.SendGameEvent(
                        BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtueMinigameWater5);
                }
                EventDispatcher.Instance.DispatchEventImmediately(EventEnum.CompletedDigTrenchGame);
                GameBIManager.Instance.SendGameEvent(
                    BiEventAdventureIslandMerge.Types.GameEventType.GameEventMinigameWaterLevelend,
                    data1:_levelId.ToString(),data2:"0");
                _mainUI.HideCloseBtn();
                EnableClickEvent(false);
                StopWomanLoopPainEffect();
                _camera.transform.DOLocalMove(_gameConfig.CameraConfig.EndPosition.ToVector3(), 0.5f).OnUpdate(() =>
                {
                    _camera.transform.position = SetCameraPositionInScreen(_camera.transform.position);
                }).SetEase(Ease.Linear);//结束时把镜头拉到终点
                var winStaticNode = _staticLayer.Find("FinalWin");
                if (winStaticNode)
                {
                    PerformEndCameraBackWin(winStaticNode);
                }
                else
                {
                    PlayWomanWinEffect();
                    XUtility.WaitSeconds(5f, PerformNormalWin);
                }
            }
        }

        public async void PerformNormalWin()
        {
            PlayWomanCelebrateEffect();
            _mainUI.SetFinishLevel();
        }
        public async void PerformEndCameraBackWin(Transform winStaticNode)
        {
            await XUtility.WaitSeconds(2f);
            var staticSpinePlayer = new StaticSpinPlayer(winStaticNode);
            staticSpinePlayer.PlayGetWaterSpineState("sad_happy2");
            var moveTask = new TaskCompletionSource<bool>();
            _camera.transform.DOLocalMove(_gameConfig.CameraConfig.Position.ToVector3(), _gameConfig.CameraConfig.CameraMoveTime).OnUpdate(() =>
            {
                _camera.transform.position = SetCameraPositionInScreen(_camera.transform.position);
            }).SetEase(Ease.Linear).OnComplete(()=>moveTask.SetResult(true));//结束时把镜头拉到终点
            await moveTask.Task;
            PlayWomanWinEffect();
            if (!this)
                return;
            _camera.transform.localPosition = _gameConfig.CameraConfig.Position.ToVector3();
            var totalTargetPosition = _camera.transform.position;
            _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
            var startSize = _camera.orthographicSize;
            var endSize = startSize * 0.7f;
            var task1 = new TaskCompletionSource<bool>();
            DOTween.To(() => startSize, (v) =>
            {
                if (this)
                {
                    _camera.orthographicSize = v;
                    _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
                }
            }, endSize, 1f).OnComplete(() =>
            {
                if (this)
                {
                    _camera.orthographicSize = endSize;
                    _camera.transform.position = SetCameraPositionInScreen(totalTargetPosition);
                }
                task1.SetResult(true);
            });
            await task1.Task;
            XUtility.WaitSeconds(3f, PerformNormalWin);
        }
        public void OnDragInRoad(PointerEventData data)
        {
            if (_isDigEnd)
                return;
            var cellPosition = ToCellPosition(data);
            var connectType = GetConnectType(_lastCellPos, cellPosition);
            if (connectType == ConnectDirection.DisConnect)
            {
                Debug.LogError("跳格子了 " + _lastCellPos + "=>" + cellPosition);
                EndDig(DigEndType.PointerUp);
                return;
            }
            
            if (connectType != ConnectDirection.SamePosition)
            {
                var lastTile = GetTile(_lastCellPos);
                if (lastTile != null && lastTile.IsOpen)
                {
                    var connectState = lastTile.TileConnectState[connectType];
                    var newTile = GetTile(cellPosition);
                    if (connectState == ConnectState.Target)
                    {
                        if (!newTile.IsOpen)
                        {
                            var digResult = DigTile(newTile,connectType);
                            if (digResult == DigResult.Failed)
                                HandleDigFailed(digResult, newTile);
                            else
                                EndDig(DigEndType.Target);
                        }
                    }
                    else if (connectState == ConnectState.Tile)
                    {
                        _lastCellPos = cellPosition;
                        if (newTile != null && !newTile.IsOpen)
                        {
                            var digResult = DigTile(newTile,connectType);
                            if (digResult == DigResult.Failed)
                                HandleDigFailed(digResult, newTile);
                            else if (digResult == DigResult.VideoSuccess)
                                EndDig(DigEndType.PointerUp);
                        }
                    }
                    else if (_isDigStart)
                    {
                        if (connectState == ConnectState.Wall)
                        {
                            EndDig(DigEndType.Wall);
                        }
                        else if (connectState == ConnectState.Trap)
                        {
                            EndDig(DigEndType.Trap);
                        }
                    }
                }
                _lastCellPos = cellPosition;
            }
        }

        public void OnDragBackground(PointerEventData data)
        {
            var camera = data.pressEventCamera;
            // var layerTransform = _camera.transform.parent;
            // var nowPointLocalPosition = layerTransform.InverseTransformPoint(camera.ScreenToWorldPoint(data.position));
            // var startPointLocalPosition = layerTransform.InverseTransformPoint(camera.ScreenToWorldPoint(_dragLastScreenPosition));
            var nowPointPosition = camera.ScreenToWorldPoint(data.position);
            var startPointPosition = camera.ScreenToWorldPoint(_dragLastScreenPosition);
            
            // var moveLocalPosition = nowPointLocalPosition - startPointLocalPosition;
            var movePosition = nowPointPosition - startPointPosition;
            var cameraTransform = _camera.transform;
            var nextPos = cameraTransform.position - movePosition; //向手指移动反方向移动
            nextPos = SetCameraPositionInScreen(nextPos);
            cameraTransform.position = nextPos;
            _dragLastScreenPosition = data.position;
        }
        
        
        public void OnPointerDown(PointerEventData data)
        {
            if (eventSystemEnableFrameCount > 0 && (Time.frameCount - eventSystemEnableFrameCount) < 10)
            {
                Debug.LogError("触发忽略第一次触碰机制"+(Time.frameCount - eventSystemEnableFrameCount));
                eventSystemEnableFrameCount = 0;
                return;
            }
            else
            {
                Debug.LogError("未触发忽略第一次触碰机制"+(Time.frameCount - eventSystemEnableFrameCount));
                eventSystemEnableFrameCount = 0;
            }
            if (_isInDrag)
                return;
            _isInDrag = true;
            var cellPosition = ToCellPosition(data);
            var touchTile = GetTile(cellPosition);
            if (touchTile != null && touchTile.IsOpen && GetConnectUnOpenTile(touchTile)!=null)
            {
                _touchType = TouchType.Road;
                StartDig(cellPosition);   
            }
            else if (touchTile != null && !touchTile.IsOpen && GetConnectOpenTile(touchTile) != null)
            {
                _touchType = TouchType.Road;
                StartDig(GetConnectOpenTile(touchTile).Position);
                OnDragInRoad(data);
            }
            else
            {
                _touchType = TouchType.Background;
                StartDragBackground(data);
            }
        }

        public DigTrenchTile GetConnectOpenTile(DigTrenchTile touchTile)
        {
            foreach (var pair in touchTile.TileConnectState)
            {
                if (pair.Value == ConnectState.Tile)
                {
                    var connectTile = GetTile(GetConnectPosition(touchTile.Position, pair.Key));
                    if (connectTile != null && connectTile.IsOpen)
                    {
                        return connectTile;
                    }
                }
            }
            return null;
        }
        public DigTrenchTile GetConnectUnOpenTile(DigTrenchTile touchTile)
        {
            foreach (var pair in touchTile.TileConnectState)
            {
                if (pair.Value == ConnectState.Tile || pair.Value == ConnectState.Target || pair.Value == ConnectState.Trap)
                {
                    var connectTile = GetTile(GetConnectPosition(touchTile.Position, pair.Key));
                    if (connectTile != null && !connectTile.IsOpen)
                    {
                        return connectTile;
                    }
                }
            }
            return null;
        }

        // public void LogFrameCount()
        // {
        //     Debug.LogError("帧差"+(Time.frameCount - frameCount));
        //     frameCount = Time.frameCount;
        // }
        // private int frameCount = 0;
        public void OnDrag(PointerEventData data)
        {
            // LogFrameCount();
            if (Input.touchCount > 1)
            {
                Debug.LogError("触碰点数量大于1");
                return;
            }
            if (!_isInDrag)
                return;
            if ( _touchType == TouchType.Road)
            {
                OnDragInRoad(data);
            }
            else if( _touchType == TouchType.Background)
            {
                OnDragBackground(data);
            }
        }
        public void OnPointerUp(PointerEventData data)
        {
            if (!_isInDrag)
                return;
            _isInDrag = false;
            if (_touchType == TouchType.Road)
            {
                EndDig(DigEndType.PointerUp);
            }
            else if(_touchType == TouchType.Background)
            {
                EndDragBackground(data);
            }
            _touchType = TouchType.None;
        }
    }
}