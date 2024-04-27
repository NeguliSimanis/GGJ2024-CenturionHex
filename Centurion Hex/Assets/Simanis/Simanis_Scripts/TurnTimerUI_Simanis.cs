using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTimerUI_Simanis : MonoBehaviour
{
    public Image turnTimerBar;

    private float turnDuration;
    private bool isTimerStarted = false;
    private float turnEndTime;
    private float turnStartTime;

    public void RestartTimer(float newTurnDuration = 30f)
    {
        turnDuration = newTurnDuration;

        turnStartTime = Time.time;
        turnEndTime = Time.time + newTurnDuration;

        isTimerStarted = true;
    }

    public void RestartMultiplayerTimer(float newTurnDuration = 30f)
    {
        if (CenturionGame.Instance.mRoundState == CenturionGame.RoundState.rsManagement
            || CenturionGame.Instance.mRoundState == CenturionGame.RoundState.rsMovingCharacters)
        {
            RestartTimer(newTurnDuration);
        }
    }

    private void Update()
    {
        if (isTimerStarted)
            UpdateTimer();
    }

    private void UpdateTimer()
    {
        float elapsedTime = Time.time - turnStartTime;
        turnTimerBar.fillAmount = 1 - elapsedTime / turnDuration;

        if (elapsedTime >= turnDuration)
        {
            turnTimerBar.fillAmount = 0f;
            isTimerStarted = false;
        }
        
    }
}
