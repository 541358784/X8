using System;
using System.Collections.Generic;
using DragonPlus;
using DragonPlus.SpineExtensions;
using DragonU3DSDK.Network.API.Protocol;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TMatch
{


    public partial class UIItemBox : UIWindowController,IHospitalState
    {
        public enum BoxType
        {
            Blue = 0,
            Purple = 1,
            Red = 2,
            Green = 3,
            Blue1 = 4,
            Red1 = 5,
            N1Red = 6,
            N1Purple = 7,
            N1Cyan = 8,
            N1Blue = 9,
            N1Yellow = 10,
        }

        public class Data : UIWindowData
        {
            public int BoxNum;
            public BoxType boxType;
            public List<int> ItemIds;
            public List<int> ItemNums;
            public Action OnFinish;
            public Action OnDestroy;
            public Vector3? boxFlyOrgin;
            public bool HasReceive;
            public GameBIManager.ItemChangeReasonArgs? args;
            public bool DefaultItemBar;
        }

        private Data data;
        private float openTime;
        private SkeletonGraphic box;
        private SkeletonGraphic boxRed;
        private GridLayoutGroup layout;
        private Rect rect;
        private bool isInit;
        private CanvasGroup canvasGroup;
        private Button touchButton;

        private Animator animator;

        // private SkeletonGraphic tempBox;
        // private SkeletonGraphic tempRedBox;
        private LocalizeTextMeshProUGUI txtCount;

        private HierarchicalStateMachine _hsm;
        private List<ICustomState> _state;
        private GameObject itemTemp;

        private Transform boxGroup;
        private Transform boxTempGroup;
        private SpineAnimationChange groupChange;
        private SpineAnimationChange tempGroupChange;


        private void OnCloseClick()
        {
            if (Time.realtimeSinceStartup - openTime < 1f) return;
            touchButton.interactable = false;
            _hsm?.ChangeState(nameof(UIItemBox_CloseState));
        }

        public override void PrivateAwake()
        {
            isPlayDefaultOpenAudio = false;
            isPlayDefaultCloseAudio = false;
            InitComponent();
        }

        private void Update()
        {
            _hsm?.Update();
        }

        private void OnDestroy()
        {
            _hsm?.CurrentState?.OnExit();
            data.OnDestroy?.Invoke();
        }

        protected virtual void InitComponent()
        {
            if (isInit) return;
            animator = transform.GetComponent<Animator>();
            animator.enabled = false;

            boxGroup = transform.Find($"Root/BoxGroup");
            boxTempGroup = transform.Find($"Root/BoxTempGroup");
            groupChange = boxGroup.GetComponent<SpineAnimationChange>();
            tempGroupChange = boxTempGroup.GetComponent<SpineAnimationChange>();
            // box = transform.Find($"Root/Box").GetComponent<SkeletonGraphic>();
            // boxRed = transform.Find($"Root/BoxRed").GetComponent<SkeletonGraphic>();
            txtCount = transform.Find("Root/Count").GetComponent<LocalizeTextMeshProUGUI>();
            // tempBox = transform.Find($"Root/BoxTemp").GetComponent<SkeletonGraphic>();
            // tempRedBox = transform.Find($"Root/BoxRedTemp").GetComponent<SkeletonGraphic>();
            // tempBox.transform.localScale = Vector3.zero;
            // tempRedBox.transform.localScale = Vector3.zero;
            canvasGroup = transform.GetComponent<CanvasGroup>();
            Transform layoutTrans = transform.Find("Root/Layout");
            rect = layoutTrans.GetComponent<RectTransform>().rect;
            this.layout = layoutTrans.GetComponent<GridLayoutGroup>();
            touchButton = transform.Find("Root/Touch").GetComponent<Button>();
            touchButton.interactable = false;
            touchButton.onClick.RemoveAllListeners();
            touchButton.onClick.AddListener(OnCloseClick);
            itemTemp = GetItem("Root/Layout/Item");
            itemTemp.SetActive(false);
            openTime = Time.realtimeSinceStartup;
            isInit = true;
        }

        protected override void OnOpenWindow(UIWindowData windowData)
        {
            base.OnOpenWindow(windowData);
            this.data = (Data) windowData;
            if (data == null)
                return;

            _hsm = new HierarchicalStateMachine(false, gameObject);
            _state = new List<ICustomState>();
            _state.Add(new UIItemBox_InitState(this, _hsm));
            _state.Add(new UIItemBox_IdleState(this, _hsm));
            _state.Add(new UIItemBox_FlyState(this, _hsm));
            _state.Add(new UIItemBox_PreOpenState(this, _hsm));
            _state.Add(new UIItemBox_OpenState(this, _hsm));
            _state.Add(new UIItemBox_CloseState(this, _hsm));
            _hsm.Init(_state, nameof(UIItemBox_InitState));
            int index = 0;
            for (int i = 0; i < boxGroup.childCount; i++)
            {
                bool isOpen = (int) data.boxType == i;
                boxGroup.GetChild(i).gameObject.SetActive(isOpen);
                if (isOpen) index = i;
            }

            groupChange.m_skeletonGraphic = boxGroup.GetChild(index).GetComponent<SkeletonGraphic>();
            tempGroupChange.m_skeletonGraphic = boxTempGroup.GetChild(index).GetComponent<SkeletonGraphic>();
            animator.enabled = true;
            // switch (data.boxType)
            // {
            //     case BoxType.Blue:
            //     case BoxType.Purple:
            //         box.gameObject.SetActive(true);
            //         boxRed.gameObject.SetActive(false);
            //         break;
            //     case BoxType.Red:
            //         box.gameObject.SetActive(false);
            //         boxRed.gameObject.SetActive(true);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }

            if (data != null && data.BoxNum > 1)
            {
                txtCount.gameObject.SetActive(true);
                txtCount.SetText("x" + data.BoxNum);
            }
            else
            {
                txtCount.gameObject.SetActive(false);
            }

            // if (data.DefaultItemBar)
            //     ShowDefaultItemBar();
        }

        protected virtual void OnCollectReward()
        {
            if (data == null) return;
            if (data.ItemIds != null && data.ItemNums != null)
            {
                var flyCount = data.ItemIds.Count;
                for (int i = 0; i < data.ItemIds.Count; i++)
                {
                    Animator childAnimator = layout.transform.GetChild(i).GetComponent<Animator>();
                    ItemFlyModel.Instance.PlayItemAdd(data.ItemIds[i], data.ItemNums[i], layout.transform.GetChild(i),
                        () =>
                        {
                            flyCount--;
                            if (flyCount > 0)
                                return;

                            layout.gameObject.SetActive(false);
                            animator.Play($"disappear_01");
                            StartCoroutine(CommonUtils.DelayCall(1f, () =>
                            {
                                if (!data.HasReceive)
                                    ItemModel.Instance.Add(data.ItemIds, data.ItemNums, data.args, true);
                                CloseWindowWithinUIMgr();
                                data.OnFinish?.Invoke();
                            }));
                        });
                    childAnimator?.Play("disappear");
                }
            }
            else
            {
                layout.gameObject.SetActive(false);
                animator.Play($"disappear_01");
                StartCoroutine(CommonUtils.DelayCall(0.5f, () =>
                {
                    CloseWindowWithinUIMgr();
                    data.OnFinish?.Invoke();
                }));
            }

        }

        public static T Open<T>(int id, int num, Action onFinish = null) where T : UIItemBox
        {
            return Open<T>(new Data()
                {ItemIds = new List<int>() {id}, ItemNums = new List<int>() {num}, OnFinish = onFinish});
        }

        public static T Open<T>(Data data) where T : UIItemBox
        {
            return UIManager.Instance.OpenWindow<T>($"TMatch/Prefabs/{typeof(T).Name}", data);
        }
    }
}