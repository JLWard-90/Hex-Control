using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        updateExplanationText();
        globalContol.updateWinCondition();
    }

    public void updateExplanationText()
    {
        int winConditionValue = (int)GameObject.Find("VicConDD").GetComponent<Dropdown>().value;
        Text explanationText = GameObject.Find("RulesExplanation").GetComponent<Text>();
        if (winConditionValue == 0)
        {
            explanationText.text = "Be the first to reach 500 influence";
        }
        else if(winConditionValue == 1)
        {
            explanationText.text = "Control 30 of the city's districts";
        }
        else if(winConditionValue == 2)
        {
            explanationText.text = "Best the first to reach $10000 cash";
        }
        else if(winConditionValue == 3)
        {
            explanationText.text = "Control 3 landmarks";
        }
        else
        {
            Debug.Log("Error in LevelSetupController:UpdateExplanationText -- win condition not recognised.");
        }
    }
}
