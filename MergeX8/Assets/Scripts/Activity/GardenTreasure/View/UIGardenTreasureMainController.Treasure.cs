using System;
using System.Collections.Generic;
using Activity.GardenTreasure.Model;
using DragonPlus.Config.GardenTreasure;
using UnityEngine;

public partial class UIGardenTreasureMainController
{
     public class TreasureItem
     {
          public GameObject _gameObject;
          public string _name;
          public GameObject _normalObject;
          public GameObject _finishObject;
          public int _index = -1;
          private GameObject _finishEffectObject;
          
          public TreasureItem(GameObject gameObject)
          {
               _gameObject = gameObject;
               _name = gameObject.name;
               
               _normalObject = gameObject.transform.Find("Normal").gameObject;
               _finishObject = gameObject.transform.Find("Finish").gameObject;
               _finishEffectObject = gameObject.transform.Find("Finish/FX_confetti_1").gameObject;
               _finishEffectObject.gameObject.SetActive(false);
               
               UpdateStatus(false);
          }

          public void UpdateStatus(bool isFinish, bool isInit = true)
          {
               _normalObject.gameObject.SetActive(!isFinish);
               _finishObject.gameObject.SetActive(isFinish);
               
               if(isInit || !isFinish)
                    return;
               
               _finishEffectObject.gameObject.SetActive(false);
               _finishEffectObject.gameObject.SetActive(isFinish);
          }
     }
     public class TreasureGroup
     {
          public GameObject _gameObject;
          public int _index;
          public List<TreasureItem> _treasureItems = new List<TreasureItem>();

          public TreasureGroup(GameObject gameObject, int index)
          {
               _gameObject = gameObject;
               _index = index;

               for (int i = 0; i < _gameObject.transform.childCount; i++)
               {
                    TreasureItem item = new TreasureItem(_gameObject.transform.GetChild(i).gameObject);
                    _treasureItems.Add(item);
               }
          }

          public void UpdateStatus(int index, bool isFinish)
          {
               foreach (var treasureItem in _treasureItems)
               {
                    if (index < 0)
                    {
                         treasureItem.UpdateStatus(isFinish);
                         continue;
                    }
                    
                    if(treasureItem._index != index)
                         continue;
                    treasureItem.UpdateStatus(isFinish);
               }
          }

          public void AdaptIndex(GardenTreasureConfigManager.BoardData boardData)
          {
               _treasureItems.ForEach(a => a._index = -1);
               
               foreach (var shapeData in boardData._shapeDatas)
               {
                    bool isGet = GardenTreasureModel.Instance.GardenTreasure.GetShapes.Contains(shapeData._index);
                    if(!isGet)
                         continue;
                    
                    foreach (var treasureItem in _treasureItems)
                    {
                         if(treasureItem._index >= 0)
                              continue;

                         string shapeName1 = shapeData._size.x + "" + shapeData._size.y;
                         string shapeName2 = shapeData._size.y + "" + shapeData._size.x;
                         if (treasureItem._name == shapeName1 || treasureItem._name == shapeName2)
                         {
                              treasureItem._index = shapeData._index;
                              break;
                         }
                    }
               }
          }

          public TreasureItem GetTreasureItem(ShapeItem item)
          {
               foreach (var treasureItem in _treasureItems)
               {
                    if(treasureItem._index >= 0)
                         continue;

                    string shapeName1 = item._shapeData._size.x + "" + item._shapeData._size.y;
                    string shapeName2 = item._shapeData._size.y + "" + item._shapeData._size.x;
                    if (treasureItem._name == shapeName1 || treasureItem._name == shapeName2)
                    {
                         treasureItem._index = item._shapeData._index;
                         return treasureItem;
                    }
               }
               
               return null;
          }
     }

     public List<TreasureGroup> _treasureGroups = new List<TreasureGroup>();
     private TreasureGroup _currentTreasureGroup;

     private Animator _animator;

     private string[] _normalPath = new[]
     {
          "Root/Treasure/Root/BGNormal",
          "Root/Treasure/Root/CloseNormal",
          "Root/Treasure/Root/OpenNormal"
     };
     
     
     private string[] _randomPath = new[]
     {
          "Root/Treasure/Root/BGGold",
          "Root/Treasure/Root/CloseGold",
          "Root/Treasure/Root/OpenGold"
     };
     
     private List<GameObject> _normalObjs = new List<GameObject>(); 
     private List<GameObject> _randomObjs = new List<GameObject>(); 
     private void AwakeTreasure()
     {
          for (int i = 1; i < 20; i++)
          {
               var child = transform.Find($"Root/Treasure/Root/{i}");
               if(child == null)
                    break;

               TreasureGroup group = new TreasureGroup(child.gameObject, i-1);
               _treasureGroups.Add(group);
               
               group._gameObject.SetActive(false);
          }

          int randomIndex = 99;
          var randomChild = transform.Find($"Root/Treasure/Root/{randomIndex}");
          TreasureGroup randomGroup = new TreasureGroup(randomChild.gameObject, randomIndex);
          randomGroup._gameObject.SetActive(false);
          _treasureGroups.Add(randomGroup);

          foreach (var path in _normalPath)
          {
               _normalObjs.Add(transform.Find(path).gameObject);
          }
         
          foreach (var path in _randomPath)
          {
               _randomObjs.Add(transform.Find(path).gameObject);
          }
          
          _animator = transform.Find($"Root/Treasure").GetComponent<Animator>();
     }

     private void InitTreasure()
     {
          _treasureGroups.ForEach(a=>a._gameObject.SetActive(false));
          _normalObjs.ForEach(a=>a.gameObject.SetActive(false));
          _randomObjs.ForEach(a=>a.gameObject.SetActive(false));

          List<GameObject> objs = _normalObjs;
          if (GardenTreasureModel.Instance.IsRandomLevel())
               objs = _randomObjs;
          
          objs.ForEach(a=>a.gameObject.SetActive(true));
          
          _currentTreasureGroup = _treasureGroups[_treasureGroups.Count - 1];
          if (!GardenTreasureModel.Instance.IsRandomLevel())
          {
               int index = GardenTreasureModel.Instance.GardenTreasure.NormalLevelId - 1;
               _currentTreasureGroup = _treasureGroups[index];
          }
          
          _currentTreasureGroup._gameObject.SetActive(true);
          _currentTreasureGroup.UpdateStatus(-1, false);
          
          var boardData = GardenTreasureConfigManager.Instance.GetBoardData(GardenTreasureModel.Instance.GardenTreasure.BoardId);
          _currentTreasureGroup.AdaptIndex(boardData);
          
          foreach (var shapeData in boardData._shapeDatas)
          {
               _currentTreasureGroup.UpdateStatus(shapeData._index, GardenTreasureModel.Instance.GardenTreasure.GetShapes.Contains(shapeData._index));
          }
     }

     public TreasureItem GetTreasureItem(ShapeItem item)
     {
          if (_currentTreasureGroup == null)
               return null;

          return _currentTreasureGroup.GetTreasureItem(item);
     }

     private void PlayCloseAnimation(Action call)
     {
          StartCoroutine(CommonUtils.PlayAnimation(_animator, "close", "", call));
     }
     private void PlayOpenAnimation(Action call)
     {
          StartCoroutine(CommonUtils.PlayAnimation(_animator, "appear", "", call));
     }
}