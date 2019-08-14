using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    GlobalController gcontroller;

    private void Start()
    {
        gcontroller = GameObject.Find("GlobalController").GetComponent<GlobalController>();
    }

    public void OnExitButtonPress()
    {
        Application.Quit();
    }
    public void OnQuickGameButtonPress()
    {
        gcontroller.useDefaults = true;
        SceneManager.LoadScene("MainScene");
    }
    public void OnNewGameButtonPress()
    {
        Debug.Log("Game setup screen not implemented yet");
        SceneManager.LoadScene("GameSetupMenu");
    }
    
}
