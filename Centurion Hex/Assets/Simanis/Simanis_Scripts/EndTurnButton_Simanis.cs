using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton_Simanis : MonoBehaviour
{
    public GameObject endButtonTeam0;
    public GameObject endButtonTeam1;

    private void Awake()
    {
        endButtonTeam0.SetActive(true);
        endButtonTeam1.SetActive(true);

        endButtonTeam0.GetComponent<Button>().onClick.AddListener(EndTurn);
        endButtonTeam1.GetComponent<Button>().onClick.AddListener(EndTurn);
        endButtonTeam0.SetActive(false);
        endButtonTeam1.SetActive(false);

    }

    public void EndTurn()
    {
        //Debug.Log("yay");
        Network.instance.EndMove();
    }
}

