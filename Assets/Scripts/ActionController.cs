using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour {

    HexGrid theGrid;
    HexMesh hexMesh;
    UIController uIController;
    LevelController level;

    [SerializeField]
    int greenInitiativeCost = 100;
    [SerializeField]
    int greenInitiativePPdrain = 100;
    [SerializeField]
    int changeToIndCost = 100;

    AudioController audioCont;

    private void Awake()
    {
        theGrid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        hexMesh = theGrid.GetComponentInChildren<HexMesh>();
        uIController = GameObject.Find("UICanvas").GetComponent<UIController>();
        level = GameObject.Find("LevelController").GetComponent<LevelController>();
        audioCont = GameObject.Find("AudioController").GetComponent<AudioController>();
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
            uIController.updateMessageBox(CurrentPlayer.PlayerName, "lobbied for a green energy initiative");
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
                uIController.updateMessageBox(CurrentPlayer.PlayerName, "changed a cell type to industrial");
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
}
