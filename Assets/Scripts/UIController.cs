using System.Collections;
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
    Text scoreBoxHeaderText;

    [SerializeField]
    GameObject gameOverPrefab;

    [SerializeField]
    Text messageBox;
    string[] messageString = new string[12];

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
        if(lCont.VictoryCondition == 2)
        {
            PlayerCashText.text = string.Format("Cash:${0} (+${1}) / {2}", CurrentPlayer.playerCash, CashIncrease, lCont.TargetCash);
        }
        else
        {
            PlayerCashText.text = string.Format("Cash: ${0} (+${1})", CurrentPlayer.playerCash, CashIncrease);
        }
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
        if(cell.cellType == 3)
        {
            if(turnController.powerToThepeople == true)
            {
                CellInfoText.text = string.Format("{0} \n \n Value: {1} \n \n Owner: \n {2} \n \n Res $ multiplier: {3}", cell.cellTypeString, cell.cellPrice, cell.cellOwnerString, lCont.PowerPlantMultiplier);
            }
            else
            {
                CellInfoText.text = string.Format("{0} \n \n Value: {1} \n \n Owner: \n {2} \n \n Ind $ multiplier: {3}", cell.cellTypeString, cell.cellPrice, cell.cellOwnerString, lCont.PowerPlantMultiplier);
            }
            
        }
        else
        {
            CellInfoText.text = string.Format("{0} \n \n Value: {1} \n \n Owner: \n {2} \n \n $/Inf: {3}/{4}", cell.cellTypeString, cell.cellPrice, cell.cellOwnerString, lCont.CellCashList[cell.cellType], lCont.CellInfList[cell.cellType]);
        }
    }
        

    public void OnBuyButtonPress()
    {
        if(CurrentPlayer.AIplayer != true)
        {
            GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
            if (instance != null)
            {
                Debug.Log("pointer cleanup on enter");
                GameObject.Destroy(instance);
            }
            else
            {
                HexCell cell = gridController.cells[gridController.SelectedCellIndex];
                actionController.OnBuyCell(CurrentPlayer, cell);
            }
        }
        
    }

    public void OnSellButtonPress()
    {
        if (CurrentPlayer.AIplayer != true)
        {
            GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
            if (instance != null)
            {
                Debug.Log("pointer cleanup on enter");
                GameObject.Destroy(instance);
            }
            else
            {
                HexCell cell = gridController.cells[gridController.SelectedCellIndex];
                actionController.OnSellCell(CurrentPlayer, cell);
            }
            
        }
    }

    public void OnLobbyButtonPress()
    {
        if (CurrentPlayer.AIplayer != true)
        {
            GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
            if (instance != null)
            {
                Debug.Log("pointer cleanup on enter");
                GameObject.Destroy(instance);
            }
            else
            {
                GameObject LobbyMenu = Instantiate<GameObject>(LobbyMenuPrefab);
                LobbyMenu.transform.SetParent(transform, false);
            }
            
        }
    }

    public void OnMenuButtonPress()
    {
        GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
        if (instance != null)
        {
            Debug.Log("pointer cleanup on enter");
            GameObject.Destroy(instance);
        }
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
            GameObject instance = GameObject.FindGameObjectWithTag("mouseOverText");
            if (instance != null)
            {
                Debug.Log("pointer cleanup on enter");
                GameObject.Destroy(instance);
            }
            else
            {
                turnController.EndTurn();
                gridController.DeselectCell();
            }
            
        }
    }

    public void SpawnWinText(string playername, int playerscore)
    {
        GameObject winnerAnnounce = Instantiate<GameObject>(gameOverPrefab);
        Text WinText = winnerAnnounce.transform.Find("gameOverText").GetComponent<Text>();
        winnerAnnounce.GetComponent<RectTransform>().SetParent(this.transform, false);
        winnerAnnounce.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        WinText.text = string.Format("Game Over \n {0} wins! \n Winning score: {1}", playername, playerscore);
        turnController.paused = true;
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
        int winCondition = lCont.VictoryCondition;
        string headerOutputString = "";
        if(winCondition == 0)
        {
            headerOutputString = "Gain 500 influence\nScores:\n";
        }
        else if(winCondition == 1)
        {
            headerOutputString = "Control 30 districts\nScores:\n";
        }
        else if (winCondition == 2)
        {
            headerOutputString = "Obtain $10000\nScores:\n";
        }
        else if (winCondition == 3)
        {
            headerOutputString = "Hold 3 landmarks\nScores:\n";
        }
        string outputString = "";
        foreach(Player thePlayer in thePlayers)
        {
            int playerScore = thePlayer.playerInfluence;
            if (winCondition == 0)
            {
                playerScore = thePlayer.playerInfluence;
            }
            else if (winCondition == 1)
            {
                playerScore = thePlayer.tileCounts[1] + thePlayer.tileCounts[2] + thePlayer.tileCounts[3] + thePlayer.tileCounts[4] + thePlayer.tileCounts[5];
            }
            else if (winCondition == 2)
            {
                playerScore = thePlayer.playerCash;
            }
            else if(winCondition == 3)
            {
                playerScore = thePlayer.tileCounts[5];
            }
            else
            {
                playerScore = thePlayer.playerInfluence;
            }
            outputString = outputString + string.Format("{0}: {1}\n", thePlayer.PlayerName, playerScore);
        }
        scoreBoxHeaderText.text = headerOutputString;
        ScoreBoxScoreText.text = outputString;
    }

    public void updateMessageBox(string PlayerName, string Action)
    {
        string outstring = PlayerName + Action;
        for(int  i=0; i < (messageString.Length - 1); i++)
        {
            messageString[i] = messageString[i + 1];
        }
        messageString[messageString.Length - 1] = outstring;
        string boxString = "";
        for(int i=0; i< messageString.Length; i++)
        {
            boxString = boxString + messageString[i] + "\n";
        }
        messageBox.text = boxString;
    }

}
