using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using asmr_new;
using Newtonsoft.Json;
using DG.Tweening;
using DragonPlus.Config.Makeover;
using DragonPlus.Haptics;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniGame
{
    public partial class AsmrLevel
    {
        public Camera camera;
        public TableMoLevel Config;

        private Fsm _fsm;

        private List<AsmrGroup> _asmrGroups;

        public AsmrGroup CurrentGroup => _asmrGroups[_currentGroupIndex];

        private int _currentGroupIndex;

        private GameObject gameObject;
        private GameObject gameObject_fake;
        public Transform transform => gameObject.transform;

        private Transform _guideHandRoot;
        private SkeletonAnimation _guideHandSpine;
        private Transform _doubleHandRoot;


        public Animator Animator;
        public VideoHandler VideoHandler;

        public AsmrLevel(GameObject obj)
        {
            gameObject = obj;
            gameObject_fake = GameObject.Instantiate(gameObject);
            gameObject_fake.SetActive(false);
            
            HapticsManager.Init();
        }

        public void Init(int subID)
        {
            Config = MakeoverConfigManager.Instance.levelList.Find(c => c.subID == subID);
            _toolInitPos = new Dictionary<string, Vector3>();
            _toolInitRot = new Dictionary<string, Quaternion>();
            VideoHandler = new VideoHandler(gameObject.transform, Config);
            
            InitGroups();

            camera = transform.Find("Camera").GetComponent<Camera>();
            camera.transform.position = new Vector3(Config.cameraInitPos[0], Config.cameraInitPos[1], camera.transform.position.z);
            Animator = transform.GetComponent<Animator>();

            _fsm = new Fsm();
            _fsm.ChangeState<AsmrState_Intro>(new AsmrStateParamBase(this));

            InitMonos(gameObject);
        }

        public void Release()
        {
            VideoHandler?.Release();
            VideoHandler = null;
            GameObject.Destroy(gameObject);
            GameObject.Destroy(gameObject_fake);
        }

        private void InitMonos(GameObject root)
        {
            InitSpineAndColliders(root);
            InitToolsAndTarget(root);
            InitTips(root);
        }

        public void Update()
        {
            _fsm.Update();
        }

        public void FixedUpdate()
        {
            _fsm.FixedUpdate(Time.deltaTime);
        }

    }
}