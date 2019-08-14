using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

    //Notes to improve game:
    //Need more possible win conditions: Influencer (race to get 2000 influence), Super Majority (This should be the standard game, race to own 66% of the cells), Domination (Long game, own 100% of the board)

    //This will include level specific info in order to generate the map:

    [Range(0, 100)]
    public int Residential;
    [Range(0, 100)]
    public int Industrial;
    [Range(0, 100)]
    public int Power;
    [Range(0, 100)]
    public int Civic;
    [Range(0, 10)]
    public int Landmark;

    public Player[] players; //List of the players

    public List<Color> playerColors = new List<Color>
    {
        Color.green,
        Color.red,
        Color.blue,
        Color.magenta,
        Color.cyan
    };

    [Range(1,4)]
    public int playerCount;
    [Range(1, 4)]
    public int AiPlayerCount;

    [Range(-1000, 1000)]
    public int CashPerPow;//Amount of cash produced/needed by power plant tile
    [Range(-1000, 1000)]
    public int CashPerInd;//Amount of cash produced/needed by industry tile
    [Range(-1000, 1000)]
    public int CashPerRes;//Amount of cash produced/needed by residential tile
    [Range(-1000, 1000)]
    public int CashPerCiv;//Amount of cash produced/needed by civic tile
    [Range(-1000, 1000)]
    public int CashPerLan;//Amount of cash produced/needed by Landmark tile

    [Range(-1000, 1000)]
    public int InfPerPow;//Amount of influence produced/needed by power plant tile
    [Range(-1000, 1000)]
    public int InfPerInd;//Amount of influence produced/needed by industry tile
    [Range(-1000, 1000)]
    public int InfPerRes;//Amount of influence produced/needed by residential tile
    [Range(-1000, 1000)]
    public int InfPerCiv;//Amount of influence produced/needed by civic tile
    [Range(-1000, 1000)]
    public int InfPerLan;//Amount of influence produced/needed by Landmark tile

    [Range(1.0f, 10.0f)]
    public float PowerPlantMultiplier;

    [Range(100, 10000)]
    public int TargetInfluence;

    public int StartingCash = 100;
    public int StartingInfluence = 0;

    public int ResCost = 100;
    public int IndCost = 300;
    public int PowCost = 500;
    public int CivCost = 500;
    public int LanCost = 2000;
    public List<int> CellPrices;
    public List<int> CellCashList;
    public List<int> CellInfList;
    
    public Player PlayerPrefab;

    GlobalController globalController;

    //Aditional lists for players:
    public string[] playerNames;
    public int[] playerTypes;

    private void Awake()
    {
        //First thing to do is to find the Global controller
        globalController = GameObject.Find("GlobalController").GetComponent<GlobalController>();
        if(globalController.useDefaults != true)
        {
            //If the global controller specifies that we do not use the default values stored in the Level controller object, then we must be playing a custom game and need to get those values from the global controller
            Debug.Log("Default options not being used.");
            Residential = globalController.Residential;
            Industrial = globalController.Industrial;
            Power = globalController.Power;
            Civic = globalController.Civic;
            Landmark = globalController.Landmark;
            players = globalController.players;
            playerColors = globalController.playerColors;
            playerCount = globalController.playerCount;
            AiPlayerCount = globalController.AiPlayerCount;
            CashPerPow = globalController.CashPerPow;
            CashPerInd = globalController.CashPerInd;
            CashPerRes = globalController.CashPerRes;
            CashPerCiv = globalController.CashPerCiv;
            CashPerLan = globalController.CashPerLan;
            InfPerPow = globalController.InfPerPow;
            InfPerInd = globalController.InfPerInd;
            InfPerRes = globalController.InfPerRes;
            InfPerCiv = globalController.InfPerCiv;
            InfPerLan = globalController.InfPerLan;
            PowerPlantMultiplier = globalController.PowerPlantMultiplier;
            TargetInfluence = globalController.TargetInfluence;
            StartingCash = globalController.StartingCash;
            StartingInfluence = globalController.StartingInfluence;
            ResCost = globalController.ResCost;
            IndCost = globalController.IndCost;
            PowCost = globalController.PowCost;
            CivCost = globalController.CivCost;
            LanCost = globalController.LanCost;
            playerNames = globalController.playerNames;
            playerTypes = globalController.playerTypes;
        }
        //Move this stuff to start so that it definitely happens after setting the level parameters.
        CellPrices = new List<int>
        {
            50, ResCost, IndCost,PowCost,CivCost,LanCost
        };
        CellCashList = new List<int>
        {
            0, CashPerRes, CashPerInd, CashPerPow, CashPerCiv, CashPerLan
        };
        CellInfList = new List<int>
        {
            0, InfPerRes, InfPerInd, InfPerPow, InfPerCiv, InfPerLan
        };
        if(globalController.useDefaults == true)
        {
            InitPlayers(playerCount);
        }
        else
        {
            //Here I need a method that will instantiate new players from playerNames and player types
            InitPlayersNonDefault(playerCount, playerNames, playerTypes);
        }
    }

    private void Start()
    {
        Debug.Log("Level controller setup complete.");
    }

    void InitPlayers(int playerCount)
    {
        players = new Player[playerCount];
        for (int i = 0; i < (playerCount); i++)
        {
            Player player = players[i] = Instantiate<Player>(PlayerPrefab);
            int j = i + 1;
            players[i].PlayerName = "Player " + j;
            players[i].playerCash = StartingCash;
            players[i].playerInfluence = StartingInfluence;
            players[i].playerNumber = i;
            players[i].PlayerColor = playerColors[i];
            if (i >= playerCount - AiPlayerCount)
            {
                Debug.Log("Instantiating AI player");
                players[i].AIplayer = true;
                players[i].PlayerName = "AI Player " + j;
            }
        }

    }

    void InitPlayersNonDefault(int playerCount, string[] playerNames, int[] playerTypes)
    {
        players = new Player[playerCount];
        for(int i = 0; i < playerCount; i++)
        {
            if(playerTypes[i] != globalController.noPlayerIndex)
            {
                Player player = players[i] = Instantiate<Player>(PlayerPrefab);
                players[i].PlayerName = playerNames[i];
                players[i].playerCash = StartingCash;
                players[i].playerInfluence = StartingInfluence;
                players[i].playerNumber = i;
                players[i].PlayerColor = playerColors[i];
                if (playerTypes[i] == 1)
                {
                    players[i].AIplayer = true;
                }
            }
        }
    }




}
