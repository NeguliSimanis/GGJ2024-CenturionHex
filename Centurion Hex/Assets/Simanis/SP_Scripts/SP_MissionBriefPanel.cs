using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SP_MissionBriefPanel : MonoBehaviour
{
    public static SP_MissionBriefPanel instance;

    public GameObject popup;
    Vector3 popupTargetScale;
    public Image popupBG;

    [Header("POPUP CONTENTS")]
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDescription;

    private void Awake()
    {
        instance = this;
        popupTargetScale = popup.transform.localScale;
    }

    private void Start()
    {
        InitializePopup();
    }

    public void InitializePopup()
    {
        missionTitle.text = SP_MissionDescriptions.GetMissionName(SP_GameControl.instance.mission);
        missionDescription.text = SP_MissionDescriptions.GetMissionDescription(SP_GameControl.instance.mission);

        popup.SetActive(true);
        popupBG.gameObject.SetActive(true);
    }

    public void ShowPopup(bool show)
    {
        if (!show)
        {
            popupBG.gameObject.SetActive(false);
            popup.SetActive(false);
            return;
        }

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
