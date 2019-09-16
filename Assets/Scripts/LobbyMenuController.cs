using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuController : MonoBehaviour {

    ActionController actionController;
    Player currentPlayer;
    Player[] theplayers;
    LevelController lCont;
    TurnController turnController;
    HexGrid theGrid;
    GameObject[] instance;
    [SerializeField]
    GameObject LobbyMenuButtonPrefab;

    private void Awake()
    {
        instance = GameObject.FindGameObjectsWithTag("LobbyMenu");
        Debug.Log(instance.Length);
        if(instance.Length > 1)
        {
            Destroy(this.gameObject);
        }
        lCont = GameObject.Find("LevelController").GetComponent<LevelController>();
        turnController = GameObject.Find("GameController").GetComponent<TurnController>();
        actionController = GameObject.Find("GameController").GetComponent<ActionController>();
        theGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        /*if(turnController.powerToThepeople == true) //This is old and should be removed when the up to date code is finished and tested
        {
            Button pttpButton = GameObject.Find("powerToThePeopleButton").GetComponent<Button>();
            pttpButton.GetComponent<Image>().color = Color.red;
        }
        */
        int nButtons = lCont.NLobbyOptions;
        int[] lobbyOptions = lCont.lobbyingOptions;
        for(int i=0; i < nButtons; i++)
        {
            //For each lobby button, we need to instantiate it, set it's parent, update the action it performs, and update it's appearence.
            GameObject newLobbyButton = Instantiate<GameObject>(LobbyMenuButtonPrefab);
            newLobbyButton.transform.SetParent(transform, false);
            newLobbyButton.GetComponent<LobbyMenuButtonController>().lobbyActionIndex = lobbyOptions[i];
            newLobbyButton.GetComponent<LobbyMenuButtonController>().updateButtonAppearence();
        }
    }

    void Start()
    {
        int playerindex = turnController.CurrentPlayer;
        currentPlayer = lCont.players[playerindex];
        theplayers = lCont.players;
    }

    public void OnCloseButton()
    {
        Destroy(gameObject);
    }

    public void OnGreenInitiativeButton()
    {
        if(currentPlayer.AIplayer != true)
        {
            Debug.Log(currentPlayer);
            Debug.Log(theplayers);
            actionController.OnGreenEnergyInitiative(currentPlayer, theplayers);
            Destroy(gameObject);
        }
    }

    public void OnChangeToIndButton()
    {
        if(currentPlayer.AIplayer != true)
        {
            Debug.Log("Change to ind...");
            Debug.Log(theGrid.SelectedCellIndex);
            HexCell cell = theGrid.cells[theGrid.SelectedCellIndex];
            Debug.Log(currentPlayer);
            actionController.ChangeCellType(cell, currentPlayer, 2, theGrid.SelectedCellIndex);
            Destroy(gameObject);
        }
        
    }

    public void OnPowerToThePeopleButton()
    {
        if (turnController.powerToThepeople == false)
        {
            actionController.PowerToThePeople(currentPlayer);
            Button pttpButton = GameObject.Find("powerToThePeopleButton").GetComponent<Button>();
            pttpButton.GetComponent<Image>().color = Color.red;
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("PowerToThePeople Already in play!");
        }
    }


}
