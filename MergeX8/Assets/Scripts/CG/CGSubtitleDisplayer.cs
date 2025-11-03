using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DragonPlus;

public class CGSubtitleDisplayer
{
    private TextAsset Subtitle;
    private LocalizeTextMeshProUGUI Text;
    private LocalizeTextMeshProUGUI Text2;
    private float FadeTime = 0.3f;
    private MonoBehaviour m_CGManager;

    public void Init(MonoBehaviour cgManager, LocalizeTextMeshProUGUI _text1, LocalizeTextMeshProUGUI _text2)
    {
        m_CGManager = cgManager;
        Text = _text1;
        Text2 = _text2;
        
        Subtitle = Resources.Load<TextAsset>($"CG/CGSubtitle_{LanguageModel.Instance.GetLocale()}");
    }

    public IEnumerator Begin()
    {
        var currentlyDisplayingText = Text;
        var fadedOutText = Text2;

        currentlyDisplayingText.SetText(string.Empty);
        fadedOutText.SetText(string.Empty);

        currentlyDisplayingText.gameObject.SetActive(true);
        fadedOutText.gameObject.SetActive(true);

        yield return FadeTextOut(currentlyDisplayingText);
        yield return FadeTextOut(fadedOutText);

        var parser = new CGSubtitleParser(Subtitle);

        var startTime = Time.time;
        SubtitleBlock currentSubtitle = null;
        while (true)
        {
            var elapsed = Time.time - startTime;
            var subtitle = parser.GetForTime(elapsed);
            if (subtitle != null)
            {
                if (!subtitle.Equals(currentSubtitle))
                {
                    currentSubtitle = subtitle;

                    var temp = currentlyDisplayingText;
                    currentlyDisplayingText = fadedOutText;
                    fadedOutText = temp;

                    currentlyDisplayingText.SetText(currentSubtitle.Text);

                    m_CGManager.StartCoroutine(FadeTextOut(fadedOutText));

                    yield return new WaitForSeconds(FadeTime / 3);

                    yield return FadeTextIn(currentlyDisplayingText);
                }
                yield return null;
            }
            else
            {
                m_CGManager.StartCoroutine(FadeTextOut(currentlyDisplayingText));
                yield return FadeTextOut(fadedOutText);
                currentlyDisplayingText.gameObject.SetActive(false);
                fadedOutText.gameObject.SetActive(false);
                yield break;
            }
        }
    }

    IEnumerator FadeTextOut(LocalizeTextMeshProUGUI text)
    {
        var toColor = text.m_TmpText.color;
        toColor.a = 0;
        yield return Fade(text, toColor, Ease.OutSine);
    }

    IEnumerator FadeTextIn(LocalizeTextMeshProUGUI text)
    {
        var toColor = text.m_TmpText.color;
        toColor.a = 1;
        yield return Fade(text, toColor, Ease.InSine);
    }

    IEnumerator Fade(LocalizeTextMeshProUGUI text, Color toColor, Ease ease)
    {
        yield return DOTween.To(() => text.m_TmpText.color, color => text.m_TmpText.color = color, toColor, FadeTime).SetEase(ease).WaitForCompletion();
    }
}
