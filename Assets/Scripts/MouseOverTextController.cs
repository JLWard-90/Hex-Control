using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseOverTextController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    GameObject mouseOverTextPrefab;
    int ActionID = -1; //This is a unique ID for each button that will be used to decide which text should be displayed
    Text mouseOverText;
    GameObject TextObject;
    GameObject instance;
    LevelController levelCont;
    bool mouseOverTextOn = false;
    private void Awake()
    {
        levelCont = GameObject.Find("LevelController").GetComponent<LevelController>();
        mouseOverTextOn = levelCont.mouseOverText;
        instance = GameObject.FindGameObjectWithTag("mouseOverText");
        if (instance != null)
        {
            GameObject.Destroy(instance);
        }
    }

    void FixedUpdate()
    {

    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if(mouseOverTextOn == true)
        {
            //If your mouse hovers over the GameObject with the script attached, output this message
            Debug.Log("Mouse is over button.");
            if (TextObject != null)
            {
                GameObject.Destroy(TextObject);
            }
            instance = GameObject.FindGameObjectWithTag("mouseOverText");
            if (instance != null)
            {
                GameObject.Destroy(instance);
            }
            TextObject = Instantiate(mouseOverTextPrefab);
            TextObject.GetComponentInChildren<Text>().text = getButtonInfo(GetActionID());
            TextObject.GetComponent<RectTransform>().SetParent(this.transform, false);
            TextObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, 0);
        }
        
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse has left button");
        if(TextObject != null)
        {
            GameObject.Destroy(TextObject);
        }
        instance = GameObject.FindGameObjectWithTag("mouseOverText");
        if (instance != null)
        {
            GameObject.Destroy(instance);
        }
    }

    private string getButtonInfo(int actionID)
    {
        string infoText = "Sample text";
        Debug.Log(actionID);
        if(actionID == -1)
        {
            infoText = "Something has gone wrong.\nI don't know what this button does...";
        }
        else if (actionID == 99)
        {
            infoText = "Buy the selected district";
        }
        else if (actionID == 98)
        {
            infoText = "Sell the selected district";
        }
        else if (actionID == 97)
        {
            infoText = "View available Edicts to Lobby the city council for";
        }
        else if (actionID == 96)
        {
            infoText = "End your turn";
        }
        else if (actionID == 0)
        {
            infoText = "Repeal an existing edict that is in effect (Edits in effect are shown in red)";
        }
        else if (actionID == 1)
        {
            infoText = "Each player is charged $100 for every Power Plant district they own";
        }
        else if (actionID == 2)
        {
            infoText = "Power plants give bonuses to residential districts rather than industrial districts";
        }
        else if (actionID == 3)
        {
            infoText = "Increases the influence output of Civic districts";
        }
        else if (actionID == 4)
        {
            infoText = "Reduces the upkeep cost of Civic districts";
        }
        else if (actionID == 5)
        {
            infoText = "Randomly selects a new victory condition";
        }
        else if  (actionID == 6)
        {
            infoText = "Increases the income from residences but decreases their influence production";
        }
        else if (actionID == 7)
        {
            infoText = "Reduces the upkeep cost of Civic districts but also decreases their influence production";
        }
        else if (actionID == 8)
        {
            infoText = "Charges each player $200 for each Civic district they own";
        }
        else if (actionID == 9)
        {
            infoText = "Influence output of residential districts is increased but income from residential districts is reduced to 0";
        }
        return infoText;
    }

    private int GetActionID()
    {
        int id = -1;
        Debug.Log(transform.name);
        if(transform.name == "BuyButton")
        {
            id = 99;
        }
        else if(transform.name == "SellButton")
        {
            id = 98;
        }
        else if (transform.name == "LobbyButton")
        {
            id = 97;
        }
        else if (transform.name == "EndTurnButton")
        {
            id = 96;
        }
        else if (transform.tag == "LobbyButton")
        {
            id = transform.GetComponent<LobbyMenuButtonController>().lobbyActionIndex;
        }
        Debug.Log(id);
        return id;
    }
}
