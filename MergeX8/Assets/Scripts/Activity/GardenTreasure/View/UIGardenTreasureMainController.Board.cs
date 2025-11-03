using System;
using System.Collections;
using System.Collections.Generic;
using Activity.Base;
using Activity.GardenTreasure.Model;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.GardenTreasure;
using DragonU3DSDK.Asset;
using DragonU3DSDK.Network.API.Protocol;
using Framework;
using Gameplay;
using SomeWhere;
using UnityEngine;
using UnityEngine.UI;

public partial class UIGardenTreasureMainController
{
    public class ShapeItem
    {
        public GameObject _gameObject;
        public string _name;
        public GardenTreasureConfigManager.ShapeData _shapeData;

        public int index
        {
            get
            {
                return _shapeData._index;
            }
        }
        
        public ShapeItem(GameObject gameObject, GardenTreasureConfigManager.ShapeData shapeData, Vector3 localPosition)
        {
            _gameObject = gameObject;
            _shapeData = shapeData;

            var shapeConfig = GardenTreasureConfigManager.Instance.GardenTreasureShapeConfigList.Find(a => a.Id.ToString() == shapeData._shapeSize);
                    
            var image = _gameObject.transform.GetComponent<Image>();
            image.sprite = ResourcesManager.Instance.GetSpriteVariant("GardenTreasureAtlas", GardenTreasureModel.Instance.IsRandomLevel() ? shapeConfig.RandomShapeName : shapeConfig.ShapeName);
            image.SetNativeSize();

            _name = shapeData._shapeSize;
            
            _gameObject.transform.localPosition = localPosition;
        }
        public void Destroy()
        {
            if(_gameObject == null)
                return;
            
            DestroyImmediate(_gameObject);
            _gameObject = null;
        }

        public void SetActive(bool isActive)
        {
            _gameObject.gameObject.SetActive(isActive);
        }
    }
    
    public class ShapeGroup
    {
        public GameObject _gameObject;
        private GameObject _shapeItem;
        private GardenTreasureConfigManager.BoardData _boardData;
        private List<ShapeItem> _shapeItems = new List<ShapeItem>();
        private int _size;
        private BoardGroup _boardGroup;
        public ShapeGroup(GameObject gameObject, BoardGroup boardGroup, int size)
        {
            _boardGroup = boardGroup;
            _size = size;
            _gameObject = gameObject.transform.Find("RewardRoot").gameObject;
            _shapeItem = _gameObject.transform.Find("Image").gameObject;
            _shapeItem.gameObject.SetActive(false);
        }

        public void InitShape(GardenTreasureConfigManager.BoardData boardData)
        {
            _boardData = boardData;

            foreach (var shapeData in boardData._shapeDatas)
            {
                bool isAllOpen = true;
                foreach (var grid in shapeData._shapeGrids)
                {
                    int index = grid.x * _size + grid.y;
                    bool isOpen = GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Contains(index);
                    if(isOpen)
                        continue;

                    isAllOpen = false;
                    break;
                }

                if (!isAllOpen)
                {
                    GameObject shape = Instantiate(_shapeItem, _gameObject.transform, false);
                    shape.gameObject.SetActive(true);

                    ShapeItem item = new ShapeItem(shape, shapeData,_boardGroup.GetItem(shapeData._shapeGrids[0].x, shapeData._shapeGrids[0].y)._gameObject.transform.localPosition);
                    _shapeItems.Add(item);
                }
            }
        }
        
        public void Destroy()
        {
            foreach (var shapeItem in _shapeItems)
            {
                shapeItem.Destroy();
            }
            _shapeItems.Clear();
        }

        public ShapeItem GetShapeItem(int index)
        {
            return _shapeItems.Find(a => a.index == index);
        }
    }
    
    public class BoardItem
    {
        public GameObject _gameObject;
        public int _row;
        public int _col;
        private Transform _selectTransform;

        public Vector3 EffectPosition => _selectTransform.transform.position;
        
        public BoardItem(GameObject gameObject, int row, int col)
        {
            _gameObject = gameObject;
            _row = row;
            _col = col;

            _selectTransform = _gameObject.transform.Find("Select");
            SetSelectActive(false);
        }

        public void SetActive(bool isActive)
        {
            _gameObject.SetActive(isActive);
        }

        public void SetSelectActive(bool isActive)
        {
            _selectTransform?.gameObject.SetActive(isActive);
        }
        
        public void Destroy()
        {
            
        }

        public void Open()
        {
            SetActive(false);
        }
    }
    
    public class BoardGroup
    {
        public GameObject _gameObject;
        public BoardItem[,] _boardItems;
        public ShapeGroup _shapeGroup;
        public int _size = 0;
        private GameObject _normalBg;
        private GameObject _randomBg;
        
        public BoardGroup(int size, GameObject gameObject, Action<BoardItem, int,int> clickAction)
        {
            _size = size;
            _gameObject = gameObject;
            _shapeGroup = new ShapeGroup(_gameObject, this, size);
            
            _boardItems = new BoardItem[size, size];

            _normalBg = gameObject.transform.Find("BGNormal").gameObject;
            _randomBg = gameObject.transform.Find("BGGold").gameObject;
            
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int index = i * size + j +  1;
                    var itemObj = _gameObject.transform.Find(index.ToString()).gameObject;

                    int row = i;
                    int col = j;
                    BoardItem item = new BoardItem(itemObj, i, j);
                    _boardItems[i, j] = item;
                    
                    var button = itemObj.transform.GetComponent<Button>();
                    button.onClick.AddListener(() =>
                    {
                        clickAction?.Invoke(item, row, col);
                    });

                    CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWorkFrame(2, () =>
                    {
                        CommonUtils.SetShieldButUnEnable(button.gameObject);
                    }));
                }
            }
        }

        public void Init(GardenTreasureLevelConfig config, GardenTreasureConfigManager.BoardData boardData)
        {
            _shapeGroup.Destroy();

            for (int i = 0; i < _boardItems.GetLength(0); i++)
            {
                for (int j = 0; j < _boardItems.GetLength(1); j++)
                {
                    int index = i * _size + j;

                    bool isOpen = GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Contains(index);
                    _boardItems[i,j].SetActive(!isOpen);
                    _boardItems[i,j].SetSelectActive(false);
                }
            }
            
            _shapeGroup.InitShape(boardData);
            
            _normalBg.gameObject.SetActive(!GardenTreasureModel.Instance.IsRandomLevel());
            _randomBg.gameObject.SetActive(GardenTreasureModel.Instance.IsRandomLevel());
        }
        
        public void Destroy()
        {
            for (int i = 0; i < _boardItems.GetLength(0); i++)
            {
                for (int j = 0; j < _boardItems.GetLength(1); j++)
                {
                    _boardItems[i,j].Destroy();
                }
            }
            
            _shapeGroup.Destroy();
            _boardItems = null;
        }

        public BoardItem GetItem(int row, int col)
        {
            if (row >= _boardItems.GetLength(0))
                return null;
            
            if (col >= _boardItems.GetLength(1))
                return null;
            
            return _boardItems[row, col];
        }

        public void CleanSelectStatus()
        {
            for (int i = 0; i < _boardItems.GetLength(0); i++)
            {
                for (int j = 0; j < _boardItems.GetLength(1); j++)
                {
                    _boardItems[i,j].SetSelectActive(false);
                }
            }
        }
    }

    private Dictionary<string, BoardGroup> _boardGroups = new Dictionary<string, BoardGroup>();
    private BoardGroup _currentBoard = null;
    private int _curRow;
    private int _curCol;
    private BoardItem _curBoardItem;
    
    
    private int[,] _gridDir = new int[,]
    {
        { 0, -1 },
        { 0, 1 },
        { 1, 0 },
        { -1, 0 },
        { 0, 0 },
    };
    
    private void AwakeBoard()
    {
        _curRow = -1;
        _curCol = -1;
        
        for (int i = 3; i <= 10; i++)
        {
            var grid = transform.Find($"Root/Grid/{i}");
            if(grid == null)
                continue;

            BoardGroup group = new BoardGroup(i, grid.gameObject, ClickAction);
            group._gameObject.SetActive(false);
            _boardGroups.Add(""+i+i, group);
        }
    }

    private void InitBoard()
    {
        var config = GardenTreasureModel.Instance.GetCurrentLevelConfig();
        var boardData = GardenTreasureConfigManager.Instance.GetBoardData(GardenTreasureModel.Instance.GardenTreasure.BoardId);

        if(_currentBoard != null)
            _currentBoard._gameObject.SetActive(false);
        
        if (_boardGroups.ContainsKey(boardData._boardSize))
            _currentBoard = _boardGroups[boardData._boardSize];

        if (_currentBoard == null)
        {
            Debug.LogError("---------- 棋盘获取错误 " + boardData._boardSize);
            return;
        }
        
        _currentBoard._gameObject.SetActive(true);
        _currentBoard.Init(config, boardData);

        if (!GuideSubSystem.Instance.isFinished(GuideTriggerPosition.GardenTreasureOpenGrid))
        {
            var fistItem = _currentBoard.GetItem(0, 0);
            List<Transform> topLayer = new List<Transform>();
            topLayer.Add(fistItem._gameObject.transform);
            GuideSubSystem.Instance.RegisterTarget(GuideTargetType.GardenTreasureOpenGrid, fistItem._gameObject.transform as RectTransform, topLayer: topLayer);
            
            GuideSubSystem.Instance.Trigger(GuideTriggerPosition.GardenTreasureOpenGrid, "");
        }
    }
    
    private void DestroyBoard()
    {
        foreach (var kv in _boardGroups)
        {
            kv.Value.Destroy();
        }
        _boardGroups.Clear();
        _boardGroups = null;
    }

    private void ClickAction(BoardItem boardItem, int row, int col)
    {
        int index = row * _currentBoard._size + col;
        
        if(GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Contains(index))
            return;
        
        GuideSubSystem.Instance.FinishCurrent(GuideTargetType.GardenTreasureOpenGrid, null);
        
        if (_isSelcetShovel)
        {
            if (!UserData.Instance.CanAford(UserData.ResourceId.GardenShovel, 1))
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupGardenTreasureNoItem);
                return;
            }
            
            UserData.Instance.ConsumeRes(UserData.ResourceId.GardenShovel, 1, new GameBIManager.ItemChangeReasonArgs());
            _curRow = row;
            _curCol = col;
            _curBoardItem = boardItem;

            GardenTreasureModel.Instance.RecordConsume((int)UserData.ResourceId.GardenShovel);
            
            UpdateValues();
            OpenBoardItem(_curRow, _curCol);
            CleanBoardTags();
            AudioManager.Instance.PlaySound(159);
        }
        else
        {
            if (!UserData.Instance.CanAford(UserData.ResourceId.GardenBomb, 1))
            {
                UIManager.Instance.OpenUI(UINameConst.UIPopupGardenTreasureNoItem);
                return;
            }

            if (_curBoardItem == null)
            {
                _curRow = row;
                _curCol = col;
                _curBoardItem = boardItem;

                ShowBombArea(_curRow, _curCol);
                return;
            }

            if (_curBoardItem != boardItem)
            {
                _curRow = row;
                _curCol = col;
                _curBoardItem = boardItem;

                ShowBombArea(_curRow, _curCol);
                return;
            }
            
            UserData.Instance.ConsumeRes(UserData.ResourceId.GardenBomb, 1, new GameBIManager.ItemChangeReasonArgs());
            UpdateValues();
            OpenBombArea(_curRow, _curCol);
            CleanBoardTags();
        }
    }

    private void CleanBoardTags()
    {
        _currentBoard?.CleanSelectStatus();
        
        _curRow = -1;
        _curCol = -1;
        _curBoardItem = null;
    }
    
    private void ShowBombArea(int row, int col)
    {
        if(_currentBoard == null)
            return;
        
        _currentBoard.CleanSelectStatus();

        List<Vector2Int> gridArea = new List<Vector2Int>();
        for (int i = 0; i < _gridDir.GetLength(0); i++)
        {
            int newRow = row + _gridDir[i, 0];
            int newCol = col + _gridDir[i, 1];
                
            if(newRow < 0 || newRow >= _currentBoard._size)
                continue;
                
            if(newCol < 0 || newCol >= _currentBoard._size)
                continue;

            Vector2Int size = new Vector2Int(newRow, newCol);
            gridArea.Add(size);
        }

        for (int i = 0; i < _currentBoard._size; i++)
        {
            for (int j = 0; j < _currentBoard._size; j++)
            {
                int index = i * _currentBoard._size + j;
                
                if(GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Contains(index))
                    continue;
                
                bool isArea = false;
                foreach (var rc in gridArea)
                {
                    if (i == rc.x && j == rc.y)
                    {
                        isArea = true;
                        break;
                    }
                }
                
                if(isArea)
                    continue;
                
                var item = _currentBoard.GetItem(i, j);
                if(item == null)
                    continue;
                
                item.SetSelectActive(true);
            }
        }
    }

    private void OpenBombArea(int row, int col)
    {
        GardenTreasureModel.Instance.RecordConsume((int)UserData.ResourceId.GardenBomb);
        
        for (int i = 0; i < _gridDir.GetLength(0); i++)
        {
            int newRow = row + _gridDir[i, 0];
            int newCol = col + _gridDir[i, 1];
                
            if(newRow < 0 || newRow >= _currentBoard._size)
                continue;
                
            if(newCol < 0 || newCol >= _currentBoard._size)
                continue;
            
            int index = newRow * _currentBoard._size + newCol;
            if(GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Contains(index))
                continue;
            
            var item = _currentBoard.GetItem(newRow, newCol);
            if(item == null)
                continue;

            OpenBoardItem(newRow, newCol);
        }
        AudioManager.Instance.PlaySound(159);
    }
    private void OpenBoardItem(int row, int col)
    {
        int index = row * _currentBoard._size + col;
        GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Add(index);

        var config = GardenTreasureModel.Instance.GetCurrentLevelConfig();
        List<ResData> resDatas = new List<ResData>();
        for (int j = 0; j < config.RewardId.Count; j++)
        {
            ResData resData = new ResData(config.RewardId[j], config.RewardNum[j]);
            resDatas.Add(resData);
        }
        
        var boardData = GardenTreasureConfigManager.Instance.GetBoardData(GardenTreasureModel.Instance.GardenTreasure.BoardId);
        bool isGetAllShape = true;
        foreach (var shapeData in boardData._shapeDatas)
        {
            if(GardenTreasureModel.Instance.GardenTreasure.GetShapes.Contains(shapeData._index))
                continue;

            bool isGetShape = true;
            foreach (var shapeGrid in shapeData._shapeGrids)
            {
                int gridIndex = shapeGrid.x * _currentBoard._size + shapeGrid.y;
                
                if(GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Contains(gridIndex))
                   continue;

                isGetShape = false;
                isGetAllShape = false;
            }
            
            if(!isGetShape)
                continue;
            
            GardenTreasureModel.Instance.GardenTreasure.GetShapes.Add(shapeData._index);
            
            // if(GardenTreasureModel.Instance.IsRandomLevel())
            //     GardenTreasureLeaderBoardModel.Instance.AddScore();
            
            StartCoroutine(FlyShapeUnit(shapeData._index, () =>
            {
                if (isGetAllShape)
                {
                    PassBoardLogic(resDatas);
                }
            }));
        }

        UpdateProgressAnim(isGetAllShape);
        if (isGetAllShape)
        {
            int levelId = GardenTreasureModel.Instance.GetCurrentLevelId();
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGardenTreasureLevelFinish,
                levelId.ToString(),GardenTreasureModel.Instance.GetEnterCurrentLevelCount().ToString(),GardenTreasureModel.Instance.GetTotalEnterLevelCount().ToString());
            
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGardenTreasureReward,
                levelId.ToString(),GardenTreasureModel.Instance.GetEnterCurrentLevelCount().ToString(),GardenTreasureModel.Instance.GetRecordConsume((int)UserData.ResourceId.GardenShovel)+"_"+GardenTreasureModel.Instance.GetRecordConsume((int)UserData.ResourceId.GardenBomb));

            GetAllRewardLogic(resDatas);

            UIRoot.Instance.EnableEventSystem = false;
        }
        
        BoardItem boardItem = _currentBoard.GetItem(row, col);

        StartCoroutine(OpenBoardItem(boardItem));
    }

    private IEnumerator OpenBoardItem(BoardItem boardItem)
    {
        if(boardItem == null)
            yield break;
        
        PlayEffect(_isSelcetShovel ? EffectType.Shovel : EffectType.Bomb, boardItem.EffectPosition);
        yield return new WaitForSeconds(0.25f);
        
        boardItem.Open();
    }
    
    private IEnumerator FlyShapeUnit(int index, Action flyEndCall)
    {
        yield return new WaitForSeconds(0.3f);
        
        var shapeItem = _currentBoard._shapeGroup.GetShapeItem(index);
        if(shapeItem == null)
            yield break;

        var treasureItem = GetTreasureItem(shapeItem);
        if(treasureItem == null)
            yield break;

        ChangeAnchorPivot((RectTransform)(shapeItem._gameObject.transform));
        
        shapeItem._gameObject.transform.parent = shapeItem._gameObject.transform.parent.parent;
        shapeItem._gameObject.transform.SetAsLastSibling();
        
        var shapeConfig = GardenTreasureConfigManager.Instance.GardenTreasureShapeConfigList.Find(a => a.Id.ToString() == shapeItem._shapeData._shapeSize);

        float scaleTime = 0.3f;
        float moveTime = 1f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(shapeItem._gameObject.transform.DOScale(1.3f, scaleTime));
        sequence.Join(shapeItem._gameObject.transform.DORotate(new Vector3(0, 0, shapeConfig.RotateZ), scaleTime));
        sequence.AppendInterval(0.1f);
        sequence.Append(shapeItem._gameObject.transform.DOShakeRotation(0.3f, new Vector3(0,0, 30), 20, fadeOut:false));
        sequence.Append(shapeItem._gameObject.transform.DOMove(treasureItem._gameObject.transform.position, moveTime).SetEase(Ease.Linear));
        sequence.Join(shapeItem._gameObject.transform.DOScale(1.8f, 0.6f).SetEase(Ease.Linear));
        sequence.Insert(scaleTime+0.1f+0.3f+0.6f, shapeItem._gameObject.transform.DOScale(1f, 0.4f).SetEase(Ease.Linear));

        yield return new WaitForSeconds(scaleTime + moveTime+0.2f);
        shapeItem.SetActive(false);
        treasureItem.UpdateStatus(true, false);
        AudioManager.Instance.PlaySound(160);
        
        yield return new WaitForSeconds(0.2f);
        flyEndCall?.Invoke();
    }

    private void ChangeAnchorPivot(RectTransform rectTransform)
    {
        Vector2 center = new Vector2(0.5f, 0.5f);
        
        Vector2 oldAnchoredPosition = rectTransform.anchoredPosition;
        Vector2 oldAnchorMin = rectTransform.anchorMin;
        Vector2 oldAnchorMax = rectTransform.anchorMax;
        Vector2 oldPivot = rectTransform.pivot;

        RectTransform parentRectTransform = rectTransform.parent as RectTransform;
        Vector2 parentSize = parentRectTransform != null ? parentRectTransform.rect.size : Vector2.zero;

        Vector2 oldAnchorCenter = (oldAnchorMin + oldAnchorMax) * 0.5f;
        Vector2 newAnchorCenter = (center + center) * 0.5f;
        Vector2 anchorDelta = newAnchorCenter - oldAnchorCenter;

        Vector2 pivotDelta = oldPivot - center;
        Vector2 pivotOffset = new Vector2(
            pivotDelta.x * rectTransform.rect.width,
            pivotDelta.y * rectTransform.rect.height
        );

        Vector2 totalOffset = new Vector2(
            anchorDelta.x * parentSize.x + pivotOffset.x,
            anchorDelta.y * parentSize.y + pivotOffset.y
        );

        rectTransform.anchorMin = center;
        rectTransform.anchorMax = center;
        rectTransform.pivot = center;

        rectTransform.anchoredPosition = oldAnchoredPosition - totalOffset;
    }
    private void GetAllRewardLogic(List<ResData> resDatas)
    {
        UserData.Instance.AddRes(resDatas, new GameBIManager.ItemChangeReasonArgs()
        {
            reason = BiEventAdventureIslandMerge.Types.ItemChangeReason.GardenTreasureGet
        }, false);
        
        foreach (var resData in resDatas)
        {
            if(UserData.Instance.IsResource(resData.id))
                continue;
            
            GameBIManager.Instance.SendMergeEvent(new GameBIManager.MergeEventArgs
            {
                MergeEventType = BiEventAdventureIslandMerge.Types.MergeEventType.MergeChangeReasonGardenTreasureGet,
                itemAId =resData.id,
                isChange = true,
            });
        }

        GardenTreasureModel.Instance.GardenTreasure.ShowLevelId++;
        GardenTreasureModel.Instance.GardenTreasure.GetShapes.Clear();
        GardenTreasureModel.Instance.GardenTreasure.OpenGrids.Clear();
            
        if (GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel)
        {
            int loopNum = 0;
            while (loopNum < 20)
            {
                int id = GardenTreasureConfigManager.Instance._randomLevelConfig.BoardConfigIds.Random();
                loopNum++;
                
                if(GardenTreasureModel.Instance.GardenTreasure.BoardId == id)
                    continue;

                GardenTreasureModel.Instance.GardenTreasure.BoardId = id;
                break;
            }
        }
        else
        {
            int levelId = GardenTreasureModel.Instance.GardenTreasure.NormalLevelId;
            if (levelId == GardenTreasureConfigManager.Instance._normalLevelConfigs[GardenTreasureConfigManager.Instance._normalLevelConfigs.Count - 1].Id)
            {
                GardenTreasureModel.Instance.SetActivityStatus(I_ActivityStatus.ActivityStatus.Completed);
                GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel = true;
                GardenTreasureModel.Instance.GardenTreasure.BoardId = GardenTreasureConfigManager.Instance._randomLevelConfig.BoardConfigIds.Random();
            }
            else
            {
                int findIndex = GardenTreasureConfigManager.Instance._normalLevelConfigs.FindIndex(a => a.Id == levelId);
                var config = GardenTreasureConfigManager.Instance._normalLevelConfigs[findIndex + 1];
                GardenTreasureModel.Instance.GardenTreasure.NormalLevelId = config.Id;
                GardenTreasureModel.Instance.GardenTreasure.BoardId = config.BoardConfigIds.Random();
            }
        }
        
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventGardenTreasureCreateBoard, GardenTreasureModel.Instance.GetCurrentLevelId().ToString(), GardenTreasureModel.Instance.GardenTreasure.BoardId.ToString());

    }
    private void PassBoardLogic(List<ResData> resDatas)
    {
        UIRoot.Instance.EnableEventSystem = true;
        PlayCloseAnimation(null);
        
        CommonRewardManager.Instance.PopCommonReward(resDatas, CurrencyGroupManager.Instance.currencyController, false, animEndCall:()=>
        {
            UIRoot.Instance.EnableEventSystem = true;

            if (GardenTreasureModel.Instance.GardenTreasure.IsRandomLevel)
            {
                AnimCloseWindow();
                return;
            }
            
            InitBoard();
            InitTreasure();
            UpdateRewardStatus();
            PlayOpenAnimation(null);
            UpdateValues();
        });
    }
}