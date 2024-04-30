using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControl_Simanis : MonoBehaviour
{
    public GameObject mainButtonPanel;
    public GameObject missionSelectPanel;

    public Button closeButt;
    public Button multiplayerButt;
    public Button singleplayerButt;

    
    void Start()
    {
        mainButtonPanel.SetActive(true);
        missionSelectPanel.SetActive(false);
        //AddButtonListeners();
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

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadSingleplayerScene()
    {
        SceneManager.LoadScene("SimanisSingle");
    }
}
