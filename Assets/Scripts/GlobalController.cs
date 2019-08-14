using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour {

    public int noPlayerIndex = 2;

    public static GlobalController instance;

    //Need to have the level controller settings stored in the global controller because this is the only object that does not get destroyed when a new scene is loaded
    public bool useDefaults = true;
    //Below are all the details that are used to set up the game
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
    [Range(1, 4)]
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
    //Aditional lists for players:
    public string[] playerNames;
    public int[] playerTypes;
    //End of setup list

    void Awake()
    {
        //check if instance exists
        if (instance == null)
        {
            //assign it to the current object:
            instance = this;
        }
        //make sure instance is the current object
        else if (instance != this)
        {
            //destroy the current game object
            Destroy(gameObject);
        }
        //don't destroy on changing scene
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
		
	}

    public void updateResidentialValue()
    {
        Residential =(int) GameObject.Find("ResidentialSlider").GetComponent<Slider>().value;
    }
    public void updateIndustrialValue()
    {
        Industrial = (int)GameObject.Find("IndustrialSlider").GetComponent<Slider>().value;
    }
    public void updateCivicValue()
    {
        Civic = (int)GameObject.Find("CivicSlider").GetComponent<Slider>().value;
    }
    public void updatePowerPlantValue()
    {
        Power = (int)GameObject.Find("PowerSlider").GetComponent<Slider>().value;
    }
    public void updateLandmarkValue()
    {
        Landmark = (int)GameObject.Find("LandmarkSlider").GetComponent<Slider>().value;
    }

    public void updatePlayerList()
    {
        playerCount = getPlayerCount();
        playerNames = new string[playerCount];
        playerTypes = new int[playerCount];
        int j = 0;
        for(int i = 0; i < playerCount; i++)
        {
            bool Found = false;
            while (Found == false)
            {
                int pnum = j + 1;
                string typename = "PlayerType" + pnum;
                int ptype = GameObject.Find(typename).GetComponent<Dropdown>().value;
                if (ptype != noPlayerIndex)
                {
                    string textboxName = "Player" + pnum + "Field";
                    playerNames[i] = GameObject.Find(textboxName).GetComponent<InputField>().text;
                    playerTypes[i] = ptype;
                    Found = true;
                    Debug.Log(textboxName);
                    j++;
                }
                else
                {
                    j++;
                }
            }
            
            
        }
    }
    int getPlayerCount()
    {
        int count = 0;
        int curr = GameObject.Find("PlayerType1").GetComponent<Dropdown>().value;
        if (curr != noPlayerIndex) count++;
        curr = GameObject.Find("PlayerType2").GetComponent<Dropdown>().value;
        if (curr != noPlayerIndex) count++;
        curr = GameObject.Find("PlayerType3").GetComponent<Dropdown>().value;
        if (curr != noPlayerIndex) count++;
        curr = GameObject.Find("PlayerType4").GetComponent<Dropdown>().value;
        if (curr != noPlayerIndex) count++;
        return count;
    }
    }
