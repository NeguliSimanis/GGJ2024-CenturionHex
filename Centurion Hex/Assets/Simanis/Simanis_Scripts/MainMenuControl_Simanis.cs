using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControl_Simanis : MonoBehaviour
{
    public Button closeButt;
    public Button multiplayerButt;
    public Button singleplayerButt;

    
    void Start()
    {
        AddButtonListeners();
    }

    private void AddButtonListeners()
    {
        closeButt.onClick.AddListener(CloseGame);
        multiplayerButt.onClick.AddListener(LoadMultiplayerScene);
        singleplayerButt.onClick.AddListener(LoadSingleplayerScene);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void LoadMultiplayerScene()
    {
        SceneManager.LoadScene("Simanis");
    }

    public void LoadSingleplayerScene()
    {
        SceneManager.LoadScene("SimanisSingle");
    }
}
