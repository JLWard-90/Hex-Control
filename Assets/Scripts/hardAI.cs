//This file will contain the necessary functions for the hard AI mode for HexControl
//Current section to work on: preventOpponentVictory method
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
            //Do the next thing
            ActionSelected = PreventOpponentVictory(); //If the AI can stop another player from winning, then it will do so and ActionSelected will be set to true
        }
        if(ActionSelected == true)
        {
            Debug.Log("AI attempted to prevent opponent victory");
        }
        else
        {
            //Carry on down the decision tree
        }
    }

    private int chooseLobbyAction() //Here we select a lobby action at random, check if it is worth doing
    {
        int lobbyAction = 0; //If lobbyAction == 0, no action is performed. We start with this value to ensure that nothing weird is likely to happen
        
        
        if(lobbyAction == 0)
        {
            Debug.Log("No suitable lobby action was selected");
        }
        return lobbyAction;
    }

    //This method will see whether the AI can win this turn and, if it can, take the appropriate action
    private bool winningMove()
    {
        //If a winning move can be made, we execute that move and return true.
        //If a winning move cannot be made, we return false
        bool winMove = false; // We initialise with false to make sure nothing weird happens.
        //First check what the win condition is...
        int winCondition = levelcont.VictoryCondition; 
        int NlandmarkAvailable = CountAvailableCellsOfType(AIPlayer,5);
        int NResAvailable = CountAvailableCellsOfType(AIPlayer, 1);
        int NIndAvailable = CountAvailableCellsOfType(AIPlayer, 2);
        int NPowAvailable = CountAvailableCellsOfType(AIPlayer, 3);
        int NCivAvailable = CountAvailableCellsOfType(AIPlayer, 4);
        if (winCondition==0)
        {
            //Win condition is to reach a target influence level
            if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NlandmarkAvailable > 0)
            {
                //If buying a landmark will win the game and there is a landmark available that the AIplayer can afford
                //Then buy the landmark
                winMove = BuyBestcellOfType(AIPlayer, 5);
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NCivAvailable > 0)
            {
                //If we cannot get a landmark then we will try for a civic building
                winMove = BuyBestcellOfType(AIPlayer, 4);
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) + levelcont.InfPerLan >= levelcont.TargetInfluence && NResAvailable > 0)
            {
                //Try for a residential building
                winMove = BuyBestcellOfType(AIPlayer, 1);
            }
            else if(tcontrol.powerToThepeople == true)
            {
                //Else if power to the people is switched on, then buying a powerplant might win the game
                if (AIPlayer.playerInfluence + AIPlayer.tileCounts[1] * (int)(levelcont.InfPerRes * (AIPlayer.tileCounts[3]+1) * levelcont.PowerPlantMultiplier) >= levelcont.TargetInfluence && NPowAvailable > 0)
                {
                    winMove = BuyBestcellOfType(AIPlayer, 3);
                }
            }
            else if(AIPlayer.playerInfluence + tcontrol.CalculateInfIncrease(AIPlayer.playerNumber) - levelcont.InfPerInd >= levelcont.TargetInfluence && AIPlayer.tileCounts[2] > 0)
            {
                //If selling an industrial district will allow the AI player to win the game
                winMove = SellBestCellOfType(AIPlayer, 2);
            }
            else
            {
                winMove = false;
            }
        }
        else if (winCondition==1)
        {
            //Win condition is to control the map
            int sumTiles = AIPlayer.tileCounts[0] + AIPlayer.tileCounts[1] + AIPlayer.tileCounts[2] + AIPlayer.tileCounts[3] + AIPlayer.tileCounts[4] + AIPlayer.tileCounts[5];
            if(sumTiles + 1 >= levelcont.TargetCells)
            {
                //If buying one more tile will win the game
                List<GameObject> availableCells = FindAvailableCells(AIPlayer);
                if(availableCells.Count > 0) //If there are any available cells in the list
                {
                    GameObject cheapest = cheapestCell(availableCells);
                    HexCell cheapestHexCellComponent = cheapest.GetComponent<HexCell>();
                    actions.OnBuyCell(AIPlayer, cheapestHexCellComponent); //Buy the cheapest cell available to win the game
                    winMove = true;
                }
            }
        }
        else if (winCondition==2)
        {
            //Win condition is to gain target cash
            int sumTiles = AIPlayer.tileCounts[0] + AIPlayer.tileCounts[1] + AIPlayer.tileCounts[2] + AIPlayer.tileCounts[3] + AIPlayer.tileCounts[4] + AIPlayer.tileCounts[5];
            float CpI = levelcont.CashPerInd;
            float CpR = levelcont.CashPerRes;
            if (tcontrol.powerToThepeople == true)
            {
                for (int j = 0; j < AIPlayer.tileCounts[3]; j++)
                {
                    CpR = (CpR * levelcont.PowerPlantMultiplier);
                }
            }
            else
            {
                for (int j = 0; j < AIPlayer.tileCounts[3]; j++)
                {
                    CpI = (CpI * levelcont.PowerPlantMultiplier);
                }
            }

            float CpIplus1PP = CpI * levelcont.PowerPlantMultiplier;
            float CpRpluss1PP = CpR * levelcont.PowerPlantMultiplier;

            int minimumIndCost = minimumCellCost(2, AIPlayer);
            int minimumPowCost = minimumCellCost(3, AIPlayer);
            int minimumResCost = minimumCellCost(1, AIPlayer);
            if (AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) + CpI - minimumIndCost >= levelcont.TargetCash && NIndAvailable > 0)
            {
                //If buying a new industrial district makes sense
                winMove = BuyBestcellOfType(AIPlayer, 2);
            }
            else if(AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) - (CpI * AIPlayer.tileCounts[2]) + (CpIplus1PP*AIPlayer.tileCounts[2]) - minimumPowCost >= levelcont.TargetCash && NPowAvailable > 0 && tcontrol.powerToThepeople != true)
            {
                //If buying a powerplant makes sense when powerplants affect industrial districts
                winMove = BuyBestcellOfType(AIPlayer, 3);
                
            }
            else if(AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) - (CpR * AIPlayer.tileCounts[1]) + (CpRpluss1PP * AIPlayer.tileCounts[1]) - minimumPowCost >= levelcont.TargetCash && NPowAvailable >0 && tcontrol.powerToThepeople == true)
            {
                //If buying a powerplant makes sense when powerplants affect residential districts
                winMove = BuyBestcellOfType(AIPlayer, 3);
            }
            else if(AIPlayer.playerCash + tcontrol.CalculateCashIncrease(AIPlayer.playerNumber) + CpR - minimumResCost >= levelcont.TargetCash && NResAvailable > 0)
            {
                winMove = BuyBestcellOfType(AIPlayer, 1);
            }
            else if(sumTiles > 5) //If we have more than 5 tiles owned
            {
                List<GameObject> allOwnedCells = OwnedCellsAll(AIPlayer);
                GameObject mostExpensiveOwnedCell = mostExpensiveCell(allOwnedCells);
                HexCell expensiveComponent = mostExpensiveOwnedCell.GetComponent<HexCell>();
                if((expensiveComponent.cellPrice * 0.5) + AIPlayer.playerCash >= levelcont.TargetCash)
                {
                    actions.OnSellCell(AIPlayer, expensiveComponent);
                    winMove = true;
                }
                else
                {
                    winMove = false;
                }
            }
            else
            {
                winMove = false;
            }
        }
        else if(winCondition==3)
        {
            //win condition is to gain 3 landmarks
            if(AIPlayer.tileCounts[5] >= levelcont.TargetLandmarks && NlandmarkAvailable > 0)
            {
                winMove = BuyBestcellOfType(AIPlayer, 5); //If the player can get a landmark and they only need one more landmark, then buy it
            }
            else
            {
                winMove = false; //Otherwise no winning move is available.
            }
        }
        else
        {
            Debug.Log("Error encountered in hardAI.cs:winningMove:: Victory condition not found");
        }
        return winMove;
    }
    //winning move method ends
    //This method will test whether an opponent may win this turn and, if so, try to do something about it
    private bool PreventOpponentVictory()
    {
        //For each victory condition:
        //First we need to get a list of all other players
        //Then see if any of those players are about to win
        //Then decide how to prevent that from happening
        int winCondition = levelcont.VictoryCondition;
        bool takenAction = false;
        bool opponentWinPossible = false;
        Player targetPlayer = AIPlayer; //Declaring a Player component without a value may be dangerous so I will actually set the target player to be the current player for now because then it will ensure that nothing is likely to go seriously wrong
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("player");
        if (winCondition == 0)
        {
            int distanceToVictory = 1000; //
            //Victory condition is to reach target influence
            foreach(GameObject playerObject in allPlayers)
            {
                Player playerComponent = playerObject.GetComponent<Player>();
                if(playerComponent.playerNumber != AIPlayer.playerNumber && (levelcont.TargetInfluence - playerComponent.playerInfluence) < distanceToVictory && (playerComponent.playerInfluence + tcontrol.CalculateInfIncrease(playerComponent.playerNumber)) >= levelcont.TargetInfluence)
                {
                    //If the playerComponent does not belong to the target player AND the distance to victory is lower than the current stored value AND the player in question is on track to win
                    opponentWinPossible = true;
                    distanceToVictory = levelcont.TargetInfluence - playerComponent.playerInfluence; //Set the new distance to Victory (this will ensure that the player closest to winning will be targeted)
                    targetPlayer = playerComponent; //Set the target player
                }
                else
                {
                    Debug.Log("Player does not appear to be about to win");
                }
            }
            if (opponentWinPossible == true)
            {
                //If an opponent has been found that might win we will try to take some action
                //Need to get the cheapest available cells of each type for the target player
                List<GameObject> targetPlayerLandmarks = ownedCellsOfType(targetPlayer, 5);
                List<GameObject> targetPlayerCivics = ownedCellsOfType(targetPlayer, 4);
                List<GameObject> targetPlayerRes = ownedCellsOfType(targetPlayer, 1);
                if(targetPlayer.playerInfluence + tcontrol.CalculateInfIncrease(targetPlayer.playerNumber) - levelcont.InfPerLan < levelcont.TargetInfluence && targetPlayer.tileCounts[5] > 0)
                {
                    //If the removing a landmark will prevent the player from winning and the target player owns a landmark then we will see if we can buy it
                }
            }
            else
            {
                takenAction = false;
            }
        }
        else if (winCondition == 1)
        {

        }
        else if (winCondition == 2)
        {

        }
        else if (winCondition == 3)
        {

        }
        else
        {
            Debug.Log("Error encountered in hardAI.cs:preventOpponentVictory:: Victory condition not found");
        }
        return takenAction;
    }
    //Here ends the core logic methods of the AI
    //Below this point are general purpose methods for performing calculations, buying and selling districts etc.
    int CountAvailableCellsOfType(Player currentPlayer, int targetCellType)//This is very similar to FindAvailableCells and works in the same way but returns a simple integer of the number of available cells
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

    List<GameObject> FindAvailableCellsOfType(Player currentPlayer, int targetCellType) //This is very similar to Count available cells and works in the same way but returns a list of gameObjects
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

    GameObject mostExpensiveCell(List<GameObject> cellList)
    {
        int maxCellPrice = 0;
        GameObject mostExpensiveCell = new GameObject();
        bool foundMaxCell = false;
        foreach(GameObject cell in cellList)
        {
            HexCell theCellComp = cell.GetComponent<HexCell>();
            if(theCellComp.cellPrice > maxCellPrice)
            {
                maxCellPrice = theCellComp.cellPrice;
                mostExpensiveCell = cell;
                foundMaxCell = true;
            }
        }
        if(foundMaxCell != true)
        {
            Debug.Log("Error in hardAI:mostExpensiveCell :: Cannot determine most expensive cell! Input list of cells may be empty");
        }
        return mostExpensiveCell;
    }

    List<GameObject> ownedCellsOfType(Player AIPlayer, int cellType)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> ownedCells = new List<GameObject>();
        foreach(GameObject cell in fullCellsList)
        {
            HexCell hexcomp = cell.GetComponent<HexCell>();
            if(hexcomp.cellOwner == AIPlayer.playerNumber && hexcomp.cellType == cellType)
            {
                ownedCells.Add(cell);
            }
        }
        return ownedCells;
    }

    List<GameObject> OwnedCellsAll(Player currentPlayer)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> ownedCells = new List<GameObject>();
        foreach(GameObject cell in fullCellsList)
        {
            HexCell hexComponent = cell.GetComponent<HexCell>();
            if(hexComponent.cellOwner == currentPlayer.playerNumber)
            {
                ownedCells.Add(cell);
            }
        }
        return ownedCells;
    }

    List<GameObject> FindAvailableCells(Player currentPlayer)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> posscells = new List<GameObject>();
        foreach(GameObject cell in fullCellsList)
        {
            HexCell component = cell.GetComponent<HexCell>();
            if(component.cellOwner != AIPlayer.playerNumber && component.cellPrice <= AIPlayer.playerCash)
            {
                posscells.Add(cell);
            }
        }
        return posscells;
    }

    int minimumCellCost(int cellType, Player currentPlayer)
    {
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        List<GameObject> possCells = new List<GameObject>();
        int minimum = 1000000;
        foreach(GameObject cell in fullCellsList)
        {
            HexCell hexComponent = cell.GetComponent<HexCell>();
            if(hexComponent.cellType == cellType && hexComponent.cellOwner != currentPlayer.playerNumber && hexComponent.cellPrice < minimum)
            {
                minimum = hexComponent.cellPrice;
            }
        }
        return minimum;
    }

    bool BuyBestcellOfType(Player currentPlayer, int cellTypeNo)
    {
        List<GameObject> availableCellsOfType = FindAvailableCellsOfType(currentPlayer, cellTypeNo);
        GameObject CellToBuy = cheapestCell(availableCellsOfType);
        HexCell theCell = CellToBuy.GetComponent<HexCell>(); //Get the HexCell component of the cell to buy
        actions.OnBuyCell(AIPlayer, theCell); //Buy the cell
        bool winMove = true; //Confirm that a winning move has been performed.
        return winMove;
    }
    bool SellBestCellOfType(Player currentPlayer, int cellTypeNo)
    {
        List<GameObject> ownedIndCells = ownedCellsOfType(currentPlayer, cellTypeNo);
        GameObject theCellToSell = mostExpensiveCell(ownedIndCells);
        HexCell theHexCellToSell = theCellToSell.GetComponent<HexCell>();
        actions.OnSellCell(AIPlayer, theHexCellToSell);
        bool winMove = true;
        return winMove;
    }
}