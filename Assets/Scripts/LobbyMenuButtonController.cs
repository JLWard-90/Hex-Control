using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuButtonController : MonoBehaviour
{

    public int lobbyActionIndex = 0;
    public string ButtonText = "Default button";
    public string ExplanationText = "Default text";
    public Button parentButton;
    TurnController tcontrol;
    LevelController lcont;
    ActionController actions;

    // Start is called before the first frame update
    void Awake()
    {
        parentButton = transform.GetComponent<Button>();
        tcontrol = GameObject.Find("GameController").GetComponent<TurnController>();
        lcont = GameObject.Find("GameController").GetComponent<LevelController>();
        actions = GameObject.Find("GameController").GetComponent<ActionController>();
    }

    public void updateButtonAppearence()
    {
        bool active = false;
        active = CheckIfActive(lobbyActionIndex);
        if(active == true)
        {
            parentButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            parentButton.GetComponent<Image>().color = Color.white;
        }
        ButtonText = GetButtonText(lobbyActionIndex);
        parentButton.GetComponent<Text>().text = ButtonText;
    }

    public void buttonClicked()
    {
        Player[] thePlayers = lcont.players;
        Player currentPlayer = thePlayers[tcontrol.CurrentPlayer];
        actions.PerformLobbyAction(lobbyActionIndex, currentPlayer, thePlayers);
    }

    bool CheckIfActive(int actionIndex)
    {
        bool isActive = false;
        if(actionIndex == 2)
        {
            if(tcontrol.powerToThepeople == true)
            {
                isActive = true;
            }
        }
        else if (actionIndex == 3)
        {
            if(tcontrol.ruleOfLaw == true)
            {
                isActive = true;
            }
        }
        else if (actionIndex == 4)
        {
            if(tcontrol.bribesAsIndustry == true)
            {
                isActive = true;
            }
        }
        else if(actionIndex == 6)
        {
            if(tcontrol.rentHike == true)
            {
                isActive = true;
            }
        }
        else if(actionIndex == 7)
        {
            if(tcontrol.decentraliseGovernment == true)
            {
                isActive = true;
            }
        }
        else if(actionIndex == 9)
        {
            if(tcontrol.freeHousingInitiative == true)
            {
                isActive = true;
            }
        }
        else
        {
            isActive = false;
        }
        return isActive;
    }

    string GetButtonText(int actionIndex)
    {
        string outputText = "oh dear...";
        if (actionIndex == 0)
        {
            outputText = "Repeal";
        }
        else if (actionIndex == 1)
        {
            outputText = "Green Energy Initiative";
        }
        else if (actionIndex == 2)
        {
            outputText = "Power to the People";
        }
        else if (actionIndex == 3)
        {
            outputText = "Rule of Law";
        }
        else if (actionIndex == 4)
        {
            outputText = "Bribes as Industry";
        }
        else if (actionIndex == 5)
        {
            outputText = "Move Goalposts";
        }
        else if (actionIndex == 6)
        {
            outputText = "Rent Hike";
        }
        else if (actionIndex == 7)
        {
            outputText = "Decentralise Government";
        }
        else if (actionIndex == 8)
        {
            outputText = "Office Refurbishment";
        }
        else if (actionIndex == 9)
        {
            outputText = "Free Housing Initiative";
        }
        return outputText;
    }
}
