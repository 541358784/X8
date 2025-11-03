using System;
using System.Collections.Generic;
using System.Linq;
using DragonPlus;
using DragonPlus.Config.ClimbTower;
using UnityEngine;

namespace ActivityLocal.ClimbTower.Model
{
    // public class LevelGroup
    // {
    //     public int _index;
    //     public Transform _root;
    //     public Transform _moveNode;
    //     public Vector3 _startPosition;
    //         
    //     public List<LevelCell> _levelCells = new List<LevelCell>();
    //     
    //     public LevelGroup(int index, Transform root)
    //     {
    //         _root = root;
    //         _index = index;
    //             
    //         var cell = root.Find("Lv/Content/1");
    //         cell.gameObject.SetActive(false);
    //         
    //         _moveNode = cell.parent;
    //         _startPosition = cell.parent.transform.localPosition;
    //             
    //         int i = 0;
    //         foreach (var config in ClimbTowerConfigManager.Instance.GetRewardConfig())
    //         {
    //             var item = GameObject.Instantiate(cell, cell.parent);
    //             item.transform.localScale=Vector3.one;
    //             item.gameObject.SetActive(true);
    //             
    //             _levelCells.Add(item.gameObject.AddComponent<LevelCell>());
    //             _levelCells.Last().InitData(config, ++i);
    //         }
    //     }
    //
    //     public void Refresh()
    //     {
    //         _levelCells.ForEach(a=>a.Refresh());
    //     }
    // }
    
    public class LevelCell : MonoBehaviour
    {
        private TableClimbTowerReward _config;
        private int _index;

        private Transform _normal;

        private LocalizeTextMeshProUGUI _normalText;

        private Animator _animator;
        public Animator Animator => _animator;
        
        private void Awake()
        {
            _animator = transform.GetComponent<Animator>();
        }

        public void InitData(TableClimbTowerReward config, int index)
        {
            _config = config;
            _index = index;
        }
    }
}