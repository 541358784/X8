using System.Collections;
using Deco.Node;
using DragonPlus.Config.Farm;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Storage;
using Farm.Model;
using Spine.Unity;
using UnityEngine;

namespace Farm.Logic
{
    public class MachineLogic : MonoBehaviour, ILogic
    {
        public DecoNode _node { get; set; }
        public FarmType _type { get; set; }
        public bool _isInit { get; set; }
        public GameObject _root{ get; set; }
        
        private StorageMachine _storage;

        private TableFarmMachine _config;

        private string[] _statusNames = new[]
        {
            "Normal",
            "finish"
        };

        private string _status = "";
        
        private SkeletonAnimation[] _skeletonAnimations;
        
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

            InitConfig();
            InitSkeleton();
            UpdateStatus();
        }
        public void UpdateStatus()
        {
            InitConfig();
            InitStatus();
        }
        private void InitSkeleton()
        {
            _skeletonAnimations = null;
            _skeletonAnimations = _root.transform.gameObject.GetComponentsInChildren<SkeletonAnimation>();
        }
        
        private void InitConfig()
        {
            _storage = FarmModel.Instance.GetStorageMachine(_node);
            _config = FarmConfigManager.Instance.GetFarmMachineConfig(_node.Id);
        }
        public void Select()
        {
        }

        public void UnSelect()
        {
        }      
        
        private void InvokeUpdate()
        {
            if(!_isInit)
                return;
            
            if(_root == null)
                return;

            if(!gameObject.activeSelf)
                return;
            
            InitStatus();
        }     
        
        private bool InitStatus()
        {
            long diffTime = 0;
            
            if(_storage != null)
                diffTime = _storage.RipeningTime - (long)APIManager.Instance.GetServerTime();
            
            string newStatus = "";
            if (diffTime <= 0)
            {
                newStatus = _statusNames[0];
            }
            else
            {
                newStatus = _statusNames[1];
            }

            if (newStatus == _status)
                return false;

            _status = newStatus;

            StopAllCoroutines();
            StartCoroutine(PlaySkeletonAnim(_status));

            return true;
        }
        
        IEnumerator PlaySkeletonAnim(string animName)
        {
            if(_skeletonAnimations == null)
                yield break;

            for (var i = 0; i < _skeletonAnimations.Length; i++)
            {
                PlaySkeletonAnimation(animName, _skeletonAnimations[i]);

                yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            }
        }
        private void PlaySkeletonAnimation(string animName ,SkeletonAnimation skeletonAnimation)
        {
            skeletonAnimation.AnimationState?.SetAnimation(0, animName, true);
            skeletonAnimation.Update(0);
        }
    }
}