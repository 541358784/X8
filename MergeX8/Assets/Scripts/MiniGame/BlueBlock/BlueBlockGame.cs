using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using MagneticScrollView;
using Mosframe;
using Psychology;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BlueBlockGame:MonoBehaviour
{
    private Camera MainCamera => UIRoot.Instance.mUICamera;
    public TableBlueBlockLevel CurLevelConfig;
    public BlueBlockType[][] BackGroundState;
    public List<BlueBlockPuzzle> PuzzleList = new List<BlueBlockPuzzle>();
    public Transform DefaultRedShadowBlock;
    public Transform DefaultGreenShadowBlock;
    public List<Transform> RedShadowList = new List<Transform>();
    public List<Transform> GreenShadowList = new List<Transform>();
    public List<Transform> RedShadowPool = new List<Transform>();
    public List<Transform> GreenShadowPool = new List<Transform>();
    public Transform MapTransform;
    public Action<BlueBlockGame> WinCallback;
    public void CheckWin()
    {
        foreach (var puzzle in PuzzleList)
        {
            if (!puzzle.InMap)
                return;
        }
        WinCallback(this);
    }
    public void InitGame(int level,Action<BlueBlockGame> winCallback)
    {
        WinCallback = winCallback;
        var levelConfig = PsychologyConfigManager.Instance.BlueBlockLevelConfigs.Find(a => a.id == level);
        if (levelConfig == null)
            return;
        CurLevelConfig = levelConfig;
        var bgShapeConfig =
            PsychologyConfigManager.Instance.BlueBlockShapeConfigs.Find(a => a.id == CurLevelConfig.map);
        BlueBlockUtils.BuildShapeState(bgShapeConfig, out BackGroundState);
        MapTransform = transform.Find(bgShapeConfig.id.ToString());
        {
            var sortingGroup = MapTransform.gameObject.AddComponent<SortingGroup>();
            sortingGroup.enabled = true;
            sortingGroup.sortingOrder = 0;
        }
        for (var i = 0; i < CurLevelConfig.shapes.Length; i++)
        {
            var puzzleShapeConfig =
                PsychologyConfigManager.Instance.BlueBlockShapeConfigs.Find(a => a.id == CurLevelConfig.shapes[i]);
            var puzzle = transform.Find(puzzleShapeConfig.id.ToString()).gameObject.AddComponent<BlueBlockPuzzle>();
            var nameKey = CurLevelConfig.shapeNames[i];
            //var name = LocalizationManager.Instance.GetLocalizedString(nameKey);
            var text = puzzle.transform.Find("Text");
            if (text)
            {
                var textProMesh = text.gameObject.GetComponent<LocalizeTextMeshPro>();
                if (textProMesh)
                {
                    textProMesh.SetTerm(nameKey);
                }
            }
            puzzle.InitPuzzle(puzzleShapeConfig,this);
            PuzzleList.Add(puzzle);
            {
                var sortingGroup = puzzle.gameObject.AddComponent<SortingGroup>();
                sortingGroup.enabled = true;
                sortingGroup.sortingOrder = 2;
            }
        }

        DefaultRedShadowBlock = transform.Find("ColorLayer/Red");
        DefaultRedShadowBlock.gameObject.SetActive(false);
        {
            var sortingGroup = DefaultRedShadowBlock.gameObject.AddComponent<SortingGroup>();
            sortingGroup.enabled = true;
            sortingGroup.sortingOrder = 1;
        }
        DefaultGreenShadowBlock = transform.Find("ColorLayer/Green");
        DefaultGreenShadowBlock.gameObject.SetActive(false);
        {
            var sortingGroup = DefaultGreenShadowBlock.gameObject.AddComponent<SortingGroup>();
            sortingGroup.enabled = true;
            sortingGroup.sortingOrder = 1;
        }
    }

    private BlueBlockPuzzle MovingContainer;
    private int LastTouchID;
    private void Update()
    {
        if (Application.isEditor)
        {
            if (MovingContainer is null)
            {
                if (!Input.GetMouseButtonDown(0))//第一次按下会查看是否摁在指定范围
                    return;
                Vector2 touchPosition = Input.mousePosition;
                Vector3 touchWorldPosition = MainCamera.ScreenToWorldPoint(touchPosition);
                touchWorldPosition.z = 0;
                foreach (var itemGroupContainer in PuzzleList)
                {
                    if (itemGroupContainer.ContainsPosition(touchWorldPosition))
                    {
                        LastTouchID = 0;
                        StartMoving(itemGroupContainer,touchWorldPosition);
                        return;
                    }
                }
                return;
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    var tempMovingContainer = MovingContainer;
                    MovingContainer = null;
                    LastTouchID = -1;
                    EndMoving(tempMovingContainer);
                    return;
                }
                else
                {
                    Vector2 touchPosition = Input.mousePosition;
                    Vector3 touchWorldPosition = MainCamera.ScreenToWorldPoint(touchPosition);
                    touchWorldPosition.z = 0;
                    FollowPointer(MovingContainer,touchWorldPosition);
                    return;
                }
            }   
        }
        else
        {
            int touchCount = Input.touchCount;
            if (MovingContainer is null)
            {
                if (touchCount == 0)
                    return;
                for (int i = 0; i < touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase != TouchPhase.Began)
                    {
                        continue;
                    }
                    Vector2 touchPosition = touch.position;
                    Vector3 touchWorldPosition = MainCamera.ScreenToWorldPoint(touchPosition);
                    touchWorldPosition.z = 0;
                    foreach (var itemGroupContainer in PuzzleList)
                    {
                        if (itemGroupContainer.ContainsPosition(touchWorldPosition))
                        {
                            LastTouchID = touch.fingerId;
                            StartMoving(itemGroupContainer,touchWorldPosition);
                            return;
                        }
                    }
                }
                return;
            }
            else
            {
                for (int i = 0; i < touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.fingerId == LastTouchID && touch.phase != TouchPhase.Ended)
                    {
                        Vector2 touchPosition = touch.position;
                        Vector3 touchWorldPosition = MainCamera.ScreenToWorldPoint(touchPosition);
                        touchWorldPosition.z = 0;
                        FollowPointer(MovingContainer,touchWorldPosition);
                        return;
                    }
                }
                var tempMovingContainer = MovingContainer;
                MovingContainer = null;
                LastTouchID = -1;
                EndMoving(tempMovingContainer);
                return;
            }   
        }
    }

    public void FollowPointer(BlueBlockPuzzle puzzle,Vector3 pointerPosition)
    {
        puzzle.transform.DOKill(false);
        puzzle.transform.DOScale(Vector3.one, 0.1f);
        var targetPosition = pointerPosition - PointerOffset.MultiVec3(puzzle.transform.lossyScale);
        puzzle.transform.DOMove(targetPosition, 0.1f);
        SetShadow(puzzle);
    }

    private Vector3 PointerOffset;
    public void StartMoving(BlueBlockPuzzle puzzle, Vector3 pointerPosition)
    {
        var mainUI =
            UIManager.Instance.GetOpenedUIByPath<UIBlueBlockMainController>(UINameConst.UIBlueBlockMain);
        if (mainUI)
            mainUI.RemoveAllHandGuide();
        MovingContainer = puzzle;
        // PointerOffset = pointerPosition - MovingContainer.transform.position;
        PointerOffset = new Vector3((puzzle.Config.width/2f - 0.5f) * BlockSizeX, -(puzzle.Config.height/2f-0.5f) * BlockSizeY-2f, 0);
        AudioManager.Instance.PlaySound("sfx_blueprint_pickup");
        if (!puzzle.InMap)
        {
            puzzle.transform.DOKill(false);
            puzzle.transform.DOScale(Vector3.one, 0.2f);
            // puzzle.transform.localScale = Vector3.one;
        }
        puzzle.Shadow.gameObject.SetActive(true);
        ReducePuzzle(puzzle);
        FollowPointer(puzzle, pointerPosition);
        {
            var sortingGroup = puzzle.gameObject.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 3;
        }
    }
    public void EndMoving(BlueBlockPuzzle puzzle)
    {
        AudioManager.Instance.PlaySound("sfx_blueprint_putdown");
        //默认每一块shape都以左上角第一个格子中心为坐标原点
        var rootPosition = puzzle.transform.localPosition - MapTransform.transform.localPosition;
        
        var xOffset = (int)((rootPosition.x + BlockSizeX / 2) / BlockSizeX);
        var yOffset = (int)((-rootPosition.y + BlockSizeY / 2) / BlockSizeY);//Y轴使用负方向
        var canSet = CanPuzzleSet(puzzle, xOffset, yOffset);
        puzzle.Shadow.gameObject.SetActive(false);
        HideShadow();
        {
            var sortingGroup = puzzle.gameObject.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 2;
        }
        if (canSet)
        {
            var tempPos = puzzle.transform.localPosition;
            tempPos.x = xOffset * BlockSizeX + MapTransform.transform.localPosition.x;
            tempPos.y = -(yOffset * BlockSizeY) + MapTransform.transform.localPosition.y;
            // puzzle.transform.localPosition = tempPos;
            puzzle.transform.DOKill(false);
            puzzle.transform.DOLocalMove(tempPos, 0.1f);
            puzzle.transform.DOScale(Vector3.one, 0.1f);
            AddPuzzle(puzzle,xOffset,yOffset);
            CheckWin();
        }
        else
        {
            // puzzle.transform.localPosition = puzzle.InitPosition;
            puzzle.transform.DOKill(false);
            puzzle.transform.DOScale(puzzle.InitScale, 0.2f);
            puzzle.transform.DOLocalMove(puzzle.InitPosition, 0.2f);
        }
    }
    
    public void HideShadow()
    {
        foreach (var shadow in RedShadowList)
        {
            shadow.gameObject.SetActive(false);
        }
        RedShadowPool.AddRange(RedShadowList);
        RedShadowList.Clear();
        foreach (var shadow in GreenShadowList)
        {
            shadow.gameObject.SetActive(false);
        }
        GreenShadowPool.AddRange(GreenShadowList);
        GreenShadowList.Clear();
    }
    public void SetShadow(BlueBlockPuzzle puzzle)
    {
        HideShadow();
        var rootPosition = puzzle.transform.localPosition - MapTransform.transform.localPosition;
        var xOffset = (int)((rootPosition.x + BlockSizeX / 2) / BlockSizeX);
        var yOffset = (int)((-rootPosition.y + BlockSizeY / 2) / BlockSizeY);//Y轴使用负方向
        var canSet = CanPuzzleSet(puzzle, xOffset, yOffset);
        var pool = canSet ? GreenShadowPool : RedShadowPool;
        var visibleList = canSet ? GreenShadowList : RedShadowList;
        var defaultShadowItem = canSet ? DefaultGreenShadowBlock : DefaultRedShadowBlock;
        for (var x = 0; x < puzzle.State.Length; x++)
        {
            var mapX = x + xOffset;
            if (mapX < 0 || mapX >= BackGroundState.Length)
                continue;
            for (var y = 0; y < puzzle.State[x].Length; y++)
            {
                var targetState = puzzle.State[x][y];
                if (targetState == BlueBlockType.Empty)
                    continue;
                var mapY = y + yOffset;
                if (mapY < 0 || mapY >= BackGroundState[mapX].Length)
                    continue;
                var mapState = BackGroundState[mapX][mapY];
                if (mapState == BlueBlockType.Empty)
                    continue;
                Transform shadowItem = null;
                if (pool.Count == 0)
                {
                    shadowItem = Instantiate(defaultShadowItem, defaultShadowItem.parent);
                }
                else
                {
                    shadowItem = pool.Pop();
                }
                shadowItem.gameObject.SetActive(true);
                var tempPos = shadowItem.localPosition;
                tempPos.x = mapX * BlockSizeX + MapTransform.transform.localPosition.x - shadowItem.parent.localPosition.x;
                tempPos.y = -(mapY * BlockSizeY) + MapTransform.transform.localPosition.y - shadowItem.parent.localPosition.y;
                shadowItem.localPosition = tempPos;
                visibleList.Add(shadowItem);
            }
        }
    }

    public bool CanPuzzleSet(BlueBlockPuzzle puzzle,int xOffset,int yOffset)
    {
        for (var x = 0; x < puzzle.State.Length; x++)
        {
            for (var y = 0; y < puzzle.State[x].Length; y++)
            {
                var targetState = puzzle.State[x][y];
                if (targetState == BlueBlockType.Empty)
                    continue;
                var mapX = x + xOffset;
                var mapY = y + yOffset;
                if (mapX < 0 || mapX >= BackGroundState.Length)
                    return false;
                if (mapY < 0 || mapY >= BackGroundState[mapX].Length)
                    return false;
                var mapState = BackGroundState[mapX][mapY];
                if (!CanBlockSet(mapState, targetState))
                    return false;
            }
        }
        return true;
    }

    public void AddPuzzle(BlueBlockPuzzle puzzle,int xOffset,int yOffset)
    {
        if (puzzle.InMap)
            return;
        for (var x = 0; x < puzzle.State.Length; x++)
        {
            var mapX = x + xOffset;
            if (mapX < 0 || mapX >= BackGroundState.Length)
                continue;
            for (var y = 0; y < puzzle.State[x].Length; y++)
            {
                var mapY = y + yOffset;
                if (mapY < 0 || mapY >= BackGroundState[mapX].Length)
                    continue;
                var targetState = puzzle.State[x][y];
                BackGroundState[mapX][mapY] = BackGroundState[mapX][mapY].Add(targetState);
            }
        }
        puzzle.InMap = true;
        puzzle.PosX = xOffset;
        puzzle.PosY = yOffset;
    }

    public void ReducePuzzle(BlueBlockPuzzle puzzle)
    {
        if (!puzzle.InMap)
            return;
        var xOffset = puzzle.PosX;
        var yOffset = puzzle.PosY;
        for (var x = 0; x < puzzle.State.Length; x++)
        {
            var mapX = x + xOffset;
            if (mapX < 0 || mapX >= BackGroundState.Length)
                continue;
            for (var y = 0; y < puzzle.State[x].Length; y++)
            {
                var mapY = y + yOffset;
                if (mapY < 0 || mapY >= BackGroundState[mapX].Length)
                    continue;
                var targetState = puzzle.State[x][y];
                BackGroundState[mapX][mapY] = BackGroundState[mapX][mapY].Reduce(targetState);
            }
        }
        puzzle.InMap = false;
        puzzle.PosX = 0;
        puzzle.PosY = 0;
    }

    public bool CanBlockSet(BlueBlockType back,BlueBlockType target)
    {
        if (target == BlueBlockType.Empty)
            return true;
        if (back == BlueBlockType.Normal)
            return true;
        return back == target;
    }

    public const float BlockSizeX = 1f;
    public const float BlockSizeY = 1f;
}

public class BlueBlockPuzzle : MonoBehaviour
{
    public BlueBlockType[][] State;
    public bool InMap;
    public int PosX;
    public int PosY;
    public Vector3 InitPosition;
    public Vector3 InitScale;
    public List<Transform> BlockList = new List<Transform>();
    public BlueBlockGame Game;
    public Transform Shadow;
    public TableBlueBlockShape Config;
    public Color shadowColor = new Color(0.5f, 0.5f, 0.5f, 0.3f); // 灰色，50% 透明度
    public Vector2 shadowOffset = new Vector2(1, -1); // 偏移量
    private Material shadowMaterial;
    public void InitPuzzle(TableBlueBlockShape shapeConfig,BlueBlockGame game)
    {
        Game = game;
        Config = shapeConfig;
        BlueBlockUtils.BuildShapeState(shapeConfig, out State);
        InitPosition = transform.localPosition;
        InitScale = transform.localScale;
        InMap = false;
        var children = transform.childCount;
        for (var i = 0; i<children; i++)
        {
            BlockList.Add(transform.GetChild(i));
        }
        Shadow = new GameObject("Shadow").transform;
        Shadow.SetParent(transform,false);
        Shadow.localPosition = Vector3.zero;
        Shadow.gameObject.AddComponent<SortingGroup>().sortingOrder = -1;
        var imageList = transform.GetComponentsInChildren<SpriteRenderer>(true);
        if (imageList.Length > 0)
        {
            shadowMaterial = new Material(Shader.Find("Custom/GrayShadow"));
            shadowMaterial.SetColor("_GrayColor", shadowColor);
            shadowMaterial.renderQueue = imageList[0].material.renderQueue;
            foreach (var image in imageList)
            {
                var shadowImage = Instantiate(image.gameObject, Shadow);
                shadowImage.GetComponent<SpriteRenderer>().material = shadowMaterial;
            }   
        }
        Shadow.localPosition = new Vector3(0.2f, -0.2f, 0);
        Shadow.gameObject.SetActive(false);
        Shadow.gameObject.setLayer(5,true);
    }

    // private float SelectDistance = 0.2f;
    public bool ContainsPosition(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        foreach (var item in BlockList)
        {
            var leftX = item.localPosition.x - BlueBlockGame.BlockSizeX/2;
            var rightX = item.localPosition.x + BlueBlockGame.BlockSizeX/2;
            var upY = item.localPosition.y + BlueBlockGame.BlockSizeY/2;
            var downY = item.localPosition.y - BlueBlockGame.BlockSizeY/2;
            if (position.x > leftX && position.x < rightX &&
                position.y > downY && position.y < upY)

            {
                return true;
            }
        }
        return false;
    }
}
