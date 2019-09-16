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
        globalContol.updatePlayerList();
        globalContol.useDefaults = false;
        globalContol.updateWinCondition();
        SceneManager.LoadScene("MainScene");
    }
    public void UpdateResBuildings()
    {
        globalContol.updateResidentialValue();
    }
    public void UpdateIndBuildings()
    {
        globalContol.updateIndustrialValue();
    }
    public void UpdateCivBuildings()
    {
        globalContol.updateCivicValue();
    }
    public void UpdatePowBuildings()
    {
        globalContol.updatePowerPlantValue();
    }
    public void UpdateLanBuildings()
    {
        globalContol.updateLandmarkValue();
    }
    public void UpdateWinCondition()
    {
        globalContol.updateWinCondition();
    }
}
