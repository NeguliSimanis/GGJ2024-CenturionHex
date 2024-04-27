using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SP_HUD_Control : MonoBehaviour
{
    private SP_GameControl gameControl;
    public TextMeshProUGUI turnInfoTextBig;

    public string allyTurnInfoText = "Ally Turn";
    public string enemyTurnInfoText = "Enemy Turn";

    public Image turnDurationBar;
    public TurnTimerUI_Simanis turnTimerControl;

    private void Awake()
    {
        gameControl = gameObject.GetComponent<SP_GameControl>();
    }

    public void ShowTurnDuration(bool show)
    {
        turnDurationBar.transform.parent.gameObject.SetActive(show);

        if (show)
            turnTimerControl.RestartTimer(gameControl.enemyTurnDuration);
    }

    public void SetTurnInfoText()
    {
        if (gameControl.isAllyTurn)
        {
            turnInfoTextBig.text = allyTurnInfoText;
        }
        else
        {
            turnInfoTextBig.text = enemyTurnInfoText;
        }
    }
}
