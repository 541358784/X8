using DragonPlus;
using DragonU3DSDK.Storage;
using UnityEngine;

public class PhotoAlbumRedPoint : MonoBehaviour
{
    private StoragePhotoAlbum Storage;
    private LocalizeTextMeshProUGUI NumText;
    private void Awake()
    {
        NumText = transform.Find("Label")?.GetComponent<LocalizeTextMeshProUGUI>();
        NumText.gameObject.SetActive(false);
        EventDispatcher.Instance.AddEvent<EventPhotoAlbumScoreChange>(OnDiceCountChange);
    }
    private void OnDestroy()
    {
        EventDispatcher.Instance.RemoveEvent<EventPhotoAlbumScoreChange>(OnDiceCountChange);
    }

    public void OnDiceCountChange(EventPhotoAlbumScoreChange evt)
    {
        UpdateUI();
    }
    public void Init(StoragePhotoAlbum storage)
    {
        Storage = storage;
        UpdateUI();
    }

    public void UpdateUI()
    {
        var count = Storage.Score;
        var nextFish = PhotoAlbumModel.Instance.GetNextFish();
        if (nextFish == null)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(count >= nextFish.Price);
        }
        // NumText?.SetText(count.ToString());
    }
}