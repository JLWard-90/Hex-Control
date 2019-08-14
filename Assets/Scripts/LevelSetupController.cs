using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSetupController : MonoBehaviour
{
    GlobalController globalContol;
    private void Awake()
    {
        globalContol = GameObject.Find("GlobalController").GetComponent<GlobalController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBackButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnStartGameButton()
    {
        globalContol.useDefaults = false;
        SceneManager.LoadScene("MainScene");
    }
}
