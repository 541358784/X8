using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DragonPlus;
using DragonPlus.Config.KeepPet;
using DragonU3DSDK.Asset;
using UnityEngine;
using UnityEngine.UI;

public class UIPopupKeepPetClueItem
{
    private GameObject _gameObject;
    public KeepPetClueConfig _config { get; private set; }
    private GameObject _haveObj;
    private GameObject _notHaveObj;

    private Image _haveIcon;
    private LocalizeTextMeshProUGUI _haveText;

    private Image _notHaveIcon;
    private LocalizeTextMeshProUGUI _notHaveText;

    public Image icon
    {
        get { return _haveIcon; }
    }

    public Sprite DisPlaySprite
    {
        get
        {
            return ResourcesManager.Instance.GetSpriteVariant("KeepPetAtlas", _config.ImageBigName);
        }
    }
    
    public void Init(GameObject gameObject, KeepPetClueConfig config)
    {
        _gameObject = gameObject;
        _config = config;

        _haveObj = gameObject.transform.Find("Have").gameObject;
        _haveIcon = gameObject.transform.Find("Have/Image").GetComponent<Image>();
        _haveText = gameObject.transform.Find("Have/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        _notHaveObj = gameObject.transform.Find("NotHave").gameObject;
        _notHaveIcon = gameObject.transform.Find("NotHave/Image").GetComponent<Image>();
        _notHaveText = gameObject.transform.Find("NotHave/Text").GetComponent<LocalizeTextMeshProUGUI>();

        Recover();
        
        var sprite = ResourcesManager.Instance.GetSpriteVariant("KeepPetAtlas", config.ImageName);
        _haveIcon.sprite = sprite;
        _haveText.SetTerm(config.Name);
    }

    public void Hide()
    {
        _haveObj.SetActive(false);
        _notHaveObj.SetActive(false);
    }

    public void Recover()
    {
        bool haveClue = KeepPetModel.Instance.HaveClue(_config.Id);
        _haveObj.SetActive(haveClue);
        _notHaveObj.SetActive(!haveClue);
    }
}


public class UIPopupKeepPetClueController : UIWindowController
{
    private GameObject _item;
    private List<UIPopupKeepPetClueItem> _clueItems = new List<UIPopupKeepPetClueItem>();
    private GameObject _displayObj;
    private Image _displayIcon;
    private Button _displayClose;
    private UIPopupKeepPetClueItem _displayItem;
    
    private float _scale = 1.5f;
    private float _animTime = 0.5f;

    private LocalizeTextMeshProUGUI _titleText;
    private LocalizeTextMeshProUGUI _contentText;
    
    
    public override void PrivateAwake()
    {
        transform.Find("Root/ButtonClose").GetComponent<Button>().onClick.AddListener(()=>
        {
            AnimCloseWindow();
        });
        
        _item = transform.Find("Root/Scroll View/Viewport/Content/1").gameObject;
        _item.gameObject.SetActive(false);

        _displayObj = transform.Find("Root/Display").gameObject;
        _displayObj.gameObject.SetActive(false);

        _displayIcon = transform.Find("Root/Display/Icon").GetComponent<Image>();
        _displayClose = transform.Find("Root/Display/DisplayClose").GetComponent<Button>();
        _displayClose.onClick.AddListener(RecoverDisplay);
        
        _titleText = transform.Find("Root/Display/TextTitle").GetComponent<LocalizeTextMeshProUGUI>();
        _contentText = transform.Find("Root/Display/Text").GetComponent<LocalizeTextMeshProUGUI>();
        
        foreach (var config in KeepPetConfigManager.Instance.GetConfig<KeepPetClueConfig>())
        {
            var item = Instantiate(_item.gameObject, _item.transform.parent);
            item.gameObject.SetActive(true);

            UIPopupKeepPetClueItem clueItem = new UIPopupKeepPetClueItem();
            clueItem.Init(item, config);
                
            _clueItems.Add(clueItem);

            var clueConfig = config;
            item.transform.GetComponent<Button>().onClick.AddListener(() =>
            {
                if(!KeepPetModel.Instance.HaveClue(clueConfig.Id))
                    return;

                DisPlay(clueItem);
            });
        }
    }

    private void DisPlay(UIPopupKeepPetClueItem clueItem)
    {
        //clueItem.Hide();
        
        AudioManager.Instance.PlaySound(150);
        
        _displayItem = clueItem;
        _displayIcon.sprite = clueItem.DisPlaySprite;
        _displayIcon.SetNativeSize();
        
        _titleText.SetTerm(clueItem._config.Name);
        _contentText.SetTerm(clueItem._config.Explain);
        
        // _displayIcon.transform.position = clueItem.icon.transform.position;
        // _displayIcon.transform.localScale = Vector3.one;
        
        _displayObj.gameObject.SetActive(true);
        _displayClose.gameObject.SetActive(true);

        // _displayIcon.transform.DOScale(_scale,_animTime).SetEase(Ease.Linear);
        // _displayIcon.transform.DOLocalMove(Vector3.zero, _animTime).SetEase(Ease.Linear).OnComplete(() =>
        // {
        //     _displayClose.gameObject.SetActive(true);
        // });
    }

    private void RecoverDisplay()
    {
        _displayClose.gameObject.SetActive(false);
        _displayObj.gameObject.SetActive(false);
        _displayItem = null;
        
        // _displayIcon.transform.DOScale(Vector3.one, _animTime).SetEase(Ease.Linear);
        // _displayIcon.transform.DOMove(_displayItem.icon.transform.position, _animTime).SetEase(Ease.Linear).OnComplete(() =>
        // {
        //     _displayObj.gameObject.SetActive(false);
        //     _displayItem.Recover();
        //     _displayItem = null;
        // });
    }
    
    public static void OpenUI()
    {
        UIManager.Instance.OpenUI(UINameConst.UIPopupKeepPetClue);
    }
    
}