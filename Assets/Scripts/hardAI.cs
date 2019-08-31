//This file will contain the necessary functions for the hard AI mode for HexControl
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardAIController : MonoBehaviour
{
    TurnController tcontrol;
    LevelController levelcont;
    Player AIPlayer;

    private void Awake()
    {
        tcontrol = GameObject.Find("GameController").GetComponent<TurnController>();
        levelcont = GameObject.Find("GameController").GetComponent<LevelController>();
    }

    public void hardAIAction()
    {
        bool ActionSelected = false; //Keep a running track of whether or not we have arrived at a decision
        ActionSelected = winningMove(); //If a winning move is possible, then it will be carried out and ActionSelected will be set to true
        if(ActionSelected == true)
        {
            Debug.Log("AI played a winning move");
        }
        else
        {
            //If the winning move did not succeed then we need to carry on down the tree
            Debug.Log("Winning move not available at this time");
        }
    }

    private int chooseLobbyAction()
    {
        int lobbyAction = 0; //If lobbyAction == 0, no action is performed. We start with this value to ensure that nothing weird is likely to happen
        
        
        if(lobbyAction == 0)
        {
            Debug.Log("No suitable lobby action was selected");
        }
        return lobbyAction;
    }

    private bool winningMove()
    {
        //If a winning move can be made, we execute that move and return true.
        //If a winning move cannot be made, we return false
        bool winMove = false; // We initialise with false to make sure nothing weird happens.
        //First check what the win condition is...
        int winCondition = levelcont.VictoryCondition; //Check that this is the correct variable (and in the correct class!)
        int NlandmarkAvailable = CountAvailableCells(AIPlayer,5);
        if (winCondition==0)
        {
            //Win condition is to reach a target influence level
            if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NlandmarkAvailable > 0)
            {
                //If buying a landmark will win the game and there is a landmark available
                //Then buy the landmark
            }
        }
        else if (winCondition==1)
        {
            //Win condition is to control entire map
        }
        else if (winCondition==2)
        {
            //Win condition is to gain target cash
        }
        else if(winCondition==3)
        {
            //win condition is to gain 3 landmarks
        }
        else
        {
            Debug.Log("Error encountered in hardAI.cs:winningMove:: Victory condition not found");
        }
        return winMove;
    }

    int CountAvailableCells(Player currentPlayer, int targetCellType)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell"); //This line needs finishing! Where can we get all of the cells from???
        int count = 0;//Start a counter with the value 0 
        List<GameObject> possibleCells = new List<GameObject>();
        foreach(GameObject cell in fullCellsList)
        {
            HexCell theCellComponent = cell.GetComponent<HexCell>(); //Get the HexCell component of the HexCell game object
            if(theCellComponent.cellType == targetCellType && AIPlayer.playerCash <= theCellComponent.cellPrice) //Check that cell price is the correct variable name
            {
                //If the cell is of the correct type and the player can afford to buy it
            }
            else
            {
                Debug.Log("Everything is probably fine.");
            }
        }
        return count;
    }

    List<GameObject> FindAvailableCells(Player currentPlayer, int cellTypeInt)
    {
        List<GameObject> possibleCells = new List<GameObject>();
        return possibleCells;
    }
}