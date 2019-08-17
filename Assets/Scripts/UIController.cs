﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    Text PlayerNameText;
    [SerializeField]
    Text PlayerCashText;
    [SerializeField]
    Text PlayerInfluenceText;
    [SerializeField]
    Text CellInfoText;

    Player CurrentPlayer;
    LevelController lCont;
    TurnController turnController;

    ActionController actionController;

    HexGrid gridController;
    [SerializeField]
    Text WinTextPrefab;

    [SerializeField]
    GameObject LobbyMenuPrefab;
    [SerializeField]
    GameObject EscapeMenuPrefab;

    [SerializeField]
    GameObject gameController;
    GameController gameControl;

    [SerializeField]
    Text ScoreBoxScoreText;

    [SerializeField]
    GameObject gameOverPrefab;

    private void Awake()
    {
        lCont = GameObject.Find("LevelController").GetComponent<LevelController>();
        turnController = GameObject.Find("GameController").GetComponent<TurnController>();
        actionController = GameObject.Find("GameController").GetComponent<ActionController>();
        gridController = GameObject.Find("HexGrid").GetComponent<HexGrid>();
        gameControl = gameController.GetComponent<GameController>();
    }

    private void Start()
    {
        int playerindex = turnController.CurrentPlayer;
        CurrentPlayer = lCont.players[playerindex];
        UpdateInfoPanel();
    }

    private void Update()
    {
        //In Update I will go to a menu if Esc is pressed.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
             print("Esc key was pressed");
             OnMenuButtonPress();
        }
    }

    public void UpdateInfoPanel()
    {
        int CurrentTurn = turnController.CurrentTurn;
        int playerindex = turnController.CurrentPlayer;
        int InfIncrease = turnController.CalculateInfIncrease(playerindex);
        int CashIncrease = turnController.CalculateCashIncrease(playerindex);
        CurrentPlayer = lCont.players[playerindex];
        PlayerNameText.text = string.Format("Turn {0} {1}", CurrentTurn, CurrentPlayer.PlayerName);
        PlayerCashText.text = string.Format("Cash: ${0} (+${1})",CurrentPlayer.playerCash,CashIncrease);
        if(lCont.VictoryCondition == 0)
        {
            PlayerInfluenceText.text = string.Format("Influence: {0} (+{2})/{1}", CurrentPlayer.playerInfluence, lCont.TargetInfluence, InfIncrease);
        }
        else
        {
            PlayerInfluenceText.text = string.Format("Influence: {0} (+{1})", CurrentPlayer.playerInfluence, InfIncrease);
        }
        
    }

    public void UpdateCellPanel(HexCell cell)
    {
        CellInfoText.text = string.Format("{0} \n \n Value: {1} \n \n Owner: \n {2} \n \n $/Inf: {3}/{4}", cell.cellTypeString, cell.cellPrice, cell.cellOwnerString,lCont.CellCashList[cell.cellType], lCont.CellInfList[cell.cellType]);
    }

    public void OnBuyButtonPress()
    {
        if(CurrentPlayer.AIplayer != true)
        {
            HexCell cell = gridController.cells[gridController.SelectedCellIndex];
            actionController.OnBuyCell(CurrentPlayer, cell);
        }
        
    }

    public void OnSellButtonPress()
    {
        if (CurrentPlayer.AIplayer != true)
        {
            HexCell cell = gridController.cells[gridController.SelectedCellIndex];
            actionController.OnSellCell(CurrentPlayer, cell);
        }
    }

    public void OnLobbyButtonPress()
    {
        if (CurrentPlayer.AIplayer != true)
        {
            GameObject LobbyMenu = Instantiate<GameObject>(LobbyMenuPrefab);
            LobbyMenu.transform.SetParent(transform, false);
        }
    }

    public void OnMenuButtonPress()
    {
        GameObject Escmen = GameObject.Find("MenuOnEsc(Clone)");
        if (Escmen != null)
        {
            GameObject.Destroy(Escmen);
        }
        else
        {
            GameObject EscMenu = Instantiate<GameObject>(EscapeMenuPrefab);
            EscMenu.transform.SetParent(transform, false);
        }
    }

    public void OnEndTurnButtonPress()
    {
        if (CurrentPlayer.AIplayer != true)
        {
            turnController.EndTurn();
            gridController.DeselectCell();
        }
    }

    public void SpawnWinText(string playername, int playerscore)
    {
        GameObject winnerAnnounce = Instantiate<GameObject>(gameOverPrefab);
        Text WinText = winnerAnnounce.transform.Find("gameOverText").GetComponent<Text>();
        winnerAnnounce.GetComponent<RectTransform>().SetParent(this.transform, false);
        winnerAnnounce.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        WinText.text = string.Format("Game Over \n {0} wins! \n Winning score: {1}", playername, playerscore);
    }

    public void OnExitToOSPress()
    {
        Application.Quit();
    }

    public void OnExitToMainMenuPress()
    {
        gameControl.GotToMainMenu();
    }

    public void UpdateScoreTextBox(Player[] thePlayers)
    {
        string outputString = "";
        foreach(Player thePlayer in thePlayers)
        {
            int playerScore = thePlayer.playerInfluence;
            outputString = outputString + string.Format("{0}: {1}\n", thePlayer.PlayerName, playerScore);
        }
        ScoreBoxScoreText.text = outputString;
    }

}
