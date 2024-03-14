using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AnnouncementUi_Simanis : MonoBehaviour
{
    public static AnnouncementUi_Simanis instance;

    public TextMeshProUGUI bigAnnouncmentText;
    public TextMeshProUGUI smallAnnouncmentText;

    private Image bgImage;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        bgImage = this.GetComponent<Image>();
        HideAnnouncmentText(0f);
    }

    public void ShowAnnouncmentText(string bigAnnounce, string smallAnnounce,
        float appearDuration, float disappearDelay, float disappearDuration,
        bool disappearAfter = true)
    {
        smallAnnouncmentText.text = smallAnnounce;
        bigAnnouncmentText.text = bigAnnounce;

        // Text color setup
        Color transparentColor = new Color(1f, 1f, 1f, 0f);
        bigAnnouncmentText.color = transparentColor;
        smallAnnouncmentText.color = transparentColor;

        // Fade in text
        bigAnnouncmentText.DOColor(Color.white, appearDuration);
        smallAnnouncmentText.DOColor(Color.white, appearDuration);

        // Background image setup
        Color startColor = Color.black;
        startColor.a = 0f;
        bgImage.color = startColor;

        // Fade in background image
        bgImage.DOColor(Color.black, appearDuration).SetEase(Ease.OutQuad);

        if (disappearAfter)
        {
            DOVirtual.DelayedCall(disappearDelay, () => HideAnnouncmentText(disappearDuration));
        }
    }

    public void HideAnnouncmentText(float disappearDuration)
    {
        // Text color setup
        Color transparentColor = new Color(0f, 0f, 0f, 0f);

        // Fade out text
        bigAnnouncmentText.DOColor(transparentColor, disappearDuration);
        smallAnnouncmentText.DOColor(transparentColor, disappearDuration);

        // Fade out background image
        bgImage.DOColor(transparentColor, disappearDuration).SetEase(Ease.OutQuad);
    }
}
