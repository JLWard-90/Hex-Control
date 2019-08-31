//This file will contain the necessary functions for the hard AI mode for HexControl
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardAIController : MonoBehaviour
{
    TurnController tcontrol;
    LevelController levelcont;
    ActionController actions;
    Player AIPlayer;

    private void Awake()
    {
        tcontrol = GameObject.Find("GameController").GetComponent<TurnController>();
        levelcont = GameObject.Find("GameController").GetComponent<LevelController>();
        actions = GameObject.Find("GameController").GetComponent<ActionController>();
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
        int NResAvailable = CountAvailableCells(AIPlayer, 1);
        int NIndAvailable = CountAvailableCells(AIPlayer, 2);
        int NPowAvailable = CountAvailableCells(AIPlayer, 3);
        int NCivAvailable = CountAvailableCells(AIPlayer, 4);
        if (winCondition==0)
        {
            //Win condition is to reach a target influence level
            if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NlandmarkAvailable > 0)
            {
                //If buying a landmark will win the game and there is a landmark available that the AIplayer can afford
                //Then buy the landmark
                List<GameObject> availableLandmarks = FindAvailableCells(AIPlayer, 5); //First get the list of available landmarks
                GameObject CellToBuy = cheapestCell(availableLandmarks); //Then select the cheapest landmark cell available.
                HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
                actions.OnBuyCell(AIPlayer, theCell); //Buy the cell
                winMove = true; //Confirm that a winning move has been performed.
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NCivAvailable > 0)
            {
                //If we cannot get a landmark then we will try for a civic building
                List<GameObject> availableCivics = FindAvailableCells(AIPlayer, 4);
                GameObject CellToBuy = cheapestCell(availableCivics);
                HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
                actions.OnBuyCell(AIPlayer, theCell); //Buy the cell
                winMove = true; //Confirm that a winning move has been performed.
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NResAvailable > 0)
            {
                //Try for a residential building
                List<GameObject> availableCellsOfType = FindAvailableCells(AIPlayer, 1);
                GameObject CellToBuy = cheapestCell(availableCellsOfType);
                HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
                actions.OnBuyCell(AIPlayer, theCell); //Buy the cell
                winMove = true; //Confirm that a winning move has been performed.
            }
            else if(tcontrol.powerToThepeople == true)
            {
                //Else if power to the people is switched on, then buying a powerplant might win the game
                if (AIPlayer.playerInfluence + AIPlayer.tileCounts[1] * (int)(levelcont.InfPerRes * (AIPlayer.tileCounts[3]+1) * levelcont.PowerPlantMultiplier) >= levelcont.TargetInfluence && NPowAvailable > 0)
                {
                    List<GameObject> availableCellsOfType = FindAvailableCells(AIPlayer, 3);
                    GameObject CellToBuy = cheapestCell(availableCellsOfType);
                    HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
                    actions.OnBuyCell(AIPlayer, theCell); //Buy the cell
                    winMove = true; //Confirm that a winning move has been performed.
                }
            }
            else
            {
                winMove = false;
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

    int CountAvailableCells(Player currentPlayer, int targetCellType)//This is very similar to FindAvailableCells and works in the same way but returns a simple integer of the number of available cells
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell"); //Get all of the objects with the tag "HexCells" This should be every cell on the board and nothing else
        int count = 0;//Start a counter with the value 0 
        foreach(GameObject cell in fullCellsList)
        {
            HexCell theCellComponent = cell.GetComponent<HexCell>(); //Get the HexCell component of the HexCell game object
            if(theCellComponent.cellType == targetCellType && AIPlayer.playerCash <= theCellComponent.cellPrice && theCellComponent.cellOwner != AIPlayer.playerNumber) //Check that cell price is the correct variable name
            {
                //If the cell is of the correct type and the player can afford to buy it
                count++;
            }
          /*  else //This else statement should be commented out unless we need to test that the default state is proceding as expected. Otherwise it will just clutter up the log
            {
                Debug.Log("Everything is probably fine.");
            }*/
        }
        return count;
    }

    List<GameObject> FindAvailableCells(Player currentPlayer, int targetCellType) //This is very similar to Count available cells and works in the same way but returns a list of gameObjects
    {
        List<GameObject> possibleCells = new List<GameObject>();
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        foreach(GameObject cell in fullCellsList)
        {
            HexCell theCellComponent = cell.GetComponent<HexCell>();
            if (theCellComponent.cellType == targetCellType && AIPlayer.playerCash <= theCellComponent.cellPrice && theCellComponent.cellOwner != AIPlayer.playerNumber) //Check that cell price is the correct variable name
            {
                //If the cell is of the correct type and the player can afford to buy it
                possibleCells.Add(cell);//Add the HexCell gameObject to the list
            }
            /*  else //This else statement should be commented out unless we need to test that the default state is proceding as expected. Otherwise it will just clutter up the log
            {
                Debug.Log("Everything is probably fine.");
            }*/
        }
        return possibleCells;
    }

    GameObject cheapestCell(List<GameObject> cellList) //This method finds the cheapest
    {
        int minCellPrice = 100000; //Start at an arbitrarily large number that would not be found within the main game
        GameObject cheapestCell = new GameObject(); //Assign cheapest cell to be an empty game object on initialisation. This is simply to avoid errors.
        bool foundMinCell = false; //This boolean will just be used to write an error message to the log if a cheapest cell cannot be found
        foreach(GameObject cell in cellList)
        {
            HexCell hexCellComp = cell.GetComponent<HexCell>();
            if(hexCellComp.cellPrice < minCellPrice)
            {
                minCellPrice = hexCellComp.cellPrice;
                cheapestCell = cell;
                foundMinCell = true;
            }
        }
        if(foundMinCell != true)
        {
            Debug.Log("Error in hardAI.cs:cheapestCell :: Cannot determine cheapest cell! Input list of cells may be empty");
        }
        return cheapestCell;
    }
}