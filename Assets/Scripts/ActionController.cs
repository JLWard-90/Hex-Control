using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

    HexGrid theGrid;
    HexMesh hexMesh;
    UIController uIController;
    LevelController level;
    TurnController tcontroller;

    [SerializeField]
    int greenInitiativeCost = 100;
    [SerializeField]
    int greenInitiativePPdrain = 200;
    [SerializeField]
    int changeToIndCost = 100;
    [SerializeField]
    int powerToThePeopleCost = 100;
    [SerializeField]
    int championOfIndustryCost = 100;
    [SerializeField]
    int ruleOfLawCost = 100;
    [SerializeField]
    int bribesAsIndustryCost = 100;
    [SerializeField]
    int moveGoalpostCost = 300;
    [SerializeField]
    int rentHikeCost = 100;
    [SerializeField]
    int decentraliseGovernmentCost = 100;
    [SerializeField]
    int freeHousingInitiativeCost = 100;
    [SerializeField]
    int officeRefurbishmentCost = 100;
    [SerializeField]
    int officeRefurbishmentPenalty = 200;

    AudioController audioCont;

    private void Awake()
    {
        theGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        hexMesh = theGrid.GetComponentInChildren<HexMesh>();
        uIController = GameObject.Find("UICanvas").GetComponent<UIController>();
        level = GameObject.Find("LevelController").GetComponent<LevelController>();
        audioCont = GameObject.Find("AudioController").GetComponent<AudioController>();
        tcontroller = GameObject.Find("GameController").GetComponent<TurnController>();
    }

    public void OnBuyCell(Player CurrentPlayer, HexCell Cell)
    {
        if (theGrid.SelectedCellIndex != -1) //This only makes sense if the player is human!
        {
            if (Cell.cellPrice <= CurrentPlayer.playerCash)
            {
                CurrentPlayer.playerCash -= Cell.cellPrice;
                CurrentPlayer.tileCounts[Cell.cellType]++;
                if(Cell.cellOwner != -1) //If the cell already has an owner
                {
                    level.players[Cell.cellOwner].tileCounts[Cell.cellType]--; //Take the cell away from the old owner's cell count
                    level.players[Cell.cellOwner].playerCash += Cell.cellPrice; //Pay the money directly to the previous owner of the cell.
                }
                Cell.cellOwner = CurrentPlayer.playerNumber;
                audioCont.PlayBuySound();
                Cell.cellOwnerString = CurrentPlayer.PlayerName;
                Cell.color = CurrentPlayer.PlayerColor;
                Cell.CellOwnerColor = CurrentPlayer.PlayerColor;
                Cell.cellPrice = Cell.cellPrice + (Cell.cellPrice / 2);
                uIController.UpdateInfoPanel();
                uIController.UpdateCellPanel(Cell);
                hexMesh.Triangulate(theGrid.cells);
                uIController.updateMessageBox(CurrentPlayer.PlayerName, "bought a district");
            }
            else
            {
                Debug.Log("Not enough money to buy this cell.");
            }
        }
        else //Follow exactly the same procedure if it is an AI player but do not require a cell to be selected with the mouse
        {
            if (CurrentPlayer.AIplayer)
            {
                if (Cell.cellPrice <= CurrentPlayer.playerCash)
                {
                    CurrentPlayer.playerCash -= Cell.cellPrice;
                    CurrentPlayer.tileCounts[Cell.cellType]++;
                    if (Cell.cellOwner != -1) //If the cell already has an owner
                    {
                        level.players[Cell.cellOwner].tileCounts[Cell.cellType]--; //Take the cell away from the old owner's cell count
                        level.players[Cell.cellOwner].playerCash += Cell.cellPrice; //Pay the money directly to the previous owner of the cell.
                    }
                    Cell.cellOwner = CurrentPlayer.playerNumber;
                    audioCont.PlayBuySound();
                    Cell.cellOwnerString = CurrentPlayer.PlayerName;
                    Cell.color = CurrentPlayer.PlayerColor;
                    Cell.CellOwnerColor = CurrentPlayer.PlayerColor;
                    Cell.cellPrice = Cell.cellPrice + (Cell.cellPrice / 2);
                    uIController.UpdateInfoPanel();
                    uIController.UpdateCellPanel(Cell);
                    hexMesh.Triangulate(theGrid.cells);
                    uIController.updateMessageBox(CurrentPlayer.PlayerName, "bought a district");
                }
            }
        }

    }

    public void OnSellCell(Player CurrentPlayer, HexCell Cell)
    {
        if (theGrid.SelectedCellIndex != -1) //This only makes sense if the player is human!
        {
            if(CurrentPlayer.playerNumber == Cell.cellOwner)
            {
                int StartCellValue = Cell.cellPrice;
                Cell.cellPrice = StartCellValue / 2;
                CurrentPlayer.playerCash += Cell.cellPrice;
                CurrentPlayer.tileCounts[Cell.cellType]--;
                Cell.cellOwner = -1;
                audioCont.PlaySellSound();
                Cell.cellOwnerString = "None";
                Cell.CellOwnerColor = Color.white;
                Cell.color = Cell.CellOwnerColor;
                uIController.UpdateInfoPanel();
                uIController.UpdateCellPanel(Cell);
                hexMesh.Triangulate(theGrid.cells);
                uIController.updateMessageBox(CurrentPlayer.PlayerName, "sold a district");
            }
        }
        else 
        { 
            if (CurrentPlayer.AIplayer) //Follow exactly the same procedure if it is an AI player but do not require a cell to be selected with the mouse
            {
                if (CurrentPlayer.playerNumber == Cell.cellOwner)
                {
                    int StartCellValue = Cell.cellPrice;
                    Cell.cellPrice = StartCellValue / 2;
                    CurrentPlayer.playerCash += Cell.cellPrice;
                    CurrentPlayer.tileCounts[Cell.cellType]--;
                    Cell.cellOwner = -1;
                    audioCont.PlaySellSound();
                    Cell.cellOwnerString = "None";
                    Cell.CellOwnerColor = Color.white;
                    Cell.color = Cell.CellOwnerColor;
                    uIController.UpdateInfoPanel();
                    uIController.UpdateCellPanel(Cell);
                    hexMesh.Triangulate(theGrid.cells);
                    uIController.updateMessageBox(CurrentPlayer.PlayerName, "sold a district");
                }
            }
        }
    }

    //The following are all of the Lobbying actions. The exact actions that are available will be randomised at the start of each game.

    public void OnGreenEnergyInitiative(Player CurrentPlayer, Player[] players) //Lobbying ability that charges every player on the board per power plant
    {
        int influenceCost = greenInitiativeCost;
        int CostPerPP = greenInitiativePPdrain;
        if(CurrentPlayer.playerInfluence >= influenceCost)
        {
            CurrentPlayer.playerInfluence -= influenceCost;
            foreach(Player player in players)
            {
                player.playerCash -= CostPerPP * player.tileCounts[3];
            }
            audioCont.PlayLobbySound();
            uIController.UpdateInfoPanel();
            uIController.updateMessageBox(CurrentPlayer.PlayerName, "played green energy initiative");
        }
        else
        {
            Debug.Log("insufficient influence");
        }
    }

    public void ChangeCellType(HexCell cell, Player CurrentPlayer,int newCellType, int cellIndex)
    {
        if (cell.cellOwner == CurrentPlayer.playerNumber)
        {
            if(CurrentPlayer.playerInfluence >= changeToIndCost)
            {
                CurrentPlayer.playerInfluence -= changeToIndCost;
                CurrentPlayer.tileCounts[cell.cellType]--;
                theGrid.destroyModel(cell);
                cell.cellType = newCellType;
                cell.SetCellTypeProperties();
                CurrentPlayer.tileCounts[cell.cellType]++;
                hexMesh.Triangulate(theGrid.cells);
                theGrid.UpdateCellLabel(cell, cellIndex);
                audioCont.PlayLobbySound();
                Debug.Log("Changed cell to Industrial district");
                string messagestring = "changed a district to " + cell.cellTypeString;
                uIController.updateMessageBox(CurrentPlayer.PlayerName, messagestring);
            }
            else
            {
                Debug.Log("Insufficient influence.");
            }
        }
        else
        {
            Debug.Log("Cell does not belong to current player.");
        }
    }

    public void PowerToThePeople(Player CurrentPlayer)
    {
        if(CurrentPlayer.playerInfluence >= powerToThePeopleCost)
        {
            tcontroller.powerToThepeople = true;
            CurrentPlayer.playerInfluence -= powerToThePeopleCost;
            uIController.updateMessageBox(CurrentPlayer.PlayerName, "played Power to the people");
        }
        
    }

    public void ChampionOfIndustry(Player CurrentPlayer)
    {
        //Increases industrial district output by 10%
        if(CurrentPlayer.playerInfluence >= championOfIndustryCost)
        {
            tcontroller.championOfIndustry = true;
            CurrentPlayer.playerInfluence -= championOfIndustryCost;
            uIController.updateMessageBox(CurrentPlayer.PlayerName, "played Champion of Industry");
        }
    }

    public void RuleOfLaw(Player CurrentPlayer)
    {
        //Increases Civic district influence output by 10%
        if(CurrentPlayer.playerInfluence >= ruleOfLawCost)
        {
            tcontroller.ruleOfLaw = true;
            CurrentPlayer.playerInfluence -= ruleOfLawCost;
            uIController.updateMessageBox(CurrentPlayer.PlayerName, "played Rule of Law");
        }
    }

    public void BribesAsIndustry(Player CurrentPlayer)
    {
        //Decreases the cash cost of maintaining civic buildings by 20%
        if(CurrentPlayer.playerInfluence >= bribesAsIndustryCost)
        {
            tcontroller.bribesAsIndustry = true;
            CurrentPlayer.playerInfluence -= bribesAsIndustryCost;
            uIController.updateMessageBox(CurrentPlayer.PlayerName, "played Bribes as Industry");
        }
    }

    public void MoveGoalposts(Player CurrentPlayer)
    {
        LevelController levelController = GameObject.Find("LevelController").GetComponent<LevelController>();
        int winCondition = levelController.VictoryCondition;
        int Nlandmarks = 0;
        GameObject[] fullCellsList = GameObject.FindGameObjectsWithTag("HexCell");
        foreach(GameObject cell in fullCellsList)
        {
            HexCell cellComponent = cell.GetComponent<HexCell>();
            if(cellComponent.cellType == 5)
            {
                Nlandmarks++;
            }
        }
        if (CurrentPlayer.playerInfluence >= moveGoalpostCost)
        {
            if (Nlandmarks >= 3)
            {
                if (winCondition == 0)
                {
                    levelController.VictoryCondition = Random.Range(1, 4);
                }
                else if(winCondition == 1)
                {
                    int randNum = Random.Range(1, 4);
                    if(randNum == 1)
                    {
                        levelController.VictoryCondition = 0;
                    }
                    else if(randNum == 2)
                    {
                        levelController.VictoryCondition = 2; 
                    }
                    else if (randNum == 3)
                    {
                        levelController.VictoryCondition = 3;
                    }
                }
                else if (winCondition == 2)
                {
                    int randNum = Random.Range(1, 4);
                    if (randNum == 1)
                    {
                        levelController.VictoryCondition = 0;
                    }
                    else if (randNum == 2)
                    {
                        levelController.VictoryCondition = 1;
                    }
                    else if (randNum == 3)
                    {
                        levelController.VictoryCondition = 3;
                    }
                }
                else if (winCondition == 3)
                {
                    int randNum = Random.Range(1, 4);
                    if (randNum == 1)
                    {
                        levelController.VictoryCondition = 0;
                    }
                    else if (randNum == 2)
                    {
                        levelController.VictoryCondition = 2;
                    }
                    else if (randNum == 3)
                    {
                        levelController.VictoryCondition = 1;
                    }
                }
            }
            else
            {
                if (winCondition == 0)
                {
                    levelController.VictoryCondition = Random.Range(1, 3);
                }
                else if (winCondition == 1)
                {
                    int randNum = Random.Range(1, 3);
                    if (randNum == 1)
                    {
                        levelController.VictoryCondition = 0;
                    }
                    else
                    {
                        levelController.VictoryCondition = 2;
                    }
                }
                else if (winCondition == 2)
                {
                    int randNum = Random.Range(1, 3);
                    if (randNum == 1)
                    {
                        levelController.VictoryCondition = 0;
                    }
                    else
                    {
                        levelController.VictoryCondition = 1;
                    }
                }
            }
            if (levelController.VictoryCondition == 0)
            {
                uIController.updateMessageBox(CurrentPlayer.PlayerName, "changed the victory condition to Influencer");
            }
            else if (levelController.VictoryCondition == 1)
            {
                uIController.updateMessageBox(CurrentPlayer.PlayerName, "changed the victory condition to Domination");
            }
            else if (levelController.VictoryCondition == 2)
            {
                uIController.updateMessageBox(CurrentPlayer.PlayerName, "changed the victory condition to Mogul");
            }
            else if (levelController.VictoryCondition == 3)
            {
                uIController.updateMessageBox(CurrentPlayer.PlayerName, "changed the victory condition to Landmark");
            }
            else
            {
                Debug.Log("Something has gone wrong! No longer recognise victory condition (ActionController::MoveGoalposts)");
            }

        }
    }

    public void RentHike(Player currentPlayer)
    {
        //increase cash from residences by 50% but decreases their influence to 0
        if(currentPlayer.playerInfluence >= rentHikeCost)
        {
            tcontroller.rentHike = true;
            currentPlayer.playerInfluence -= rentHikeCost;
            uIController.updateMessageBox(currentPlayer.PlayerName, "played Rent Hike");
        }
    }

    public void DecentraliseGovernment(Player currentPlayer)
    {
        //decrease civic influence and cost
        if(currentPlayer.playerInfluence >= decentraliseGovernmentCost)
        {
            tcontroller.decentraliseGovernment = true;
            currentPlayer.playerInfluence -= decentraliseGovernmentCost;
            uIController.updateMessageBox(currentPlayer.PlayerName, "played decentralise government");
        }
    }

    public void FreeHousingInitiative(Player currentPlayer)
    {
        if(currentPlayer.playerInfluence >= freeHousingInitiativeCost)
        {
            tcontroller.freeHousingInitiative = true;
            currentPlayer.playerInfluence -= freeHousingInitiativeCost;
            uIController.updateMessageBox(currentPlayer.PlayerName, "played Free Housing Initiative");
        }
    }

    public void OfficeRefurbishment(Player currentPlayer, Player[] players)
    {
        if (currentPlayer.playerInfluence >= officeRefurbishmentCost)
        {
            foreach (Player player in players)
            {
                player.playerCash -= officeRefurbishmentPenalty * player.tileCounts[3];
            }
            currentPlayer.playerInfluence -= officeRefurbishmentCost;
            uIController.updateMessageBox(currentPlayer.PlayerName, "played Office Refurbishment Programme");
        }
    }

    public void RepealEdict(Player currentPlayer)
    {
        Debug.Log("Sorry repeal action is not yet implemented");
    }

    public void PerformLobbyAction(int actionToPerform, Player currentPlayer, Player[] thePlayers)
    {
        if (actionToPerform == 0)
        {
            //Repeal button selected
            RepealEdict(currentPlayer);
        }
        else if (actionToPerform == 1)
        {
            //Green Energy Initiative
            OnGreenEnergyInitiative(currentPlayer, thePlayers);
        }
        else if (actionToPerform == 2)
        {
            //Power To the people
            PowerToThePeople(currentPlayer);
        }
        else if (actionToPerform == 3)
        {
            //Rule of Law
            RuleOfLaw(currentPlayer);
        }
        else if (actionToPerform == 4)
        {
            //Bribes as Industry
            BribesAsIndustry(currentPlayer);
        }
        else if (actionToPerform == 5)
        {
            //Move Goalposts
            MoveGoalposts(currentPlayer);
        }
        else if (actionToPerform == 6)
        {
            //Rent hike
            RentHike(currentPlayer);
        }
        else if (actionToPerform == 7)
        {
            //Decentralise government
            DecentraliseGovernment(currentPlayer);
        }
        else if (actionToPerform == 8)
        {
            //Office refurbishment
            OfficeRefurbishment(currentPlayer, thePlayers);
        }
        else if (actionToPerform == 9)
        {
            //Free housing initiative
            FreeHousingInitiative(currentPlayer);
        }
        else
        {
            Debug.Log("Error in LobbyMenuController::performLobbyAction : unkown actionToPerform");
        }
    }

}
