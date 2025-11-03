using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonPlus.UI;
using DragonU3DSDK.Storage;
using UnityEngine;
using UnityEngine.UI;

public partial class UIPhotoAlbumShopController
{
    private List<PhotoPage> Pages = new List<PhotoPage>();
    private int CurPageId = 0;
    public Button LeftBtn;
    public Button RightBtn;
    public void InitPhotoGroup()
    {
        var defaultPage = transform.Find("Root/BuyGroup/Grid");
        defaultPage.gameObject.SetActive(false);
        var photoConfigs = PhotoAlbumModel.Instance.PhotoConfig;
        var keys = photoConfigs.Keys.ToList();
        var curStoreLevelConfig = Storage.GetCurStoreLevel();
        for (var i=0;i<keys.Count;i++)
        {
            var photoConfig = photoConfigs[keys[i]];
            var photoGroup = transform.Find("Root/PhotoGroup/Photo/"+(i+1)).gameObject
                .AddComponent<UIPopupPhotoAlbumProgressController.PhotoGroup>();
            photoGroup.Init(photoConfig,photoGroup.transform.Find("PhotoFinish"));
            var page = Instantiate(defaultPage, defaultPage.parent).gameObject.AddComponent<PhotoPage>();
            page.gameObject.SetActive(true);
            page.Init(photoConfig, photoGroup,this);
            Pages.Add(page);
            if (photoConfig.PageLevels.Contains(curStoreLevelConfig.Id))
                CurPageId = i;
        }
        for (var i = 0; i < Pages.Count; i++)
        {
            if (i < CurPageId)
            {
                Pages[i].SetLeft();
            }
            else if(i > CurPageId)
            {
                Pages[i].SetRight();
            }
            else
            {
                Pages[i].SetMiddle();
            }
        }

        LeftBtn = transform.Find("Root/ButtonPrevious").GetComponent<Button>();
        RightBtn = transform.Find("Root/ButtonNext").GetComponent<Button>();
        LeftBtn.onClick.AddListener(() =>
        {
            if (DisableChangePageBtn)
                return;
            Pages[CurPageId].HideRight();
            CurPageId--;
            Pages[CurPageId].ShowLeft();
            UpdatePageBtnState();
        });
        RightBtn.onClick.AddListener(() =>
        {
            if (DisableChangePageBtn)
                return;
            Pages[CurPageId].HideLeft();
            CurPageId++;
            Pages[CurPageId].ShowRight();
            UpdatePageBtnState();
        });
        UpdatePageBtnState();
    }

    public bool DisableChangePageBtn = false;

    public void UpdatePageBtnState()
    {
        LeftBtn.interactable = CurPageId > 0;
        RightBtn.interactable = CurPageId < Pages.Count-1;
    }

    public class PhotoPage : MonoBehaviour
    {
        public UIPopupPhotoAlbumProgressController.PhotoGroup PhotoGroup;
        public PhotoAlbumPhotoConfig Config;
        public List<StoreItemLevel> LevelList = new List<StoreItemLevel>();
        public Transform DefaultLevelGroup;
        public UIPhotoAlbumShopController Controller;
        public StoragePhotoAlbum Storage => Controller.Storage;
        
        public void Init(PhotoAlbumPhotoConfig config,UIPopupPhotoAlbumProgressController.PhotoGroup photoGroup,UIPhotoAlbumShopController controller)
        {
            Controller = controller;
            PhotoGroup = photoGroup;
            Config = config;
            DefaultLevelGroup = transform.Find("1");
            DefaultLevelGroup.gameObject.SetActive(false);
            
            
            var storeLevelConfigList = PhotoAlbumModel.Instance.StoreLevelConfig;
            for (var i = 0; i < Config.PageLevels.Count; i++)
            {
                var levelItem = Instantiate(DefaultLevelGroup, DefaultLevelGroup.parent).gameObject
                    .AddComponent<StoreItemLevel>();
                levelItem.gameObject.SetActive(true);
                var levelConfig = storeLevelConfigList.Find(a => a.Id == Config.PageLevels[i]);
                levelItem.InitStoreItemLevel(Storage,levelConfig);
                LevelList.Add(levelItem);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(DefaultLevelGroup.parent as RectTransform);
        }

        public Task PerformUnlock(PhotoAlbumStoreLevelConfig unlockLevel)
        {
            var task = new TaskCompletionSource<bool>();
            DOVirtual.DelayedCall(0.3f, () =>
            {
                task.SetResult(true);
                if (!this)
                    return;
                var level = LevelList.Find(a => a.StoreLevelConfig == unlockLevel);
                level.PerformUnlock(() =>
                {
                    // ScrollView.enabled = true;
                });
            }).SetTarget(transform);
            return task.Task;
        }

        private float MoveTime = 0.3f;
        private RectTransform Rect=>transform as RectTransform;
        float PageWidth => Rect.GetWidth();

        public void SetLeft()
        {
            Rect.SetAnchorPositionX(-PageWidth);
            Rect.gameObject.SetActive(false);
            PhotoGroup.Hide();
        }
        public void SetRight()
        {
            Rect.SetAnchorPositionX(PageWidth);
            Rect.gameObject.SetActive(false);
            PhotoGroup.Hide();
        }
        public void SetMiddle()
        {
            Rect.SetAnchorPositionX(0);
            Rect.gameObject.SetActive(true);
            PhotoGroup.Show();
        }
        public void ShowLeft()
        {
            PhotoGroup.Show();
            Rect.DOKill();
            Rect.gameObject.SetActive(true);
            Rect.DOAnchorPosX(0, MoveTime).SetEase(Ease.Linear);
        }
        public void HideLeft()
        {
            PhotoGroup.Hide();
            Rect.DOKill();
            Rect.DOAnchorPosX(-PageWidth, MoveTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                Rect.gameObject.SetActive(false);
            });
        }
        public void ShowRight()
        {
            PhotoGroup.Show();
            Rect.DOKill();
            Rect.gameObject.SetActive(true);
            Rect.DOAnchorPosX(0, MoveTime).SetEase(Ease.Linear);
        }
        public void HideRight()
        {
            PhotoGroup.Hide();
            Rect.DOKill();
            Rect.DOAnchorPosX(PageWidth, MoveTime).SetEase(Ease.Linear).OnComplete(() =>
            {
                Rect.gameObject.SetActive(false);
            });
        }
    }
}