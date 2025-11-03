using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using DragonPlus;
using System.Collections;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;
using Framework;
// using BiEventCooking = DragonU3DSDK.Network.API.Protocol.BiEventCK7;

public class CGVideoManager : GlobalSystem<CGVideoManager>
{
    private static int PRIVATE_VIDEO_WIDTH = 1365;
    private static int PRIVATE_VIDEO_HEIGHT = 768;

    private Button m_SkipButton;
    private GameObject m_SfxObj;
    private LocalizeTextMeshProUGUI m_SubtitleText;
    private CGSubtitleDisplayer m_CGSubtitleDisplayer;
    private Coroutine m_CGSubtitleCoroutine;

    private LocalizeTextMeshProUGUI m_FadeInText;
    private LocalizeTextMeshProUGUI m_FadeOutText;

    private VideoPlayer m_VideoPlayer;
    private RenderTexture m_RenderTexture;
    private GameObject m_VideoObject;
    private RawImage m_VideoRawImage;
    private Action m_VideoPreloadCallbacck;
    private Action m_VideoStatrPlayCallback;
    private Action m_VideoEndCallback;

    private IEnumerator m_CheckVideoIsPlayingCoroutine;
    private IEnumerator m_FadeOutCoroutine;
    private IEnumerator m_DisplayWorldMapCoroutine;
    
    public bool IsVideoPlaying => m_VideoPlayer != null && m_VideoPlayer.isPlaying;

    public static int VIDEO_WIDTH { get => PRIVATE_VIDEO_WIDTH; set => PRIVATE_VIDEO_WIDTH = value; }
    public static int VIDEO_HEIGHT { get => PRIVATE_VIDEO_HEIGHT; set => PRIVATE_VIDEO_HEIGHT = value; }

    public bool _canSkip = true;
    public void Clean()
    {
        m_CheckVideoIsPlayingCoroutine = null;
        m_FadeOutCoroutine = null;
        m_DisplayWorldMapCoroutine = null;
    }

    public void PreloadVideo(Transform parentTr, Action preloadedCallback,GameObject prefab )
    {
        if (prefab != null)
        {
            m_VideoObject = UnityEngine.Object.Instantiate(prefab);
            m_VideoObject.transform.SetParent(parentTr);
            RectTransform rectTransform = m_VideoObject.GetComponent<RectTransform>();
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition3D = Vector3.zero;
            m_VideoObject.transform.localScale = Vector3.one;
            m_VideoRawImage = m_VideoObject.transform.Find("RawImage_Video").GetComponent<RawImage>();
            m_VideoRawImage.gameObject.SetActive(false);

            m_SkipButton = m_VideoObject.transform.Find("SkipButton").GetComponent<Button>();
            m_SkipButton.onClick.RemoveAllListeners();
            m_SkipButton.onClick.AddListener(OnBtnSkip);
            m_SkipButton.gameObject.SetActive(false);

            m_SfxObj = m_VideoObject.transform.Find("Sfx").gameObject;

            m_FadeInText = m_VideoObject.transform.Find("Text1").GetComponent<LocalizeTextMeshProUGUI>();
            m_FadeOutText = m_VideoObject.transform.Find("Text2").GetComponent<LocalizeTextMeshProUGUI>();

            m_CGSubtitleDisplayer = new CGSubtitleDisplayer();
            // m_CGSubtitleDisplayer.Init(UIRoot.Instance, m_FadeInText, m_FadeOutText);
        }
        m_VideoPlayer = m_VideoObject.GetOrCreateComponent<VideoPlayer>();
        m_RenderTexture = m_VideoPlayer.targetTexture;
        m_VideoPreloadCallbacck = preloadedCallback;
        m_VideoPlayer.prepareCompleted += VideoPrepareCompleted;
        m_VideoPlayer.loopPointReached += VideoLoopPointReached;
        m_VideoPlayer.errorReceived += VideoErrorReceived;
        m_VideoPlayer.started += VideoStarted;
        m_VideoPlayer.Prepare();
    }

    private void VideoPrepareCompleted(VideoPlayer player)
    {
        //m_VideoPlayer.Pause();
        m_CheckVideoIsPlayingCoroutine = CheckVideoIsPlaying();
        UIRoot.Instance.StartCoroutine(m_CheckVideoIsPlayingCoroutine);
    }

    private IEnumerator CheckVideoIsPlaying()
    {
        while (m_VideoPlayer.frame <= 0)
        {
            yield return null;
        }
        m_VideoPreloadCallbacck?.Invoke();
        m_SfxObj.SetActive(true);
        m_VideoRawImage.gameObject.SetActive(true);
        m_VideoRawImage.CrossFadeAlpha(1.0f, 0.03f, true);
        m_SkipButton.gameObject.SetActive(_canSkip);
        m_DisplayWorldMapCoroutine = DisplayWorldMap();
        yield return UIRoot.Instance.StartCoroutine(m_DisplayWorldMapCoroutine);
    }

    private IEnumerator DisplayWorldMap()
    {
        yield return new WaitForSeconds(1.0f);
        //GameMain.App.DecorationMgr.EnableUpdate = true;
        //GameMain.App.DecorationMgr.RefreshWorld(1, UserDataMoudule.RequireStorageCookWorld(1));
        //GameMain.App.DecorationMgr.CurrentWorld.PinchMap.FocusVisitPosition(100);
        m_FadeOutCoroutine = FadeOut();
        yield return UIRoot.Instance.StartCoroutine(m_FadeOutCoroutine);
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(24.0f);
        m_VideoRawImage.CrossFadeAlpha(0.0f, 2f, true);
        yield break;
    }

    private void VideoLoopPointReached(VideoPlayer player)
    {
        // CKBI.Instance.SendGameEvent(BiEventCooking.Types.GameEventType.GameEventFteAnimationFinished);
        StopVideo();
    }

    private void VideoErrorReceived(VideoPlayer player, string errorMesage)
    {
        StopVideo();
    }

    private void VideoStarted(VideoPlayer player)
    {
        m_VideoStatrPlayCallback?.Invoke();
        m_VideoStatrPlayCallback = null;
    }

    private void SetVideoAspect(RawImage rawImage)
    {
        AspectRatioFitter asp = rawImage.gameObject.GetOrCreateComponent<AspectRatioFitter>();
        if (asp)
        {
            asp.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            asp.aspectRatio = m_RenderTexture.width / (float)m_RenderTexture.height;
        }
    }

    public void PlayVideo()
    {
        if (m_VideoPlayer == null) return;

        if (m_VideoPlayer.isPlaying)
        {
            //StopVideo();
            return;
        }

        if (!m_VideoObject.activeSelf)
        {
            m_VideoObject.SetActive(true);
        }

        m_VideoRawImage.texture = m_RenderTexture;
        //SetVideoAspect(m_VideoRawImage);
        m_VideoPlayer.targetTexture = m_RenderTexture;
        m_VideoPlayer.Play();
        
        // var commonData = StorageManager.Instance.GetStorage<StorageMain>()?.CommonData;
        // commonData.PlayCG = true;

        // m_CGSubtitleCoroutine = UIRoot.Instance.StartCoroutine(m_CGSubtitleDisplayer.Begin());
    }

    public void SetVideoCallback(Action onStart, Action onEnd)
    {
        m_VideoStatrPlayCallback = onStart;
        m_VideoEndCallback = onEnd;
    }

    public void StopVideo()
    {
        UIRoot.Instance.mRoot.transform.Find("BlackMask").gameObject.SetActive(false);
        if (m_VideoPlayer != null && m_VideoPlayer.isPlaying)
        {
            m_VideoPlayer.Stop();

            if (m_VideoObject != null && m_VideoObject.activeSelf)
                m_VideoObject.SetActive(false);

            if (m_VideoRawImage != null)
                m_VideoRawImage.texture = null;

            m_VideoEndCallback?.Invoke();
            m_VideoEndCallback = null;
        }

        // if (m_CGSubtitleCoroutine != null)
        // {
        //     UIRoot.Instance.StopCoroutine(m_CGSubtitleCoroutine);
        //     m_CGSubtitleCoroutine = null;
        // }

        if (m_CheckVideoIsPlayingCoroutine != null)
        {
            UIRoot.Instance.StopCoroutine(m_CheckVideoIsPlayingCoroutine);
            m_CheckVideoIsPlayingCoroutine = null;
        }

        if (m_DisplayWorldMapCoroutine != null)
        {
            UIRoot.Instance.StopCoroutine(m_DisplayWorldMapCoroutine);
            m_DisplayWorldMapCoroutine = null;
        }

        if (m_FadeOutCoroutine != null)
        {
            UIRoot.Instance.StopCoroutine(m_FadeOutCoroutine);
            m_FadeOutCoroutine = null;
        }

        Shutdown();
    }

    public void Shutdown()
    {
        if (m_RenderTexture != null)
        {
            //RenderTexture.ReleaseTemporary(m_RenderTexture);
            m_RenderTexture.Release();
            m_RenderTexture = null;
        }
        // m_CGSubtitleDisplayer = null;
        UnityEngine.Object.Destroy(m_VideoObject);
    }
    
    public void TryStartCG(Action startCallBack, Action endCallBack)
    {
        GameObject prefab = Resources.Load<GameObject>("CG/UICGVideo");
        TryStartCG(startCallBack, endCallBack, prefab);
    }

    public void TryStartCG(Action startCallBack, Action endCallBack, GameObject prefab ,bool CanSkip=true)
    {
        _canSkip = CanSkip;
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue1);
        UIRoot.Instance.mRoot.transform.Find("BlackMask").gameObject.SetActive(true);
        PreloadVideo(UIRoot.Instance.mRoot.transform, null,prefab);
        SetVideoCallback(startCallBack, endCallBack);
        PlayVideo();
        AudioManager.Instance.PauseAllMusic();
    }
    public void DebugPlayCG()
    {
        UIRoot.Instance.mRoot.transform.Find("BlackMask").gameObject.SetActive(true);
        GameObject prefab = Resources.Load<GameObject>("CG/UICGVideo");
        PreloadVideo(UIRoot.Instance.mRoot.transform, null,prefab);
        SetVideoCallback(()=>{ }, () =>
        {
            GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue3);
            AudioManager.Instance.ResumeAllMusic();
        });
        PlayVideo();
        AudioManager.Instance.PauseAllMusic();
    }


    private void OnBtnSkip()
    {
        GameBIManager.Instance.SendGameEvent(BiEventAdventureIslandMerge.Types.GameEventType.GameEventFtue2);
        StopVideo();
    }
}
