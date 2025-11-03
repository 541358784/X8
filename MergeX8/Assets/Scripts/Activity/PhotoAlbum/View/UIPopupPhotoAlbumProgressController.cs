using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupPhotoAlbumProgressController:UIWindowController
{
    public static UIPopupPhotoAlbumProgressController Instance;
    public static UIPopupPhotoAlbumProgressController Open(StoragePhotoAlbum storagePhotoAlbum)
    {
        if (Instance)
            Instance.CloseWindowWithinUIMgr(true);
        Instance = UIManager.Instance.OpenUI(UINameConst.UIPopupPhotoAlbumProgress, storagePhotoAlbum) as
            UIPopupPhotoAlbumProgressController;
        return Instance;
    }
    public override void PrivateAwake()
    {
        ClseBtn = transform.Find("Root/ButtonClose").GetComponent<Button>();
        ClseBtn.onClick.AddListener(() =>
        {
            AnimCloseWindow();
        });
    }

    private Button ClseBtn;
    public StoragePhotoAlbum Storage;
    protected override void OnOpenWindow(params object[] objs)
    {
        base.OnOpenWindow(objs);
        Storage = objs[0] as StoragePhotoAlbum;
        var photoConfigs = PhotoAlbumModel.Instance.PhotoConfig;
        var keys = photoConfigs.Keys.ToList();
        for (var i = 0; i < keys.Count; i++)
        {
            var photoConfig = photoConfigs[keys[i]];
            var photoObj = transform.Find("Root/Photo/" + (i + 1));
            var finishObj = transform.Find("Root/PhotoFinish/" + (i + 1));
            var photoGroup = photoObj.gameObject.AddComponent<PhotoGroup>();
            photoGroup.Init(photoConfig,finishObj,true);
            PhotoGroups.Add(photoGroup);
        }
    }

    public async Task PerformCollectAll()
    {
        var photoConfigs = PhotoAlbumModel.Instance.PhotoConfig;
        var keys = photoConfigs.Keys.ToList();
        for (var i = 0; i < keys.Count; i++)
        {
            var photoConfig = photoConfigs[keys[i]];
            var task = new TaskCompletionSource<bool>();
            UIPopupPhotoAlbumSpineController.Open(photoConfig,true,true).AddCallback(()=>task.SetResult(true));
            await task.Task;
        }
    }

    private List<PhotoGroup> PhotoGroups = new List<PhotoGroup>();
    public class PhotoGroup : MonoBehaviour
    {
        public PhotoAlbumPhotoConfig Config;
        public StoragePhotoAlbum Storage => PhotoAlbumModel.Instance.Storage;
        public List<PhotoPieceGroup> PieceGroups = new List<PhotoPieceGroup>();
        public Transform FinishView;
        public Animator FinishAnimator;
        public Button Btn;
        public bool IsCollect => Storage.PhotoAlbumCollectState.TryGetValue(Config.Id, out var photoStorage) &&
                                 photoStorage.CollectState.Count == Config.Parts.Count;
        public void Init(PhotoAlbumPhotoConfig config,Transform finishView,bool useAllKey = false)
        {
            Config = config;
            FinishView = finishView;
            FinishAnimator = finishView.GetComponent<Animator>();
            for (var i = 0; i < Config.Parts.Count; i++)
            {
                var pieceObj = transform.Find((i + 1).ToString());
                var pieceGroup = pieceObj.gameObject.AddComponent<PhotoPieceGroup>();
                var pieceConfig = PhotoAlbumModel.Instance.PhotoPieceConfig[Config.Parts[i]];
                pieceGroup.Init(this,pieceConfig);
                PieceGroups.Add(pieceGroup);
            }
            FinishView.gameObject.SetActive(IsCollect);
            if (FinishAnimator && FinishView.gameObject.activeSelf)
            {
                FinishAnimator.PlayAnimation("Finish_"+(Config.Id - PhotoAlbumModel.Instance.GlobalConfig.PhotoOffset)+"_Normal");
            }
            Btn = finishView.GetComponent<Button>();
            if (Btn)
            {
                Btn.onClick.AddListener(() =>
                {
                    UIPopupPhotoAlbumSpineController.Open(Config,useAllKey && PhotoAlbumModel.Instance.IsFinish,false).EnableCloseBtn();
                });
            }
        }

        public async Task PerformCollectPiece(PhotoAlbumPhotoPieceConfig pieceConfig,bool isFull)
        {
            var piece = PieceGroups.Find(a => a.Config == pieceConfig);
            await piece.PerformCollect();
            if (!this)
                return;
            if (isFull)
            {
                FinishView.gameObject.SetActive(IsCollect);
                await FinishAnimator.PlayAnimationAsync("Finish_"+(Config.Id - PhotoAlbumModel.Instance.GlobalConfig.PhotoOffset));
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            foreach (var group in PieceGroups)
            {
                group.UpdateAnimator();
            }
            FinishView.gameObject.SetActive(IsCollect);
            if (FinishAnimator && FinishView.gameObject.activeSelf)
            {
                FinishAnimator.PlayAnimation("Finish_"+(Config.Id - PhotoAlbumModel.Instance.GlobalConfig.PhotoOffset)+"_Normal");
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            FinishView.gameObject.SetActive(false);
        }

        private void Awake()
        {
        }
    }
    public class PhotoPieceGroup : MonoBehaviour
    {
        public PhotoGroup PhotoGroup;
        public PhotoAlbumPhotoPieceConfig Config;
        public StoragePhotoAlbum Storage => PhotoAlbumModel.Instance.Storage;
        // public Transform Normal;
        public Transform Finish;
        public Animator FinishAnimator;

        public bool IsCollect => Storage.PhotoAlbumCollectState.TryGetValue(Config.PhotoId, out var photoStorage) &&
                                 photoStorage.CollectState.Contains(Config.Id);

        public void Init(PhotoGroup photoGroup,PhotoAlbumPhotoPieceConfig config)
        {
            Config = config;
            PhotoGroup = photoGroup;
            // Normal = transform.Find("Normal");
            Finish = transform.Find("Finish");
            FinishAnimator = Finish.GetComponent<Animator>();
            // Normal.gameObject.SetActive(!IsCollect);
            Finish.gameObject.SetActive(IsCollect);
            if (FinishAnimator && Finish.gameObject.activeSelf)
            {
                FinishAnimator.PlayAnimation("Finish_Normal");
            }
        }

        public async Task PerformCollect()
        {
            // Normal.gameObject.SetActive(!IsCollect);
            Finish.gameObject.SetActive(IsCollect);
            if (FinishAnimator)
            {
                await FinishAnimator.PlayAnimationAsync("Finish");
            }
        }

        public void UpdateAnimator()
        {
            if (FinishAnimator && Finish.gameObject.activeSelf)
            {
                FinishAnimator.PlayAnimation("Finish_Normal");
            }
        }
        private void Awake()
        {
            
        }

        private void OnDestroy()
        {
            
        }
    }
}