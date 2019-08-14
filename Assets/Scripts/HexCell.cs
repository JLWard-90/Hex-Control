using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour {

    public HexCoordinates coordinates;

    public Color color = Color.white;

    public bool selectedCell = false; //Boolean to say whether this cell is selected or not

    public int cellType = 0; //Cell type is type of region: 
    public string cellTypeString;
    public int cellOwner = -1; //Owner of each cell
    public string cellOwnerString = "None";
    public int cellPrice = 200; //Price of cell
    public string CellTypeShortString = "N/A";
    public Color CellOwnerColor = Color.white;

    GameController gController;
    LevelController lCont;

    private void Awake()
    {
        gController = GameObject.Find("GameController").GetComponent<GameController>();
        color = Color.gray;
        SetCellType();
        cellOwner = -1;
    }

     void SetCellType()
    {
        //cellType = Random.Range(1, 5); //Old for testing
        lCont = GameObject.Find("LevelController").GetComponent<LevelController>();
        int total = lCont.Residential + lCont.Industrial + lCont.Power + lCont.Civic + lCont.Landmark;
        int cellTypenum = Random.Range(1, total);
        int Reslim = lCont.Residential;
        int Indlim = lCont.Residential + lCont.Industrial;
        int Powlim = Indlim + lCont.Power;
        int civLim = Powlim + lCont.Civic;
        if(cellTypenum <= Reslim)
        {
            cellType = 1;
        }
        else if(cellTypenum <= Indlim)
        {
            cellType = 2;
        }
        else if(cellTypenum <= Powlim)
        {
            cellType = 3;
        }
        else if(cellTypenum <= civLim)
        {
            cellType = 4;
        }
        else if(cellTypenum <= total)
        {
            cellType = 5;
        }
        else
        {
            cellType = 0;
        }

        SetCellTypeProperties();
    }

    public void SetCellTypeProperties() //This lists the properties for each cell type so that it is properly represented on the map.
    {
        if(cellType == 0) //Empty cell
        {
            cellTypeString = "Empty";
            CellTypeShortString = "N/A";
            if(cellOwner == -1)
            {
                cellPrice = lCont.CellPrices[cellType];
            }
        }
        if(cellType == 1) // Residential
        {
            cellTypeString = "Residential";
            CellTypeShortString = "R";
            if (cellOwner == -1)
            {
                cellPrice = lCont.CellPrices[cellType];
            }
        }
        if (cellType == 2)//Industrial
        {
            cellTypeString = "Industrial";
            CellTypeShortString = "I";
            if (cellOwner == -1)
            {
                cellPrice = lCont.CellPrices[cellType];
            }
        }
        if (cellType == 3)//Power supply
        {
            cellTypeString = "Power Plant";
            CellTypeShortString = "P";
            if (cellOwner == -1)
            {
                cellPrice = lCont.CellPrices[cellType];
            }
        }
        if(cellType == 4)//Civic
        {
            cellTypeString = "Civic";
            CellTypeShortString = "C";
            if (cellOwner == -1)
            {
                cellPrice = lCont.CellPrices[cellType];
            }
        }
        if(cellType == 5)//Landmark
        {
            cellTypeString = "Landmark";
            CellTypeShortString = "L";
            if (cellOwner == -1)
            {
                cellPrice = lCont.CellPrices[cellType];
            }
        }
    }
}
