using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonU3DSDK;
using Framework;
using SomeWhereTileMatch;
using TileMatch.Game.Block;
using UnityEngine;

namespace TileMatch.Game.Shuffle
{
    public class ShuffleBlockManager : Manager<ShuffleBlockManager>
    {
       private enum ShuffleStatus
       {
          Fly,
          Hide,
          End,
       }
       
       private ShuffleStatus _shuffleStatus = ShuffleStatus.End;
       
       private PathMap _pathMapScript = null;
       private GameObject _pathMapPrefab = null;
       private GameObject _shuffleRoot = null;

       private float _updateTimeStep = 0;
       private float _flySpeedMax = 30;
   
       private List<int> _randomPathIndex = new List<int>();
   
       private List<Block.Block> _shuffleBlocks = null;

       private string _effectPath = "TileMatch/Prefabs/Animationwind";

       public bool IsShuffle()
       {
          return _shuffleStatus != ShuffleStatus.End;
       }
       public void Shuffle(List<Block.Block> blocks)
       {
          AudioManager.Instance.PlaySound(23+TileMatchRoot.AudioDistance);
          _shuffleBlocks = blocks;
          FilterBlock();
          
          InitShuffleRoot();
          InitPathMap();
   
          RandomPathIndex();
          
          _shuffleStatus = ShuffleStatus.Fly;
          
          UpdateLogic();

          UIRoot.Instance.EnableEventSystem = false;
          
          var prefab = DragonU3DSDK.Asset.ResourcesManager.Instance.LoadResource<GameObject>(_effectPath);
          var clonePrefab = Instantiate(prefab);
          CommonUtils.AddChild(TileMatchRoot.Instance.transform, clonePrefab.transform);
          clonePrefab.transform.position = new Vector3(0,1,0);

          CoroutineManager.Instance.StartCoroutine(CommonUtils.DelayWork(3, () =>
          {
               GameObject.DestroyImmediate(clonePrefab);
          }));
       }

       private void FilterBlock()
       {
          for (int i = 0; i < _shuffleBlocks.Count; i++)
          {
             if (_shuffleBlocks[i].IsValidState())
                continue;
             
             _shuffleBlocks.RemoveAt(i);
             i--;
          }
       }
       private void InitShuffleRoot()
       {
          if(_shuffleRoot != null)
             return;
          
          _shuffleRoot = new GameObject("ShuffleRoot");
       }

       private void InitPathMap()
       {
          if(_pathMapPrefab != null)
             DestroyImmediate(_pathMapPrefab);
          
          StopAllCoroutines();
          GameObject pathPfb = DragonU3DSDK.Asset.ResourcesManager.Instance.LoadResource<GameObject>("TileMatch/Prefabs/circular");
          
          _pathMapPrefab = GameObject.Instantiate(pathPfb);
          _pathMapScript = _pathMapPrefab.GetComponent<PathMap>();
          _pathMapPrefab.transform.SetParent(_shuffleRoot.transform);
          _pathMapPrefab.transform.localPosition = Vector3.zero;
          _pathMapPrefab.transform.localScale = Vector3.one;
          
          _pathMapScript.InitPathMoveItems();
       }
    
       public void Update()
       {
            if (_pathMapScript == null)
                return;
      
            switch (_shuffleStatus)
            {
                case ShuffleStatus.Fly:
                    FlyUpdate();
                    break;
            }
      
            if (_pathMapScript.pathMoveItems == null)
                return;
      
            foreach (var kv in _pathMapScript.pathMoveItems)
            {
                kv?.Update();
            }
        }

       private void UpdateLogic()
       {
          switch (_shuffleStatus)
          {
             case ShuffleStatus.Fly:
             {
                _updateTimeStep = 0;
                _shuffleBlocks.ForEach(a=>a.StartShuffle());
                
                TileMatchGameManager.Instance.StartShuffle();
                break;
             }
             case ShuffleStatus.Hide:
             {
                ShuffleBlockStrategy_Default.Shuffle(_shuffleBlocks);
                _shuffleBlocks.ForEach(a=>a.ShuffleRefresh());
                _updateTimeStep = 0;
                StartCoroutine(HideUpdate());
             }
                break;
             case ShuffleStatus.End:
             {
                PathAnimEnd();
                TileMatchGameManager.Instance.StopShuffle();
                break;
             }
          }
       }

       private void PathAnimEnd()
       {
          if (_pathMapScript != null)
          {
             Destroy(_pathMapScript.gameObject);
             _pathMapScript = null;
          }
   
          StopAllCoroutines();
          
          UIRoot.Instance.EnableEventSystem = true;
       }

       private void FlyUpdate()
       {
          int flyIndex = 0;
          foreach (var block in _shuffleBlocks)
          {
             GameObject blockRoot = block._blockView._root;
             Vector3 position = blockRoot.transform.position;
             position.z = -20 + block._blockModel.position.z;
             blockRoot.transform.position = position;
             Vector3 cuttingPos = _pathMapScript.GetPathMovePosition(GetRandomIndex(flyIndex));
             cuttingPos.z = position.z;
             float time = Vector3.Distance(blockRoot.transform.position, cuttingPos)/_pathMapScript.flySpeed;
             blockRoot.transform.DOKill();
             blockRoot.transform.DOMove(cuttingPos, time);
                
             flyIndex++;
          }
   
          _pathMapScript.flySpeed += Time.deltaTime*_pathMapScript.flyStep;
          _pathMapScript.flySpeed = _pathMapScript.flySpeed >= _flySpeedMax ? _flySpeedMax : _pathMapScript.flySpeed;
   
          _updateTimeStep += Time.deltaTime;
          if(_updateTimeStep < _pathMapScript.showTime)
             return;
   
          _updateTimeStep = 0;
          _shuffleStatus = ShuffleStatus.Hide;
          UpdateLogic();
       }
    
       private IEnumerator HideUpdate()
       {
          int flyFinishNum = 0;
          foreach (var block in _shuffleBlocks)
          {
             var animBlock = block;
             GameObject blockRoot = block._blockView._root;
             blockRoot.transform.DOKill();
             blockRoot.transform.DOMove(block._blockModel.position, _pathMapScript.hideTime).OnComplete(() =>
             {
                animBlock.StopShuffle();
                flyFinishNum++;
             });
          }
   
          while (flyFinishNum < _shuffleBlocks.Count)
          {
             yield return null;
          }
   
          _shuffleStatus = ShuffleStatus.End;
          UpdateLogic();
       }

       private void RandomPathIndex()
       {
          _randomPathIndex.Clear();
          List<int> indexList = new List<int>();
          for (int i = 0; i < _pathMapScript.pathMoveItems.Count; i++)
             indexList.Add(i);
   
          for (int i = 0; i < _pathMapScript.pathMoveItems.Count; i++)
          {
             int indx = UnityEngine.Random.Range(0, indexList.Count);
             _randomPathIndex.Add(indx);
             indexList.Remove(indx);
          }
       }

       private int GetRandomIndex(int index)
       {
          if (_randomPathIndex == null || _randomPathIndex.Count == 0)
             return index;
   
          if (index < 0)
             index = 0;
   
          if (index >= _randomPathIndex.Count)
             index = _randomPathIndex.Count - 1;
   
          return _randomPathIndex[index];
       }
    }
}