using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class SP_GameOverPopup : MonoBehaviour
{
    public static SP_GameOverPopup instance;

    public GameObject popup;
    Vector3 popupTargetScale;
    public Image popupBG;

    [Header("POPUP CONTENTS")]
    public TextMeshProUGUI popupTitle;
    public TextMeshProUGUI popupSubTitle;

    private void Awake()
    {
        instance = this;
        popupTargetScale = popup.transform.localScale;
    }

    public void InitializePopup(bool isVictory)
    {
        ShowPopup(true);
        
        if (isVictory)
        {
            popupTitle.text = "Glorious Victory";
            
        }
        else
        {
            popupTitle.text = "Defeat!";
        }
    }

    public void ShowPopup(bool show)
    {

        DOVirtual.DelayedCall(0.5f, () =>
        {
            // background animation
            popupBG.gameObject.SetActive(true);
            Color targetColor = popupBG.color;
            popupBG.color = new Color(0, 0, 0, 0);
            popupBG.DOFade(targetColor.a, 0.7f);


            // scale up popup group
            popup.SetActive(show);
            
            popup.transform.localScale = Vector3.zero;
            popup.transform.DOScale(popupTargetScale, 0.4f).SetEase(Ease.InOutQuad);
        });

        
    }
}
