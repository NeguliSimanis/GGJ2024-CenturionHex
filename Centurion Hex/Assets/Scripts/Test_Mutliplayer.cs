using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Test_Mutliplayer : MonoBehaviour
{
    private Alteruna.Avatar _avatar;
    private TextMeshProUGUI playerText;

    private void Awake()
    {
        GameObject parent = GameObject.FindGameObjectWithTag("MultiplayerSpawnPoint");
        transform.parent = parent.transform;
    }

    void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();
        playerText = GetComponent<TextMeshProUGUI>();
        if (!_avatar.IsMe)
            return;
    }

    void Update()
    {
        if (!_avatar.IsMe)
            return;
        if (Input.anyKeyDown)
        {
            string keyPressed = GetKeyPressed();
            playerText.text = keyPressed;
            Debug.Log("Key Pressed: " + keyPressed);
        }
    }

    string GetKeyPressed()
    {
        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(keyCode))
            {
                return keyCode.ToString();
            }
        }
        return "None";
    }
}
