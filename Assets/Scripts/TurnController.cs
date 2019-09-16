using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour {

    public int CurrentPlayer;
    public int CurrentTurn;
    int TotalPlayerNumber;
    LevelController lCont;
    Player[] players;
    Player thePlayer;
    UIController uiC;
    int WinCondition = 0;
    Player WinningPlayer;
    HexCell[] Cells;
    HexGrid Grid;
    public bool powerToThepeople = false;
    public bool championOfIndustry = false;
    public bool ruleOfLaw = false;
    public bool bribesAsIndustry = false;
    public bool rentHike = false;
    public bool decentraliseGovernment = false;
    public bool freeHousingInitiative = false;

    private void Awake()
    {
        lCont = GameObject.Find("LevelController").GetComponent<LevelController>();
        uiC = GameObject.Find("UICanvas").GetComponent<UIController>();
        Grid = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        TotalPlayerNumber = lCont.playerCount;
        CurrentTurn = 1;
        CurrentPlayer = 0; //Start with the first player
        
    }

    private void Start()
    {
        players = lCont.players;
        thePlayer = players[CurrentPlayer];
        ResetPrices();
        uiC.UpdateInfoPanel();
        uiC.UpdateScoreTextBox(players);
    }

    public void EndTurn()
    {
        if (CurrentPlayer < TotalPlayerNumber - 1)
        {
            NextPlayer();
            if(lCont.players[CurrentPlayer].GetComponent<Player>().AIplayer)
            {
                Debug.Log("AI player is up!");
                lCont.players[CurrentPlayer].GetComponent<AIController>().RunAITurn();
            }
        }
        else
        {
            NextRound();
            if (lCont.players[CurrentPlayer].GetComponent<Player>().AIplayer)
            {
                lCont.players[CurrentPlayer].GetComponent<AIController>().RunAITurn();
            }
        }
        uiC.UpdateScoreTextBox(players);
    }

    void NextPlayer()
    {
        CurrentPlayer++;
        uiC.UpdateInfoPanel();
    }
    void NextRound()
    {
        AddCash();
        AddInfluence();
        CheckOtherWinConditions();
        CurrentPlayer = 0;
        CurrentTurn++;
        ResetPrices();
        uiC.UpdateInfoPanel();
    }

    void AddCash()
    {
        for(int i = 0; i < TotalPlayerNumber; i++)
        {
            Player CurrentPlayer = players[i];
            //Count of each builing type: [0] empty land, [1] Residential, [2] Industrial, [3] PowerSupply, [4] Civic, [5] landmark
            CurrentPlayer.playerCash += CurrentPlayer.tileCounts[1] * lCont.CashPerRes;
            float CpI = lCont.CashPerInd;
            float CpR = lCont.CashPerRes;
            if (powerToThepeople == true)
            {
                for (int j = 0; j < CurrentPlayer.tileCounts[3]; j++)
                {
                    CpR = (CpR * lCont.PowerPlantMultiplier);
                }
            }
            else
            {
                for (int j = 0; j < CurrentPlayer.tileCounts[3]; j++)
                {
                    CpI = (CpI * lCont.PowerPlantMultiplier);
                }
            }
            if (championOfIndustry == true)
            {
                CpI = CpI * 1.1f;
            }
            if (rentHike == true)
            {
                CpR = CpR * 1.5f;
            }
            if (freeHousingInitiative == true)
            {
                CpR = CpR * 0;
            }
            int newCashPerInd = (int)CpI;
            int newCashPerRes = (int)CpR;
            CurrentPlayer.playerCash += CurrentPlayer.tileCounts[2] * newCashPerInd;
            CurrentPlayer.playerCash += CurrentPlayer.tileCounts[3] * lCont.CashPerPow;
            if(bribesAsIndustry == true && decentraliseGovernment == false)
            {
                CurrentPlayer.playerCash += (int)(CurrentPlayer.tileCounts[4] * lCont.CashPerCiv * 0.8f);
            }
            else if (bribesAsIndustry == false && decentraliseGovernment == false)
            {
                CurrentPlayer.playerCash += CurrentPlayer.tileCounts[4] * lCont.CashPerCiv;
            }
            else if (bribesAsIndustry == true && decentraliseGovernment == true)
            {
                CurrentPlayer.playerCash += (int)(CurrentPlayer.tileCounts[4] * lCont.CashPerCiv * 0.8f * 0.5f);
            }
            else if(bribesAsIndustry == false && decentraliseGovernment == true)
            {
                CurrentPlayer.playerCash += (int)(CurrentPlayer.tileCounts[4] * lCont.CashPerCiv * 0.5f);
            }
            CurrentPlayer.playerCash += CurrentPlayer.tileCounts[5] * lCont.CashPerLan;

            if(CurrentPlayer.playerCash >= lCont.TargetCash && lCont.VictoryCondition == 2)
            {
                if (WinCondition != 1)
                {
                    WinCondition = 1;
                    WinningPlayer = CurrentPlayer;
                }
                else
                {
                    if (CurrentPlayer.playerCash > WinningPlayer.playerCash)
                    {
                        WinningPlayer = CurrentPlayer;
                    }
                }
            }

        }
        if (WinCondition == 1)
        {
            uiC.SpawnWinText(WinningPlayer.PlayerName, WinningPlayer.playerCash);
        }
    }
    void AddInfluence()
    {
        for (int i = 0; i < TotalPlayerNumber; i++)
        {
            Player CurrentPlayer = players[i];
            float IpC = lCont.InfPerCiv;
            float IpR = lCont.InfPerRes;

            if(powerToThepeople == true)
            {
                IpR = IpR * CurrentPlayer.tileCounts[3] * lCont.PowerPlantMultiplier;
            }
            if(rentHike == true)
            {
                IpR = IpR * 0.5f;
            }
            if(freeHousingInitiative == true)
            {
                IpR = IpR * 1.5f; 
            }
            CurrentPlayer.playerInfluence += (int)(CurrentPlayer.tileCounts[1] * IpR);
            CurrentPlayer.playerInfluence += CurrentPlayer.tileCounts[2] * lCont.InfPerInd;
            CurrentPlayer.playerInfluence += CurrentPlayer.tileCounts[3] * lCont.InfPerPow;

            if(ruleOfLaw == true)
            {
                IpC = IpC * 1.1f;
            }
            if(decentraliseGovernment == true)
            {
                IpC = IpC * 0.5f;
            }
            CurrentPlayer.playerInfluence += (int)(CurrentPlayer.tileCounts[4] * IpC);
            
            CurrentPlayer.playerInfluence += CurrentPlayer.tileCounts[5] * lCont.InfPerLan;

            if(CurrentPlayer.playerInfluence >= lCont.TargetInfluence && lCont.VictoryCondition == 0)
            {
                if(WinCondition != 1)
                {
                    WinCondition = 1;
                    WinningPlayer = CurrentPlayer;
                }
                else
                {
                    if(CurrentPlayer.playerInfluence > WinningPlayer.playerInfluence)
                    {
                        WinningPlayer = CurrentPlayer;
                    }
                }
            }
        }
        if(WinCondition == 1)
        {
            uiC.SpawnWinText(WinningPlayer.PlayerName, WinningPlayer.playerInfluence);
        }
    }
    private void ResetPrices()
    {
        Cells = Grid.cells;
        foreach(HexCell cell in Cells)
        {
            if(cell.cellOwner == -1)
            {
                cell.cellPrice = lCont.CellPrices[cell.cellType];
            }
        }
    }

    public int CalculateInfIncrease(int CurrentPlayer)
    {
        Debug.Log("TurnController.cs, line 136:" + CurrentPlayer);
        Player thePlayer = players[CurrentPlayer];
        int calculatedIncrease = thePlayer.tileCounts[1] * lCont.InfPerRes + thePlayer.tileCounts[2] * lCont.InfPerInd + thePlayer.tileCounts[3] * lCont.InfPerPow + thePlayer.tileCounts[4] * lCont.InfPerCiv + thePlayer.tileCounts[5] * lCont.InfPerLan;
        return calculatedIncrease;
    }
    public int CalculateCashIncrease(int CurrentPlayer)
    {
        Player thePlayer = players[CurrentPlayer];
        float CpI = lCont.CashPerInd;
        for (int i = 0; i < thePlayer.tileCounts[3]; i++)
        {
            CpI = (CpI * lCont.PowerPlantMultiplier);
        }
        int newCashPerInd = (int)CpI;
        int calculatedIncrease = thePlayer.tileCounts[1] * lCont.CashPerRes + thePlayer.tileCounts[2] * newCashPerInd + thePlayer.tileCounts[3] * lCont.CashPerPow + thePlayer.tileCounts[4] * lCont.CashPerCiv + thePlayer.tileCounts[5] * lCont.CashPerLan;
        return calculatedIncrease;
    }

    public void CheckOtherWinConditions()
    {
        for (int i = 0; i < TotalPlayerNumber; i++)
        {
            Player CurrentPlayer = players[i];
            int Ncells = CurrentPlayer.tileCounts[0] + CurrentPlayer.tileCounts[1] + CurrentPlayer.tileCounts[2] + CurrentPlayer.tileCounts[3] + CurrentPlayer.tileCounts[4] + CurrentPlayer.tileCounts[5];
            Debug.Log("Checking victory condition:");
            Debug.Log(Ncells);
            Debug.Log(lCont.TargetCells);
            Debug.Log(lCont.VictoryCondition);
            if (Ncells >= lCont.TargetCells && lCont.VictoryCondition == 1)
            {
                if (WinCondition != 1)
                {
                    WinCondition = 1;
                    WinningPlayer = CurrentPlayer;
                }
                else
                {
                    if (CurrentPlayer.playerInfluence > WinningPlayer.playerInfluence)
                    {
                        WinningPlayer = CurrentPlayer;
                    }
                }
            }
            if (CurrentPlayer.tileCounts[5] >= lCont.TargetLandmarks && lCont.VictoryCondition == 3)
            {
                if (WinCondition != 1)
                {
                    WinCondition = 1;
                    WinningPlayer = CurrentPlayer;
                }
                else
                {
                    if (CurrentPlayer.playerInfluence > WinningPlayer.playerInfluence)
                    {
                        WinningPlayer = CurrentPlayer;
                    }
                }
            }
        }
        if (WinCondition == 1)
        {
            uiC.SpawnWinText(WinningPlayer.PlayerName, WinningPlayer.playerInfluence);
        }
    }
}
