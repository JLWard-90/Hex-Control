using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenuController : MonoBehaviour {

    ActionController actionController;
    Player currentPlayer;
    Player[] theplayers;
    LevelController lCont;
    TurnController turnController;
    HexGrid theGrid;
    GameObject[] instance;

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
        }
        
    }
}
