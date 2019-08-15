using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    TurnController turnController;
    int aiLevel = 0; //[0] the simplest possible AI. Makes decisions at random.
    Player AIPlayer;
    LevelController level;
    ActionController actions;//Need this to be able to perform actions

    private void Awake()
    {
        turnController = GameObject.Find("GameController").GetComponent<TurnController>();
        AIPlayer = this.GetComponent<Player>();
        level = GameObject.Find("LevelController").GetComponent<LevelController>();
        actions = GameObject.Find("GameController").GetComponent<ActionController>();
    }

    int ChooseAction() //Actions
    {
        //This method will choose which action the AI will take. [0] = No action, [1] = Buy, [2] = sell, [3] = Lobby
        int ActionToTake = 0;
        if (aiLevel == 0)
        {
            int numberOfCellsOwned = AIPlayer.tileCounts[0] + AIPlayer.tileCounts[1] + AIPlayer.tileCounts[2] + AIPlayer.tileCounts[3] + AIPlayer.tileCounts[4] + AIPlayer.tileCounts[5];//This is the number of tiles that the AI player owns
            if (numberOfCellsOwned >= 2)
            {
                float randomNumber = Random.Range(0, 10);
                if(randomNumber >= 4)
                {
                    ActionToTake = 1; //Take the Buy action
                } else if(randomNumber >= 2)
                {
                    ActionToTake = 2; //Take the sell action
                }
                else
                {
                    ActionToTake = 0; //perform no action
                }
            }
            else
            {
                ActionToTake = 1;
            }
        }
        return ActionToTake;
    }

    GameObject ChooseCell(int action, int targetCellType)
    {
        //This method will choose the cell on which to perform the action if necessary.
        GameObject thisCell;
        GameObject[] listHexCells;
        HexCell cellCont;
        List<GameObject> possibleCells = new List<GameObject>(); //This contains the cells that match the type that we are looking for.
        listHexCells = GameObject.FindGameObjectsWithTag("HexCell");
        int Nyes = 0;
        int indexCount = 0;
        foreach (GameObject Cell in listHexCells)
        {
            indexCount++;
            cellCont = Cell.GetComponent<HexCell>();
            //Debug.Log("Cell type: " + cellCont.cellType);
            if(cellCont.cellType == targetCellType)
            {
                possibleCells.Add(Cell);
                Nyes++;
                Debug.Log("Found cell" + indexCount);
            }
        }
        Debug.Log("Cells tested: " + indexCount);
        if(Nyes == 1)
        {
            thisCell = possibleCells[0];
        } else if (Nyes > 1)
        {
            int randomNumber = Random.Range(0, Nyes-1);
            thisCell = possibleCells[randomNumber];
        }
        else
        {
            thisCell = new GameObject();
        }
        return thisCell;
   }

    public int ChooseTargetCellType(int Action)
    {
        if (Action == 1)//If the action is to buy
        {
            if(turnController.CalculateCashIncrease(AIPlayer.playerNumber) <= 0) //If losing money
            {
                if (AIPlayer.playerCash >= level.IndCost) //If can afford industry tile
                {
                    return 2;
                } else if(AIPlayer.playerCash >= level.ResCost) //If can afford residential
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            } else if (turnController.CalculateInfIncrease(AIPlayer.playerNumber) <= 0){ //If not gaining influence
                if(AIPlayer.playerCash >= level.CivCost)
                {
                    return 4;
                } else if(AIPlayer.playerCash >= level.ResCost)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return Random.Range(0, 4); //Otherwise just choose a tile at random to try to buy
            }
        } else if (Action == 2) //If the action is to sell
        {
            if (turnController.CalculateInfIncrease(AIPlayer.playerNumber) <= 0)
            {
                if (AIPlayer.tileCounts[1] > 0)
                {
                    return 2;
                } else if (AIPlayer.tileCounts[2] > 0)
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            } else if (turnController.CalculateCashIncrease(AIPlayer.playerNumber) <= 0)
            {
                if(AIPlayer.tileCounts[5] > 0)
                {
                    return 5;
                } else if (AIPlayer.tileCounts[4] > 0)
                {
                    return 4;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                int RandomNumbernew = Random.Range(0, 1);
                if (RandomNumbernew == 1){
                    if (AIPlayer.tileCounts[1] > 0)
                    {
                        return 1;
                    } else if (AIPlayer.tileCounts[3] > 0)
                    {
                        return 3;
                    } else if(AIPlayer.tileCounts[5] > 0)
                    {
                        return 5;
                    } else if(AIPlayer.tileCounts[2] > 0)
                    {
                        return 2;
                    } else if (AIPlayer.tileCounts[4] > 0)
                    {
                        return 4;
                    }
                    else { return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        else //If the action is not buying or selling.
        {
            return 0;
        }
        
    }

    public void TakeAction()
    {
        int actionToTake = ChooseAction();
        Debug.Log("Action chosen: " + actionToTake);
        int targetCellType = ChooseTargetCellType(actionToTake); //This needs a function that will decide what cell type to go for.
        Debug.Log("Target cell type: " + targetCellType);
        GameObject theCell = ChooseCell(actionToTake, targetCellType);
        if (theCell.GetComponent<HexCell>())
        {
            Debug.Log("AI to take action, Action: " + actionToTake + ", cell: " + theCell);
        }
        else
        {
            Debug.Log("could not find suitable cell...");
            actionToTake = 0;
        }
        if(actionToTake == 0) //If there is no available/desirable action to take
        {
            Debug.Log("Action to take == 0");
        }
        else
        {
            if (actionToTake == 1) //If buying
            {
                Debug.Log("AI player buying cell");
                actions.OnBuyCell(AIPlayer, theCell.GetComponent<HexCell>());
            } else if (actionToTake == 2) //If selling
            {
                actions.OnSellCell(AIPlayer, theCell.GetComponent<HexCell>());
            } else
            {
                Debug.Log("Invalid action selected");
            }
        }
    }

    public void RunAITurn()
    {
        StartCoroutine(TakeActionCoroutine());
    }

    IEnumerator TakeActionCoroutine()
    {
        Debug.Log("Called RunAITurn");
        int nActionsToTake = 2;
        for (int i = 0; i < nActionsToTake; i++)
        {
            yield return new WaitForSeconds(0.5f);
            TakeAction();
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("AI turn complete");
        turnController.EndTurn();
    }
    
}
