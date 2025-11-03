using System;
using System.Collections.Generic;
using Deco.Node;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Farm.Model;
using UnityEngine;

namespace Farm.Logic
{
    public class TreeLogic : MonoBehaviour, ILogic
    {
        public DecoNode _node { get; set; }
        public FarmType _type { get; set; }
        public bool _isInit { get; set; }
        public GameObject _root{ get; set; }

        private int _status = -1;

        private List<int> _stages = null;

        private StorageTree _storage;

        private TableFarmTree _config;

        private List<Transform> _stageObjects = new List<Transform>();

        private string[] _stageNames = new[]
        {
            "01",
            "02",
            "03",
        };

        private void Awake()
        {
            _isInit = false;
            InvokeRepeating("InvokeUpdate", 0, 1);
        }

        public void Init(Transform root, DecoNode node, FarmType type)
        {
            _isInit = true;
            _node = node;
            _type = type;

            _root = root.gameObject;
            _stages = FarmConfigManager.Instance.TableFarmSettingList[0].TreeStages;

            InitConfig();
            _status = -1;
            
            InitStages();
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            InitConfig();
            InitStatus();
        }

        private void InitConfig()
        {
            _storage = FarmModel.Instance.GetStorageTree(_node);
            _config = FarmConfigManager.Instance.GetFarmTreeConfig(_node.Id);
        }
        
        public void Select()
        {
        }

        public void UnSelect()
        {
        }
        
        private bool InitStatus()
        {
            long diffTime = _storage.RipeningTime - (long)APIManager.Instance.GetServerTime();
            int newStatus = -1;
            
            if (diffTime < 0)
            {
                newStatus = _stages.Count-1;
            }
            else
            {
                int ratio = (int)(1.0f * diffTime / (_config.RipeningTime * 1000)*100);
                ratio = 100 - ratio;
                
                for (int i = _stages.Count-1; i >=0 ; i--)
                {
                    if (ratio >= _stages[i])
                    {
                        newStatus = i;
                        break;
                    }
                }
            }

            if (newStatus == _status)
                return false;

            _status = newStatus;

            for (int i = 0; i < _stageObjects.Count; i++)
            {
                if(_stageObjects[i] == null)
                    continue;
                
                _stageObjects[i].gameObject.SetActive(i == _status);
            }

            return true;
        }
        
        private void InitStages()
        {
            if(_stageObjects.Count > 0)
                return;
            
            foreach (var stageName in _stageNames)
            {
                _stageObjects.Add(_root.transform.Find(stageName));
            }
            
            _stageObjects.ForEach(a =>
            {
                if(a != null)
                    a.gameObject.SetActive(false);
            });
        }
        
        private void InvokeUpdate()
        {
            if(!_isInit)
                return;
            
            if(_root == null)
                return;

            InitStatus();
        }
    }
}