using System;
using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Asset;
using TileMatch.Game;
using UnityEngine;

public class TileMatchRoot : MonoBehaviour
{
    public const int AudioDistance = 207;
    public Camera _matchMainCamera;
    public Camera _matchGuideCamera;
    public static TileMatchRoot Instance;

    private GameObject _matchBoard;
    private void Awake()
    {
        Instance = this;
        
        if(_matchGuideCamera != null)
            _matchGuideCamera.gameObject.SetActive(false);
        
        _matchMainCamera.gameObject.SetActive(false);
        
        DontDestroyOnLoad(this);
        ResourcesManager.Instance.AddAtlasPrefix("TileMatch/");
    }
    public Dictionary<int, int> PropUseState = new Dictionary<int, int>();
    public void LoadBoard(int levelId)
    {
        _matchMainCamera.gameObject.SetActive(true);
        PropUseState.Clear();
        UIManager.Instance.OpenWindow(UINameConst.TileMatchMain);
        
        string boardPath = "TileMatch/Prefabs/TileMatchBoard";
        GameObject boardObj = null;
        if (CommonUtils.IsLE_16_10())
        {
            boardObj = ResourcesManager.Instance.LoadResource<GameObject>(boardPath+"_Pad");
        }
        
        if(boardObj == null)
            boardObj =  ResourcesManager.Instance.LoadResource<GameObject>(boardPath);
        
        _matchBoard = Instantiate(boardObj, transform);
        var bg = _matchBoard.transform.Find("Board/BG");
        if (bg && CommonUtils.IsLE_16_10())
        {
            bg.localScale = new Vector3(1.9f, 1.9f, 1f);
        }
        _matchBoard.AddComponent<TileMatchGameManager>().InitLevel(levelId);
    }

    public void DestroyBoard()
    {
        UIManager.Instance.CloseUI(UINameConst.TileMatchMain,true);
        if(_matchBoard != null)
            GameObject.Destroy(_matchBoard);

        _matchBoard = null;
    }
}
