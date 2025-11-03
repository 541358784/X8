using System.Collections.Generic;
using Decoration;
using DragonPlus.Config.MiniGame;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Video;

namespace fsm_new
{
    public partial class AsmrLevel
    {
        public Camera camera;
        public AsmrLevelConfig Config;

        private AsmrFsm<AsmrState_Base> _fsm;

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
        public VideoPlayer VideoPlayer;
        public AudioSource VideoAudioSource;

        public AsmrLevel(GameObject obj)
        {
            gameObject = obj;
        }

        public void Init(AsmrLevelConfig levelConfig)
        {
            Config = levelConfig;

            _toolInitPos = new();
            _toolInitRot = new();

            InitGroups();

            camera = transform.Find("Camera").GetComponent<Camera>();
            
            if (Config.CameraInitPos != null && Config.CameraInitPos.Count >= 2) camera.transform.position = new Vector3(Config.CameraInitPos[0], Config.CameraInitPos[1], -20);
            VideoPlayer = transform.Find("video")?.GetComponent<VideoPlayer>();
            VideoAudioSource = transform.Find("audio")?.GetComponent<AudioSource>();

            gameObject_fake = GameObject.Instantiate(gameObject);
            gameObject_fake.SetActive(false);

            _fsm = new AsmrFsm<AsmrState_Base>();
            _fsm.ChangeState<AsmrState_Intro>(new AsmrStateParamBase(this, _fsm));

            InitMonos(gameObject);
        }

        public void Release()
        {
            if (VideoPlayer) Resources.UnloadAsset(VideoPlayer.clip);

            GameObject.Destroy(gameObject);
            GameObject.Destroy(gameObject_fake);
        }

        private void InitMonos(GameObject root)
        {
            Animator = root.GetComponent<Animator>();

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